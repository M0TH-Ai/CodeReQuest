using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Core.Serialization.Contents;
using Xenko.Engine;
using ThirdPersonPlatformer.Model;
using ThirdPersonPlatformer.Player;
using ThirdPersonPlatformer.World;

namespace ThirdPersonPlatformer.Managers
{
    public class WorldManager:SyncScript
    {
        public static Dictionary<Vector3, Chunk> Chunks { get; set; }
        public static ContentManager ContentMgr { get; set; }
        public static SceneSystem SceneMgr { get; set; }
        //private PlayerController PlayerController;
        public TransformComponent Player;
        public float VoxelScale;
        private bool isGening = false;


        public int ViewDistanceInChunks { get; set; } = 1;

        public override void Start()
        {
            ContentMgr = Content;
            SceneMgr = SceneSystem;
            BlockManager.LoadTypes();
            Chunks = new Dictionary<Vector3, Chunk>();
        }

        public override void Update()
        {
            Coordinate roundedPlayerPosition = new Coordinate(Player.Position.X * 1 / VoxelScale, Player.Position.Y * 1 / VoxelScale, Player.Position.Z * 1 / VoxelScale);


            if (!isGening)
            {
                isGening = true;
                CreateChunks(roundedPlayerPosition);
            }


            

            //chunk range checking
        }


        private void CreateChunks(Coordinate PlayerPos)
        {
            int px =  (int)Math.Ceiling((double)(PlayerPos.X / 16));
            int py = (int)Math.Ceiling((double)(PlayerPos.Y / 16));
            int pz = (int)Math.Ceiling((double)(PlayerPos.Z / 16));

            for (int x = 0-ViewDistanceInChunks; x <= ViewDistanceInChunks; x++)
            {
                for (int y = 0-1; y <= 1; y++)
                {
                    for (int z = 0-ViewDistanceInChunks; z <= ViewDistanceInChunks; z++)
                    {
                        Vector3 ChunkPos = new Vector3(px+x,py+y,pz+z);
                        if (!Chunks.ContainsKey(ChunkPos))
                        {
                            Chunk C = new Chunk();
                            C.GenChunk(ChunkPos);
                            Chunks.Add(ChunkPos,C);
                            C.ShowChunk();
                        }
                    }
                }
            }
        }

    }
}
