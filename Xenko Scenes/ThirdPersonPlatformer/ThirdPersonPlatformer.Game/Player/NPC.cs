using Xenko.Engine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPersonPlatformer.Animation_Controller;

namespace ThirdPersonPlatformer.Player
{
    public abstract class NPC : SyncScript
    {
        public IList<AnimationState> animations { get; set;}
        public Queue<AnimationState> animationStates { get; set; }

        public AnimationComponent animationComponent { get; set; }

        public NPC()
        {
            animationStates = new Queue<AnimationState>();
            animationComponent.Entity.Get<AnimationComponent>();
        }

        public bool IsPlaying { get; set; }


        public void AddAnimationState(AnimationState state)
        {
            if (state != null)
            {
                animationStates.Enqueue(state);
            }
            
        }
       



    }
}
