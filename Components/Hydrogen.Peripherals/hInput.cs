#region Copyright Notice & License Information
//
// hInput.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
//       Robin Southern <betajaen@ihoed.com>
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

/// <summary>
/// A drop in implementation of the Hydrogen.Peripherals.Input manager.  This simply makes the class an accessible 
/// singleton with some very simple additional functionality.
/// </summary>
[AddComponentMenu ("Hydrogen/Singletons/Input")]
public sealed class hInput : Hydrogen.Peripherals.Input
{
		/// <summary>
		/// Should this input manager survive scene switches?
		/// </summary>
		public bool Persistent = true;
		/// <summary>
		/// Internal fail safe to maintain instance across threads.
		/// </summary>
		/// <remarks>
		/// Multithreaded Safe Singleton Pattern.
		/// </remarks>
		/// <description>
		/// http://msdn.microsoft.com/en-us/library/ms998558.aspx
		/// </description>
		static readonly System.Object _syncRoot = new System.Object ();
		/// <summary>
		/// Internal reference to the static instance of the input manager.
		/// </summary>
		static volatile hInput _staticInstance;

		/// <summary>
		/// Gets the input manager instance, creating one if none is found.
		/// </summary>
		/// <value>
		/// The Input Manager.
		/// </value>
		public static hInput Instance {
				get {
						if (_staticInstance == null) {
								lock (_syncRoot) {
										_staticInstance = FindObjectOfType (typeof(hInput)) as hInput;

										// If we don't have it, lets make it!
										if (_staticInstance == null) {
												var go = GameObject.Find (Hydrogen.Components.DefaultSingletonName) ??
												         new GameObject (Hydrogen.Components.DefaultSingletonName);

												go.AddComponent<hInput> ();
												_staticInstance = go.GetComponent<hInput> ();
										}
								}
						}
						return _staticInstance;
				}
		}

		/// <summary>
		/// Does an Input Manager already exist?
		/// </summary>
		public static bool Exists ()
		{
				return _staticInstance != null;
		}

		/// <summary>
		/// Unity's Awake Event
		/// </summary>
		protected override void Awake ()
		{
				// Make sure to do the object pools normal initialization
				base.Awake ();

				// Should this gameObject be kept around :) I think so.
				if (Persistent)
						DontDestroyOnLoad (gameObject);
		}
}