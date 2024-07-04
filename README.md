![nuget](https://github.com/Jake-Derrick/Plugin.Maui.OtpReader/assets/60721064/8e1c4e09-5b76-435b-bb8a-f333630a31d4)
# Plugin.Maui.OtpReader


`Plugin.Maui.OtpReader` provides the ability to do automatically retrieve one-time passcodes from SMS messages in .NET MAUI.

![AndroidOtpReader](https://github.com/Jake-Derrick/Plugin.Maui.OtpReader/assets/60721064/c97ed33a-ac79-4142-84ac-8daf5e891a6e)
![IosOtpReader](https://github.com/Jake-Derrick/Plugin.Maui.OtpReader/assets/60721064/8cda704e-edff-4c18-889c-e67e80f2cd30)

## Install Plugin

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.Feature.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.OtpReader/)


Available on [NuGet](http://www.nuget.org/packages/Plugin.Maui.OtpReader).

Install with the dotnet CLI: `dotnet add package Plugin.Maui.OtpReader`, or through the NuGet Package Manager in Visual Studio.

### Supported Platforms

| Platform | Minimum Version Supported |
|----------|---------------------------|
| iOS      | 12+                       |
| macOS    | Not Supported             |
| Android  | 5.0 (API 21)              |
| Windows  | Not Supported             |

## Usage
#### Unfortunately, the usage between Android and iOS differs, but both can be easily implemented within the same project.</p>

### iOS
The code will be automatically parsed and displayed above the keyboard, but it cannot be auto-filled. To display the code above the keyboard, set up an Entry box using the `SetContentTypeAsOtp` extension:
```xaml
<Entry x:Name="OtpEntry" />
```
```csharp
OtpEntry.SetContentTypeAsOtp();
```
Alternatively, in C#:
```
var OtpEntry = new Entry();
OtpEntry.SetContentTypeAsOtp();
```

### Android
Although setting up on Android requires a bit more work, it offers greater flexibility in handling the code.

You can use OtpReader as a static class, e.g., `OtpReader.Default;`, or with dependency injection: `builder.Services.AddSingleton<IOtpReader>(OtpReader.Default);`.

Once you have access to `OtpReader`, you can subscribe to the `OtpReceived` event, which is triggered when the SMS message is received.
```csharp
private readonly IOtpReader _otpReader;
public MainPage(IOtpReader otpReader)
{
  _otpReader = otpReader;
}

protected override void OnAppearing()
{
  _otpReader.OtpReceived += OnOtpReceivedFromSms
  base.OnAppearing();
}

private void OnOtpReceivedFromSms(string? otp)
{
  // Do something with the otp code here. For example, set an Entry's text to the otp code.
}

```

To trigger the `OtpReceived` event, you must first call the `StartSmsListener` method to begin listening for the SMS message. The listener will timeout after 5 minutes if no message is received, so start it right before you send the SMS message. 
You can optionally supply a regex to parse the code from the message. If no regex is supplied, the entire message contents will be returned.

```csharp
private void StartListenerClicked(object sender, EventArgs e)
{
  var otpRegex = @"\d{6}" // Matches exactly 6 consecutive digits
  _otpReader.StartSmsListener(OtpRegex);
}
```

For the listener to read your SMS message, the message must contain a hash unique to your app. See Google's documentation for [computing your apps hash](https://developers.google.com/identity/sms-retriever/verify#computing_your_apps_hash_string). Alternatively, you can get your app's hash with the [AppHashHelper](samples/Plugin.Maui.OtpReader.Sample/Platforms/Android/AppHashHelper.cs) class from the sample app.
