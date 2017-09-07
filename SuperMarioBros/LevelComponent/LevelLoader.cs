using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperMarioBros.DisplayComponent;

namespace SuperMarioBros.LevelComponent
{
    public class LevelLoader
    {
        enum ObjectType
        {
            None,
            Ground,
            BreakableBricks,
            UnbreakableBricks,
            QuestionMarkBricks,
            Pipe,
            PlayerStartPoint,
            End
        }

        protected enum LevelContext
        {
            OVERWORLD,
            UNDERWORLD,
            CASTLE,
            UNDERWATER
        }
        public List<DrawableObstacle> mObstacles { get => _mObstacles; private set => _mObstacles = value; }

        public Vector2 mMarioStartPosition;
        private TMXParser mFileParser;
        private LevelContext mLevelContext;
        private string mFilePath;
        private Texture2D[] mTextures;
        Dictionary<ObjectType, List<Rectangle>> mMapDictionary;
        private List<DrawableObstacle> _mObstacles;

        public LevelLoader(string filePath = "C:\\Users\\Mickael\\Desktop\\C#\\Mario\\Mario!\\Mario!\\Content\\Map\\1-1.tmx")
        {
            mFilePath = filePath;
            mFileParser = new TMXParser(mFilePath);
            mTextures = new Texture2D[(int)ObjectType.End];

            mObstacles = new List<DrawableObstacle>();

            if (mFileParser.mContextName == "Overworld")
            {
                mLevelContext = LevelContext.OVERWORLD;
            }
            else if (mFileParser.mContextName == "Underworld")
            {
                mLevelContext = LevelContext.UNDERWORLD;
            }
            else if (mFileParser.mContextName == "Castle")
            {
                mLevelContext = LevelContext.CASTLE;
            }
            else if (mFileParser.mContextName == "Underwater")
            {
                mLevelContext = LevelContext.UNDERWATER;
            }

            mMapDictionary = new Dictionary<ObjectType, List<Rectangle>>();
            mMapDictionary[ObjectType.Ground] = new List<Rectangle>();
            mMapDictionary[ObjectType.BreakableBricks] = new List<Rectangle>();
            mMapDictionary[ObjectType.UnbreakableBricks] = new List<Rectangle>();
            mMapDictionary[ObjectType.QuestionMarkBricks] = new List<Rectangle>();
            mMapDictionary[ObjectType.Pipe] = new List<Rectangle>();
            mMapDictionary[ObjectType.PlayerStartPoint] = new List<Rectangle>();

            int lastId = 0;
            int indexFirstIdFound = -1;
            for (int i = 0; i < mFileParser.mTilePosition.Length; i++)
            {
                for (int j = 0; j < mFileParser.mTilePosition[i].Length; j++)
                {
                    if (mFileParser.mTilePosition[i][j] != 0 && lastId == 0)
                    {
                        indexFirstIdFound = j;
                        lastId = mFileParser.mTilePosition[i][j];
                    }
                    else if (mFileParser.mTilePosition[i][j] == lastId && lastId != 0 && j == mFileParser.mTilePosition[i].Length - 1)
                    {
                        mMapDictionary[NameToObjectType(mFileParser.WhichTile(lastId).mTextureName)].Add(new Rectangle(indexFirstIdFound * mFileParser.mTileSize.X, i * mFileParser.mTileSize.Y, (j - indexFirstIdFound) * mFileParser.mTileSize.X, 1 * mFileParser.mTileSize.Y));
                        indexFirstIdFound = -1;
                        lastId = 0;
                    }
                    else if (mFileParser.mTilePosition[i][j] != lastId && mFileParser.mTilePosition[i][j] == 0)
                    {
                        mMapDictionary[NameToObjectType(mFileParser.WhichTile(lastId).mTextureName)].Add(new Rectangle(indexFirstIdFound * mFileParser.mTileSize.X, i * mFileParser.mTileSize.Y, (j - indexFirstIdFound) * mFileParser.mTileSize.X, 1 * mFileParser.mTileSize.Y));
                        indexFirstIdFound = -1;
                        lastId = mFileParser.mTilePosition[i][j];
                    }
                    else if (mFileParser.mTilePosition[i][j] != lastId && mFileParser.mTilePosition[i][j] != 0)
                    {
                        mMapDictionary[NameToObjectType(mFileParser.WhichTile(lastId).mTextureName)].Add(new Rectangle(indexFirstIdFound * mFileParser.mTileSize.X, i * mFileParser.mTileSize.Y, (j - indexFirstIdFound) * mFileParser.mTileSize.X, 1 * mFileParser.mTileSize.Y));
                        indexFirstIdFound = j;
                        lastId = mFileParser.mTilePosition[i][j];
                    }
                }
            }
            Tile pipeTile = null;
            foreach(Tile t in mFileParser.mTileList)
            {
                if(t.mTextureName == "Pipe")
                {
                    pipeTile = t;
                }
            }
            for (int i = 0; i < mFileParser.mPipeTilePosition.Length; ++i)
            {
                for (int j = 0; j < mFileParser.mPipeTilePosition[i].Length; j++)
                {
                    int tileNum = mFileParser.mPipeTilePosition[i][j];
                    if (pipeTile.TileCorrespondance(tileNum))
                    {
                        if(tileNum == pipeTile.mFirstId)
                        {
                            int x = i+1;
                            while (pipeTile.TileCorrespondance(mFileParser.mPipeTilePosition[x][j]))
                            {
                                x++;
                            }
                            mMapDictionary[ObjectType.Pipe].Add(new Rectangle(j * mFileParser.mTileSize.X, i * mFileParser.mTileSize.Y, mFileParser.mTileSize.X * 2, mFileParser.mTileSize.Y * (x-i)));
                            j++;
                        }
                    }
                }
            }
        }

