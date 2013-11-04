##Introduction

The TestFlight SDK lets you track how testers are testing your application. We track usage information, such as who is using your application, device model/OS, how long they used the application, logs and any crashes they encounter.

To get the most out of the SDK we have provided the Checkpoint API.

The Checkpoint API helps you track how testers are using your application. Curious about who passed level 5 in your game, or posted their high score to Twitter, or found that obscure feature? With a single line of code you can gather all this information. Wondering how many times your app has crashed? Wondering who your power testers are? We've got you covered.

For more detailed debugging we have a remote logging solution. Find out more about our logging system in the Remote Logging section.


##Considerations
       
Information gathered by the SDK is sent to the website in real time. When an application is put into the background or terminated we try to send the finalizing information for the session during the time allowed for finalizing the application. Should all of the data not get sent the remaining data will be sent the next time the application is launched.

                
##Integration

1. Add "TestFlightLib.jar" as a dependency to your Android application.

2. Import TestFlight in your main application class and call `takeOff` in the `onCreate` method of your application class, passing your unique application token. To get your app token, create or go to your app in the dashboard and click "App Token". 

        import com.testflightapp.lib.TestFlight;
    	...
        
		public class MyApplication extends Application {
    		@Override
	        public void onCreate() {
	        	super.onCreate();
        		//Initialize TestFlight with your app token.
	            TestFlight.takeOff(this, YOUR_APP_TOKEN);
	            ...
	        }
        }

3. Update your AndroidManifest.xml file to point to your application class and grant the permissions `INTERNET` and `ACCESS_NETWORK_STATE`.

		<manifest ...>
			<application ... android:name="MyApplication">
			...
			</application>
			<uses-permission android:name="android.permission.INTERNET"/>
			<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>
		</manifest>

###Minimum Android SDK Version

The minimum Android SDK version supported by TestFlight is 8 (2.2).

	<uses-sdk android:minSdkVersion="8" ... />
	
    
##Checkpoint API

When a tester does something you care about in your app you can pass a checkpoint. For example completing a level, adding a todo item, etc. The checkpoint progress is used to provide insight into how your testers are testing your apps. The passed checkpoints are also attached to crashes, which can help when creating steps to replicate.

    TestFlight.passCheckpoint(CHECKPOINT_NAME);

Use `passCheckpoint()` to track when a user performs certain tasks in your application. This can be useful for making sure testers are hitting all parts of your application, as well as tracking which testers are being thorough.


##Upload your build
    
After you have integrated the SDK into your application you need to upload your build to TestFlight. You can upload from your dashboard or or using the Upload API, full documentation at [https://testflightapp.com/api/doc/](https://testflightapp.com/api/doc/)

###Debug vs. Release build signing

TestFlight will show all session and crash data for builds signed with a debug certificate. Builds signed with a release certificate are considered production apps and only crash data will be visible in the TestFlight UI. All production data will still be visible in FlightPath.


##View the results
                
As testers install your build and start to test it you will see their session data on the web on the build report page for the build you've uploaded.


##Remote Logging
       
To perform remote logging you can use the `TestFlight.log()` method.

    TestFlight.log("Logging info here…");
