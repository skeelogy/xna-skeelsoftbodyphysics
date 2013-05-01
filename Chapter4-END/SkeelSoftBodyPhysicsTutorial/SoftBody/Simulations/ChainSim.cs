using System;
using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.Main;
using SkeelSoftBodyPhysicsTutorial.SoftBody.Constraints;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.Simulations
{
    public class ChainSim : Simulation
    {
        public ChainSim(Game game, GameModel[] ropeSegmentModels, float totalMass, float stiffness, float damping)
            : base(game)
        {
            //create sim data
            float sphereMass = totalMass / ropeSegmentModels.Length;
            SimModel[] simObjs = new SimModel[ropeSegmentModels.Length];
            for (int i = 0; i < ropeSegmentModels.Length; i++)
            {
                simObjs[i] = new SimModel(ropeSegmentModels[i], sphereMass, SimObjectType.ACTIVE);
                this.AddSimObject(simObjs[i]);
            }

            //attach springs between the sim objects
            for (int i = 1; i < ropeSegmentModels.Length; i++)
            {
                this.AddSpring(stiffness, damping, simObjs[i - 1], simObjs[i]);
                this.Constraints.Add(new LengthConstraint((simObjs[i - 1].Model.Translate - simObjs[i].Model.Translate).Length(), simObjs[i - 1], simObjs[i]));
            }
        }

        Vector3 currToChildVector;
        float angle;
        Vector3 rotAxis;
        public override void Update(GameTime gameTime)
        {
            //call base to perform simulation
            base.Update(gameTime);

            //orient rope segments so that they point to immediate child
            for (int i = 0; i < simObjects.Count - 1; i++)
            {
                //get vector that points to child
                currToChildVector = simObjects[i + 1].CurrPosition - simObjects[i].CurrPosition;
                if (currToChildVector != Vector3.Zero)
                {
                    //normalize
                    currToChildVector.Normalize();

                    //find out angle to rotate
                    angle = (float)Math.Acos(Vector3.Dot(Vector3.UnitY, -currToChildVector));
                    angle = MathHelper.ToDegrees(angle);

                    //find out axis to rotate about
                    rotAxis = Vector3.Cross(Vector3.UnitY, -currToChildVector);
                    if (rotAxis != Vector3.Zero) rotAxis.Normalize();

                    //need to just rotate model about the zaxis for our case
                    ((SimModel)simObjects[i]).Model.RotateZ = rotAxis.Z * angle;
                }
            }

            //make the orientation of the last segment follow the parent segment
            ((SimModel)simObjects[simObjects.Count - 1]).Model.RotateZ = ((SimModel)simObjects[simObjects.Count - 2]).Model.RotateZ;
        }
    }
}
