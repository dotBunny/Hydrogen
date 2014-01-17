#region Copyright Notice & License Information
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
using UnityEngine;

namespace Hydrogen.Threading.Jobs
{
		public class MeshCombiner : ThreadPoolJob
		{
				readonly List<MeshInput> _meshInputs = new List<MeshInput> ();
				readonly List<MeshOutput> _meshOutputs = new List<MeshOutput> ();
				readonly List<TransitionMesh> _transitionMeshes = new List<TransitionMesh> ();
				int[] _transitionMeshCounter = new int[Mesh.VerticesArrayLimit];
				/// <summary>
				/// Reference to Action to be used when ThreadedFunction is completed, however it requires that
				/// the Check method be called by Unity's main thread periodically. This will simply passback the 
				/// MeshDescriptions which then can be processed vai a coroutine/etc.
				/// </summary>
				Action<int, MeshOutput[]> _callback;
				/// <summary>
				/// An internal hash used to identify the Combine method instance.
				/// </summary>
				int _hash;
				/// <summary>
				/// A dictionary of Materials referenced by the added meshes.
				/// </summary>
				Dictionary<int, UnityEngine.Material> _materialLookup = new Dictionary<int, UnityEngine.Material> ();

				/// <summary>
				/// Material reference .
				/// </summary>
				/// <value>The combined Materials.</value>
				public Dictionary<int, UnityEngine.Material> MaterialsLookup {
						get { return _materialLookup; }
				}

				public bool AddMaterial (UnityEngine.Material material)
				{
						// Cache our generating of the lookup code.
						int check = material.GetDataHashCode ();

						// Check if we have an entry already, and if we do not add it
						if (!_materialLookup.ContainsKey (check)) {
								_materialLookup.Add (check, material);
								return true;
						}

						return false;
				}

				public bool AddMaterial (int code, UnityEngine.Material material)
				{
						if (!_materialLookup.ContainsKey (code)) {
								_materialLookup.Add (code, material);
								return true;
						}
						return false;
				}

				public bool AddMesh (MeshFilter meshFilter, Renderer renderer, Transform transform)
				{


						// If we add te
						if (AddMesh (CreateMeshInput (meshFilter, renderer, transform))) {

								return true;
						}
						return false;
				}

				public bool AddMesh (MeshInput newMeshDescription)
				{
				
						if (!_meshInputs.Contains (newMeshDescription)) {
								_meshInputs.Add (newMeshDescription);
								return true;
						}
						return false;
				}

				public void ClearMaterials ()
				{
						_materialLookup.Clear ();
				}

				public void ClearMeshes ()
				{
						_meshInputs.Clear ();
						_meshOutputs.Clear ();
						_transitionMeshes.Clear ();
						_transitionMeshCounter = new int[Mesh.VerticesArrayLimit];
				}

				public int Combine (Action<int, MeshOutput[]> onFinished)
				{
						return Combine (System.Threading.ThreadPriority.Normal, onFinished);
				}

				public int Combine (System.Threading.ThreadPriority priority, 
				                    Action<int, MeshOutput[]> onFinished)
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

				public MeshInput CreateMeshInput (MeshFilter meshFilter, Renderer renderer, Transform transform)
				{
						var newMeshInput = new MeshInput ();

						newMeshInput.Mesh = new BufferedMesh ();
						newMeshInput.Mesh.Name = meshFilter.name;
						newMeshInput.Mesh.Vertices = meshFilter.sharedMesh.vertices;
						newMeshInput.Mesh.Normals = meshFilter.sharedMesh.normals;
						newMeshInput.Mesh.Colors = meshFilter.sharedMesh.colors;
						newMeshInput.Mesh.Tangets = meshFilter.sharedMesh.tangents;
						newMeshInput.Mesh.UV = meshFilter.sharedMesh.uv;
						newMeshInput.Mesh.UV1 = meshFilter.sharedMesh.uv1;
						newMeshInput.Mesh.UV2 = meshFilter.sharedMesh.uv2;

						for (var i = 0; i < meshFilter.sharedMesh.subMeshCount; i++) {
								var indexes = meshFilter.sharedMesh.GetIndices (i);
								newMeshInput.Mesh.Indexes.Add (indexes);
						}

						// Create Materials
						newMeshInput.Materials = MaterialsToMaterialDataHashCodes (renderer.sharedMaterials);
						newMeshInput.LocalToWorldMatrix = transform.localToWorldMatrix;

						return newMeshInput;
				}

				public MeshInput[] CreateMeshInputs (MeshFilter[] meshFilters, Renderer[] renderer, Transform[] transforms)
				{
						// Create our holder
						var meshInputs = new MeshInput[meshFilters.Length];

						// Lazy way of making a whole bunch.
						for (int i = 0; i < meshFilters.Length; i++) {
								meshInputs [i] = CreateMeshInput (meshFilters [i], renderer [i], transforms [i]);
						}

						// Send it back!
						return meshInputs;
				}

