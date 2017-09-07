using Microsoft.Xna.Framework;
using SuperMarioBros.PhysicComponent;
namespace SuperMarioBros.DisplayComponent
{
    public class Character : DrawableObstacle
    {
        private Vector2 _mMoveVector;

        public Vector2 mMoveVector { get => _mMoveVector; set => _mMoveVector = value; }
        public bool mIsFalling { get => _mIsFalling; set => _mIsFalling = value; }

        public Speed mVerticalSpeed;
        public Speed mHorizontalSpeed;
        private bool _mIsFalling;

        public Character ()
        {

        }

        public new void Update()
        {
            base.Update();
            mPosition = new Vector2(mPosition.X + mMoveVector.X * mHorizontalSpeed.mCurrentSpeed, mPosition.Y + mMoveVector.Y * mHorizontalSpeed.mCurrentSpeed);
        }

        public void InverseInX()
        {
            _mMoveVector.X = -_mMoveVector.X;
        }

        public void InverseInY()
        {
            _mMoveVector.Y = -_mMoveVector.Y;
        }
    }
}
