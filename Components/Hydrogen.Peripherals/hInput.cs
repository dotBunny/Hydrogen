#region Copyright Notice & License Information
// 
// hInput.cs
//  
// Author:
//	 Matthew Davey <matthew.davey@dotbunny.com>
//   Robin Southern <betajaen@ihoed.com>
//
// Copyright (c) 2013 dotBunny Inc. (http://www.dotbunny.com)
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
using System.Collections.Generic;
using Hydrogen.Core;
using UnityEngine;


/// <summary>
/// Input manager to bind controls to actions
/// </summary>
[AddComponentMenu("Hydrogen/Input Manager")]
public class hInput : Hydrogen.Peripherals.Input
{

  private static readonly System.Object _syncRoot = new System.Object();

  private static volatile hInput _staticInstance;

  public static hInput Instance
  {
    get
    {
      if (_staticInstance == null)
      {
        lock (_syncRoot)
        {
          _staticInstance = FindObjectOfType(typeof(hInput)) as hInput;

          // If we don't have it, lets make it!
          if (_staticInstance == null)
          {
			GameObject go = GameObject.Find("Hydrogen");
			if ( go == null ) go = new GameObject("Hydrogen");

            go.AddComponent<hInput>();
            _staticInstance = go.GetComponent<hInput>();
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
    if (_staticInstance == null) return false;
    else return true;
  }
}