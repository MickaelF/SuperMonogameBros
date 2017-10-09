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
        
        public GameInfos()
        {
            mLineX = new int[4];
            mFirstLine = new string[4];
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
            mCoin = content.Load<Texture2D>("font/coin");
        }

        public virtual void Update(GameTime gameTime)
        {
            mTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
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
            for (int i = 0; i < 4; ++i)
            {
                spriteBatch.DrawString(mFont, secondLine[i], cam.ScreenToWorldPosition(new Vector2(mLineX[i], 30)), Color.White);
            }
            spriteBatch.Draw(mCoin, cam.ScreenToWorldPosition(new Vector2(mLineX[1] - 14, 30)), Color.White);
        }
    }
}
