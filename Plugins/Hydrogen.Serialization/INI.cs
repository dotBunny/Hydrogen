#region Copyright Notice & License Information
//
// INI.cs
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

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hydrogen.Serialization
{
		public static class INI
		{
				/// <summary>
				/// Deserialize the specified iniString and seperatorCharacter.
				/// </summary>
				/// <param name="iniString">Ini string.</param>
				/// <param name="seperatorCharacter">Seperator character.</param>
				public static List<KeyValuePair<string, string>> Deserialize (string iniString, char seperatorCharacter = '=')
				{
						var entries = new List<KeyValuePair<string, string>> ();

						using (var reader = new StringReader (iniString)) {
								string line;
								while ((line = reader.ReadLine ()) != null) {
										line = line.Trim ();
										if (line.Length == 0)
												continue;

										if (!line.Contains (seperatorCharacter.ToString ()))
												continue;

										int first = line.IndexOf (seperatorCharacter);

										entries.Add (new KeyValuePair<string, string> (
												line.Substring (0, first).Trim (), line.Substring (first + 1).Trim ()));
								}
						}
						return entries;
				}

				/// <summary>
				/// Serialize the specified data and seperatorCharacter.
				/// </summary>
				/// <param name="data">Data.</param>
				/// <param name="seperatorCharacter">Seperator character.</param>
				public static string Serialize (Dictionary<string, string> data, char seperatorCharacter = '=')
				{
						var iniString = new StringBuilder ();

						foreach (string s in data.Keys) {
								iniString.AppendLine (s.Trim () + seperatorCharacter + data [s].Trim ());
						}

						return iniString.ToString ();
				}

				/// <summary>
				/// Serialize the specified data and seperatorCharacter.
				/// </summary>
				/// <param name="data">Data.</param>
				/// <param name="seperatorCharacter">Seperator character.</param>
				public static string Serialize (List<KeyValuePair<string, string>> data, char seperatorCharacter = '=')
				{
						var iniString = new StringBuilder ();

						for (int x = 0; x < data.Count; x++) {
								iniString.AppendLine (data [x].Key.Trim () + seperatorCharacter + data [x].Value.Trim ());
						}

						return iniString.ToString ();
				}
		}
}