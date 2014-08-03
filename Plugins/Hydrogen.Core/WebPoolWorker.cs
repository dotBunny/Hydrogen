#region Copyright Notice & License Information
//
// WebPoolItem.cs
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
using System.Collections.Generic;
using UnityEngine;

// TODO: Update to handle however Unity fixes the bug with Status Codes
namespace Hydrogen.Core
{
		/// <summary>
		/// An internal web call used within the Hydrogen Framework. 
		/// 
		/// Currently this is broken for servers that alter their status codes based on their operation.
		///
		/// A bug has been filed with Unity demonstrating this:
		/// http://fogbugz.unity3d.com/default.asp?577333_a0l3gs97bj9ubdis
		/// </summary>
		[AddComponentMenu ("")]
		public class WebPoolWorker : ObjectPoolItemBase
		{
				/// <summary>
				/// A hash representation of the current call.
				/// </summary>
				int _hash;
				/// <summary>
				/// A simple reference if a coroutine is running or not.
				/// </summary>
				bool _busy;

				/// <summary>
				/// Fallback Despawn Function
				/// </summary>
				public override void DespawnSafely ()
				{
						OnDespawned ();
				}

				/// <summary>
				/// Form the specified URI, formStringData, formBinaryData, cookie and callback.
				/// </summary>
				/// <returns>The Call Hash</returns>
				/// <param name="URI">The Target URI</param>
				/// <param name="formStringData">A Dictionary<string,string> of Form Data</param>
				/// <param name="formBinaryData">A custom binary dataset. Useful for uploading pictures.</param>
				/// <param name="cookie">Any previous cookie data to be used for authentication.</param>
				/// <param name="callback">A callback function (int hash, Hashtable headers, string payload).</param>
				public int Form (string URI, Dictionary<string,string> formStringData, WebPool.FormBinaryData[] formBinaryData, string cookie, System.Action<int, Dictionary<string,string>, string> callback)
				{
						_hash = (Time.time + URI + formStringData.GetHashCode () + Random.Range (0, 100)).GetHashCode ();

						StartCoroutine (FormReturnedText (URI, formStringData, formBinaryData, cookie, callback));

						return _hash;
				}

				/// <summary>
				/// GE the specified URI, cookie and callback.
				/// </summary>
				/// <returns>The Call Hash</returns>
				/// <param name="URI">The Target URI</param>
				/// <param name="cookie">Any previous cookie data to be used for authentication.</param>
				/// <param name="callback">A callback function (int hash, Hashtable headers, string payload).</param>
				public int GET (string URI, string cookie, System.Action<int, Dictionary<string,string>, string> callback)
				{
						_hash = (Time.time + URI + Random.Range (0, 100)).GetHashCode ();

						StartCoroutine (GetReturnedText (URI, cookie, callback));

						return _hash;
				}

				/// <summary>
				/// Is the GameObject idle, and therefore can be despawned organically?
				/// </summary>
				public override bool IsInactive ()
				{
						return !_busy;
				}

				/// <summary>
				/// Raised when the GameObject is despawned back into it's Web Pool.
				/// </summary>
				public override void OnDespawned ()
				{
						StopAllCoroutines ();
						_hash = 0;
						gameObject.SetActive (false);
				}

				/// <summary>
				/// Raised when the GameObject is spawned from it's Web Pool.
				/// </summary>			
				public override void OnSpawned ()
				{
						gameObject.SetActive (true);
						_hash = 0;
				}

				/// <summary>
				/// HTTP POST Form to URI.
				/// </summary>
				/// <returns>The Call Hash</returns>
				/// <param name="URI">The Target URI.</param>
				/// <param name="formStringData">A Dictionary<string,string> of Form Data</param>
				/// <param name="formBinaryData">A custom binary dataset. Useful for uploading pictures.</param>
				/// <param name="cookie">Any previous cookie data to be used for authentication.</param>
				/// <param name="callback">A callback function (int hash, Hashtable headers, string payload).</param>
				public int POST (string URI, string contentType, string payload, string cookie, System.Action<int, Dictionary<string,string>, string> callback)
				{
						_hash = (Time.time + URI + payload + Random.Range (0, 100)).GetHashCode ();

						StartCoroutine (PostReturnedText (URI, contentType, payload, cookie, callback));

						return _hash;
				}

