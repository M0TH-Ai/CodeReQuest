// Copyright (C) 2014-2016 Silicon Studio Corp. (http://co.jp)
// This file is distributed as part of the Xenko Game Studio Samples
// Detailed license can be found at: http://xenko.com/legal/eula/

using System;
using Xenko.Core;
using Xenko.Core.Mathematics;
using Xenko.Engine;
using Xenko.Engine.Events;
using Xenko.Physics;
using ThirdPersonPlatformer.Animation_Controller;
using Xenko.Animations;

namespace ThirdPersonPlatformer.Player
{
    public class PlayerController : SyncScript
    {
        [Display("Run Speed")]
        public float MaxRunSpeed { get; set; } = 10;

        public static readonly EventKey<bool> IsGroundedEventKey = new EventKey<bool>();

        public static readonly EventKey<float> RunSpeedEventKey = new EventKey<float>();

        // The PlayerController will propagate if it is attacking to the AnimationController
        public static readonly EventKey<bool> IsAttackingEventKey = new EventKey<bool>();

        public static readonly EventKey<bool> IsAttackingEventKey2 = new EventKey<bool>();

        public static readonly EventKey<bool> IsAttackingQuickEventKey = new EventKey<bool>();

        public static readonly EventKey<bool> IsAttackIdleEventKey = new EventKey<bool>();
        public AnimationController animator { get; set; }

        [Display("Attack Distance")]
        public float AttackDistance { get; set; } = 1f;

        /// <summary>
        /// Cooldown in seconds required for the character to recover from starting an attack until it can choose another action
        /// </summary>
        [Display("Attack Cooldown")]
        public float AttackCooldown { get; set; } = 0.65f;

        // This component is the physics representation of a controllable character
        private CharacterComponent character;
        private Entity modelChildEntity;
        public Entity sword;

        private float yawOrientation;

        private readonly EventReceiver<Vector3> moveDirectionEvent = new EventReceiver<Vector3>(PlayerInput.MoveDirectionEventKey);

        private readonly EventReceiver<bool> jumpEvent = new EventReceiver<bool>(PlayerInput.JumpEventKey);

        private Entity attackEntity = null;
        private float attackCooldown = 0f;
        public float MaxShootDistance { get; set; } = 2f;
        public float ShootImpulse { get; set; } = 5f;

        public WeaponUIscript weaponUI;
        public PlayerInput inputSystem;

        /// <summary>
        /// Allow for some latency from the user input to make jumping appear more natural
        /// </summary>
        [Display("Jump Time Limit")]
        public float JumpReactionThreshold { get; set; } = 0.3f;

        // When the character falls off a surface, allow for some reaction time
        private float jumpReactionRemaining;

        // Allow some inertia to the movement
        private Vector3 moveDirection = Vector3.Zero;

        // Attacking
        [Display("Punch Collision")]
        public RigidbodyComponent PunchCollision { get; set; }

        public bool attacking;

        public bool quickattack;

        public Quaternion runningRotation;

        public Entity player;

        public Quaternion idleRotation;


        /// <summary>
        /// Called when the script is first initialized
        /// </summary>
        public override void Start()
        {
            base.Start();

            //this.GetSimulation().ColliderShapesRendering = true;

            jumpReactionRemaining = JumpReactionThreshold;

            // Will search for an CharacterComponent within the same entity as this script
            character = Entity.Get<CharacterComponent>();
            if (character == null) throw new ArgumentException("Please add a CharacterComponent to the entity containing PlayerController!");

            modelChildEntity = Entity.GetChild(0);

            PunchCollision.Enabled = false;
            sword.Get<ModelNodeLinkComponent>().NodeName = "Hips";
            sword.Get<TransformComponent>().Position = new Vector3(-0.026f, 0, -0.147f);
            sword.Transform.Scale = new Vector3(1f, 1f, 1f);
        }

        /// <summary>
        /// Called on every frame update
        /// </summary>
        public override void Update()
        {
            // var dt = Game.UpdateTime.Elapsed.Milliseconds * 0.001;
            var dt = (float)Game.UpdateTime.Elapsed.Seconds;
            Move(MaxRunSpeed);

            weaponUI.currentWeapon = sword;

            Jump();

            animator.attackIdleOn = attacking;

            if (attacking == true && inputSystem.moving == true)
            {
                sword.Get<TransformComponent>().Rotation = runningRotation;
                sword.Get<TransformComponent>().Position = new Vector3(-0.250f, 0.085f, 0.027f);
            }
            else
            {
                sword.Get<TransformComponent>().Rotation = idleRotation;
                sword.Get<TransformComponent>().Position = new Vector3(0.316f, 0.085f, 0.027f);
            }

            if (attacking == false)
            {
                PunchCollision.Enabled = false;
                //Input.Enabled = true;
                sword.Get<ModelNodeLinkComponent>().NodeName = "Hips";
                sword.Get<TransformComponent>().Position = new Vector3(-0.026f, 0, -0.147f);
                sword.Transform.Scale = new Vector3(1f, 1f, 1f);
            }
        }

