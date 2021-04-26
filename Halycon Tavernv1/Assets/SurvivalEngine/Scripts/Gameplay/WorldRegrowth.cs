using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalEngine
{
    public enum WorldRegrowthType
    {
        OnDeath = 0,
        OnCreate = 10,
    }

    /// <summary>
    /// Attach this to a Item or Plant or Character to have it regrow automatically on death
    /// </summary>

    [RequireComponent(typeof(Craftable))]
    [RequireComponent(typeof(UniqueID))]
    public class WorldRegrowth : MonoBehaviour
    {
        public WorldRegrowthType type; //When will it try to spawn a new one? on death or when created?
        public float range = 10f; //Range around original object where it can regrow
        public int max = 5; //Wont regrow if already this amount of same object in range
        public float probability = 0.5f; //Probability to regrow 1 on death
        public float duration = 48f; //Duration in in-game hours, between death and regrowth
        public LayerMask valid_floor = 1 << 9; //Floor on which it can grow
        public bool random_rotation = false; //If true, Y axis will be rotated at random
        public bool random_scale = false; //If true, scale will be resized by a value between -0.25 and +0.25

        private UniqueID unique_id;
        private Craftable craftable;
        private Destructible destruct; //Can be null
        private Item item; //Can be null

        private void Awake()
        {
            unique_id = GetComponent<UniqueID>();
            craftable = GetComponent<Craftable>();
            destruct = GetComponent<Destructible>();
            item = GetComponent<Item>();

            if (type == WorldRegrowthType.OnDeath)
            {
                if (destruct != null)
                    destruct.onDeath += SpawnRegrow;
                if (item != null)
                    item.onDestroy += SpawnRegrow;
            }
        }

        private void Start()
        {
            if (type == WorldRegrowthType.OnCreate)
            {
                SpawnRegrow();
            }
        }

        private void SpawnRegrow() {

            CraftData data = craftable.GetData();
            if (data != null && !string.IsNullOrEmpty(unique_id.unique_id) && !PlayerData.Get().HasWorldRegrowth(unique_id.unique_id))
            {
                int nb = Craftable.CountObjectInRadius(data, transform.position, range);
                if (nb < max)
                {
                    //Find position
                    Vector3 position = FindPosition();
                    if (IsPositionValid(position))
                    {
                        Quaternion rotation = transform.rotation;
                        float scale = 1f;
                        if (random_rotation)
                            rotation = Quaternion.Euler(rotation.eulerAngles.x, Random.Range(0f, 360f), 0f);
                        if (random_scale)
                            scale = Random.Range(0.75f, 1.25f);

                        CreateRegrowthData(unique_id.unique_id, data, SceneNav.GetCurrentScene(), position, rotation, scale, duration, probability);
                    }
                }
            }
        }

        private Vector3 FindPosition()
        {
            int nbtry = 0;
            bool valid = false;
            Vector3 position = transform.position;
            while (!valid && nbtry < 10) { //Try find a valid position 10 times
                position = transform.position;
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float radius = Random.Range(0f, range);
                position += new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
                position.y = FindYPosition(position);
                valid = IsPositionValid(position);
                nbtry++;
            }
            return position;
        }

        private float FindYPosition(Vector3 pos)
        {
            Vector3 center = pos + Vector3.up * 10f;
            Vector3 ground_pos;
            bool found = PhysicsTool.FindGroundPosition(center, 20f, valid_floor, out ground_pos);
            return found ? ground_pos.y : pos.y;
        }

        private bool IsPositionValid(Vector3 pos)
        {
            Vector3 center = pos + Vector3.up * 0.5f;
            Vector3 ground_pos;
            return PhysicsTool.FindGroundPosition(center, 1f, valid_floor, out ground_pos);
        }

        //Spawn the prefab from existin regrowth data, after its timer reaches the duration
        public static GameObject SpawnRegrowth(WorldRegrowthData data)
        {
            CraftData craftable = CraftData.Get(data.data_id);
            if (craftable != null && data.scene == SceneNav.GetCurrentScene())
            {
                if (Random.value < data.probability)
                {
                    GameObject nobj = Craftable.Create(craftable, data.pos);
                    nobj.transform.rotation = data.rot;
                    nobj.transform.localScale = nobj.transform.localScale * data.scale;
                    return nobj;
                }
            }
            return null;
        }

        //Create regrowth data after an object dies
        public static WorldRegrowthData CreateRegrowthData(string uid, CraftData item, string scene, Vector3 pos, Quaternion rot, float scale, float duration, float probability)
        {
            WorldRegrowthData data = new WorldRegrowthData();
            data.data_id = item.id;
            data.uid = uid;
            data.scene = scene;
            data.pos = pos;
            data.rot = rot;
            data.scale = scale;
            data.time = duration;
            data.probability = probability;
            PlayerData.Get().AddWorldRegrowth(uid, data);
            return data;
        }
    }

}
