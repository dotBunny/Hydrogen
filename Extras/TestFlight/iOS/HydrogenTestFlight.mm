#include "TestFlight.h"
#include "TestFlight+AsyncLogging.h"
#include "TestFlight+ManualSessions.h"


extern "C" void TestFlight_Initialize() //char* deviceUniqueIdentifier)
{
    // Manual Session Control
    [TestFlight setOptions:@{ TFOptionManualSessions : @YES }];
    
    // Lets use Unity's device identifier for kicks
    //NSString *stringFromChar = [NSString stringWithCString:deviceUniqueIdentifier encoding:NSASCIIStringEncoding];
    
    //[TestFlight setDeviceIdentifier:stringFromChar];
}
extern "C" void TestFlight_TakeOff(char* token)
{
    NSString *stringFromChar = [NSString stringWithCString:token encoding:NSASCIIStringEncoding];
    [TestFlight takeOff:stringFromChar];
}

extern "C" void TestFlight_StartSession()
{
    [TestFlight manuallyStartSession];
}

extern "C" void TestFlight_EndSession()
{
    [TestFlight manuallyEndSession];
}

extern "C" void TestFlight_PassCheckpoint(char* checkpointName)
{
    NSString *stringFromChar = [NSString stringWithCString:checkpointName encoding:NSASCIIStringEncoding];
    [TestFlight passCheckpoint:stringFromChar];
}

extern "C" void TestFlight_SubmitFeedback(char* feedbackString)
{
    NSString *stringFromChar = [NSString stringWithCString:feedbackString encoding:NSASCIIStringEncoding];
    [TestFlight submitFeedback:stringFromChar];
}

extern "C" void TestFlight_AddCustomEnvironmentInformation(char* information, char* key)
{
    NSString *stringFromCharA = [NSString stringWithCString:information encoding:NSASCIIStringEncoding];
    NSString *stringFromCharB = [NSString stringWithCString:key encoding:NSASCIIStringEncoding];
    [TestFlight addCustomEnvironmentInformation:stringFromCharA forKey:stringFromCharB];
}

extern "C" void TestFlight_Log(char* message)
{
    NSString *stringFromChar = [NSString stringWithCString:message encoding:NSASCIIStringEncoding];
    TFLogPreFormatted(stringFromChar);
}

extern "C" void TestFlight_LogAsync(char* message)
{
    NSString *stringFromChar = [NSString stringWithCString:message encoding:NSASCIIStringEncoding];
    TFLog_async(@"%@", stringFromChar);
}