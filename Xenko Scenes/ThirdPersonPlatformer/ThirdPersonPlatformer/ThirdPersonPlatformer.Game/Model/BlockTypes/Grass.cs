using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Engine;
using ThirdPersonPlatformer.Managers;

namespace ThirdPersonPlatformer.Model.BlockTypes
{
    public class Grass: BlockType
    {
        public override Prefab BlockPrefab { get; set; }

        public Grass()
        {
            TypeName = "grass";
            BlockPrefab = WorldManager.ContentMgr.Load<Prefab>("Prefabs/VoxelGrassPrefab");
        }
    }
}
