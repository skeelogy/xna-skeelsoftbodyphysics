using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkeelSoftBodyPhysicsTutorial.Main
{
    public interface ILine3DComponent { }

    /// <summary>
    /// A simple component class that draws a 3d line in space
    /// </summary>
    public sealed class Line3DComponent : Microsoft.Xna.Framework.DrawableGameComponent, ILine3DComponent
    {
        private Game game;
        private BasicEffect effect;
        private CameraComponent cameraComponent;
        private VertexPositionColor[] vertices;
        private short[] indices;

        public Vector3 StartPosition
        {
            get { return vertices[0].Position; }
            set { vertices[0].Position = value; }
        }

        public Vector3 EndPosition
        {
            get { return vertices[1].Position; }
            set { vertices[1].Position = value; }
        }

        public Color Color
        {
            get { return vertices[0].Color; }
            set { vertices[0].Color = value; vertices[1].Color = value; }
        }

        //--------------------------------------------------------------

        public Line3DComponent(Game game)
            : base(game)
        {
            this.game = game;

            //add itself as a game service
            game.Services.AddService(typeof(ILine3DComponent), this);

            vertices = new VertexPositionColor[2];
            indices = new short[2] { 0, 1 };
        }

        public override void Initialize()
        {
            effect = new BasicEffect(game.GraphicsDevice, null);
            cameraComponent = (CameraComponent)game.Services.GetService(typeof(ICameraComponent));
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            effect.Projection = cameraComponent.Projection;
            effect.View = cameraComponent.View;
            effect.World = Matrix.Identity;
            effect.VertexColorEnabled = true;
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                game.GraphicsDevice.VertexDeclaration = new VertexDeclaration(game.GraphicsDevice, VertexPositionColor.VertexElements);
                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, vertices.Length, indices, 0, indices.Length / 2);
                pass.End();
            }
            effect.End();
            base.Draw(gameTime);
        }
    }
}