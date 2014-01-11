#region Copyright Notice & License Information
//
// Mesh.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
//       Robin Southern <betajaen@ihoed.com>
//
// Copyright (c) 2014 dotBunny Inc. (http://www.dotbunny.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

namespace Hydrogen
{
		/// <summary>
		/// Additional static functions and constants used to extend existing Mesh support inside of Unity.
		/// </summary>
		public static class Mesh
		{
				/// <summary>
				/// The maximum vertices allowed in a Mesh.
				/// </summary>
				/// <remarks>This is a Unity imposed limitation, presumably this may grow over time.</remarks>
				public const int VerticesLimit = 65536;

				/// <summary>
				/// Return the index of the closest vertex to the targetPosition on the Mesh.
				/// </summary>
				/// <returns>The vertex index on the mesh.</returns>
				/// <param name="mesh">The mesh to compare against.</param>
				/// <param name="targetPosition">The position to check against.</param>
				public static int NearestVertexIndex (this UnityEngine.Mesh mesh, UnityEngine.Vector3 targetPosition)
				{
						// Create local reference
						UnityEngine.Vector3[] vertices = mesh.vertices;

						// Our best distance so far
						float nearestDistance = UnityEngine.Mathf.Infinity;

						// The index (-1) if we don't find one
						int best = vertices.Length - 1;

						for (var i = 0; i < (best + 1); i++) {
								UnityEngine.Vector3 vertex = vertices [i];
								UnityEngine.Vector3 difference = targetPosition - vertex;

								float differenceDist = difference.sqrMagnitude;
				
								if (differenceDist < nearestDistance) {
										nearestDistance = differenceDist;
										best = i;
								}
						}

						// Send it back!
						return best;
				}
		}
}