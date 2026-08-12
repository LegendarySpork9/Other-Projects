// Copyright © - 12/08/2026 - Toby Hunter
using System.Windows;
using System.Windows.Input;

namespace LiveScreensaver
{
    public partial class ScreensaverWindow : Window
    {
        private readonly string[] Videos;
        private readonly List<int> Playlist = [];
        private int PlaylistIndex;
        private Point? LastMousePosition;
        private readonly Random RNG = new();

        // Sets the class's global variables.
        public ScreensaverWindow(string[] videos)
        {
            InitializeComponent();

            Videos = videos;

            ShufflePlaylist();
        }

        /// <summary>
        /// Creates a playlist of videos.
        /// </summary>
        private void ShufflePlaylist()
        {
            Playlist.Clear();
            Playlist.AddRange(Enumerable.Range(
                0,
                Videos.Length));

            for (int i = Playlist.Count - 1; i > 0; i--)
            {
                int j = RNG.Next(i + 1);
                (Playlist[i], Playlist[j]) = (Playlist[j], Playlist[i]);
            }

            PlaylistIndex = 0;
        }

        /// <summary>
        /// Starts the next video.
        /// </summary>
        private void PlayNext()
        {
            if (PlaylistIndex >= Playlist.Count)
            {
                ShufflePlaylist();
            }

            Uri uri = new(
                Videos[Playlist[PlaylistIndex]],
                UriKind.Absolute);
            PlaylistIndex++;

            VideoPlayer.Source = uri;
            VideoPlayer.Play();
        }

        /// <summary>
        /// Triggers a the video.
        /// </summary>
        private void VideoPlayerMediaEnded(
            object sender,
            RoutedEventArgs e)
        {
            PlayNext();
        }

        /// <summary>
        /// Triggers a the video.
        /// </summary>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            PlayNext();
        }

        /// <summary>
        /// Closes the screensaver.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Closes the screensaver.
        /// </summary>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Closes the screensaver.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point pos = e.GetPosition(this);

            if (LastMousePosition == null)
            {
                LastMousePosition = pos;

                return;
            }

            Vector delta = pos - LastMousePosition.Value;

            if (Math.Abs(delta.X) > 10 || Math.Abs(delta.Y) > 10)
            {
                Close();
            }
        }
    }
}
