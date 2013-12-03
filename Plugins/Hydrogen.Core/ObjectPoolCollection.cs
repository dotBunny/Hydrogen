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
		// TODO DOCUMENT!
		[System.Serializable]
		public class ObjectPoolCollection
		{
				public GameObject Prefab;
				public int PreloadAmount;
				public bool SpawnMoreIfNeeded;
				public bool SendMessage;
				public bool ManageParticles = true;
				public bool TrackSpawnedObjects;
				public bool CullExtraObjects;
				bool _hasObjectPoolItem;
				bool _hasParticleSystem;
				bool _hasLegacyParticleEmitter;
				bool _hasLegacyParticleAnimator;
				bool _hasRigidbody;

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

				/// <summary>
				/// The pooled objects currently available.
				/// </summary>
				GameObject[] _pooledObjects;
				Transform _parentTransform;
				int _poolID;
				GameObject[] _spawnedObjects;

				public GameObject[] SpawnedObjects { get { return _spawnedObjects; } }

				bool _initialized;

				public bool Initialized { get { return _initialized; } }

				public ObjectPoolCollection (int preload, bool spawnMore, bool slowMessage, bool handleParticles, bool trackSpawned, bool cullExtras)
				{
						ManageParticles = handleParticles;
						PreloadAmount = preload;
						SpawnMoreIfNeeded = spawnMore;
						SendMessage = slowMessage;
						TrackSpawnedObjects = trackSpawned;
						CullExtraObjects = cullExtras;
				}

				public void Initialize (GameObject gameObject, Transform parent, int poolID)
				{
						Prefab = gameObject;

						_parentTransform = parent;
						_poolID = poolID;
						_pooledObjects = new GameObject[0];

						_hasObjectPoolItem |= Prefab.GetComponent<ObjectPoolItemBase> ();

						_hasRigidbody |= gameObject.GetComponent<Rigidbody> ();

						if (ManageParticles) {
								_hasLegacyParticleEmitter |= gameObject.GetComponent<ParticleEmitter> ();
								_hasLegacyParticleAnimator |= gameObject.GetComponent<ParticleAnimator> ();
								_hasParticleSystem |= gameObject.GetComponent<ParticleSystem> ();
						}

						ManageParticles &= _hasLegacyParticleAnimator || _hasLegacyParticleEmitter || _hasParticleSystem;

						for (int x = 0; x < PreloadAmount; x++) {
								AddToPool ();
						}
						_initialized = true;
				}

				public void AddToPool ()
				{
						var newObject = Object.Instantiate (Prefab) as GameObject;

						newObject.name = Prefab.name;


						// Check if there is our helper item on the gameObject
						if (newObject.GetComponent<ObjectPoolItemBase> ()) {
								newObject.GetComponent<ObjectPoolItemBase> ().ParentPool = this;
								newObject.GetComponent<ObjectPoolItemBase> ().PoolID = _poolID;
						}

						Array.Add<GameObject> (ref _pooledObjects, newObject, false);

						Despawn (newObject, true);
				}

				public bool RemoveFromPool (GameObject gameObject, bool destroyObject)
				{
						if (destroyObject) {
								Object.DestroyImmediate (gameObject);
								return Array.Remove<GameObject> (ref _pooledObjects, gameObject);
						}
						return Array.Remove<GameObject> (ref _pooledObjects, gameObject);

				}

				public GameObject Spawn (Vector3 location, Quaternion rotation)
				{
						if (_pooledObjects != null && _pooledObjects.Length > 0) {
								GameObject spawnedObject = _pooledObjects [0];
				
								if (spawnedObject == null) {
										Debug.LogError (
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
										spawnedObject.GetComponent<ParticleAnimator> ().autodestruct &= !_hasLegacyParticleAnimator;
										spawnedObject.GetComponent<ParticleEmitter> ().emit |= _hasLegacyParticleEmitter;
								}

								if (_hasObjectPoolItem) {
										//spawnedObject.GetComponent<ObjectPoolItemBase>().poolID = _poolID; 

										spawnedObject.GetComponent<ObjectPoolItemBase> ().OnSpawned ();
								} else if (SendMessage) {
										spawnedObject.SendMessage ("OnSpawned", SendMessageOptions.DontRequireReceiver);
								} else {
										spawnedObject.SetActive (true);
								}

								if (TrackSpawnedObjects) {
										// No already spawned objects
										if (_spawnedObjects == null)
												_spawnedObjects = new GameObject[0];

										Array.Add<GameObject> (ref _spawnedObjects, spawnedObject, false);
								}

								return spawnedObject;
						}
						if (SpawnMoreIfNeeded) {
								AddToPool ();
								return Spawn (location, rotation);
						}
						return null;
				}

				public void Despawn (GameObject gameObject)
				{
						Despawn (gameObject, false);
				}

				public void Despawn (GameObject gameObject, bool onSpawn)
				{
						if (!ManageParticles)
								gameObject.transform.parent = _parentTransform;

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
								if (_hasRigidbody) {
										gameObject.rigidbody.velocity = Vector3.zero;
										gameObject.rigidbody.angularVelocity = Vector3.zero;
								}

								gameObject.SetActive (false);

						}

						if (!onSpawn) {
								if (TrackSpawnedObjects)
										Array.Remove<GameObject> (ref _spawnedObjects, gameObject);
								Array.Add<GameObject> (ref _pooledObjects, gameObject, false);
						}
				}

				public void DespawnImmediate (GameObject gameObject)
				{
						gameObject.transform.parent = _parentTransform;
						if (TrackSpawnedObjects)
								Array.Remove<GameObject> (ref _spawnedObjects, gameObject);
						Array.Add<GameObject> (ref _pooledObjects, gameObject, false);	
				}
		}
}