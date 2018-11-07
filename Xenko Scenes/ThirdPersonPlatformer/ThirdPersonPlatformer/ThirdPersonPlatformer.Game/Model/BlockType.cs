using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Engine;
using ThirdPersonPlatformer.Managers;

namespace ThirdPersonPlatformer.Model
{
    public abstract class BlockType
    {
        public string TypeName { get; set; }
        public virtual Prefab BlockPrefab { get; set; }

        protected BlockType()
        {
            BlockPrefab = WorldManager.ContentMgr.Load<Prefab>("Prefabs/VoxelPrefab");
            BlockPrefab.Instantiate();
        }

    }
}
