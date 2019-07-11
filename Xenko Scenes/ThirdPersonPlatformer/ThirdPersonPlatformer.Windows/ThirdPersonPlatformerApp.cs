using Xenko.Engine;

namespace ThirdPersonPlatformer.Windows
{
    class ThirdPersonPlatformerApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
