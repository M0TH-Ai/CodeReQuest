using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using TopDownRPG2.Gameplay;
using Xenko.Engine.Events;

namespace ThirdPersonPlatformer
{
    public class UIMainScene : SyncScript
    {
        // Declared public member fields and properties will show in the game studio
         public Trigger Trigger { get; set; }

        private EventReceiver<bool> triggeredEvent;

        private bool activated = false;
        
        public UIComponent UI;
        

        public override void Start()
        {
            // Initialization of the script.
            triggeredEvent = (Trigger != null) ? new EventReceiver<bool>(Trigger.TriggerEvent) : null;
            UI.Enabled = false;
        }

        public override void Update()
        {
            // Do stuff every new frame
            bool triggered;
            if (!activated && (triggeredEvent?.TryReceive(out triggered) ?? false))
            {
                UI.Enabled = true;
                
            }
      }
    }
}
