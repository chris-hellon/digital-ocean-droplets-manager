using DigitalOceanManager.Models;

namespace DigitalOceanManager.Interfaces;

public interface IDigitalOceanApiClient
{
    Task<List<Droplet>> GetDropletsAsync();
}