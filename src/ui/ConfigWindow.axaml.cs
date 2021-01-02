using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System.Net;

namespace Quest_Discord_Presence_Client
{
    public class ConfigWindow : Window
    {
        private Client client;
        private Button confirmIp;
        private TextBox ipBox;
        private TextBlock ipText;
        private TextBlock requestStatusText;
        public ConfigWindow()
        {
            this.client = Client.Instance;

            // Register events and get components from the UI
            InitializeComponents();
            UpdateCurrentIpText();

#if DEBUG
            this.AttachDevTools();
#endif
            // Listen for when fetching the presence becomes successful/fails
            client.PresenceManager.LastRequestStatusChanged += OnRequestStatusChanged;
        }

        private void InitializeComponents()
        {
            AvaloniaXamlLoader.Load(this);
            // Grab the UI elements used for configuring stuff
            confirmIp = this.FindControl<Button>("confirmIp");
            ipBox = this.FindControl<TextBox>("ipBox");
            ipText = this.FindControl<TextBlock>("ipText");
            requestStatusText = this.FindControl<TextBlock>("requestStatusText");
                
            // Subscribe to events
            ipBox.PropertyChanged += OnIpTextChange;
            confirmIp.Click += OnConfirmIpAddress;

            UpdateConfirmButton();
            UpdateStatusText();
        }

        // Listen for when the IP text changes to show/hide the confirm button
        private void OnIpTextChange(object sender, AvaloniaPropertyChangedEventArgs args)
        {
           if(args.Property.Name != "Text") { return; }

            UpdateConfirmButton();
        }

        private void OnConfirmIpAddress(object sender, RoutedEventArgs args)
        {
            // Make sure the entered IP is actually valid
            if(!IsEnteredIpAddressValid()) {return;}

            // Update it in the config and save
            client.Config.QuestAddress = ipBox.Text;
            client.SaveConfig();

            UpdateCurrentIpText();
        }

        private void UpdateCurrentIpText()
        {
            string configIp = client.Config.QuestAddress;
            // Use None as the IP if it is set to the default
            string ip = configIp == "REPLACE WITH QUEST IP" ? "None" : configIp;
            ipText.Text = "Currently selected IP: " + ip;
        }

        // Makes the set ip button grey if the entered ip is invalid
        private void UpdateConfirmButton() {
            bool valid = IsEnteredIpAddressValid();

            confirmIp.Foreground = valid ? Brushes.Black : Brushes.Red;
            confirmIp.Content = valid ? "Change IP" : "Enter a valid IP";
        }

        // Checks if the entered new ip address is in a valid format
        private bool IsEnteredIpAddressValid()
        {
            string text = ipBox.Text;
            if(text == null) {return false;} // There must be text in the box

            if(text.Split('.').Length != 4) { return false; } // Only allow valid IP addresses with 4 numbers
            IPAddress result = null;
            return IPAddress.TryParse(text, out result);
        }

        // Updates the status of the last request text bot
        private void UpdateStatusText() {
            string newText = "Status: " + client.PresenceManager.LastRequestStatus.Info;
            requestStatusText.Text = newText;
        }

        private void OnRequestStatusChanged(object sender, StatusChangedEventArgs args) {
            // Make sure to run it on the UI thread
            Dispatcher.UIThread.InvokeAsync(() => {
                UpdateStatusText();
            });
        }
    }
}