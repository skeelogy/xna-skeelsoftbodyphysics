using System;
using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.Integrators
{
    public sealed class VerletNoVelocityIntegrator : Integrator
    {
        private float drag;

        public float Drag
        {
            get { return drag; }
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentException("Air resistance must be between 0 and 1");
                }
                drag = value;
            }
        }

        public VerletNoVelocityIntegrator(Game game) : this(game, 0.005f) { }

        public VerletNoVelocityIntegrator(Game game, float drag)
            : base(game)
        {
            this.drag = drag;
        }

        Vector3 newPosition;
        public override void Integrate(Vector3 acceleration, SimObject simObject)
        {
            newPosition = (2 - drag) * simObject.CurrPosition
                - (1 - drag) * simObject.PrevPosition
                + acceleration * fixedTimeStep * fixedTimeStep;

            simObject.PrevPosition = simObject.CurrPosition;
            simObject.CurrPosition = newPosition;
        }
    }
}
