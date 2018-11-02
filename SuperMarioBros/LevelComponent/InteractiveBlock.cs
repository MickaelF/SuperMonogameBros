using SuperMarioBros.DisplayComponent;
using SuperMarioBros.PhysicComponent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace SuperMarioBros.LevelComponent
{
    public class InteractiveBlock : MovableObstacle
    {
        private bool mIsCollided;
        public ITEM_TYPE mContent;
        private bool mIsAQuestionMarkBlock;
                
        public enum ITEM_TYPE
        {
            NONE,
            COIN,
            ONE_UP,
            STAR,
            RED_MUSHROOM, 
            FLOWER
        }

        enum BRICK_STATE
        {
            COLLIDABLE, 
            USED
        }

        public InteractiveBlock(int contextLevel, bool questionMarkBlock, Rectangle position, ITEM_TYPE contains = ITEM_TYPE.NONE)
        {
            mPrimaryDrawEvent = base.AnimationWithFirstSpriteShowingMoreTime;
            mContent = contains;
            mIsAQuestionMarkBlock = questionMarkBlock;
            mVerticalSpeed = new Speed(100);
            mVerticalSpeed.mAcceleration = 10;
            mVerticalSpeed.mAllowNegativeSpeed = true;
            mMoveVector = new Vector2(0.0f, -1.0f);
            mIsCollided = false;
            mSpriteSize = new Point(16, 16);
            mPosition = new Vector2(position.Location.X, position.Location.Y);
            mSize = new Vector2(position.Width, position.Height);
            SetBoundingBoxSize(new Vector2(16, 16));
            mSpriteAnimationStepNumber = new int[2];
            mAnimationStartArray = new Rectangle[2];
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
            mSpriteAnimationStepNumber[1] = 1;

            switch (contextLevel)
            {
                case 0: // Overworld
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);

                    mAnimationStartArray[1] =  new Rectangle(64, 64, mSpriteSize.X, mSpriteSize.Y);
                    break;
                case 1: // Underworld
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);
                    mAnimationStartArray[1] = new Rectangle(384 + 3 * mSpriteSize.X, positionSprite.Y, mSpriteSize.X, mSpriteSize.Y);
                    break;
                case 2: // Castle
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);
                    mAnimationStartArray[1] = new Rectangle(384 + 3 * mSpriteSize.X, positionSprite.Y, mSpriteSize.X, mSpriteSize.Y);
                    break;
                case 3: // Underwater
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);
                    mAnimationStartArray[1] = new Rectangle(384 + 3 * mSpriteSize.X, positionSprite.Y, mSpriteSize.X, mSpriteSize.Y);
                    break;
            }
            mIndexDrawnSprite = 0;
            mDrawnRectangle = mAnimationStartArray[mIndexDrawnSprite];
            //InitObstacle();
        }

        private bool CreateBonus()
        {
            switch (mContent)
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
            return true;
        }

        public override void CollisionEffect(Obstacle obst, CollisionWay way)
        { 
            if (!mIsCollided && obst is Mario && way == CollisionWay.BELOW)
            {
                // Si la brique ne contient pas de bonus et que Mario n'est pas grand, alors il peut la casser.
                if (mContent == ITEM_TYPE.NONE && (obst as Mario).mMarioState != Mario.MarioState.SMALL)
                {
                    // On ne peut plus se prendre l'objet.
                    mIsCollidable = false;

                    new BreakableBrickDestroyAnimation(mPosition, mSpriteSheet);
                    ObstacleAccessor.Instance.Remove(this);
                }
                // Sinon, Mario fait sauter la brique. Et si elle contient un bonus, celui-ci sort une fois le saut terminé. Sauf pour la pièce.
                else
                {
                    mPrimaryDrawEvent = base.Animate;
                    mTopLeftStopFallPosition = mPosition;
                    AddEvent(base.JumpUntilPosition);
                    AddEvent(CreateBonus);
                    mIsCollided = true;
                    if (mContent != ITEM_TYPE.NONE)
                    {
                        mIndexDrawnSprite = 1;
                    }
                    switch (mContent)
                    {
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
