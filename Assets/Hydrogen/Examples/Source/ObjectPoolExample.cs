#region Copyright Notice & License Information
// 
// ObjectPoolExample.cs
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

public class ObjectPoolExample : MonoBehaviour {
	
	private int[] _poolIDs;
	public GameObject[] prefabs;
	
	// Use this for initialization
	void Start () {
		
		// Add all of our prefabs to the Object Pool
		_poolIDs = hObjectPool.Instance.Add(prefabs);	
	}
	
	// Update is called once per frame
	void Update () {
		
		// This returns a GameObject reference to the spawned GameObject.
		hObjectPool.Instance.Spawn(
			Random.Range(0, _poolIDs.Length),
			gameObject.transform.position, Random.rotation);
			
	}
}
