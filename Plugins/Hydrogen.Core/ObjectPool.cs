#region Copyright Notice & License Information
//
// ObjectPool.cs
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

using System.Collections.Generic;
using UnityEngine;

namespace Hydrogen.Core
{
		/// <summary>
		/// An internal pooling system used within the Hydrogen Framework. 
		/// 
		/// Each GameObject / Prefab has its own pool, this is done for many reasons, 
		/// but ultimately seems to be the best performance scenario.
		/// </summary>
		[AddComponentMenu ("")]
		public class ObjectPool : MonoBehaviour
		{
				/// <summary>
				/// Should extra objects be culled when not in use?
				/// </summary>
				/// <remarks>
				/// The default value used when adding objects to the Object Pool.
				/// </remarks>
				public bool CullExtras;
				/// <summary>
				/// How often should we look at culling extra objects.
				/// </summary>
				/// <remarks>
				/// The default value used when adding objects to the Object Pool.
				/// </remarks>
				public float CullInterval = 3f;
				/// <summary>
				/// Should particle systems be appropriately handled when despawning?
				/// </summary>
				/// <remarks>
				/// The default value used when adding objects to the Object Pool.
				/// </remarks>
				public bool HandleParticles = true;
				/// <summary>
				/// Our pooled object collections.
				/// </summary>
				/// <remarks>
				/// One for each GameObject / Prefab added to the Object Pool.
				/// </remarks>
				public ObjectPoolCollection[] ObjectPools;
				/// <summary>
				/// The number of objects to preload in an Object Pool.
				/// </summary>
				/// <remarks>
				/// The default value used when adding objects to the Object Pool.
				/// </remarks>
				public int PreloadAmount = 5;
				/// <summary>
				/// Should Unity's SendMessage be used. (OnSpawned, WaitToDespawn, OnDespawned)
				/// </summary>
				/// <remarks>
				/// The default value used when adding objects to the Object Pool. 
				/// </remarks>
				public bool SlowMessage;
				/// <summary>
				/// Should additional objects be spawned as needed?
				/// </summary>
				/// <remarks>
				/// The default value used when adding objects to the Object Pool. 
				/// </remarks>
				public bool SpawnMore = true;
				/// <summary>
				/// Should objects be tracked when they are spawned?
				/// </summary>
				/// <remarks>
				/// Useful for when you need to keep track of what objects are in use.
				/// The default value used when adding objects to the Object Pool.
				/// </remarks>
				public bool TrackObjects;
				/// <summary>
				/// A lookup table of indexes of the object pools so we don't always have to know, but really should. 
				/// </summary>
				Dictionary<string, int> _poolStringLookupTable;

				/// <summary>
				/// Add a Prefab or GameObject to the Object Pool.
				/// </summary>
				/// <returns>
				/// The index (Pool ID) of the newly created pool, or if a pool already exists, return its index.
				/// </returns>
				/// <remarks>
				/// If you pass an existing GameObject from the scene, it will need to exist in the future to be able 
				/// to spawn any additional objects outside of what was preloaded upon its addition to the Object Pool. 
				/// It can be disabled, or inactive, it simply needs to exist if you want to add more later.
				/// </remarks>
				/// <param name="prefab">The Prefab or GameObject.</param>
				public int Add (GameObject prefab)
				{
						return Add (prefab, PreloadAmount, SpawnMore, SlowMessage, HandleParticles, TrackObjects, 
								CullExtras, CullInterval);
				}

				/// <summary>
				/// Add a Prefab or GameObject to the Object Pool.
				/// </summary>
				/// <returns>
				/// The index (Pool ID) of the newly created pool, or if a pool already exists, return its index.
				/// </returns>
				/// <remarks>
				/// If you pass an existing GameObject from the scene, it will need to exist in the future to be able 
				/// to spawn any additional objects outside of what was preloaded upon its addition to the Object Pool. 
				/// It can be disabled, or inactive, it simply needs to exist if you want to add more later.
				/// </remarks>
				/// <param name="prefab">The Prefab or GameObject.</param>
				/// <param name="preloadAmount">The number of objects that should be preloaded.</param>
				public int Add (GameObject prefab, int preloadAmount)
				{
						return Add (prefab, preloadAmount, SpawnMore, SlowMessage, HandleParticles, TrackObjects, 
								CullExtras, CullInterval);
				}

