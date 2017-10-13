using SuperMarioBros.DisplayComponent;
using SuperMarioBros.PhysicComponent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace SuperMarioBros.LevelComponent
{
    public class InteractiveBlock : MovableObstacle
    {
        private int mStayAtZeroStepAnimation;
        private bool mIsCollided;
        public ITEM_TYPE mContent;
        private bool mIsAQuestionMarkBlock;
        private bool mCanCreateBonus;
        private Rectangle mEmptyBlockRectangleDrawing;
        private Vector2 mTopLeftOrigin;
                
        public enum ITEM_TYPE
        {
            NONE,
            COIN,
            ONE_UP,
            STAR,
            RED_MUSHROOM, 
            FLOWER
        }

        public InteractiveBlock(int contextLevel, bool questionMarkBlock, Rectangle position, ITEM_TYPE contains = ITEM_TYPE.NONE)
        {
            mCanCreateBonus = true;
            mStayAtZeroStepAnimation = 0;
            mContent = contains;
            mIsAQuestionMarkBlock = questionMarkBlock;
            mVerticalSpeed = new Speed(100);
            mVerticalSpeed.mAcceleration = 10;
            mVerticalSpeed.mAllowNegativeSpeed = true;
            mMoveVector = new Vector2(0.0f, -1.0f);
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
                    if (lengthLeft == 0.0f)
                    {
                        mIsFalling = false;
                        mCanCreateBonus = true;
                    }
                }
                if (mCanCreateBonus && mIsCollided)
                {
                    switch(mContent)
                    {
                        case ITEM_TYPE.ONE_UP:
                            new MushroomBonus(true, mPosition, mSpriteSheet);
                            break;
                        case ITEM_TYPE.RED_MUSHROOM:
                            new MushroomBonus(false, mPosition, mSpriteSheet);
                            break;
                        case ITEM_TYPE.FLOWER:
                            new FlowerBonus(mPosition, mSpriteSheet);
                            break;
                        case ITEM_TYPE.NONE:
                            mIsCollided = false;
                            break;
                    }
                }
                base.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void CollisionEffect(Obstacle obst, CollisionWay way)
        {
            if (!mIsCollided && obst is Mario && way == CollisionWay.BELOW)
            {
                mIsAnimated = false;
                mIsCollided = true;
                if (mContent == ITEM_TYPE.NONE && (obst as Mario).mMarioState != Mario.MarioState.SMALL)
                {
                    // On ne peut plus se prendre l'objet.
                    mIsCollidable = false;

                    new BreakableBrickDestroyAnimation(mPosition, mSpriteSheet);
                    ObstacleAccessor.Instance.Remove(this);
                }
                else
                {
                    mIsFalling = true;
                    mVerticalSpeed.SpeedToMax();
                    mCanCreateBonus = false;

                    if(mContent != ITEM_TYPE.NONE)
                    {
                        mDrawnRectangle = mEmptyBlockRectangleDrawing;
                    }
                    switch (mContent)
                    {
                        case ITEM_TYPE.ONE_UP:
                            break;
                        case ITEM_TYPE.RED_MUSHROOM:
                            Mario.MarioState state = (obst as Mario).mMarioState;
                            if (state == Mario.MarioState.BIG || state == Mario.MarioState.FLOWER)
                            {
                                mContent = ITEM_TYPE.FLOWER;
                            }
                            break;
                        case ITEM_TYPE.COIN:
                            (obst as Mario).CoinUp();
                            new CoinBonus(mPosition, mSpriteSheet, true);
                            break;
                    }
                }               
            }
        }
    }
}