				/// <summary>
				/// IEnumeratable HTTP POST Form to URI.
				/// </summary>
				/// <returns>IEnumeratable Function</returns>
				/// <param name="URI">The Target URI.</param>
				/// <param name="formStringData">A Dictionary<string,string> of Form Data</param>
				/// <param name="formBinaryData">A custom binary dataset. Useful for uploading pictures.</param>
				/// <param name="cookie">Any previous cookie data to be used for authentication.</param>
				/// <param name="callback">A callback function (int hash, Hashtable headers, string payload).</param>
				IEnumerator FormReturnedText (string URI, Dictionary<string, string> formStringData, WebPool.FormBinaryData[] formBinaryData, string cookie, System.Action<int, Dictionary<string,string>, string> callback)
				{
						// Assign Busy Flag
						_busy = true;

						var newForm = new WWWForm ();


						// Add string data
						if (formStringData != null) {
								foreach (string s in formStringData.Keys) {
										newForm.AddField (s, formStringData [s]);
								}
						}

						// Add binary data
						if (formBinaryData != null) {
								foreach (WebPool.FormBinaryData b in formBinaryData) {
										newForm.AddBinaryData (b.FieldName, b.Data, b.FileName, b.MimeType);
								}
						}
								
						var headers = new Dictionary<string, string> ();
						foreach (KeyValuePair<string, string> entry in newForm.headers) {
								headers.Add (entry.Key, entry.Value);
						}

						if (cookie != null)
								headers.Add ("Cookie", cookie);

						var newCall = new WWW (URI, newForm.data, headers);

						yield return newCall;

						while (!newCall.isDone)
								yield return new WaitForSeconds (0.01f);

						// Callback!
						if (callback != null) {
								if (newCall.responseHeaders.ContainsKey ("STATUS") && newCall.responseHeaders ["STATUS"].Contains (" 200 "))
										callback (_hash, new Dictionary<string,string> (newCall.responseHeaders), newCall.text);
								else
										callback (_hash, new Dictionary<string,string> (newCall.responseHeaders), "");
						}

						_busy = false;

						ParentPool.Despawn (gameObject);
				}

				/// <summary>
				/// IEnumeratable HTTP GET Request to URI.
				/// </summary>
				/// <returns>Enumeratable Function</returns>
				/// <param name="URI">The Target URI.</param>
				/// <param name="cookie">Any previous cookie data to be used for authentication.</param>
				/// <param name="callback">A callback function (int hash, Hashtable headers, string payload).</param>
				IEnumerator GetReturnedText (string URI, string cookie, System.Action<int, Dictionary<string,string>, string> callback)
				{
						// Assign Busy Flag
						_busy = true;

						// Process Headers
						var headers = new Dictionary<string,string> ();
						if (cookie != null)
								headers.Add ("Cookie", cookie);

						// Make the call
						var newCall = new WWW (URI, null, headers);

						yield return newCall;

						while (!newCall.isDone)
								yield return new WaitForSeconds (0.01f);

						// Callback! (Avoid Unity Bitching)
						if (callback != null) {
								if (newCall.responseHeaders.ContainsKey ("STATUS") && newCall.responseHeaders ["STATUS"].Contains (" 200 "))
										callback (_hash, new Dictionary<string,string> (newCall.responseHeaders), newCall.text);
								else
										callback (_hash, new Dictionary<string,string> (newCall.responseHeaders), "");
						}

						_busy = false;

						ParentPool.Despawn (gameObject);
				}

				/// <summary>
				/// IEnumeratable HTTP POST to URI.
				/// </summary>
				/// <returns>Enumeratable Function</returns>
				/// <param name="URI">The Target URI.</param>
				/// <param name="contentType">The Content-Type Header</param>
				/// <param name="payload">The data to be posted.</param>
				/// <param name="cookie">Any previous cookie data to be used for authentication.</param>
				/// <param name="callback">A callback function (int hash, Hashtable headers, string payload).</param>
				IEnumerator PostReturnedText (string URI, string contentType, string payload, string cookie, System.Action<int, Dictionary<string,string>, string> callback)
				{
						// Assign Busy Flag
						_busy = true;

						// Message Data
						byte[] postData = System.Text.Encoding.ASCII.GetBytes (payload.ToCharArray ());

						// Process Headers
						var headers = new Dictionary<string,string> ();

						//headers.Add ("Content-Type", contentType);
						//headers.Add ("Content-Length", postData.Length);
						//if (cookie != null)
						//		headers.Add ("Cookie", cookie);

						var newCall = new WWW (URI, postData, headers);

						yield return newCall;

						while (!newCall.isDone)
								yield return new WaitForSeconds (0.01f);

						if (callback != null) {
								if (newCall.responseHeaders ["STATUS"].Contains (" 200 "))
										callback (_hash, new Dictionary<string,string> (newCall.responseHeaders), newCall.text);
								else
										callback (_hash, new Dictionary<string,string> (newCall.responseHeaders), "");
						}

						_busy = false;

						ParentPool.Despawn (gameObject);
				}
		}
}