using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkeelSoftBodyPhysicsTutorial.Main;
using SkeelSoftBodyPhysicsTutorial.SoftBody.Constraints;
using SkeelSoftBodyPhysicsTutorial.SoftBody.ForceGenerators;
using SkeelSoftBodyPhysicsTutorial.SoftBody.Integrators;
using SkeelSoftBodyPhysicsTutorial.SoftBody.Simulations;

namespace SkeelSoftBodyPhysicsTutorial
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //component members
        CameraComponent cameraComponent;
        InputComponent inputComponent;
        ModelComponent modelComponent;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //instantiate the components and add them to this.Components
            Vector3 cameraPosition = new Vector3(0, -3, 15);
            Vector3 targetPosition = new Vector3(0, -3, 0);
            Vector3 upVector = Vector3.UnitY;
            string backgroundImage = "background-chain";
            cameraComponent = new CameraComponent(this, cameraPosition, targetPosition, upVector, backgroundImage);
            this.Components.Add(cameraComponent);

            bool showMouse = true;
            inputComponent = new InputComponent(this, showMouse);
            this.Components.Add(inputComponent);

            modelComponent = new ModelComponent(this);
            this.Components.Add(modelComponent);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
