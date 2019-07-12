using System.Linq;
using Xenko.Core.Mathematics;
using Xenko.Engine;
using Xenko.UI;
using Xenko.UI.Controls;
using Xenko.Input;
using Xenko.Physics;

namespace ThirdPersonPlatformer
{
    public class WeaponUIscript : UISceneBase
    {

        private UIPage page;

        public Prefab SyrikSword;
        public Prefab SierraSword;

        public Entity Sierra;

        public Player.PlayerController player;

        private bool sierraActive = false;

        private bool syrikActive = false;

        public Entity currentWeapon;

        public Quaternion idleSyrik;
        public Quaternion runningSyrik;

        public Quaternion idleSierra;
        public Quaternion runningSierra;

        public bool switchleft = false;
        public bool switchright = false;

        public bool Startswitch  = false;
        public bool Swithcdone = false;
        protected override void LoadScene()
        {
            // Allow user to resize the window with the mouse.
            Game.Window.AllowUserResizing = true;
            page = Entity.Get<UIComponent>().Page;

            var rootelement = page.RootElement;
            var sierraButton = rootelement.FindVisualChildOfType<Button>("SierraButton");
            var syrikButton = rootelement.FindVisualChildOfType<Button>("SyrikButton");

            if (Input.HasGamePad)
            {
                int gamePadCount = Input.GamePadCount;

                foreach (var gamepad in Input.GamePads)
                {
                    if (gamepad.IsButtonPressed(GamePadButton.A) && syrikActive == true)
                    {
                        var syrik = SyrikSword.Instantiate();
                        //syrik.RemoveRange(0, 1);
                        Game.RemoveEntity(currentWeapon);
                        SceneSystem.SceneInstance.RootScene.Entities.AddRange(syrik);
                        syrik.First().Transform.Parent = Sierra.Transform;
                        syrik.First().Get<ModelNodeLinkComponent>().NodeName = "Hips";
                        syrik.First().Transform.Position = new Vector3(-0.026f, 0, -0.147f);
                        syrik.First().Get<RigidbodyComponent>().Enabled = false;
                        player.PunchCollision = syrik.First().Get<RigidbodyComponent>();
                        player.sword = syrik.First();
                        player.idleRotation = idleSyrik;
                        player.runningRotation = runningSyrik;
                        syrikActive = true;

                    }

                    if (gamepad.IsButtonPressed(GamePadButton.A) && sierraActive == true)
                    {
                        var sierra = SierraSword.Instantiate();
                        //sierra.RemoveRange(0, 1);
                        Game.RemoveEntity(currentWeapon);
                        SceneSystem.SceneInstance.RootScene.Entities.AddRange(sierra);
                        sierra.First().Transform.Parent = Sierra.Transform;
                        sierra.First().Get<ModelNodeLinkComponent>().NodeName = "Hips";
                        sierra.First().Transform.Position = new Vector3(-0.026f, 0, -0.147f);
                        sierra.First().Get<RigidbodyComponent>().Enabled = false;
                        player.PunchCollision = sierra.First().Get<RigidbodyComponent>();
                        player.sword = sierra.First();
                        player.idleRotation = idleSierra;
                        player.runningRotation = runningSierra;
                    }
                }
            }

            syrikButton.Click += delegate
                    {
                        var syrik = SyrikSword.Instantiate();
                        if (syrikActive == true)
                        {
                            syrik.RemoveRange(0, 1);
                            syrikActive = false;
                        }
                        else
                        {
                            Game.RemoveEntity(currentWeapon);
                            SceneSystem.SceneInstance.RootScene.Entities.AddRange(syrik);
                            syrik.First().Transform.Parent = Sierra.Transform;
                            syrik.First().Get<ModelNodeLinkComponent>().NodeName = "Hips";
                            syrik.First().Transform.Position = new Vector3(-0.026f, 0, -0.147f);
                            syrik.First().Get<RigidbodyComponent>().Enabled = false;
                            player.PunchCollision = syrik.First().Get<RigidbodyComponent>();
                            player.sword = syrik.First();
                            player.idleRotation = idleSyrik;
                            player.runningRotation = runningSyrik;
                            syrikActive = true;
                        }
                    };

            sierraButton.Click += delegate
            {
                var sierra = SierraSword.Instantiate();
                if (syrikActive == true)
                {
                    sierra.RemoveRange(0, 1);
                    syrikActive = false;
                }
                else
                {
                    Game.RemoveEntity(currentWeapon);
                    SceneSystem.SceneInstance.RootScene.Entities.AddRange(sierra);
                    sierra.First().Transform.Parent = Sierra.Transform;
                    sierra.First().Get<ModelNodeLinkComponent>().NodeName = "Hips";
                    sierra.First().Transform.Position = new Vector3(-0.026f, 0, -0.147f);
                    sierra.First().Get<RigidbodyComponent>().Enabled = false;
                    player.PunchCollision = sierra.First().Get<RigidbodyComponent>();
                    player.sword = sierra.First();
                    player.idleRotation = idleSierra;
                    player.runningRotation = runningSierra;
                    syrikActive = true;
                }
            };
        }


        protected override void UpdateScene()
        {
            var rootelement = page.RootElement;
            var sierraButton = rootelement.FindVisualChildOfType<Button>("SierraButton");
            var syrikButton = rootelement.FindVisualChildOfType<Button>("SyrikButton");
            if (switchright == true)
            {
                if (sierraButton.GetPanelZIndex() == 1)
                {
                    sierraButton.SetPanelZIndex(0);
                    syrikButton.SetPanelZIndex(1);
                    switchright = false;
                }
                else
                {
                    sierraButton.SetPanelZIndex(1);
                    syrikButton.SetPanelZIndex(0);
                    switchright = false;
                }
            }

            if (switchleft == true)
            {
                if (sierraButton.GetPanelZIndex() == 1)
                {
                    sierraButton.SetPanelZIndex(0);
                    syrikButton.SetPanelZIndex(1);
                    switchright = false;
                }
                else
                {
                    sierraButton.SetPanelZIndex(1);
                    syrikButton.SetPanelZIndex(0);
                    switchright = false;
                }
            }
        }
    }
}
