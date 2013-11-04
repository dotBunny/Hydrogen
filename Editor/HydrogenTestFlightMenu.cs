using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class HydrogenTestFlightMenu  {
	
	public static char DS = System.IO.Path.DirectorySeparatorChar;

	[MenuItem("Hydrogen/Plugins/Install TestFlight for iOS")]
	public static void InstallTestFlightForIOS()
	{
		EditorUtility.DisplayProgressBar("Installing", "Initializing ...", 0.00f);
		
		string[] _locations = System.IO.Directory.GetFiles(Application.dataPath, "libTestFlight.a", System.IO.SearchOption.AllDirectories);
		
		string _location = "";
		foreach(string location in _locations)
		{
			if ( location.Contains("Vendors") && location.Contains("TestFlight") )
			{
				_location = location;
				break;
			}
		}
		
		EditorUtility.DisplayProgressBar("Installing", "Finding iOS Files ...", 0.1f);
		
		if (_location != "" )
		{
			// Determine where our main Android file is to allow for some sort of relative path usage
			_location = _location.Replace("libTestFlight.a", "");
	
			// Create our output directories (Cool Chain Effect! +10 Fun)
			Directory.CreateDirectory(Application.dataPath + DS + "Plugins" + DS + "iOS");
			
			EditorUtility.DisplayProgressBar("Installing", "Copying iOS Files ...", 0.2f);
			
			// Copy over the main TestFlight iOS system
			File.Copy(_location + DS + "libTestFlight.a", 
				Application.dataPath + DS + "Plugins" + DS + "iOS" + DS + "libTestFlight.a", 
				true);
			
			EditorUtility.DisplayProgressBar("Installing", "Copying iOS Files ...", 0.3f);
			
			File.Copy(_location + DS + "HydrogenTestFlight.mm", 
				Application.dataPath + DS + "Plugins" + DS + "iOS" + DS + "HydrogenTestFlight.mm", 
				true);
			
			EditorUtility.DisplayProgressBar("Installing", "Copying iOS Files ...", 0.4f);
			
			File.Copy(_location + DS + "TestFlight.h", 
				Application.dataPath + DS + "Plugins" + DS + "iOS" + DS + "TestFlight.h", 
				true);
			EditorUtility.DisplayProgressBar("Installing", "Copying iOS Files ...", 0.5f);
			
			File.Copy(_location + DS + "TestFlight+AsyncLogging.h", 
				Application.dataPath + DS + "Plugins" + DS + "iOS" + DS + "TestFlight+AsyncLogging.h", 
				true);
			
			EditorUtility.DisplayProgressBar("Installing", "Copying iOS Files ...", 0.6f);
			
			File.Copy(_location + DS + "TestFlight+ManualSessions.h", 
				Application.dataPath + DS + "Plugins" + DS + "iOS" + DS + "TestFlight+ManualSessions.h", 
				true);
			
			EditorUtility.DisplayProgressBar("Installing", "Finished Copying iOS Files.", 0.7f);
			
			AssetDatabase.Refresh();
			
			EditorUtility.DisplayProgressBar("Installing", "Establishing Defined Symbols", 1f);
		
			if ( !PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iPhone).Contains("HYDROGEN_TESTFLIGHT") )
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iPhone, 
					"HYDROGEN_TESTFLIGHT;" + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iPhone));
			}
			
			EditorUtility.DisplayProgressBar("Completed", "Finished Installing iOS Files", 1f);
		}
		else
		{
			Debug.Log ("Unable to find TestFlight's iOS Library");
		}

		EditorUtility.ClearProgressBar();
	}
	
	[MenuItem("Hydrogen/Plugins/Install TestFlight for Android")]
	public static void InstallTestFlightForAndroid()
	{
		EditorUtility.DisplayProgressBar("Installing", "Initializing ...", 0f);
		
		string[] _locations = System.IO.Directory.GetFiles(Application.dataPath, "TestFlightLib.jar", System.IO.SearchOption.AllDirectories);
		string _location = "";
		
		foreach(string location in _locations)
		{
			if ( location.Contains("Vendors") && location.Contains("TestFlight") )
			{
				_location = location;
				break;
			}
		}
		
		EditorUtility.DisplayProgressBar("Installing", "Finding Android Files ...", 0.05f);
			
		if (_location != "" )
		{
			// Determine where our main Android file is to allow for some sort of relative path usage
			_location = _location.Replace("TestFlightLib.jar", "");
			
			EditorUtility.DisplayProgressBar("Installing", "Creating Additional Folders for Android Files ...", 0.05f);
			
			// Create our output directories (Cool Chain Effect! +10 Fun)
			Directory.CreateDirectory(Application.dataPath + DS + "Plugins" + DS + "Android" + DS + "res" + DS + "raw");
			
			EditorUtility.DisplayProgressBar("Installing", "Copying Android Files ...", 0.1f);
			
			// Copy over the main TestFlight android system
			File.Copy(_location + DS + "TestFlightLib.jar", Application.dataPath + DS + "Plugins" + DS + "Android" + DS + "TestFlightLib.jar", true);
			
			// Copy over the failsafe little 'notifier' file
			File.Copy(_location + DS +  "res" + DS + "raw" + DS + "tf.properties",
				Application.dataPath + DS + "Plugins" + DS + "Android" + DS + "res" + DS + "raw" + DS + "tf.properties", 
				true);
			
			EditorUtility.DisplayProgressBar("Installing", "Finished Copying Android Files.", .99f);
			
			
			AssetDatabase.Refresh();
			
			EditorUtility.DisplayProgressBar("Installing", "Establishing Defined Symbols", 1f);
		
			if ( !PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Contains("HYDROGEN_TESTFLIGHT") )
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, 
					"HYDROGEN_TESTFLIGHT;" + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
			}
			
			EditorUtility.DisplayProgressBar("Completed", "Finished Installing Android Files", 1f);
		}
		else
		{
			Debug.Log ("Unable to find TestFlight's Android Library");
		}
			


		EditorUtility.ClearProgressBar();
	}
}
