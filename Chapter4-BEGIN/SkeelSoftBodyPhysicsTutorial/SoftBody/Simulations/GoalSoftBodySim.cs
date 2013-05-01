using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.Primitives;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.Simulations
{
    public sealed class GoalSoftBodySim : SoftBodySim
    {
        private TexturedPrimitive softBodyObject;
        private TexturedPrimitive softBodyGoalObject;
        private SimVertex[] simVerticesForGoal;

        public GoalSoftBodySim(Game game, TexturedPrimitive softBodyObject, TexturedPrimitive softBodyGoalObject, float mass, float stiffness, float damping)
            : base(game)
        {
            this.softBodyObject = softBodyObject;
            this.softBodyGoalObject = softBodyGoalObject;

            //create sim vertices on both the goal and non-goal object
            CreateSimVertices(softBodyObject, mass);
            CreateSimVerticesForGoal(softBodyGoalObject, mass);

            //connect springs between the vertices of the goal and non-goal object
            ConnectSprings(stiffness, damping);
        }

        private void CreateSimVertices(TexturedPrimitive softBodyObject, float mass)
        {
            int numVertices = softBodyObject.NumVertices;
            float vertexMass = mass / numVertices;
            simVertices = new SimVertex[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                simVertices[i] = new SimVertex(vertexMass, SimObjectType.ACTIVE, i, softBodyObject);
                this.AddSimObject(simVertices[i]);
            }
        }

        private void CreateSimVerticesForGoal(TexturedPrimitive softBodyObject, float mass)
        {
            int numVertices = softBodyObject.NumVertices;
            float vertexMass = mass / numVertices;
            simVerticesForGoal = new SimVertex[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                simVerticesForGoal[i] = new SimVertex(vertexMass, SimObjectType.PASSIVE, i, softBodyObject);
                this.AddSimObject(simVerticesForGoal[i]);
            }
        }

        private void ConnectSprings(float stiffness, float damping)
        {
            //find the increment step for each subsequent spring
            float increment = (0.5f * stiffness) / softBodyObject.NumVertices;

            for (int i = 0; i < softBodyObject.NumVertices; i++)
            {
                //create a gradient stiffness from 0.5 stiffness to 1.0 stiffness
                float thisStiffness = (increment * i) + 0.5f * stiffness;
                this.AddSpring(thisStiffness, damping, simVertices[i], simVerticesForGoal[i]);
            }
        }

        public override void Update(GameTime gameTime)
        {
            //call base.Update() to update the vertex positions
            base.Update(gameTime);

            //recalculate the vertex normals
            softBodyObject.RecalculateNormals();

            //commit the vertex position and normal changes
            softBodyObject.CommitChanges();
        }

    }
}
