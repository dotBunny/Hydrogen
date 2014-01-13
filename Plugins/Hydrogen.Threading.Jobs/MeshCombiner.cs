﻿#region Copyright Notice & License Information
//
// MeshCombiner.cs
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

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Hydrogen.Threading.Jobs
{
		public class MeshCombiner : ThreadPoolJob
		{
				Action<int, UnityEngine.Mesh[]> _callback;
				/// <summary>
				/// An array of MeshDescriptions generated by the Combine method. 
				/// </summary>
				MeshDescription[] _combinedDescriptions = new MeshDescription[0];
				/// <summary>
				/// An array of Materials generated by the Combine method.
				/// </summary>
				Material[] _combinedMaterials = new Material[0];
				/// <summary>
				/// An array of Meshes generated by the Combine method.
				/// </summary>
				UnityEngine.Mesh[] _combinedMeshes = new UnityEngine.Mesh[0];
				/// <summary>
				/// An internal hash used to identify the Combine method instance.
				/// </summary>
				int _hash;
				/// <summary>
				/// An internally used datastore housing added mesh data to be combined by the Combine method.
				/// </summary>
				MeshDescription[] _meshDescriptions = new MeshDescription[0];

				/// <summary>
				/// Gets the array of MeshDescriptions generated by the Combine method. 
				/// </summary>
				/// <value>The the combined MeshDescritions.</value>
				public MeshDescription[] CombinedDescriptions {
						get { return _combinedDescriptions; }
				}

				/// <summary>
				/// Gets the array of Materials generated by the Combine method.
				/// </summary>
				/// <value>The combined Materials.</value>
				public Material[] CombinedMaterials {
						get { return _combinedMaterials; }
				}

				/// <summary>
				/// Gets the array of Meshes generated by the Combine method.
				/// </summary>
				/// <value>The combined Meshes.</value>
				public UnityEngine.Mesh[] CombinedMeshes {
						get { return _combinedMeshes; }
				}

				/// <summary>
				/// Creates a thread safe dataset used when combining meshes via the MeshCombiner.
				/// </summary>
				/// <returns>The parsed MeshDescription.</returns>
				/// <param name="mesh">Target Mesh.</param>
				/// <param name="transform">Target Transform.</param>
				public static MeshDescription CreateDescription (UnityEngine.Mesh mesh, Transform transform)
				{
						var vertexCount = mesh.vertexCount;
						var md = new MeshDescription (vertexCount);

						md.VertexObject.Vertices.CopyFrom (mesh.vertices);
						md.VertexObject.Normals.CopyFrom (mesh.normals);
						md.VertexObject.Tangents.CopyFrom (mesh.tangents);
						md.VertexObject.Colors.CopyFrom (mesh.colors);
						md.VertexObject.UV.CopyFrom (mesh.uv);
						md.VertexObject.UV1.CopyFrom (mesh.uv1);
						md.VertexObject.UV2.CopyFrom (mesh.uv2);
						md.VertexObject.WorldTransform = transform.localToWorldMatrix;

						var nbSubMeshes = mesh.subMeshCount;

						for (var j = 0; j < nbSubMeshes; j++) {
								Material mat = null;
								var indices = mesh.GetIndices (j);
								var sm = md.AddSubMesh (mat, indices.Length);
								sm.Indices.CopyFrom (indices);
						} 

						return md;
				}

				/// <summary>
				/// Creates the thread safe dataset used when combining meshes via the MeshCombiner.
				/// </summary>
				/// <returns>The parsed MeshDescriptions.</returns>
				/// <param name="meshes">Target Mesh.</param>
				/// <param name="transforms">Target Transform.</param>
				public static MeshDescription[] CreateDescriptions (UnityEngine.Mesh[] meshes, Transform[] transforms)
				{
						var mds = new MeshDescription[meshes.Length];

						for (int i = 0; i < meshes.Length; i++) {
								mds [i] = CreateDescription (meshes [i], transforms [i]);
						}
						return mds;
				}

				/// <summary>
				/// Add a Mesh to be combined.
				/// </summary>
				/// <returns><c>true</c>, if mesh was added, <c>false</c> otherwise.</returns>
				/// <param name="meshDescription">The MeshDescription to be added.</param>
				public bool AddMesh (MeshDescription meshDescription)
				{
						return Array.Add<MeshDescription> (ref _meshDescriptions, meshDescription);
				}

				/// <summary>
				/// Add a Mesh to be combined.
				/// </summary>
				/// <returns><c>true</c>, if mesh was added, <c>false</c> otherwise.</returns>
				/// <param name="mesh">The Mesh to be added.</param>
				/// <param name="transform">The Mesh's transform.</param>
				public bool AddMesh (UnityEngine.Mesh mesh, Transform transform)
				{
						return Array.Add<MeshDescription> (ref _meshDescriptions, CreateDescription (mesh, transform));
				}

				public int Combine (Action<int, UnityEngine.Mesh[]> onFinished)
				{
						return Combine (System.Threading.ThreadPriority.Normal, onFinished);
				}

				public int Combine (System.Threading.ThreadPriority priority, Action<int, UnityEngine.Mesh[]> onFinished)
				{
						// Generate Hash Code
						_hash = (Time.time + UnityEngine.Random.Range (0, 100)).GetHashCode ();

						// Start the threaded prcess
						if (onFinished != null) {
								_callback = onFinished;
						}

						Start (true, priority);

						return _hash;
				}

				public bool RemoveMesh (MeshDescription meshDescription)
				{
						return Array.Remove (ref _meshDescriptions, meshDescription);
				}
				//TODO: Add optimization to mesh data?
				protected sealed override void ThreadedFunction ()
				{
						// Combine meshes
						// Find all of the materials that are being used by the meshes
						// We do this so we make only one mesh (at best, per material)
						var seen = new List<Material> (_meshDescriptions.Length);

						// Itterate through all of the descriptions, and 
						foreach (var m in _meshDescriptions) {
								foreach (var sm in m.SubMeshes) {
										if (!seen.Contains (sm.SharedMaterial)) {
												seen.Add (sm.SharedMaterial);
										}
								}
						}

						// Here are the materials we're using!
						_combinedMaterials = seen.ToArray ();

						// Create a temporary set of meshes based on the number of materials that are being used.
						var multiMeshDescriptions = new MultiMeshDescription[_combinedMaterials.Length];


						// Initialize each new mesh with their respective material
						for (int i = 0; i < _combinedMaterials.Length; i++) {
								multiMeshDescriptions [i] = new MultiMeshDescription (_combinedMaterials [i]);
						}


						// Itterate through all of the MeshDescriptions present to be combined.
						foreach (var meshDescription in _meshDescriptions) {
						
								// Itterate through all SubMeshes in the MeshDescription
								foreach (var subMesh in meshDescription.SubMeshes) {
								
										// Itterate through all of our MultiMeshDescriptions adding meshes that
										// have the same material as its own.
										foreach (var multiMesh in multiMeshDescriptions) {

												if (multiMesh.SharedMaterial == subMesh.SharedMaterial) {
														multiMesh.AddSubMesh (subMesh);
														break;
												}
										}
								}
						}

						// Create our finalized MeshDescription holder, at the size of the materials
						var finalMeshDescriptions = new List<MeshDescription> (_combinedMaterials.Length);

						// Itterate over our MultiMeshDescriptions again and have them execute their 
						// Combine method forcing them down into MeshDescriptions.
						foreach (var multiMesh in multiMeshDescriptions) {
								finalMeshDescriptions.AddRange (multiMesh.Combine ());
						}

						// Assign the finalized MeshDescriptions to our internal storage.
						_combinedDescriptions = finalMeshDescriptions.ToArray ();
				}

				protected sealed override void OnFinished ()
				{
						// Create our mesh
						CreateMeshes ();

						// Callback
						if (_callback != null)
								_callback (_hash, _combinedMeshes);

				}

				void CreateMeshes ()
				{
						_combinedMeshes = new UnityEngine.Mesh[_combinedDescriptions.Length];

						for (int x = 0; x <= _combinedDescriptions.Length; x++) {

								_combinedMeshes [x] = new UnityEngine.Mesh ();
								_combinedMeshes [x].name = "MeshCombiner_" + _hash + "_" + x;

								_combinedMeshes [x].vertices = _combinedDescriptions [x].VertexObject.Vertices.ToArray ();
								_combinedMeshes [x].normals = _combinedDescriptions [x].VertexObject.Normals.ToArray ();
								_combinedMeshes [x].tangents = _combinedDescriptions [x].VertexObject.Tangents.ToArray ();
								_combinedMeshes [x].colors = _combinedDescriptions [x].VertexObject.Colors.ToArray ();
								_combinedMeshes [x].uv = _combinedDescriptions [x].VertexObject.UV.ToArray ();
								_combinedMeshes [x].uv1 = _combinedDescriptions [x].VertexObject.UV1.ToArray ();
								_combinedMeshes [x].uv2 = _combinedDescriptions [x].VertexObject.UV2.ToArray ();


								// TODO: Um the indices?
								//	_combinedMeshes [x].SetIndices (_combinedDescriptions [x].VertexObject.Indices, MeshTopology.Triangles, 0);
						}
				}

				public class VertexArrayDescription<T>
				{
						public readonly int Size;
						readonly T[] values;

						public bool HasValues { get; private set; }

						public VertexArrayDescription (int nbVertices)
						{
								Size = nbVertices;
								values = new T[Size];
								HasValues = false;
						}

						public T this [int i] {
								get { return values [i]; }
								set {
										values [i] = value;
										HasValues = true;
								}
						}

						public T[] ToArray ()
						{
								return values;
						}

						public void CopyFrom (T[] other)
						{
								if (other != null && other.Length > 0) {
										// TODO Exception/Assert here when size != values.length
										for (var i = 0; i < Size; i++) {
												values [i] = other [i];
										}
										HasValues = true;
								}
						}
				}

				public class IndexArrayDescription
				{
						public int Size { get; private set; }

						readonly int[] values;

						public IndexArrayDescription (int nbIndexes)
						{
								Size = nbIndexes;
								if (Size % 3 != 0) {
										Debug.Log ("Bad index array, count is not a multiple of 3!");
										return;
								}
								values = new int[Size];
						}

						public int this [int i] {
								get { return values [i]; }
								set { values [i] = value; }
						}

						internal void CopyFrom (int[] other)
						{
								for (var i = 0; i < Size; i++) {
										values [i] = other [i];
								}
						}
				}

				public class MeshDescription
				{
						public readonly List<SubMeshDescription> SubMeshes;
						public readonly VertexObjectDescription VertexObject;

						public MeshDescription (int verticesCount)
						{
								VertexObject = new VertexObjectDescription (verticesCount);
								SubMeshes = new List<SubMeshDescription> ();
						}

						public SubMeshDescription AddSubMesh (Material sharedMaterial, int nbIndexes)
						{
								var smd = new SubMeshDescription (nbIndexes, VertexObject, sharedMaterial);
								SubMeshes.Add (smd);
								return smd;
						}

						internal void DebugPrint (StringBuilder sb)
						{
								sb.AppendFormat ("Mesh#{0:X8}\n", GetHashCode ());
								sb.AppendFormat ("Vertices.Size={0}\n", VertexObject.Size);
								sb.AppendFormat ("Vertices.Vertices =[{0},{1},{2}], [{3},{4},{5}], [{6},{7},{8}]...\n", VertexObject.Vertices [0].x, VertexObject.Vertices [1].y, VertexObject.Vertices [2].z, VertexObject.Vertices [3].x, VertexObject.Vertices [4].y, VertexObject.Vertices [5].z, VertexObject.Vertices [6].x, VertexObject.Vertices [7].y, VertexObject.Vertices [8].z);
								sb.AppendFormat ("Vertices.Normals={0}\n", VertexObject.Normals.HasValues);
								sb.AppendFormat ("Vertices.Tangents={0}\n", VertexObject.Tangents.HasValues);
								sb.AppendFormat ("Vertices.Colours={0}\n", VertexObject.Colors.HasValues);
								sb.AppendFormat ("Vertices.UV={0}\n", VertexObject.UV.HasValues);
								sb.AppendFormat ("Vertices.UV1={0}\n", VertexObject.UV1.HasValues);
								sb.AppendFormat ("Vertices.UV2={0}\n", VertexObject.UV2.HasValues);
								sb.AppendFormat ("Vertices.WorldTransform={0}\n", VertexObject.WorldTransform.ToString ().Replace ('\n', ' '));
								sb.AppendFormat ("SubMesh.Count={0}\n", SubMeshes.Count);
								for (var i = 0; i < SubMeshes.Count; i++) {
										var sm = SubMeshes [i];
										sb.AppendFormat ("SubMesh[{0}].Indexes={1}\n", i, sm.Indices.Size);
								}
						}
				}

				public class MultiMeshDescription
				{
						public readonly Material SharedMaterial;
						public readonly List<SubMeshDescription> SubMeshes;

						public MultiMeshDescription (Material material)
						{
								SharedMaterial = material;
								SubMeshes = new List<SubMeshDescription> (4);
						}

						public void AddSubMesh (SubMeshDescription sm)
						{
								SubMeshes.Add (sm);
						}

						public MeshDescription[] Combine ()
						{

								// Create our holder 
								var finalMeshDescriptions = new List<MeshDescription> ();

								// Determine the total number of vertices accross submeshes
								int TotalVerticesCount = 0;
								foreach (var subMesh in SubMeshes) {
										TotalVerticesCount += subMesh.CountUsedVertices ();
								}

								// Divide up meshes equally with a maxium vertex limit
								var nbVerticesPerMesh = new List<int> ();
								int vertexCount = 0;
								while (vertexCount != TotalVerticesCount) {

										int used = vertexCount;

										if (used > (Hydrogen.Mesh.VerticesLimit - 6)) {
												used = Hydrogen.Mesh.VerticesLimit - 6;
										}

										nbVerticesPerMesh.Add (used);
										vertexCount += used;
								}

								//MeshDescription newMesh = new MeshDescription ()


								// TODO: Create meshDescription(s)
								// TODO: Add to finalMeshDescription


								// Return our MeshDescriptions
								return finalMeshDescriptions.ToArray ();
						}
				}

				public class SubMeshDescription
				{
						public readonly IndexArrayDescription Indices;
						public readonly Material SharedMaterial;
						public readonly int[] Used;
						public readonly VertexObjectDescription VertexObject;

						public SubMeshDescription (int indexCount, VertexObjectDescription vertices, Material material)
						{
								SharedMaterial = material;
								VertexObject = vertices;
								Indices = new IndexArrayDescription (indexCount);
								Used = new int[vertices.Size];
						}

						public int CountUsedVertices ()
						{
								int count = 0;

								// Count used vertices.  
								// This uses an 'if-less' solution so should be faster but 
								// uses 4x (int > bool) memory do to so.
								for (int i = 0; i < Used.Length; i++)
										Used [i] = 0;

								for (int i = 0; i < Indices.Size; i++)
										Used [Indices [i]] = 1;

								for (int i = 0; i < Indices.Size; i++)
										count += Used [i];

								return count;
						}
				}

				public class VertexObjectDescription
				{
						public readonly VertexArrayDescription<Color> Colors;
						public readonly VertexArrayDescription<Vector3> Normals;
						public readonly int Size;
						public readonly VertexArrayDescription<Vector4> Tangents;
						public readonly VertexArrayDescription<Vector2> UV;
						public readonly VertexArrayDescription<Vector2> UV1;
						public readonly VertexArrayDescription<Vector2> UV2;
						public readonly VertexArrayDescription<bool> Used;
						public readonly VertexArrayDescription<Vector3> Vertices;
						public Matrix4x4 WorldTransform;

						public VertexObjectDescription (int verticesCount)
						{
								Size = verticesCount;
								Vertices = new VertexArrayDescription<Vector3> (Size);
								Normals = new VertexArrayDescription<Vector3> (Size);
								Tangents = new VertexArrayDescription<Vector4> (Size);
								Colors = new VertexArrayDescription<Color> (Size);
								UV = new VertexArrayDescription<Vector2> (Size);
								UV1 = new VertexArrayDescription<Vector2> (Size);
								UV2 = new VertexArrayDescription<Vector2> (Size);
								Used = new VertexArrayDescription<bool> (Size);
								WorldTransform = Matrix4x4.identity;
						}
				}
		}
}
