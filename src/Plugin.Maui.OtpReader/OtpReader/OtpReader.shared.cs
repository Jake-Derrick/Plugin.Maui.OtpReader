namespace Plugin.Maui.OtpReader;

public static class OtpReader
{
    static IOtpReader? defaultImplementation;

    /// <summary>
    /// Provides the default implementation for static usage of this API.
    /// </summary>
    public static IOtpReader Default =>
        defaultImplementation ??= new OtpReaderImplementation();

    internal static void SetDefault(IOtpReader? implementation) =>
        defaultImplementation = implementation;
}
