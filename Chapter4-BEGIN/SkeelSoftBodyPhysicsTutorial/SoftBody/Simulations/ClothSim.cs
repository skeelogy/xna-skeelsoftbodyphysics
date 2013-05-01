using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkeelSoftBodyPhysicsTutorial.Primitives;
using SkeelSoftBodyPhysicsTutorial.SoftBody.Constraints;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.Simulations
{
    public sealed class ClothSim : SoftBodySim
    {
        private TexturedPlane clothPlane;

        public ClothSim(Game game, TexturedPlane clothPlane, float clothMass,
                        float structStiffness, float structDamping,
                        float shearStiffness, float shearDamping,
                        float bendStiffness, float bendDamping)
            : base(game)
        {
            this.clothPlane = clothPlane;

            //create sim data
            CreateSimVertices(clothPlane, clothMass);

            //connect springs and add constraints
            ConnectSprings(structStiffness, structDamping,
                           shearStiffness, shearDamping,
                           bendStiffness, bendDamping);
        }

        private void CreateSimVertices(TexturedPlane clothPlane, float clothMass)
        {
            int numVertices = clothPlane.NumVertices;
            float vertexMass = clothMass / numVertices;
            simVertices = new SimVertex[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                simVertices[i] = new SimVertex(vertexMass, SimObjectType.ACTIVE, i, clothPlane);
                this.AddSimObject(simVertices[i]);
            }
        }

        private void ConnectSprings(float structStiffness, float structDamping,
                                    float shearStiffness, float shearDamping,
                                    float bendStiffness, float bendDamping)
        {
            for (int x = 0; x < clothPlane.LengthSegments; x++)
            {
                for (int y = 0; y <= clothPlane.WidthSegments; y++)
                {
                    //structural spring: horizontal (-)
                    int vertexAId = x + y * (clothPlane.LengthSegments + 1);
                    int vertexBId = (x + 1) + y * (clothPlane.LengthSegments + 1);
                    this.AddSpring(structStiffness, structDamping, simVertices[vertexAId], simVertices[vertexBId]);
                    float length = (clothPlane.GetVertexPosition(vertexAId) - clothPlane.GetVertexPosition(vertexBId)).Length();
                    this.Constraints.Add(new LengthConstraint(length, simVertices[vertexAId], simVertices[vertexBId]));
                }
            }

            for (int x = 0; x <= clothPlane.LengthSegments; x++)
            {
                for (int y = 0; y < clothPlane.WidthSegments; y++)
                {
                    //structural spring: vertical (|)
                    int vertexAId = x + y * (clothPlane.LengthSegments + 1);
                    int vertexBId = x + (y + 1) * (clothPlane.LengthSegments + 1);
                    this.AddSpring(structStiffness, structDamping, simVertices[vertexAId], simVertices[vertexBId]);
                    float length = (clothPlane.GetVertexPosition(vertexAId) - clothPlane.GetVertexPosition(vertexBId)).Length();
                    this.Constraints.Add(new LengthConstraint(length, simVertices[vertexAId], simVertices[vertexBId]));
                }
            }

            for (int x = 0; x < clothPlane.LengthSegments; x++)
            {
                for (int y = 0; y < clothPlane.WidthSegments; y++)
                {
                    //shear spring: diagonal (/)
                    int vertexAId = (x + 1) + y * (clothPlane.LengthSegments + 1);
                    int vertexBId = x + (y + 1) * (clothPlane.LengthSegments + 1);
                    this.AddSpring(shearStiffness, shearDamping, simVertices[vertexAId], simVertices[vertexBId]);
                    float length = (clothPlane.GetVertexPosition(vertexAId) - clothPlane.GetVertexPosition(vertexBId)).Length();
                    this.Constraints.Add(new LengthConstraint(length, simVertices[vertexAId], simVertices[vertexBId]));
                    
                    //shear spring: diagonal (\)
                    vertexAId = x + y * (clothPlane.LengthSegments + 1);
                    vertexBId = (x + 1) + (y + 1) * (clothPlane.LengthSegments + 1);
                    this.AddSpring(shearStiffness, shearDamping, simVertices[vertexAId], simVertices[vertexBId]);
                    length = (clothPlane.GetVertexPosition(vertexAId) - clothPlane.GetVertexPosition(vertexBId)).Length();
                    this.Constraints.Add(new LengthConstraint(length, simVertices[vertexAId], simVertices[vertexBId]));
                }
            }

            for (int x = 0; x < clothPlane.LengthSegments - 1; x++)
            {
                for (int y = 0; y <= clothPlane.WidthSegments; y++)
                {
                    //bend spring: horizontal (--)
                    int vertexAId = x + y * (clothPlane.LengthSegments + 1);
                    int vertexBId = (x + 2) + y * (clothPlane.LengthSegments + 1);
                    this.AddSpring(bendStiffness, bendDamping, simVertices[vertexAId], simVertices[vertexBId]);
                    float length = (clothPlane.GetVertexPosition(vertexAId) - clothPlane.GetVertexPosition(vertexBId)).Length();
                    this.Constraints.Add(new LengthConstraint(length, simVertices[vertexAId], simVertices[vertexBId]));
                }
            }

            for (int x = 0; x <= clothPlane.LengthSegments; x++)
            {
                for (int y = 0; y < clothPlane.WidthSegments - 1; y++)
                {
                    //bend spring: vertical (||)
                    int vertexAId = x + y * (clothPlane.LengthSegments + 1);
                    int vertexBId = x + (y + 2) * (clothPlane.LengthSegments + 1);
                    this.AddSpring(bendStiffness, bendDamping, simVertices[vertexAId], simVertices[vertexBId]);
                    float length = (clothPlane.GetVertexPosition(vertexAId) - clothPlane.GetVertexPosition(vertexBId)).Length();
                    this.Constraints.Add(new LengthConstraint(length, simVertices[vertexAId], simVertices[vertexBId]));
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            //call base.Update() to update the vertex positions
            base.Update(gameTime);

            //recalculate the vertex normals
            clothPlane.RecalculateNormals();

            //commit the vertex position and normal changes
            clothPlane.CommitChanges();
        }

    }
}
