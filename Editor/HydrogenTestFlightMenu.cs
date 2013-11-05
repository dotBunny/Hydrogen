#region Copyright Notice & License Information
// 
// HydrogenTestFlightMenu.cs
//  
// Author(s):
//   Matthew Davey <matthew.davey@dotbunny.com>
//
// Copyright (C) 2013 dotBunny Inc. (http://www.dotbunny.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;


public class HydrogenTestFlightMenu  {
	
	public static char DS = System.IO.Path.DirectorySeparatorChar;
	
	[MenuItem("Hydrogen/Plugins/Install TestFlight for iOS")]
	public static void InstallTestFlightForIOS()
	{
		EditorUtility.DisplayProgressBar("Installing", "Getting Path Information  ...", 0.00f);
		
		// Find the base location for the iOS TestFlight files
		string[] _locations = System.IO.Directory.GetFiles(Application.dataPath, "libTestFlight.a", System.IO.SearchOption.AllDirectories);
		string _baseLocation = "";
		foreach(string location in _locations)
		{
			if ( location.Contains("Hydrogen") && location.Contains("Vendors") && location.Contains("TestFlight") )
			{
				_baseLocation = location.Replace("libTestFlight.a", "");;
				break;
			}
		}		
		
		// Find the location of our extras that we want to add into the mix
		string[] _extras = System.IO.Directory.GetFiles(Application.dataPath, "HydrogenTestFlight.mm", System.IO.SearchOption.AllDirectories);
		string _extraLocation = "";
		foreach(string extra in _extras)
		{
			if ( extra.Contains("Hydrogen") && extra.Contains("Extras") && extra.Contains("TestFlight") )
			{
				_extraLocation = extra.Replace("HydrogenTestFlight.mm", "");;
				break;
			}
		}
		
		EditorUtility.DisplayProgressBar("Installing", "Installing iOS Files ...", 0.1f);
		
		if (_baseLocation != "" && _extraLocation != "" )
		{
			// Create our output directories (Cool Chain Effect! +10 Fun)
			Directory.CreateDirectory(Application.dataPath + DS + "Plugins" + DS + "iOS");
			
			EditorUtility.DisplayProgressBar("Installing", "Copying iOS Files ...", 0.2f);
			
			// Copy over the main TestFlight iOS system
			File.Copy(_baseLocation + DS + "libTestFlight.a", 
				Application.dataPath + DS + "Plugins" + DS + "iOS" + DS + "libTestFlight.a", 
				true);
			
			EditorUtility.DisplayProgressBar("Installing", "Copying iOS Files ...", 0.3f);
			
			File.Copy(_baseLocation + DS + "TestFlight.h", 
				Application.dataPath + DS + "Plugins" + DS + "iOS" + DS + "TestFlight.h", 
				true);
			EditorUtility.DisplayProgressBar("Installing", "Copying iOS Files ...", 0.4f);
			
			File.Copy(_baseLocation + DS + "TestFlight+AsyncLogging.h", 
				Application.dataPath + DS + "Plugins" + DS + "iOS" + DS + "TestFlight+AsyncLogging.h", 
				true);
			
			EditorUtility.DisplayProgressBar("Installing", "Copying iOS Files ...", 0.5f);
			
			File.Copy(_baseLocation + DS + "TestFlight+ManualSessions.h", 
				Application.dataPath + DS + "Plugins" + DS + "iOS" + DS + "TestFlight+ManualSessions.h", 
				true);
			
			EditorUtility.DisplayProgressBar("Installing", "Copying iOS Extras ...", 0.6f);
			
			// Copy over our little bit of code to make it all work inside of Xcode
			File.Copy(_extraLocation + DS + "HydrogenTestFlight.mm", 
				Application.dataPath + DS + "Plugins" + DS + "iOS" + DS + "HydrogenTestFlight.mm", 
				true);
			
			EditorUtility.DisplayProgressBar("Installing", "Finished Copying iOS Files ...", 0.7f);
			
			// Make it all pretty with Unity
			AssetDatabase.Refresh();
			
			EditorUtility.DisplayProgressBar("Installing", "Establishing Defined Symbols ...", 0.9f);
		
			// Better turn on the compilation part for iOS, this was a handy way to get around errors
			if ( !PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iPhone).Contains("HYDROGEN_TESTFLIGHT") )
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iPhone, 
					"HYDROGEN_TESTFLIGHT;" + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iPhone));
			}
			
			EditorUtility.DisplayProgressBar("Completed", "Finished Installing iOS Files.", 1f);
		}
		else
		{
			Debug.Log ("Unable to find the required TestFlight files for iOS.");
		}

		EditorUtility.ClearProgressBar();
	}
	
	
	[MenuItem("Hydrogen/Plugins/Install TestFlight for Android")]
	public static void InstallTestFlightForAndroid()
	{

		EditorUtility.DisplayProgressBar("Installing", "Getting Path Information  ...", 0.00f);
		
		// Find the base location for the Android TestFlight files
		string[] _locations = System.IO.Directory.GetFiles(Application.dataPath, "TestFlightLib.jar", System.IO.SearchOption.AllDirectories);
		string _baseLocation = "";
		foreach(string location in _locations)
		{
			if ( location.Contains("Hydrogen") && location.Contains("Vendors") && location.Contains("TestFlight") )
			{
				_baseLocation = location.Replace("TestFlightLib.jar", "");;
				break;
			}
		}
		
		// Find the location of our extras that we want to add into the mix
		string[] _extras = System.IO.Directory.GetFiles(Application.dataPath, "tf.properties", System.IO.SearchOption.AllDirectories);
		string _extraLocation = "";
		
		foreach(string extra in _extras)
		{
			if ( extra.Contains("Hydrogen") && extra.Contains("Extras") && extra.Contains("TestFlight") )
			{
				_extraLocation = extra.Replace("tf.properties", "");;
				break;
			}
		}
		
		EditorUtility.DisplayProgressBar("Installing", "Installing Android Files ...", 0.1f);
		
		if (_baseLocation != "" && _extraLocation != "" )
		{
			// Create our output directories (Cool Chain Effect! +10 Fun)
			Directory.CreateDirectory(Application.dataPath + DS + "Plugins" + DS + "Android" + DS + "res" + DS + "raw");
			
			EditorUtility.DisplayProgressBar("Installing", "Copying Android Files ...", 0.2f);
			
			// Copy over the main TestFlight Android system
			File.Copy(_baseLocation + DS + "TestFlightLib.jar", 
				Application.dataPath + DS + "Plugins" + DS + "Android" + DS + "TestFlightLib.jar", 
				true);
			
			EditorUtility.DisplayProgressBar("Installing", "Copying Android Extras ...", 0.3f);
			
			// Copy that little annoying file into position that makes TestFlight actually know that we have TestFlight packed away into the build
			File.Copy(_extraLocation + DS + "tf.properties",
				Application.dataPath + DS + "Plugins" + DS + "Android" + DS + "res" + DS + "raw" + DS + "tf.properties", 
				true);
			
			EditorUtility.DisplayProgressBar("Installing", "Copying Android Extras ...", 0.4f);
			
			// Grab a copy of Unity's AndroidManifest and shove it in the Android plugins folder (only if we don't already have one there)
			if ( !File.Exists(Application.dataPath + DS + "Plugins" + DS + "Android" + DS + "AndroidManifest.xml"))
			{
				File.Copy (EditorApplication.applicationContentsPath + DS + "PlaybackEngines" + DS + "AndroidPlayer" + DS + "AndroidManifest.xml",
					Application.dataPath + DS + "Plugins" + DS + "Android" + DS + "AndroidManifest.xml", true);
			}
			
			// Add the required permissions for TestFlight
			string[] _manifestLines = System.IO.File.ReadAllLines(Application.dataPath + DS + "Plugins" + DS + "Android" + DS + "AndroidManifest.xml");
			int _insertionLocation = 0;
			bool _foundInternet = false;
			bool _foundNetState = false;
			
			// Loop through the file's lines looking for something like what we need
			for(int x=0;x<_manifestLines.Length;x++)
			{
				if ( _manifestLines[x].Contains ("<uses-permission android:name=\"android.permission.INTERNET\"") ) 
				{
					_foundInternet = true;
				}
				if ( _manifestLines[x].Contains ("<uses-permission android:name=\"android.permission.ACCESS_NETWORK_STATE\"") ) 
				{
					_foundNetState = true;
				}
				if ( _manifestLines[x].Contains ("</application>")) 
				{
					_insertionLocation = x + 1;
				}
			}
			
			if ( !_foundInternet ) 
			{
				Hydrogen.Array.AddAt(ref _manifestLines, _insertionLocation, "<uses-permission android:name=\"android.permission.INTERNET\" />", true);
			}
			if ( !_foundNetState ) 
			{
				Hydrogen.Array.AddAt(ref _manifestLines, _insertionLocation, "<uses-permission android:name=\"android.permission.ACCESS_NETWORK_STATE\" />", true);
			}
			
			System.IO.File.WriteAllLines(Application.dataPath + DS + "Plugins" + DS + "Android" + DS + "AndroidManifest.xml", _manifestLines);
			
			
			EditorUtility.DisplayProgressBar("Installing", "Finished Copying iOS Files ...", 0.7f);
			
			// Make it all pretty with Unity
			AssetDatabase.Refresh();
			
			EditorUtility.DisplayProgressBar("Installing", "Establishing Defined Symbols ...", 0.9f);
		
			// Better turn on the compilation part for Android, this was a handy way to get around errors
			if ( !PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Contains("HYDROGEN_TESTFLIGHT") )
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, 
					"HYDROGEN_TESTFLIGHT;" + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
			}
			
			EditorUtility.DisplayProgressBar("Completed", "Finished Installing Android Files.", 1f);
		}
		else
		{
			Debug.Log ("Unable to find the required TestFlight files for Android.");
		}

		EditorUtility.ClearProgressBar();
	}
}
