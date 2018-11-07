using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using Xenko.Physics;

namespace Player
{
    public class PhysicsTrigger : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio

 public override async Task Execute()
         {
             var trigger = Entity.Get<PhysicsComponent>();
             trigger.ProcessCollisions = true;

             // Start state machine
             while (Game.IsRunning)
             {
                 // 1. Wait for an entity to collide with the trigger
                 var firstCollision = await trigger.NewCollision();

                 var otherCollider = trigger == firstCollision.ColliderA
                     ? firstCollision.ColliderB
                     : firstCollision.ColliderA;
                     if(otherCollider.Entity.Get<RigidbodyComponent>() != null){
                otherCollider.Entity.Get<RigidbodyComponent>().ApplyImpulse(otherCollider.Entity.Transform.WorldMatrix.Forward * 5);
                otherCollider.Entity.Get<RigidbodyComponent>().ApplyTorqueImpulse(otherCollider.Entity.Transform.WorldMatrix.Forward * 5 + new Vector3(0, 1, 0));
                }

                 // 2. Wait for the entity to exit the trigger
                 //Collision collision;

                 //do
                 //{
                  //   collision = await trigger.CollisionEnded();
                // }
                // while (collision != firstCollision);

                 //otherCollider.Entity.Transform.Scale= new Vector3(1.0f, 1.0f, 1.0f);
             }
         }
     }
 }
 