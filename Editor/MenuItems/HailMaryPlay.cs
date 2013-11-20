using UnityEngine;
using UnityEditor;
using System.Collections;

public class HailMaryPlay {
	
	public static string injection =  "UnityEngine.Debug.Log(\".:Hail Mary:.\");"; 
	
	[MenuItem("Assets/The \"Hail Mary\" Play/Enable Project Wide")]
	public static void EnableHailMary () 
	{
		string[] files = System.IO.Directory.GetFiles(".", "*.cs", System.IO.SearchOption.AllDirectories);
		foreach(string filePath in files)
		{
			EnableHailMaryOnFile(filePath);
		}
		AssetDatabase.Refresh();
	}
	
	[MenuItem("Assets/The \"Hail Mary\" Play/Disable Project Wide")]
	public static void DisableHailMary ()
	{
		string[] files = System.IO.Directory.GetFiles(".", "*.cs", System.IO.SearchOption.AllDirectories);
		foreach(string filePath in files)
		{
			DisableHailMaryOnFile(filePath);
		}
		AssetDatabase.Refresh();
	}

	[MenuItem("Assets/The \"Hail Mary\" Play/Enable On Selected", true)]
	public static bool EnableHailMaryOnSelectedCheck ()
	{
		if ( Selection.instanceIDs.Length == 0 ) return false;
		return true;
	}

	[MenuItem("Assets/The \"Hail Mary\" Play/Enable On Selected")]
	public static void EnableHailMaryOnSelected () 
	{
		foreach(int id in Selection.instanceIDs)
		{
			string filePath = AssetDatabase.GetAssetPath(id);
			if ( filePath.EndsWith(".cs") )
			{
				EnableHailMaryOnFile(filePath);
			}
		}
		AssetDatabase.Refresh();
	}
	

	[MenuItem("Assets/The \"Hail Mary\" Play/Disable On Selected", true)]
	public static bool DisableHailMaryOnSelectedCheck ()
	{
		if ( Selection.instanceIDs.Length == 0 ) return false;
		return true;
	}

	[MenuItem("Assets/The \"Hail Mary\" Play/Disable On Selected")]
	public static void DisableHailMaryOnSelected ()
	{
		foreach(int id in Selection.instanceIDs)
		{
			string filePath = AssetDatabase.GetAssetPath(id);
			if ( filePath.EndsWith(".cs") )
			{
				DisableHailMaryOnFile(filePath);
			}
		}
		AssetDatabase.Refresh();
	}
	
	public static void EnableHailMaryOnFile(string filePath)
	{
		if ( filePath.EndsWith("HailMary.cs") ) return;
		
		string[] lines = System.IO.File.ReadAllLines(filePath);
		
		bool writeFile = false;
		
		for(int x=0;x<lines.Length;x++)
		{
			var test = System.Text.RegularExpressions.Regex.Matches(lines[x],@"([a-zA-Z0-9]*)\s*\([^()]*\)");
			
			if (test.Count > 0 
			    && (lines[x].Contains("{") || lines[x+1].Contains("{"))
			    && !(lines[x].Contains("{") && lines[x+1].Contains("{"))
			    && !lines[x].Contains("CValue")
			    && !lines[x].Contains("new")
			    && !lines[x].TrimStart().StartsWith("if") 
			    && !lines[x].TrimStart().StartsWith("elseif")
			    && !lines[x].TrimStart().StartsWith("else")
			    && !lines[x].TrimStart().StartsWith("foreach") 
			    && !lines[x].TrimStart().StartsWith("for")
			    && !lines[x].TrimStart().StartsWith("switch")
			    && !lines[x].TrimStart().StartsWith("var") 
			    && !lines[x].TrimStart().StartsWith("while") 
			    && !lines[x].TrimStart().StartsWith("catch") 
			    && !lines[x].TrimStart().StartsWith("throw") 
			    && !lines[x].TrimStart().StartsWith("get")
			    && !lines[x].TrimStart().StartsWith("set") 
			    && !lines[x].TrimStart().StartsWith("//") 
			    && !lines[x].Contains("Debug.Log")) 
			{
				if ( lines[x].Contains("{"))
				{
					Hydrogen.Array.AddAt(ref lines, x+1, injection, false);
				}
				else
				{
					Hydrogen.Array.AddAt(ref lines, x+2, injection, false);
				}
				writeFile = true;
			}
			
		}
		
		if ( writeFile ) 
		{
			System.IO.File.WriteAllLines(filePath, lines);
		}
	}	
	
	public static void DisableHailMaryOnFile(string filePath)
	{
		if ( filePath.EndsWith("HailMary.cs") ) return;
		
		string[] lines = System.IO.File.ReadAllLines(filePath);
		
		bool writeFile = false;
		
		for(int x=0;x<lines.Length;x++)
		{
			if ( lines[x].StartsWith(injection) )
			{
				Hydrogen.Array.RemoveAt(ref lines, x);
				writeFile = true;
			}
		}
		
		if ( writeFile ) 
		{
			System.IO.File.WriteAllLines(filePath, lines);
		}
		
	}
}