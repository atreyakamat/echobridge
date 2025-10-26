using Newtonsoft.Json;
using System;
using System.IO;

namespace EchoBridge.Services
{
    public class SpotifyConfig
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; } = string.Empty;

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; } = string.Empty;

        [JsonProperty("redirect_uri")]
        public string RedirectUri { get; set; } = "http://localhost:5000/callback";

        public static SpotifyConfig Load(string configPath = "spotify_config.json")
        {
            try
            {
                if (File.Exists(configPath))
                {
                    var json = File.ReadAllText(configPath);
                    return JsonConvert.DeserializeObject<SpotifyConfig>(json) ?? new SpotifyConfig();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading Spotify config: {ex.Message}");
            }

            return new SpotifyConfig();
        }

        public void Save(string configPath = "spotify_config.json")
        {
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving Spotify config: {ex.Message}");
            }
        }
    }
}
