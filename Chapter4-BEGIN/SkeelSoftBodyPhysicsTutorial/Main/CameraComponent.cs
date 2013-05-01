using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkeelSoftBodyPhysicsTutorial.Main
{
    public interface ICameraComponent { }

    /// <summary>
    /// A simple component class that represents a camera
    /// </summary>
    public sealed class CameraComponent : Microsoft.Xna.Framework.DrawableGameComponent, ICameraComponent
    {
        private Game game;
        private Matrix view;
        private Matrix projection;
        private Vector3 position;
        private Vector3 target;
        private Vector3 up;

        string backgroundTextureFile;
        Texture2D backgroundTexture;
        SpriteBatch spriteBatch;

        public Vector3 Up
        {
            get { return up; }
            set { up = value; }
        }

        public Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Matrix Projection
        {
            get { return projection; }
        }

        public Matrix View
        {
            get { return view; }
        }

        //------------------------------------------------------------------------

        public CameraComponent(Game game, Vector3 position, Vector3 target, Vector3 up, string backgroundTextureFile)
            : base(game)
        {
            this.game = game;
            this.position = position;
            this.target = target;
            this.up = up;
            this.backgroundTextureFile = backgroundTextureFile;

            //add itself as a game service
            game.Services.AddService(typeof(ICameraComponent), this);
        }

        public override void Initialize()
        {
            InitializeCamera();
            base.Initialize();
        }

        private void InitializeCamera()
        {
            float aspectRatio = (float)game.GraphicsDevice.Viewport.Width / (float)game.GraphicsDevice.Viewport.Height;

            //create the projection matrix
            Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.1f, 1000.0f, out projection);

            //create the view matrix
            Matrix.CreateLookAt(ref position, ref target, ref up, out view);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            if (!string.IsNullOrEmpty(backgroundTextureFile))
            {
                backgroundTexture = game.Content.Load<Texture2D>(backgroundTextureFile);
            }
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //recalculate the view matrix based on the position, target and up
            Matrix.CreateLookAt(ref position, ref target, ref up, out view);

            //draw a background image
            if (backgroundTexture != null)
            {
                spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
                spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}