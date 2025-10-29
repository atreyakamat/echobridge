using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EchoBridge.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBridge.UI
{
    public partial class SpotifyWidgetViewModel : ObservableObject
    {
        private readonly SpotifyService _spotifyService;
        private Timer? _updateTimer;

        [ObservableProperty]
        private bool _isConnected;

        [ObservableProperty]
        private string _statusMessage = "Not connected";

        [ObservableProperty]
        private string _trackName = "No track playing";

        [ObservableProperty]
        private string _artistName = "";

        [ObservableProperty]
        private string _albumName = "";

        [ObservableProperty]
        private string _albumArtUrl = "";

        [ObservableProperty]
        private bool _isPlaying;

        [ObservableProperty]
        private double _progress;

        [ObservableProperty]
        private string _progressText = "0:00 / 0:00";

        public SpotifyWidgetViewModel()
        {
            _spotifyService = new SpotifyService();
            _spotifyService.ConnectionStatusChanged += OnConnectionStatusChanged;
        }

        [RelayCommand]
        private async Task ConnectAsync()
        {
            StatusMessage = "Connecting...";
            var success = await _spotifyService.AuthenticateAsync();
            
            if (success)
            {
                IsConnected = true;
                StartPolling();
            }
        }

        [RelayCommand]
        private async Task PlayPauseAsync()
        {
            if (!IsConnected) return;
            
            await _spotifyService.PlayPauseAsync();
            await UpdateNowPlayingAsync();
        }

        [RelayCommand]
        private async Task SkipNextAsync()
        {
            if (!IsConnected) return;
            
            await _spotifyService.SkipToNextAsync();
            await Task.Delay(500); // Wait for Spotify to update
            await UpdateNowPlayingAsync();
        }

        [RelayCommand]
        private async Task SkipPreviousAsync()
        {
            if (!IsConnected) return;
            
            await _spotifyService.SkipToPreviousAsync();
            await Task.Delay(500); // Wait for Spotify to update
            await UpdateNowPlayingAsync();
        }

        private void StartPolling()
        {
            // Poll Spotify API every 2 seconds for updates
            _updateTimer = new Timer(async _ => await UpdateNowPlayingAsync(), null, 0, 2000);
        }

        private async Task UpdateNowPlayingAsync()
        {
            try
            {
                var track = await _spotifyService.GetCurrentlyPlayingAsync();
                
                if (track != null)
                {
                    TrackName = track.TrackName;
                    ArtistName = track.ArtistName;
                    AlbumName = track.AlbumName;
                    AlbumArtUrl = track.AlbumArtUrl;
                    IsPlaying = track.IsPlaying;
                    
                    // Update progress
                    if (track.DurationMs > 0)
                    {
                        Progress = (double)track.ProgressMs / track.DurationMs * 100;
                        ProgressText = $"{FormatTime(track.ProgressMs)} / {FormatTime(track.DurationMs)}";
                    }
                }
                else
                {
                    TrackName = "No track playing";
                    ArtistName = "";
                    AlbumName = "";
                    AlbumArtUrl = "";
                    IsPlaying = false;
                    Progress = 0;
                    ProgressText = "0:00 / 0:00";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating now playing: {ex.Message}");
            }
        }

        private string FormatTime(int milliseconds)
        {
            var seconds = milliseconds / 1000;
            var minutes = seconds / 60;
            seconds %= 60;
            return $"{minutes}:{seconds:D2}";
        }

        private void OnConnectionStatusChanged(object? sender, string message)
        {
            StatusMessage = message;
        }

        public void Dispose()
        {
            _updateTimer?.Dispose();
        }
    }
}
