using Microsoft.Xna.Framework;

namespace SuperMarioBros.PhysicComponent
{
    public class Speed
    {
        private float _mSpeedLimit;
        private float _mAcceleration;
        private float _mCurrentSpeed;

        public float mSpeedLimit { get => _mSpeedLimit; set => _mSpeedLimit = value; }
        public float mAcceleration { get => _mAcceleration; set => _mAcceleration = value; }
        public float mCurrentSpeed { get => _mCurrentSpeed; private set => _mCurrentSpeed = value; }
        public bool mNegativeSpeed;

        public Speed(float speedLimit)
        {
            mCurrentSpeed = 0.0f;
            mSpeedLimit = speedLimit;
            mNegativeSpeed = false;
        }
        
        public void SpeedUp()
        {
            mCurrentSpeed = MathHelper.Clamp(mCurrentSpeed + mAcceleration, 0, mSpeedLimit);
        }

        public void SlowDown()
        {
            mCurrentSpeed = MathHelper.Clamp(mCurrentSpeed - mAcceleration, (mNegativeSpeed) ? -mSpeedLimit : 0, mSpeedLimit);
        }

        public void SpeedToMax()
        {
            mCurrentSpeed = mSpeedLimit;
        }

        public void SpeedToMin()
        {
            mCurrentSpeed = -mSpeedLimit;
        }

        public void Stop()
        {
            mCurrentSpeed = 0.0f;
        }

        public void DoubleSpeedLimit()
        {
            mSpeedLimit *= 2.0f;
        }

        public void SpeedLimitToNormal()
        {
            mSpeedLimit *= 0.5f;
        }
    }
}