        private ObjectType NameToObjectType(string name)
        {
            ObjectType type = ObjectType.None;
            switch (name)
            {
                case "Ground":
                    type = ObjectType.Ground;
                    break;
                case "BreakableBricks":
                    type = ObjectType.BreakableBricks;
                    break;
                case "UnbreakableBricks":
                    type = ObjectType.UnbreakableBricks;
                    break;
                case "QuestionMarkBricks":
                    type = ObjectType.QuestionMarkBricks;
                    break;
                case "Pipe":
                    type = ObjectType.Pipe;
                    break;
                case "Player-Start-Mark":
                    type = ObjectType.PlayerStartPoint;
                    break;
            }
            return type;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, GraphicsDevice graphics)
        {
            Texture2D spriteSheet = content.Load<Texture2D>("BackGroundObject");
            mTextures[(int)ObjectType.BreakableBricks] = spriteSheet;
            mTextures[(int)ObjectType.QuestionMarkBricks] = spriteSheet;
            mTextures[(int)ObjectType.Pipe] = spriteSheet;
            switch (mLevelContext)
            {
                case LevelContext.OVERWORLD:
                    mTextures[(int)ObjectType.Ground] = content.Load<Texture2D>("Tiles/Ground-OW");
                    mTextures[(int)ObjectType.UnbreakableBricks] = content.Load<Texture2D>("Tiles/UnbreakableBrick-OW");
                    break;
                case LevelContext.UNDERWATER:
                    mTextures[(int)ObjectType.Ground] = content.Load<Texture2D>("Tiles/Ground-W");
                    mTextures[(int)ObjectType.UnbreakableBricks] = content.Load<Texture2D>("Tiles/UnbreakableBrick-W");
                    break;
                case LevelContext.UNDERWORLD:
                    mTextures[(int)ObjectType.Ground] = content.Load<Texture2D>("Tiles/Ground-UW");
                    mTextures[(int)ObjectType.UnbreakableBricks] = content.Load<Texture2D>("Tiles/UnbreakableBrick-UW");
                    break;
                case LevelContext.CASTLE:
                    mTextures[(int)ObjectType.Ground] = content.Load<Texture2D>("Tiles/Ground-C");
                    mTextures[(int)ObjectType.UnbreakableBricks] = content.Load<Texture2D>("Tiles/UnbreakableBrick-C");
                    break;
            }
            Create();
        }

        public void Create()
        {
            mMarioStartPosition = new Vector2(mMapDictionary[ObjectType.PlayerStartPoint][0].X, mMapDictionary[ObjectType.PlayerStartPoint][0].Y);
            foreach (Rectangle rect in mMapDictionary[ObjectType.QuestionMarkBricks])
            {
                InteractiveBlock block = new InteractiveBlock((int)mLevelContext, 0, rect);
                block.mSpriteSheet = mTextures[(int)ObjectType.QuestionMarkBricks];
                mObstacles.Add(block);
            }

            foreach (Rectangle rect in mMapDictionary[ObjectType.BreakableBricks])
            {
                InteractiveBlock block = new InteractiveBlock((int)mLevelContext, 1, rect);
                block.mSpriteSheet = mTextures[(int)ObjectType.BreakableBricks];
                mObstacles.Add(block);
            }

            foreach (Rectangle rect in mMapDictionary[ObjectType.Ground])
            {
                TilableBlock block = new TilableBlock(rect);
                block.mSpriteSheet = mTextures[(int)ObjectType.Ground];
                mObstacles.Add(block);
            }

            foreach (Rectangle rect in mMapDictionary[ObjectType.UnbreakableBricks])
            {
                TilableBlock block = new TilableBlock(rect);
                block.mSpriteSheet = mTextures[(int)ObjectType.UnbreakableBricks];
                mObstacles.Add(block);
            }
            foreach (Rectangle rect in mMapDictionary[ObjectType.Pipe])
            {
                PipeBlock pipe = new PipeBlock((int)mLevelContext, rect);
                pipe.mSpriteSheet = mTextures[(int)ObjectType.Pipe];
                mObstacles.Add(pipe);
            }
        }

        public void Update()
        {
            foreach (DrawableObstacle obst in mObstacles)
            {
                obst.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(DrawableObstacle obst in mObstacles)
            {
                obst.Draw(spriteBatch);
            }
        }
    }
}
