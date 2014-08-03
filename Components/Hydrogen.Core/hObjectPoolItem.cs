#region Copyright Notice & License Information
//
// hObjectPoolItem.cs
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

using System.Collections;
using UnityEngine;

/// <summary>
/// A drop in implementation of the Hydrogen.Core.ObjectPoolItem. This is one possible way of setting up an 
/// ObjectPoolItem to handle spawning and despawning appropriately.
/// </summary>
/// <remarks>
/// Learn from it, make your own, as long as you extend from the base class
/// you still get the performance benefits.
/// </remarks>
[AddComponentMenu ("Hydrogen/Object Pool Item")]
public sealed class hObjectPoolItem : Hydrogen.Core.ObjectPoolItemBase
{
		/// <summary>
		/// Despawn gameObject after this number of seconds.
		/// </summary>
		/// <remarks>In seconds, use 0 to disable.</remarks>
		public float LifeTime;

		Rigidbody _rigidbody;
		ParticleEmitter _emitter;
		ParticleSystem _particles;


		public void Awake ()
		{
				_rigidbody = GetComponent<Rigidbody> ();
				_emitter = GetComponent<ParticleEmitter> ();
				_particles = GetComponent<ParticleSystem> ();
		}

		/// <summary>
		/// Despawn the gameObject safely after all particles have done their thing. 
		/// </summary>
		/// <remarks>If you are going to utilize this make sure that your function contains:
		/// hObjectPool.Instance.objectPools[poolID].DespawnImmediate(gameObject);</remarks>
		public override void DespawnSafely ()
		{
				StartCoroutine (WaitForParticles ());
		}

		/// <summary>
		/// Is the object idle, and therefore can be despawned organically?
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <remarks>This will only work on tracked spawned objects.</remarks>
		public override bool IsInactive ()
		{
				// A simple rigidbody check, otherwise no bueno
				return ParentPool.HasRigidbody && _rigidbody.IsSleeping ();
		}

		/// <summary>
		/// Raised when the object is 'despawned' back into the pool.
		/// </summary>
		/// <remarks>It does not set "active", you must handle that yourself.</remarks>
		public override void OnDespawned ()
		{
				// If our object has a rigidbody (cached check), make sure to zero its velocity.
				if (ParentPool.HasRigidbody)
						_rigidbody.velocity = Vector3.zero;
		
				// Disable the gameObject
				gameObject.SetActive (false);
		}

		/// <summary>
		/// Raised when the object is 'spawned' from the pool.
		/// </summary>
		/// <remarks>It does not set "active", you must handle that yourself.</remarks>
		public override void OnSpawned ()
		{
				// Make sure our object is active please and thank you
				gameObject.SetActive (true);

				// If there is a LifeTime greater then 0, we set a timer to despawn
				if (LifeTime > 0)
						StartCoroutine (DespawnTimer ());
		}

		/// <summary>
		/// Coroutine for despawning our gameObject after the timer value.
		/// </summary>
		IEnumerator DespawnTimer ()
		{
				yield return new WaitForSeconds (LifeTime);
				hObjectPool.Instance.Despawn (gameObject, PoolID);
		}

		/// <summary>
		/// Coroutine to wait for particles to be finished, then despawn gameObject.
		/// </summary>
		IEnumerator WaitForParticles ()
		{
				if (_emitter != null) {
						yield return null;
						yield return new WaitForEndOfFrame ();

						while (_emitter.particleCount > 0) {
								yield return null;
						}
						_emitter.emit = false;
				} else if (_particles != null) {
						yield return new WaitForSeconds (_particles.startDelay + 0.25f);
						while (_particles.IsAlive (true)) {
								if (!_particles.gameObject.activeSelf) {
										_particles.Clear (true);
										yield break;
								}
								yield return null;
						}
				}

				// Disable the gameObject
				gameObject.SetActive (false);

				// Immediately get rid of the object as we don't want any artifacts showing up
				hObjectPool.Instance.ObjectPools [PoolID].DespawnImmediate (gameObject);
		}
}