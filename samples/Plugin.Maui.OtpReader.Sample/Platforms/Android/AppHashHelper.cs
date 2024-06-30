using Android.Content;
using Android.Content.PM;
using Android.Util;
using Java.Security;
using System.Text;

namespace Plugin.Maui.OtpReader.Sample.Platforms.Android;

public static class AppHashHelper
{
    private const string TAG = "AppHashHelper";
    private const int NUM_HASHED_BYTES = 9;
    private const int NUM_BASE64_CHAR = 11;

    public static string? GetAppHash(Context context)
    {
        // Get all package signatures for the app
        var packageInfo = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.Signatures);
        foreach (var signature in packageInfo.Signatures)
        {
            var hash = Hash(packageInfo.PackageName, signature.ToCharsString());
            if (!string.IsNullOrEmpty(hash))
            {
                return hash;
            }
        }

        return null;
    }

    private static string? Hash(string packageName, string signature)
    {
        string appInfo = packageName + " " + signature;

        using var messageDigest = MessageDigest.GetInstance("SHA-256");
        messageDigest.Update(Encoding.UTF8.GetBytes(appInfo));
        var hashSignature = messageDigest.Digest();

        var truncatedHash = new byte[NUM_HASHED_BYTES];
        Array.Copy(hashSignature, 0, truncatedHash, 0, NUM_HASHED_BYTES);

        // Encode it in Base64
        var base64Hash = Base64.EncodeToString(truncatedHash, Base64Flags.NoPadding | Base64Flags.NoWrap);
        return base64Hash?.Substring(0, NUM_BASE64_CHAR);
    }
}
