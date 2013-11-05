#region Copyright Notice & License Information
// 
// ObjectPool.cs
//  
// Author:
//   Matthew Davey <matthew.davey@dotbunny.com>
//
// Copyright (C) 2013 dotBunny Inc. (http://www.dotbunny.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen.Core
{
	/// <summary>
	/// An internal pooling system used within the Hydrogen Framework. 
	/// 
	/// Each GameObject / Prefab has its own pool, this is done for many reasons, 
	/// but ultimately seems to be the best performance scenario.
	/// </summary>
	public class ObjectPool : MonoBehaviour
	{
		public int defaultPreloadAmount = 5;
		public bool defaultSpawnMore = true;
		public bool defaultSendMessage = false;
		public bool defaultHandleParticles = true;
		public bool defaultTrackSpawnedObjects = false;
		public bool defaultCullExtras = true;
			
		public Hydrogen.Core.ObjectPoolCollection[] objectPools;
		private Dictionary<string, int> _poolStringLookupTable;
		
		
		public void Awake()
		{
			if (objectPools == null) objectPools = new Hydrogen.Core.ObjectPoolCollection[0];
			if ( _poolStringLookupTable == null ) _poolStringLookupTable = new Dictionary<string, int>();
		
			
			for (int x = 0; x < objectPools.Length; x++ )
			{
				
				if ( objectPools[x].prefab == null ) 
				{
					throw new MissingReferenceException("You have not set the prefab in the hObjectPool Inspector for Object Pools->Element " + x.ToString());
				}
				
				if ( !objectPools[x].Initialized )
				{
					objectPools[x].Initialize(objectPools[x].prefab, this.transform, x);
					_poolStringLookupTable.Add(objectPools[x].prefab.name,x);
				}
			}
		}
		
		public int GetPoolID(GameObject gameObject)
		{
			return GetPoolID(gameObject.name);
		}
		public int GetPoolID(string prefabName)
		{
			if ( prefabName == null || prefabName == "" ) {
				throw new MissingReferenceException(
					"You are passing a null or empty prefabName to hObjectPool.Instance.GetPoolID()");
			}	
			
			if ( _poolStringLookupTable.ContainsKey(prefabName) )
			{
				return _poolStringLookupTable[prefabName];
			}
			
			return -1;
		}
		

		public int[] Add(GameObject[] gameObjects)
		{
			return Add(gameObjects, defaultPreloadAmount, defaultSpawnMore, defaultSendMessage, defaultHandleParticles, defaultTrackSpawnedObjects, defaultCullExtras);
		}
		public int[] Add(GameObject[] gameObjects, int preloadAmount)
		{
			return Add(gameObjects, preloadAmount, defaultSpawnMore, defaultSendMessage, defaultHandleParticles, defaultTrackSpawnedObjects, defaultCullExtras);
		}
		public int[] Add(GameObject[] gameObjects, int preloadAmount, bool spawnMore, bool slowMessage, bool handleParticles, bool trackSpawned, bool cullExtras)
		{
			int[] returnIDs = new int[gameObjects.Length];
			
			for ( int x = 0; x < gameObjects.Length; x++ )
			{
				returnIDs[x] = Add (gameObjects[x], preloadAmount, spawnMore, slowMessage,handleParticles, trackSpawned, cullExtras);
			}
			
			return returnIDs;
		}
		
		public int Add(GameObject gameObject)
		{
			return Add(gameObject, defaultPreloadAmount, defaultSpawnMore, defaultSendMessage, defaultHandleParticles, defaultTrackSpawnedObjects, defaultCullExtras);
		}
		public int Add(GameObject gameObject, int preload)
		{
			return Add (gameObject, preload, defaultSpawnMore, defaultSendMessage, defaultHandleParticles, defaultTrackSpawnedObjects, defaultCullExtras);
		}
		public int Add(GameObject gameObject, int preload, bool spawnMore, bool slowMessage, bool handleParticles, bool trackSpawned, bool cullExtras)
		{
			if ( gameObject == null ) {
				throw new MissingReferenceException("You are passing a null gameObject reference to hObjectPool.Instance.Add()");
			}
			
			int tempLookup = GetPoolID(gameObject.name);
			if ( tempLookup == -1 )
			{
				// Create our new pool
				Hydrogen.Core.ObjectPoolCollection newPool = new Hydrogen.Core.ObjectPoolCollection(preload, spawnMore, slowMessage, handleParticles, trackSpawned, cullExtras);
				
				// Initialize pool
				newPool.Initialize(gameObject, this.transform, objectPools.Length);
				
				// Add a pool for the new object
				Hydrogen.Array.Add<Hydrogen.Core.ObjectPoolCollection>(ref objectPools, newPool, false);
				
				// Add reference for lookup table
				if ( _poolStringLookupTable == null ) _poolStringLookupTable = new Dictionary<string, int>();
				
				_poolStringLookupTable.Add (newPool.prefab.name, objectPools.Length - 1);
				
				return objectPools.Length - 1;
			}
			return tempLookup;
		}

		
		public bool Remove(GameObject gameObject, bool destroyImmediate)
		{
			int tempLookup = GetPoolID(gameObject.name);
			if ( tempLookup != -1 )
			{
				// Remove object from lookup table
				_poolStringLookupTable.Remove(gameObject.name);
				
				// Remove from pooling system
				return objectPools[tempLookup].RemoveFromPool(gameObject, destroyImmediate);
			}
			return false;
		}
		
	    /// <summary>
	    /// Spawn a GameObject out of the pools.
	    /// </summary>
	    /// <remarks>
	    /// This method is slower then using Spawn(ID), as it is doing a lookup.
	    /// </remarks>
		public GameObject Spawn(Transform transform)
		{
			return objectPools[GetPoolID(transform.gameObject.name)].Spawn(Vector3.zero, Quaternion.identity);
		}
		public GameObject Spawn(GameObject gameObject)
		{
			return objectPools[GetPoolID(gameObject.name)].Spawn(Vector3.zero, Quaternion.identity);
		}
		public GameObject Spawn(Transform transform, Vector3 position, Quaternion rotation)
		{
			return objectPools[GetPoolID(transform.gameObject.name)].Spawn(position, rotation);
		}
		public GameObject Spawn(GameObject gameObject, Vector3 position, Quaternion rotation)
		{
			return objectPools[GetPoolID(gameObject.name)].Spawn(position, rotation);
		}
		public GameObject Spawn(int poolID)
		{
			return objectPools[poolID].Spawn(Vector3.zero, Quaternion.identity);
		}
		public GameObject Spawn(int poolID, Vector3 position, Quaternion rotation)
		{
			return objectPools[poolID].Spawn(position, rotation);
		}
	
		
		/// <summary>
		/// Despawn the specified GameObject back into its object pool
		/// </summary>
		/// <param name='go'>
		/// Target GameObject
		/// </param>
		public void Despawn(Transform transform)
		{
			Despawn(transform.gameObject, GetPoolID(gameObject.name));
		}
		public void Despawn(GameObject gameObject)
		{
			Despawn(gameObject, GetPoolID(gameObject.name));
		}
		public void Despawn(Transform transform, int poolID)
		{
			objectPools[poolID].Despawn(transform.gameObject);
		}
		public void Despawn(GameObject gameObject, int poolID)
		{
			objectPools[poolID].Despawn(gameObject);
		}
	}
}