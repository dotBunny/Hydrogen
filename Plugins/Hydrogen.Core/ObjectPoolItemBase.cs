#region Copyright Notice & License Information
//
// ObjectPoolItemBase.cs
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
		/// The base for all classes which want to extend the behaviour of an ObjectPoolItem, these methods need to be 
		/// implemented for the efficient use of the Object Pool system.
		/// </summary>
		public abstract class ObjectPoolItemBase : MonoBehaviour
		{
				/// <summary>
				/// The parent pool of the GameObject.
				/// </summary>
				internal ObjectPoolCollection ParentPool;
				/// <summary>
				/// The Object Pool ID from which this object belongs.
				/// </summary>
				internal int PoolID;

				/// <summary>
				/// Simple function which delays the despawning based on whatever you define.
				/// </summary>
				/// <remarks>
				/// If you are going to utilize this make sure that your function contains:
				/// hObjectPool.Instance.objectPools[poolID].DespawnImmediate(gameObject);
				/// </remarks>
				public abstract void DespawnSafely ();

				/// <summary>
				/// Is the GameObject idle, and therefore can be despawned organically?
				/// </summary>
				/// <returns><c>true</c> if this instance is inactive; otherwise, <c>false</c>.</returns>
				/// <remarks>This will only work on tracked spawned objects.</remarks>
				public abstract bool IsInactive ();

				/// <summary>
				/// Raised when the GameObject is despawned back into it's Object Pool.
				/// </summary>
				/// <remarks>
				/// It does not set "active", you must handle that yourself.
				/// </remarks>
				public abstract void OnDespawned ();

				/// <summary>
				/// Raised when the GameObject is spawned from it's Object Pool.
				/// </summary>
				/// <remarks>
				/// It does not set "active", you must handle that yourself.
				/// </remarks>
				public abstract void OnSpawned ();
		}
}
