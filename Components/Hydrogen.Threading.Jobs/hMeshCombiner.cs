#region Copyright Notice & License Information
//
// hMeshCombiner.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
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

using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

/// <summary>
/// A drop in implementation of how to interact with the Hydrogen.Threading.Jobs.MeshCombiner. This is meant really as 
/// an example of one way of using it, but you will probably want to create your own method to further optimize the 
/// workflow.
/// </summary>
[AddComponentMenu ("Hydrogen/Singletons/Mesh Combiner")]
public class hMeshCombiner : MonoBehaviour
{
		/// <summary>
		/// An instance of the MeshCombiner.
		/// </summary>
		public Hydrogen.Threading.Jobs.MeshCombiner Combiner = new Hydrogen.Threading.Jobs.MeshCombiner ();
		/// <summary>
		/// This is used in our example to throttle things a bit when accessing Unity objects.
		/// </summary>
		/// <remarks>It seems at 180, its a nice sweet spot for the meshes in our example scene.</remarks>
		public int ThrottleRate = 180;
		/// <summary>
		/// Should this input manager survive scene switches?
		/// </summary>
		public bool Persistent = true;
		/// <summary>
		/// Internal fail safe to maintain instance across threads.
		/// </summary>
		/// <remarks>
		/// Multithreaded Safe Singleton Pattern.
		/// </remarks>
		/// <description>
		/// http://msdn.microsoft.com/en-us/library/ms998558.aspx
		/// </description>
		static readonly System.Object _syncRoot = new System.Object ();
		/// <summary>
		/// Internal reference to the static instance of the Mesh Combiner component.
		/// </summary>
		static volatile hMeshCombiner _staticInstance;
		/// <summary>
		/// Is the MeshCombiner thread running?
		/// </summary>
		bool _threadRunning;
		/// <summary>
		/// Internal reference of what transform a combine operation should parent its meshes too.
		/// </summary>
		Dictionary<int, Transform> _parentLookup = new Dictionary<int, Transform> ();

		/// <summary>
		/// Gets the input manager instance, creating one if none is found.
		/// </summary>
		/// <value>
		/// The Input Manager.
		/// </value>
		public static hMeshCombiner Instance {
				get {
						if (_staticInstance == null) {
								lock (_syncRoot) {
										_staticInstance = FindObjectOfType (typeof(hMeshCombiner)) as hMeshCombiner;

										// If we don't have it, lets make it!
										if (_staticInstance == null) {
												var go = GameObject.Find (Hydrogen.Components.DefaultSingletonName) ??
												         new GameObject (Hydrogen.Components.DefaultSingletonName);

												go.AddComponent<hMeshCombiner> ();
												_staticInstance = go.GetComponent<hMeshCombiner> ();
										}
								}
						}
						return _staticInstance;
				}
		}

		/// <summary>
		/// Does an Input Manager already exist?
		/// </summary>
		public static bool Exists ()
		{
				return _staticInstance != null;
		}

		/// <summary>
		/// Combine all active meshes under the root object.
		/// </summary>
		/// <remarks>
		/// You do not need to wait till completion to call this again with more meshes to combine, however the thread 
		/// will not call back to Unity till all meshes have been processed in the queue.
		/// </remarks>
		/// <param name="rootObject">The "root" GameObject.</param>
		/// <param name="outputParent">Sets the output parent transform.</param>
		/// <param name="disableRootObject">
		/// If set to <c>true</c> disable root object (and its children) after iterating
		/// through its children..
		/// </param>
		public void Combine (GameObject rootObject, Transform outputParent, bool disableRootObject)
		{
				// Do it!
				StartCoroutine (AddMeshes (rootObject, outputParent));

				// Disable our example dat
				if (disableRootObject) {
						rootObject.SetActive (false);
				}
		}

		/// <summary>
		/// This function is called in the example after the MeshCombiner has processed the meshes, it starts a Coroutine 
		/// to create the actual meshes based on the flat data. This is the most optimal way to do this sadly as we cannot
		/// create or touch Unity based meshes outside of the main thread.
		/// </summary>
		/// <param name="hash">Instance Hash.</param>
		/// <param name="meshOutputs">.</param>
		public void ThreadCallback (int hash, Hydrogen.Threading.Jobs.MeshCombiner.MeshOutput[] meshOutputs)
		{
				// This is just a dirty way to see if we can squeeze jsut a bit more performance out of Unity when 
				// making all of the meshes for us (instead of it being done in one call, we use a coroutine with a loop.
				_threadRunning = false;
				StartCoroutine (CreateMeshes (hash, meshOutputs));
		}

