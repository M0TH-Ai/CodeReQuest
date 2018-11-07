using System;
using Xenko.Core.Collections;
using Xenko.Animations;
using Xenko.Engine;
using ThirdPersonPlatformer.Player;

namespace ThirdPersonPlatformer.Animation_Controller
{
    class WalkState : AnimationState, IBlendTreeBuilder
    {
        public WalkState(PlayerController playerController) : base(playerController)
        {
        }

        public override void OnEnter()
        {
            throw new NotImplementedException();
        }

        public override void OnExit()
        {
            throw new NotImplementedException();
        }

        public override void OnUpdate()
        {
            throw new NotImplementedException();
        }

        public void BuildBlendTree(FastList<AnimationOperation> animationList)
        {
            throw new NotImplementedException();
        }
    }
}
