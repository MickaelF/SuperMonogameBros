using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperMarioBros.DisplayComponent
{
    public class GameInfos
    {
        private SpriteFont mFont;
        private int[] mLineX;
        private string[] mFirstLine;
        private float mTimeLeft;
        private Texture2D mCoin;
        private Texture2D mMarioSprite;
        private int mElapsedTime;
        private int mStopFirstFrameCoinAnimation;

        private Rectangle mDrawnRectangle;
        
        public GameInfos()
        {
            mLineX = new int[4];
            mFirstLine = new string[4];
            mElapsedTime = 0;
            mDrawnRectangle = new Rectangle(0, 0, 9, 13);
            mStopFirstFrameCoinAnimation = 0;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, GraphicsDeviceManager graphics)
        {
            mLineX[0] = 10;
            mLineX[1] = graphics.PreferredBackBufferWidth / 4;
            mLineX[2] = graphics.PreferredBackBufferWidth / 2;
            mLineX[3] = (graphics.PreferredBackBufferWidth * 3) / 4;

            mFirstLine[0] = "Mario";
            mFirstLine[1] = "";
            mFirstLine[2] = "World";
            mFirstLine[3] = "Time";
            mTimeLeft = 400000;
            mFont = content.Load<SpriteFont>("font/UI");
            mMarioSprite = content.Load<Texture2D>("Player-Start-Mark");
            mCoin = content.Load<Texture2D>("font/coin");
        }

        public virtual void Update(GameTime gameTime)
        {
            if (mTimeLeft == -1)
            {
                mTimeLeft = 400000;
            }
            mTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
            mElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            if (mElapsedTime > 100)
            {
                bool changeDrawnCoin = true;
                if(mDrawnRectangle.X == 0)
                {
                    if (++mStopFirstFrameCoinAnimation < 5)
                    {
                        changeDrawnCoin = false;
                    }
                }
                if (changeDrawnCoin)
                { 
                    mDrawnRectangle.X = (mDrawnRectangle.X >= 40) ? 0 : mDrawnRectangle.X + 10;
                    mStopFirstFrameCoinAnimation = 0;
                }
                mElapsedTime = 0;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, ref Mario mario, ref Camera cam)
        {
            for (int i = 0; i < 4; ++i)
            {
                spriteBatch.DrawString(mFont, mFirstLine[i], cam.ScreenToWorldPosition(new Vector2(mLineX[i], 10)), Color.White);
            }
            int time = (int)(mTimeLeft / 1000);
            string[] secondLine = new string[4];
            secondLine[0] = mario.mScore.ToString();
            secondLine[1] = "x" + mario.mNbCoins.ToString();
            secondLine[2] = "1-1";
            secondLine[3] = time.ToString();
            if (mario.mIsDead)
            {
                secondLine[3] = "";
                mTimeLeft = -1;
                spriteBatch.Draw(mMarioSprite, cam.ScreenToWorldPosition(new Vector2(334, 200)), Color.White);
                spriteBatch.DrawString(mFont, "x" + mario.mNbLife.ToString(), cam.ScreenToWorldPosition(new Vector2(370, 200)), Color.White);
            }
            for (int i = 0; i < 4; ++i)
            {
                spriteBatch.DrawString(mFont, secondLine[i], cam.ScreenToWorldPosition(new Vector2(mLineX[i], 30)), Color.White);
            }
            
            spriteBatch.Draw(mCoin, cam.ScreenToWorldPosition(new Vector2(mLineX[1] - 20, 30)), mDrawnRectangle, Color.White);

            
        }
    }
}
