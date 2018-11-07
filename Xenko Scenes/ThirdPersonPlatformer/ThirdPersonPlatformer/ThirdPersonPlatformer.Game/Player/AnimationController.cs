// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.co.jp)
// See LICENSE.md for full license information.
using System;
using Xenko.Core;
using Xenko.Core.Annotations;
using Xenko.Core.Collections;
using Xenko.Core.Mathematics;
using Xenko.Animations;
using Xenko.Engine;
using Xenko.Engine.Events;

namespace ThirdPersonPlatformer.Player
{
    public class AnimationController : SyncScript, IBlendTreeBuilder
    {
        [Display("Animation Component")]
        public AnimationComponent AnimationComponent { get; set; }

        [Display("Idle")]
        public AnimationClip AnimationIdle { get; set; }

        [Display("Walk")]
        public AnimationClip AnimationWalk { get; set; }

        [Display("Attack Walk")]
        public AnimationClip AnimationAWalk { get; set; }

        [Display("Run")]
        public AnimationClip AnimationRun { get; set; }

        [Display("Attack Run")]
        public AnimationClip AnimationARun { get; set; }

        [Display("Jump")]
        public AnimationClip AnimationJumpStart { get; set; }

        [Display("Airborne")]
        public AnimationClip AnimationJumpMid { get; set; }

        [Display("Landing")]
        public AnimationClip AnimationJumpEnd { get; set; }

        [DataMemberRange(0, 1, 0.01, 0.1, 3)]
        [Display("Walk Threshold")]
        public float WalkThreshold { get; set; } = 0.25f;

        [DataMemberRange(0, 1, 0.01, 0.1, 3)]
        [Display("Attack Walk Threshold")]
        public float AWalkThreshold { get; set; } = 0.25f;

        [Display("Punch")]
        public AnimationClip AnimationPunch { get; set; }

        [Display("Punch2")]
        public AnimationClip AnimationPunch2 { get; set; }

        [Display("Quick Attack")]
        public AnimationClip AnimationQuickAttack { get; set; }

        [Display("Attack Idle")]
        public AnimationClip AnimationAIdle { get; set; }

        [Display("Time Scale")]
        public double TimeFactor { get; set; } = 1;

        public bool attackIdleOn;

        public PlayerInput input;

        private AnimationClipEvaluator animEvaluatorIdle;
        private AnimationClipEvaluator animEvaluatorWalk;
        private AnimationClipEvaluator animEvaluatorRun;
        private AnimationClipEvaluator animEvaluatorJumpStart;
        private AnimationClipEvaluator animEvaluatorJumpMid;
        private AnimationClipEvaluator animEvaluatorJumpEnd;
        private AnimationClipEvaluator animEvaluatorPunch;
        private AnimationClipEvaluator animEvaluatorAidle;
        private AnimationClipEvaluator animEvaluatorPunch2;
        private AnimationClipEvaluator animEvaluatorQuickAttack;
        private AnimationClipEvaluator animEvaluatorAWalk;
        private AnimationClipEvaluator animEvaluatorARun;
        public double currentTime = 0;

        // Idle-Walk-Run lerp
        private AnimationClipEvaluator animEvaluatorWalkLerp1;
        private AnimationClipEvaluator animEvaluatorWalkLerp2;
        private AnimationClip animationClipWalkLerp1;
        private AnimationClip animationClipWalkLerp2;
        private float walkLerpFactor = 0.5f;

        // Idle-Walk-Run lerp
        private AnimationClipEvaluator animEvaluatorAWalkLerp1;
        private AnimationClipEvaluator animEvaluatorAWalkLerp2;
        private AnimationClip animationClipAWalkLerp1;
        private AnimationClip animationClipAWalkLerp2;
        private float AwalkLerpFactor = 0.5f;