				public MeshObject CreateMeshObject (MeshOutput transitionMesh)
				{
						var meshObject = new MeshObject ();


						meshObject.Materials = MaterialDataHashCodesToMaterials (transitionMesh.Materials.ToArray ());

						meshObject.Mesh = new UnityEngine.Mesh ();
						meshObject.Mesh.vertices = transitionMesh.Positions.ToArray ();

						// If there are normals we need to assign them to the mesh.
						if (transitionMesh.Normals != null) {
								meshObject.Mesh.normals = transitionMesh.Normals.ToArray ();
						}

						// Much like normals, if we've got tangents lets throw them on there too.
						if (transitionMesh.Tangents != null) {
								meshObject.Mesh.tangents = transitionMesh.Tangents.ToArray ();
						}

						// How about some vertex color data? Sounds like a good idea to add that too.
						if (transitionMesh.Colors != null) {
								meshObject.Mesh.colors = transitionMesh.Colors.ToArray ();
						}

						// Better make those textures work too while were at it.
						if (transitionMesh.UV != null) {
								meshObject.Mesh.uv = transitionMesh.UV.ToArray ();
						}

						// How about some more UV's?
						if (transitionMesh.UV1 != null) {
								meshObject.Mesh.uv1 = transitionMesh.UV1.ToArray ();
						}

						// Lightmapping UV's anyone?
						if (transitionMesh.UV2 != null) {
								meshObject.Mesh.uv2 = transitionMesh.UV2.ToArray ();
						}


						meshObject.Mesh.subMeshCount = transitionMesh.Indexes.Count;
						for (int i = 0; i < transitionMesh.Indexes.Count; i++) {
								meshObject.Mesh.SetIndices (transitionMesh.Indexes [i].ToArray (), MeshTopology.Triangles, i);
						}

						// Recalculate mesh's bounds for fun.
						meshObject.Mesh.RecalculateBounds ();

						// Return our processed object.
						return meshObject;
				}

				public UnityEngine.Material[] MaterialDataHashCodesToMaterials (int[] codes)
				{
						UnityEngine.Material[] materials = new UnityEngine.Material[codes.Length];
						for (int x = 0; x < codes.Length; x++) {
								materials [x] = _materialLookup [codes [x]];
						}
						return materials;
				}

				public int[] MaterialsToMaterialDataHashCodes (UnityEngine.Material[] materials)
				{

						var hashcodes = new int[materials.Length];
						for (var x = 0; x < materials.Length; x++) {
								// Generate Code
								int newCode = materials [x].GetDataHashCode ();

								// Add if we need it
								AddMaterial (newCode, materials [x]);

								// Assign Code
								hashcodes [x] = newCode;
						}
						return hashcodes;
				}

				public bool RemoveMaterial (UnityEngine.Material material)
				{
						int check = material.GetDataHashCode ();
						if (_materialLookup.ContainsKey (check)) {
								_materialLookup.Remove (check);
								return true;
						}
						return false;
				}

				public bool RemoveMesh (MeshFilter meshFilter, Renderer renderer, Transform transform)
				{
						MeshInput meshInput = CreateMeshInput (meshFilter, renderer, transform);
						return RemoveMesh (meshInput);
				}

				public bool RemoveMesh (MeshInput meshInput)
				{
						if (_meshInputs.Contains (meshInput)) {
								_meshInputs.Remove (meshInput);
								return true;
						}
						return false;

				}

