using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

/// <summary>
/// A drop in implementation of how to interact with the Hydrogen.Threading.Jobs.MeshCombiner. This is meant really as 
/// an example of one way of using it, but you will probably want to create your own method to further optimize the 
/// workflow.
/// </summary>
public class hMeshCombiner : MonoBehaviour
{
		/// <summary>
		/// This is used in our example to throttle things a bit when accessing Unity objects.
		/// </summary>
		/// <remakrs>It seems at 180, its a nice sweet spot for these meshes in this scene.</remarks>
		public int ThrottleRate = 180;
		Hydrogen.Threading.Jobs.MeshCombiner Combiner = new Hydrogen.Threading.Jobs.MeshCombiner ();
		Transform _parentTransform;
		GameObject _rootObject;
		bool _threadRunning;
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
		/// Unity's Awake Event
		/// </summary>
		protected void Awake ()
		{
				// Make sure to do the object pools normal initialization

				// Should this gameObject be kept around :) I think so.
				if (Persistent)
						DontDestroyOnLoad (gameObject);
		}
		// This is an implementation that can be just thrown meshes and it does the rest.
		// do i make meshcombines per instance ... probably... so you can keep throwing stuff into it.
		// will mention the abilityto store stuff and use manually later
		// TODO Make these incubated and run in their own coroutines (need to store the parent transform for later? dictionary hash/index0
		public void Combine (GameObject rootObject, Transform outputParent, bool disableRootObject)
		{
				_rootObject = rootObject;
				_parentTransform = outputParent;

				// Do it!
				StartCoroutine (AddMeshes ());

				// Disable our example dat
				if (disableRootObject) {
						_rootObject.SetActive (false);
				}
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
		IEnumerator AddMeshes ()
		{
				// Yes We Hate This - There Are Better Implementations
				MeshFilter[] meshFilters = _rootObject.GetComponentsInChildren<MeshFilter> ();

				// Loop through all of our mesh filters and add them to the combiner to be combined.
				for (int x = 0; x < meshFilters.Length; x++) {

						if (meshFilters [x].gameObject.activeSelf) {
								Combiner.AddMesh (meshFilters [x], 
										meshFilters [x].renderer, 
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
						Combiner.Combine (ThreadCallback);
				}
				yield return new WaitForEndOfFrame ();
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
						meshObject.transform.parent = _parentTransform.transform;
						meshObject.transform.position = Vector3.zero;
						meshObject.transform.rotation = Quaternion.identity;


						// Fake Unity Threading
						if (x > 0 && x % ThrottleRate == 0) {
								yield return new WaitForEndOfFrame ();
						}
				}
				// Clear previous data (for demonstration purposes)
				// It could be useful to keep some mesh data in already parsed, then you could use the RemoveMesh function
				// to remove ones that you want changed, without having to reparse mesh data.
				Combiner.ClearMeshes ();
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
