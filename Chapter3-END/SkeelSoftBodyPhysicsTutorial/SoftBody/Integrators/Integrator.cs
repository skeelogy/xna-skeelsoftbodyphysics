using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.Integrators
{
    public abstract class Integrator
    {
        private Game game;

        //use a fixed time step to get predictable sim
        protected float fixedTimeStep;

        public float FixedTimeStep
        {
            get { return fixedTimeStep; }
            set { fixedTimeStep = value; }
        }

        //-------------------------------------------------------------

        public Integrator(Game game)
        {
            this.game = game;

            //set the fixed time step to target elapsed time (default at 1/60)
            fixedTimeStep = (float)game.TargetElapsedTime.TotalSeconds;
        }

        public abstract void Integrate(Vector3 acceleration, SimObject simObject);
    }
}
