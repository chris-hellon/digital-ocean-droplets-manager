using System.Text.Json.Serialization;

namespace DigitalOceanManager.Models;

public class NetworkInfo
{
    [JsonPropertyName("v4")]
    public List<NetworkDetails> V4 { get; set; } = new();
}