using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.ForceGenerators
{
    public sealed class Gravity : ForceGenerator
    {
        private Vector3 acceleration;

        public Vector3 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        public float AccelerationX
        {
            get { return acceleration.X; }
            set { acceleration.X = value; }
        }

        public float AccelerationY
        {
            get { return acceleration.Y; }
            set { acceleration.Y = value; }
        }

        public float AccelerationZ
        {
            get { return acceleration.Z; }
            set { acceleration.Z = value; }
        }

        //---------------------------------------------------------------------

        public Gravity()
            : this(new Vector3(0, -9.81f, 0)) { }

        public Gravity(Vector3 acceleration)
            : base()
        {
            this.acceleration = acceleration;
        }

        public void ApplyForce(SimObject simObject)
        {
            simObject.ResultantForce += simObject.Mass * acceleration;
        }
    }
}
