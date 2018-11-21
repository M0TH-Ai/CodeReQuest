using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPersonPlatformer.Model;
using ThirdPersonPlatformer.Model.BlockTypes;

namespace ThirdPersonPlatformer.Managers
{
    public class BlockManager
    {
        public static List<BlockType> BlockTypes { get; set; }

        public static void LoadTypes()
        {
            BlockTypes = new List<BlockType>();

            BlockTypes.Add(new Grass());
        }

        public static BlockType GetBlockType(string Name)
        {
            BlockType Btype = BlockTypes.FirstOrDefault(x => x.TypeName.ToLower() == Name.ToLower());
            if (Btype == null)
            {
                return new DefaultBType();
            }
            return Btype;
        }

    }
}