        // Internal state
        private bool isGrounded = false;
        public AnimationState state = AnimationState.Airborne;
        private readonly EventReceiver<float> runSpeedEvent = new EventReceiver<float>(PlayerController.RunSpeedEventKey);
        private readonly EventReceiver<bool> isGroundedEvent = new EventReceiver<bool>(PlayerController.IsGroundedEventKey);
        private readonly EventReceiver<bool> attackEvent = new EventReceiver<bool>(PlayerController.IsAttackingEventKey);
        private readonly EventReceiver<bool> attackEvent2 = new EventReceiver<bool>(PlayerController.IsAttackingEventKey2);
        private readonly EventReceiver<bool> attackIdle = new EventReceiver<bool>(PlayerController.IsAttackIdleEventKey);
        private readonly EventReceiver<bool> attackQUick = new EventReceiver<bool>(PlayerController.IsAttackingQuickEventKey);
        float runSpeed;

        public override void Start()
        {
            base.Start();

            if (AnimationComponent == null)
                throw new InvalidOperationException("The animation component is not set");

            if (AnimationIdle == null)
                throw new InvalidOperationException("Idle animation is not set");

            if (AnimationWalk == null)
                throw new InvalidOperationException("Walking animation is not set");

            if (AnimationRun == null)
                throw new InvalidOperationException("Running animation is not set");

            if (AnimationJumpStart == null)
                throw new InvalidOperationException("Jumping animation is not set");

            if (AnimationJumpMid == null)
                throw new InvalidOperationException("Airborne animation is not set");

            if (AnimationJumpEnd == null)
                throw new InvalidOperationException("Landing animation is not set");

            // By setting a custom blend tree builder we can override the default behavior of the animation system
            //  Instead, BuildBlendTree(FastList<AnimationOperation> blendStack) will be called each frame
            AnimationComponent.BlendTreeBuilder = this;

            animEvaluatorIdle = AnimationComponent.Blender.CreateEvaluator(AnimationIdle);
            animEvaluatorWalk = AnimationComponent.Blender.CreateEvaluator(AnimationWalk);
            animEvaluatorRun = AnimationComponent.Blender.CreateEvaluator(AnimationRun);
            animEvaluatorJumpStart = AnimationComponent.Blender.CreateEvaluator(AnimationJumpStart);
            animEvaluatorJumpMid = AnimationComponent.Blender.CreateEvaluator(AnimationJumpMid);
            animEvaluatorJumpEnd = AnimationComponent.Blender.CreateEvaluator(AnimationJumpEnd);
            animEvaluatorAidle = AnimationComponent.Blender.CreateEvaluator(AnimationAIdle);
            animEvaluatorARun = AnimationComponent.Blender.CreateEvaluator(AnimationARun);
            animEvaluatorAWalk = AnimationComponent.Blender.CreateEvaluator(AnimationAWalk);
            animEvaluatorPunch = AnimationComponent.Blender.CreateEvaluator(AnimationPunch);
            animEvaluatorPunch2 = AnimationComponent.Blender.CreateEvaluator(AnimationPunch2);
            animEvaluatorQuickAttack = AnimationComponent.Blender.CreateEvaluator(AnimationQuickAttack);

            // Initial walk lerp
            walkLerpFactor = 0;
            animEvaluatorWalkLerp1 = animEvaluatorIdle;
            animEvaluatorWalkLerp2 = animEvaluatorWalk;
            animationClipWalkLerp1 = AnimationIdle;
            animationClipWalkLerp2 = AnimationWalk;

            // Initial walk lerp
            AwalkLerpFactor = 0;
            animEvaluatorAWalkLerp1 = animEvaluatorAidle;
            animEvaluatorAWalkLerp2 = animEvaluatorAWalk;
            animationClipAWalkLerp1 = AnimationAIdle;
            animationClipAWalkLerp2 = AnimationAWalk;
        }

