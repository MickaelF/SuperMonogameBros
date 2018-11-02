 using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperMarioBros.DisplayComponent;

namespace SuperMarioBros.LevelComponent
{
    public class LevelLoader
    {
        public enum ObjectType
        {
            None,
            Ground,
            BreakableBricks,
            UnbreakableBricks,
            QuestionMarkBricks,
            Pipe,
            PlayerStartPoint,
            SmallHills, 
            LargeHills, 
            Clouds,
            Bushes,
            Coins, 
            Mushroom, 
            Stars,
            OneUp,
            Goomba,
            End
        }

        protected enum LevelContext
        {
            OVERWORLD,
            UNDERWORLD,
            CASTLE,
            UNDERWATER
        }
       
        private ObstacleAccessor mObstacleAccessor;
        public Vector2 mMarioStartPosition;
        private TMXParser mFileParser;
        private LevelContext mLevelContext;
        private string mFilePath;
        private Texture2D[] mTextures;
        Dictionary<ObjectType, List<Rectangle> > mMapDictionary;

        public LevelLoader(string filePath = "C:\\Users\\Mickael\\Desktop\\SuperMarioBros\\SuperMarioBros\\Content\\Maps\\1-1.tmx")
        {
            mFilePath = filePath;
            mFileParser = new TMXParser(mFilePath);
            mTextures = new Texture2D[(int)ObjectType.End];
            mObstacleAccessor = ObstacleAccessor.Instance;
            
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

            mMapDictionary = new Dictionary<ObjectType, List<Rectangle> >();

            foreach (ObjectType type in System.Enum.GetValues(typeof(ObjectType)))
            {
                mMapDictionary[type] = new List<Rectangle>();
            }


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
                    else
                    {                         
                        if (mFileParser.mTilePosition[i][j] == lastId && lastId != 0 && j == mFileParser.mTilePosition[i].Length - 1)
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

            for (int i = 0; i < mFileParser.mBackgroundTilePosition.Length; ++i)
            {
                for (int j = 0; j < mFileParser.mBackgroundTilePosition[i].Length; j++)
                {
                    if (mFileParser.mBackgroundTilePosition[i][j] != 0)
                    {
                        mMapDictionary[NameToObjectType(mFileParser.WhichTile(mFileParser.mBackgroundTilePosition[i][j]).mTextureName)].Add(new Rectangle(j * mFileParser.mTileSize.X, i * mFileParser.mTileSize.Y, mFileParser.mTileSize.X, mFileParser.mTileSize.Y));
                    }
                }
            }

            for (int i = 0; i < mFileParser.mBonusTilePosition.Length; ++i)
            {
                for (int j = 0; j < mFileParser.mBonusTilePosition[i].Length; j++)
                {
                    if (mFileParser.mBonusTilePosition[i][j] != 0)
                    {
                        mMapDictionary[NameToObjectType(mFileParser.WhichTile(mFileParser.mBonusTilePosition[i][j]).mTextureName)].Add(new Rectangle(j * mFileParser.mTileSize.X, i * mFileParser.mTileSize.Y, mFileParser.mTileSize.X, mFileParser.mTileSize.Y));
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
                case "UnbreakableBrick-OW":
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
                case "Bushe":
                    type = ObjectType.Bushes;
                    break;
                case "LongHills":
                    type = ObjectType.LargeHills;
                    break;
                case "SmallHills":
                    type = ObjectType.SmallHills;
                    break;
                case "Cloud":
                    type = ObjectType.Clouds;
                    break;
                case "Coin":
                    type = ObjectType.Coins;
                    break;
                case "Stars":
                    type = ObjectType.Stars;
                    break;
                case "Mushroom":
                    type = ObjectType.Mushroom;
                    break;
                case "OneUp":
                    type = ObjectType.OneUp;
                    break;
                case "Goomba":
                    type = ObjectType.Goomba;
                    break;
                default:
                    break;
            }
            return type;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, GraphicsDeviceManager graphics)
        {
            Texture2D untilableSheet = content.Load<Texture2D>("UnTilableObjects-OW1");
            Texture2D enemiesSheet = content.Load<Texture2D>("Enemies");
            mTextures[(int)ObjectType.BreakableBricks] = untilableSheet;
            mTextures[(int)ObjectType.QuestionMarkBricks] = untilableSheet;
            mTextures[(int)ObjectType.Pipe] = untilableSheet;
            mTextures[(int)ObjectType.Clouds] = untilableSheet;
            mTextures[(int)ObjectType.LargeHills] = untilableSheet;
            mTextures[(int)ObjectType.SmallHills] = untilableSheet;
            mTextures[(int)ObjectType.Bushes] = untilableSheet;
            mTextures[(int)ObjectType.Goomba] = enemiesSheet;
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
                int posX = rect.Left;
                while (posX < rect.Right)
                {
                    Rectangle rectPos = new Rectangle(posX, rect.Top, 16, 16);
                    InteractiveBlock block = new InteractiveBlock((int)mLevelContext, true, rectPos);
                    block.mContent = ContentInBlock(rectPos.Location);
                    block.mSpriteSheet = mTextures[(int)ObjectType.BreakableBricks];
                    posX += 16;
                }
            }

            foreach (Rectangle rect in mMapDictionary[ObjectType.BreakableBricks])
            {
                int posX = rect.Left;
                while(posX < rect.Right)
                {
                    Rectangle rectPos = new Rectangle(posX, rect.Top, 16, 16);
                    InteractiveBlock block = new InteractiveBlock((int)mLevelContext, false, rectPos);
                    block.mContent = ContentInBlock(rectPos.Location);
                    block.mSpriteSheet = mTextures[(int)ObjectType.BreakableBricks];
                    posX += 16;
                }
            }

            foreach (Rectangle rect in mMapDictionary[ObjectType.Ground])
            {
                TilableBlock block = new TilableBlock(rect);
                block.mSpriteSheet = mTextures[(int)ObjectType.Ground];
            }

            foreach (Rectangle rect in mMapDictionary[ObjectType.UnbreakableBricks])
            {
                TilableBlock block = new TilableBlock(rect);
                block.mSpriteSheet = mTextures[(int)ObjectType.UnbreakableBricks];
            }

            foreach (Rectangle rect in mMapDictionary[ObjectType.Pipe])
            {
                PipeBlock pipe = new PipeBlock((int)mLevelContext, rect);
                pipe.mSpriteSheet = mTextures[(int)ObjectType.Pipe];
            }

            foreach (Rectangle rect in mMapDictionary[ObjectType.Clouds])
            {
                BackgroundSprites cloud = new BackgroundSprites(ObjectType.Clouds, rect.Location);
                cloud.mSpriteSheet = mTextures[(int)ObjectType.Clouds];
            }
            foreach (Rectangle rect in mMapDictionary[ObjectType.Bushes])
            {
                BackgroundSprites bushe = new BackgroundSprites(ObjectType.Bushes, rect.Location);
                bushe.mSpriteSheet = mTextures[(int)ObjectType.Bushes];
            }
            foreach (Rectangle rect in mMapDictionary[ObjectType.SmallHills])
            {
                BackgroundSprites sh = new BackgroundSprites(ObjectType.SmallHills, rect.Location);
                sh.mSpriteSheet = mTextures[(int)ObjectType.SmallHills];
            }
            foreach (Rectangle rect in mMapDictionary[ObjectType.LargeHills])
            {
                BackgroundSprites lh = new BackgroundSprites(ObjectType.LargeHills, rect.Location);
                lh.mSpriteSheet = mTextures[(int)ObjectType.LargeHills];
            }
            foreach (Rectangle rect in mMapDictionary[ObjectType.Goomba])
            {
                Goomba g = new Goomba(rect.Location);
                g.mSpriteSheet = mTextures[(int)ObjectType.Goomba];
            }
        }

        public void Reload()
        {
            mObstacleAccessor.Clear();
            Create();
        }
        
        private InteractiveBlock.ITEM_TYPE ContentInBlock(Point location)
        {
            foreach (Rectangle rect in mMapDictionary[ObjectType.Coins])
            {
                if (rect.Location == location)
                {
                    return InteractiveBlock.ITEM_TYPE.COIN;
                }
            }
            foreach (Rectangle rect in mMapDictionary[ObjectType.Stars])
            {
                if (rect.Location == location)
                {
                    return InteractiveBlock.ITEM_TYPE.STAR;
                }
            }
            foreach (Rectangle rect in mMapDictionary[ObjectType.OneUp])
            {
                if (rect.Location == location)
                {
                    return InteractiveBlock.ITEM_TYPE.ONE_UP;
                }
            }

            foreach (Rectangle rect in mMapDictionary[ObjectType.Mushroom])
            {
                if (rect.Location == location)
                {
                    return InteractiveBlock.ITEM_TYPE.RED_MUSHROOM;
                }
            }
            return InteractiveBlock.ITEM_TYPE.NONE;
        }
    }
}
