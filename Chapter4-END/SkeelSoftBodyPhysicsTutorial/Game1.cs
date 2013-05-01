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
            InitChainScene();
            base.Initialize();
        }

        ChainSim chainSim;
        PointConstraint chainHeadPoint;
        private void InitChainScene()
        {
            //chain attributes
            int numSegments = 30;
            float separation = 0.65f;

            //load in segments
            GameModel[] segments = new GameModel[numSegments];
            for (int i = 0; i < numSegments; i++)
            {
                segments[i] = modelComponent.LoadGameModel("chainSegment");

                //reduce the size of each segment
                segments[i].Scale = 0.3f * Vector3.One;

                //position each segment downwards incrementally to form the chain
                segments[i].TranslateY = -i * separation;

                //rotate the even segments by 90 degrees in Y axis
                segments[i].RotateY = (i % 2) * 90.0f;
            }

            //create a chain sim
            float totalMass = 1.0f;
            float stiffness = 0.3f;
            float damping = 0.01f;
            chainSim = new ChainSim(this, segments, totalMass, stiffness, damping);

            //add in a global force generator: gravity
            Gravity gravity = new Gravity(new Vector3(0, -9.81f * 5, 0));
            chainSim.AddGlobalForceGenerator(gravity);

            //constrain the head point so that we can control it
            chainHeadPoint = new PointConstraint(Vector3.Zero, chainSim.SimObjects[0]);
            chainSim.Constraints.Add(chainHeadPoint);

            //create a integrator and assign it to the sim
            float drag = 0.005f;
            Integrator integrator = new VerletNoVelocityIntegrator(this, drag);
            chainSim.Integrator = integrator;
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
            //poll for input
            HandleInput(gameTime);

            //update the simulation
            chainSim.Update(gameTime);

            base.Update(gameTime);
        }

        private void HandleInput(GameTime gameTime)
        {
            if (inputComponent.IsMouseHeldDown(MouseButtonType.Left))
            {
                chainHeadPoint.PointX += -inputComponent.MouseMoved.X / 40;
                chainHeadPoint.PointY += inputComponent.MouseMoved.Y / 40;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
