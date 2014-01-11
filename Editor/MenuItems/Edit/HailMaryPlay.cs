//
// HailMaryPlay.cs
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

using UnityEditor;

public static class HailMaryPlay
{
		public static string Injection = "UnityEngine.Debug.Log(\".:Hail Mary:.\");";

		[MenuItem ("Edit/The \"Hail Mary\" Play/Enable Project Wide")]
		public static void EnableHailMary ()
		{
				string[] files = System.IO.Directory.GetFiles (".", "*.cs", System.IO.SearchOption.AllDirectories);
				foreach (string filePath in files) {
						EnableHailMaryOnFile (filePath);
				}
				AssetDatabase.Refresh ();
		}

		[MenuItem ("Edit/The \"Hail Mary\" Play/Disable Project Wide")]
		public static void DisableHailMary ()
		{
				string[] files = System.IO.Directory.GetFiles (".", "*.cs", System.IO.SearchOption.AllDirectories);
				foreach (string filePath in files) {
						DisableHailMaryOnFile (filePath);
				}
				AssetDatabase.Refresh ();
		}

		[MenuItem ("Edit/The \"Hail Mary\" Play/Enable On Selected", true)]
		public static bool EnableHailMaryOnSelectedCheck ()
		{
				return Selection.instanceIDs.Length != 0;
		}

		[MenuItem ("Edit/The \"Hail Mary\" Play/Enable On Selected")]
		public static void EnableHailMaryOnSelected ()
		{
				foreach (int id in Selection.instanceIDs) {
						string filePath = AssetDatabase.GetAssetPath (id);
						if (filePath.EndsWith (".cs", System.StringComparison.Ordinal)) {
								EnableHailMaryOnFile (filePath);
						}
				}
				AssetDatabase.Refresh ();
		}

		[MenuItem ("Edit/The \"Hail Mary\" Play/Disable On Selected", true)]
		public static bool DisableHailMaryOnSelectedCheck ()
		{
				return Selection.instanceIDs.Length != 0;
		}

		[MenuItem ("Edit/The \"Hail Mary\" Play/Disable On Selected")]
		public static void DisableHailMaryOnSelected ()
		{
				foreach (int id in Selection.instanceIDs) {
						string filePath = AssetDatabase.GetAssetPath (id);
						if (filePath.EndsWith (".cs", System.StringComparison.Ordinal)) {
								DisableHailMaryOnFile (filePath);
						}
				}
				AssetDatabase.Refresh ();
		}

		public static void EnableHailMaryOnFile (string filePath)
		{
				if (filePath.EndsWith ("HailMary.cs", System.StringComparison.Ordinal))
						return;
		
				string[] lines = System.IO.File.ReadAllLines (filePath);
		
				bool writeFile = false;
		
				for (int x = 0; x < lines.Length; x++) {
						var test = System.Text.RegularExpressions.Regex.Matches (lines [x], @"([a-zA-Z0-9]*)\s*\([^()]*\)");
			
						if (test.Count > 0
						    && (lines [x].Contains ("{") || lines [x + 1].Contains ("{"))
						    && !(lines [x].Contains ("{") && lines [x + 1].Contains ("{"))
						    && !lines [x].Contains ("CValue")
						    && !lines [x].Contains ("new")
						    && !lines [x].TrimStart ().StartsWith ("if", System.StringComparison.Ordinal)
						    && !lines [x].TrimStart ().StartsWith ("elseif", System.StringComparison.Ordinal)
						    && !lines [x].TrimStart ().StartsWith ("else", System.StringComparison.Ordinal)
						    && !lines [x].TrimStart ().StartsWith ("foreach", System.StringComparison.Ordinal)
						    && !lines [x].TrimStart ().StartsWith ("for", System.StringComparison.Ordinal)
						    && !lines [x].TrimStart ().StartsWith ("switch", System.StringComparison.Ordinal)
						    && !lines [x].TrimStart ().StartsWith ("var", System.StringComparison.Ordinal)
						    && !lines [x].TrimStart ().StartsWith ("while", System.StringComparison.Ordinal)
						    && !lines [x].TrimStart ().StartsWith ("catch", System.StringComparison.Ordinal)
						    && !lines [x].TrimStart ().StartsWith ("throw", System.StringComparison.Ordinal)
						    && !lines [x].TrimStart ().StartsWith ("get", System.StringComparison.Ordinal)
						    && !lines [x].TrimStart ().StartsWith ("set", System.StringComparison.Ordinal)
						    && !lines [x].TrimStart ().StartsWith ("//", System.StringComparison.Ordinal)
						    && !lines [x].Contains ("Debug.Log")) {
								if (lines [x].Contains ("{")) {
										Hydrogen.Array.AddAt (ref lines, x + 1, Injection, false);
								} else {
										Hydrogen.Array.AddAt (ref lines, x + 2, Injection, false);
								}
								writeFile = true;
						}

				}
		
				if (writeFile) {
						System.IO.File.WriteAllLines (filePath, lines);
				}
		}

		public static void DisableHailMaryOnFile (string filePath)
		{
				if (filePath.EndsWith ("HailMary.cs", System.StringComparison.Ordinal))
						return;

				string[] lines = System.IO.File.ReadAllLines (filePath);
		
				bool writeFile = false;
		
				for (int x = 0; x < lines.Length; x++) {
						if (lines [x].StartsWith (Injection, System.StringComparison.Ordinal)) {
								Hydrogen.Array.RemoveAt (ref lines, x);
								writeFile = true;
						}
				}

				if (writeFile) {
						System.IO.File.WriteAllLines (filePath, lines);
				}
		}
}