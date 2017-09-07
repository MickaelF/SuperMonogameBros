using SuperMarioBros.DisplayComponent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperMarioBros.LevelComponent
{
    public class PipeBlock : DrawableObstacle
    {
        private Rectangle mPipeRectangle;
        public PipeBlock(int contextLevel, Rectangle rect)
        {
            mPipeRectangle = rect;
            mPosition = new Vector2(rect.Location.X, rect.Location.Y);
            mSize = new Vector2(rect.Width, rect.Height);
            mSpriteSize = new Point(32, 16);
            mSpriteAnimationStepNumber = new int[1];
            mAnimationStartArray = new Rectangle[2];

            Point positionSprite = Point.Zero;
            switch (contextLevel)
            {
                case 0: // Overworld
                    positionSprite.Y = 128;
                    mAnimationStartArray[0] = new Rectangle(positionSprite, mSpriteSize);
                    positionSprite.Y = 128 + 16;
                    mAnimationStartArray[1] = new Rectangle(positionSprite, mSpriteSize);
                    break;
                case 1: // Underworld
                    positionSprite.Y = 160;
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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 drawnPosition = mPosition;
            spriteBatch.Draw(mSpriteSheet, drawnPosition, mAnimationStartArray[0], Color.White, mRotation, new Vector2(0.0f, 0.0f), mScalling, SpriteEffects.None, 0);
           
            while (drawnPosition.Y < mPipeRectangle.Top + mPipeRectangle.Height)
            {
                drawnPosition.Y += 16;
                spriteBatch.Draw(mSpriteSheet, drawnPosition, mAnimationStartArray[1], Color.White, mRotation, new Vector2(0.0f, 0.0f), mScalling, SpriteEffects.None, 0);
            }
        }
    }
}
