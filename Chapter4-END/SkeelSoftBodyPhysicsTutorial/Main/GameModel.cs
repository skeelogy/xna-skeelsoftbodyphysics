using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkeelSoftBodyPhysicsTutorial.Main
{
    /// <summary>
    /// A class that loads, displays and transforms a Model.
    /// </summary>
    public sealed class GameModel
    {
        private Game game;
        private string modelFile;
        private Model model;
        private CameraComponent cameraComponent;

        private Matrix world;
        private Vector3 translate;
        private Vector3 rotate;
        private Vector3 scale;
        private Vector3 pivot;

        public Model Model
        {
            get { return model; }
            set { model = value; }
        }

        public Matrix World
        {
            get { return world; }
            set { world = value; }
        }

        public Vector3 Translate
        {
            get { return translate; }
            set { translate = value; UpdateWorldMatrix(); }
        }

        public float TranslateX
        {
            get { return translate.X; }
            set { translate.X = value; UpdateWorldMatrix(); }
        }

        public float TranslateY
        {
            get { return translate.Y; }
            set { translate.Y = value; UpdateWorldMatrix(); }
        }

        public float TranslateZ
        {
            get { return translate.Z; }
            set { translate.Z = value; UpdateWorldMatrix(); }
        }

        public Vector3 Rotate
        {
            get { return rotate; }
            set { rotate = value; UpdateWorldMatrix(); }
        }

        public float RotateX
        {
            get { return rotate.X; }
            set { rotate.X = value; UpdateWorldMatrix(); }
        }

        public float RotateY
        {
            get { return rotate.Y; }
            set { rotate.Y = value; UpdateWorldMatrix(); }
        }

        public float RotateZ
        {
            get { return rotate.Z; }
            set { rotate.Z = value; UpdateWorldMatrix(); }
        }

        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; UpdateWorldMatrix(); }
        }

        public float ScaleX
        {
            get { return scale.X; }
            set { scale.X = value; UpdateWorldMatrix(); }
        }

        public float ScaleY
        {
            get { return scale.Y; }
            set { scale.Y = value; UpdateWorldMatrix(); }
        }

        public float ScaleZ
        {
            get { return scale.Z; }
            set { scale.Z = value; UpdateWorldMatrix(); }
        }

        public Vector3 Pivot
        {
            get { return pivot; }
            set { pivot = value; UpdateWorldMatrix(); }
        }

        public float PivotX
        {
            get { return pivot.X; }
            set { pivot.X = value; UpdateWorldMatrix(); }
        }

        public float PivotY
        {
            get { return pivot.Y; }
            set { pivot.Y = value; UpdateWorldMatrix(); }
        }

        public float PivotZ
        {
            get { return pivot.Z; }
            set { pivot.Z = value; UpdateWorldMatrix(); }
        }

        //-------------------------------------------------------------------------

        public GameModel(Game game, string modelFile)
            : this(game, modelFile, Vector3.Zero, Vector3.Zero, Vector3.One) { }

        public GameModel(Game game, string modelFile, Vector3 translate, Vector3 rotate, Vector3 scale)
        {
            this.game = game;
            this.modelFile = modelFile;

            this.translate = translate;
            this.rotate = rotate;
            this.scale = scale;
            this.pivot = Vector3.Zero;
            UpdateWorldMatrix();
        }

        private void UpdateWorldMatrix()
        {
            world = Matrix.CreateTranslation(-pivot) *
                      Matrix.CreateScale(scale) *
                      Matrix.CreateRotationX(MathHelper.ToRadians(rotate.X)) *
                      Matrix.CreateRotationY(MathHelper.ToRadians(rotate.Y)) *
                      Matrix.CreateRotationZ(MathHelper.ToRadians(rotate.Z)) *
                      Matrix.CreateTranslation(translate + pivot);
        }

        internal void Initialize()
        {
            cameraComponent = (CameraComponent)game.Services.GetService(typeof(ICameraComponent));
        }

        internal void LoadContent()
        {
            model = game.Content.Load<Model>(modelFile);
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
        }

        internal void Draw(GameTime gameTime)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = cameraComponent.Projection;
                    effect.View = cameraComponent.View;
                    effect.World = mesh.ParentBone.Transform * this.world;
                    mesh.Draw();
                }
            }
        }
    }
}
