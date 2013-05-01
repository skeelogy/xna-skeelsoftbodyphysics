using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.Integrators
{
    public sealed class ForwardEulerIntegrator : Integrator
    {
        public ForwardEulerIntegrator(Game game)
            : base(game) { }

        public override void Integrate(Vector3 acceleration, SimObject simObject)
        {
            //calculate new position using the velocity at current time
            simObject.CurrPosition += simObject.CurrVelocity * fixedTimeStep;

            //calculate new velocity using the acceleration at current time
            simObject.CurrVelocity += acceleration * fixedTimeStep;
        }
    }
}
