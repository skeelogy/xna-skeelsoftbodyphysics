using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.ForceGenerators
{
    public sealed class Medium : ForceGenerator
    {
        private float dragCoefficient;

        public float DragCoefficient
        {
            get { return dragCoefficient; }
            set { dragCoefficient = value; }
        }

        //--------------------------------------------------------

        public Medium(float dragCoefficient)
            : base()
        {
            this.dragCoefficient = dragCoefficient;
        }

        public void ApplyForce(SimObject simObject)
        {
            simObject.ResultantForce += - dragCoefficient * simObject.CurrVelocity;
        }
    }
}
