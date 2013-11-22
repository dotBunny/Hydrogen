#region Copyright Notice & License Information
// 
// WebPool.cs
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

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen.Core
{
	/// <summary>
	/// An internal web pool system used within the Hydrogen Framework. 
	/// </summary>
	[AddComponentMenu("")]
	public class WebPool : MonoBehaviour
	{
		private int _poolID;


		public void Awake()
		{
			// Create our buddy object
			GameObject newWebObject = new GameObject();
			newWebObject.AddComponent(typeof(Hydrogen.Core.WebPoolItem));
			newWebObject.name = "Web Call";

			_poolID = hObjectPool.Instance.Add(newWebObject);

			// Remove our little buddy
			GameObject.DestroyImmediate(newWebObject);
		}

		public void GET(string URI, string cookie, System.Action<int, string> callback)
		{
			GameObject go = hObjectPool.Instance.Spawn(_poolID);
			go.GetComponent<WebPoolItem>().GET(URI, cookie, callback);
		}

		public void POST(string URI, string contentType, string payload, string cookie, System.Action<int, string> callback)
		{
			GameObject go = hObjectPool.Instance.Spawn(_poolID);
			go.GetComponent<WebPoolItem>().POST(URI, contentType, payload, cookie, callback);
		}
	}
}