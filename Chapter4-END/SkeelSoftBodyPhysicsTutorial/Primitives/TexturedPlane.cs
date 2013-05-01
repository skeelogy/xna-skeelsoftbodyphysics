using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkeelSoftBodyPhysicsTutorial.Primitives
{
    public sealed class TexturedPlane : TexturedPrimitive
    {
        //for length and width
        private float width;
        private float length;
        private int widthSegments;
        private int lengthSegments;
        private float gridWidth;
        private float gridHeight;

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        public float Length
        {
            get { return length; }
            set { length = value; }
        }

        public int WidthSegments
        {
            get { return widthSegments; }
            set { widthSegments = value; }
        }

        public int LengthSegments
        {
            get { return lengthSegments; }
            set { lengthSegments = value; }
        }

        //-----------------------------------------------------------------

        public TexturedPlane(Game game, float length, float width, int lengthSegments, int widthSegments, string textureFile)
            : base(game, textureFile)
        {
            this.length = length;
            this.width = width;
            this.lengthSegments = lengthSegments;
            this.widthSegments = widthSegments;
            this.gridWidth = (float)length / (float)lengthSegments;
            this.gridHeight = (float)width / (float)widthSegments;

            this.numRealVertices = (lengthSegments + 1) * (widthSegments + 1);
            this.numTris = lengthSegments * widthSegments * 2;
            this.numVertices = numRealVertices;

            CreateVertexBuffer();
            CreateIndexBuffer();

            //allocate space for the triangle normals (used when updating normals)
            triangleNormals = new Vector3[numTris];
            triangleAreas = new float[numTris];
            RecalculateNormals();
            CommitChanges();
        }

        internal override void CreateVertexBuffer()
        {
            vertices = new VertexPositionNormalTexture[(lengthSegments + 1) * (widthSegments + 1)];
            for (int x = 0; x < lengthSegments + 1; x++)
            {
                for (int y = 0; y < widthSegments + 1; y++)
                {
                    vertices[x + y * (lengthSegments + 1)].Position = new Vector3(x * gridWidth - length / 2, 0, -y * gridHeight + width / 2);
                    vertices[x + y * (lengthSegments + 1)].TextureCoordinate = new Vector2((float)x / lengthSegments, (float)y / widthSegments);
                    vertices[x + y * (lengthSegments + 1)].Normal = new Vector3(0, 1, 0);
                }
            }
            VertexBuffer vb = new VertexBuffer(game.GraphicsDevice, sizeof(float) * 8 * (lengthSegments + 1) * (widthSegments + 1), BufferUsage.WriteOnly);
            vb.SetData<VertexPositionNormalTexture>(vertices);
            this.vertexBuffer = vb;

            //create the vertex mapping:
            //for plane, there are no dup vertices, so the pseudo vertex id is just the real vertex id
            for (int i = 0; i < numVertices; i++)
            {
                vertexMappingPseudoToReal.Add(new List<int> { i });
            }
        }

        internal override void CreateIndexBuffer()
        {
            int[] indices = new int[lengthSegments * widthSegments * 6];
            for (int x = 0; x < lengthSegments; x++)
            {
                for (int y = 0; y < widthSegments; y++)
                {
                    //specify the indices for the first tri
                    indices[(x + y * lengthSegments) * 6] = (x + 1) + (y + 1) * (lengthSegments + 1);
                    indices[(x + y * lengthSegments) * 6 + 1] = (x + 1) + y * (lengthSegments + 1);
                    indices[(x + y * lengthSegments) * 6 + 2] = x + y * (lengthSegments + 1);

                    //store triangle vertex info for future use
                    int[] triangleVertex = new int[3];
                    triangleVertex[0] = indices[(x + y * lengthSegments) * 6];
                    triangleVertex[1] = indices[(x + y * lengthSegments) * 6 + 1];
                    triangleVertex[2] = indices[(x + y * lengthSegments) * 6 + 2];
                    triangleVertexInfo.Add(triangleVertex);

                    //specify the indices for the second tri
                    indices[(x + y * lengthSegments) * 6 + 3] = (x + 1) + (y + 1) * (lengthSegments + 1);
                    indices[(x + y * lengthSegments) * 6 + 4] = x + y * (lengthSegments + 1);
                    indices[(x + y * lengthSegments) * 6 + 5] = x + (y + 1) * (lengthSegments + 1);

                    //store triangleVertex info for future use
                    triangleVertex = new int[3];
                    triangleVertex[0] = indices[(x + y * lengthSegments) * 6 + 3];
                    triangleVertex[1] = indices[(x + y * lengthSegments) * 6 + 4];
                    triangleVertex[2] = indices[(x + y * lengthSegments) * 6 + 5];
                    triangleVertexInfo.Add(triangleVertex);
                }
            }

            IndexBuffer ib = new IndexBuffer(game.GraphicsDevice, typeof(int), this.lengthSegments * this.widthSegments * 6, BufferUsage.WriteOnly);
            ib.SetData<int>(indices);
            this.indexBuffer = ib;
        }
    }
}
