using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using Xenko.Engine.Events;
using TopDownRPG2.Gameplay;

namespace ThirdPersonPlatformer
{
    public class BoxCollisionScript : SyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public int counter = 0;

        public Trigger Trigger { get; set; }

        private EventReceiver<bool> triggeredEvent;

        private bool activated = false;
        public bool count = false;
        
        private float animationTime = 0;


        public override void Start()
        {
            // Initialization of the script.
            triggeredEvent = (Trigger != null) ? new EventReceiver<bool>(Trigger.TriggerEvent) : null;
        }

        public override void Update()
        {
        
            UpdateAnimation();

            bool triggered;
            if (!activated && (triggeredEvent?.TryReceive(out triggered) ?? false))
            {
                counter += 1;
                count = false;
            }

            // Do stuff every new frame
            Func<Task> cleanupTask = async () =>
            {
                await Game.WaitTime(TimeSpan.FromMilliseconds(1500));

                Game.RemoveEntity(Entity);
            };

            if(counter == 8)
            {
            activated = true;
                Script.AddTask(cleanupTask);
            }
         }
         
          public void UpdateAnimation()
        {
            if (!activated)
                return;

            var dt = (float)Game.UpdateTime.Elapsed.TotalSeconds;

            animationTime += dt * 8;
            var coinHeight = Math.Max(0, Math.Sin(animationTime));
            Entity.Transform.Position.Y = 1 + (float)coinHeight;

            var uniformScale = (float) Math.Max(0, Math.Min(1, (2 * Math.PI - animationTime) / Math.PI));
            Entity.Transform.Scale = new Vector3(uniformScale);
        }
    }
}
