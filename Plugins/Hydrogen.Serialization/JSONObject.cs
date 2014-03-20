// ------------------------------------------------------------------------------
#region Copyright Notice & License Information
//
// JSONObject.cs
//
// Author:
//       Patrick van Bergen
//       Matthew Davey <matthew.davey@dotbunny.com>
//       Calvin Rien
//       WyrmTale Games <info@wyrmtale.com>
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hydrogen.Serialization
{
		public class JSONObject
		{
				public Dictionary<string, object> Fields = new Dictionary<string, object> ();

				public JSONObject ()
				{
				}

				/// <summary>
				/// Create a JSONObject and prepopulate it with the values from a Dictionary
				/// </summary>
				/// <param name="prepopulate">A Dictionary<string,string> of values</param>
				public JSONObject (Dictionary<string, string> prepopulate)
				{
						foreach (string s in prepopulate.Keys) {
								Fields.Add (s, prepopulate [s]);
						}
				}

				/// <summary>
				/// Create a JSONObject from a JSON payload
				/// </summary>
				/// <param name="json">The JSON payload</param>
				public JSONObject (string json)
				{
						Fields = JSON.Deserialize (json).Fields;
				}
				// Indexer to provide read/write access to the fields
				public object this [string fieldName] {
						// Read one byte at offset index and return it.
						get {
								return Fields.ContainsKey (fieldName) ? (Fields [fieldName]) : null;
						}
						// Write one byte at offset index and return it.
						set {
								if (Fields.ContainsKey (fieldName))
										Fields [fieldName] = value;
								else
										Fields.Add (fieldName, value);
						}
				}

				public string ToString (string fieldName)
				{
						return Fields.ContainsKey (fieldName) ? Convert.ToString (Fields [fieldName]) : "";
				}

				public int ToInt (string fieldName)
				{
						return Fields.ContainsKey (fieldName) ? Convert.ToInt32 (Fields [fieldName]) : 0;
				}

				public float ToFloat (string fieldName)
				{
						return Fields.ContainsKey (fieldName) ? Convert.ToSingle (Fields [fieldName]) : 0.0f;
				}

				public bool ToBoolean (string fieldName)
				{
						return Fields.ContainsKey (fieldName) && Convert.ToBoolean (Fields [fieldName]);
				}

				public string Serialized {
						get {
								return JSON.Serialize (this);
						}
						set {
								JSONObject o = JSON.Deserialize (value);
								if (o != null)
										Fields = o.Fields;
						}
				}

				public JSONObject ToJSON (string fieldName)
				{
						if (!Fields.ContainsKey (fieldName))
								Fields.Add (fieldName, new JSONObject ());
			
						return (JSONObject)this [fieldName];
				}
				// to serialize/deserialize a Vector2
				public static implicit operator Vector2 (JSONObject value)
				{
						return new Vector3 (
								Convert.ToSingle (value ["x"]),
								Convert.ToSingle (value ["y"]));
				}

				public static explicit operator JSONObject (Vector2 value)
				{
						checked {
								var o = new JSONObject ();
								o ["x"] = value.x;
								o ["y"] = value.y;
								return o;
						}
			
				}
				// to serialize/deserialize a Vector3
				public static implicit operator Vector3 (JSONObject value)
				{
						return new Vector3 (
								Convert.ToSingle (value ["x"]),
								Convert.ToSingle (value ["y"]),
								Convert.ToSingle (value ["z"]));
				}

				public static explicit operator JSONObject (Vector3 value)
				{
						checked {
								var o = new JSONObject ();
								o ["x"] = value.x;
								o ["y"] = value.y;
								o ["z"] = value.z;
								return o;
						}
				}
				// to serialize/deserialize a Quaternion
				public static implicit operator Quaternion (JSONObject value)
				{
						return new Quaternion (
								Convert.ToSingle (value ["x"]),
								Convert.ToSingle (value ["y"]),
								Convert.ToSingle (value ["z"]),
								Convert.ToSingle (value ["w"])
						);
				}

				public static explicit operator JSONObject (Quaternion value)
				{
						checked {
								var o = new JSONObject ();
								o ["x"] = value.x;
								o ["y"] = value.y;
								o ["z"] = value.z;
								o ["w"] = value.w;
								return o;
						}
				}
				// to serialize/deserialize a Color
				public static implicit operator Color (JSONObject value)
				{
						return new Color (
								Convert.ToSingle (value ["r"]),
								Convert.ToSingle (value ["g"]),
								Convert.ToSingle (value ["b"]),
								Convert.ToSingle (value ["a"])
						);
				}

				public static explicit operator JSONObject (Color value)
				{
						checked {
								var o = new JSONObject ();
								o ["r"] = value.r;
								o ["g"] = value.g;
								o ["b"] = value.b;
								o ["a"] = value.a;
								return o;
						}
				}
				// to serialize/deserialize a Color32
				public static implicit operator Color32 (JSONObject value)
				{
						return new Color32 (
								Convert.ToByte (value ["r"]),
								Convert.ToByte (value ["g"]),
								Convert.ToByte (value ["b"]),
								Convert.ToByte (value ["a"])
						);
				}

				public static explicit operator JSONObject (Color32 value)
				{
						checked {
								var o = new JSONObject ();
								o ["r"] = value.r;
								o ["g"] = value.g;
								o ["b"] = value.b;
								o ["a"] = value.a;
								return o;
						}
				}
				// to serialize/deserialize a Rect
				public static implicit operator Rect (JSONObject value)
				{
						return new Rect (
								Convert.ToByte (value ["left"]),
								Convert.ToByte (value ["top"]),
								Convert.ToByte (value ["width"]),
								Convert.ToByte (value ["height"])
						);
				}

				public static explicit operator JSONObject (Rect value)
				{
						checked {
								var o = new JSONObject ();
								o ["left"] = value.xMin;
								o ["top"] = value.yMax;
								o ["width"] = value.width;
								o ["height"] = value.height;
								return o;
						}
				}
				// get typed array out of the object as object[]
				public T[] ToArray<T> (string fieldName)
				{
						if (Fields.ContainsKey (fieldName)) {				
								if (Fields [fieldName] is IEnumerable) {
										List<T> l = new List<T> ();
										foreach (object o in (Fields[fieldName] as IEnumerable)) {
												if (l is List<string>)
														(l as List<string>).Add (Convert.ToString (o));
												else if (l is List<int>)
														(l as List<int>).Add (Convert.ToInt32 (o));
												else if (l is List<float>)
														(l as List<float>).Add (Convert.ToSingle (o));
												else if (l is List<bool>)
														(l as List<bool>).Add (Convert.ToBoolean (o));
												else if (l is List<Vector2>)
														(l as List<Vector2>).Add ((Vector2)((JSONObject)o));
												else if (l is List<Vector3>)
														(l as List<Vector3>).Add ((Vector3)((JSONObject)o));
												else if (l is List<Rect>)
														(l as List<Rect>).Add ((Rect)((JSONObject)o));
												else if (l is List<Color>)
														(l as List<Color>).Add ((Color)((JSONObject)o));
												else if (l is List<Color32>)
														(l as List<Color32>).Add ((Color32)((JSONObject)o));
												else if (l is List<Quaternion>)
														(l as List<Quaternion>).Add ((Quaternion)((JSONObject)o));
												else if (l is List<JSONObject>)
														(l as List<JSONObject>).Add ((JSONObject)o);
										}
										return l.ToArray ();
								}
						}
						return new T[]{ };
				}
		}
}
	