using SuperMarioBros.DisplayComponent;
using SuperMarioBros.PhysicComponent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperMarioBros.LevelComponent
{
    public class Enemy : MovableObstacle
    {
        protected enum EnemyState
        {
            WALK,
            CRUSHED
        }

        protected bool mCrushed;

        public Enemy(Point position)
        {
            mPosition = new Vector2(position.X, position.Y);
            mSize = new Vector2(16.0f, 16.0f);
            mSpriteSize = new Point(16, 16);
            mIsAnimated = true;
            SetTimeBetweenAnimation(50);
        }

        public void FireballKill()
        {
            AddEventFront(this.DeleteObstacle);
            AddEventFront(this.JumpAboveHeight);
            mHeightMin = 400f;
            SetCollidable(false);
            TextureVerticalyReversed();
        }

        public override void CollisionEffect(Obstacle obst, CollisionWay way)
        {
            if (obst is Mario)
            {
                switch (way)
                {
                    case CollisionWay.LEFT:
                    case CollisionWay.RIGHT:
                    case CollisionWay.BELOW:
                        (obst as Mario).MarioGotHit();
                        break;
                    case CollisionWay.ABOVE:
                        mCrushed = true;
                        mIndexDrawnSprite = (int)EnemyState.CRUSHED;
                        (obst as Mario).AddEvent((obst as Mario).Jump);
                        break;
                }
            }
        }
    }
}
