using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Oberon
{
    /// <summary>
    /// Allows the app to stay running in the background by playing media
    /// </summary>
    public class Persistence
    {
        /// <summary>
        /// Used to play silent background noise (to keep the app alive)
        /// </summary>
        public MediaPlayer IdlePlayer;

        /// <summary>
        /// Used to play sounds when a button is pressed on the remote
        /// </summary>
        public MediaPlayer ActivePlayer;

        public Persistence() 
        {
            IdlePlayer = new MediaPlayer();
            IdlePlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Data/idle.wav"));
            IdlePlayer.IsLoopingEnabled = true;
            IdlePlayer.AutoPlay = true;

            ActivePlayer = new MediaPlayer();
            ActivePlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Data/navigate.wav"));
            ActivePlayer.IsLoopingEnabled = false;
            ActivePlayer.AutoPlay = false;
        }

        public void PlayRemoteSound()
        {
            ActivePlayer.Play();
        }
    }
}
