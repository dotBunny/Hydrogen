/**
 * 
 * 
 *  * <?xml version="1.0" encoding="utf-8"?>
<manifest>
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>   
</manifest>
*/

using UnityEngine;

public class hTestFlight : MonoBehaviour
{
	public bool Flying { get; private set; }
	
	public bool presistant = true;
	public string tokenAndroid = "";
	public string tokenIOS = "";
	
	/// <summary>
	/// Internal fail safe to maintain instance across threads
	/// </summary>
	/// <remarks>
	/// Multithreaded Safe Singleton Pattern
	/// </remarks>
	/// <description>
	/// http://msdn.microsoft.com/en-us/library/ms998558.aspx
	/// </description>
	private static readonly System.Object _syncRoot = new System.Object();
    
	/// <summary>
	/// Internal reference to the static instance of the TestFlight interface.
	/// </summary>
	private static volatile hTestFlight _staticInstance;
	
	/// <summary>
	/// Gets the TestFlight interface instance.
	/// </summary>
	/// <value>
	/// The TestFlight interface
	/// </value>
	public static hTestFlight Instance 
	{
        get {
            if (_staticInstance == null) {				
                lock (_syncRoot) {
                    _staticInstance = FindObjectOfType (typeof(hTestFlight)) as hTestFlight;
                    
					// If we don't have it, lets make it!
					if (_staticInstance == null) {
						GameObject newTestFlight = new GameObject("TestFlight");
						newTestFlight.AddComponent<hTestFlight>();
						_staticInstance = newTestFlight.GetComponent<hTestFlight>();	
                    }
                }
            }
            return _staticInstance;
        }
    }
	
	public void Awake()
	{
		// Should this gameObject be kept around :) I think so.
		if ( presistant ) DontDestroyOnLoad( this.gameObject );
		
#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
		TakeOff(tokenIOS);
#elif UNITY_ANDROID && !UNITY_EDITOR
		TakeOff(tokenAndroid);
#endif
	}
	
	public void TakeOff(string token)
	{
		if ( Flying ) return;
		
		if ( token != "" && token != null )
		{
			Hydrogen.Plugins.TestFlight.TakeOff(tokenAndroid);
			Flying = true;
		}
	}
	
	public void SubmitFeedback(string message)
	{
		if ( !Flying )
		{
			Debug.Log ("Unable to submit feedback, TestFlight is not in flight.");
			return;
		}
		
		Hydrogen.Plugins.TestFlight.SubmitFeedback(message);
	}
	
	public void PassCheckpoint(string checkpointName)
	{
		if ( !Flying )
		{
			Debug.Log ("Unable to send checkpoint data, TestFlight is not in flight.");
			return;
		}
		
		Hydrogen.Plugins.TestFlight.PassCheckpoint(checkpointName);
	}
	
	public void AddCustomEnvironmentInformation(string data, string key)
	{
		if ( !Flying ) 
		{
			Debug.Log ("Unable to send information, TestFlight is not in flight.");
			return;
		}
		
		Hydrogen.Plugins.TestFlight.AddCustomEnvironmentInformation(data,key);
	}
	
	public void Log(string message)
	{
		if ( !Flying ) 
		{
			Debug.Log ("Unable to send log data, TestFlight is not in flight.");
			return;
		}
		
		Hydrogen.Plugins.TestFlight.Log(message);
	}
	
	public void LogAsync(string message)
	{
		if ( !Flying ) 
		{
			Debug.Log ("Unable to send log data, TestFlight is not in flight.");
			return;
		}
			
			
		Hydrogen.Plugins.TestFlight.LogAsync(message);
	}
}