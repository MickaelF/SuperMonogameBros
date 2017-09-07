using Microsoft.Xna.Framework;

namespace SuperMarioBros.PhysicComponent
{
    public class Obstacle
    {
        public enum CollisionDirection
        {
            Unknown = 0,
            Left,
            Top,
            Right,
            Bottom
        }

        private Vector2 _mPosition;
        public Vector2 mPosition { get => _mPosition; set => _mPosition = value; }
        private Vector2 _mSize;
        public Vector2 mSize { get => _mSize; set => _mSize = value; }


        public Obstacle()
        { 
        }

        public void CollisionEffect(Obstacle obst, CollisionDirection direction = CollisionDirection.Unknown)
        {

        }

        public bool Intersect(Obstacle other)
        {
            Rectangle rect = new Rectangle();
            return Intersect(other, ref rect);
        }

        public bool Intersect(Obstacle other, ref Rectangle intersection)
        {
            float intersectionLeft = System.Math.Max(mPosition.X, other.mPosition.X);
            float intersectionRight = System.Math.Min(mPosition.X + mSize.X, other.mPosition.X + other.mSize.X);
            float intersectionTop = System.Math.Max(mPosition.Y, other.mPosition.Y);
            float intersectionBottom = System.Math.Min(mPosition.Y + mSize.Y, other.mPosition.Y + other.mSize.Y);
            if ((intersectionLeft < intersectionRight) && (intersectionTop < intersectionBottom))
            {
                intersection = new Rectangle((int)intersectionLeft, (int)intersectionTop, (int)(intersectionRight - intersectionLeft), (int)(intersectionBottom - intersectionTop));
                return true;
            }
            return false;
        }

        public bool Intersect(Ray2D ray)
        {
            if (Contains(ray.mPosition))
            {
                return true;
            }

            return false;
        }

        public bool Contains(Vector2 pos)
        {
            if ((pos.X > mPosition.X && pos.X < mPosition.X + mSize.X)
                && (pos.Y > mPosition.Y && pos.Y < mPosition.X + mSize.Y))
            {
                return true;
            }
            return false;
        }
    }
}
