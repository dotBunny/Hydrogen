#region Copyright Notice & License Information
// 
// ObjectPoolCollection.cs
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

namespace Hydrogen.Core
{
	[System.Serializable]
	public class ObjectPoolCollection
	{		
		public GameObject prefab;
		public int preloadAmount;
		public bool spawnMoreIfNeeded;
		public bool sendMessage = false;
		public bool manageParticles = true;
		public bool trackSpawnedObjects = false;
		public bool cullExtraObjects = false;
		
		
		[HideInInspector]
		public bool hasObjectPoolItem;
		[HideInInspector]
		public bool hasParticleSystem;
		[HideInInspector]
		public bool hasLegacyParticleEmitter;
		[HideInInspector]
		public bool hasLegacyParticleAnimator;
		[HideInInspector]
		public bool hasRigidbody;
	
		
		/// <summary>
    	/// The pooled objects currently available.
		/// </summary>
    	private GameObject[] _pooledObjects;
		private Transform _parentTransform;
		private int _poolID;
		
		private GameObject[] _spawnedObjects = new GameObject[0];
		
		public GameObject[] SpawnedObjects { get { return _spawnedObjects; }}
		
		private bool _initialized = false;
		public bool Initialized { get { return _initialized; }}
		
		
		
		public ObjectPoolCollection(int preload, bool spawnMore, bool slowMessage, bool handleParticles, bool trackSpawned, bool cullExtras)
		{
			manageParticles = handleParticles;
			preloadAmount = preload;
			spawnMoreIfNeeded = spawnMore;
			sendMessage = slowMessage;
			trackSpawnedObjects = trackSpawned;
			cullExtraObjects = cullExtras;
		}
		
		public void Initialize(GameObject gameObject, Transform parent, int poolID)
		{
			prefab = gameObject;
			
			_parentTransform = parent;
			_poolID = poolID;
			_pooledObjects = new GameObject[0];
			//_spawnedObjects = new GameObject[0];
			
			if ( prefab.GetComponent<ObjectPoolItemBase>() ) hasObjectPoolItem = true;
			
			if ( gameObject.GetComponent<Rigidbody>() ) { hasRigidbody = true; }
			
			if ( manageParticles )
			{
				if (gameObject.GetComponent<ParticleEmitter>() ) 
				{
					hasLegacyParticleEmitter = true;
				}
				if ( gameObject.GetComponent<ParticleAnimator>() )
				{
					hasLegacyParticleAnimator = true;
				}
				if ( gameObject.GetComponent<ParticleSystem>() )
				{
					hasParticleSystem = true;
				}
			}
			
			if ( !hasLegacyParticleAnimator && !hasLegacyParticleEmitter && !hasParticleSystem )
			{
				manageParticles =  false;
			}
			
			for ( int x = 0; x < preloadAmount; x++ )
			{
				AddToPool();
			}
			_initialized = true;
		}
		
		public void AddToPool()
		{
			GameObject newObject = GameObject.Instantiate(prefab) as GameObject;
			
			newObject.name = prefab.name;
			newObject.GetComponent<ObjectPoolItemBase>().parentPool = this;
			newObject.GetComponent<ObjectPoolItemBase>().poolID = _poolID;
			
			Hydrogen.Array.Add<GameObject>(ref _pooledObjects, newObject, false);
			
			Despawn(newObject, true);
		}
		
		
		public bool RemoveFromPool(GameObject gameObject, bool destroyObject)
		{
			return Hydrogen.Array.Remove<GameObject>(ref _pooledObjects, gameObject);
			if ( destroyObject ) GameObject.DestroyImmediate(gameObject);
		}
		
		
		
		public GameObject Spawn(Vector3 location, Quaternion rotation)
		{
			if ( _pooledObjects != null && _pooledObjects.Length > 0 )
			{
				GameObject spawnedObject = _pooledObjects[0];
				
				if ( spawnedObject == null ) { Debug.LogError(
						"[h] A GameObject (" + prefab.name + ") has been destroyed," +
						"but is still referenced by the Object Pool. If you must destroy a GameObject manually," +
						"please make sure to remove it from the pool as well:" +
						"hObjectPool.Instance.Remove(GameObject);");
						
					return null;
				}
					
				Hydrogen.Array.RemoveAt<GameObject>(ref _pooledObjects, 0);
				
				// Move and rotate before we call on spawned
				spawnedObject.transform.position = location;
				spawnedObject.transform.rotation = rotation;
				
				if ( manageParticles )
				{
					if (hasLegacyParticleAnimator) spawnedObject.GetComponent<ParticleAnimator>().autodestruct = false;
					if (hasLegacyParticleEmitter) spawnedObject.GetComponent<ParticleEmitter>().emit = true;
				}
				
				if ( hasObjectPoolItem ) 
				{
					//spawnedObject.GetComponent<ObjectPoolItemBase>().poolID = _poolID; 
					
					spawnedObject.GetComponent<ObjectPoolItemBase>().OnSpawned();
				}
				else if ( sendMessage ) 
				{
					spawnedObject.SendMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					spawnedObject.SetActive(true);
				}
				
				if ( trackSpawnedObjects ) Hydrogen.Array.Add<GameObject>(ref _spawnedObjects, spawnedObject, false);
				
				return spawnedObject;
			}
			else if ( spawnMoreIfNeeded ) 
			{
				AddToPool();
				return Spawn(location, rotation);
			}
			else
			{
				return null;
			}
		}
		

		public void Despawn(GameObject gameObject)
		{
			Despawn(gameObject, false);
		}
		public void Despawn(GameObject gameObject, bool onSpawn)
		{
			if ( !manageParticles ) gameObject.transform.parent = _parentTransform;
			
			// Has our handler
			if ( hasObjectPoolItem ) 
			{
				if ( manageParticles )
				{
					gameObject.GetComponent<ObjectPoolItemBase>().DespawnSafely();	
					return;
				}
				else
				{
					gameObject.GetComponent<ObjectPoolItemBase>().OnDespawned();	
				}
				
			}
			// Use slow method
			else if (sendMessage )
			{
				if ( manageParticles )
				{
					gameObject.SendMessage("WaitToDespawn", SendMessageOptions.DontRequireReceiver);
					return;
				}
				else
				{
					gameObject.SendMessage("OnDespawned", SendMessageOptions.DontRequireReceiver);
				}
			}
			// Fail safe
			else
			{
				// This stops things from keeping their velocity from previous
				if ( hasRigidbody ) 
				{
					gameObject.rigidbody.velocity = Vector3.zero;
					gameObject.rigidbody.angularVelocity = Vector3.zero;
				}
				
				gameObject.SetActive(false);
				
			}
				
			if ( !onSpawn )
			{
				if ( trackSpawnedObjects ) Hydrogen.Array.Remove<GameObject>(ref _spawnedObjects, gameObject);
				Hydrogen.Array.Add<GameObject>(ref _pooledObjects, gameObject, false);
			}
		}
		
		public void DespawnImmediate(GameObject gameObject)
		{
			gameObject.transform.parent = _parentTransform;
			if ( trackSpawnedObjects ) Hydrogen.Array.Remove<GameObject>(ref _spawnedObjects, gameObject);
			Hydrogen.Array.Add<GameObject>(ref _pooledObjects, gameObject, false);	
		}
	}
}