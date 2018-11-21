using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPersonPlatformer.Player;


namespace ThirdPersonPlatformer.Animation_Controller
{
    public abstract class AnimationState
    {
        public string AnimationStateName { get; set; }
        protected PlayerController playerController;

        protected AnimationState(string Aname, PlayerController playerController)
        {
            this.playerController = playerController;
            this.AnimationStateName = Aname;
        }

        protected AnimationState(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate();

    }
}
