using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.SoftBody.ForceGenerators;
using SkeelSoftBodyPhysicsTutorial.SoftBody.Integrators;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.Simulations
{
    public class Simulation
    {
        protected Game game;
        protected List<SimObject> simObjects = new List<SimObject>();
        protected List<ForceGenerator> globalForceGenerators = new List<ForceGenerator>();
        protected List<Spring> springs = new List<Spring>();
        protected Integrator integrator;

        public List<SimObject> SimObjects
        {
            get { return simObjects; }
            set { simObjects = value; }
        }

        public Integrator Integrator
        {
            get { return integrator; }
            set { integrator = value; }
        }

        //-----------------------------------------------------------------------

        public Simulation(Game game)
        {
            this.game = game;

            //create a default integrator
            this.integrator = new ForwardEulerIntegrator(game);
        }

        public void AddSpring(float stiffness, float damping, SimObject simObjA, SimObject simObjB)
        {
            Spring spring = new Spring(stiffness, damping, simObjA, simObjB);
            springs.Add(spring);
        }

        public void AddSimObject(SimObject simObject)
        {
            simObjects.Add(simObject);
        }

        public void AddGlobalForceGenerator(ForceGenerator forceGenerator)
        {
            globalForceGenerators.Add(forceGenerator);
        }

        Vector3 acceleration;
        public virtual void Update(GameTime gameTime)
        {
            //sum all local forces
            foreach (Spring spring in springs)
            {
                spring.ApplyForce(null);  //no need to specify any simObj
            }

            //sum all global forces acting on the objects
            foreach (SimObject simObject in simObjects)
            {
                if (simObject.SimObjectType == SimObjectType.ACTIVE)
                {
                    foreach (ForceGenerator forceGenerator in globalForceGenerators)
                    {
                        forceGenerator.ApplyForce(simObject);
                    }
                }
            }

            foreach (SimObject simObject in simObjects)
            {
                if (simObject.SimObjectType == SimObjectType.ACTIVE)
                {
                    //find acceleration
                    acceleration = simObject.ResultantForce / simObject.Mass;

                    //integrate
                    integrator.Integrate(acceleration, simObject);
                }
            }

            //update object
            foreach (SimObject simObject in simObjects)
            {
                simObject.Update(gameTime);
            }

            //reset forces on sim objects
            foreach (SimObject simObject in simObjects)
            {
                if (simObject.SimObjectType == SimObjectType.ACTIVE)
                {
                    simObject.ResetForces();
                }
            }
        }
    }
}
