using System;
using System.Collections.Generic;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void AddRemote(object sender, RoutedEventArgs e)
        {
            if (loadingRing.IsActive) { return; }

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
        }
    }
}