        /// <summary>
        /// Jump makes the character jump and also accounts for the player's reaction time, making jumping feel more natural by
        /// allowing jumps within some limit of the last time the character was on the ground
        /// </summary>
        private void Jump()
        {
            var dt = this.GetSimulation().FixedTimeStep;



            // Check if conditions allow the character to jump
            if (JumpReactionThreshold <= 0)
            {
                // No reaction threshold. The character can only jump if grounded
                if (!character.IsGrounded)
                {
                    IsGroundedEventKey.Broadcast(false);
                    return;
                }
            }
            else
            {
                // If there is still enough time left for jumping allow the character to jump even when not grounded
                if (jumpReactionRemaining > 0)
                    jumpReactionRemaining -= dt;

                // If the character on the ground reset the jumping reaction time
                if (character.IsGrounded)
                    jumpReactionRemaining = JumpReactionThreshold;

                // If there is no more reaction time left don't allow the character to jump
                if (jumpReactionRemaining <= 0)
                {
                    IsGroundedEventKey.Broadcast(character.IsGrounded);
                    return;
                }
            }

            // If the player didn't press a jump button we don't need to jump
            jumpEvent.TryReceive(out bool didJump);
            if (!didJump)
            {
                IsGroundedEventKey.Broadcast(true);
                return;
            }

            // Jump!!
            jumpReactionRemaining = 0;
            character.Jump();

            // Broadcast that the character is jumping!
            IsGroundedEventKey.Broadcast(false);
        }

        private void Move(float speed)
        {
            // Character speed
            moveDirectionEvent.TryReceive(out Vector3 newMoveDirection);

            // Allow very simple inertia to the character to make animation transitions more fluid
            moveDirection = moveDirection * 0.85f + newMoveDirection * 0.15f;

            character.SetVelocity(moveDirection * speed);

            // Broadcast speed as per cent of the max speed
            RunSpeedEventKey.Broadcast(moveDirection.Length());

            // Character orientation
            if (moveDirection.Length() > 0.001)
            {
                yawOrientation = MathUtil.RadiansToDegrees((float)Math.Atan2(-moveDirection.Z, moveDirection.X) + MathUtil.PiOverTwo);
            }
            modelChildEntity.Transform.Rotation = Quaternion.RotationYawPitchRoll(MathUtil.DegreesToRadians(yawOrientation), 0, 0);
        }

        public void Attack()
        {
            var dt = (float)Game.UpdateTime.Elapsed.TotalSeconds;
            attackCooldown = (attackCooldown > 0) ? attackCooldown - dt : 0f;

            //PunchCollision.Enabled = (attackCooldown > 0);

            if (attacking == true && quickattack == false)
            {
                quickattack = true;
                IsAttackingEventKey.Broadcast(true);
            }
            else
            {
                QuickAttack();
            }
        }

        public void QuickAttack()
        {
            var dt = (float)Game.UpdateTime.Elapsed.TotalSeconds;
            attackCooldown = (attackCooldown > 0) ? attackCooldown - dt : 0f;
            //PunchCollision.Enabled = (attackCooldown > 0);

            if (quickattack == true)
            {
                IsAttackingQuickEventKey.Broadcast(true);
                MoveForwards();
                quickattack = false;
            }

        }

        public void SetAttack()
        {
            if (attacking == true)
            {
                attacking = false;
                IsAttackIdleEventKey.Broadcast(false);
                animator.state = AnimationController.AnimationState.Walking;
                animator.currentTime = 0;
                animator.UpdateWalking();
                PunchCollision.Enabled = false;
            }
            else
            {
                PunchCollision.Enabled = true;
                attacking = true;
                IsAttackIdleEventKey.Broadcast(true);
                animator.attackIdleOn = true;
                sword.Get<ModelNodeLinkComponent>().NodeName = "RightHand";
                sword.Get<TransformComponent>().Position = new Vector3(0.395f, 0.085f, 0.027f);
            }
        }

        public void MoveForwards()
        {
            player.Transform.Position += new Vector3(0, 5, 0);
        }
    }
}

