#region Copyright Notice & License Information
//
// LibraryDocumentation.cs
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
using UnityEditor;

public static class LibraryDocumentation
{
		[MenuItem ("Help/Hydrogen Documentation", false, 200)]
		static void HelpForHydrogenComponents ()
		{
				UnityEngine.Application.OpenURL ("http://hydrogen.dotbunny.com/getting-started/");

		}
		//[MenuItem ("Help/Library Documentation/Install for MonoDevelop", true)]
		public static bool InstallLibraryDocumentationMonoDevelopCheck ()
		{
#if UNITY_EDITOR_OSX
				// Restrict our search area for performance reasons
				string hydrogenPath = HydrogenUtility.GetHydrogenPath ();
		
				if (Directory.GetFiles (hydrogenPath, "Unity.source", SearchOption.AllDirectories).Length == 0)
						return false;
				if (Directory.GetFiles (hydrogenPath, "Unity.tree", SearchOption.AllDirectories).Length == 0)
						return false;
				if (Directory.GetFiles (hydrogenPath, "Unity.zip", SearchOption.AllDirectories).Length == 0)
						return false;
	
				return true;
#else
				return false;
#endif
		}
		//[MenuItem ("Help/Library Documentation/Install for MonoDevelop")]
		static void InstallLibraryDocumentationMonoDevelop ()
		{
#if UNITY_EDITOR_OSX
				EditorUtility.DisplayProgressBar ("Installing", "Initializing ...", 0.00f);
				string _baseLocation = HydrogenUtility.GetHydrogenPath () + HydrogenUtility.DS + "Extras" + HydrogenUtility.DS + "MonoDevelop";



				//Library/Frameworks/Mono.framework/Versions/Current/lib/monodoc/sources
				string _unityBase = EditorApplication.applicationPath.Replace ("Unity.app", "");

				// Quick Way of Finding
				EditorUtility.DisplayProgressBar ("Installing", "Getting Path Information  ...", 0.20f);
				string _monoBase = Directory.GetFiles (_unityBase, "Mono.source", SearchOption.AllDirectories) [0].Replace ("/Mono.source", "");

				EditorUtility.DisplayProgressBar ("Installing", "Copy Unity.source ... ", 0.50f);

				File.Copy (_baseLocation + HydrogenUtility.DS + "Unity.source", 
						_monoBase + HydrogenUtility.DS + "Unity.source",
						true);

				EditorUtility.DisplayProgressBar ("Installing", "Copy Unity.source ... ", 0.70f);

				File.Copy (_baseLocation + HydrogenUtility.DS + "Unity.tree", 
						_monoBase + HydrogenUtility.DS + "Unity.tree",
						true);

				EditorUtility.DisplayProgressBar ("Installing", "Copy Unity.zip ... ", 0.80f);

				File.Copy (_baseLocation + HydrogenUtility.DS + "Unity.zip", 
						_monoBase + HydrogenUtility.DS + "Unity.zip",
						true);

				EditorUtility.DisplayProgressBar ("Completed", "Finished Installing MonoDevelop Documentation.", 1f);
		
				EditorUtility.ClearProgressBar ();
#endif
		}
}