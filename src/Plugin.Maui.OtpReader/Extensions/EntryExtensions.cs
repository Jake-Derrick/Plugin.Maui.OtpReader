using Microsoft.Maui.Handlers;

namespace Plugin.Maui.OtpReader.Extensions;

/// <summary>
/// One-time password extensions for <see cref="Entry"/>
/// </summary>
public static class EntryExtensions
{
    /// <summary>
    /// Sets the TextContentType of the Entry to OneTimeCode on iOS.
    /// </summary>
    /// <remarks>
    /// Has no effect on other platforms.
    /// </remarks>
    public static void SetContentTypeAsOtp(this Entry entry)
    {
#if IOS
        var handler = entry.Handler as EntryHandler;
        if (handler?.PlatformView is UIKit.UITextField nativeEntry)
        {
            nativeEntry.TextContentType = UIKit.UITextContentType.OneTimeCode;
        }
#endif
    }
}
