
namespace Plugin.Maui.OtpReader;

partial class OtpReaderImplementation : IOtpReader
{
    // This usually is a placeholder as .NET MAUI apps typically don't run on .NET generic targets unless through unit tests and such
    public event Action<string>? OtpReceived;

    public BaseResult StartSmsListener(string? otpRegex = null)
    {
        throw new NotImplementedException();
    }
}