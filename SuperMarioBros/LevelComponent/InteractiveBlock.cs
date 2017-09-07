using SuperMarioBros.DisplayComponent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace SuperMarioBros.LevelComponent
{
    public class InteractiveBlock : DrawableObstacle
    {
        private int mStayAtZeroStepAnimation;

        public InteractiveBlock(int contextLevel, int interactiveBlockType, Rectangle position)
        {
            mStayAtZeroStepAnimation = 0;
            mSpriteSize = new Point(16, 16);
            mPosition = new Vector2(position.Location.X, position.Location.Y);
            mSize = new Vector2(position.Width, position.Height);
            mSpriteAnimationStepNumber = new int[1];
            mAnimationStartArray = new Rectangle[1];
            Point positionSprite = new Point(0, 0);
            if (interactiveBlockType == 0)
            {
                // C'est un point d'interrogation
                mSpriteAnimationStepNumber[0] = 3;
                mIsAnimated = true;
                SetTimeBetweenAnimation(200);
                positionSprite.X = 384;
            }
            else if(interactiveBlockType == 1)
            {
                // C'est une brique cassable
                mSpriteAnimationStepNumber[0] = 1;
                positionSprite.X = 16;
            }

            switch (contextLevel)
            {
                case 0: // Overworld
                    positionSprite.Y = 0;
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);
                    break;
                case 1: // Underworld
                    positionSprite.Y = 32;
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);
                    break;
                case 2: // Castle
                    positionSprite.Y = 64;
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);
                    break;
                case 3: // Underwater
                    positionSprite.Y = 96;
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);
                    break;
            }

            mIndexDrawnSprite = 0;
        }

        public override void Update()
        {
            if (mIsAnimated && mTimer.ElapsedMilliseconds > mNextAnimationTimeLimit)
            {
                mTimer.Restart();
                if (mStayAtZeroStepAnimation >= 4 || mCurrentAnimationStepDrawn != 0)
                {
                    mStayAtZeroStepAnimation = 0;
                    mCurrentAnimationStepDrawn++;
                    if (mCurrentAnimationStepDrawn >= mSpriteAnimationStepNumber[mIndexDrawnSprite])
                    {
                        mCurrentAnimationStepDrawn = 0;
                    }

                    mDrawnRectangle = mAnimationStartArray[mIndexDrawnSprite];
                    mDrawnRectangle.X += mCurrentAnimationStepDrawn * mSpriteSize.X;
                }
                mStayAtZeroStepAnimation++;
            }
        }

        /*public new void CollisionEffect(Obstacle obst, CollisionDirection direction = CollisionDirection.Unknown)
        {
            if (obst is Player)
            {
                if (direction == CollisionDirection.Top)
                {
                    mNbAnimationStep[0] = 5;
                    SetCurrentAnimationStep(4);
                    mIsAnimated = false;
                }
            }
        }*/
    }
}