				protected sealed override void ThreadedFunction ()
				{
						try {
								// Empty out preexisting parsed data
								_transitionMeshes.Clear ();
								_meshOutputs.Clear ();


								foreach (var meshRend in _meshInputs) {

										var mesh = meshRend.Mesh;
										var subMeshCount = mesh.SubMeshCount;
										var vertices = mesh.Vertices;
										var normals = mesh.Normals;
										var Colors = mesh.Colors;
										var tangents = mesh.Tangets;
										var uv = mesh.UV;
										var uv1 = mesh.UV1;
										var uv2 = mesh.UV2;

										int lastValidMaterial = 0;
										for (var i = 0; i < subMeshCount; i++) {
												var tm = new TransitionMesh ();
												var indexes = mesh.GetIndices (i);

												if (i > (meshRend.Materials.Length - 1)) {
														tm.Material = meshRend.Materials [lastValidMaterial];
												} else {
														tm.Material = meshRend.Materials [i];
														lastValidMaterial = i;
												}

												tm.VertexCount = CountUsedVertices (mesh, ref indexes);
												tm.IndexCount = indexes.Length;

												tm.Positions = new Vector3[tm.VertexCount];

												tm.Indexes = new int[tm.IndexCount];

												var inversedTransposedMatrix = meshRend.LocalToWorldMatrix.inverse.transpose;

												for (var j = 0; j < tm.IndexCount; j++) {
														var index = indexes [j];
														var kindex = _transitionMeshCounter [index];
														tm.Indexes [j] = kindex;
												}

												for (var j = 0; j < tm.IndexCount; j++) {
														var index = indexes [j];
														var kindex = _transitionMeshCounter [index];
														var vertex = vertices [index];

														// TODO: Issue with putting object where it was
														tm.Positions [kindex] = meshRend.LocalToWorldMatrix.MultiplyPoint (vertex);
												}

												if (mesh.Normals != null && mesh.Normals.Length > 0) {
														tm.Normals = new Vector3[tm.VertexCount];
														for (var j = 0; j < tm.IndexCount; j++) {
																var index = indexes [j];
																var kindex = _transitionMeshCounter [index];
																var normal = normals [index];
																tm.Normals [kindex] = inversedTransposedMatrix.MultiplyVector (normal).normalized;
														}
												}

												if (mesh.Colors != null && mesh.Colors.Length > 0) {
														tm.Colors = new Color[tm.VertexCount];
														for (var j = 0; j < tm.IndexCount; j++) {
																var index = indexes [j];
																var kindex = _transitionMeshCounter [index];
																var Color = Colors [index];
																tm.Colors [kindex] = Color;
														}
												}

												if (mesh.Tangets != null && mesh.Tangets.Length > 0) {
														tm.Tangents = new Vector4[tm.VertexCount];
														for (var j = 0; j < tm.IndexCount; j++) {
																var index = indexes [j];
																var kindex = _transitionMeshCounter [index];
																var p = tangents [index];
																var w = p.w;
																p = inversedTransposedMatrix.MultiplyVector (p);
																tm.Tangents [kindex] = new Vector4 (p.x, p.y, p.z, w);
														}
												}

												if (mesh.UV != null && mesh.UV.Length > 0) {
														tm.UV = new Vector2[tm.VertexCount];
														for (var j = 0; j < tm.IndexCount; j++) {
																var index = indexes [j];
																var kindex = _transitionMeshCounter [index];
																tm.UV [kindex] = uv [index];
														}
												}

												if (mesh.UV1 != null && mesh.UV1.Length > 0) {
														tm.UV1 = new Vector2[tm.VertexCount];
														for (var j = 0; j < tm.IndexCount; j++) {
																var index = indexes [j];
																var kindex = _transitionMeshCounter [index];
																tm.UV1 [kindex] = uv1 [index];
														}
												}

												if (mesh.UV2 != null && mesh.UV2.Length > 0) {
														tm.UV2 = new Vector2[tm.VertexCount];
														for (var j = 0; j < tm.IndexCount; j++) {
																var index = indexes [j];
																var kindex = _transitionMeshCounter [index];
																tm.UV2 [kindex] = uv2 [index];
														}
												}

												_transitionMeshes.Add (tm);
										}
								}


								// MAKE TASKS
								_transitionMeshes.Sort (new TransitionMeshSorter ());

								// when making TMTs.
								// It should onlt move to the next TMT either;
								//  the vertex count is to high!
								//  the bitmask has changed.
								// We should use as many submeshes as possible, keeping individual index
								// counts for all of them (a list).

								var tmt = new MeshOutput ();
								int bitmask = _transitionMeshes [0].GetBitMask ();

								foreach (var tmesh in _transitionMeshes) {
										if (tmesh.GetBitMask () != bitmask || tmesh.VertexCount + tmt.VertexCount > Mesh.VerticesArrayLimit) {
												_meshOutputs.Add (tmt);
												tmt = new MeshOutput ();
										}

										var baseIndex = tmt.VertexCount;
										tmt.VertexCount += tmesh.VertexCount;
										tmt.SortedSources.Add (tmesh);
										tmt.Positions.AddRange (tmesh.Positions);

										if (tmesh.Normals != null) {
												tmt.Normals.AddRange (tmesh.Normals);
										}

										if (tmesh.Colors != null) {
												tmt.Colors.AddRange (tmesh.Colors);
										}

										if (tmesh.Tangents != null) {
												tmt.Tangents.AddRange (tmesh.Tangents);
										}

										if (tmesh.UV != null) {
												tmt.UV.AddRange (tmesh.UV);
										}

										if (tmesh.UV1 != null) {
												tmt.UV1.AddRange (tmesh.UV1);
										}

										if (tmesh.UV2 != null) {
												tmt.UV2.AddRange (tmesh.UV2);
										}

										var indexes = tmt.GetSubMesh (tmesh.Material);
										indexes.Capacity = indexes.Count + tmesh.IndexCount;
										for (var i = 0; i < tmesh.IndexCount; i++) {
												indexes.Add (baseIndex + tmesh.Indexes [i]);
										}

										bitmask = tmesh.GetBitMask ();
								}

								_meshOutputs.Add (tmt);

						} catch (Exception e) {
								Debug.Log (e.Message);
								Debug.Log (e.StackTrace);
						}
				}

