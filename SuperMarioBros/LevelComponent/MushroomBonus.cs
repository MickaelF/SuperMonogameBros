using SuperMarioBros.DisplayComponent;
using SuperMarioBros.PhysicComponent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SuperMarioBros.LevelComponent
{
    public class MushroomBonus : MovableObstacle
    {
        private bool mIsOneUp;
        private bool mStartAnimation;
        private float mStartPosition;
        public MushroomBonus(bool oneUpBonus, Vector2 origin, Texture2D spriteSheet)
        {
            mStartAnimation = true;
            mStartPosition = origin.Y - 16.0f;
            mSpriteSheet = spriteSheet;
            mIsOneUp = oneUpBonus;
            mMoveVector = new Vector2(1.0f, -1.0f);
            mPosition = new Vector2(origin.X, origin.Y);
            mHorizontalSpeed = new Speed(50);
            mHorizontalSpeed.mAllowNegativeSpeed = true;
            mHorizontalSpeed.SpeedToMax();
            mVerticalSpeed = new Speed(100);
            mVerticalSpeed.mAllowNegativeSpeed = true;
            mVerticalSpeed.mAcceleration = 10;
            mSize = new Vector2(16.0f, 16.0f);

            mSpriteSize = new Point(16, 16);
             
            if(oneUpBonus)
            {
                mDrawnRectangle = new Rectangle(new Point(96, 96), mSpriteSize);
            }
            else
            {
                mDrawnRectangle = new Rectangle(new Point(80, 96), mSpriteSize);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!mStartAnimation)
            {
                mMovementInPixel = new Vector2(mHorizontalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f, mVerticalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

                CollisionDetection();
                if (mHorizontalCollision)
                {
                    mMovementInPixel.X = mHorizontalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                }
                if (mIsFalling)
                {
                    mVerticalSpeed.SlowDown();
                }
                else if (mVerticalSpeed.mCurrentSpeed != 0.0f)
                {
                    mVerticalSpeed.Stop();
                    mMovementInPixel.Y = 0.0f;
                }
            }
            else
            {
                if (mPosition.Y > mStartPosition)
                {
                    mMovementInPixel = new Vector2(0.0f, 20 * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                }
                else
                {
                    mStartAnimation = false;
                }
            }
            base.Update(gameTime);
        }

        public override void CollisionEffect(Obstacle obst, CollisionWay way)
        {
            if (obst is Mario)
            {
                ObstacleAccessor.Instance.Remove(this);
                if (mIsOneUp)
                {
                    (obst as Mario).LifeUp();
                }
                else
                {
                    (obst as Mario).StateUp();
                }
            }
            else if (way == CollisionWay.LEFT || way == CollisionWay.RIGHT)
            {
                mHorizontalSpeed.mEvolveInPositiveNumber = !mHorizontalSpeed.mEvolveInPositiveNumber;
                mHorizontalSpeed.SpeedToMax();
            }
        }
    }
}
