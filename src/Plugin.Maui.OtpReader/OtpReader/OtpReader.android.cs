using Android.Content;
using Android.Gms.Auth.Api.Phone;
using Android.Gms.Common.Apis;
using Android.Gms.Tasks;
using System.Text.RegularExpressions;

namespace Plugin.Maui.OtpReader;

partial class OtpReaderImplementation : IOtpReader
{
    private string? OtpRegex;
    public event Action<string>? OtpReceived;

    public BaseResult StartSmsListener(string? otpRegex = null)
    {
        OtpRegex = otpRegex;
        var client = SmsRetriever.GetClient(Android.App.Application.Context);

        client.StartSmsRetriever()
            .AddOnSuccessListener(new SuccessListener(OnSmsReceived))
            .AddOnFailureListener(new FailureListener());

        return new BaseResult();
    }

    private void OnSmsReceived(Context context, Intent intent)
    {
        if (!SmsRetriever.SmsRetrievedAction.Equals(intent.Action))
            return;

        var extras = intent.Extras;
        var status = (Statuses?)extras?.Get(SmsRetriever.ExtraStatus);

        switch (status?.StatusCode)
        {
            case CommonStatusCodes.Success:
                var message = (string?)extras?.Get(SmsRetriever.ExtraSmsMessage);
                OnOtpMessageSuccess(message);
                break;
            case CommonStatusCodes.Timeout:
                break;
            default:
                break;
        }
    }

    private void OnOtpMessageSuccess(string? message)
    {
        var returnValue = message;
        if (OtpRegex is not null && message is not null)
        {
            var otpMatch = Regex.Match(message, OtpRegex);
            if (otpMatch.Success)
                returnValue = otpMatch.Value;
        }

        OtpReceived(returnValue);
    }
}
internal class SuccessListener(Action<Context, Intent> onOtpReceived) : Java.Lang.Object, IOnSuccessListener
{
    public void OnSuccess(Java.Lang.Object result)
    {
        var receiver = new SmsBroadcastReceiver(onOtpReceived);
        Android.App.Application.Context.RegisterReceiver(receiver, new IntentFilter(SmsRetriever.SmsRetrievedAction));
    }
}

internal class FailureListener : Java.Lang.Object, IOnFailureListener
{
    public void OnFailure(Java.Lang.Exception e)
    {
        // Handle failure
    }
}

internal class SmsBroadcastReceiver(Action<Context, Android.Content.Intent> onOtpReceived) : BroadcastReceiver
{
    public Action<Context, Intent>? OtpReceived = onOtpReceived;

    public override void OnReceive(Context context, Intent intent)
    {
        OtpReceived(context, intent);
    }
}
