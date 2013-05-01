using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.Constraints
{
    public sealed class PointConstraint : Constraint
    {
        private Vector3 point;
        private SimObject simObject;

        public Vector3 Point
        {
            get { return point; }
            set { point = value; }
        }

        public float PointX
        {
            get { return point.X; }
            set { point.X = value; }
        }

        public float PointY
        {
            get { return point.Y; }
            set { point.Y = value; }
        }

        public float PointZ
        {
            get { return point.Z; }
            set { point.Z = value; }
        }

        public SimObject SimModel
        {
            get { return simObject; }
            set { simObject = value; }
        }

        //------------------------------------------------------

        public PointConstraint(Vector3 point, SimObject simObject)
        {
            this.point = point;
            this.simObject = simObject;
        }

        public void SatisfyConstraint()
        {
            //move to goal position
            simObject.CurrPosition = point;
        }
    }
}
