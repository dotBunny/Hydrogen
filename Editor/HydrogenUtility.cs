#region Copyright Notice & License Information
//
// HydrogenUtility.cs
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

using System.IO;
using UnityEngine;

public static class HydrogenUtility
{
		/// <summary>
		/// A quick reference to the Directory Seperator
		/// </summary>
		public static char DS = Path.DirectorySeparatorChar;

		/// <summary>
		/// Gets the path of the Hydrogen Package
		/// </summary>
		/// <returns>The absolute path to Hydrogen</returns>
		public static string GetHydrogenPath ()
		{
				string[] paths = Directory.GetFiles (Application.dataPath, "HydrogenUtility.cs", SearchOption.AllDirectories);

				if (paths.Length > 1) { 
						Debug.LogError (
								"Found multiple identifiers, unable to proceed." +
								"We search for the 'HydrogenUtility.cs' file to determine the base location to be " +
								"used by editor scripts. ");
						return null;
				}

				if (paths.Length == 0) {
						Debug.LogError (
								"Unable to determine the path of Hydrogen. " +
								"We search for the 'HydrogenUtility.cs' file to determine the base location to be " +
								"used by editor scripts. ");
						return null;
				}

				return paths [0].Replace ("Editor" + DS + "HydrogenUtility.cs", "");
		}
}
