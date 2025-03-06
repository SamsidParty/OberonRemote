using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Oberon
{
    public sealed partial class MainPage : Page
    {
        public SettingsFile Settings;
        public ObservableCollection<PairedRemote> Remotes { get; } = new ObservableCollection<PairedRemote>();

        public bool InteractionEnabled => !loadingRing.IsActive;

        public MainPage()
        {
            this.InitializeComponent();
            RefreshSettings();
        }

        private async Task RefreshSettings()
        {
            remoteList.SelectedIndex = -1;

            Settings = await SettingsFile.Get();

            // Update observable remotes list
            Remotes.Clear();
            foreach (var remote in Settings.PairedRemotes)
            {
                Remotes.Add(remote);
            }

        }

        private void AddRemote(object sender, RoutedEventArgs e)
        {
            if (loadingRing.IsActive) { return; }

            // Limit to 4 paired remotes
            if (Settings.PairedRemotes.Count >= 4)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Content = "You may only have a maximum of 4 remotes paired at a time, please unpair one to continue.",
                    Title = "Remote Limit Reached",
                    PrimaryButtonText = "Ok"
                };

                DialogSemaphore.ShowContentDialogInSemaphore(dialog);

                return;
            }

            remoteIP.Text = ""; // Clear text
            DialogSemaphore.ShowContentDialogInSemaphore(addRemoteDialog);
        }

        private async void FinishRemotePairing(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            loadingRing.IsActive = true;

            var ipToConnect = Networking.FormatWebSocketHost(remoteIP.Text);

            var connectionResult = await Task.Run(async () => await ConnectionTester.TestConnectionToIP(ipToConnect));

            loadingRing.IsActive = false;

            if (!connectionResult.Contains("Success"))
            {
                remoteFailedErrorMessage.Text = connectionResult;
                await DialogSemaphore.ShowContentDialogInSemaphore(remoteFailedDialog);
            }
            else
            {
                // Save the paired remote
                await RefreshSettings();
                Settings.PairedRemotes.Add(new PairedRemote()
                {
                    DisplayName = connectionResult.Split("\n")[1],
                    InternalID = Guid.NewGuid().ToString(),
                    IPAddress = ipToConnect
                });
                await SettingsFile.Write(Settings);

                // Update
                await RefreshSettings();

                // Success notification
                new ToastContentBuilder()
                    .AddText("Remote Pairing Complete")
                    .AddText("Paired with " + connectionResult.Split("\n")[1])
                    .Show();
            }
        }

        private async void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            remoteList.SelectedIndex = -1;
        }

        private async Task DisconnectActiveRemote()
        {
            var activeRemote = App.Instance.Client?.Remote ?? null;

            if (activeRemote != null)
            {
                App.Instance.Client?.Close();

                while (activeRemote.IsConnected || App.Instance.Client != null)
                {
                    await Task.Delay(250);
                }
            }
        }

        private async Task ConnectRemote(PairedRemote remote)
        {
            App.Instance.Client = new SocketClient(remote);

            while (!remote.IsConnected)
            {
                await Task.Delay(250);
            }
        }

        private async void RemoteItemClicked(object sender, ItemClickEventArgs e)
        {
            if (loadingRing.IsActive) { return; }

            loadingRing.IsActive = true;
            var remote = (PairedRemote)e.ClickedItem;

            if (remote.IsConnected)
            {
                await DisconnectActiveRemote();

                // Disconnect notification
                new ToastContentBuilder()
                    .AddText("Remote Disconnected")
                    .AddText(remote.DisplayName)
                    .Show();
            }
            else
            {
                await DisconnectActiveRemote();
                await ConnectRemote(remote);

                // Success notification
                new ToastContentBuilder()
                    .AddText("Remote Connected")
                    .AddText(remote.DisplayName)
                    .Show();
            }


            loadingRing.IsActive = false;
            await RefreshSettings();
        }
    }
}
