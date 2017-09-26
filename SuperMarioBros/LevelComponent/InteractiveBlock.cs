using SuperMarioBros.DisplayComponent;
using SuperMarioBros.PhysicComponent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace SuperMarioBros.LevelComponent
{
    public class InteractiveBlock : Character
    {
        private int mStayAtZeroStepAnimation;
        private bool mIsCollided;
        public ITEM_TYPE mContent;
        private bool mIsAQuestionMarkBlock;
        private Rectangle mEmptyBlockRectangleDrawing;
        private Vector2 mTopLeftOrigin;

        private Character[] mParticlesBreakableBrick;
        private bool mIsDestroyed;


        public enum ITEM_TYPE
        {
            NONE,
            COIN,
            ONE_UP,
            STAR,
            RED_MUSHROOM
        }

        public InteractiveBlock(int contextLevel, bool questionMarkBlock, Rectangle position, ITEM_TYPE contains = ITEM_TYPE.NONE)
        {
            mIsDestroyed = false;
            mStayAtZeroStepAnimation = 0;
            mContent = contains;
            mIsAQuestionMarkBlock = questionMarkBlock;
            mVerticalSpeed = new Speed(100);
            mVerticalSpeed.mAcceleration = 10;
            mVerticalSpeed.mNegativeSpeed = true;
            mIsCollided = false;
            mSpriteSize = new Point(16, 16);
            mPosition = new Vector2(position.Location.X, position.Location.Y);
            mTopLeftOrigin = mPosition;
            mSize = new Vector2(position.Width, position.Height);
            mSpriteAnimationStepNumber = new int[1];
            mAnimationStartArray = new Rectangle[1];
            Point positionSprite = new Point(16, 0);
            if (mIsAQuestionMarkBlock)
            {
                // C'est un point d'interrogation
                mSpriteAnimationStepNumber[0] = 3;
                mIsAnimated = true;
                SetTimeBetweenAnimation(200);
                positionSprite.Y = 64;
            }
            else
            {
                // C'est une brique cassable
                mSpriteAnimationStepNumber[0] = 1;
                positionSprite.Y = 16;
            }

            switch (contextLevel)
            {
                case 0: // Overworld
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);
                    mEmptyBlockRectangleDrawing = new Rectangle(64, 64, mSpriteSize.X, mSpriteSize.Y);
                    break;
                case 1: // Underworld
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);
                    mEmptyBlockRectangleDrawing = new Rectangle(384 + 3 * mSpriteSize.X, positionSprite.Y, mSpriteSize.X, mSpriteSize.Y);
                    break;
                case 2: // Castle
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);
                    mEmptyBlockRectangleDrawing = new Rectangle(384 + 3 * mSpriteSize.X, positionSprite.Y, mSpriteSize.X, mSpriteSize.Y);
                    break;
                case 3: // Underwater
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);
                    mEmptyBlockRectangleDrawing = new Rectangle(384 + 3 * mSpriteSize.X, positionSprite.Y, mSpriteSize.X, mSpriteSize.Y);
                    break;
            }

            mIndexDrawnSprite = 0;
            mDrawnRectangle = mAnimationStartArray[mIndexDrawnSprite];
        }

        public override void Update(GameTime gameTime)
        {
            mMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
            if (mIsAnimated && mMilliseconds > mNextAnimationTimeLimit)
            {
                mMilliseconds = 0;
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
            else if (mIsFalling)
            {
                if (gameTime.ElapsedGameTime.Milliseconds != 0)
                {
                    mMovementInPixel = new Vector2(0.0f, mVerticalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                    mVerticalSpeed.SlowDown();
                }
                if (mVerticalSpeed.mCurrentSpeed < 0.0f)
                {
                    // Block en phase de chute. Stopper la chute au bon moment.
                    float lengthLeft = (mPosition - mTopLeftOrigin).Length();
                    if (lengthLeft < System.Math.Abs(mMovementInPixel.Y))
                    {
                        mMovementInPixel.Y = System.Math.Sign(mMovementInPixel.Y) * lengthLeft;
                    }
                    if(lengthLeft == 0.0f)
                    {
                        mIsFalling = false;
                    }
                }
                base.Update(gameTime);
            }
            else if (mIsDestroyed)
            {
                if (gameTime.ElapsedGameTime.Milliseconds != 0)
                {
                    foreach(Character c in mParticlesBreakableBrick)
                    {
                        c.mMovementInPixel = new Vector2(0.50f, c.mVerticalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                        c.mVerticalSpeed.SlowDown();
                        c.mRotation += 1.0f;
                        c.Update(gameTime);
                    }
                }           
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (mIsDestroyed)
            {
                foreach(DrawableObstacle draw in mParticlesBreakableBrick)
                {
                    draw.Draw(spriteBatch);
                }
            }
            else
            {
                base.Draw(spriteBatch);
            }
        }

        public override void CollisionEffect()
        {
            if (!mIsCollided)
            {
                mIsAnimated = false;
                mDrawnRectangle = mEmptyBlockRectangleDrawing;
                mIsCollided = true;
                if (mContent == ITEM_TYPE.NONE)
                {
                    // L'objet est détruit.
                    mIsDestroyed = true;
                    // On ne peut plus se prendre l'objet.
                    mIsCollidable = false; 
                    mParticlesBreakableBrick = new Character[4];
                    for (int i = 0; i < 4; ++i)
                    { 
                        mParticlesBreakableBrick[i] = new Character();
                        mParticlesBreakableBrick[i].mPosition = mPosition;
                        mParticlesBreakableBrick[i].mSpriteSheet = mSpriteSheet;
                        mParticlesBreakableBrick[i].mSpriteSize = new Point(16, 16);
                        mParticlesBreakableBrick[i].mSpriteAnimationStepNumber = new int[1];
                        mParticlesBreakableBrick[i].mSpriteAnimationStepNumber[0] = 1;

                        mParticlesBreakableBrick[i].mAnimationStartArray = new Rectangle[1];
                        if (i % 2 == 0)
                        {
                            mParticlesBreakableBrick[i].mDrawnRectangle = new Rectangle(new Point(32, 16), mParticlesBreakableBrick[i].mSpriteSize);
                            mParticlesBreakableBrick[i].mVerticalSpeed = new Speed(100);
                            mParticlesBreakableBrick[i].mMoveVector = new Vector2((i > 0) ?1.0f : -1.0f, -1.0f);
                        }
                        else
                        {
                            mParticlesBreakableBrick[i].mDrawnRectangle = new Rectangle(new Point(48, 16), mSpriteSize);
                            mParticlesBreakableBrick[i].mVerticalSpeed = new Speed(200);
                            mParticlesBreakableBrick[i].mMoveVector = new Vector2((i > 1) ? 1.0f : -1.0f, -1.0f);
                        }
                        mParticlesBreakableBrick[i].mVerticalSpeed.mNegativeSpeed = true;
                        mParticlesBreakableBrick[i].mVerticalSpeed.SpeedToMax();
                        mParticlesBreakableBrick[i].mVerticalSpeed.mAcceleration = 10;
                    }
                }
                else
                {
                    mIsFalling = true;
                    mMoveVector = new Vector2(0.0f, -1.0f);
                    mVerticalSpeed.SpeedToMax();
                }
                switch (mContent)
                {
                    case ITEM_TYPE.ONE_UP:
                        break;
                    case ITEM_TYPE.RED_MUSHROOM:
                        break;
                    case ITEM_TYPE.COIN:
                        break;
                }
            }
        }
    }
}
