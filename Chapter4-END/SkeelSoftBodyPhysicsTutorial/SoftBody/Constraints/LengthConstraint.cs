using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.Constraints
{
    public sealed class LengthConstraint : Constraint
    {
        private float length;
        private SimObject simObj1;
        private SimObject simObj2;

        public float Length
        {
            get { return length; }
            set { length = value; }
        }

        //------------------------------------------------------------

        public LengthConstraint(float length, SimObject simObj1, SimObject simObj2)
        {
            this.length = length;
            this.simObj1 = simObj1;
            this.simObj2 = simObj2;
        }

        Vector3 direction;
        float currentLength;
        Vector3 moveVector;
        public void SatisfyConstraint()
        {
            //calculate direction
            direction = simObj2.CurrPosition - simObj1.CurrPosition;

            //calculate current length
            currentLength = direction.Length();

            //check for zero vector
            if (direction != Vector3.Zero)
            {
                //normalize direction vector
                direction.Normalize();

                //move to goal positions
                moveVector = 0.5f * (currentLength - length) * direction;
                simObj1.CurrPosition += moveVector;
                simObj2.CurrPosition += -moveVector;
            }
        }

    }
}
