using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalEngine
{
    /// <summary>
    /// Open a package with more items inside (like a gift, box)
    /// </summary>
    

    [CreateAssetMenu(fileName = "Action", menuName = "SurvivalEngine/Actions/Open", order = 50)]
    public class ActionOpen : SAction
    {
        public ItemData[] items;

        public override void DoAction(PlayerCharacter character, ItemSlot slot)
        {
            InventoryData inventory = slot.GetInventory();
            inventory.RemoveItemAt(slot.index, 1);
            foreach (ItemData item in items)
            {
                if (item != null)
                {
                    character.Inventory.GainItem(item, 1);
                }
            }

        }

        public override bool CanDoAction(PlayerCharacter character, ItemSlot slot)
        {
            return true;
        }
    }

}