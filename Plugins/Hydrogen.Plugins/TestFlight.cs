/**
 * 
 * 
 * 
 * 
 */

using System.Collections;
#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace Hydrogen.Plugins
{
	public class TestFlight 
	{
#if HYDROGEN_TESTFLIGHT && (UNITY_IPHONE || UNITY_IOS) && (!UNITY_EDITOR)
		[DllImport ("__Internal")]
		private static extern void TestFlight_TakeOff (string token);
		public static void TakeOff(string token)
		{
			TestFlight_TakeOff(token);
		}
	
		[DllImport ("__Internal")]
		private static extern void TestFlight_SubmitFeedback(string feedbackString);
		public static void SubmitFeedback(string feedbackString)
		{
			TestFlight_SubmitFeedback(feedbackString);
		}
		
		[DllImport ("__Internal")]
		private static extern void TestFlight_PassCheckpoint(string checkpointName);
		public static void PassCheckpoint(string checkpointName) 
		{
			TestFlight_PassCheckpoint(checkpointName);
		}	
		
		[DllImport ("__Internal")]
		private static extern void TestFlight_AddCustomEnvironmentInformation(string information, string forKey);
		public static void AddCustomEnvironmentInformation(string information, string forKey) 
		{
			TestFlight_AddCustomEnvironmentInformation(information, forKey);
		}
		
		[DllImport ("__Internal")]
		private static extern void TestFlight_Log(string message);
		public static void Log(string message) 
		{
			TestFlight_Log(message);
		}	
		
		[DllImport ("__Internal")]
		private static extern void TestFlight_LogAsync(string message);
		public static void LogAsync(string message) 
		{
			TestFlight_LogAsync(message);
		}
#elif HYDROGEN_TESTFLIGHT && UNITY_ANDROID && !UNITY_EDITOR
		private static AndroidJavaClass _testFlight = null;
		private static AndroidJavaClass _unityPlayer = null;
		private static AndroidJavaObject _activity = null;
		private static AndroidJavaObject _application = null;
		
		public static void TakeOff(string token)
		{
			_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
			_activity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			_application = _activity.Call<AndroidJavaObject>("getApplication");
			
			_testFlight = new AndroidJavaClass("com.testflightapp.lib.TestFlight"); 
			_testFlight.CallStatic("takeOff", _application, token);
		}
		
		public static void OpenFeedbackView() {}
		
		public static void PassCheckpoint(string checkpointName)
		{
			_testFlight.CallStatic("passCheckpoint", checkpointName);
		}
		
		public static void AddCustomEnvironmentInformation(string information, string forKey) {}
		
		public static void SubmitFeedback(string feedbackString) {}
		
		public static void Log(string message)
		{
			_testFlight.CallStatic("log", message);
		}
		
		public static void LogAsync(string message) {}
		
#else
		public static void TakeOff(string token) {}
		public static void OpenFeedbackView() {}
		public static void PassCheckpoint(string checkpointName) {}
		public static void AddCustomEnvironmentInformation(string information, string forKey) {}
		public static void SubmitFeedback(string feedbackString) {}
		public static void Log(string message) {}
		public static void LogAsync(string message) {}
#endif
	}
}
