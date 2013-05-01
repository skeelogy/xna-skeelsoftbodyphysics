using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkeelSoftBodyPhysicsTutorial.Main;

namespace SkeelSoftBodyPhysicsTutorial.Primitives
{
    public abstract class TexturedPrimitive
    {
        protected Game game;

        protected VertexPositionNormalTexture[] vertices;
        protected VertexBuffer vertexBuffer;
        protected IndexBuffer indexBuffer;

        protected int numRealVertices;  //number of vertices in the buffer (might have dups)
        protected int numTris;
        protected int numVertices;  //number of pseudo vertices (no dups)
        protected List<List<int>> vertexMappingPseudoToReal = new List<List<int>>();
        protected int[] vertexMappingRealToPseudo;

        //for tri info
        protected Vector3[] triangleNormals;
        protected float[] triangleAreas;
        protected List<int[]> triangleVertexInfo = new List<int[]>();

        //for rendering
        protected CameraComponent cameraComponent;
        protected BasicEffect effect;

        //for texture
        protected string textureFile;
        protected Texture2D texture;

        public VertexPositionNormalTexture[] Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        public VertexBuffer VertexBuffer
        {
            get { return vertexBuffer; }
            set { vertexBuffer = value; }
        }

        public int NumVertices
        {
            get { return numVertices; }
            set { numVertices = value; }
        }

        public int NumTris
        {
            get { return numTris; }
            set { numTris = value; }
        }

        public Vector3[] TriangleNormals
        {
            get { return triangleNormals; }
            set { triangleNormals = value; }
        }

        public float[] TriangleAreas
        {
            get { return triangleAreas; }
            set { triangleAreas = value; }
        }

        public List<int[]> TriangleVertexInfo
        {
            get { return triangleVertexInfo; }
            set { triangleVertexInfo = value; }
        }

        //----------------------------------------------------------------

        public TexturedPrimitive(Game game, string textureFile)
        {
            this.game = game;
            this.textureFile = textureFile;
        }

        internal abstract void CreateVertexBuffer();
        internal abstract void CreateIndexBuffer();

        public void SetVertexPosition(int vertexId, Vector3 position)
        {
            foreach (int realVertexId in vertexMappingPseudoToReal[vertexId])
            {
                this.vertices[realVertexId].Position = position;
            }
        }

        public Vector3 GetVertexPosition(int vertexId)
        {
            return this.vertices[vertexMappingPseudoToReal[vertexId][0]].Position;
        }

        public void SetVertexNormal(int vertexId, Vector3 normal)
        {
            foreach (int realVertexId in vertexMappingPseudoToReal[vertexId])
            {
                this.vertices[realVertexId].Normal = normal;
            }
        }

        public void AddVertexNormal(int vertexId, Vector3 normal)
        {
            foreach (int realVertexId in vertexMappingPseudoToReal[vertexId])
            {
                this.vertices[realVertexId].Normal += normal;
            }
        }

        public Vector3 GetVertexNormal(int vertexId)
        {
            return this.vertices[vertexMappingPseudoToReal[vertexId][0]].Normal;
        }

        Vector3 point1, point2, point3;
        int thisTriVertex1Id, thisTriVertex2Id, thisTriVertex3Id;
        public void RecalculateNormals()
        {
            //reset all vertex normals first
            for (int i = 0; i < this.numVertices; i++)
            {
                this.SetVertexNormal(i, Vector3.Zero);
            }

            //iterate through all tris
            for (int i = 0; i < this.triangleVertexInfo.Count; i++)
            {
                //get the vertex ids of the current triangle
                thisTriVertex1Id = this.triangleVertexInfo[i][0];
                thisTriVertex2Id = this.triangleVertexInfo[i][1];
                thisTriVertex3Id = this.triangleVertexInfo[i][2];

                //get the positions of those vertices
                point1 = this.GetVertexPosition(thisTriVertex1Id);
                point2 = this.GetVertexPosition(thisTriVertex2Id);
                point3 = this.GetVertexPosition(thisTriVertex3Id);
                //point1 = this.vertices[thisTriVertex1Id].Position;
                //point2 = this.vertices[thisTriVertex2Id].Position;
                //point3 = this.vertices[thisTriVertex3Id].Position;

                //update tri normal
                triangleNormals[i] = -Vector3.Cross(point3 - point2, point1 - point2);
                if (triangleNormals[i] != Vector3.Zero) triangleNormals[i].Normalize();

                //update triangle area
                triangleAreas[i] = triangleNormals[i].Length();

                //add the tri normal to the vertices that are sharing it
                this.AddVertexNormal(thisTriVertex1Id, triangleNormals[i]);
                this.AddVertexNormal(thisTriVertex2Id, triangleNormals[i]);
                this.AddVertexNormal(thisTriVertex3Id, triangleNormals[i]);
                //this.vertices[thisTriVertex1Id].Normal += triangleNormals[i];
                //this.vertices[thisTriVertex2Id].Normal += triangleNormals[i];
                //this.vertices[thisTriVertex3Id].Normal += triangleNormals[i];
            }
        }

        public void CommitChanges()
        {
            //commit any vertex position and normal changes
            game.GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
        }

        public float RecalculateVolume()
        {
            float volume = 0;
            Vector3 point1, point2, point3;
            for (int i = 0; i < this.numTris; i++)
            {
                point1 = GetVertexPosition(triangleVertexInfo[i][0]);
                point2 = GetVertexPosition(triangleVertexInfo[i][1]);
                point3 = GetVertexPosition(triangleVertexInfo[i][2]);
                volume += triangleAreas[i] * triangleNormals[i].X * (point1.X + point2.X + point3.X);
            }
            return volume / 3;
        }

        public void Initialize()
        {
            cameraComponent = (CameraComponent)game.Services.GetService(typeof(ICameraComponent));
        }

        public virtual void LoadContent()
        {
            effect = new BasicEffect(game.GraphicsDevice, null);
            texture = game.Content.Load<Texture2D>(textureFile);
        }

        public virtual void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            game.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            game.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One;
            game.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            game.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
            game.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;

            effect.TextureEnabled = true;
            effect.Texture = texture;
            effect.Projection = cameraComponent.Projection;
            effect.View = cameraComponent.View;
            effect.World = Matrix.Identity;
            effect.EnableDefaultLighting();
            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
                game.GraphicsDevice.Indices = indexBuffer;
                game.GraphicsDevice.VertexDeclaration = new VertexDeclaration(game.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
                game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numTris);

                pass.End();
            }
            effect.End();
        }
    }
}
