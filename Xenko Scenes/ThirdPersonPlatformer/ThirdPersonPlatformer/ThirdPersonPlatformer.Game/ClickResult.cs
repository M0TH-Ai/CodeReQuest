// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.co.jp)
// See LICENSE.md for full license information.
using Xenko.Core.Mathematics;
using Xenko.Engine;
using Xenko.Physics;

namespace ThirdPersonPlatformer
{
    public enum ClickType
    {
        /// <summary>
        /// The result didn't hit anything
        /// </summary>
        Empty,

        /// <summary>
        /// The result hit a ground object
        /// </summary>
        Ground,

        /// <summary>
        /// The result hit a treasure chest object
        /// </summary>
        LootCrate,
    }

    /// <summary>
    /// Result of the user clicking/tapping on the world
    /// </summary>
    public struct ClickResult
    {
        /// <summary>
        /// The world-space position of the click, where the raycast hits the collision body
        /// </summary>
        public Vector3      WorldPosition;

        /// <summary>
        /// The Entity containing the collision body which was hit
        /// </summary>
        public Entity       ClickedEntity;

        /// <summary>
        /// What kind of object did we hit
        /// </summary>
        public ClickType    Type;

        /// <summary>
        /// The HitResult received from the physics simulation
        /// </summary>
        public HitResult    HitResult;
    }
}
