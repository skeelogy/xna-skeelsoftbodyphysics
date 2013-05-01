using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkeelSoftBodyPhysicsTutorial.Primitives
{
    public sealed class TexturedCylinder : TexturedPrimitive
    {
        private float length;
        private float radius;
        private int lengthSegments;
        private int radialSegments;
        private float lengthSegmentSize;
        private float radialSegmentSize;

        public int RadialSegments
        {
            get { return radialSegments; }
            set { radialSegments = value; }
        }

        public int LengthSegments
        {
            get { return lengthSegments; }
            set { lengthSegments = value; }
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public float Length
        {
            get { return length; }
            set { length = value; }
        }

        //----------------------------------------------------------------

        public TexturedCylinder(Game game, float length, float radius, int lengthSegments, int radialSegments, string textureFile)
            : base(game, textureFile)
        {
            this.length = length;
            this.radius = radius;
            this.lengthSegments = lengthSegments;
            this.radialSegments = radialSegments;
            this.lengthSegmentSize = length / lengthSegments;
            this.radialSegmentSize = 360.0f / radialSegments;

            this.numRealVertices = (radialSegments + 1) * (lengthSegments + 1);
            this.numTris = lengthSegments * radialSegments * 2;
            this.numVertices = radialSegments * (lengthSegments + 1);

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
            vertices = new VertexPositionNormalTexture[numRealVertices];
            for (int x = 0; x < lengthSegments + 1; x++)
            {
                for (int y = 0; y < radialSegments + 1; y++)
                {
                    vertices[y + x * (radialSegments + 1)].Position = new Vector3(radius * (float)Math.Cos(MathHelper.ToRadians(radialSegmentSize * y)), lengthSegmentSize * x, radius * (float)Math.Sin(MathHelper.ToRadians(radialSegmentSize * y)));
                    vertices[y + x * (radialSegments + 1)].TextureCoordinate = new Vector2(1.0f - y / (float)(radialSegments), 1.0f - x / (float)(lengthSegments));
                }
            }
            VertexBuffer vb = new VertexBuffer(game.GraphicsDevice, sizeof(float) * 8 * numRealVertices, BufferUsage.WriteOnly);
            vb.SetData<VertexPositionNormalTexture>(vertices);
            this.vertexBuffer = vb;

            //create vertex mapping
            //cylinder has dup vertices at the UV seam, so those has to be combined
            vertexMappingRealToPseudo = new int[numRealVertices];
            for (int x = 0; x < lengthSegments + 1; x++)
            {
                for (int y = 0; y < radialSegments; y++)
                {
                    if (y != 0)
                    {
                        vertexMappingPseudoToReal.Add(new List<int> { y + x * (radialSegments + 1) });
                        vertexMappingRealToPseudo[y + x * (radialSegments + 1)] = y + x * (radialSegments + 1) - x;
                    }
                    else
                    {
                        vertexMappingPseudoToReal.Add(new List<int> { y + x * (radialSegments + 1), y + x * (radialSegments + 1) + radialSegments });
                        vertexMappingRealToPseudo[y + x * (radialSegments + 1)] = y + x * (radialSegments + 1) - x;
                        vertexMappingRealToPseudo[y + x * (radialSegments + 1) + radialSegments] = y + x * (radialSegments + 1) - x;
                    }
                }
            }
        }

        internal override void CreateIndexBuffer()
        {
            int[] indices = new int[numTris * 3];
            int[] triangleVertex;
            for (int x = 0; x < lengthSegments; x++)
            {
                for (int y = 0; y < radialSegments; y++)
                {
                    //specify the indices for the first tri
                    indices[(y + x * radialSegments) * 6] = y + x * (radialSegments + 1);
                    indices[(y + x * radialSegments) * 6 + 1] = ((y + 1) + x * (radialSegments + 1));
                    indices[(y + x * radialSegments) * 6 + 2] = (y + (x + 1) * (radialSegments + 1));

                    //store triangle vertex info for future use
                    triangleVertex = new int[3];
                    triangleVertex[0] = indices[(y + x * radialSegments) * 6];
                    triangleVertex[1] = indices[(y + x * radialSegments) * 6 + 1];
                    triangleVertex[2] = indices[(y + x * radialSegments) * 6 + 2];
                    triangleVertex[0] = vertexMappingRealToPseudo[triangleVertex[0]];
                    triangleVertex[1] = vertexMappingRealToPseudo[triangleVertex[1]];
                    triangleVertex[2] = vertexMappingRealToPseudo[triangleVertex[2]];
                    triangleVertexInfo.Add(triangleVertex);

                    //specify the indices for the second tri
                    indices[(y + x * radialSegments) * 6 + 3] = ((y + 1) + x * (radialSegments + 1));
                    indices[(y + x * radialSegments) * 6 + 4] = ((y + 1) + (x + 1) * (radialSegments + 1));
                    indices[(y + x * radialSegments) * 6 + 5] = (y + (x + 1) * (radialSegments + 1)); 

                    //store triangleVertex info for future use
                    triangleVertex = new int[3];
                    triangleVertex[0] = indices[(y + x * radialSegments) * 6 + 3];
                    triangleVertex[1] = indices[(y + x * radialSegments) * 6 + 4];
                    triangleVertex[2] = indices[(y + x * radialSegments) * 6 + 5];
                    triangleVertex[0] = vertexMappingRealToPseudo[triangleVertex[0]];
                    triangleVertex[1] = vertexMappingRealToPseudo[triangleVertex[1]];
                    triangleVertex[2] = vertexMappingRealToPseudo[triangleVertex[2]];
                    triangleVertexInfo.Add(triangleVertex);
                }
            }

            IndexBuffer ib = new IndexBuffer(game.GraphicsDevice, typeof(int), numTris * 3, BufferUsage.WriteOnly);
            ib.SetData<int>(indices);
            this.indexBuffer = ib;
        }
    }
}
