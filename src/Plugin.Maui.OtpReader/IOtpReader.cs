namespace Plugin.Maui.OtpReader;

/// <summary>
/// TODO: Provide relevant comments for your APIs
/// </summary>
public interface IOtpReader
{
    BaseResult StartSmsListener(string? otpRegex = null);
    event Action<string>? OtpReceived;
}