				/// <summary>
				/// Add a Prefab or GameObject to the Object Pool.
				/// </summary>
				/// <returns>
				/// The index (Pool ID) of the newly created pool, or if a pool already exists, return its index.
				/// </returns>
				/// <remarks>
				/// If you pass an existing GameObject from the scene, it will need to exist in the future to be able 
				/// to spawn any additional objects outside of what was preloaded upon its addition to the Object Pool. 
				/// It can be disabled, or inactive, it simply needs to exist if you want to add more later.
				/// </remarks>
				/// <param name="prefab">The Prefab or GameObject.</param>
				/// <param name="preloadAmount">The number of objects that should be preloaded.</param>
				/// <param name="spawnMore">Should more objects be added to the pool as needed?</param>
				/// <param name="slowMessage">Should Unity's SendMessage be used?</param>
				/// <param name="handleParticles">Should accomodations for Particles be made?</param>
				/// <param name="trackSpawned">Should spawned objects be tracked?</param>
				/// <param name="cullExtras">Should extra objects be removed from the Object Pool when not in use?</param>
				/// <param name="cullInterval">How often should we check for extras to cull.</param>
				public int Add (GameObject prefab, int preloadAmount, bool spawnMore, bool slowMessage, 
				                bool handleParticles, bool trackSpawned, bool cullExtras, float cullInterval)
				{
						if (prefab == null) {
								throw new MissingReferenceException ("You are passing a null gameObject reference to hObjectPool.Instance.Add()");
						}

						var tempLookup = GetPoolID (prefab.name);
						if (tempLookup == -1) {
								var newPool = new ObjectPoolCollection (preloadAmount, spawnMore, slowMessage, 
										              handleParticles, trackSpawned, cullExtras, cullInterval);
								newPool.Initialize (prefab, transform, ObjectPools.Length);
								Array.Add<ObjectPoolCollection> (ref ObjectPools, newPool, false);
								if (_poolStringLookupTable == null)
										_poolStringLookupTable = new Dictionary<string, int> ();
								_poolStringLookupTable.Add (newPool.Prefab.name, ObjectPools.Length - 1);
								return ObjectPools.Length - 1;
						}
						return tempLookup;
				}

				/// <summary>
				/// Add an array of Prefabs or GameObjects to the Object Pool.
				/// </summary>
				/// <returns>
				/// An array of indexs (Pool IDs) of the newly created pools, or if pools already exists, use their indexes.
				/// </returns>
				/// <remarks>
				/// If you pass an existing GameObject from the scene, it will need to exist in the future to be able 
				/// to spawn any additional objects outside of what was preloaded upon its addition to the Object Pool. 
				/// It can be disabled, or inactive, it simply needs to exist if you want to add more later.
				/// </remarks>
				/// <param name="prefabs">An array of Prefabs or GameObjects.</param>
				public int[] Add (GameObject[] prefabs)
				{
						return Add (prefabs, PreloadAmount, SpawnMore, SlowMessage, HandleParticles, TrackObjects, 
								CullExtras, CullInterval);
				}

				/// <summary>
				/// Add an array of Prefabs or GameObjects to the Object Pool.
				/// </summary>
				/// <returns>
				/// An array of indexs (Pool IDs) of the newly created pools, or if pools already exists, use their indexes.
				/// </returns>
				/// <remarks>
				/// If you pass an existing GameObject from the scene, it will need to exist in the future to be able 
				/// to spawn any additional objects outside of what was preloaded upon its addition to the Object Pool. 
				/// It can be disabled, or inactive, it simply needs to exist if you want to add more later.
				/// </remarks>
				/// <param name="prefabs">An array of Prefabs or GameObjects.</param>
				/// <param name="preloadAmount">The number of objects that should be preloaded.</param>
				public int[] Add (GameObject[] prefabs, int preloadAmount)
				{
						return Add (prefabs, preloadAmount, SpawnMore, SlowMessage, HandleParticles, TrackObjects, 
								CullExtras, CullInterval);
				}

