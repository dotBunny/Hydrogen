#region Copyright Notice & License Information
// 
// WebPoolItem.cs
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
	/// An internal web call used within the Hydrogen Framework. 
	/// </summary>
	public class WebPoolItem : Hydrogen.Core.ObjectPoolItemBase
	{
		private int _hash;

		public override void OnSpawned()
		{
			_hash = 0;
		}

		public override void OnDespawned ()
		{
			StopAllCoroutines();
			_hash = 0;
		}

		public override void DespawnSafely()
		{

		}

		public int Call(string URI, string contentType, string payload, string cookie, System.Action<int, string> callback)
		{
			_hash = (Time.time.ToString() + URI + payload + Random.Range(0,100)).GetHashCode();

			StartCoroutine(GetReturnedText (URI, contentType, payload, cookie, callback));

			return _hash;
		}

		public IEnumerator GetReturnedText(string URI, string contentType, string payload, string cookie, System.Action<int, string> callback)
		{
			// Message Data
			byte[] postData = System.Text.Encoding.ASCII.GetBytes(payload.ToCharArray());
		
			// Process Headers
			Hashtable headers = new Hashtable();

			headers.Add("Content-Type", contentType);
			headers.Add("Content-Length", postData.Length);
			if ( cookie != null ) headers.Add("Cookie", cookie);

			// Make the call
			WWW newCall = new WWW(URI, postData);

			yield return newCall;

			while ( !newCall.isDone ) yield return new WaitForSeconds(0.01f);

			// Callback!
			callback(_hash, newCall.text);

			hObjectPool.Instance.Despawn(this.gameObject, this.poolID);
		}
	}
}