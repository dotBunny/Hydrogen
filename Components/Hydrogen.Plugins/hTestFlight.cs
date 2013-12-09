#region Copyright Notice & License Information
//
// hInput.cs
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
/// A drop in implementation of the Hydrogen.Plugins.TestFlight manager. It implements advanced features included with
/// TestFlight allowing for proper session tracking and reporting.
/// </summary>
[AddComponentMenu ("Hydrogen/TestFlight Manager")]
public sealed class hTestFlight : MonoBehaviour
{
		/// <summary>
		/// Should this TestFlight manager survive scene switches.
		/// </summary>
		public bool Presistant = true;
		/// <summary>
		/// The TestFlight token for Android.
		/// </summary>
		public string TokenAndroid = "";
		/// <summary>
		/// The TestFlight token iOS.
		/// </summary>
		public string TokenIOS = "";
		/// <summary>
		/// Internal reference to the static instance of the TestFlight interface.
		/// </summary>
		static volatile hTestFlight _staticInstance;
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
		/// Gets the TestFlight interface instance.
		/// </summary>
		/// <value>
		/// The TestFlight interface
		/// </value>
		public static hTestFlight Instance {
				get {
						if (_staticInstance == null) {				
								lock (_syncRoot) {
										_staticInstance = FindObjectOfType (typeof(hTestFlight)) as hTestFlight;

										// If we don't have it, lets make it!
										if (_staticInstance == null) {

												var go = GameObject.Find (Hydrogen.Components.DefaultSingletonName) ??
												         new GameObject (Hydrogen.Components.DefaultSingletonName);

												go.AddComponent<hTestFlight> ();
												_staticInstance = go.GetComponent<hTestFlight> ();
										}
								}
						}
						return _staticInstance;
				}
		}

		/// <summary>
		/// Adds an entry into the Key-Value store for this TestFlight session.
		/// </summary>
		/// <param name="key">The Key.</param>
		/// <param name="data">The Value.</param>

		public void AddCustomEnvironmentInformation (string key, string data)
		{
				// Make sure that we are infact flying and there is a session present, if not we'll dump out a 
				// message to console if on a platform where this should have worked.
				if (!Hydrogen.Plugins.TestFlight.Flying || !Hydrogen.Plugins.TestFlight.Session) {

						#if (UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
						Debug.Log ("Unable to send information, TestFlight is not in flight.");
						#endif

						return;
				}

				// Notice that the data/key is swapped here. Not to sure why TestFlight decided on this format, 
				// it really seems backwards.
				Hydrogen.Plugins.TestFlight.AddCustomEnvironmentInformation (data, key);
		}

		/// <summary>
		/// Does a TestFlight manager instance exist?
		/// </summary>
		public static bool Exists ()
		{
				return _staticInstance != null;
		}

		/// <summary>
		/// Initialize our TestFlight plugin with the appropriate settings.
		/// </summary>
		public IEnumerator Initialize ()
		{
				// Depending on the platform there are some things that need to be handled before we can 
				// take off and start submitting data
				Hydrogen.Plugins.TestFlight.Initialize ();
		
				// Issue the appropriate "takeoff" token
#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
				Hydrogen.Plugins.TestFlight.TakeOff(TokenIOS);
#elif UNITY_ANDROID && !UNITY_EDITOR
				Hydrogen.Plugins.TestFlight.TakeOff(TokenAndroid);
#endif
				// Wait for the end of the frame for kicks
				yield return new WaitForEndOfFrame ();

				// Start up our session captain!
				Hydrogen.Plugins.TestFlight.StartSession ();
		}

		/// <summary>
		/// Send a message to TestFlight to appear in it's console.
		/// </summary>
		/// <param name="message">The Message.</param>
		public void Log (string message)
		{
				// Make sure that we are infact flying and there is a session present, if not we'll dump out a 
				// message to console if on a platform where this should have worked.
				if (!Hydrogen.Plugins.TestFlight.Flying || !Hydrogen.Plugins.TestFlight.Session) {
						#if (UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
						Debug.Log ("Unable to send log data, TestFlight is not in flight.");
						#endif
						return;
				}

				Hydrogen.Plugins.TestFlight.Log (message);
		}

		/// <summary>
		/// Send a message to TestFlight asynchronously to appear in it's console.
		/// </summary>
		/// <param name="message">The Message.</param>
		public void LogAsync (string message)
		{
				// Make sure that we are infact flying and there is a session present, if not we'll dump out a 
				// message to console if on a platform where this should have worked.
				if (!Hydrogen.Plugins.TestFlight.Flying || !Hydrogen.Plugins.TestFlight.Session) {
						#if (UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
						Debug.Log ("Unable to send log data, TestFlight is not in flight.");
						#endif
						return;
				}

				Hydrogen.Plugins.TestFlight.LogAsync (message);
		}

		/// <summary>
		/// Report to TestFlight that the session has passed a Checkpoint.
		/// </summary>
		/// <param name="checkpointName">The Checkpoint name.</param>
		public void PassCheckpoint (string checkpointName)
		{
				// Make sure that we are infact flying and there is a session present, if not we'll dump out a 
				// message to console if on a platform where this should have worked.
				if (!Hydrogen.Plugins.TestFlight.Flying || !Hydrogen.Plugins.TestFlight.Session) {
						#if (UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
						Debug.Log ("Unable to send checkpoint data, TestFlight is not in flight.");
						#endif
						return;
				}

				Hydrogen.Plugins.TestFlight.PassCheckpoint (checkpointName);
		}

		/// <summary>
		/// Submits the feedback.
		/// </summary>
		/// <param name="message">The feedback message.</param>
		public void SubmitFeedback (string message)
		{
				// Make sure that we are infact flying and there is a session present, if not we'll dump out a 
				// message to console if on a platform where this should have worked.
				if (!Hydrogen.Plugins.TestFlight.Flying || !Hydrogen.Plugins.TestFlight.Session) {
#if (UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
						Debug.Log ("Unable to submit feedback, TestFlight is not in flight.");
#endif
						return;
				}

				Hydrogen.Plugins.TestFlight.SubmitFeedback (message);
		}

		/// <summary>
		/// Unity's Awake Event
		/// </summary>
		void Awake ()
		{
				// Should this gameObject be kept around :) I think so.
				if (Presistant)
						DontDestroyOnLoad (gameObject);

				// Let's get this party started, but in a somewhat safe manner
				StartCoroutine (Initialize ());
		}

		/// <summary>
		/// Raised when your application is paused, as a more accurate depiction of Session times.
		/// </summary>
		/// <remarks>
		/// This was meant originally as a fix for Android, but we've carried it over to iOS and 
		/// switched everything to manual for better integration with Unity
		/// </remarks>
		void OnApplicationPause ()
		{
				if (!Hydrogen.Plugins.TestFlight.Session)
						Hydrogen.Plugins.TestFlight.StartSession ();
				else
						Hydrogen.Plugins.TestFlight.EndSession ();
		}

		/// <summary>
		/// Raised when your application is quit, ending the session for you via code.
		/// </summary>
		void OnApplicationQuit ()
		{
				Hydrogen.Plugins.TestFlight.EndSession ();
		}
}