				/// <summary>
				/// Add an array of Prefabs or GameObjects to the Object Pool.
				/// </summary>
				/// <returns>
				/// An array of indexs (Pool IDs) of the newly created pools, or if pools already exists, use their indexes.
				/// </returns>
				/// <remarks>
				/// If you pass an existing GameObject from the scene, it will need to exist in the future to be able 
				/// to spawn any additional objects outside of what was preloaded upon its addition to the Object Pool. 
				/// It can be disabled, or inactive, it simply needs to exist if you want to add more later.
				/// </remarks>
				/// <param name="prefabs">An array of Prefabs or GameObjects.</param>
				/// <param name="preloadAmount">The number of objects that should be preloaded.</param>
				/// <param name="spawnMore">Should more objects be added to the pool as needed?</param>
				/// <param name="slowMessage">Should Unity's SendMessage be used?</param>
				/// <param name="handleParticles">Should accomodations for Particles be made?</param>
				/// <param name="trackSpawned">Should spawned objects be tracked?</param>
				/// <param name="cullExtras">Should extra objects be removed from the Object Pool when not in use?</param>
				/// <param name="cullInterval">How often should we check for extras to cull.</param>
				public int[] Add (GameObject[] prefabs, int preloadAmount, bool spawnMore, bool slowMessage, 
				                  bool handleParticles, bool trackSpawned, bool cullExtras, float cullInterval)
				{
						var returnIDs = new int[prefabs.Length];

						for (int x = 0; x < prefabs.Length; x++) {
								returnIDs [x] = Add (prefabs [x], preloadAmount, spawnMore, slowMessage, handleParticles, 
										trackSpawned, cullExtras, cullInterval);
						}

						return returnIDs;
				}

				/// <summary>
				/// Despawn the specified Transform's GameObject back into it's Object Pool.
				/// </summary>
				/// <remarks>
				/// This method should only be used if the Pool ID is not known.
				/// </remarks>
				/// <param name="transform">Target Transform</param>
				public void Despawn (Transform transform)
				{
						Despawn (transform.gameObject, GetPoolID (gameObject.name));
				}

				/// <summary>
				/// Despawn the specified GameObject back into it's Object Pool.
				/// </summary>
				/// <remarks>
				/// This method should only be used if the Pool ID is not known.
				/// </remarks>
				/// <param name="gameObject">Target GameObject.</param>
				public void Despawn (GameObject gameObject)
				{
						Despawn (gameObject, GetPoolID (gameObject.name));
				}

				/// <summary>
				/// Despawn the specified Transform's GameObject back into it's Object Pool.
				/// </summary>
				/// <param name="transform">Target Transform.</param>
				/// <param name="poolID">The Pool ID</param>
				public void Despawn (Transform transform, int poolID)
				{
						ObjectPools [poolID].Despawn (transform.gameObject);
				}

				/// <summary>
				/// Despawn the specified GameObject back into it's Object Pool.
				/// </summary>
				/// <param name="gameObject">Target GameObject</param>
				/// <param name="poolID">The Pool ID</param>
				public void Despawn (GameObject gameObject, int poolID)
				{
						ObjectPools [poolID].Despawn (gameObject);
				}

				/// <summary>
				/// Gets the Pool ID for the GameObject.
				/// </summary>
				/// <returns>The Pool ID, -1 if not found.</returns>
				/// <param name="gameObject">Target GameObject.</param>
				public int GetPoolID (GameObject gameObject)
				{
						return GetPoolID (gameObject.name);
				}

				/// <summary>
				/// Gets the Pool ID by name.
				/// </summary>
				/// <returns>The Pool ID, -1 if not found.</returns>
				/// <param name="prefabName">Lookup Name</param>
				public int GetPoolID (string prefabName)
				{
						if (string.IsNullOrEmpty (prefabName)) {
								throw new MissingReferenceException (
										"You are passing a null or empty prefabName to hObjectPool.Instance.GetPoolID()");
						}	
						if (_poolStringLookupTable == null)
								return -1;

						return _poolStringLookupTable.ContainsKey (prefabName) ? _poolStringLookupTable [prefabName] : -1;
				}

				/// <summary>
				/// Remove the specified GameObject from it's Object Pool.
				/// </summary>
				/// <param name="gameObject">Target GameObject</param>
				/// <param name="destroyImmediate">Should the object be destroied immediately.</param>
				public bool Remove (GameObject gameObject, bool destroyImmediate)
				{
						var tempLookup = GetPoolID (gameObject.name);
						if (tempLookup != -1) {
								// Remove object from lookup table
								_poolStringLookupTable.Remove (gameObject.name);
				
								// Remove from pooling system
								return ObjectPools [tempLookup].RemoveFromPool (gameObject, destroyImmediate);
						}
						return false;
				}

