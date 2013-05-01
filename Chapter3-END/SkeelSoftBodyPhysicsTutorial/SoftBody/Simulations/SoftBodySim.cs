using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.Simulations
{
    public class SoftBodySim : Simulation
    {
        protected SimVertex[] simVertices;

        public SimVertex[] SimVertices
        {
            get { return simVertices; }
            set { simVertices = value; }
        }

        //-----------------------------------------------------------------------

        public SoftBodySim(Game game)
            : base(game) { }
    }
}
