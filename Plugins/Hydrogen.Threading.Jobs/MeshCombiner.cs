using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hydrogen.Threading.Jobs
{
		public class MeshCombiner : ThreadPoolJob
		{
				/// <summary>
				/// Internal array of meshes to combine.
				/// </summary>
				MeshCombiner.Mesh[] _meshes;
				/// <summary>
				/// The combined mesh.
				/// </summary>
				UnityEngine.Mesh[] _combinedMeshes;
				int _hash;
				/// <summary>
				/// The _callback.
				/// </summary>
				Action<int, UnityEngine.Mesh[]> _callback;
				MeshData[] _meshData;
				int _meshIndex;

				public UnityEngine.Mesh[] CombinedMeshes {
						get { return _combinedMeshes; }
				}

				public int CombineMeshes (MeshCombiner.Mesh[] meshes)
				{
						return CombineMeshes (meshes, null);
				}

				public int CombineMeshes (MeshCombiner.Mesh[] meshes, Action<int, UnityEngine.Mesh[]> onFinished)
				{
						// Assign Mesh Data
						_meshes = meshes;

						// Generate Hash Code
						_hash = (Time.time + UnityEngine.Random.Range (0, 100)).GetHashCode ();

						// Start the threaded prcess
						if (onFinished != null) {
								_callback = onFinished;
						}
								
						Start (true, System.Threading.ThreadPriority.Normal);

						return _hash;
				}

				public void ProcessMeshes ()
				{
						Debug.Log ("Processing " + _meshData.Length + " Items");
						_combinedMeshes = new UnityEngine.Mesh[_meshData.Length];

						for (int x = 0; x <= _meshIndex; x++) {

								_combinedMeshes [x] = new UnityEngine.Mesh ();
								_combinedMeshes [x].name = "H_Combined_" + _hash + "_" + x;

								_combinedMeshes [x].vertices = _meshData [x].VerticesArray;
								_combinedMeshes [x].normals = _meshData [x].NormalsArray;
								_combinedMeshes [x].tangents = _meshData [x].TangentsArray;
								_combinedMeshes [x].colors = _meshData [x].ColorsArray;

								_combinedMeshes [x].uv = _meshData [x].UVArray;
								_combinedMeshes [x].uv1 = _meshData [x].UV1Array;
								_combinedMeshes [x].uv2 = _meshData [x].UV2Array;

								_combinedMeshes [x].SetIndices (_meshData [x].IndicesArray, MeshTopology.Triangles, 0);

								Debug.Log ("[" + _combinedMeshes [x].name + "] " + "Vertices: " + _meshData [x].VerticesArray.Length + " || Normals: " + _meshData [x].NormalsArray.Length +
								" || Tangents: " + _meshData [x].TangentsArray.Length + " || Colors: " + _meshData [x].ColorsArray.Length + " || UV: " + _meshData [x].UVArray.Length +
								" || UV1: " + _meshData [x].UV1Array.Length + " || UV2: " + _meshData [x].UV2Array.Length + " || Indices: " + _meshData [x].IndicesArray.Length);

								string verts = "";
								foreach (var v3 in _combinedMeshes[x].vertices) {
										verts += "[" + v3.ToString () + "] ";
								}
								Debug.Log ("VERTICES: " + verts);
						}
				}

				public static MeshCombiner.Mesh MeshFilterToMesh (MeshFilter meshFilter)
				{
						var mesh = new MeshCombiner.Mesh ();

						// Thank you Unity for making us have to have all our data like this
						mesh.Vertices = meshFilter.sharedMesh.vertices;
						mesh.VertexCount = meshFilter.sharedMesh.vertexCount;
						mesh.Normals = meshFilter.sharedMesh.normals;
						mesh.Tangents = meshFilter.sharedMesh.tangents;
						mesh.UV = meshFilter.sharedMesh.uv;
						mesh.UV1 = meshFilter.sharedMesh.uv1;
						mesh.UV2 = meshFilter.sharedMesh.uv2;
						mesh.Colors = meshFilter.sharedMesh.colors;
						mesh.SubMeshCount = meshFilter.sharedMesh.subMeshCount;
						mesh.WorldMatrix = meshFilter.transform.localToWorldMatrix;

						// Match out the SubMeshes
						mesh.SubMeshCount = meshFilter.sharedMesh.subMeshCount;
						mesh.SubMeshes = new MeshCombiner.SubMesh[mesh.SubMeshCount ];

						for (int z = 0; z < mesh.SubMeshCount; z += 1) {
								var subMesh = new MeshCombiner.SubMesh ();
								subMesh.Indices = meshFilter.sharedMesh.GetIndices (z);
								mesh.SubMeshes [z] = subMesh;
						}

						return mesh;
				}

				protected sealed override void ThreadedFunction ()
				{
						_meshIndex = 0;
						_meshData = new MeshData[1];

						// Initialize our first mesh
						_meshData [0].Vertices = new List<Vector3> (Hydrogen.Mesh.VerticesLimit);
						_meshData [0].Normals = new List<Vector3> ();
						_meshData [0].Tangents = new List<Vector4> ();
						_meshData [0].Colors = new List<Color> ();
						_meshData [0].UV = new List<Vector2> ();
						_meshData [0].UV1 = new List<Vector2> ();
						_meshData [0].UV2 = new List<Vector2> ();
						_meshData [0].Indices = new List<int> ();

						int baseIndex = 0;
						MeshCombiner.Mesh mesh;

						// Loop through all of those meshes (yay for threading!)
						for (int x = 0; x < _meshes.Length; x++) {
								// If there are no vertices what are we doing here?
								if (_meshes [x].Vertices.Length == 0) {
										continue;
								}




// Create quick local reference 
								mesh = _meshes [x];



/*
 * _vertices.AddRange(instance.mesh.vertices.Select(instance.transform.MultiplyPoint));
 * _normals.AddRange(instance.mesh.normals.Select(n => instance.transform.inverse.transpose.MultiplyVector(n).normalized));
 * _tangents.AddRange(instance.mesh.tangents.Select(t =>
                                                             {
                                                                 var p = new Vector3(t.x, t.y, t.z);
                                                                 p =
                                                                     instance.transform.inverse.transpose.
                                                                         MultiplyVector(p).normalized;
                                                                 return new Vector4(p.x, p.y, p.z, t.w);
                                                             }));
*/


								for (int y = 0; y < mesh.Vertices.Length; y++) {

										// Add check here for vert limit ?
										// TODO: Add increase to _meshIndex if it detects over limit (increase _meshData array too); 
										// TODO: will prolly need indices to be reset or linked to the actual meshdata

										// Add Em All In! (if not empty! THREAD CRASH!)




										if (mesh.Vertices.Length > 0) {

												// TODO : Need to map this to handle is position in the real world?
												_meshData [_meshIndex].Vertices.Add (mesh.Vertices [y]);
										}
										if (mesh.Normals.Length > 0) {
												_meshData [_meshIndex].Normals.Add (mesh.Normals [y]);
										}
										if (mesh.Tangents.Length > 0) {
												_meshData [_meshIndex].Tangents.Add (mesh.Tangents [y]);
										}
										if (mesh.Colors.Length > 0) {
												_meshData [_meshIndex].Colors.Add (mesh.Colors [y]);
										}
										if (mesh.UV.Length > 0) {
												_meshData [_meshIndex].UV.Add (mesh.UV [y]);
										}
										if (mesh.UV1.Length > 0) {
												_meshData [_meshIndex].UV1.Add (mesh.UV1 [y]);
										}
										if (mesh.UV2.Length > 0) {
												_meshData [_meshIndex].UV2.Add (mesh.UV2 [y]);
										}
								}


								for (int a = 0; a < mesh.SubMeshCount; a++) {

										var test = mesh.SubMeshes [a].Indices;
										for (int b = 0; b < test.Length; b++) {
								
												_meshData [_meshIndex].Indices.Add (test [b] + baseIndex);

										}
										baseIndex += test.Length;
								}
						}

						// Do as much data manipulation in the thread as possible (sadly).
						// This seems kinda dumb eh? But it is just another thing that saves on time later.
						// It isn't great on memory tho ;(
						for (int z = 0; z < _meshData.Length; z++) {
								_meshData [z].VerticesArray = _meshData [z].Vertices.ToArray ();
								_meshData [z].NormalsArray = _meshData [z].Normals.ToArray ();
								_meshData [z].TangentsArray = _meshData [z].Tangents.ToArray ();
								_meshData [z].ColorsArray = _meshData [z].Colors.ToArray ();
								_meshData [z].UVArray = _meshData [z].UV.ToArray ();
								_meshData [z].UV1Array = _meshData [z].UV1.ToArray ();
								_meshData [z].UV2Array = _meshData [z].UV2.ToArray ();
								_meshData [z].IndicesArray = _meshData [z].Indices.ToArray ();
						}
				}

				protected sealed override void OnFinished ()
				{
						// Create our mesh
						ProcessMeshes ();
			
						// Callback
						if (_callback != null) {
								_callback (_hash, _combinedMeshes);
						}

				}

				public struct MeshData
				{
						public List<Vector3> Vertices;
						public List<Vector3> Normals;
						public List<Vector4> Tangents;
						public List<Color> Colors;
						public List<Vector2> UV;
						public List<Vector2> UV1;
						public List<Vector2> UV2;
						public List<int> Indices;
						public Vector3[] VerticesArray;
						public Vector3[] NormalsArray;
						public Vector4[] TangentsArray;
						public Color[] ColorsArray;
						public Vector2[] UVArray;
						public Vector2[] UV1Array;
						public Vector2[] UV2Array;
						public int[] IndicesArray;
				}

				public struct Mesh
				{
						public Vector3[] Vertices;
						public int VertexCount;
						public Matrix4x4 WorldMatrix;
						public Vector3[] Normals;
						public Vector4[] Tangents;
						public Vector2[] UV;
						public Vector2[] UV1;
						public Vector2[] UV2;
						public Color[] Colors;
						public MeshCombiner.SubMesh[] SubMeshes;
						public int SubMeshCount;
				}

				public struct SubMesh
				{
						public int[] Indices;
				}
		}
}
