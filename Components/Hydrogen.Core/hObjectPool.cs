#region Copyright Notice & License Information
// 
// hObjectPool.cs
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

/// <summary>
/// Hydrogen.Core.ObjectPool Instance
/// </summary>
[AddComponentMenu("Hydrogen/Object Pool Manager")]
public class hObjectPool : Hydrogen.Core.ObjectPool
{
	/// <summary>
	/// Internal fail safe to maintain instance across threads
	/// </summary>
	/// <remarks>
	/// Multithreaded Safe Singleton Pattern
	/// </remarks>
	/// <description>
	/// http://msdn.microsoft.com/en-us/library/ms998558.aspx
	/// </description>
	private static readonly System.Object _syncRoot = new System.Object();
    
	/// <summary>
	/// Internal reference to the static instance of the object pool.
	/// </summary>
	private static volatile hObjectPool _staticInstance;
	
	/// <summary>
	/// Gets the Object Pool instance, creating one if none is found.
	/// </summary>
	/// <value>
	/// The Object Pool
	/// </value>
	public static hObjectPool Instance 
	{
        get {
            if (_staticInstance == null) {				
                lock (_syncRoot) {
                    _staticInstance = FindObjectOfType (typeof(hObjectPool)) as hObjectPool;
                    
					// If we don't have it, lets make it!
					if (_staticInstance == null)
					{
						GameObject go = GameObject.Find("Hydrogen");
						if ( go == null ) go = new GameObject("Hydrogen");

						go.AddComponent<hObjectPool>();
						_staticInstance = go.GetComponent<hObjectPool>();	
                    }
                }
            }
            return _staticInstance;
        }
    }

	/// <summary>
	/// Does an instance exist of this class?
	/// </summary>
	public static bool Exists()
	{
		if ( _staticInstance == null ) return false;
		else return true;
	}
}