using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkeelSoftBodyPhysicsTutorial.Main;
using SkeelSoftBodyPhysicsTutorial.Primitives;
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
            Vector3 cameraPosition = new Vector3(0, 15, 15);
            Vector3 targetPosition = new Vector3(0, 5, 0);
            Vector3 upVector = Vector3.UnitY;
            string backgroundImage = "background-slinky";
            cameraComponent = new CameraComponent(this, cameraPosition, targetPosition, upVector, backgroundImage);
            this.Components.Add(cameraComponent);

            bool showMouse = true;
            inputComponent = new InputComponent(this, showMouse);
            this.Components.Add(inputComponent);
        }

        protected override void Initialize()
        {
            InitGoalSoftBodyScene();
            base.Initialize();
        }

        TexturedCylinder cylinder;
        TexturedCylinder goalCylinder;
        GoalSoftBodySim goalSoftBodySim;
        Gravity push;
        private void InitGoalSoftBodyScene()
        {
            //cylinder attribute
            float length = 10.0f;
            float radius = 3.0f;
            int lengthSegments = 30;
            int radialSegments = 50;

            //load in the cylinder
            cylinder = new TexturedCylinder(this, length, radius, lengthSegments, radialSegments, "slinky");
            cylinder.Initialize();

            //load in another same cylinder as the goal
            goalCylinder = new TexturedCylinder(this, length, radius, lengthSegments, radialSegments, "checkerboard");
            goalCylinder.Initialize();

            //create a goal soft body sim
            float mass = 0.1f;
            float stiffness = 0.03f;
            float damping = 0.0f;
            goalSoftBodySim = new GoalSoftBodySim(this, cylinder, goalCylinder, mass, stiffness, damping);

            //create a force generator that we will use to push the cylinder later
            push = new Gravity(Vector3.Zero);
            goalSoftBodySim.AddGlobalForceGenerator(push);

            //create an integrator and assign it to the sim
            float drag = 0.05f;
            Integrator integrator = new VerletNoVelocityIntegrator(this, drag);
            goalSoftBodySim.Integrator = integrator;
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cylinder.LoadContent();
            goalCylinder.LoadContent();
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            //poll for input
            HandleInput(gameTime);

            //update the simulation
            goalSoftBodySim.Update(gameTime);

            base.Update(gameTime);
        }

        Vector2 startPos, endPos;
        private void HandleInput(GameTime gameTime)
        {
            if (inputComponent.IsMouseJustDown(MouseButtonType.Left))
            {
                //mouse is just down, so store the starting position
                startPos = inputComponent.MousePosition;
            }
            else if (inputComponent.IsMouseJustUp(MouseButtonType.Left))
            {
                //mouse has just been released, so find the ending position
                endPos = inputComponent.MousePosition;

                //use distance mouse moved as the push acceleration
                push.AccelerationX = (endPos.X - startPos.X) * 3;
                push.AccelerationY = -(endPos.Y - startPos.Y) * 3;
                
                return;
            }

            //set the push acceleration to zero
            push.Acceleration = Vector3.Zero;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.RenderState.CullMode = CullMode.None;
            base.Draw(gameTime);
            cylinder.Draw(gameTime);
        }
    }
}
