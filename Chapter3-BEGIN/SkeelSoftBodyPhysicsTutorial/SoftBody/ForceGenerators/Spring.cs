using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.ForceGenerators
{
    public sealed class Spring : ForceGenerator
    {
        private float stiffness;
        private float damping;
        private float restLength;
        private SimObject simObjectA;
        private SimObject simObjectB;

        public float Stiffness
        {
            get { return stiffness; }
            set { stiffness = value; }
        }

        public float Damping
        {
            get { return damping; }
            set { damping = value; }
        }

        public SimObject SimObjectA
        {
            get { return simObjectA; }
            set { simObjectA = value; }
        }

        public SimObject SimObjectB
        {
            get { return simObjectB; }
            set { simObjectB = value; }
        }

        //-----------------------------------------------------------

        public Spring(float stiffness, float damping, SimObject simObjectA, SimObject simObjectB)
            : this(stiffness, damping, simObjectA, simObjectB, (simObjectA.CurrPosition - simObjectB.CurrPosition).Length()) { }

        public Spring(float stiffness, float damping, SimObject simObjectA, SimObject simObjectB, float restLength)
            : base()
        {
            this.stiffness = stiffness;
            this.damping = damping;
            this.simObjectA = simObjectA;
            this.simObjectB = simObjectB;
            this.restLength = restLength;
        }

        private Vector3 direction;
        private float currLength;
        private Vector3 force;
        public void ApplyForce(SimObject simObject)
        {
            //get the direction vector
            direction = simObjectA.CurrPosition - simObjectB.CurrPosition;

            //check for zero vector
            if (direction != Vector3.Zero)
            {
                //get length
                currLength = direction.Length();

                //normalize
                direction.Normalize();

                //add spring force
                force = -stiffness * ((currLength - restLength) * direction);

                //add spring damping force
                force += -damping * Vector3.Dot(simObjectA.CurrVelocity - simObjectB.CurrVelocity, direction) * direction;

                //apply the equal and opposite forces to the objects
                simObjectA.ResultantForce += force;
                simObjectB.ResultantForce += -force;
            }
        }
    }
}
