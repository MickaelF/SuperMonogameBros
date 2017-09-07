using SuperMarioBros.DisplayComponent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace SuperMarioBros.LevelComponent
{
    public class TilableBlock : DrawableObstacle
    {
        private Rectangle mDestinationRectangle;
        public TilableBlock(Rectangle position)
        {
            mDestinationRectangle = position;
            mPosition = new Vector2(position.Location.X, position.Location.Y);
            mSize = new Vector2(position.Width, position.Height);
            mSpriteAnimationStepNumber = new int[1];
            mAnimationStartArray = new Rectangle[1];
            mSpriteAnimationStepNumber[0] = 1;

            mAnimationStartArray[0] = new Rectangle(Point.Zero, mSpriteSize);
           
            mIndexDrawnSprite = 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (mDestinationRectangle.Size != mSpriteSize)
            {
                spriteBatch.Draw(mSpriteSheet, mPosition, mDestinationRectangle, Color.White, mRotation, Vector2.Zero, mScalling, SpriteEffects.None, 0);
            }
            else
            {
                base.Draw(spriteBatch);
            }
        }
    }
}
