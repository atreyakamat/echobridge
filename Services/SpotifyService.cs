using SpotifyAPI.Web;
using System;
using System.Threading.Tasks;

namespace EchoBridge.Services
{
    public class SpotifyService
    {
        private SpotifyClient? _spotify;
        private readonly SpotifyConfig _config;
        private bool _isAuthenticated;

        public bool IsAuthenticated => _isAuthenticated;

        public event EventHandler<CurrentlyPlaying>? NowPlayingChanged;
        public event EventHandler<string>? ConnectionStatusChanged;

        public SpotifyService()
        {
            _config = SpotifyConfig.Load();
        }

        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_config.ClientId) || string.IsNullOrEmpty(_config.ClientSecret))
                {
                    ConnectionStatusChanged?.Invoke(this, "Spotify credentials not configured");
                    return false;
                }

                var config = SpotifyClientConfig.CreateDefault();
                var request = new ClientCredentialsRequest(_config.ClientId, _config.ClientSecret);
                var response = await new OAuthClient(config).RequestToken(request);

                _spotify = new SpotifyClient(config.WithToken(response.AccessToken));
                _isAuthenticated = true;
                ConnectionStatusChanged?.Invoke(this, "Connected to Spotify");
                return true;
            }
            catch (Exception ex)
            {
                _isAuthenticated = false;
                ConnectionStatusChanged?.Invoke(this, $"Connection failed: {ex.Message}");
                return false;
            }
        }

        public async Task<CurrentlyPlayingTrack?> GetCurrentlyPlayingAsync()
        {
            if (!_isAuthenticated || _spotify == null)
                return null;

            try
            {
                var playing = await _spotify.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());
                
                if (playing?.Item is FullTrack track)
                {
                    return new CurrentlyPlayingTrack
                    {
                        TrackName = track.Name,
                        ArtistName = string.Join(", ", track.Artists.ConvertAll(a => a.Name)),
                        AlbumName = track.Album.Name,
                        AlbumArtUrl = track.Album.Images.Count > 0 ? track.Album.Images[0].Url : string.Empty,
                        IsPlaying = playing.IsPlaying,
                        DurationMs = track.DurationMs,
                        ProgressMs = playing.ProgressMs ?? 0
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting currently playing: {ex.Message}");
            }

            return null;
        }

        public async Task<bool> PlayPauseAsync()
        {
            if (!_isAuthenticated || _spotify == null)
                return false;

            try
            {
                var current = await _spotify.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());
                
                if (current?.IsPlaying == true)
                {
                    await _spotify.Player.PausePlayback();
                }
                else
                {
                    await _spotify.Player.ResumePlayback();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error toggling playback: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SkipToNextAsync()
        {
            if (!_isAuthenticated || _spotify == null)
                return false;

            try
            {
                await _spotify.Player.SkipNext();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error skipping to next: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SkipToPreviousAsync()
        {
            if (!_isAuthenticated || _spotify == null)
                return false;

            try
            {
                await _spotify.Player.SkipPrevious();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error skipping to previous: {ex.Message}");
                return false;
            }
        }
    }

    public class CurrentlyPlayingTrack
    {
        public string TrackName { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string AlbumName { get; set; } = string.Empty;
        public string AlbumArtUrl { get; set; } = string.Empty;
        public bool IsPlaying { get; set; }
        public int DurationMs { get; set; }
        public int ProgressMs { get; set; }
    }
}
