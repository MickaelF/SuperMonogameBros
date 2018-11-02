using SuperMarioBros.DisplayComponent;
using SuperMarioBros.PhysicComponent;
using Microsoft.Xna.Framework;

namespace SuperMarioBros.LevelComponent
{
    public class InvisibleWall : MovableObstacle
    {
        public InvisibleWall(Vector2 position, Vector2 size)
        {
            SetDisplayed(false);
            mPosition = position;
            mSize = size;
            SetBoundingBoxSize(size);
            //InitObstacle();
        }

        public override bool CanCollide(Obstacle other)
        {
            if(other is Mario)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Move(Vector2 translation)
        {
            mPosition += translation;
        }
    }
}
