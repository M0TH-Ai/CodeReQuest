using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPersonPlatformer.Player;

namespace ThirdPersonPlatformer.Animation_Controller
{
    public class AnimationManager
    {
        private IList<NPC> npcs = new List<NPC>();

        public void Process(NPC npc, AnimationState state)
        {
            npc.animationComponent.Play(state.AnimationStateName);
        }

        public void ProcessNPCS()
        {
            for (int index = npcs.Count-1; index >= 0; index--)
            {
                NPC npc = npcs[index];
                if (npc.animationStates.Count > 0 && !npc.IsPlaying)
                {
                    AnimationState state = npc.animationStates.Dequeue();
                    Process(npc, state);
                }
            }
        }






    }
}
