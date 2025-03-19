using System.Text.Json.Serialization;

namespace DigitalOceanManager.Models;

public class NetworkDetails
{
    [JsonPropertyName("ip_address")]
    public string? IpAddress { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}