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
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Oberon
{
    public sealed partial class MainPage : Page
    {
        public static MainPage Instance;

        public SettingsFile Settings;
        public ObservableCollection<PairedRemote> Remotes { get; } = new ObservableCollection<PairedRemote>();

        public PairedRemote HoveredRemote;

        public bool InteractionEnabled => !loadingRing.IsActive;

        public MainPage()
        {
            Instance = this;
            this.InitializeComponent();
            Window.Current.CoreWindow.KeyDown += KeyDown;

            RefreshSettings();
        }

        private void KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (loadingRing.IsActive) { return; }

            if (args.VirtualKey == VirtualKey.GamepadMenu)
            {
                if (HoveredRemote == null) { return; }

                ContentDialog dialog = new ContentDialog
                {
                    Content = $"Are you sure you would like to remove '{HoveredRemote.DisplayName}' from the paired remotes?",
                    Title = "Unpair Remote",
                    PrimaryButtonText = "Cancel",
                    SecondaryButtonText = "Remove"
                };

                dialog.SecondaryButtonClick += async (_sender, e) =>
                {
                    if (loadingRing.IsActive) { return; }
                    loadingRing.IsActive = true;

                    // Check if the remote is connected first
                    if (HoveredRemote.IsConnected)
                    {
                        await DisconnectActiveRemote();
                    }

                    // Remove it
                    await RefreshSettings();
                    var remoteIndex = Settings.PairedRemotes.FindIndex((r) => r.InternalID == HoveredRemote.InternalID);

                    if (remoteIndex < 0) { return; }

                    Settings.PairedRemotes.RemoveAt(remoteIndex);
                    await SettingsFile.Write(Settings);

                    await RefreshSettings();
                    loadingRing.IsActive = false;
                };

                DialogSemaphore.ShowContentDialogInSemaphore(dialog);
            }
        }

        public async Task RefreshSettings()
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

        private async void Donate(object sender, RoutedEventArgs e)
        {
            await CurrentApp.RequestProductPurchaseAsync("donation2");
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
            if (e.AddedItems.Count > 0) {
                HoveredRemote = (PairedRemote)e.AddedItems.First();
            }

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

        private async Task<bool> ConnectRemote(PairedRemote remote)
        {
            App.Instance.Client = new SocketClient(remote);

            while (!remote.IsConnected)
            {
                // Connection failed
                if (App.Instance.Client == null)
                {
                    return false;
                }

                await Task.Delay(250);
            }

            return true;
        }

        private async void RemoteItemClicked(object sender, ItemClickEventArgs e)
        {
            if (loadingRing.IsActive) { return; }

            loadingRing.IsActive = true;
            var remote = (PairedRemote)e.ClickedItem;

            if (remote.IsConnected)
            {
                await DisconnectActiveRemote();
            }
            else
            {
                await DisconnectActiveRemote();
                var result = await ConnectRemote(remote);

                if (result)
                {
                    // Success notification
                    new ToastContentBuilder()
                        .AddText("Remote Connected")
                        .AddText(remote.DisplayName)
                        .Show();
                }
                else
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Content = "Failed to connect to the remote, please make sure the remote is running on your local network.",
                        Title = "Connection Failed",
                        PrimaryButtonText = "Ok"
                    };

                    DialogSemaphore.ShowContentDialogInSemaphore(dialog);

                }

            }


            loadingRing.IsActive = false;
            await RefreshSettings();
        }
    }
}
