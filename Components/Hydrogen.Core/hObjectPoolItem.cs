#region Copyright Notice & License Information
// 
// hObjectPoolItem.cs
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

/// <summary>
/// This is one possible way of setting up a ObjectPoolItem to handle spawning 
/// and despawning appropriately.
/// </summary>
/// <remarks>
/// Learn from it, make your own, as long as you extend from the base class
/// you still get the performance benefits.
/// </remarks>
public  class hObjectPoolItem : Hydrogen.Core.ObjectPoolItemBase {
	
	public float lifeTime = 0f;
		
	private IEnumerator DespawnTimer()
	{
		yield return new WaitForSeconds(lifeTime);
		hObjectPool.Instance.Despawn(gameObject, poolID);
	}
	
	public override void OnDespawned ()
	{
		if ( parentPool.hasRigidbody ) gameObject.rigidbody.velocity = Vector3.zero;
		
		gameObject.SetActive(false);
	}
	public override void OnSpawned()
	{
		
		gameObject.SetActive(true);
		StartCoroutine(DespawnTimer());
	}	
	
	public override void DespawnSafely()
	{
		StartCoroutine(WaitForParticles());
	}
	
	private IEnumerator WaitForParticles()
	{
		if ( particleEmitter != null)
		{
			yield return null;
			yield return new WaitForEndOfFrame();
			
			while ( particleEmitter.particleCount > 0)
			{
				yield return null;
			}
			particleEmitter.emit = false;
		}
		else if ( particleSystem != null )
		{
			yield return new WaitForSeconds(particleSystem.startDelay + 0.25f);
			while(particleSystem.IsAlive(true))
			{
				if (!particleSystem.gameObject.activeSelf)
       			{
					particleSystem.Clear(true);
					yield break;
        		}
				yield return null;
			}
		}
		gameObject.SetActive(false);
		
		hObjectPool.Instance.objectPools[poolID].DespawnImmediate(gameObject);
	}
}