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
		public class WebPoolWorker : ObjectPoolItemBase
		{
				int _hash;

				public override void OnSpawned ()
				{
						gameObject.SetActive (true);
						_hash = 0;
				}

				public override void OnDespawned ()
				{
						StopAllCoroutines ();
						_hash = 0;
						gameObject.SetActive (false);
				}

				public override void DespawnSafely ()
				{

				}

				public override bool IsInactive ()
				{
						return false;
				}

				#region GET

				public int GET (string URI, string cookie, System.Action<int, Hashtable, string> callback)
				{
						_hash = (Time.time + URI + Random.Range (0, 100)).GetHashCode ();

						StartCoroutine (GetReturnedText (URI, cookie, callback));

						return _hash;
				}

				public IEnumerator GetReturnedText (string URI, string cookie, System.Action<int, Hashtable, string> callback)
				{
						// Process Headers
						var headers = new Hashtable ();
						if (cookie != null)
								headers.Add ("Cookie", cookie);
			
						// Make the call
						var newCall = new WWW (URI, null, headers);
			
						yield return newCall;
			
						while (!newCall.isDone)
								yield return new WaitForSeconds (0.01f);

						// Callback! (Avoid Unity Bitching)
						if (callback != null) {
								if (newCall.responseHeaders ["STATUS"].Contains (" 200 "))
										callback (_hash, new Hashtable (newCall.responseHeaders), newCall.text);
								else
										callback (_hash, new Hashtable (newCall.responseHeaders), "");
						}
						ParentPool.Despawn (gameObject);
				}

				#endregion

				#region POST

				public int POST (string URI, string contentType, string payload, string cookie, System.Action<int, Hashtable, string> callback)
				{
						_hash = (Time.time + URI + payload + Random.Range (0, 100)).GetHashCode ();
			
						StartCoroutine (PostReturnedText (URI, contentType, payload, cookie, callback));
			
						return _hash;
				}

				public IEnumerator PostReturnedText (string URI, string contentType, string payload, string cookie, System.Action<int, Hashtable, string> callback)
				{
						// Message Data
						byte[] postData = System.Text.Encoding.ASCII.GetBytes (payload.ToCharArray ());
			
						// Process Headers
						var headers = new Hashtable ();
			
						headers.Add ("Content-Type", contentType);
						headers.Add ("Content-Length", postData.Length);
						if (cookie != null)
								headers.Add ("Cookie", cookie);
			
						var newCall = new WWW (URI, postData, headers);

						yield return newCall;
			
						while (!newCall.isDone)
								yield return new WaitForSeconds (0.01f);

						if (callback != null) {
								if (newCall.responseHeaders ["STATUS"].Contains (" 200 "))
										callback (_hash, new Hashtable (newCall.responseHeaders), newCall.text);
								else
										callback (_hash, new Hashtable (newCall.responseHeaders), "");
						}
						ParentPool.Despawn (gameObject);
				}

				#endregion

				#region FORM

				public int Form (string URI, Dictionary<string,string> formStringData, WebPool.FormBinaryData[] formBinaryData, string cookie, System.Action<int, Hashtable, string> callback)
				{
						_hash = (Time.time + URI + formStringData.GetHashCode () + Random.Range (0, 100)).GetHashCode ();
			
						StartCoroutine (FormReturnedText (URI, formStringData, formBinaryData, cookie, callback));
			
						return _hash;
				}

				public IEnumerator FormReturnedText (string URI, Dictionary<string, string> formStringData, WebPool.FormBinaryData[] formBinaryData, string cookie, System.Action<int, Hashtable, string> callback)
				{
						var newForm = new WWWForm ();

						// Add string data
						foreach (string s in formStringData.Keys) {
								newForm.AddField (s, formStringData [s]);
						}

						// Add binary data
						foreach (WebPool.FormBinaryData b in formBinaryData) {
								newForm.AddBinaryData (b.FieldName, b.Data, b.FileName, b.MimeType);
						}

						var headers = newForm.headers;

						if (cookie != null)
								headers.Add ("Cookie", cookie);
			
						var newCall = new WWW (URI, newForm.data, headers);
			
						yield return newCall;
			
						while (!newCall.isDone)
								yield return new WaitForSeconds (0.01f);
			
						// Callback!
						if (callback != null) {
								if (newCall.responseHeaders ["STATUS"].Contains (" 200 "))
										callback (_hash, new Hashtable (newCall.responseHeaders), newCall.text);
								else
										callback (_hash, new Hashtable (newCall.responseHeaders), "");
						}
						ParentPool.Despawn (gameObject);
				}

				#endregion

		}
}