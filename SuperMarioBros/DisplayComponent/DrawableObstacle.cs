using SuperMarioBros.PhysicComponent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace SuperMarioBros.DisplayComponent
{
    public class DrawableObstacle : Obstacle
    {
        public bool mIsAnimated { get => _mIsAnimated; set => _mIsAnimated = value; }
        public Rectangle[] mAnimationStartArray { get => _mAnimationStartArray; set => _mAnimationStartArray = value; }
        public int[] mSpriteAnimationStepNumber { get => _mSpriteAnimationStepNumber; set => _mSpriteAnimationStepNumber = value; }
        public float mScalling { get => _mScalling; set => _mScalling = value; }
        public float mRotation { get => _mRotation; set => _mRotation = value; }
        public bool mIsHorizontalyFlipped { get => _mIsHorizontalyFlipped; private set => _mIsHorizontalyFlipped = value; }
        public bool mIsVerticalyFlipped { get => _mIsVerticalyFlipped; private set => _mIsVerticalyFlipped = value; }
        public Point mSpriteSize { get => _mSpriteSize; set => _mSpriteSize = value; }
        public Texture2D mSpriteSheet { get => _mSpriteSheet; set => _mSpriteSheet = value; }
        protected float mNextAnimationTimeLimit { get => _mNextAnimationTimeLimit; private set => _mNextAnimationTimeLimit = value; }
        public int mIndexDrawnSprite {
            get => _mIndexDrawnSprite;
            set
            {
                if (value != _mIndexDrawnSprite)
                {
                    _mIndexDrawnSprite = value;
                    mDrawnRectangle = mAnimationStartArray[value];
                }
            }
        }

        public Vector2 mSpritePivotPoint { get => _mSpritePivotPoint; set => _mSpritePivotPoint = value; }
        private int mStayAtZeroStepAnimation;

        //! Z-level of the drawable
        protected int mDepthLevel;
        //! If the object has an animation.
        private bool _mIsAnimated;
        //! If the object is drawn horizontaly reversed.
        private bool _mIsHorizontalyFlipped;
        //! If the object is drawn verticaly reversed.
        private bool _mIsVerticalyFlipped;
        //! Currently drawn rectangle in the spite sheet.
        public Rectangle mDrawnRectangle;
        //! Index of the currently drawn sprite animation.
        private int _mIndexDrawnSprite;
        //! Number of animation step in each sprite animation contained in the spite sheet.
        private int[] _mSpriteAnimationStepNumber;
        //! Current step drawn.
        protected int mCurrentAnimationStepDrawn;
        //! Array containing the start of different animation on the sprite sheet.
        private Rectangle[] _mAnimationStartArray;
        private bool _mIsDisplayed;

        //! Time to surpass for the animation to trigger.
        private float _mNextAnimationTimeLimit;
        //! Sprite sheet texture
        private Texture2D _mSpriteSheet;
        //! Rotation applied to the texture.
        private float _mRotation;
        //! Scalling applied to the texture.
        private float _mScalling;
        //! Sprite size in pixels.
        private Point _mSpriteSize;
        //! Milliseconds pass since last animation
        protected int mMilliseconds;
        //! If the block is meant to be displayed, true. False otherwise.
        public bool mIsDisplayed { get => _mIsDisplayed; private set => _mIsDisplayed = value; }
        //! Pivot point of the sprite (local coordinate)
        private Vector2 _mSpritePivotPoint;
        //! Offset between animation step in sprite sheet. 
        public int mAnimationOffset;

        //! Event called when there is no event in the event list
        protected Event mPrimaryDrawEvent;

        public DrawableObstacle()
        {
            mScalling = 1.0f;
            mStayAtZeroStepAnimation = 0;
            mMilliseconds = 0;
            mIsDisplayed = true;
            mSpritePivotPoint = Vector2.Zero;
            mDepthLevel = 0;
            mAnimationOffset = 0;
            mPrimaryDrawEvent = Animate;
        }

        public override void Update(GameTime gameTime)
        {
            mPrimaryDrawEvent();
            base.Update(gameTime);
        }

        public void SetTimeBetweenAnimation(float time)
        {
            mNextAnimationTimeLimit = time;
        }

        public void SetDisplayed(bool state)
        {
            mIsDisplayed = state;
            if (mIsInitialized)
            {
                if (mIsDisplayed)
                {
                    ObstacleAccessor.Instance.AddDrawnObstacle(cId);
                }
                else
                {
                    ObstacleAccessor.Instance.RemoveDrawnObstacle(cId);
                }
            }            
        }

        public bool Animate()
        {
            mMilliseconds += ObstacleAccessor.Instance.mGameTime.ElapsedGameTime.Milliseconds;
            if (mIsAnimated && mSpriteAnimationStepNumber[mIndexDrawnSprite] > 1 && mMilliseconds > mNextAnimationTimeLimit)
            {
                mMilliseconds = 0;
                mCurrentAnimationStepDrawn = (mCurrentAnimationStepDrawn + 1 >= mSpriteAnimationStepNumber[mIndexDrawnSprite]) ? 0 : mCurrentAnimationStepDrawn + 1;

                mDrawnRectangle = mAnimationStartArray[mIndexDrawnSprite];
                mDrawnRectangle.X += mCurrentAnimationStepDrawn * (mSpriteSize.X + mAnimationOffset);
            }
            return false;
        }

        public void TextureFaceLeft()
        {
            mIsHorizontalyFlipped = true;
        }

        public void TextureFaceRight()
        {
            mIsHorizontalyFlipped = false;
        }

        public void TextureVerticalyReversed()
        {
            mIsVerticalyFlipped = !mIsVerticalyFlipped;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (mIsDisplayed)
            {
                if (mSpriteSheet != null)
                {
                    if (mIsHorizontalyFlipped)
                    {
                        spriteBatch.Draw(mSpriteSheet, mPosition, mDrawnRectangle, Color.White, mRotation, mSpritePivotPoint, mScalling, SpriteEffects.FlipHorizontally, mDepthLevel);
                    }
                    else if (mIsVerticalyFlipped)
                    {
                        spriteBatch.Draw(mSpriteSheet, mPosition, mDrawnRectangle, Color.White, mRotation, mSpritePivotPoint, mScalling, SpriteEffects.FlipVertically, mDepthLevel);
                    }
                    else
                    {
                        spriteBatch.Draw(mSpriteSheet, mPosition, mDrawnRectangle, Color.White, mRotation, mSpritePivotPoint, mScalling, SpriteEffects.None, mDepthLevel);
                    }
                }
            }
        }

        public bool AnimationWithFirstSpriteShowingMoreTime()
        {
            mMilliseconds += ObstacleAccessor.Instance.mGameTime.ElapsedGameTime.Milliseconds;
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
            return false;
        }
    }
}
