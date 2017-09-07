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
        public bool mIsHorizontalyFlipped { get => _mIsHorizontalyFlipped; set => _mIsHorizontalyFlipped = value; }
        public Point mSpriteSize { get => _mSpriteSize; set => _mSpriteSize = value; }
        public Texture2D mSpriteSheet { get => _mSpriteSheet; set => _mSpriteSheet = value; }
        protected float mNextAnimationTimeLimit { get => _mNextAnimationTimeLimit; private set => _mNextAnimationTimeLimit = value; }
        public int mIndexDrawnSprite {
            get => _mIndexDrawnSprite;
            set
            {
                _mIndexDrawnSprite = value;
                mDrawnRectangle = mAnimationStartArray[value];
            }
        }

        //! If the object has an animation.
        private bool _mIsAnimated;
        //! If the object is drawn reversed.
        private bool _mIsHorizontalyFlipped;
        //! Currently drawn rectangle in the spite sheet.
        protected Rectangle mDrawnRectangle;
        //! Index of the currently drawn sprite animation.
        private int _mIndexDrawnSprite;
        //! Number of animation step in each sprite animation contained in the spite sheet.
        private int[] _mSpriteAnimationStepNumber;
        //! Current step drawn.
        protected int mCurrentAnimationStepDrawn;
        //! Array containing the start of different animation on the sprite sheet.
        private Rectangle[] _mAnimationStartArray;
        //! Timer clock for animations.
        protected Stopwatch mTimer;
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

        public DrawableObstacle()
        {
            mTimer = new Stopwatch();
            mTimer.Start();
            mScalling = 1.0f;
        }

        public void SetTimeBetweenAnimation(float time)
        {
            mNextAnimationTimeLimit = time;
        }
        
        public virtual void Update()
        {
            if (mIsAnimated && mTimer.ElapsedMilliseconds > mNextAnimationTimeLimit)
            {
                mTimer.Restart();
                mCurrentAnimationStepDrawn++; 
                if(mCurrentAnimationStepDrawn >= mSpriteAnimationStepNumber[mIndexDrawnSprite])
                {
                    mCurrentAnimationStepDrawn = 0;
                }
                mDrawnRectangle = mAnimationStartArray[mIndexDrawnSprite];
                mDrawnRectangle.X += mCurrentAnimationStepDrawn * mSpriteSize.X;
            }
        }
        public void TextureFaceLeft()
        {
            mIsHorizontalyFlipped = true;
        }

        public void TextureFaceRight()
        {
            mIsHorizontalyFlipped = false;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (mSpriteSheet != null)
            {
                if (mAnimationStartArray.Length < 1)
                {
                    spriteBatch.Draw(mSpriteSheet, mPosition, Color.White);
                }
                else
                {
                    if (mIsHorizontalyFlipped)
                    {
                        spriteBatch.Draw(mSpriteSheet, mPosition, mDrawnRectangle, Color.White, mRotation, Vector2.Zero, mScalling, SpriteEffects.FlipHorizontally, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(mSpriteSheet, mPosition, mDrawnRectangle, Color.White, mRotation, Vector2.Zero, mScalling, SpriteEffects.None, 0);
                    }
                }
            }
        }


    }
}
