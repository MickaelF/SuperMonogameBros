using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperMarioBros.PhysicComponent;

namespace SuperMarioBros.LevelComponent
{
    public class CoinBonus : Bonus
    {
        private Vector2 mOrigin;
        private bool mJumping;
        public CoinBonus(Vector2 origin, Texture2D spriteSheet, bool jumping)
            : base(origin, spriteSheet)
        {
            mOrigin = origin;
            mPosition = new Vector2(mPosition.X, mPosition.Y - 16);
            mJumping = jumping;
            if (!mJumping)
            {
                mDrawnRectangle = new Rectangle(new Point(48, 113), mSpriteSize);
            }
            else
            {                
                mDrawnRectangle = new Rectangle(new Point(48, 128), mSpriteSize);
                mIsCollidable = false;
                mTopLeftStopFallPosition = mPosition;
                AddEvent(JumpUntilPosition);
                AddEvent(DeleteObstacle);
            }
            mAnimationStartArray = new Rectangle[1];
            mAnimationStartArray[0] = mDrawnRectangle;
            mIsAnimated = true;
            mSpriteAnimationStepNumber = new int[1];
            mSpriteAnimationStepNumber[0] = 3;
            SetTimeBetweenAnimation(50.0f);

            mVerticalSpeed = new Speed(300);
            mVerticalSpeed.mAcceleration = 10;
            mVerticalSpeed.mAllowNegativeSpeed = true;
            mVerticalSpeed.mEvolveInPositiveNumber = true;
            mVerticalSpeed.SpeedToMax();
        }
    }
}
