
namespace Plugin.Maui.OtpReader;

partial class OtpReaderImplementation : IOtpReader
{
    public event Action<string>? OtpReceived;

    public BaseResult StartSmsListener(string? otpRegex = null)
    {
        throw new NotImplementedException();
    }
}