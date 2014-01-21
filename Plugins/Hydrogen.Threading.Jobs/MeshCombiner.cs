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
using System.Threading;

#endregion
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hydrogen.Threading.Jobs
{
		/// <summary>
		/// A MeshCombiner that runs in another thread.
		/// </summary>
		public class MeshCombiner : ThreadPoolJob
		{
				readonly List<MeshInput> _meshInputs = new List<MeshInput> ();
				readonly List<MeshOutput> _meshOutputs = new List<MeshOutput> ();
				readonly List<TransitionMesh> _transitionMeshes = new List<TransitionMesh> ();
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
				readonly Dictionary<int, UnityEngine.Material> _materialLookup = new Dictionary<int, UnityEngine.Material> ();

				/// <summary>
				/// Material reference .
				/// </summary>
				/// <value>The combined Materials.</value>
				public Dictionary<int, UnityEngine.Material> MaterialsLookup {
						get { return _materialLookup; }
				}

				public int MeshInputCount {
						get { return _meshInputs.Count; }
				}

				public int MeshOutputCount {
						get { return _meshOutputs.Count; }
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

				public bool AddMesh (MeshFilter meshFilter, Renderer renderer, Matrix4x4 localToWorldMatrix)
				{
						// If we add te
						if (AddMesh (CreateMeshInput (meshFilter, renderer, localToWorldMatrix))) {

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

				public MeshInput CreateMeshInput (MeshFilter meshFilter, Renderer renderer, Matrix4x4 localToWorldMatrix)
				{
						var newMeshInput = new MeshInput ();

						newMeshInput.Mesh = new BufferedMesh ();
						newMeshInput.Mesh.Name = meshFilter.name;

						newMeshInput.Mesh.Vertices = meshFilter.sharedMesh.vertices;

						newMeshInput.Mesh.Normals = meshFilter.sharedMesh.normals;
						newMeshInput.Mesh.Colors = meshFilter.sharedMesh.colors;
						newMeshInput.Mesh.Tangents = meshFilter.sharedMesh.tangents;
						newMeshInput.Mesh.UV = meshFilter.sharedMesh.uv;
						newMeshInput.Mesh.UV1 = meshFilter.sharedMesh.uv1;
						newMeshInput.Mesh.UV2 = meshFilter.sharedMesh.uv2;

						for (var i = 0; i < meshFilter.sharedMesh.subMeshCount; i++) {
								var indexes = meshFilter.sharedMesh.GetIndices (i);
								newMeshInput.Mesh.Indexes.Add (indexes);
						}

						// Create Materials
						newMeshInput.Materials = MaterialsToMaterialDataHashCodes (renderer.sharedMaterials);

						newMeshInput.LocalToWorldMatrix = localToWorldMatrix;

						return newMeshInput;
				}

				public MeshInput[] CreateMeshInputs (MeshFilter[] meshFilters, Renderer[] renderers, Matrix4x4[] localToWorldMatrices)
				{
						// Create our holder
						var meshInputs = new MeshInput[meshFilters.Length];

						// Lazy way of making a whole bunch.
						for (int i = 0; i < meshFilters.Length; i++) {
								meshInputs [i] = CreateMeshInput (meshFilters [i], renderers [i], localToWorldMatrices [i]);
						}

						// Send it back!
						return meshInputs;
				}

				public MeshObject CreateMeshObject (MeshOutput meshOutput)
				{
						var meshObject = new MeshObject ();


						meshObject.Materials = MaterialDataHashCodesToMaterials (meshOutput.Materials.ToArray ());

						meshObject.Mesh = new UnityEngine.Mesh ();
						meshObject.Mesh.vertices = meshOutput.Positions.ToArray ();
						meshObject.Mesh.name = "Combined Mesh (" + meshObject.Mesh.vertices.GetHashCode () + ")";

						// If there are normals we need to assign them to the mesh.
						if (meshOutput.Normals != null) {
								meshObject.Mesh.normals = meshOutput.Normals.ToArray ();
						}

						// Much like normals, if we've got tangents lets throw them on there too.
						if (meshOutput.Tangents != null) {
								meshObject.Mesh.tangents = meshOutput.Tangents.ToArray ();
						}

						// How about some vertex color data? Sounds like a good idea to add that too.
						if (meshOutput.Colors != null) {
								meshObject.Mesh.colors = meshOutput.Colors.ToArray ();
						}

						// Better make those textures work too while were at it.
						if (meshOutput.UV != null) {
								meshObject.Mesh.uv = meshOutput.UV.ToArray ();
						}

						// How about some more UV's?
						if (meshOutput.UV1 != null) {
								meshObject.Mesh.uv1 = meshOutput.UV1.ToArray ();
						}

						// Lightmapping UV's anyone?
						if (meshOutput.UV2 != null) {
								meshObject.Mesh.uv2 = meshOutput.UV2.ToArray ();
						}


						meshObject.Mesh.subMeshCount = meshOutput.Indexes.Count;
						for (int i = 0; i < meshOutput.Indexes.Count; i++) {
								meshObject.Mesh.SetIndices (meshOutput.Indexes [i].ToArray (), MeshTopology.Triangles, i);
						}

						// Recalculate mesh's bounds for fun.
						meshObject.Mesh.RecalculateBounds ();


						// Return our processed object.
						return meshObject;
				}

				public UnityEngine.Material[] MaterialDataHashCodesToMaterials (int[] codes)
				{
						// TODO: Evaluate if it would be better to instance these, instead of referencing the root again
						var materials = new UnityEngine.Material[codes.Length];
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

				public bool RemoveMesh (MeshFilter meshFilter, Renderer renderer, Matrix4x4 localToWorldMatrix)
				{
						MeshInput meshInput = CreateMeshInput (meshFilter, renderer, localToWorldMatrix);
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

						// Empty out preexisting parsed data
						_transitionMeshes.Clear ();
						_meshOutputs.Clear ();

						// Clever forker solution since we don't have Parallel.ForEach support available.
						var parallelTasks = new Forker ();
						foreach (var meshInput in _meshInputs) {
								var tempInput = meshInput;
								parallelTasks.Fork (delegate {
										CreateTransitionMesh (tempInput);
								});
						}
						parallelTasks.Join ();

						// Sort the meshes in order
						_transitionMeshes.Sort (new TransitionMeshSorter ());

						// when making TMTs.
						// It should onlt move to the next TMT either;
						//  the vertex count is to high!
						//  the bitmask has changed.
						// We should use as many submeshes as possible, keeping individual index
						// counts for all of them (a list).

						var meshOutput = new MeshOutput ();
						int bitmask = _transitionMeshes [0].GetBitMask ();
					

						foreach (var transitionMesh in _transitionMeshes) {
								if (transitionMesh.GetBitMask () != bitmask ||
								    (transitionMesh.VertexCount + meshOutput.VertexCount) > Mesh.VerticesArrayLimit) {
										_meshOutputs.Add (meshOutput);
										meshOutput = new MeshOutput ();
								}
												
								var baseIndex = meshOutput.VertexCount;
								meshOutput.VertexCount += transitionMesh.VertexCount;
								meshOutput.SortedSources.Add (transitionMesh);
								meshOutput.Positions.AddRange (transitionMesh.Positions);

								if (transitionMesh.Normals != null) {
										meshOutput.Normals.AddRange (transitionMesh.Normals);
								}

								if (transitionMesh.Colors != null) {
										meshOutput.Colors.AddRange (transitionMesh.Colors);
								}

								if (transitionMesh.Tangents != null) {
										meshOutput.Tangents.AddRange (transitionMesh.Tangents);
								}

								if (transitionMesh.UV != null) {
										meshOutput.UV.AddRange (transitionMesh.UV);
								}

								if (transitionMesh.UV1 != null) {
										meshOutput.UV1.AddRange (transitionMesh.UV1);
								}

								if (transitionMesh.UV2 != null) {
										meshOutput.UV2.AddRange (transitionMesh.UV2);
								}

								var indexes = meshOutput.GetSubMesh (transitionMesh.Material);
								indexes.Capacity = indexes.Count + transitionMesh.IndexCount;
								for (var i = 0; i < transitionMesh.IndexCount; i++) {
										indexes.Add (baseIndex + transitionMesh.Indexes [i]);
								}

								bitmask = transitionMesh.GetBitMask ();
						}
						_meshOutputs.Add (meshOutput);
				}

				protected sealed override void OnFinished ()
				{
						// Callback
						if (_callback != null)
								_callback (_hash, _meshOutputs.ToArray ());
				}

				/// <summary>
				/// Threaded creation of a TransitionMesh from a MeshInput.
				/// </summary>
				/// <param name="meshInput">Mesh input.</param>
				void CreateTransitionMesh (MeshInput meshInput)
				{
						var mesh = meshInput.Mesh;
						var subMeshCount = mesh.SubMeshCount;
						var vertices = mesh.Vertices;
						var normals = mesh.Normals;
						var Colors = mesh.Colors;
						var tangents = mesh.Tangents;
						var uv = mesh.UV;
						var uv1 = mesh.UV1;
						var uv2 = mesh.UV2;
						var transitionMeshCounter = new int [mesh.VertexCount];
						var inversedTransposedMatrix = meshInput.LocalToWorldMatrix.inverse.transpose;

						for (var i = 0; i < subMeshCount; i++) {
								var newTransitionMesh = new TransitionMesh ();
								var indexes = mesh.GetIndices (i);

								if (i > (meshInput.Materials.Length - 1)) {
										// If there is no material, dont add the mesh later to be rendered
										// as it wasn't showing anyways!.
										continue;
								}
								newTransitionMesh.Material = meshInput.Materials [i];

								// Determine how many actual vertices are to be used.
								// Former Count function
								int vc = mesh.VertexCount;
								int count = 0;

								for (int j = 0; j < indexes.Length; j++) {
										transitionMeshCounter [indexes [j]] = 1;
								}
								for (int j = 0; j < vc; j++) {
										int c = count;
										count += transitionMeshCounter [j];
										transitionMeshCounter [j] = c;
								}		
								newTransitionMesh.VertexCount = count;

								// Assign the rest of the things
								newTransitionMesh.IndexCount = indexes.Length;
								newTransitionMesh.Positions = new Vector3[newTransitionMesh.VertexCount];
								newTransitionMesh.Indexes = new int[newTransitionMesh.IndexCount];

								for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
										var index = indexes [j];
										var kindex = transitionMeshCounter [index];
										newTransitionMesh.Indexes [j] = kindex;
								}

								// Handle Vertices
								for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
										var index = indexes [j];
										var kindex = transitionMeshCounter [index];
										var vertex = vertices [index];
										newTransitionMesh.Positions [kindex] = meshInput.LocalToWorldMatrix.MultiplyPoint (vertex);
								}

								// Handle Normals
								if (mesh.Normals != null && mesh.Normals.Length > 0) {
										newTransitionMesh.Normals = new Vector3[newTransitionMesh.VertexCount];
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												var kindex = transitionMeshCounter [index];
												var normal = normals [index];
												newTransitionMesh.Normals [kindex] = inversedTransposedMatrix.MultiplyVector (normal).normalized;
										}
								}

								// Handle Colors
								if (mesh.Colors != null && mesh.Colors.Length > 0) {
										newTransitionMesh.Colors = new Color[newTransitionMesh.VertexCount];
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												var kindex = transitionMeshCounter [index];
												var Color = Colors [index];
												newTransitionMesh.Colors [kindex] = Color;
										}
								}

								// Handle Tangents
								if (mesh.Tangents != null && mesh.Tangents.Length > 0) {
										newTransitionMesh.Tangents = new Vector4[newTransitionMesh.VertexCount];
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												var kindex = transitionMeshCounter [index];
												var p = tangents [index];
												var w = p.w;
												p = inversedTransposedMatrix.MultiplyVector (p);
												newTransitionMesh.Tangents [kindex] = new Vector4 (p.x, p.y, p.z, w);
										}
								}


								// Handle UVs
								if (mesh.UV != null && mesh.UV.Length > 0) {
										newTransitionMesh.UV = new Vector2[newTransitionMesh.VertexCount];
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												var kindex = transitionMeshCounter [index];
												newTransitionMesh.UV [kindex] = uv [index];
										}
								}

								// Handle UV1s
								if (mesh.UV1 != null && mesh.UV1.Length > 0) {
										newTransitionMesh.UV1 = new Vector2[newTransitionMesh.VertexCount];
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												var kindex = transitionMeshCounter [index];
												newTransitionMesh.UV1 [kindex] = uv1 [index];
										}
								}

								// Handle UV2s
								if (mesh.UV2 != null && mesh.UV2.Length > 0) {
										newTransitionMesh.UV2 = new Vector2[newTransitionMesh.VertexCount];
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												var kindex = transitionMeshCounter [index];
												newTransitionMesh.UV2 [kindex] = uv2 [index];
										}
								}

								// Lock the reference array, and add in as it can.
								lock (_transitionMeshes) {
										_transitionMeshes.Add (newTransitionMesh);
								}

						}
				}

				public class BufferedMesh
				{
						// TODO: Add TopologyType eventually to handle this.
						public Color[] Colors;
						public List<int[]> Indexes = new List<int[]> ();
						public String Name;
						public Vector3[] Normals;
						public Vector4[] Tangents;
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