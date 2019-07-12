// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.co.jp)
// See LICENSE.md for full license information.
using System.Collections.Generic;
using System.Linq;
using Xenko.Core.Mathematics;
using Xenko.Engine;
using Xenko.Engine.Events;
using Xenko.Input;
using ThirdPersonPlatformer.Core;

namespace ThirdPersonPlatformer.Player
{
    public class PlayerInput : SyncScript
    {
        /// <summary>
        /// Raised every frame with the intended direction of movement from the player.
        /// </summary>
        // TODO Should not be static, but allow binding between player and controller
        
        public static readonly EventKey<Vector3> MoveDirectionEventKey = new EventKey<Vector3>();

        public static readonly EventKey<Vector2> CameraDirectionEventKey = new EventKey<Vector2>();

        public static readonly EventKey<bool> JumpEventKey = new EventKey<bool>();
        private bool jumpButtonDown = false;

        public float DeadZone { get; set; } = 0.25f;

        public CameraComponent Camera { get; set; }
        
        public PlayerController ControllerComponent { get; set; }
        
        public bool movementsystem = false;
        
        public bool moving = false;
        
        public WeaponUIscript.weapon;
        
        /// <summary>
        /// Multiplies move movement by this amount to apply aim rotations
        /// </summary>
        
        public float MouseSensitivity = 100.0f;

        public List<Keys> KeysLeft { get; } = new List<Keys>();

        public List<Keys> KeysRight { get; } = new List<Keys>();

        public List<Keys> KeysUp { get; } = new List<Keys>();

        public List<Keys> KeysDown { get; } = new List<Keys>();

        public List<Keys> KeysJump { get; } = new List<Keys>();
        
        public List<Keys> KeysAttack { get; } = new List<Keys>();
        
        public List<Keys> KeysAttack2 { get; } = new List<Keys>();
        
        public List<Keys> KeysAttackMode { get; } = new List<Keys>();

        public List<Keys> SwitchRight { get; } = new List<Keys>();

        public List<Keys> SwitchLeft { get; } = new List<Keys>();

        public override void Update()
        {
            // Character movement: should be camera-aware
            {
                // Left stick: movement
                var moveDirection = Input.GetLeftThumbAny(DeadZone);

                // Keyboard: movement
                if(movementsystem == false)
                {
                if (KeysLeft.Any(key => Input.IsKeyDown(key)))
                {
                    moveDirection += -Vector2.UnitX;
                    moving = true;
                }
                if (KeysRight.Any(key => Input.IsKeyDown(key)))
                {
                    moveDirection += +Vector2.UnitX;
                    moving = true;
                }
                if (KeysUp.Any(key => Input.IsKeyDown(key)))
                {
                    moveDirection += +Vector2.UnitY;
                    moving = true;
                }
                if (KeysDown.Any(key => Input.IsKeyDown(key)))
                {
                    moveDirection += -Vector2.UnitY;
                    moving = true;
                }


                //release keys

                
                if(KeysLeft.Any(key => Input.IsKeyReleased(key)))
                {
                    moving = false;
                }
                
                if(KeysRight.Any(key => Input.IsKeyReleased(key)))
                {
                    moving = false;
                }
                
                if(KeysDown.Any(key => Input.IsKeyReleased(key)))
                {
                    moving = false;
                }
                
                if(KeysUp.Any(key => Input.IsKeyReleased(key)))
                {
                    moving = false;
                }
                }

                if (SwitchLeft.Any(key => Input.IsKeyReleased(key)))
                {
                    weapon.switchleft = true;
                }

                if (SwitchRight.Any(key => Input.IsKeyReleased(key)))
                {
                    weapon.switchright = true;
                }


                if (KeysAttack.Any(key => Input.IsKeyReleased(key)))
                {
                    ControllerComponent.Attack();
                    //PlayerController.IsAttackingEventKey.Broadcast(true);
                    moveDirection += +Vector2.UnitY * 5;
                }
                
                if(KeysAttack2.Any(key => Input.IsKeyReleased(key)))
                {
                    ControllerComponent.Attack();
                    PlayerController.IsAttackingEventKey2.Broadcast(true);
                }

                if (KeysAttackMode.Any(key => Input.IsKeyReleased(key)))
                {
                    ControllerComponent.SetAttack();
                }
                    
                    

                // Broadcast the movement vector as a world-space Vector3 to allow characters to be controlled
                var worldSpeed = (Camera != null)
                    ? Utils.LogicDirectionToWorldDirection(moveDirection, Camera, Vector3.UnitY)
                    : new Vector3(moveDirection.X, 0, moveDirection.Y);

                // Adjust vector's magnitute - worldSpeed has been normalized
                var moveLength = moveDirection.Length();
                var isDeadZoneLeft = moveLength < DeadZone;
                if (isDeadZoneLeft)
                {
                    worldSpeed = Vector3.Zero;
                }
                else
                {
                    if (moveLength > 1)
                    {
                        moveLength = 1;
                    }
                    else
                    {
                        moveLength = (moveLength - DeadZone) / (1f - DeadZone);
                    }

                    worldSpeed *= moveLength;
                }

                MoveDirectionEventKey.Broadcast(worldSpeed);
            }

            // Camera rotation: left-right rotates the camera horizontally while up-down controls its altitude
            {
                // Right stick: camera rotation
                var cameraDirection = Input.GetRightThumbAny(DeadZone);
                var isDeadZoneRight = cameraDirection.Length() < DeadZone;
                if (isDeadZoneRight)
                    cameraDirection = Vector2.Zero;
                else
                    cameraDirection.Normalize();

                // Mouse-based camera rotation. Only enabled after you click the screen to lock your cursor, pressing escape cancels this
                if (Input.IsMouseButtonDown(MouseButton.Left))
                {
                    Input.LockMousePosition(true);
                    Game.IsMouseVisible = false;
                }
                if (Input.IsKeyPressed(Keys.Escape))
                {
                    Input.UnlockMousePosition();
                    Game.IsMouseVisible = true;
                }
                if (Input.IsMousePositionLocked)
                {
                    cameraDirection += new Vector2(Input.MouseDelta.X, -Input.MouseDelta.Y)*MouseSensitivity;
                }

                // Broadcast the camera direction directly, as a screen-space Vector2
                CameraDirectionEventKey.Broadcast(cameraDirection);
            }

            // Jumping: don't bother with jump restrictions here, just pass the button states
            {
                // Controller: jumping
                var isJumpDown = Input.IsGamePadButtonDownAny(GamePadButton.A);
                var didJump = (!jumpButtonDown && isJumpDown);
                jumpButtonDown = isJumpDown;

                // Keyboard: jumping
                didJump |= (KeysJump.Any(key => Input.IsKeyPressed(key)));

                JumpEventKey.Broadcast(didJump);
            }
        }
    }
}
