#region Copyright Notice & License Information
//
// WebPool.cs
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

namespace Hydrogen.Core
{
		/// <summary>
		/// An internal web pool system used within the Hydrogen Framework. 
		/// </summary>
		[AddComponentMenu ("")]
		public class WebPool : MonoBehaviour
		{
				/// <summary>
				/// Has the WebPool initialized and is ready for use?
				/// </summary>
				bool _initialized;
				/// <summary>
				/// What is the ID of the ObjectPool being used for the WebPool
				/// </summary>
				int _poolID;
				/// <summary>
				/// Local reference to the ObjectPool in use.
				/// </summary>
				/// <remarks>>
				/// System will detect the existence of another ObjectPool based system and use it.
				/// </remarks>
				ObjectPool _poolReference;

				/// <summary>
				/// HTTP POST Form to URI.
				/// </summary>
				/// <param name="URI">The Target URI</param>
				/// <param name="formStringData">A Dictionary<string,string> of Form Data</param>
				/// <returns>Call Hashcode</returns>
				public int Form (string URI, Dictionary<string, string> formStringData)
				{
						return Form (URI, formStringData, null, null, null);
				}

				/// <summary>
				/// HTTP POST Form to URI.
				/// </summary>
				/// <param name="URI">The Target URI</param>
				/// <param name="formStringData">A Dictionary<string,string> of Form Data</param>
				/// <param name="formBinaryData">A custom binary dataset. Useful for uploading pictures.</param>
				/// <param name="cookie">Any previous cookie data to be used for authentication.</param>
				/// <param name="callback">A callback function (int hash, Hashtable headers, string payload).</param>
				/// <returns>Call Hashcode</returns>
				public int Form (string URI, Dictionary<string, string> formStringData, FormBinaryData[] formBinaryData, string cookie, System.Action<int, Dictionary<string,string>, string> callback)
				{
						if (!_initialized) {
								Debug.LogError ("WebPool has not finished initializing ... " +
								"Did you call this function without having either a WebPool or ObjectPool component " +
								"already on a MonoBehaviour?");
								return 0;
						}
						GameObject go = _poolReference.Spawn (_poolID);
						return go.GetComponent<WebPoolWorker> ().Form (URI, formStringData, formBinaryData, cookie, callback);
				}

				/// <summary>
				/// HTTP GET Request to URI
				/// </summary>
				/// <param name="URI">The Target URI</param>*/
				/// <returns>Call Hashcode</returns>
				public int GET (string URI)
				{
						return GET (URI, null, null);
				}

				/// <summary>
				/// HTTP GET Request to URI
				/// </summary>
				/// <param name="URI">The Target URI</param>
				/// <param name="callback">A callback function (int hash, Hashtable headers, string payload).</param>
				/// <returns>Call Hashcode</returns>
				public int GET (string URI, System.Action<int, Dictionary<string,string>, string> callback)
				{
						return GET (URI, null, callback);
				}

				/// <summary>
				/// HTTP GET Request to URI
				/// </summary>
				/// <param name="URI">The Target URI</param>
				/// <param name="cookie">Any previous cookie data to be used for authentication.</param>
				/// <param name="callback">A callback function (int hash, Hashtable headers, string payload).</param>
				/// <returns>Call Hashcode</returns>
				public int GET (string URI, string cookie, System.Action<int, Dictionary<string,string>, string> callback)
				{
						if (!_initialized) {
								Debug.LogError ("WebPool has not finished initializing ... " +
								"Did you call this function without having either a WebPool or ObjectPool component " +
								"already on a MonoBehaviour?");
								return 0;
						}
						GameObject go = _poolReference.Spawn (_poolID);
						return go.GetComponent<WebPoolWorker> ().GET (URI, cookie, callback);
				}

				/// <summary>
				/// HTTP POST Request to URI.
				/// </summary>
				/// <param name="URI">The Target URI</param>
				/// <param name="contentType">The Content-Type Header</param>
				/// <param name="payload">The data to be posted.</param>
				/// <returns>Call Hashcode</returns>
				public int POST (string URI, string contentType, string payload)
				{
						return POST (URI, contentType, payload, null, null);
				}

				/// <summary>
				/// HTTP POST Request to URI.
				/// </summary>
				/// <param name="URI">The Target URI</param>
				/// <param name="contentType">The Content-Type Header</param>
				/// <param name="payload">The data to be posted.</param>
				/// <param name="cookie">Any previous cookie data to be used for authentication.</param>
				/// <param name="callback">A callback function (int hash, Hashtable headers, string payload).</param>
				/// <returns>Call Hashcode</returns> 
				public int POST (string URI, string contentType, string payload, string cookie, System.Action<int, Dictionary<string,string>, string> callback)
				{
						if (!_initialized) {
								UnityEngine.Debug.LogError ("WebPool has not finished initializing ... " +
								"Did you call this function without having either a WebPool or ObjectPool component " +
								"already on a MonoBehaviour?");
								return 0;
						}
						GameObject go = _poolReference.Spawn (_poolID);
						return go.GetComponent<WebPoolWorker> ().POST (URI, contentType, payload, cookie, callback);
				}

				/// <summary>
				/// Unity's Awake Event
				/// </summary>
				protected virtual void Awake ()
				{
						StartCoroutine (Initialize ());
				}

				/// <summary>
				/// Initialization process for a WebPool.
				/// </summary>
				IEnumerator Initialize ()
				{
						if (!_initialized) {

								// Create our buddy object
								var newWebObject = new GameObject ();
								newWebObject.AddComponent (typeof(WebPoolWorker));
								newWebObject.name = "Web Call";

								// Search out any existing ObjectPool
								_poolReference = (ObjectPool)FindObjectOfType (typeof(ObjectPool));

								// If we don't have an existing reference in the scene for an ObjectPool, we need to make one.
								if (_poolReference == null) {
										// Create a new ObjectPool using our default singleton.
										_poolReference = hObjectPool.Instance;

										// Wait for end of frame so that the new Object Pool can initialize.
										yield return new WaitForEndOfFrame ();
								}

								// Add the new object to the Object Pool
								_poolID = _poolReference.Add (newWebObject);

								// We need to keep this GameObject around as it is referenced for spawning.
								newWebObject.transform.parent = _poolReference.gameObject.transform;
								newWebObject.gameObject.SetActive (false);
								_initialized = true;
						}
				}

				/// <summary>
				/// Form Data Structure
				/// </summary>
				public struct FormBinaryData
				{
						public string FieldName;
						public byte[] Data;
						public string FileName;
						public string MimeType;
				}
		}
}