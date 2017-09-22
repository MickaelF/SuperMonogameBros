using Microsoft.Xna.Framework;

namespace SuperMarioBros.PhysicComponent
{
    public class Obstacle
    {
        static int sNextId = 0;
        public enum CollisionDirection
        {
            Unknown = 0,
            Left,
            Top,
            Right,
            Bottom
        }

        private Vector2 _mPosition;
        private Vector2 _mSize;
        private int cId = sNextId++;
        public Vector2 mPosition { get => _mPosition; set => _mPosition = value; }
        public Vector2 mSize { get => _mSize; set => _mSize = value; }


        public Obstacle()
        {
        }

        public void CollisionEffect(Obstacle obst, CollisionDirection direction = CollisionDirection.Unknown)
        {

        }

        public bool Intersect(Rectangle other)
        {
            float intersectionLeft = System.Math.Max(mPosition.X, other.Location.X);
            float intersectionRight = System.Math.Min(mPosition.X + mSize.X, other.Location.X + other.Size.X);
            float intersectionTop = System.Math.Max(mPosition.Y, other.Location.Y);
            float intersectionBottom = System.Math.Min(mPosition.Y + mSize.Y, other.Location.Y + other.Size.Y);
            if ((intersectionLeft < intersectionRight) && (intersectionTop < intersectionBottom))
            {
                return true;
            }
            return false;
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

        public bool Intersect(ref Ray2D ray)
        {
           
            if(ray.mDirection == Vector2.Zero)
            {
                return false;
            }
            Vector2[] bounds = new Vector2[2];
            bounds[0] = mPosition;
            bounds[1] = mPosition + mSize;
            float tmin = (bounds[ray.mSignInvDirection[0]].X - ray.mPosition.X) * ray.mInvDirection.X;
            float tmax = (bounds[1 - ray.mSignInvDirection[1]].X - ray.mPosition.X) * ray.mInvDirection.X;
            float tymin = (bounds[ray.mSignInvDirection[0]].Y - ray.mPosition.Y) * ray.mInvDirection.Y;
            float tymax = (bounds[1 - ray.mSignInvDirection[1]].Y - ray.mPosition.Y) * ray.mInvDirection.Y;

            if ((tmin > tymax) || (tymin > tmax))
            {
                return false; 
            }
            else
            {
                tmin = MathHelper.Max(tmin, tymin);
                tmax = MathHelper.Min(tmax, tymax);
                
                float coefficient = (tmax < tmin) ? tmax : tmin;

                Vector2 intersection = ray.mPosition + ray.mDirection * coefficient; 
                if (ray.IsIntersectionFound())
                {
                    if ((intersection - ray.mPosition).Length() < ray.mIntersectionDistance)
                    {
                        ray.mIdObstacleIntersected = cId;
                        ray.mIntersection = intersection;
                        return true;
                    }
                }
                else
                {
                    ray.mIdObstacleIntersected = cId;
                    ray.mIntersection = intersection;
                    return true;
                }
            }
            return false;
        }

        public bool Contains(Vector2 pos)
        {
            if ((pos.X > mPosition.X && pos.X < mPosition.X + mSize.X)
                && (pos.Y > mPosition.Y && pos.Y < mPosition.Y + mSize.Y))
            {
                return true;
            }
            return false;
        }
    }
}
