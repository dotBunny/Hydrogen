#region Copyright Notice & License Information
//
// AutomaticBuild.cs
//
// Author:
//       Josh Montoute
//       Matthew Davey <matthew.davey@dotbunny.com>
//
// Copyright (c) 2011 by Thinksquirrel Software, LLC
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
using UnityEditor;
using UnityEngine;

public static class AutomaticBuild
{
		static string GetProjectName ()
		{
				string[] s = Application.dataPath.Split ('/');
				return s [s.Length - 2];
		}

		static string[] GetScenePaths ()
		{
				var EditorScenes = new List<string> ();
				foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
						if (!scene.enabled)
								continue;
						EditorScenes.Add (scene.path);
				}
				return EditorScenes.ToArray ();
		}

		[MenuItem ("File/Automatic Build/Windows (32 bit)")]
		static void PerformWin32Build ()
		{
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneWindows);
				BuildPipeline.BuildPlayer (GetScenePaths (), "Builds/Win/" + GetProjectName () + ".exe", BuildTarget.StandaloneWindows, BuildOptions.None);
		}

		[MenuItem ("File/Automatic Build/Windows (64 bit)")]
		static void PerformWin64Build ()
		{
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneWindows);
				BuildPipeline.BuildPlayer (GetScenePaths (), "Builds/Win64/" + GetProjectName () + ".exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
		}

		[MenuItem ("File/Automatic Build/OSX (32 bit)")]
		static void PerformOSXIntelBuild ()
		{
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneOSXIntel);
				BuildPipeline.BuildPlayer (GetScenePaths (), "Builds/OSX-Intel/" + GetProjectName () + ".app", BuildTarget.StandaloneOSXIntel, BuildOptions.None);
		}

		[MenuItem ("File/Automatic Build/OSX (64 bit)")]
		static void PerformOSXIntel64Build ()
		{
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneOSXIntel64);
				BuildPipeline.BuildPlayer (GetScenePaths (), "Builds/OSX-Intel-64/" + GetProjectName () + ".app", BuildTarget.StandaloneOSXIntel64, BuildOptions.None);
		}

		[MenuItem ("File/Automatic Build/iOS")]
		static void PerformiOSBuild ()
		{
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.iPhone);
				BuildPipeline.BuildPlayer (GetScenePaths (), "Builds/iOS", BuildTarget.iPhone, BuildOptions.None);
		}

		[MenuItem ("File/Automatic Build/Android")]
		static void PerformAndroidBuild ()
		{
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.Android);
				BuildPipeline.BuildPlayer (GetScenePaths (), "Builds/Android", BuildTarget.Android, BuildOptions.None);
		}

		[MenuItem ("File/Automatic Build/Web Player")]
		static void PerformWebBuild ()
		{
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.WebPlayer);
				BuildPipeline.BuildPlayer (GetScenePaths (), "Builds/Web", BuildTarget.WebPlayer, BuildOptions.None);
		}

		[MenuItem ("File/Automatic Build/Web Player (Streamed)")]
		static void PerformWebStreamedBuild ()
		{
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.WebPlayerStreamed);
				BuildPipeline.BuildPlayer (GetScenePaths (), "Builds/Web-Streamed", BuildTarget.WebPlayerStreamed, BuildOptions.None);
		}
}