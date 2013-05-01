using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.Main;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects
{
    public enum SimObjectType { PASSIVE, ACTIVE }

    public abstract class SimObject
    {
        private float mass;
        private SimObjectType simObjectType;
        protected Vector3 currPosition;
        protected Vector3 prevPosition;
        protected Vector3 currVelocity;
        protected Vector3 resultantForce;

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public SimObjectType SimObjectType
        {
            get { return simObjectType; }
            set { simObjectType = value; }
        }

        public Vector3 CurrPosition
        {
            get { return currPosition; }
            set { currPosition = value; }
        }

        public float CurrPositionX
        {
            get { return currPosition.X; }
            set { currPosition.X = value; }
        }

        public float CurrPositionY
        {
            get { return currPosition.Y; }
            set { currPosition.Y = value; }
        }

        public float CurrPositionZ
        {
            get { return currPosition.Z; }
            set { currPosition.Z = value; }
        }

        public Vector3 PrevPosition
        {
            get { return prevPosition; }
            set { prevPosition = value; }
        }

        public Vector3 CurrVelocity
        {
            get { return currVelocity; }
            set { currVelocity = value; }
        }

        public Vector3 ResultantForce
        {
            get { return resultantForce; }
            set { resultantForce = value; }
        }

        //-----------------------------------------------------------------

        public SimObject(float mass, SimObjectType simObjectType)
        {
            this.mass = mass;
            this.currPosition = Vector3.Zero;
            this.prevPosition = currPosition;
            this.currVelocity = Vector3.Zero;
            this.simObjectType = simObjectType;
        }

        public void ResetForces()
        {
            this.resultantForce = Vector3.Zero;
        }

        public abstract void Update(GameTime gameTime);
    }
}
