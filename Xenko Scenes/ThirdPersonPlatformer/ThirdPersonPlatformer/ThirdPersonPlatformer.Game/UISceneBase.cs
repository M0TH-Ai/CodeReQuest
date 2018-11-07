// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.co.jp)
// See LICENSE.md for full license information.
using System;
using Xenko.Core;
using Xenko.Core.Mathematics;
using Xenko.Engine;
using Xenko.Games;

namespace ThirdPersonPlatformer
{
    public abstract class UISceneBase : SyncScript
    {
        protected Game UIGame;

        protected bool IsRunning;

        protected bool SceneCreated;

        public override void Start()
        {
            IsRunning = true;

#pragma warning disable CS0618 // Type or member is obsolete
            UIGame = (Game)Services.GetServiceAs<IGame>();
#pragma warning restore CS0618 // Type or member is obsolete

            AdjustVirtualResolution(this, EventArgs.Empty);
            Game.Window.ClientSizeChanged += AdjustVirtualResolution;

            CreateScene();
        }

        public override void Update()
        {
            UpdateScene();
        }

        protected virtual void UpdateScene()
        {
        }

        public override void Cancel()
        {
            Game.Window.ClientSizeChanged -= AdjustVirtualResolution;

            IsRunning = false;
            SceneCreated = false;
        }

        private void AdjustVirtualResolution(object sender, EventArgs e)
        {
            var backBufferSize = new Vector2(GraphicsDevice.Presenter.BackBuffer.Width, GraphicsDevice.Presenter.BackBuffer.Height);
            Entity.Get<UIComponent>().Resolution = new Vector3(backBufferSize, 1000);
        }

        protected void CreateScene()
        {
            if (!SceneCreated)
                LoadScene();

            SceneCreated = true;
        }

        protected abstract void LoadScene();
    }
}
