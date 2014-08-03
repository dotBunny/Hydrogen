#region Copyright Notice & License Information
//
// ObjectPoolCollection.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
//
// Copyright (c) 2013 dotBunny Inc. (http://www.dotbunny.com)
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

namespace Hydrogen.Core
{
		/// <summary>
		/// The actual pool used by the pooling system, multiple instances of these will be created per unique 
		/// Prefab or GameObject.
		/// </summary>
		[System.Serializable]
		public class ObjectPoolCollection
		{
				/// <summary>
				/// Inspector Name for Element
				/// </summary>
				public string Name;
				/// <summary>
				/// Should extra objects be culled when not in use?
				/// </summary>
				public bool CullExtras;
				/// <summary>
				/// How often should we look at culling extra objects.
				/// </summary>
				public float CullInterval;
				/// <summary>
				/// Should despawned object be returned to it's pool's origin position?
				/// </summary>
				public bool DespawnPoolLocation = true;
				/// <summary>
				/// Should particle systems be appropriately handled when despawning?
				/// </summary>
				public bool ManageParticles = true;
				/// <summary>
				/// Reference to the Prefab or GameObject used by this Object Pool.
				/// </summary>
				public GameObject Prefab;
				/// <summary>
				/// The number of objects to preload in an Object Pool.
				/// </summary>
				public int PreloadAmount;
				/// <summary>
				/// Should Unity's SendMessage be used. (OnSpawned, WaitToDespawn, OnDespawned)
				/// </summary>
				public bool SendMessage;
				/// <summary>
				/// Should additional objects be spawned as needed?
				/// </summary>
				public bool SpawnMore;
				/// <summary>
				/// Where should pooled items reside
				/// </summary>
				public Transform FixedParent;
				/// <summary>
				/// Should objects be tracked when they are spawned?
				/// </summary>
				/// <remarks>
				/// Useful for when you need to keep track of what objects are in use.
				/// </remarks>
				public bool TrackObjects;
				/// <summary>
				/// Timer used to determine when its appropriate to update objects that need to be culled.
				/// </summary>
				float _cullTimer;
				/// <summary>
				/// Does the Prefab contain a legacy Particle Animator?
				/// </summary>
				bool _hasLegacyParticleAnimator;
				/// <summary>
				/// Does the Prefab contain a legacy Particle Emitter?
				/// </summary>
				bool _hasLegacyParticleEmitter;
				/// <summary>
				/// Does the Prefab have a ObjectPoolItem based component attached to it?
				/// </summary>
				bool _hasObjectPoolItem;
				/// <summary>
				/// Does the Prefab contain a Particle System?
				/// </summary>
				bool _hasParticleSystem;
				/// <summary>
				/// Does the Prefab have a Rigidbody?
				/// </summary>
				bool _hasRigidbody;
				/// <summary>
				/// Has the Object Pool been initialized?
				/// </summary>
				bool _initialized;
				/// <summary>
				/// Local reference to the parent transform.
				/// </summary>
				Transform _parentTransform;
				/// <summary>
				/// This pool's ID
				/// </summary>
				int _poolID;
				/// <summary>
				/// An internal array of available objects.
				/// </summary>
				GameObject[] _pooledObjects;
				/// <summary>
				/// An array of all spawned objects (Requires TrackObject during construction)
				/// </summary>
				GameObject[] _spawnedObjects;

				public bool HasObjectPoolItem {
						get {
								return _hasObjectPoolItem;
						}
				}

				public bool HasRigidbody {
						get {
								return _hasRigidbody;
						}
				}

				public bool HasParticleSystem {
						get {
								return _hasParticleSystem;
						}
				}

				public bool HasLegacyParticleAnimator {
						get {
								return _hasLegacyParticleAnimator;
						}
				}

				public bool HasLegacyParticleEmitter {
						get {
								return _hasLegacyParticleEmitter;
						}
				}

				public GameObject[] SpawnedObjects { get { return _spawnedObjects; } }

				public bool Initialized { 
						get { 
								return _initialized;
						}
				}

				public ObjectPoolCollection (int preload, bool spawnMore, bool slowMessage, bool handleParticles, bool trackSpawned, bool cullExtras, float cullInterval)
				{
						ManageParticles = handleParticles;
						PreloadAmount = preload;
						SpawnMore = spawnMore;
						SendMessage = slowMessage;
						TrackObjects = trackSpawned;
						CullExtras = cullExtras;
						CullInterval = cullInterval;
						_cullTimer = cullInterval;
				}

				public void AddToPool ()
				{
						var newObject = Object.Instantiate (Prefab) as GameObject;
						newObject.name = Prefab.name;

						if (FixedParent != null) {
								newObject.transform.parent = FixedParent;
						}

						// Check if there is our helper item on the gameObject
						if (newObject.GetComponent<ObjectPoolItemBase> ()) {
								newObject.GetComponent<ObjectPoolItemBase> ().ParentPool = this;
								newObject.GetComponent<ObjectPoolItemBase> ().PoolID = _poolID;
						}

						Array.Add<GameObject> (ref _pooledObjects, newObject, false);

						Despawn (newObject, true);
				}