        public override void Cancel()
        {
            AnimationComponent.Blender.ReleaseEvaluator(animEvaluatorIdle);
            AnimationComponent.Blender.ReleaseEvaluator(animEvaluatorWalk);
            AnimationComponent.Blender.ReleaseEvaluator(animEvaluatorRun);
            AnimationComponent.Blender.ReleaseEvaluator(animEvaluatorJumpStart);
            AnimationComponent.Blender.ReleaseEvaluator(animEvaluatorJumpMid);
            AnimationComponent.Blender.ReleaseEvaluator(animEvaluatorJumpEnd);
            AnimationComponent.Blender.ReleaseEvaluator(animEvaluatorAidle);
            AnimationComponent.Blender.ReleaseEvaluator(animEvaluatorARun);
            AnimationComponent.Blender.ReleaseEvaluator(animEvaluatorAWalk);
            AnimationComponent.Blender.ReleaseEvaluator(animEvaluatorPunch);
            AnimationComponent.Blender.ReleaseEvaluator(animEvaluatorPunch2);
            AnimationComponent.Blender.ReleaseEvaluator(animEvaluatorQuickAttack);
        }

        public void UpdateWalking()
        {
            if (runSpeed < WalkThreshold)
            {
                walkLerpFactor = runSpeed / WalkThreshold;
                walkLerpFactor = (float)Math.Sqrt(walkLerpFactor);  // Idle-Walk blend looks really werid, so skew the factor towards walking
                animEvaluatorWalkLerp1 = animEvaluatorIdle;
                animEvaluatorWalkLerp2 = animEvaluatorWalk;
                animationClipWalkLerp1 = AnimationIdle;
                animationClipWalkLerp2 = AnimationWalk;
            }
            else
            {
                walkLerpFactor = (runSpeed - WalkThreshold) / (1.0f - WalkThreshold);
                animEvaluatorWalkLerp1 = animEvaluatorWalk;
                animEvaluatorWalkLerp2 = animEvaluatorRun;
                animationClipWalkLerp1 = AnimationWalk;
                animationClipWalkLerp2 = AnimationRun;
            }

            // Use DrawTime rather than UpdateTime
            var time = Game.DrawTime;
            // This update function will account for animation with different durations, keeping a current time relative to the blended maximum duration
            long blendedMaxDuration = 0;
            blendedMaxDuration =
                (long)MathUtil.Lerp(animationClipWalkLerp1.Duration.Ticks, animationClipWalkLerp2.Duration.Ticks, walkLerpFactor);

            var currentTicks = TimeSpan.FromTicks((long)(currentTime * blendedMaxDuration));

            currentTicks = blendedMaxDuration == 0
                ? TimeSpan.Zero
                : TimeSpan.FromTicks((currentTicks.Ticks + (long)(time.Elapsed.Ticks * TimeFactor)) %
                                     blendedMaxDuration);

            currentTime = ((double)currentTicks.Ticks / (double)blendedMaxDuration);
        }

        private void UpdateAWalking()
        {
            if (runSpeed < AWalkThreshold)
            {
                AwalkLerpFactor = runSpeed / AWalkThreshold;
                AwalkLerpFactor = (float)Math.Sqrt(AwalkLerpFactor);  // Idle-Walk blend looks really werid, so skew the factor towards walking
                animEvaluatorAWalkLerp1 = animEvaluatorAidle;
                animEvaluatorAWalkLerp2 = animEvaluatorAWalk;
                animationClipAWalkLerp1 = AnimationAIdle;
                animationClipAWalkLerp2 = AnimationAWalk;
            }
            else
            {
                AwalkLerpFactor = (runSpeed - AWalkThreshold) / (1.0f - AWalkThreshold);
                animEvaluatorAWalkLerp1 = animEvaluatorAWalk;
                animEvaluatorAWalkLerp2 = animEvaluatorARun;
                animationClipAWalkLerp1 = AnimationAWalk;
                animationClipAWalkLerp2 = AnimationARun;
            }

            // Use DrawTime rather than UpdateTime
            var time = Game.DrawTime;
            // This update function will account for animation with different durations, keeping a current time relative to the blended maximum duration
            long blendedMaxDuration = 0;
            blendedMaxDuration =
                (long)MathUtil.Lerp(animationClipAWalkLerp1.Duration.Ticks, animationClipAWalkLerp2.Duration.Ticks, AwalkLerpFactor);

            var currentTicks = TimeSpan.FromTicks((long)(currentTime * blendedMaxDuration));

            currentTicks = blendedMaxDuration == 0
                ? TimeSpan.Zero
                : TimeSpan.FromTicks((currentTicks.Ticks + (long)(time.Elapsed.Ticks * TimeFactor)) %
                                     blendedMaxDuration);

            currentTime = ((double)currentTicks.Ticks / (double)blendedMaxDuration);
        }

