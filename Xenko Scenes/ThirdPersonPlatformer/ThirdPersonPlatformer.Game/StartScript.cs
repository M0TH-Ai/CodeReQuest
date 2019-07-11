using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;

namespace ThirdPersonPlatformer
{
    public class StartScript : UISceneBase
    {
        protected override void LoadScene()
        {
            // Allow user to resize the window with the mouse.
            Game.Window.AllowUserResizing = true;
        }
        
         protected override void UpdateScene()
        {
            if (Input.PointerEvents.Any(e => e.EventType == PointerEventType.Pressed))
            {
                // Next scene
                SceneSystem.SceneInstance.RootScene = Content.Load<Scene>("MainScene"); //test world
                //SceneSystem.SceneInstance.RootScene = Content.Load<Scene>("OpenWorld");
                Cancel();
            }
        }
    }
 }