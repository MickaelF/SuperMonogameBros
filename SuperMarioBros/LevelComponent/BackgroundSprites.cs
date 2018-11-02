using Microsoft.Xna.Framework;
using SuperMarioBros.DisplayComponent;

namespace SuperMarioBros.LevelComponent
{
    public class BackgroundSprites : DrawableObstacle
    {
        public BackgroundSprites(LevelLoader.ObjectType objectType, Point startPosition)
        {
            mDepthLevel = 1;
            switch (objectType)
            {
                case LevelLoader.ObjectType.Bushes:
                    mSpriteSize = new Point(32, 16);
                    mPosition = new Vector2(startPosition.X, startPosition.Y);
                    mDrawnRectangle = new Rectangle(new Point(16, 112), mSpriteSize);
                    break;
                case LevelLoader.ObjectType.Clouds:
                    mSpriteSize = new Point(32, 32);
                    mPosition = new Vector2(startPosition.X, startPosition.Y);
                    mDrawnRectangle = new Rectangle(new Point(16, 128), mSpriteSize);
                    break;
                case LevelLoader.ObjectType.LargeHills:
                    mSpriteSize = new Point(80, 48);
                    mPosition = new Vector2(startPosition.X, startPosition.Y - mSpriteSize.Y + 16);
                    mDrawnRectangle = new Rectangle(new Point(16, 192), mSpriteSize);
                    break;
                case LevelLoader.ObjectType.SmallHills:
                    mSpriteSize = new Point(48, 32);
                    mPosition = new Vector2(startPosition.X, startPosition.Y - mSpriteSize.Y + 16);
                    mDrawnRectangle = new Rectangle(new Point(16, 192), mSpriteSize);
                    break;
            }
            SetCollidable(false);
            //InitObstacle();
        }
    }
}
