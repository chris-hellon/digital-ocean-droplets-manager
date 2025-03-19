using System.Text.Json.Serialization;

namespace DigitalOceanManager.Models;

public class Droplet
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("networks")]
    public NetworkInfo Networks { get; set; } = new();

    public string? IpAddress => Networks.V4.FirstOrDefault(x => x.Type == "public")?.IpAddress ?? string.Empty;
    
    public List<FirewallRule> FirewallRules { get; set; } = [];
}