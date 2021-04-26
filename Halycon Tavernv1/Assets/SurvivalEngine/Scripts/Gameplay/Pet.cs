using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalEngine
{
    public enum PetState
    {
        Idle=0,
        Follow = 2,
        Attack = 5,
        Dig = 8,
        Pet = 10,
        MoveTo=15,
        Dead = 20,
    }

    /// <summary>
    /// Pet behavior script for following player, attacking enemies, and digging
    /// </summary>
    
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(Destructible))]
    [RequireComponent(typeof(Character))]
    public class Pet : MonoBehaviour
    {
        [Header("Actions")]
        public float follow_range = 3f;
        public float detect_range = 5f;
        public float action_duration = 10f;
        public bool can_attack = false;
        public bool can_dig = false;

        public UnityAction onAttack;
        public UnityAction onDamaged;
        public UnityAction onDeath;
        public UnityAction onPet;

        private Character character;
        private Destructible destruct;

        private PetState state;
        private Vector3 start_pos;
        private Animator animator;

        private PlayerCharacter player_target = null;
        private Destructible attack_target = null;
        private GameObject action_target = null;

        private float state_timer = 0f;
        private bool force_action = false;

        void Awake()
        {
            character = GetComponent<Character>();
            destruct = GetComponent<Destructible>();
            animator = GetComponentInChildren<Animator>();
            start_pos = transform.position;

            character.onAttack += OnAttack;
            destruct.onDamaged += OnTakeDamage;
            destruct.onDeath += OnKill;

            transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }

        private void Update()
        {
            if (TheGame.Get().IsPaused())
                return;

            if (state == PetState.Dead)
                return;

            state_timer += Time.deltaTime;

            //States
            if (state == PetState.Idle)
            {
                if (state_timer > 2f)
                {
                    if (HasFollowTarget())
                    {
                        ChangeState(PetState.Follow);
                    }
                    else
                    {
                        DetectPlayer();
                    }
                }
            }

            if (state == PetState.Follow)
            {
                if(!IsMoving() && PlayerIsFar(follow_range))
                    character.Follow(player_target.gameObject);

                DetectAction();
            }

            if (state == PetState.Dig)
            {
                if (action_target == null)
                {
                    StopAction();
                    return;
                }

                Vector3 dir = action_target.transform.position - transform.position;
                if (dir.magnitude < 1f)
                {
                    character.Stop();
                    character.FaceTorward(action_target.transform.position);

                    if (animator != null)
                        animator.SetTrigger("Dig");
                    StartCoroutine(DigRoutine());
                }

                if (state_timer > 10f)
                {
                    StopAction();
                }
            }

            if (state == PetState.Attack)
            {
                if (attack_target == null || attack_target.IsDead())
                {
                    StopAction();
                    return;
                }

                Vector3 targ_dir = attack_target.transform.position - transform.position;
                if (!force_action && state_timer > action_duration)
                {
                    if (targ_dir.magnitude > detect_range || PlayerIsFar(detect_range * 2f))
                    {
                        StopAction();
                    }
                }

                if (targ_dir.y > 10f)
                    StopAction(); //Bird too high
            }

            if (state == PetState.Pet)
            {
                if (state_timer > 2f)
                {
                    if (HasFollowTarget())
                        ChangeState(PetState.Follow);
                    else
                        ChangeState(PetState.Idle);
                }
            }

            if (state == PetState.MoveTo)
            {
                if (character.HasReachedMoveTarget())
                    StopAction();
            }

            if (animator != null)
            {
                animator.SetBool("Move", IsMoving());
            }
        }

        private IEnumerator DigRoutine()
        {
            yield return new WaitForSeconds(1f);

            if (action_target != null)
            {
                DigSpot dig = action_target.GetComponent<DigSpot>();
                if (dig != null)
                    dig.Dig();
            }

            StopAction();
        }

        //Detect if the player is in vision
        private void DetectPlayer()
        {
            foreach (PlayerCharacter player in PlayerCharacter.GetAll())
            {
                Vector3 char_dir = (player.transform.position - transform.position);
                if (char_dir.magnitude < detect_range)
                {
                    StopAction();
                    player_target = player;
                    ChangeState(PetState.Follow);
                }
            }
        }

        private void DetectAction()
        {
            if (PlayerIsFar(detect_range))
                return;

            foreach (Selectable selectable in Selectable.GetAllActive())
            {
                if (selectable.gameObject != gameObject)
                {
                    Vector3 dir = (selectable.transform.position - transform.position);
                    if (dir.magnitude < detect_range)
                    {
                        DigSpot dig = selectable.GetComponent<DigSpot>();
                        Destructible destruct = selectable.GetComponent<Destructible>();

                        if (can_attack && destruct && destruct.attack_group == AttackGroup.Enemy && destruct.required_item == null)
                        {
                            attack_target = destruct;
                            action_target = null;
                            character.Attack(destruct);
                            ChangeState(PetState.Attack);
                            return;
                        }

                        else if (can_dig && dig != null)
                        {
                            attack_target = null;
                            action_target = dig.gameObject;
                            ChangeState(PetState.Dig);
                            character.MoveTo(dig.transform.position);
                            return;
                        }
                    }
                }
            }
        }

        public void PetPet(PlayerCharacter character)
        {
            StopAction();
            player_target = character;
            attack_target = null;
            action_target = null;
            ChangeState(PetState.Pet);
            if (animator != null)
                animator.SetTrigger("Pet");
        }

        public void AttackTarget(Destructible target)
        {
            if (target != null)
            {
                attack_target = target;
                action_target = null;
                force_action = true;
                character.Attack(target);
                ChangeState(PetState.Attack);
            }
        }

        public void MoveToTarget(Vector3 pos)
        {
            force_action = true;
            attack_target = null;
            action_target = null;
            ChangeState(PetState.MoveTo);
            character.MoveTo(pos);
        }

        public void StopAction()
        {
            character.Stop();
            attack_target = null;
            action_target = null;
            force_action = false;
            ChangeState(PetState.Idle);
        }

        public void ChangeState(PetState state)
        {
            this.state = state;
            state_timer = 0f;
        }

        private void OnAttack()
        {
            if (animator != null)
                animator.SetTrigger("Attack");

            if (onAttack != null)
                onAttack.Invoke();
        }

        private void OnTakeDamage()
        {
            if (IsDead())
                return;

            if (onDamaged != null)
                onDamaged.Invoke();
        }

        private void OnKill()
        {
            state = PetState.Dead;

            if (animator != null)
                animator.SetTrigger("Death");

            if (onDeath != null)
                onDeath.Invoke();
        }

        public bool HasFollowTarget()
        {
            return player_target != null;
        }

        public bool PlayerIsFar(float distance)
        {
            if (HasFollowTarget())
            {
                Vector3 dir = player_target.transform.position - transform.position;
                return dir.magnitude > distance;
            }
            return false;
        }

        public bool IsDead()
        {
            return character.IsDead();
        }

        public bool IsMoving()
        {
            return character.IsMoving();
        }

        public string GetUID()
        {
            return character.GetUID();
        }
    }

}
