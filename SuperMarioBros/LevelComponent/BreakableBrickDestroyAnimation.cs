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
                mBrickPieces[i].AddEvent(mBrickPieces[i].JumpAboveHeight);
                mBrickPieces[i].mHeightMin = 600;
                mBrickPieces[i].AddEvent(mBrickPieces[i].DeleteObstacle);
                mBrickPieces[i].mPosition = mPosition;
                mBrickPieces[i].mSpriteSheet = mSpriteSheet;
                mBrickPieces[i].mSpriteSize = new Point(16, 16);

                mBrickPieces[i].mHorizontalSpeed = new Speed(20);
                mBrickPieces[i].mHorizontalSpeed.mAllowNegativeSpeed = true;
                mBrickPieces[i].mHorizontalSpeed.SpeedToMax();
                if (i % 2 == 0)
                {
                    mBrickPieces[i].mDrawnRectangle = new Rectangle(new Point(32, 16), mBrickPieces[i].mSpriteSize);
                    mBrickPieces[i].mVerticalSpeed = new Speed(100);
                    mBrickPieces[i].mVerticalSpeed.mAcceleration = 20;
                }
                else
                {
                    mBrickPieces[i].mDrawnRectangle = new Rectangle(new Point(48, 16), mSpriteSize);
                    mBrickPieces[i].mVerticalSpeed = new Speed(200);
                    mBrickPieces[i].mVerticalSpeed.mAcceleration = 10;
                }
                mBrickPieces[i].mVerticalSpeed.mAllowNegativeSpeed = true;
                mBrickPieces[i].mSpritePivotPoint = new Vector2(8.0f, 8.0f);
            }
            mBrickPieces[0].mHorizontalSpeed.SpeedToMin();
            mBrickPieces[1].mHorizontalSpeed.SpeedToMin();
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.ElapsedGameTime.Milliseconds != 0)
            {
                foreach (MovableObstacle c in mBrickPieces)
                {
                    c.mRotation += 0.5f;
                    c.Update(gameTime);
                }
            }
        }        
    }
}
