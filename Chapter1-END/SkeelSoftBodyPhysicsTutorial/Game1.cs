using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkeelSoftBodyPhysicsTutorial.Main;
using SkeelSoftBodyPhysicsTutorial.SoftBody.ForceGenerators;
using SkeelSoftBodyPhysicsTutorial.SoftBody.Integrators;
using SkeelSoftBodyPhysicsTutorial.SoftBody.SimObjects;
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
        Line3DComponent line3DComponent;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //instantiate the components and add them to this.Components
            Vector3 cameraPosition = new Vector3(0, 6, 10);
            Vector3 targetPosition = new Vector3(0, 2, 0);
            Vector3 upVector = Vector3.UnitY;
            string backgroundImage = "";
            cameraComponent = new CameraComponent(this, cameraPosition, targetPosition, upVector, backgroundImage);
            this.Components.Add(cameraComponent);

            bool showMouse = false;
            inputComponent = new InputComponent(this, showMouse);
            this.Components.Add(inputComponent);

            modelComponent = new ModelComponent(this);
            this.Components.Add(modelComponent);

            line3DComponent = new Line3DComponent(this);
            this.Components.Add(line3DComponent);
        }

        protected override void Initialize()
        {
            InitSpringScene();
            base.Initialize();
        }

        Simulation springSim;
        SimModel stationaryCubeSimObj, movingSphereSimObj;
        private void InitSpringScene()
        {
            //load in a cube and a sphere
            GameModel stationaryCube = modelComponent.LoadGameModel(@"cube");
            stationaryCube.TranslateY = 5;
            GameModel movingSphere = modelComponent.LoadGameModel(@"sphere");

            //create a spring sim
            springSim = new Simulation(this);

            //create sim objects from the loaded models
            float sphereMass = 1.0f;
            movingSphereSimObj = new SimModel(movingSphere, sphereMass, SimObjectType.ACTIVE);
            springSim.AddSimObject(movingSphereSimObj);
            stationaryCubeSimObj = new SimModel(stationaryCube, 1000.0f, SimObjectType.PASSIVE);
            springSim.AddSimObject(stationaryCubeSimObj);

            //add in a global force generator: gravity
            Gravity gravity = new Gravity(new Vector3(0, -9.81f, 0));
            springSim.AddGlobalForceGenerator(gravity);

            //add in a global force generator: air
            float dragCoefficient = 0.5f;
            Medium air = new Medium(dragCoefficient);
            springSim.AddGlobalForceGenerator(air);

            //attach a spring between the sim objects
            float stiffness = 8.0f;
            float damping = 0.1f;
            springSim.AddSpring(stiffness, damping, stationaryCubeSimObj, movingSphereSimObj);

            //create an integrator and assign it to the sim
            ForwardEulerIntegrator integrator = new ForwardEulerIntegrator(this);
            springSim.Integrator = integrator;

            //init the line from cube to sphere that represents the spring
            line3DComponent.StartPosition = stationaryCube.Translate;
            line3DComponent.EndPosition = movingSphere.Translate;
            line3DComponent.Color = Color.White;
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

            //update the sim
            springSim.Update(gameTime);

            //update line
            line3DComponent.StartPosition = stationaryCubeSimObj.CurrPosition;
            line3DComponent.EndPosition = movingSphereSimObj.CurrPosition;

            base.Update(gameTime);
        }

        private void HandleInput(GameTime gameTime)
        {
            if (inputComponent.IsKeyHeldDown(Keys.Right))
            {
                stationaryCubeSimObj.CurrPositionX += 0.1f;
            }
            if (inputComponent.IsKeyHeldDown(Keys.Left))
            {
                stationaryCubeSimObj.CurrPositionX -= 0.1f;
            }
            if (inputComponent.IsKeyHeldDown(Keys.Up))
            {
                stationaryCubeSimObj.CurrPositionY += 0.1f;
            }
            if (inputComponent.IsKeyHeldDown(Keys.Down))
            {
                stationaryCubeSimObj.CurrPositionY -= 0.1f;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
