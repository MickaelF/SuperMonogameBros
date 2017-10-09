using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperMarioBros.DisplayComponent;
using SuperMarioBros.PhysicComponent;

namespace SuperMarioBros.LevelComponent
{
    class BreakableBrickDestroyAnimation : DrawableObstacle
    {
        private MovableObstacle[] mBrickPieces;

        public BreakableBrickDestroyAnimation(Vector2 topLeftOrigin, Texture2D spriteSheet)
        {
            mSpriteSheet = spriteSheet;
            mPosition = topLeftOrigin;
            mBrickPieces = new MovableObstacle[4];
            for (int i = 0; i < 4; ++i)
            {
                mBrickPieces[i] = new MovableObstacle();
                mBrickPieces[i].mPosition = mPosition;
                mBrickPieces[i].mSpriteSheet = mSpriteSheet;
                mBrickPieces[i].mSpriteSize = new Point(16, 16);
                
                if (i % 2 == 0)
                {
                    mBrickPieces[i].mDrawnRectangle = new Rectangle(new Point(32, 16), mBrickPieces[i].mSpriteSize);
                    mBrickPieces[i].mVerticalSpeed = new Speed(100);
                    mBrickPieces[i].mMoveVector = new Vector2((i > 0) ? 1.0f : -1.0f, -1.0f);
                }
                else
                {
                    mBrickPieces[i].mDrawnRectangle = new Rectangle(new Point(48, 16), mSpriteSize);
                    mBrickPieces[i].mVerticalSpeed = new Speed(200);
                    mBrickPieces[i].mMoveVector = new Vector2((i > 1) ? 1.0f : -1.0f, -1.0f);
                }
                mBrickPieces[i].mVerticalSpeed.mAllowNegativeSpeed = true;
                mBrickPieces[i].mVerticalSpeed.SpeedToMax();
                mBrickPieces[i].mVerticalSpeed.mAcceleration = 10;
                mBrickPieces[i].mSpritePivotPoint = new Vector2(8.0f, 8.0f);
            }                
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.ElapsedGameTime.Milliseconds != 0)
            {
                foreach (MovableObstacle c in mBrickPieces)
                {
                    c.mMovementInPixel = new Vector2(0.50f, c.mVerticalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                    c.mVerticalSpeed.SlowDown();
                    c.mRotation += 1.0f;
                    c.Update(gameTime);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (MovableObstacle c in mBrickPieces)
            {
                c.Draw(spriteBatch);
            }
        }
    }
}
