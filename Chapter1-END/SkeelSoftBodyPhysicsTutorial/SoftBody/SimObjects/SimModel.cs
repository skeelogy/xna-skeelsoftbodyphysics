using Microsoft.Xna.Framework;
using SkeelSoftBodyPhysicsTutorial.Main;

namespace SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects
{
    public sealed class SimModel : SimObject
    {
        private GameModel model;

        public GameModel Model
        {
            get { return model; }
            set { model = value; }
        }

        //--------------------------------------------------------------------

        public SimModel(GameModel model, float mass, SimObjectType simObjectType)
            : base(mass, simObjectType)
        {
            this.model = model;
            this.currPosition = model.Translate;
            this.prevPosition = currPosition;
        }

        public override void Update(GameTime gameTime)
        {
            this.model.Translate = this.currPosition;
        }
    }
}
