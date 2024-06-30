namespace Plugin.Maui.OtpReader.Sample;

public partial class MainPage : ContentPage
{
    private const string OtpRegex = @"\d{6}"; // Matches exactly 6 consecutive digits
    private readonly IOtpReader feature;

    public MainPage(IOtpReader feature)
    {
        InitializeComponent();

        this.feature = feature;
#if ANDROID
        var hash = Platforms.Android.AppHashHelper.GetAppHash(Android.App.Application.Context);
#endif
    }
    protected override void OnAppearing()
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            feature.OtpReceived += OnOtpReceivedFromSms;

        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            feature.OtpReceived -= OnOtpReceivedFromSms;
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
        feature.StartSmsListener(OtpRegex);
        ListenerStatus.Text = "Listening for SMS message!";
        ListenerStatus.TextColor = Colors.Green;
    }

    private void OnOtpBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        var currentEntry = sender as Entry;

        // If they delete an entry, move to the previous box
        if (string.IsNullOrWhiteSpace(currentEntry?.Text))
        {
            if (currentEntry == OtpBox1) return; // We should keep focus on the first box
            if (currentEntry == OtpBox2) OtpBox1.Focus();
            if (currentEntry == OtpBox3) OtpBox2.Focus();
            if (currentEntry == OtpBox4) OtpBox3.Focus();
            if (currentEntry == OtpBox5) OtpBox4.Focus();
            if (currentEntry == OtpBox6) OtpBox5.Focus();
            return;
        }

        // Handle pasting of the entire OTP
        if (currentEntry.Text.Length > 1)
        {
            SetAllOtpBoxes(currentEntry.Text);
            currentEntry.Unfocus();
            return;
        }

        // Move to the next box
        if (currentEntry == OtpBox1) OtpBox2.Focus();
        if (currentEntry == OtpBox2) OtpBox3.Focus();
        if (currentEntry == OtpBox3) OtpBox4.Focus();
        if (currentEntry == OtpBox4) OtpBox5.Focus();
        if (currentEntry == OtpBox5) OtpBox6.Focus();
        if (currentEntry == OtpBox6) OtpBox6.Unfocus(); // All done :)
    }

    private void SetAllOtpBoxes(string otp)
    {
        if (otp.Length >= 1) OtpBox1.Text = otp[0].ToString();
        if (otp.Length >= 2) OtpBox2.Text = otp[1].ToString();
        if (otp.Length >= 3) OtpBox3.Text = otp[2].ToString();
        if (otp.Length >= 4) OtpBox4.Text = otp[3].ToString();
        if (otp.Length >= 5) OtpBox5.Text = otp[4].ToString();
        if (otp.Length >= 6) OtpBox6.Text = otp[5].ToString();
    }

    private async void SendMessageClicked(object sender, EventArgs e)
    {
        if (!Sms.Default.IsComposeSupported)
            return;

        var text = "Your one time passcode is: 123456\n\n 0FxX7RvjQoB";
        var message = new SmsMessage(text, "");
        await Sms.Default.ComposeAsync(message);
    }
}