				protected sealed override void OnFinished ()
				{
						// Callback
						if (_callback != null)
								_callback (_hash, _meshOutputs.ToArray ());
				}

				int CountUsedVertices (BufferedMesh mesh, ref int[] indexes)
				{
						int vc = mesh.VertexCount;

						for (var i = 0; i < vc; i++)
								_transitionMeshCounter [i] = 0;

						for (var i = 0; i < indexes.Length; i++)
								_transitionMeshCounter [indexes [i]] = 1;

						int count = 0;

						for (var i = 0; i < vc; i++) {
								var c = count;
								count += _transitionMeshCounter [i];
								_transitionMeshCounter [i] = c;
						}
								
						return count;
				}

				public class BufferedMesh
				{
						// TODO: Add TopologyType eventually to handle this.
						public Color[] Colors;
						public List<int[]> Indexes = new List<int[]> ();
						public String Name;
						public Vector3[] Normals;
						public Vector4[] Tangets;
						public Vector2[] UV;
						public Vector2[] UV1;
						public Vector2[] UV2;
						public Vector3[] Vertices;

						public int VertexCount {
								get { return Vertices.Length; }
						}

						public int SubMeshCount {
								get { return Indexes.Count; }
						}

						public int[] GetIndices (int targetIndex)
						{
								return Indexes [targetIndex];
						}
				}

				public class MeshInput
				{
						public BufferedMesh Mesh;
						public Matrix4x4 LocalToWorldMatrix;
						public int[] Materials;
				}

				public class MeshObject
				{
						public UnityEngine.Mesh Mesh;
						public UnityEngine.Material[] Materials;
				}

				public class MeshOutput
				{
						public List<TransitionMesh> SortedSources = new List<TransitionMesh> ();
						public List<int> Materials = new List<int> ();
						public int VertexCount;
						public List<Vector3> Positions = new List<Vector3> ();
						public List<Color> Colors = new List<Color> ();
						public List<Vector3> Normals = new List<Vector3> ();
						public List<Vector4> Tangents = new List<Vector4> ();
						public List<Vector2> UV = new List<Vector2> ();
						public List<Vector2> UV1 = new List<Vector2> ();
						public List<Vector2> UV2 = new List<Vector2> ();
						public List<List<int>> Indexes = new List<List<int>> ();

						public List<int> GetSubMesh (int mat)
						{
								for (int i = 0; i < Materials.Count; i++) {

										var m = Materials [i];
										if (m == mat) {
												return Indexes [i];
										}
								}
								Materials.Add (mat);
								var indexes = new List<int> ();
								Indexes.Add (indexes);
								return indexes;
						}
				}

				public class TransitionMesh
				{
						//public Matrix4x4 localToWorldMatrix;
						public int[] Indexes;
						public int IndexCount;
						public Color[] Colors;
						public int Material;
						public Vector3[] Normals;
						public Vector3[] Positions;
						public Vector4[] Tangents;
						public Vector2[] UV;
						public Vector2[] UV1;
						public Vector2[] UV2;
						public int VertexCount;

						public int GetBitMask ()
						{
								int mask = 0x0;
								if (Colors != null)
										mask |= 1;
								if (Normals != null)
										mask |= 2;
								if (Tangents != null)
										mask |= 4;
								if (UV != null)
										mask |= 8;
								if (UV1 != null)
										mask |= 16;
								if (UV2 != null)
										mask |= 32;
								return mask;
						}
				}

				public class TransitionMeshSorter : IComparer<TransitionMesh>
				{
						public int Compare (TransitionMesh x, TransitionMesh y)
						{
								// Sort by features first? uv1, uv2, colours, tangents?  Like wise features should be grouped together
								// It is an 'AND' logic, and not 'OR'.  You can't have optional colours!
								int compare = x.GetBitMask ().CompareTo (y.GetBitMask ());
								if (compare != 0)
										return compare;

								// Then Sort by meshes -> meshes with similar meshes grouped together

								compare = x.Material.CompareTo (y.Material);
								if (compare != 0)
										return compare;

								// Then sort by size (largest to smallest) -> larger tmeshes more likely to share fewer tmeshes per mesh.
								return x.VertexCount.CompareTo (y.VertexCount);
						}
				}
		}
}