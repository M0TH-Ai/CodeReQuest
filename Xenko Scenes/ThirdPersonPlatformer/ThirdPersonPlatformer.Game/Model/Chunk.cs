using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Animations;
using Xenko.Engine;
using ThirdPersonPlatformer.Managers;

namespace ThirdPersonPlatformer.Model
{
    public class Chunk
    {
        public Vector3 ChunkPos { get; set; }
        private Dictionary<Vector3, Block> Blocks { get; set; }
        public void GenChunk(Vector3 Position)
        {
            ChunkPos = Position;
            Blocks = new Dictionary<Vector3, Block>();
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 16; k++)
                    {
                        Block B = new Block();//add worldgen here
                        B.BType = BlockManager.GetBlockType("grass");
                        B.Init();
                        Blocks.Add(new Vector3(ChunkPos.X + i, ChunkPos.Y + j, ChunkPos.Z + k),B);
                    }
                }
            }
        }

        public void ShowChunk()
        {
            foreach (KeyValuePair<Vector3, Block> block in Blocks)
            {
              //  WorldManager.SceneMgr.SceneInstance.RootScene.Entities.AddRange(block.Value.entities);
            }
        }
    }
}
