using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkeelSoftBodyPhysicsTutorial.Main;
using SkeelSoftBodyPhysicsTutorial.Primitives;
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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //instantiate the components and add them to this.Components
            Vector3 cameraPosition = new Vector3(0, -3, 15);
            Vector3 targetPosition = new Vector3(0, -3, 0);
            Vector3 upVector = Vector3.UnitY;
            string backgroundImage = "background-cloth";
            cameraComponent = new CameraComponent(this, cameraPosition, targetPosition, upVector, backgroundImage);
            this.Components.Add(cameraComponent);

            bool showMouse = true;
            inputComponent = new InputComponent(this, showMouse);
            this.Components.Add(inputComponent);
        }

        protected override void Initialize()
        {
            InitClothScene();
            base.Initialize();
        }

        TexturedPlane clothPlane;
        ClothSim clothSim;
        PointConstraint topLeftCorner, topRightCorner;
        private void InitClothScene()
        {
            //cloth attributes
            int length = 10;
            int width = 5;
            int lengthSegments = 20;
            int widthSegments = 15;

            //load in a plane
            clothPlane = new TexturedPlane(this, length, width, lengthSegments, widthSegments, @"checkerboard");
            clothPlane.Initialize();

            //create a cloth sim
            float clothMass = 2.0f;
            float structStiffness = 2.0f;
            float structDamping = 0.02f;
            float shearStiffness = 2.0f;
            float shearDamping = 0.02f;
            float bendStiffness = 2.0f;
            float bendDamping = 0.02f;
            clothSim = new ClothSim(this, clothPlane, clothMass, structStiffness, structDamping, shearStiffness, shearDamping, bendStiffness, bendDamping);
            clothSim.ConstraintIterations = 10;

            //add in a global forceGenerators: gravity
            Gravity gravity = new Gravity(new Vector3(0, -9.81f, 0));
            clothSim.AddGlobalForceGenerator(gravity);

            //constrain the two top corners of the cloth so that we can control it
            topLeftCorner = new PointConstraint(clothSim.SimVertices[0].CurrPosition, clothSim.SimVertices[0]);
            clothSim.Constraints.Add(topLeftCorner);
            topRightCorner = new PointConstraint(clothSim.SimVertices[lengthSegments].CurrPosition, clothSim.SimVertices[lengthSegments]);
            clothSim.Constraints.Add(topRightCorner);

            //create an integrator and assign it to the sim
            float drag = 0.01f;
            Integrator integrator = new VerletNoVelocityIntegrator(this, drag);
            clothSim.Integrator = integrator;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            clothPlane.LoadContent();
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            //poll for input
            HandleInput(gameTime);

            //update the simulation
            clothSim.Update(gameTime);

            base.Update(gameTime);
        }

        private void HandleInput(GameTime gameTime)
        {
            if (inputComponent.IsMouseHeldDown(MouseButtonType.Left))
            {
                topLeftCorner.PointX += -inputComponent.MouseMoved.X / 40;
                topLeftCorner.PointY += inputComponent.MouseMoved.Y / 40;
            }
            if (inputComponent.IsMouseHeldDown(MouseButtonType.Right))
            {
                topRightCorner.PointX += -inputComponent.MouseMoved.X / 40;
                topRightCorner.PointY += inputComponent.MouseMoved.Y / 40;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.RenderState.CullMode = CullMode.None;
            base.Draw(gameTime);
            clothPlane.Draw(gameTime);
        }
    }
}
