using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Oberon
{
    /// <summary>
    /// https://stackoverflow.com/a/76108316
    /// </summary>
    public class DialogSemaphore
    {
        /// <summary>
        /// In order to manage only one open contentdialog at a time, use this semaphore
        /// </summary>
        public static SemaphoreSlim contentDialogSemaphore { get; } = new SemaphoreSlim(1);


        /// <summary>
        /// Use this helper method to show your ContentDialog within the semaphore to avoid collisions
        /// </summary>
        /// <param name="dlg"></param>
        /// <returns>Null if there was no result, or the ContentDialogResult</returns>
        public static async Task<ContentDialogResult?> ShowContentDialogInSemaphore(ContentDialog dlg)
        {
            ContentDialogResult? result = null;
            if (dlg != null)
                try
                {
                    await contentDialogSemaphore.WaitAsync();
                    result = await dlg.ShowAsync();
                }
                finally
                {
                    contentDialogSemaphore.Release();
                }

            return result;
        }
    }
}
