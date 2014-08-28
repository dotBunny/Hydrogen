#region Copyright Notice & License Information
//
// MeshCombiner.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
//       Robin Southern <betajaen@ihoed.com>
//	 Lars Simkins <lars.simkins@gmail.com>
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
		/// <summary>
		/// A Multi-Threaded Mesh Combiner that runs in another thread. (Yes! It is just that cool!)
		/// </summary>
		public class MeshCombiner : ThreadPoolJob
		{
				/// <summary>
				/// Reference to Action to be used when ThreadedFunction is completed, however it requires that
				/// the Check method be called by Unity's main thread periodically. This will simply passback the 
				/// MeshOutputs which then can be processed via a coroutine/etc.
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
				/// Internal MeshInput storage that will be used to create TransitionMeshes during processing.
				/// </summary>
				readonly List<MeshInput> _meshInputs = new List<MeshInput> ();
				/// <summary>
				/// Internal MeshOutput storage that will be passed back out to the PostProcess coroutine.
				/// </summary>
				readonly List<MeshOutput> _meshOutputs = new List<MeshOutput> ();
				/// <summary>
				/// Internal TransitionMesh storage used as a go between the MeshInput -> MeshOutput.
				/// </summary>
				/// <remarks>>Handy for allowing us to work in parallel on some of the heavy processing.</remarks>
				readonly List<TransitionMesh> _transitionMeshes = new List<TransitionMesh> ();

				/// <summary>
				/// Gets the current Material reference Dictionary.
				/// </summary>
				/// <value>The Material Reference Dictionary.</value>
				public Dictionary<int, UnityEngine.Material> MaterialsLookup {
						get { return _materialLookup; }
				}

				/// <summary>
				/// Gets the MeshInput(s) list to be used to create TransitionMeshes during the processing.
				/// </summary>
				/// <value>The MeshInput(s)</value>
				public List<MeshInput> MeshInputs {
						get { return _meshInputs; }
				}

				/// <summary>
				/// The total number of MeshInput(s) currently in the system.
				/// </summary>
				/// <value>The MeshInput(s) Count.</value>
				public int MeshInputCount {
						get { return _meshInputs.Count; }
				}

				/// <summary>
				/// Gets the MeshOutput(s) list, often used during post processing to create meshes.
				/// </summary>
				/// <value>The MeshOutput(s)</value>
				public List<MeshOutput> MeshOutputs {
						get { return _meshOutputs; }
				}

				/// <summary>
				/// The total number of MeshOutput(s) currently generated in the system.
				/// </summary>
				/// <value>The MeshOutput(s) Count.</value>
				public int MeshOutputCount {
						get { return _meshOutputs.Count; }
				}

				/// <summary>
				/// Add a Material to the Material reference Dictionary.
				/// </summary>
				/// <remarks>This can only be used from within Unity's main thread.</remarks>
				/// <returns>The DataHashCode</returns>
				/// <param name="material">The UnityEngine.Material to be added.</param>
				public int AddMaterial (UnityEngine.Material material)
				{
						return AddMaterial (material.GetDataHashCode (), material);

				}

				/// <summary>
				/// Add a Material to the Material reference Dictionary with a specific code.
				/// </summary>
				/// <remarks>This can only be used from within Unity's main thread.</remarks>
				/// <returns>The DataHashCode</returns>
				/// <param name="code">The code to store as the reference for the Material.</param>
				/// <param name="material">The UnityEngine.Material to be added.</param>
				public int AddMaterial (int code, UnityEngine.Material material)
				{
						if (!_materialLookup.ContainsKey (code)) {
								_materialLookup.Add (code, material);
						}
						return code;
				}

				/// <summary>
				/// Add an array of Material(s) to the reference Dictionary.
				/// </summary>
				/// <returns>The DataHashCode(s).</returns>
				/// <param name="materials">The UnityEngine.Material(s) to be added..</param>
				public int[] AddMaterials (UnityEngine.Material[] materials)
				{
						var hashcodes = new int[materials.Length];
						for (var x = 0; x < materials.Length; x++) {
								// Add if we need it
								hashcodes [x] = AddMaterial (materials [x].GetDataHashCode (), materials [x]);
						}
						return hashcodes;
				}

				/// <summary>
				/// Add a Unity based Mesh into the MeshCombiner.
				/// </summary>
				/// <remarks>This can only be used from within Unity's main thread.</remarks>
				/// <returns><c>true</c>, if mesh was added, <c>false</c> otherwise.</returns>
				public bool AddMesh (MeshFilter meshFilter, Renderer renderer, Matrix4x4 worldMatrix)
				{					
						return AddMesh (CreateMeshInput (meshFilter, renderer, worldMatrix));
				}

				/// <summary>
				/// Adds an existing MeshInput into the MeshCombiner.
				/// </summary>
				/// <remarks>
				/// This can be done without Unity's main thread, if you have access to the data already. 
				/// Just need to make sure that the material codes are set correctly.
				/// </remarks>
				/// <returns><c>true</c>, if MeshInput was added, <c>false</c> otherwise.</returns>
				/// <param name="meshInput">The MeshInput to add.</param>
				public bool AddMesh (MeshInput meshInput)
				{
						if (!_meshInputs.Contains (meshInput)) {
								_meshInputs.Add (meshInput);
								return true;
						}
						return false;
				}

				/// <summary>
				/// Clear the Material reference Dictionary.
				/// </summary>
				public void ClearMaterials ()
				{
						_materialLookup.Clear ();
				}

				/// <summary>
				/// Clear the all Mesh data inside of the MeshCombiner.
				/// </summary>
				public void ClearMeshes ()
				{
						_meshInputs.Clear ();
						_meshOutputs.Clear ();
						_transitionMeshes.Clear ();
				}

				/// <summary>
				/// Start the actual threaded process to combine MeshInput data currently added to the MeshCombiner.
				/// </summary>
				/// <param name="onFinished">The method to call when completed inside of Unity's main thread.</param>
				public int Combine (Action<int, MeshOutput[]> onFinished)
				{
						// Generate Hash Code
						_hash = (Time.time + UnityEngine.Random.Range (0, 100)).GetHashCode ();

						// Start the threaded prcess
						if (onFinished != null) {
								_callback = onFinished;
						}

						// These values mean nothing in this case as we don't assign any of it to the ThreadPool tasks.
						Start (true, System.Threading.ThreadPriority.Normal);

						return _hash;
				}

				/// <summary>
				/// Creates a MeshInput from the passed arguements.
				/// </summary>
				/// <remarks>This can only be used from within Unity's main thread.</remarks>
				/// <returns>The created MeshInput</returns>
				/// <param name="meshFilter">The source Mesh's MeshFilter.</param>
				/// <param name="renderer">The source Mesh's Renderer.</param>
				/// <param name="worldMatrix">The source Mesh's World Matrix</param>
				public MeshInput CreateMeshInput (MeshFilter meshFilter, Renderer renderer, Matrix4x4 worldMatrix)
				{
						var newMeshInput = new MeshInput ();

						newMeshInput.Mesh = new BufferedMesh ();
						newMeshInput.Mesh.Name = meshFilter.name;
						newMeshInput.Mesh.Vertices = meshFilter.sharedMesh.vertices;
						newMeshInput.Mesh.Normals = meshFilter.sharedMesh.normals;
						newMeshInput.Mesh.Colors = meshFilter.sharedMesh.colors;
						newMeshInput.Mesh.Tangents = meshFilter.sharedMesh.tangents;

						newMeshInput.Mesh.UV = meshFilter.sharedMesh.uv;
						newMeshInput.Mesh.UV2 = meshFilter.sharedMesh.uv2;
			
#if UNITY_5_0
						newMeshInput.Mesh.UV3 = meshFilter.sharedMesh.uv3;
						newMeshInput.Mesh.UV4 = meshFilter.sharedMesh.uv4;
#endif

						newMeshInput.Mesh.Topology = new MeshTopology[meshFilter.sharedMesh.subMeshCount];

						for (var i = 0; i < meshFilter.sharedMesh.subMeshCount; i++) {
								newMeshInput.Mesh.Topology [i] = meshFilter.sharedMesh.GetTopology (i);

								// Check for Unsupported Mesh Topology
								switch (newMeshInput.Mesh.Topology [i]) {
								case MeshTopology.Lines:
								case MeshTopology.LineStrip:
								case MeshTopology.Points:
										Debug.LogWarning ("The MeshCombiner does not support this meshes (" +
										newMeshInput.Mesh.Name + "topology (" + newMeshInput.Mesh.Topology [i] + ")");
										break;
								}
								newMeshInput.Mesh.Indexes.Add (meshFilter.sharedMesh.GetIndices (i));
						}

						// Create Materials
						newMeshInput.Materials = AddMaterials (renderer.sharedMaterials);


						// Determine Inversion of Scale
						// Don't Scale (NNP, PPP, PNN, NPN)
						Vector3 scaleTest = meshFilter.gameObject.transform.localScale;
						 
						bool invertedX = (scaleTest.x < 0f); 
						bool invertedY = (scaleTest.y < 0f);
						bool invertedZ = (scaleTest.z < 0f);

						if ((invertedX && invertedY && invertedZ) ||
						    (invertedX && !invertedY && !invertedZ) ||
						    (!invertedX && invertedY && !invertedZ) ||
						    (!invertedX && !invertedY && invertedZ)) {
								newMeshInput.ScaleInverted = true;
						} 
						newMeshInput.WorldMatrix = worldMatrix;

						return newMeshInput;
				}

				/// <summary>
				/// Creates MeshInput(s) from the passed arguement arrays.
				/// </summary>
				/// <remarks>This can only be used from within Unity's main thread.</remarks>
				/// <returns>The created MeshInput(s)</returns>
				/// <param name="meshFilters">The source Meshes MeshFilters.</param>
				/// <param name="renderers">The source Meshes Renderers.</param>
				/// <param name="worldMatrices">The source Meshes World Matrices</param>
				public MeshInput[] CreateMeshInputs (MeshFilter[] meshFilters, Renderer[] renderers, Matrix4x4[] worldMatrices)
				{
						// Create our holder
						var meshInputs = new MeshInput[meshFilters.Length];

						// Lazy way of making a whole bunch.
						for (int i = 0; i < meshFilters.Length; i++) {
								meshInputs [i] = CreateMeshInput (meshFilters [i], renderers [i], worldMatrices [i]);
						}

						// Send it back!
						return meshInputs;
				}

				/// <summary>
				/// Creates a MeshObject from the MeshOutput (Instanced Materials).
				/// </summary>
				/// <returns>The created MeshObject.</returns>
				/// <param name="meshOutput">The source MeshOutput to use in creating the MeshObject.</param>
				public MeshObject CreateMeshObject (MeshOutput meshOutput)
				{
						return CreateMeshObject (meshOutput, true);
				}

				/// <summary>
				/// Creates a MeshObject from the passed MeshOutput.
				/// </summary>
				/// <returns>The created MeshObject.</returns>
				/// <param name="meshOutput">The source MeshOutput to use in creating the MeshObject.</param>
				/// <param name="instanceMaterials">If set to <c>true</c> materials will be instanced.</param>
				public MeshObject CreateMeshObject (MeshOutput meshOutput, bool instanceMaterials)
				{
						var meshObject = new MeshObject ();

						meshObject.Materials = instanceMaterials ? 
								GetMaterialInstances (meshOutput.Materials.ToArray ()) : 
								GetMaterials (meshOutput.Materials.ToArray ());

						meshObject.Mesh = new UnityEngine.Mesh ();
						meshObject.Mesh.vertices = meshOutput.Vertices.ToArray ();
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
							


						if (meshOutput.UV2 != null) {
								meshObject.Mesh.uv2 = meshOutput.UV2.ToArray ();
						}

#if UNITY_5_0			
						if (meshOutput.UV3 != null) {
								meshObject.Mesh.uv3 = meshOutput.UV3.ToArray ();
						}
						if (meshOutput.UV4 != null) {
								meshObject.Mesh.uv4 = meshOutput.UV4.ToArray ();
						}
#endif

						meshObject.Mesh.subMeshCount = meshOutput.Indexes.Count;
						for (int i = 0; i < meshOutput.Indexes.Count; i++) {
								meshObject.Mesh.SetIndices (meshOutput.Indexes [i].ToArray (), MeshTopology.Triangles, i);
						}

						// Recalculate mesh's bounds for fun.
						meshObject.Mesh.RecalculateBounds ();

					
						// Return our processed object.
						return meshObject;
				}

				/// <summary>
				/// Creates MeshObject(s) from the passed MeshOutput(s) (Instanced Materials).
				/// </summary>
				/// <returns>The created MeshObject(s).</returns>
				/// <param name="meshOutputs">The source MeshOutput(s) to use in creating the MeshObject(s).</param>
				public MeshObject[] CreateMeshObjects (MeshOutput[] meshOutputs)
				{
						return CreateMeshObjects (meshOutputs, true);
				}

				/// <summary>
				/// Creates MeshObject(s) from the passed MeshOutput(s).
				/// </summary>
				/// <returns>The created MeshObject(s).</returns>
				/// <param name="meshOutputs">The source MeshOutput(s) to use in creating the MeshObject(s).</param>
				/// <param name="instanceMaterials">If set to <c>true</c> materials will be instanced.</param>
				public MeshObject[] CreateMeshObjects (MeshOutput[] meshOutputs, bool instanceMaterials)
				{
						MeshObject[] meshObjects = new MeshObject[meshOutputs.Length];
						for (int i = 0; i < meshOutputs.Length; i++) {
								meshObjects [i] = CreateMeshObject (meshOutputs [i], instanceMaterials);
						}
						return meshObjects;
				}

				/// <summary>
				/// Gets an array of Materials.
				/// </summary>
				/// <returns>The referenced Materials.</returns>
				/// <param name="codes">An array of Material DataHashCodes.</param>
				public UnityEngine.Material[] GetMaterials (int[] codes)
				{
						var materials = new UnityEngine.Material[codes.Length];
						for (int x = 0; x < codes.Length; x++) {
								materials [x] = _materialLookup [codes [x]];
						}
						return materials;
				}

				/// <summary>
				/// Gets an array of Materials (Instanced).
				/// </summary>
				/// <returns>The referenced Materials instanced.</returns>
				/// <param name="codes">An array of Material DataHashCodes.</param>
				public UnityEngine.Material[] GetMaterialInstances (int[] codes)
				{
						var materials = new UnityEngine.Material[codes.Length];
						for (int x = 0; x < codes.Length; x++) {
								materials [x] = new UnityEngine.Material (_materialLookup [codes [x]]);
								materials [x].name += " Instance";
						}
						return materials;
				}

				/// <summary>
				/// Remove a UnityEngine.Material from the Material reference Dictionary.
				/// </summary>
				/// <returns><c>true</c>, if UnityEngine.Material was removed, <c>false</c> otherwise.</returns>
				/// <param name="material">The target UnityEngine.Material to be removed.</param>
				public bool RemoveMaterial (UnityEngine.Material material)
				{
						int check = material.GetDataHashCode ();
						if (_materialLookup.ContainsKey (check)) {
								_materialLookup.Remove (check);
								return true;
						}
						return false;
				}

				/// <summary>
				/// Removes a Unity based Mesh from the MeshInput list to be processed.
				/// </summary>
				/// <remarks>
				/// This is useful if you are caching MeshInputs for future use, and just want to remove a mesh as its 
				/// no longer being combined. Open worlds may find this very useful. This can only be used from within 
				/// Unity's main thread.
				/// </remarks>
				/// <returns><c>true</c>, if the Unity based Mesh was removed, <c>false</c> otherwise.</returns>
				/// <param name="meshFilter">The Mesh's MeshFilter.</param>
				/// <param name="renderer">The Mesh's Renderer.</param>
				/// <param name="worldMatrix">The Mesh's World Matrix</param>
				public bool RemoveMesh (MeshFilter meshFilter, Renderer renderer, Matrix4x4 worldMatrix)
				{
						return RemoveMesh (CreateMeshInput (meshFilter, renderer, worldMatrix));
				}

				/// <summary>
				/// Removes a MeshInput from the MeshInput list to be processed.
				/// </summary>
				/// <remarks>
				/// This is useful if you are caching MeshInputs for future use, and just want to remove a mesh as its 
				/// no longer being combined. Open worlds may find this very useful.
				/// </remarks>
				/// <returns><c>true</c>, if the MeshInput was removed, <c>false</c> otherwise.</returns>
				/// <param name="meshInput">The MeshInput to be removed.</param>
				public bool RemoveMesh (MeshInput meshInput)
				{
						if (_meshInputs.Contains (meshInput)) {
								_meshInputs.Remove (meshInput);
								return true;
						}
						return false;

				}

				/// <summary>
				/// This is the main workhorse method which runs in another thread.
				/// </summary>
				/// <remarks>
				/// Takes the MeshInput(s) and converts them to TransitionMeshes in parallel. It then creates optimized 
				/// MeshOutput(s) for use later or through the callback.
				/// </remarks>
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

								meshOutput.Vertices.AddRange (transitionMesh.Vertices);

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

								if (transitionMesh.UV2 != null) {
										meshOutput.UV2.AddRange (transitionMesh.UV2);
								}
								if (transitionMesh.UV3 != null) {
										meshOutput.UV3.AddRange (transitionMesh.UV3);
								}
								if (transitionMesh.UV4 != null) {
										meshOutput.UV4.AddRange (transitionMesh.UV4);
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

				/// <summary>
				/// Executed when the ThreadedFunction is finished (sort of), sending our MeshOutput(s) back to Unity 
				/// for our coroutine to use.
				/// </summary>
				/// <remarks>Can use Unity API.</remarks>
				protected sealed override void OnFinished ()
				{
						// Callback
						if (_callback != null)
								_callback (_hash, _meshOutputs.ToArray ());
				}

				/// <summary>
				/// Thread safe creation of a TransitionMesh from a MeshInput.
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
						var uv3 = mesh.UV3;
						var uv2 = mesh.UV2;
						var uv4 = mesh.UV4;
						var inversedTransposedMatrix = meshInput.WorldMatrix.inverse.transpose;

						for (var i = 0; i < subMeshCount; i++) {
								var newTransitionMesh = new TransitionMesh ();
								
								// bug fix from Lars
								var transitionMeshCounter = new int [mesh.VertexCount];
								
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
								newTransitionMesh.Vertices = new Vector3[newTransitionMesh.VertexCount];
								newTransitionMesh.Indexes = new int[newTransitionMesh.IndexCount];

								if (meshInput.ScaleInverted) {
										// Reverse Winding Order (0,2,1)
										for (var j = 0; j < newTransitionMesh.IndexCount; j += 3) {
												newTransitionMesh.Indexes [j] = transitionMeshCounter [indexes [j]];
												newTransitionMesh.Indexes [j + 1] = transitionMeshCounter [indexes [j + 2]];
												newTransitionMesh.Indexes [j + 2] = transitionMeshCounter [indexes [j + 1]];
										}
								} else {
										// Normal Winding Order (0,1,2)
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												newTransitionMesh.Indexes [j] = transitionMeshCounter [indexes [j]];
										}
								}

								// Handle Vertices
								for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
										var index = indexes [j];
										newTransitionMesh.Vertices [transitionMeshCounter [index]] = 
												meshInput.WorldMatrix.MultiplyPoint (vertices [index]);
								}

								// Handle Normals
								if (mesh.Normals != null && mesh.Normals.Length > 0) {
										newTransitionMesh.Normals = new Vector3[newTransitionMesh.VertexCount];

										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												newTransitionMesh.Normals [transitionMeshCounter [index]] = 
														inversedTransposedMatrix.MultiplyVector (normals [index]).normalized;
										}
								}

								// Handle Colors
								if (mesh.Colors != null && mesh.Colors.Length > 0) {
										newTransitionMesh.Colors = new Color[newTransitionMesh.VertexCount];
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												newTransitionMesh.Colors [transitionMeshCounter [index]] = 
														Colors [index];
										}
								}

								// Handle Tangents
								if (mesh.Tangents != null && mesh.Tangents.Length > 0) {
										newTransitionMesh.Tangents = new Vector4[newTransitionMesh.VertexCount];
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												var p = tangents [index];
												var w = p.w;
												p = inversedTransposedMatrix.MultiplyVector (p).normalized;

												newTransitionMesh.Tangents [transitionMeshCounter [index]] = 
														new Vector4 (p.x, p.y, p.z, w);
										}
								}

								// Handle UVs
								if (mesh.UV != null && mesh.UV.Length > 0) {
										newTransitionMesh.UV = new Vector2[newTransitionMesh.VertexCount];
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												newTransitionMesh.UV [transitionMeshCounter [index]] = uv [index];
										}
								}

								// Handle UV2s
								if (mesh.UV2 != null && mesh.UV2.Length > 0) {
										newTransitionMesh.UV2 = new Vector2[newTransitionMesh.VertexCount];
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												newTransitionMesh.UV2 [transitionMeshCounter [index]] = uv2 [index];
										}
								}

								// Handle UV3s
								if (mesh.UV3 != null && mesh.UV3.Length > 0) {
										newTransitionMesh.UV3 = new Vector2[newTransitionMesh.VertexCount];
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												newTransitionMesh.UV3 [transitionMeshCounter [index]] = uv3 [index];
										}
								}

								// Handle UV4s
								if (mesh.UV4 != null && mesh.UV4.Length > 0) {
										newTransitionMesh.UV4 = new Vector2[newTransitionMesh.VertexCount];
										for (var j = 0; j < newTransitionMesh.IndexCount; j++) {
												var index = indexes [j];
												newTransitionMesh.UV4 [transitionMeshCounter [index]] = uv4 [index];
										}
								}

								// Lock the reference array, and add in as it can.
								lock (_transitionMeshes) {
										_transitionMeshes.Add (newTransitionMesh);
								}
						}
				}

				/// <summary>
				/// A thread safe representation of a Mesh.
				/// </summary>
				[System.Serializable]
				public class BufferedMesh
				{
						/// <summary>
						/// Mesh's Colors Arary
						/// </summary>
						public Color[] Colors;
						/// <summary>
						/// Mesh's Indexes List
						/// </summary>
						public List<int[]> Indexes = new List<int[]> ();
						/// <summary>
						/// Mesh's Name
						/// </summary>
						public String Name;
						/// <summary>
						/// Mesh's Normal Array
						/// </summary>
						public Vector3[] Normals;
						/// <summary>
						/// Mesh's Tangents Array
						/// </summary>
						public Vector4[] Tangents;
						/// <summary>
						/// Mesh's Topology Per SubMesh
						/// </summary>
						public MeshTopology[] Topology;
						/// <summary>
						/// Mesh's UV Array
						/// </summary>
						public Vector2[] UV;
						/// <summary>
						/// Mesh's UV2 Array
						/// </summary>
						public Vector2[] UV2;
						/// <summary>
						/// Mesh's UV3 Array
						/// </summary>
						public Vector2[] UV3;
						/// <summary>
						/// Mesh's UV4 Array
						/// </summary>
						public Vector2[] UV4;
						/// <summary>
						/// Mesh's Vertex Array
						/// </summary>
						public Vector3[] Vertices;

						/// <summary>
						/// Gets the number of Vertices present in the BufferedMesh.
						/// </summary>
						/// <value>The Vertex Count.</value>
						public int VertexCount {
								get { return Vertices.Length; }
						}

						/// <summary>
						/// Gets the number of SubMeshes present in the BufferedMesh.
						/// </summary>
						/// <value>The SubMesh Count.</value>
						public int SubMeshCount {
								get { return Indexes.Count; }
						}

						/// <summary>
						/// Gets the Indices in the BufferedMesh.
						/// </summary>
						/// <returns>The indices array at index of the Indexes list of the BufferedMesh.</returns>
						/// <param name="targetIndex">Target Index.</param>
						public int[] GetIndices (int targetIndex)
						{
								return Indexes [targetIndex];
						}
				}

				/// <summary>
				/// Mesh Input Format
				/// </summary>
				[System.Serializable]
				public class MeshInput
				{
						/// <summary>
						/// Was the scale of the Mesh inverted? Therefore needs rewinding!
						/// </summary>
						public bool ScaleInverted;
						/// <summary>
						/// Thread safe representation of a UnityEngine.Mesh.
						/// </summary>
						public BufferedMesh Mesh;
						/// <summary>
						/// The provided WorldMatrix.
						/// </summary>
						public Matrix4x4 WorldMatrix;
						/// <summary>
						/// An array of DataHashCodes ordered for the BufferedMesh.
						/// </summary>
						public int[] Materials;
				}

				/// <summary>
				/// Mesh Object Format
				/// </summary>
				/// <remarks>
				/// Easy to reference object for Unity to use.
				/// </remarks>
				public class MeshObject
				{
						/// <summary>
						/// The created UnityEngine.Mesh
						/// </summary>
						public UnityEngine.Mesh Mesh;
						/// <summary>
						/// The UnityEngine.Material(s) ordered for the renderer.
						/// </summary>
						public UnityEngine.Material[] Materials;
				}

				/// <summary>
				/// Mesh Output Format
				/// </summary>
				[System.Serializable]
				public class MeshOutput
				{
						/// <summary>
						/// A sorted list of TransitionMeshes
						/// </summary>
						public List<TransitionMesh> SortedSources = new List<TransitionMesh> ();
						/// <summary>
						/// A sorted list of Material DataHashCodes.
						/// </summary>
						public List<int> Materials = new List<int> ();
						/// <summary>
						/// The number of vertices present in the Mesh
						/// </summary>
						public int VertexCount;
						/// <summary>
						/// The vertex array.
						/// </summary>
						public List<Vector3> Vertices = new List<Vector3> ();
						/// <summary>
						/// The colors array.
						/// </summary>
						public List<Color> Colors = new List<Color> ();
						/// <summary>
						/// The normals array.
						/// </summary>
						public List<Vector3> Normals = new List<Vector3> ();
						/// <summary>
						/// The tangents array.
						/// </summary>
						public List<Vector4> Tangents = new List<Vector4> ();
						/// <summary>
						/// The UV array.
						/// </summary>
						public List<Vector2> UV = new List<Vector2> ();
						/// <summary>
						/// The UV2 array.
						/// </summary>
						public List<Vector2> UV2 = new List<Vector2> ();
						/// <summary>
						/// The UV3 array.
						/// </summary>
						public List<Vector2> UV3 = new List<Vector2> ();
						/// <summary>
						/// The UV4 array.
						/// </summary>
						public List<Vector2> UV4 = new List<Vector2> ();
						/// <summary>
						/// A list of indexes defining SubMeshes
						/// </summary>
						public List<List<int>> Indexes = new List<List<int>> ();

						/// <summary>
						/// Gets the Indices of a SubMesh with the specified Material DataHashCode.
						/// </summary>
						/// <returns>The SubMesh Indices list.</returns>
						/// <param name="material">The Material's DataHashCode</param>
						public List<int> GetSubMesh (int material)
						{
								for (int i = 0; i < Materials.Count; i++) {

										var m = Materials [i];
										if (m == material) {
												return Indexes [i];
										}
								}
								Materials.Add (material);
								var indexes = new List<int> ();
								Indexes.Add (indexes);
								return indexes;
						}
				}

				/// <summary>
				/// Transition Stage Mesh
				/// </summary>
				public class TransitionMesh
				{
						public int[] Indexes;
						public int IndexCount;
						/// <summary>
						/// The Mesh's Colors array.
						/// </summary>
						public Color[] Colors;
						/// <summary>
						/// The Mesh's Material DataHashCode array.
						/// </summary>
						public int Material;
						/// <summary>
						/// The Mesh's Normals array.
						/// </summary>
						public Vector3[] Normals;
						/// <summary>
						/// The Mesh's Vertex array.
						/// </summary>
						public Vector3[] Vertices;
						/// <summary>
						/// The Mesh's Tangent array.
						/// </summary>
						public Vector4[] Tangents;
						/// <summary>
						/// The Mesh's UV array.
						/// </summary>
						public Vector2[] UV;
						/// <summary>
						/// The Mesh's UV2 array.
						/// </summary>
						public Vector2[] UV2;
						/// <summary>
						/// The Mesh's UV3 array.
						/// </summary>
						public Vector2[] UV3;
						/// <summary>
						/// The Mesh's UV4 array.
						/// </summary>
						public Vector2[] UV4;
						/// <summary>
						/// The number of vertices the Mesh has.
						/// </summary>
						public int VertexCount;

						/// <summary>
						/// Get the BitMask of the Mesh.
						/// </summary>
						/// <returns>The BitMask</returns>
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
								if (UV2 != null)
										mask |= 16;
								if (UV3 != null)
										mask |= 32;
								if (UV4 != null)
										mask |= 64;
								return mask;
						}
				}

				/// <summary>
				/// A class defining how to sort TransitionMeshes.
				/// </summary>
				public class TransitionMeshSorter : IComparer<TransitionMesh>
				{
						/// <summary>
						/// Compare the specified TransitionMeshes.
						/// </summary>
						/// <param name="leftSide">Left Side TransitionMesh.</param>
						/// <param name="rightSide">Right Side TransitionMesh.</param>
						public int Compare (TransitionMesh leftSide, TransitionMesh rightSide)
						{
								// Sort by features first? uv1, uv2, colours, tangents?  Like wise features should be grouped together
								// It is an 'AND' logic, and not 'OR'.  You can't have optional colours!
								int compare = leftSide.GetBitMask ().CompareTo (rightSide.GetBitMask ());
								if (compare != 0)
										return compare;

								// Then Sort by meshes -> meshes with similar meshes grouped together
								compare = leftSide.Material.CompareTo (rightSide.Material);

								// Then sort by size (largest to smallest) -> larger tmeshes more likely to share fewer tmeshes per mesh.
								return compare != 0 ? compare : leftSide.VertexCount.CompareTo (rightSide.VertexCount);
						}
				}
		}
}
