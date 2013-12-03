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
				//TODO Add documentation
				public int DefaultPreloadAmount = 5;
				public bool DefaultSpawnMore = true;
				public bool DefaultSendMessage;
				public bool DefaultHandleParticles = true;
				public bool DefaultTrackSpawnedObjects;
				public bool DefaultCullExtras = true;
				public ObjectPoolCollection[] ObjectPools;
				Dictionary<string, int> _poolStringLookupTable;

				public void Awake ()
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

				public int GetPoolID (GameObject gameObject)
				{
						return GetPoolID (gameObject.name);
				}

				public int GetPoolID (string prefabName)
				{
						if (string.IsNullOrEmpty (prefabName)) {
								throw new MissingReferenceException (
										"You are passing a null or empty prefabName to hObjectPool.Instance.GetPoolID()");
						}	
			
						return _poolStringLookupTable.ContainsKey (prefabName) ? _poolStringLookupTable [prefabName] : -1;
			
				}

				public int[] Add (GameObject[] gameObjects)
				{
						return Add (gameObjects, DefaultPreloadAmount, DefaultSpawnMore, DefaultSendMessage, DefaultHandleParticles, DefaultTrackSpawnedObjects, DefaultCullExtras);
				}

				public int[] Add (GameObject[] gameObjects, int preloadAmount)
				{
						return Add (gameObjects, preloadAmount, DefaultSpawnMore, DefaultSendMessage, DefaultHandleParticles, DefaultTrackSpawnedObjects, DefaultCullExtras);
				}

				public int[] Add (GameObject[] gameObjects, int preloadAmount, bool spawnMore, bool slowMessage, bool handleParticles, bool trackSpawned, bool cullExtras)
				{
						var returnIDs = new int[gameObjects.Length];
			
						for (int x = 0; x < gameObjects.Length; x++) {
								returnIDs [x] = Add (gameObjects [x], preloadAmount, spawnMore, slowMessage, handleParticles, trackSpawned, cullExtras);
						}
			
						return returnIDs;
				}

				public int Add (GameObject gameObject)
				{
						return Add (gameObject, DefaultPreloadAmount, DefaultSpawnMore, DefaultSendMessage, DefaultHandleParticles, DefaultTrackSpawnedObjects, DefaultCullExtras);
				}

				public int Add (GameObject gameObject, int preload)
				{
						return Add (gameObject, preload, DefaultSpawnMore, DefaultSendMessage, DefaultHandleParticles, DefaultTrackSpawnedObjects, DefaultCullExtras);
				}

				public int Add (GameObject gameObject, int preload, bool spawnMore, bool slowMessage, bool handleParticles, bool trackSpawned, bool cullExtras)
				{
						if (gameObject == null) {
								throw new MissingReferenceException ("You are passing a null gameObject reference to hObjectPool.Instance.Add()");
						}
			
						int tempLookup = GetPoolID (gameObject.name);
						if (tempLookup == -1) {
								// Create our new pool
								var newPool = new ObjectPoolCollection (preload, spawnMore, slowMessage, handleParticles, trackSpawned, cullExtras);
				
								// Initialize pool
								newPool.Initialize (gameObject, transform, ObjectPools.Length);
				
								// Add a pool for the new object
								Array.Add<ObjectPoolCollection> (ref ObjectPools, newPool, false);
				
								// Add reference for lookup table
								if (_poolStringLookupTable == null)
										_poolStringLookupTable = new Dictionary<string, int> ();
				
								_poolStringLookupTable.Add (newPool.Prefab.name, ObjectPools.Length - 1);
				
								return ObjectPools.Length - 1;
						}
						return tempLookup;
				}

				public bool Remove (GameObject gameObject, bool destroyImmediate)
				{
						int tempLookup = GetPoolID (gameObject.name);
						if (tempLookup != -1) {
								// Remove object from lookup table
								_poolStringLookupTable.Remove (gameObject.name);
				
								// Remove from pooling system
								return ObjectPools [tempLookup].RemoveFromPool (gameObject, destroyImmediate);
						}
						return false;
				}

				/// <summary>
				/// Spawn a GameObject out of the pools.
				/// </summary>
				/// <remarks>
				/// This method is slower then using Spawn(ID), as it is doing a lookup.
				/// </remarks>
				public GameObject Spawn (Transform transform)
				{
						return ObjectPools [GetPoolID (transform.gameObject.name)].Spawn (Vector3.zero, Quaternion.identity);
				}

				public GameObject Spawn (GameObject gameObject)
				{
						return ObjectPools [GetPoolID (gameObject.name)].Spawn (Vector3.zero, Quaternion.identity);
				}

				public GameObject Spawn (Transform transform, Vector3 position, Quaternion rotation)
				{
						return ObjectPools [GetPoolID (transform.gameObject.name)].Spawn (position, rotation);
				}

				public GameObject Spawn (GameObject gameObject, Vector3 position, Quaternion rotation)
				{
						return ObjectPools [GetPoolID (gameObject.name)].Spawn (position, rotation);
				}

				public GameObject Spawn (int poolID)
				{
						return ObjectPools [poolID].Spawn (Vector3.zero, Quaternion.identity);
				}

				public GameObject Spawn (int poolID, Vector3 position, Quaternion rotation)
				{
						return ObjectPools [poolID].Spawn (position, rotation);
				}

				/// <summary>
				/// Despawn the specified GameObject back into its object pool
				/// </summary>
				/// <param name = "transform">
				/// Target Transform
				/// </param>
				public void Despawn (Transform transform)
				{
						Despawn (transform.gameObject, GetPoolID (gameObject.name));
				}

				public void Despawn (GameObject gameObject)
				{
						Despawn (gameObject, GetPoolID (gameObject.name));
				}

				public void Despawn (Transform transform, int poolID)
				{
						ObjectPools [poolID].Despawn (transform.gameObject);
				}

				public void Despawn (GameObject gameObject, int poolID)
				{
						ObjectPools [poolID].Despawn (gameObject);
				}
		}
}