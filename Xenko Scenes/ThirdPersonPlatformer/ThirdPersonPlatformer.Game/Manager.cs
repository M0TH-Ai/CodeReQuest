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
using ThirdPersonPlatformer.Player;
using Xenko.Physics;

namespace ThirdPersonPlatformer
{
    public class Manager : SyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public int counter = 0;

        public Trigger Trigger { get; set; }

        public float AnimationTime => animationTime;

        public bool Activated { get; } = false;
        public EventReceiver<bool> TriggeredEvent { get; set; }

        public Manager(int counter, Trigger trigger, bool activated, EventReceiver<bool> triggeredEvent, float animationTime, Entity sword, Entity removeSword, Prefab weaponToSpawn, Entity sierra, PlayerController player, bool start, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            this.counter = counter;
            Trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
            Activated = activated;
            TriggeredEvent = triggeredEvent ?? throw new ArgumentNullException(nameof(triggeredEvent));
            this.animationTime = animationTime;
            Sword = sword ?? throw new ArgumentNullException(nameof(sword));
            this.removeSword = removeSword ?? throw new ArgumentNullException(nameof(removeSword));
            WeaponToSpawn = weaponToSpawn ?? throw new ArgumentNullException(nameof(weaponToSpawn));
            Sierra = sierra ?? throw new ArgumentNullException(nameof(sierra));
            this.player = player ?? throw new ArgumentNullException(nameof(player));
            this.start = start;
            SpawnPosition = spawnPosition;
            SpawnRotation = spawnRotation;
        }

        private readonly float animationTime = 0;
        
        public Entity Sword;
        public Entity removeSword;
        
        public Prefab WeaponToSpawn;
        
        public Entity Sierra;
        
        public PlayerController player;
        
        private bool start = false;
        
        public Vector3 SpawnPosition;
        
        public Quaternion SpawnRotation;

        public Manager(float animationTime)
        {
            this.animationTime = animationTime;
        }

        public override void Start()
        {
            // Initialization of the script.
            TriggeredEvent = (Trigger != null) ? new EventReceiver<bool>(Trigger.TriggerEvent) : null;
        }

        public override void Update()
        {
#pragma warning disable IDE0018 // Inline variable declaration
            bool triggered;
#pragma warning restore IDE0018 // Inline variable declaration
            if (!Activated && (TriggeredEvent?.TryReceive(out triggered) ?? false))
            {
                Sword.Transform.Parent = Sierra.Transform;
                if(player.attacking == true)
                {
                    Sword.Get<ModelNodeLinkComponent>().NodeName = "RightHand";
                    Sword.Transform.Position = new Vector3(0.133f,0.063f,0.003f);
                    Sword.Get<RigidbodyComponent>().Enabled = true;
                    player.PunchCollision = Sword.Get<RigidbodyComponent>();
                    player.sword = Sword;
                    //Sword.Get<StaticColliderComponent>().Enabled = false;
                    start = true;
                }
                else
                {
                    Sword.Get<ModelNodeLinkComponent>().NodeName = "Hips";
                    Sword.Transform.Position = new Vector3(0,0.052f,-0.158f);
                    Sword.Get<RigidbodyComponent>().Enabled = false;
                    player.PunchCollision = Sword.Get<RigidbodyComponent>();
                    player.sword = Sword;
                    //Sword.Get<StaticColliderComponent>().Enabled = false;
                    start = true;
               }
               
               
            }

            //Do stuff every new frame
            async Task cleanupTask()
            {
                await Game.WaitTime(TimeSpan.FromMilliseconds(1));

            }

            async Task spawnEntity()
            {
                await Game.WaitTime(TimeSpan.FromSeconds(10));

                removeSword.Transform.Parent = null;
                removeSword.Transform.Position = SpawnPosition;
                removeSword.Transform.Rotation = SpawnRotation;

            }



            if (start == true)
            {
                Script.AddTask(cleanupTask);
                Script.AddTask(spawnEntity);
            }
         }
    }
}
