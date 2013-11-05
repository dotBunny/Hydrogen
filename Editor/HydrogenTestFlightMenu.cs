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
		string[] _extras = System.IO.Directory.GetFiles(Application.dataPath, "AndroidManifestMods.xml", System.IO.SearchOption.AllDirectories);
		string _extraLocation = "";
		
		foreach(string extra in _extras)
		{
			if ( extra.Contains("Hydrogen") && extra.Contains("Extras") && extra.Contains("TestFlight") )
			{
				_extraLocation = extra.Replace("AndroidManifestMods.xml", "");;
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
			File.Copy(_extraLocation + DS +  "res" + DS + "raw" + DS + "tf.properties",
				Application.dataPath + DS + "Plugins" + DS + "Android" + DS + "res" + DS + "raw" + DS + "tf.properties", 
				true);
			
			EditorUtility.DisplayProgressBar("Installing", "Copying Android Extras ...", 0.4f);
			
			// Grab a copy of Unity's AndroidManifest and shove it in the Android plugins folder (only if we don't already have one there)
			if ( !File.Exists(Application.dataPath + DS + "Plugins" + DS + "Android" + "AndroidManifest.xml") )
			{
				File.Copy (EditorApplication.applicationContentsPath + DS + "PlaybackEngines" + DS + "AndroidPlayer" + DS + "AndroidManifest.xml",
					Application.dataPath + DS + "Plugins" + DS + "Android" + DS + "AndroidManifest.xml");
				
				// Do a little parsing for the user ahead of time
			}
			
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
