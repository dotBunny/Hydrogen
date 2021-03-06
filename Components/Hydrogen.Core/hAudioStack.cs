#region Copyright Notice & License Information
//
// hAudioStack.cs
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

/// <summary>
/// A drop in implementation of the Hydrogen.Core.AudioStack. This simply makes the class an accessible singleton 
/// with some very simple additional functionality.
/// </summary>
[AddComponentMenu ("Hydrogen/Singletons/Audio Stack")]
public sealed class hAudioStack : Hydrogen.Core.AudioStack
{
		/// <summary>
		/// Should this Audio Stack survive scene switches?
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
		/// Internal reference to the static instance of the Audio Stack.
		/// </summary>
		static volatile hAudioStack _staticInstance;

		/// <summary>
		/// Gets the Audio Stack instance, creating one if none is found.
		/// </summary>
		/// <value>
		/// The Audio Stack.
		/// </value>
		public static hAudioStack Instance {
				get {
						if (_staticInstance == null) {
								lock (_syncRoot) {
										_staticInstance = FindObjectOfType (typeof(hAudioStack)) as hAudioStack;

										// If we don't have it, lets make it!
										if (_staticInstance == null) {
												var go = GameObject.Find (Hydrogen.Components.DefaultSingletonName) ??
												         new GameObject (Hydrogen.Components.DefaultSingletonName);

												go.AddComponent<hAudioStack> ();
												_staticInstance = go.GetComponent<hAudioStack> ();	
										}
								}
						}
						return _staticInstance;
				}
		}

		/// <summary>
		/// Does an Audio Stack already exist?
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
				// Make sure to do the Audio Stack normal initialization
				base.Awake ();

				// Should this gameObject be kept around :) I think so.
				if (Persistent)
						DontDestroyOnLoad (gameObject);

		}
}