				public void Initialize (GameObject prefab, Transform parent, int poolID)
				{
						Prefab = prefab;

						_parentTransform = parent;
						_poolID = poolID;
						_pooledObjects = new GameObject[0];

						_hasObjectPoolItem |= Prefab.GetComponent<ObjectPoolItemBase> ();

						_hasRigidbody |= prefab.GetComponent<Rigidbody> ();

						if (ManageParticles) {

								_hasLegacyParticleEmitter |= prefab.GetComponent<ParticleEmitter> ();
								_hasLegacyParticleAnimator |= prefab.GetComponent<ParticleAnimator> ();
								_hasParticleSystem |= prefab.GetComponent<ParticleSystem> ();
						}

						ManageParticles &= _hasLegacyParticleAnimator || _hasLegacyParticleEmitter || _hasParticleSystem;

						for (int x = 0; x < PreloadAmount; x++) {
								AddToPool ();
						}
						_initialized = true;
				}

				public void CullUpdate ()
				{
						if (!TrackObjects || !_hasObjectPoolItem)
								return;

						_cullTimer -= Time.deltaTime;

						if (_cullTimer <= 0) {
								for (int x = 0; x < SpawnedObjects.Length; x++) {
										if (SpawnedObjects [x].GetComponent<ObjectPoolItemBase> ().IsInactive ()) {
												Despawn (SpawnedObjects [x]);
										}
								}
								// Establish Timer Again
								_cullTimer = CullInterval;
						}
				}

				public void Despawn (GameObject gameObject)
				{
						if (gameObject == null)
								return;

						Despawn (gameObject, false);
				}

				public void Despawn (GameObject gameObject, bool onSpawn)
				{
						Rigidbody rigidbody = gameObject.GetComponent<Rigidbody> ();

						if (!ManageParticles)
								gameObject.transform.parent = _parentTransform;

						if (FixedParent != null) {
								gameObject.transform.parent = FixedParent;
						}
						// Has our handler
						if (_hasObjectPoolItem) {
								if (ManageParticles) {
										gameObject.GetComponent<ObjectPoolItemBase> ().DespawnSafely ();	
										return;
								}
								gameObject.GetComponent<ObjectPoolItemBase> ().OnDespawned ();	

						}
						// Use slow method
						else if (SendMessage) {
								if (ManageParticles) {
										gameObject.SendMessage ("WaitToDespawn", SendMessageOptions.DontRequireReceiver);
										return;
								}
								gameObject.SendMessage ("OnDespawned", SendMessageOptions.DontRequireReceiver);
						}
						// Fail safe
						else {
								// This stops things from keeping their velocity from previous
								if (_hasRigidbody && !rigidbody.isKinematic) {
										rigidbody.velocity = Vector3.zero;
										rigidbody.angularVelocity = Vector3.zero;
								}

								gameObject.SetActive (false);

						}

						if (DespawnPoolLocation && _parentTransform != null) {
								gameObject.transform.position = _parentTransform.position;
						}

						if (!onSpawn) {
								if (TrackObjects)
										Array.Remove<GameObject> (ref _spawnedObjects, gameObject);
								Array.Add<GameObject> (ref _pooledObjects, gameObject, false);
						}
				}

				public void DespawnImmediate (GameObject gameObject)
				{
						gameObject.transform.parent = _parentTransform;
						if (TrackObjects)
								Array.Remove<GameObject> (ref _spawnedObjects, gameObject);
						Array.Add<GameObject> (ref _pooledObjects, gameObject, false);	
				}

				public bool RemoveFromPool (GameObject gameObject, bool destroyObject)
				{
						if (destroyObject) {

								Object.DestroyImmediate (gameObject);
								return Array.Remove<GameObject> (ref _pooledObjects, gameObject);
						}
						return Array.Remove<GameObject> (ref _pooledObjects, gameObject);

				}

				/// <summary>
				/// Spawn an object from the pool at origin.
				/// </summary>
				public GameObject Spawn ()
				{
						return Spawn (Vector3.zero, Quaternion.identity);
				}

				/// <summary>
				/// Spawn an object from the pool at the specified world location and rotation.
				/// </summary>
				/// <param name="location">Spawn Location</param>
				/// <param name="rotation">Spawn Rotation</param>
				public GameObject Spawn (Vector3 location, Quaternion rotation)
				{
						if (_pooledObjects != null && _pooledObjects.Length > 0) {
								GameObject spawnedObject = _pooledObjects [0];

								if (spawnedObject == null) {
										UnityEngine.Debug.LogError (
												"[h] A GameObject (" + Prefab.name + ") has been destroyed," +
												"but is still referenced by the Object Pool. If you must destroy a GameObject manually," +
												"please make sure to remove it from the pool as well:" +
												"hObjectPool.Instance.Remove(GameObject);");

										return null;
								}

								Array.RemoveAt<GameObject> (ref _pooledObjects, 0);

								// Move and rotate before we call on spawned
								spawnedObject.transform.position = location;
								spawnedObject.transform.rotation = rotation;

								if (ManageParticles) {
										if (_hasLegacyParticleAnimator) {
												spawnedObject.GetComponent<ParticleAnimator> ().autodestruct = false;
										}
										if (_hasLegacyParticleEmitter) {
												spawnedObject.GetComponent<ParticleEmitter> ().emit = true;
										}
								}

								if (_hasObjectPoolItem) {
										spawnedObject.GetComponent<ObjectPoolItemBase> ().OnSpawned ();
								} else if (SendMessage) {
										spawnedObject.SendMessage ("OnSpawned", SendMessageOptions.DontRequireReceiver);
								} else {
										spawnedObject.SetActive (true);
								}

								if (TrackObjects) {
										// No already spawned objects
										if (_spawnedObjects == null)
												_spawnedObjects = new GameObject[0];

										Array.Add<GameObject> (ref _spawnedObjects, spawnedObject, false);
								}

								return spawnedObject;
						}
						if (SpawnMore) {
								AddToPool ();
								return Spawn (location, rotation);
						}
						return null;
				}
		}
}