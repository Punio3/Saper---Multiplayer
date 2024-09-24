using multigame.GameLogic;
using multigame;
using System.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace multigame
{

    public static class Images
    {
        private static readonly Dictionary<BlockType, ImageSource> blockSources = new()
    {
        {BlockType.background_light, LoadImage("Assets/background1.png") },
        {BlockType.background_dark, LoadImage("Assets/background2.png") },
        {BlockType.grass_light, LoadImage("Assets/ground1.png") },
        {BlockType.grass_dark, LoadImage("Assets/ground2.png") },
        {BlockType.bomb, LoadImage("Assets/bomb.png") }

    };

        private static readonly Dictionary<int, ImageSource> NumberSources = new()
    {
        {1, LoadImage("Assets/jeden.png") },
        {2, LoadImage("Assets/dwa.png") },
        {3, LoadImage("Assets/trzy.png") },
        {4, LoadImage("Assets/cztery.png") },
        {5, LoadImage("Assets/piec.png") },
        {6, LoadImage("Assets/szesc.png") },
        {7, LoadImage("Assets/siedem.png") },
        {8, LoadImage("Assets/osiem.png") }
    };


        private static ImageSource LoadImage(string filePath)
        {
            return new BitmapImage(new Uri(filePath, UriKind.Relative));
        }

        public static ImageSource GetImage(GrassAndFlags block)
        {
            if (!block.isClicked) return blockSources[block.Type];
            else return null;
        }

        public static ImageSource GetImage(Block block)
        {
            return blockSources[block.Type];
        }

        public static ImageSource GetImage(bool isFlagged)
        {
            if (isFlagged) return LoadImage("Assets/flaga.png");
            else return null;
        }
        public static ImageSource GetImage(BombsAndNumbers block)
        {
            if (block.Type == BlockType.bomb)
            {
                return blockSources[block.Type];
            }
            else if (block.AmountOfBombs > 0)
            {
                return NumberSources[block.AmountOfBombs];
            }
            return null;

        }
    }
}