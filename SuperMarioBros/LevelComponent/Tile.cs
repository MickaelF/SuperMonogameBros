using Microsoft.Xna.Framework;

namespace SuperMarioBros.LevelComponent
{
    public class Tile
    {
        public readonly int mFirstId;
        public readonly int mTileCount;
        public readonly int mNbColumns;
        private string _mTextureName;
        public string mTextureName
        {
            get => _mTextureName;
            set
            {
                _mTextureName = value.Remove(value.LastIndexOf('.'), 4).Substring(value.LastIndexOf('/') + 1);
            }
        }
        public Point mSize;

        public Tile(int id, int tileCount, int nbCol, string texName)
        {
            mFirstId = id;
            mTileCount = tileCount;
            mNbColumns = nbCol;
            mTextureName = texName;
        }

        public bool TileCorrespondance(int id)
        {
            if (id >= mFirstId && id < mFirstId + mTileCount)
            {
                return true;
            }
            return false;
        }
    }
}