        private void UpdateJumping()
        {
            var speedFactor = 1;
            var currentTicks = TimeSpan.FromTicks((long)(currentTime * AnimationJumpStart.Duration.Ticks));
            var updatedTicks = currentTicks.Ticks + (long)(Game.DrawTime.Elapsed.Ticks * TimeFactor * speedFactor);

            if (updatedTicks < AnimationJumpStart.Duration.Ticks)
            {
                currentTicks = TimeSpan.FromTicks(updatedTicks);
                currentTime = ((double)currentTicks.Ticks / (double)AnimationJumpStart.Duration.Ticks);
            }
            else
            {
                state = AnimationState.Airborne;
                currentTime = 0;
                UpdateAirborne();
            }
        }

        private void UpdateAirborne()
        {
            // Use DrawTime rather than UpdateTime
            var time = Game.DrawTime;
            var currentTicks = TimeSpan.FromTicks((long)(currentTime * AnimationJumpMid.Duration.Ticks));
            currentTicks = TimeSpan.FromTicks((currentTicks.Ticks + (long)(time.Elapsed.Ticks * TimeFactor)) %
                                     AnimationJumpMid.Duration.Ticks);
            currentTime = ((double)currentTicks.Ticks / (double)AnimationJumpMid.Duration.Ticks);
        }

        private void UpdateLanding()
        {
            var speedFactor = 1;
            var currentTicks = TimeSpan.FromTicks((long)(currentTime * AnimationJumpEnd.Duration.Ticks));
            var updatedTicks = currentTicks.Ticks + (long)(Game.DrawTime.Elapsed.Ticks * TimeFactor * speedFactor);

            if (updatedTicks < AnimationJumpEnd.Duration.Ticks)
            {
                currentTicks = TimeSpan.FromTicks(updatedTicks);
                currentTime = ((double)currentTicks.Ticks / (double)AnimationJumpEnd.Duration.Ticks);
            }
            else
            {
               if(attackIdleOn == true)
                {
                    state = AnimationState.AWalking;
                    currentTime = 0;
                    UpdateAWalking();
                }
                else
                {
                state = AnimationState.Walking;
                currentTime = 0;
                UpdateWalking();
                }
            }
        }

        private void UpdatePunching()
        {
            var speedFactor = 1;
            var currentTicks = TimeSpan.FromTicks((long)(currentTime * AnimationPunch.Duration.Ticks));
            var updatedTicks = currentTicks.Ticks + (long)(Game.DrawTime.Elapsed.Ticks * TimeFactor * speedFactor);

            if (updatedTicks < AnimationPunch.Duration.Ticks && attackIdleOn == true)
            {
                currentTicks = TimeSpan.FromTicks(updatedTicks);
                currentTime = ((double)currentTicks.Ticks / (double)AnimationPunch.Duration.Ticks);
                input.movementsystem = true;
            }

            if (updatedTicks > AnimationPunch.Duration.Ticks && attackIdleOn == true)
            {
                state = AnimationState.AWalking;
                currentTime = 0;
                input.movementsystem = false;
                UpdateAWalking();
            }

            if (attackIdleOn == false && updatedTicks >= AnimationPunch.Duration.Ticks)
            {
                state = AnimationState.Walking;
                currentTime = 0;
                UpdateWalking();
            }
        }

