using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperMarioBros.DisplayComponent;
using SuperMarioBros.PhysicComponent;

namespace SuperMarioBros.LevelComponent
{
    public class FireballBonus : Bonus
    {
        private bool mIsBouncing;

        public FireballBonus(Vector2 origin, Texture2D sprite, bool goingRight)
            :base(origin, sprite)
        {
            mIsBouncing = false;

            RemoveEvent(PlayStartAnimation);
            mPrimaryEvent = FireballBounce;
            mPrimaryDrawEvent = Animate;

            mIsAnimated = true;
            mSpriteAnimationStepNumber = new int[2];
            mSpriteAnimationStepNumber[0] = 1;
            mSpriteAnimationStepNumber[1] = 3;
            SetTimeBetweenAnimation(50);
            mSpriteSize = new Point(16, 16);
            mAnimationStartArray = new Rectangle[2];
            mAnimationStartArray[0] = new Rectangle(80, 64, 16, 16);
            mAnimationStartArray[1] = new Rectangle(96, 64, 16, 16);
            mDrawnRectangle = mAnimationStartArray[0];
            mIndexDrawnSprite = 0;

            SetBoundingBoxSize(new Vector2(8, 8));
            //DefineBBPositionOffset(new Vector2(4, 4));
            mSpritePivotPoint = new Vector2(8, 8);

            mVerticalSpeed = new Speed(200);
            mVerticalSpeed.mAcceleration = 5;
            mVerticalSpeed.mAllowNegativeSpeed = true;
            mVerticalSpeed.mEvolveInPositiveNumber = false;
            mVerticalSpeed.SpeedToMax();
            mHorizontalSpeed = new Speed(250);
            mHorizontalSpeed.mAllowNegativeSpeed = true;
            if (!goingRight)
            {
                mHorizontalSpeed.mEvolveInPositiveNumber = false;
            }
            mHorizontalSpeed.SpeedToMax();
        }
            
        public override void CollisionEffect(Obstacle obst, CollisionWay way)
        {
            if (obst is Enemy)
            {
                (obst as Enemy).FireballKill();
                AddEvent(FireballExplosion);
            }
        }

        public override bool CanCollide(Obstacle other)
        {
            if (other is Mario || other is FireballBonus)
            {
                return false;
            }
            return true;
        }

        public bool FireballExplosion()
        {
            mTimeSleeping = 150;
            AddEventFront(this.DeleteObstacle);
            AddEventFront(this.SleepEvent);
            mIndexDrawnSprite = 1;
            return true;
        }
        
        private bool FireballBounce()
        {
            GameTime gt = ObstacleAccessor.Instance.mGameTime;
            if (gt.ElapsedGameTime.Milliseconds != 0)
            {
                mMovementInPixel = new Vector2(mHorizontalSpeed.mCurrentSpeed * gt.ElapsedGameTime.Milliseconds / 1000.0f, mVerticalSpeed.mCurrentSpeed * gt.ElapsedGameTime.Milliseconds / 1000.0f);
            }
            CollisionDetection();
            if (mHorizontalCollision)
            {
                AddEvent(FireballExplosion);
            }
            if (!mIsBouncing)
            {
                if (mVerticalCollision)
                {
                    mVerticalSpeed.mSpeedLimit = 100;
                    mVerticalSpeed.mEvolveInPositiveNumber = true;
                    mVerticalSpeed.SpeedToMax();
                    mIsBouncing = true;
                }
            }
            else
            {
                if (mVerticalCollision)
                {
                    mVerticalSpeed.mEvolveInPositiveNumber = true;
                    mVerticalSpeed.SpeedToMax();
                    mIsBouncing = true;
                }
                else
                {
                    mVerticalSpeed.SlowDown();
                }
            }
            
            mRotation += 1.0f;
            return false;
        }
    }
}
