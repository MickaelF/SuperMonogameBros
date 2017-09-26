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
            End
        }

        protected enum LevelContext
        {
            OVERWORLD,
            UNDERWORLD,
            CASTLE,
            UNDERWATER
        }

        private List<DrawableObstacle> _mObstacles;
        public List<DrawableObstacle> mObstacles { get => _mObstacles; private set => _mObstacles = value; }
        private List<BackgroundSprites> mBackgroundSprite;

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

            mObstacles = new List<DrawableObstacle>();
            mBackgroundSprite = new List<BackgroundSprites>();

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
            }
            return type;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, GraphicsDevice graphics)
        {
            Texture2D spriteSheet = content.Load<Texture2D>("UnTilableObjects-OW1");
            mTextures[(int)ObjectType.BreakableBricks] = spriteSheet;
            mTextures[(int)ObjectType.QuestionMarkBricks] = spriteSheet;
            mTextures[(int)ObjectType.Pipe] = spriteSheet;
            mTextures[(int)ObjectType.Clouds] = spriteSheet;
            mTextures[(int)ObjectType.LargeHills] = spriteSheet;
            mTextures[(int)ObjectType.SmallHills] = spriteSheet;
            mTextures[(int)ObjectType.Bushes] = spriteSheet;
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
                    mObstacles.Add(block);
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
                    mObstacles.Add(block);
                    posX += 16;
                }
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

            foreach (Rectangle rect in mMapDictionary[ObjectType.Clouds])
            {
                BackgroundSprites cloud = new BackgroundSprites(ObjectType.Clouds, rect.Location);
                cloud.mSpriteSheet = mTextures[(int)ObjectType.Clouds];
                mBackgroundSprite.Add(cloud);
            }
            foreach (Rectangle rect in mMapDictionary[ObjectType.Bushes])
            {
                BackgroundSprites bushe = new BackgroundSprites(ObjectType.Bushes, rect.Location);
                bushe.mSpriteSheet = mTextures[(int)ObjectType.Bushes];
                mBackgroundSprite.Add(bushe);
            }
            foreach (Rectangle rect in mMapDictionary[ObjectType.SmallHills])
            {
                BackgroundSprites sh = new BackgroundSprites(ObjectType.SmallHills, rect.Location);
                sh.mSpriteSheet = mTextures[(int)ObjectType.SmallHills];
                mBackgroundSprite.Add(sh);
            }
            foreach (Rectangle rect in mMapDictionary[ObjectType.LargeHills])
            {
                BackgroundSprites lh = new BackgroundSprites(ObjectType.LargeHills, rect.Location);
                lh.mSpriteSheet = mTextures[(int)ObjectType.LargeHills];
                mBackgroundSprite.Add(lh);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (DrawableObstacle obst in mObstacles)
            {
                obst.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (BackgroundSprites sprite in mBackgroundSprite)
            {
                sprite.Draw(spriteBatch);
            }
            foreach (DrawableObstacle obst in mObstacles)
            {
                obst.Draw(spriteBatch);
            }
        }

        private InteractiveBlock.ITEM_TYPE ContentInBlock(Point location)
        {
            foreach (Rectangle rect in mMapDictionary[ObjectType.Coins])
            {
                if (rect.Location == location)
                {
                    mMapDictionary[ObjectType.Coins].Remove(rect);
                    return InteractiveBlock.ITEM_TYPE.COIN;
                }
            }
            foreach (Rectangle rect in mMapDictionary[ObjectType.Stars])
            {
                if (rect.Location == location)
                {
                    mMapDictionary[ObjectType.Stars].Remove(rect);
                    return InteractiveBlock.ITEM_TYPE.STAR;
                }
            }
            foreach (Rectangle rect in mMapDictionary[ObjectType.OneUp])
            {
                if (rect.Location == location)
                {
                    mMapDictionary[ObjectType.OneUp].Remove(rect);
                    return InteractiveBlock.ITEM_TYPE.ONE_UP;
                }
            }

            foreach (Rectangle rect in mMapDictionary[ObjectType.Mushroom])
            {
                if (rect.Location == location)
                {
                    mMapDictionary[ObjectType.Mushroom].Remove(rect);
                    return InteractiveBlock.ITEM_TYPE.RED_MUSHROOM;
                }
            }
            return InteractiveBlock.ITEM_TYPE.NONE;
        }
    }
}