        private void UpdatePunching2()
        {
            var speedFactor = 1;
            var currentTicks = TimeSpan.FromTicks((long)(currentTime * AnimationPunch2.Duration.Ticks));
            var updatedTicks = currentTicks.Ticks + (long)(Game.DrawTime.Elapsed.Ticks * TimeFactor * speedFactor);

            if (updatedTicks < AnimationPunch2.Duration.Ticks && attackIdleOn == true)
            {
                currentTicks = TimeSpan.FromTicks(updatedTicks);
                currentTime = ((double)currentTicks.Ticks / (double)AnimationPunch2.Duration.Ticks);
                input.movementsystem = true;
            }

            if (updatedTicks >= AnimationPunch2.Duration.Ticks && attackIdleOn == true)
            {
                state = AnimationState.AWalking;
                currentTime = 0;
                input.movementsystem = false;
                UpdateAWalking();
            }

            if (attackIdleOn == false && updatedTicks >= AnimationPunch2.Duration.Ticks)
            {
                state = AnimationState.Walking;
                currentTime = 0;
                UpdateWalking();
            }
        }

        private void UpdateQuickAttack()
        {
            var speedFactor = 1;
            var currentTicks = TimeSpan.FromTicks((long)(currentTime * AnimationQuickAttack.Duration.Ticks));
            var updatedTicks = currentTicks.Ticks + (long)(Game.DrawTime.Elapsed.Ticks * TimeFactor * speedFactor);

            if (updatedTicks < AnimationQuickAttack.Duration.Ticks && attackIdleOn == true)
            {
                currentTicks = TimeSpan.FromTicks(updatedTicks);
                currentTime = ((double)currentTicks.Ticks / (double)AnimationQuickAttack.Duration.Ticks);
                input.movementsystem = true;
            }

            if (updatedTicks > AnimationQuickAttack.Duration.Ticks && attackIdleOn == true)
            {
                state = AnimationState.AWalking;
                currentTime = 0;
                input.movementsystem = false;
                UpdateAWalking();
            }

            if (attackIdleOn == false && updatedTicks >= AnimationQuickAttack.Duration.Ticks)
            {
                state = AnimationState.Walking;
                currentTime = 0;
                UpdateWalking();
            }
        }

        public override void Update()
        {
            // State control
            runSpeedEvent.TryReceive(out runSpeed);
            bool isGroundedNewValue;
            isGroundedEvent.TryReceive(out isGroundedNewValue);
            if (isGrounded != isGroundedNewValue)
            {
                currentTime = 0;
                isGrounded = isGroundedNewValue;
                state = (isGrounded) ? AnimationState.Landing : AnimationState.Jumping;
            }

            bool isAttackingNewValue;
            if (attackEvent.TryReceive(out isAttackingNewValue) && isAttackingNewValue && state != AnimationState.Punching)
            {
                currentTime = 0;
                state = AnimationState.Punching;
            }

            bool isAttackingNewValue2;
            if (attackEvent2.TryReceive(out isAttackingNewValue2) && isAttackingNewValue2 && state != AnimationState.Punching2)
            {
                currentTime = 0;
                state = AnimationState.Punching2;
            }

            bool isAttackIdle;
            if (attackIdle.TryReceive(out isAttackIdle) && isAttackIdle && state != AnimationState.AWalking)
            {
                currentTime = 0;
                state = AnimationState.AWalking;
            }

            bool isQuickAttack;
            if (attackQUick.TryReceive(out isQuickAttack) && isQuickAttack && state != AnimationState.QuickAttack)
            {
                currentTime = 0;
                state = AnimationState.QuickAttack;
            }

            switch (state)
            {
                case AnimationState.Walking: UpdateWalking(); break;
                case AnimationState.Jumping: UpdateJumping(); break;
                case AnimationState.Airborne: UpdateAirborne(); break;
                case AnimationState.Landing: UpdateLanding(); break;
                case AnimationState.Punching: UpdatePunching(); break;
                case AnimationState.Punching2: UpdatePunching2(); break;
                case AnimationState.QuickAttack: UpdateQuickAttack(); break;
                case AnimationState.AWalking: UpdateAWalking(); break;
            }
        }

