using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Engine;

namespace ThirdPersonPlatformer.Model
{
    public class Block
    {
        public BlockType BType { get; set; }
       // public List<Entity> entities;

        public Block()
        {
            
        }

        public void Init()
        {
            //entities = BType.BlockPrefab

        }
    }

}
