using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.Primitives;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects
{
    public sealed class SimVertex : SimObject
    {
        private int vertexId;
        private TexturedPrimitive primitive;

        public int VertexId
        {
            get { return vertexId; }
            set { vertexId = value; }
        }

        //----------------------------------------------------------------------------------

        public SimVertex(float mass, SimObjectType simObjectType, int vertexId, TexturedPrimitive primitive)
            : base(mass, simObjectType)
        {
            this.vertexId = vertexId;
            this.primitive = primitive;
            this.currPosition = primitive.GetVertexPosition(vertexId);
            this.prevPosition = currPosition;
        }

        public override void Update(GameTime gameTime)
        {
            primitive.SetVertexPosition(this.vertexId, this.currPosition);
        }
    }
}