		/// <summary>
		/// Process meshFilters in Unity's main thread, as we are required to by Unity. At least we've rigged it as a 
		/// coroutine! Right? OK I know I really wish we could have used mesh data in a thread but properties die as well.
		/// </summary>
		/// <returns>IEnumartor aka Coroutine</returns>
		/// <remarks>
		/// For the sake of the demo we are going to need to roll over the "Target" to find all the 
		/// meshes that we need to look at, but in theory you could do this without having to load the
		/// object by simply having raw mesh data, or any other means of accessing it.
		/// </remarks>
		IEnumerator AddMeshes (GameObject rootObject, Transform outputParent)
		{
				// Yes We Hate This - There Are Better Implementations
				MeshFilter[] meshFilters = rootObject.GetComponentsInChildren<MeshFilter> ();

				// Loop through all of our mesh filters and add them to the combiner to be combined.
				for (int x = 0; x < meshFilters.Length; x++) {

						if (meshFilters [x].gameObject.activeSelf) {
								Combiner.AddMesh (meshFilters [x], 
										meshFilters [x].GetComponent<Renderer> (), 
										meshFilters [x].transform.localToWorldMatrix);
						}

						// We implemented this as a balance point to try and break some of the processing up.
						// If we were to yield every pass it was taking to long to do nothing.
						if (x > 0 && x % ThrottleRate == 0) {
								yield return new WaitForEndOfFrame ();
						}
				}

				// Start the threaded love
				if (Combiner.MeshInputCount > 0) {
						_threadRunning = true;
						_parentLookup.Add (Combiner.Combine (ThreadCallback), outputParent);
				}
				yield return new WaitForEndOfFrame ();
		}

		/// <summary>
		/// Unity's Awake Event
		/// </summary>
		protected void Awake ()
		{
				// Should this gameObject be kept around :) I think so.
				if (Persistent)
						DontDestroyOnLoad (gameObject);
		}

		/// <summary>
		/// Process the MeshDescription data sent back from the Combiner and make it appear!
		/// </summary>
		/// <param name="hash">Instance Hash.</param>
		/// <param name="meshDescriptions">MeshDescriptions.</param>
		/// <param name="materials">Materials.</param>
		IEnumerator CreateMeshes (int hash, Hydrogen.Threading.Jobs.MeshCombiner.MeshOutput[] meshOutputs)
		{
				// Make our meshes in Unity
				for (int x = 0; x < meshOutputs.Length; x++) {
						var meshObject = new GameObject ();

						var newMesh = Combiner.CreateMeshObject (meshOutputs [x], true);

						meshObject.name = newMesh.Mesh.name;
						meshObject.AddComponent<MeshFilter> ().sharedMesh = newMesh.Mesh;
						meshObject.AddComponent<MeshRenderer> ().sharedMaterials = newMesh.Materials;

						if (_parentLookup.ContainsKey (hash)) {
								meshObject.transform.parent = _parentLookup [hash];
						}

						meshObject.transform.position = Vector3.zero;
						meshObject.transform.rotation = Quaternion.identity;


						// Fake Unity Threading
						if (x > 0 && x % ThrottleRate == 0) {
								yield return new WaitForEndOfFrame ();
						}
				}
						
				if (_parentLookup.ContainsKey (hash)) {
						_parentLookup.Remove (hash);
				}

				// Clear previous data (for demonstration purposes)
				// It could be useful to keep some mesh data in already parsed, then you could use the RemoveMesh function
				// to remove ones that you want changed, without having to reparse mesh data.
				Combiner.ClearMeshes ();
		}

		/// <summary>
		/// Unity's LateUpdate Event
		/// </summary>
		void LateUpdate ()
		{
				// If we have a MeshCombiner lets run the Check()
				if (_threadRunning) {
						// Funny thing about this method of doing this; lots of Thread based solutions in Unity have an
						// elaborate manager that does this for you ... just saying.
						Combiner.Check ();
				}
		}
}