				/// <summary>
				/// Spawn a GameObject from it's Object Pool.
				/// </summary>
				/// <remarks>
				/// This method is slower then using Spawn(PoolID), as it is doing a lookup.
				/// </remarks>
				/// <param name="transform">Target Transform.</param>
				public GameObject Spawn (Transform transform)
				{
						return ObjectPools [GetPoolID (transform.gameObject.name)].Spawn (Vector3.zero, Quaternion.identity);
				}

				/// <summary>
				/// Spawn a GameObject from it's Object Pool.
				/// </summary>
				/// <remarks>
				/// This method is slower then using Spawn(PoolID), as it is doing a lookup.
				/// </remarks>
				/// <param name="gameObject">Target GameObject.</param>
				public GameObject Spawn (GameObject gameObject)
				{
						return ObjectPools [GetPoolID (gameObject.name)].Spawn (Vector3.zero, Quaternion.identity);
				}

				/// <summary>
				/// Spawn a GameObject from it's Object Pool.
				/// </summary>
				/// <remarks>
				/// This method is slower then using Spawn(PoolID), as it is doing a lookup.
				/// </remarks>
				/// <param name="transform">Target Transform.</param>
				/// <param name="position">Position at which to spawn GameObject.</param>
				/// <param name="rotation">The rotation to set for the spawned GameObject.</param>
				public GameObject Spawn (Transform transform, Vector3 position, Quaternion rotation)
				{
						return ObjectPools [GetPoolID (transform.gameObject.name)].Spawn (position, rotation);
				}

				/// <summary>
				/// Spawn a GameObject from it's Object Pool.
				/// </summary>
				/// <remarks>
				/// This method is slower then using Spawn(PoolID), as it is doing a lookup.
				/// </remarks>
				/// <param name="gameObject">Target GameObject.</param>
				/// <param name="position">Position at which to spawn GameObject.</param>
				/// <param name="rotation">The rotation to set for the spawned GameObject.</param>
				public GameObject Spawn (GameObject gameObject, Vector3 position, Quaternion rotation)
				{
						return ObjectPools [GetPoolID (gameObject.name)].Spawn (position, rotation);
				}

				/// <summary>
				/// Spawn a GameObject from it's Object Pool.
				/// </summary>
				/// <param name="poolID">Pool ID of Pool to spawn from.</param>
				public GameObject Spawn (int poolID)
				{
						return ObjectPools [poolID].Spawn (Vector3.zero, Quaternion.identity);
				}

				/// <summary>
				/// Spawn a GameObject from it's Object Pool.
				/// </summary>
				/// <param name="poolID">Pool ID of Pool to spawn from.</param>
				/// <param name="position">Position at which to spawn GameObject.</param>
				/// <param name="rotation">The rotation to set for the spawned GameObject</param>
				public GameObject Spawn (int poolID, Vector3 position, Quaternion rotation)
				{
						return ObjectPools [poolID].Spawn (position, rotation);
				}

				/// <summary>
				/// Unity's Awake Event
				/// </summary>
				protected virtual void Awake ()
				{
						if (ObjectPools == null)
								ObjectPools = new ObjectPoolCollection[0];
						if (_poolStringLookupTable == null)
								_poolStringLookupTable = new Dictionary<string, int> ();

						for (int x = 0; x < ObjectPools.Length; x++) {

								if (ObjectPools [x].Prefab == null) {
										throw new MissingReferenceException ("You have not set the prefab in the hObjectPool Inspector for Object Pools->Element " + x);
								}

								if (!ObjectPools [x].Initialized) {
										ObjectPools [x].Initialize (ObjectPools [x].Prefab, transform, x);
										_poolStringLookupTable.Add (ObjectPools [x].Prefab.name, x);
								}
						}
				}

				/// <summary>
				/// Unity's Update Event
				/// </summary>
				protected virtual void Update ()
				{
						for (int x = 0; x < ObjectPools.Length; x++) {

								if (ObjectPools [x].TrackObjects && ObjectPools [x].CullExtras) {
										ObjectPools [x].CullUpdate ();
								}
						}
				}
		}
}