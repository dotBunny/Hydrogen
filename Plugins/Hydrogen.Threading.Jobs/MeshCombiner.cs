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
				UnityEngine.Mesh _combinedMesh;
				int _hash;
				/// <summary>
				/// The _callback.
				/// </summary>
				Action<int, UnityEngine.Mesh> _callback;
				List<Vector3> _vertices;
				List<Vector3> _normals;
				List<Vector4> _tangents;
				List<Color> _colors;
				List<Vector2> _uv;
				List<Vector2> _uv1;
				List<Vector2> _uv2;
				List<int> _indices;
				Vector3[] _verticesArray;
				Vector3[] _normalsArray;
				Vector4[] _tangentsArray;
				Color[] _colorsArray;
				Vector2[] _uvArray;
				Vector2[] _uv1Array;
				Vector2[] _uv2Array;
				int[] _indicesArray;

				public UnityEngine.Mesh CombinedMesh {
						get { return _combinedMesh; }
				}

				public int CombineMeshes (MeshCombiner.Mesh[] meshes)
				{
						return CombineMeshes (meshes, null);
				}

				public int CombineMeshes (MeshCombiner.Mesh[] meshes, Action<int, UnityEngine.Mesh> onFinished)
				{
						// Assign Mesh Data
						_meshes = meshes;

						// Generate Hash Code
						_hash = (Time.time + UnityEngine.Random.Range (0, 100)).GetHashCode ();

						// Start the threaded prcess
						if (onFinished != null) {
								_callback = onFinished;
						}

						if (_combinedMesh != null) {
								_combinedMesh.Clear ();
						}

						IsDone = false;
						IsBusy = false;
			
						Start (true, System.Threading.ThreadPriority.Normal);
						return _hash;
				}

				public void ProcessMesh ()
				{
						_combinedMesh = new UnityEngine.Mesh ();
						_combinedMesh.name = "H_Combined_Mesh";

						_combinedMesh.vertices = _verticesArray;
						_combinedMesh.normals = _normalsArray;
						_combinedMesh.tangents = _tangentsArray;
						_combinedMesh.colors = _colorsArray;
						_combinedMesh.uv = _uvArray;
						_combinedMesh.uv1 = _uv1Array;
						_combinedMesh.uv2 = _uv2Array;

						_combinedMesh.triangles = _indicesArray;
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
						IsBusy = true;

						_vertices = new List<Vector3> (Hydrogen.Mesh.VerticesLimit);
						_normals = new List<Vector3> ();
						_tangents = new List<Vector4> ();
						_colors = new List<Color> ();
						_uv = new List<Vector2> ();
						_uv1 = new List<Vector2> ();
						_uv2 = new List<Vector2> ();
						_indices = new List<int> ();

						int baseIndex = 0;
						MeshCombiner.Mesh mesh;

						// Loop through all of those meshes (yay for threading!)
						for (int x = 0; x < _meshes.Length; x++) {
								// Create quick local reference 
								mesh = _meshes [x];

								for (int y = 0; y < mesh.Vertices.Length; y++) {

										// Add Em All In! (if not empty! THREAD CRASH!)
										if (mesh.Vertices.Length > 0) {
												_vertices.Add (mesh.Vertices [y]);
										}
										if (mesh.Normals.Length > 0) {
												_normals.Add (mesh.Normals [y]);
										}
										if (mesh.Tangents.Length > 0) {
												_tangents.Add (mesh.Tangents [y]);
										}
										if (mesh.Colors.Length > 0) {
												_colors.Add (mesh.Colors [y]);
										}
										if (mesh.UV.Length > 0) {
												_uv.Add (mesh.UV [y]);
										}
										if (mesh.UV1.Length > 0) {
												_uv1.Add (mesh.UV1 [y]);
										}
										if (mesh.UV2.Length > 0) {
												_uv2.Add (mesh.UV2 [y]);
										}
								}



								for (int a = 0; a < mesh.SubMeshCount; a++) {

										var test = mesh.SubMeshes [a].Indices;
										for (int b = 0; b < test.Length; b++) {
								
												_indices.Add (test [b] + baseIndex);

										}
										baseIndex += test.Length;
								}
						}

						// Do as much data manipulation in the thread as possible (sadly).
						// This seems kinda dumb eh?
						_verticesArray = _vertices.ToArray ();
						_normalsArray = _normals.ToArray ();
						_tangentsArray = _tangents.ToArray ();
						_colorsArray = _colors.ToArray ();
						_uvArray = _uv.ToArray ();
						_uv1Array = _uv1.ToArray ();
						_uv2Array = _uv2.ToArray ();
						_indicesArray = _indices.ToArray ();
						IsBusy = false;
						IsDone = true;
				}

				protected sealed override void OnFinished ()
				{
						// Create our mesh
						ProcessMesh ();
			
						// Callback
						if (_callback != null) {
								_callback (_hash, _combinedMesh);

						}

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
