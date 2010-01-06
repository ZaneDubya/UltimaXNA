/***************************************************************************
 *   RayPicker.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion


namespace UltimaXNA.TileEngine
{
    class RayPicker : DrawableGameComponent
    {
        // Store the name of the model underneath the cursor (or null if there is none).
        public string pickedModelName;
        public MapObject pickedObject;
        public List<string> insideBoundingSpheres = new List<string>();

        // Vertex array that stores exactly which triangle was picked.
        VertexPositionColor[] pickedTriangle =
        {
            new VertexPositionColor(Vector3.Zero, Color.Magenta),
            new VertexPositionColor(Vector3.Zero, Color.Magenta),
            new VertexPositionColor(Vector3.Zero, Color.Magenta),
            new VertexPositionColor(Vector3.Zero, Color.Magenta)
        };

        // Effect and vertex declaration for drawing the picked triangle.
        BasicEffect lineEffect;
        VertexDeclaration lineVertexDeclaration;

        public RayPicker(Game game)
            : base(game)
        {
            lineEffect = new BasicEffect(game.GraphicsDevice, null);
            lineEffect.VertexColorEnabled = true;
            lineVertexDeclaration = new VertexDeclaration(game.GraphicsDevice,
                VertexPositionColorTexture.VertexElements);

            m_PickObjects = new List<PickingObject>();
        }

        public void DrawPickedTriangle(Matrix nProjectionMatrix)
        {
            if (pickedModelName != null)
            {
                RenderState renderState = this.Game.GraphicsDevice.RenderState;

                // Set line drawing renderstates. We disable backface culling
                // and turn off the depth buffer because we want to be able to
                // see the picked triangle outline regardless of which way it is
                // facing, and even if there is other geometry in front of it.
                renderState.FillMode = FillMode.WireFrame;
                renderState.CullMode = CullMode.None;
                renderState.DepthBufferEnable = false;

                // Activate the line drawing BasicEffect.
                lineEffect.Projection = nProjectionMatrix;
                // lineEffect.View = camera.View;

                lineEffect.Begin();
                lineEffect.CurrentTechnique.Passes[0].Begin();

                // Draw the triangle.
                this.Game.GraphicsDevice.VertexDeclaration = lineVertexDeclaration;

                this.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip,
                                          pickedTriangle, 0, 2);

                lineEffect.CurrentTechnique.Passes[0].End();
                lineEffect.End();

                // Reset renderstates to their default values.
                renderState.FillMode = FillMode.Solid;
                renderState.CullMode = CullMode.CullCounterClockwiseFace;
                renderState.DepthBufferEnable = true;
            }
        }

        List<PickingObject> m_PickObjects;
        
        public void FlushObjects()
        {
            m_PickObjects.Clear();
        }
        public void AddObject(MapObject nObject, VertexPositionNormalTextureHue[] nVertices)
        {
            Vector3[] iVertices = new Vector3[nVertices.Length];
            for (int i = 0; i < nVertices.Length; i++)
                iVertices[i] = nVertices[i].Position;
            PickingObject o = new PickingObject(nObject, iVertices);
            m_PickObjects.Add(o);
        }


        #region PickingCode
        public bool PickTest(Vector2 nPosition, Matrix nProjectionMatrix, Matrix nViewMatrix)
        {
            // Look up a collision ray based on the current cursor position. See the
            // Picking Sample documentation for a detailed explanation of this.
            Ray cursorRay = CalculateCursorRay(nPosition, nProjectionMatrix, nViewMatrix);

            // Clear the previous picking results.
            insideBoundingSpheres.Clear();

            pickedModelName = null;
            pickedObject = null;

            // Keep track of the closest object we have seen so far, so we can
            // choose the closest one if there are several models under the cursor.
            float closestIntersection = float.MinValue;

            // Loop over all our models.
            foreach (PickingObject iObject in m_PickObjects)
            {
                bool insideBoundingSphere;
                Vector3[] vertex;

                // Perform the ray to model intersection test.
                float? intersection = RayIntersectsModel(ref cursorRay, iObject,
                                                         out insideBoundingSphere,
                                                         out vertex);

                // If this model passed the initial bounding sphere test, remember
                // that so we can display it at the top of the screen.
                if (insideBoundingSphere)
                    insideBoundingSpheres.Add(iObject.IdentifierString);

                // Do we have a per-triangle intersection with this model?
                if (intersection != null)
                {
                    // If so, is it closer than any other model we might have
                    // previously intersected?
                    if (intersection > closestIntersection)
                    {
                        // Store information about this model.
                        closestIntersection = intersection.Value;

                        pickedModelName = iObject.IdentifierString;
                        pickedObject = iObject.Object;

                        // Store vertex positions so we can display the picked triangle.
                        for (int i = 0; i < 4; i++)
                        {
                            pickedTriangle[i].Position = vertex[i];
                        }
                    }
                }
            }

            if (pickedModelName != null)
                return true;
            else
                return false;
        }

        static Matrix _modelTransform = Matrix.Identity;
        static float? RayIntersectsModel(ref Ray ray, PickingObject model,
                                         out bool insideBoundingSphere,
                                         out Vector3[] vertex)
        {
            vertex = new Vector3[4];

            

            // The input ray is in world space, but our model data is stored in object
            // space. We would normally have to transform all the model data by the
            // modelTransform matrix, moving it into world space before we test it
            // against the ray. That transform can be slow if there are a lot of
            // triangles in the model, however, so instead we do the opposite.
            // Transforming our ray by the inverse modelTransform moves it into object
            // space, where we can test it directly against our model data. Since there
            // is only one ray but typically many triangles, doing things this way
            // around can be much faster.

            Matrix inverseTransform = Matrix.Invert(_modelTransform);

            ray.Position = Vector3.Transform(ray.Position, inverseTransform);
            ray.Direction = Vector3.TransformNormal(ray.Direction, inverseTransform);

            // Look up our custom collision data from the Tag property of the model.
            // Dictionary<string, object> tagData = (Dictionary<string, object>)model.Tag;

            // Start off with a fast bounding sphere test.

            bool iInRange = true; // model.InRange(ray.Position);

            //float? iReturn = boundingSphere.Intersects(ray);

            if (iInRange == false)
            {
                // If the ray does not intersect the bounding sphere, we cannot
                // possibly have picked this model, so there is no need to even
                // bother looking at the individual triangle data.
                insideBoundingSphere = false;

                return null;
            }
            else
            {
                // The bounding sphere test passed, so we need to do a full
                // triangle picking test.
                insideBoundingSphere = true;

                // Keep track of the closest triangle we found so far,
                // so we can always return the closest one.
                float? closestIntersection = null;

                // Loop over the vertex data, 3 at a time (3 vertices = 1 triangle).
                Vector3[] vertices = (Vector3[])model.Vertices;

                for (int i = 0; i < vertices.Length - 2; i++)
                {
                    // Perform a ray to triangle intersection test.
                    float? intersection;

                    RayIntersectsTriangle(ref ray,
                                          ref vertices[i],
                                          ref vertices[i + 1],
                                          ref vertices[i + 2],
                                          out intersection);

                    // Does the ray intersect this triangle?
                    if (intersection != null)
                    {
                        // If so, is it closer than any other previous triangle?
                        if ((closestIntersection == null) ||
                            (intersection > closestIntersection))
                        {
                            // Store the distance to this triangle.
                            closestIntersection = intersection;

                            // Transform the three vertex positions into world space,
                            // and store them into the output vertex parameters.
                            
                            for (int j = 0; j < 4; j++)
                            {
                                Vector3.Transform(ref vertices[j],
                                                  ref _modelTransform, out vertex[j]);
                            }
                            /*
                            Vector3.Transform(ref vertices[i + 1],
                                              ref modelTransform, out vertex2);

                            Vector3.Transform(ref vertices[i + 2],
                                              ref modelTransform, out vertex3);
                             */
                        }
                    }
                }

                return closestIntersection;
            }
        }
        /// <summary>
        /// Checks whether a ray intersects a triangle. This uses the algorithm
        /// developed by Tomas Moller and Ben Trumbore, which was published in the
        /// Journal of Graphics Tools, volume 2, "Fast, Minimum Storage Ray-Triangle
        /// Intersection".
        /// 
        /// This method is implemented using the pass-by-reference versions of the
        /// XNA math functions. Using these overloads is generally not recommended,
        /// because they make the code less readable than the normal pass-by-value
        /// versions. This method can be called very frequently in a tight inner loop,
        /// however, so in this particular case the performance benefits from passing
        /// everything by reference outweigh the loss of readability.
        /// </summary>
        static void RayIntersectsTriangle(ref Ray ray,
                                          ref Vector3 vertex1,
                                          ref Vector3 vertex2,
                                          ref Vector3 vertex3, out float? result)
        {
            // Compute vectors along two edges of the triangle.
            Vector3 edge1, edge2;

            Vector3.Subtract(ref vertex2, ref vertex1, out edge1);
            Vector3.Subtract(ref vertex3, ref vertex1, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref ray.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                result = null;
                return;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref ray.Position, ref vertex1, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                result = null;
                return;
            }

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return;
            }

            result = rayDistance;
        }
        #endregion

        // CalculateCursorRay Calculates a world space ray starting at the camera's
        // "eye" and pointing in the direction of the cursor. Viewport.Unproject is used
        // to accomplish this. see the accompanying documentation for more explanation
        // of the math behind this function.
        private Ray CalculateCursorRay(Vector2 nPosition, Matrix projectionMatrix, Matrix viewMatrix)
        {
            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 1 is as far away as possible.
            Vector3 nearSource = new Vector3(nPosition, 0f);
            Vector3 farSource = new Vector3(nPosition, 1f);

            // use Viewport.Unproject to tell what those two screen space positions
            // would be in world space. we'll need the projection matrix and view
            // matrix, which we have saved as member variables. We also need a world
            // matrix, which can just be identity.
            Vector3 nearPoint = this.Game.GraphicsDevice.Viewport.Unproject(nearSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            Vector3 farPoint = this.Game.GraphicsDevice.Viewport.Unproject(farSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            // find the direction vector that goes from the nearPoint to the farPoint
            // and normalize it....
            Vector3 direction = farSource - nearSource;
            direction.Normalize();

            // and then create a new ray using nearPoint as the source.
            return new Ray(nearSource, direction);
        }
    }

    class PickingObject
    {
        public MapObject Object;
        public Vector3[] Vertices;

        bool _hasCenter = false;
        Point _center;
        public Point CenterPoint
        {
            get
            {
                if (!_hasCenter)
                {
                    _hasCenter = true;
                    float x = 0f, y = 0f;
                    for (int i = 0; i < Vertices.Length; i++)
                    {
                        x += Vertices[i].X;
                        y += Vertices[i].Y;
                    }
                    _center = new Point((int)(x / Vertices.Length), (int)(y / Vertices.Length));
                }
                return _center;
            }
        }

        public PickingObject(MapObject nObject, Vector3[] nVertices)
        {
            Object = nObject;
            Vertices = nVertices;
        }

        public string IdentifierString
        {
            get { return Vertices[0].ToString(); }
        }

        const float m_InRange = 44f;

        public bool InRange(Vector3 nPosition)
        {
            if (Utility.InRange(new Point((int)Vertices[0].X, (int)Vertices[0].Y), new Point((int)nPosition.X, (int)nPosition.Y), (int)m_InRange))
                return true;
            if (Utility.InRange(new Point((int)Vertices[3].X, (int)Vertices[3].Y), new Point((int)nPosition.X, (int)nPosition.Y), (int)m_InRange))
                return true;
            return false;
        }
    }
}