        /// <summary>
        /// BuildBlendTree is called every frame from the animation system when the <see cref="AnimationComponent"/> needs to be evaluated
        /// It overrides the default behavior of the <see cref="AnimationComponent"/> by setting a custom blend tree
        /// </summary>
        /// <param name="blendStack">The stack of animation operations to be blended</param>
        public void BuildBlendTree(FastList<AnimationOperation> blendStack)
        {
            switch (state)
            {
                case AnimationState.Walking:
                    {
                        // Note! The tree is laid out as a stack and has to be flattened before returning it to the animation system!
                        blendStack.Add(AnimationOperation.NewPush(animEvaluatorWalkLerp1,
                            TimeSpan.FromTicks((long)(currentTime * animationClipWalkLerp1.Duration.Ticks))));
                        blendStack.Add(AnimationOperation.NewPush(animEvaluatorWalkLerp2,
                            TimeSpan.FromTicks((long)(currentTime * animationClipWalkLerp2.Duration.Ticks))));
                        blendStack.Add(AnimationOperation.NewBlend(CoreAnimationOperation.Blend, walkLerpFactor));
                    }
                    break;

                case AnimationState.Jumping:
                    {
                        blendStack.Add(AnimationOperation.NewPush(animEvaluatorJumpStart,
                            TimeSpan.FromTicks((long)(currentTime * AnimationJumpStart.Duration.Ticks))));
                    }
                    break;

                case AnimationState.Airborne:
                    {
                        blendStack.Add(AnimationOperation.NewPush(animEvaluatorJumpMid,
                            TimeSpan.FromTicks((long)(currentTime * AnimationJumpMid.Duration.Ticks))));
                    }
                    break;

                case AnimationState.Landing:
                    {
                        blendStack.Add(AnimationOperation.NewPush(animEvaluatorJumpEnd,
                            TimeSpan.FromTicks((long)(currentTime * AnimationJumpEnd.Duration.Ticks))));
                    }
                    break;

                case AnimationState.Punching:
                    {
                        blendStack.Add(AnimationOperation.NewPush(animEvaluatorPunch,
                            TimeSpan.FromTicks((long)(currentTime * AnimationPunch.Duration.Ticks))));
                    }
                    break;

                case AnimationState.Punching2:
                    {
                        blendStack.Add(AnimationOperation.NewPush(animEvaluatorPunch2,
                            TimeSpan.FromTicks((long)(currentTime * AnimationPunch2.Duration.Ticks))));
                    }
                    break;

                case AnimationState.QuickAttack:
                    {
                        blendStack.Add(AnimationOperation.NewPush(animEvaluatorQuickAttack,
                            TimeSpan.FromTicks((long)(currentTime * AnimationQuickAttack.Duration.Ticks))));
                    }
                    break;

                case AnimationState.AWalking:
                    {
                        // Note! The tree is laid out as a stack and has to be flattened before returning it to the animation system!
                        blendStack.Add(AnimationOperation.NewPush(animEvaluatorAWalkLerp1,
                            TimeSpan.FromTicks((long)(currentTime * animationClipAWalkLerp1.Duration.Ticks))));
                        blendStack.Add(AnimationOperation.NewPush(animEvaluatorAWalkLerp2,
                            TimeSpan.FromTicks((long)(currentTime * animationClipAWalkLerp2.Duration.Ticks))));
                        blendStack.Add(AnimationOperation.NewBlend(CoreAnimationOperation.Blend, AwalkLerpFactor));
                    }
                    break;
            }
        }

        public enum AnimationState
        {
            Walking,
            Jumping,
            Airborne,
            Landing,
            Punching,
            Punching2,
            QuickAttack,
            AWalking,
        }
    }
}
