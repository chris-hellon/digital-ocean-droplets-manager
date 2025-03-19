using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using DigitalOceanManager.Config;
using DigitalOceanManager.Interfaces;
using DigitalOceanManager.Models;

namespace DigitalOceanManager.Services;

public class DigitalOceanApiClient : IDigitalOceanApiClient
{
    private readonly HttpClient _httpClient;

    public DigitalOceanApiClient(HttpClient httpClient, IOptions<DigitalOceanApiSettings> apiSettings)
    {
        _httpClient = httpClient;
        var apiSettings1 = apiSettings.Value;

        _httpClient.BaseAddress = new Uri(apiSettings1.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiSettings1.PersonalAccessToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<Droplet>> GetDropletsAsync()
    {
        var response = await _httpClient.GetAsync("droplets");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DropletResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result?.Droplets ?? [];
    }
}