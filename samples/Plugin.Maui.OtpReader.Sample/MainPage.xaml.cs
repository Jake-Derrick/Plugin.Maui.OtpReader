using Plugin.Maui.OtpReader.Extensions;

namespace Plugin.Maui.OtpReader.Sample;

public partial class MainPage : ContentPage
{
    private const string OtpRegex = @"\d{6}"; // Matches exactly 6 consecutive digits
    private readonly IOtpReader _otpReader;
    private readonly string? AppHash;
    private readonly Entry[] OtpBoxes = [];

    public MainPage(IOtpReader otpReader)
    {
        InitializeComponent();
        SetupOtpBoxesForIos();

        _otpReader = otpReader;
        OtpBoxes = [OtpBox1, OtpBox2, OtpBox3, OtpBox4, OtpBox5, OtpBox6];
#if ANDROID
        // This unique app hash needs to be included in the android SMS message for the app to be able to read the message contents.
        AppHash = Platforms.Android.AppHashHelper.GetAppHash(Android.App.Application.Context);
#endif
    }

    protected override void OnAppearing()
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            _otpReader.OtpReceived += OnOtpReceivedFromSms;

        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            _otpReader.OtpReceived -= OnOtpReceivedFromSms;
    }

    private void OnOtpReceivedFromSms(string? otp)
    {
        if (string.IsNullOrWhiteSpace(otp))
        {
            ListenerStatus.Text = "Error! Listener stopped.";
            ListenerStatus.TextColor = Colors.Red;
            return;
        }

        SetAllOtpBoxes(otp);
        ListenerStatus.Text = "Code received! Listener stopped.";
    }

    // Starts listening for an SMS message (5 minutes)
    private void StartListenerClicked(object sender, EventArgs e)
    {
        _otpReader.StartSmsListener(OtpRegex);
        ListenerStatus.Text = "Listening for SMS message!";
        ListenerStatus.TextColor = Colors.Green;
    }

    private void OnOtpBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        var currentEntry = sender as Entry;
        var currentIndex = Array.IndexOf(OtpBoxes, currentEntry);

        // If they delete an entry, move focus to the previous box
        if (string.IsNullOrEmpty(currentEntry?.Text))
        {
            if (currentIndex > 0)
                OtpBoxes[currentIndex - 1].Focus();

            return;
        }

        // Handle pasting of the entire OTP
        if (currentEntry.Text.Length > 1)
        {
            SetAllOtpBoxes(currentEntry.Text);
            currentEntry.Unfocus();
            return;
        }

        // Move to the next box or unfocus if at the last box
        (currentIndex < OtpBoxes.Length - 1 ? OtpBoxes[currentIndex + 1] : currentEntry).Focus();
    }

    private void SetAllOtpBoxes(string otp)
    {
        for (int i = 0; i < otp.Length && i < OtpBoxes.Length; i++)
        {
            OtpBoxes[i].Text = otp[i].ToString();
        }
    }

    private void SetupOtpBoxesForIos()
    {
        foreach (var otpBox in OtpBoxes)
            otpBox.SetContentTypeAsOtp();
    }

    private async void SendMessageClicked(object sender, EventArgs e)
    {
        if (!Sms.Default.IsComposeSupported)
            return;

        var text = $"Your one time passcode is: 123456\n\n {AppHash}";
        var message = new SmsMessage(text, "");
        await Sms.Default.ComposeAsync(message);
    }
}
