using DigitalOceanManager.Models;

namespace DigitalOceanManager.Interfaces;

public interface IDropletStateService
{
    Task<List<Droplet>> GetDropletsAsync();
    Task<Droplet?> GetDropletById(int id);

    /// <summary>
    /// Loads all site configurations for a droplet **and caches the results in session storage**.
    /// </summary>
    Task LoadAllSitesAsync(Droplet droplet);
    
    /// <summary>
    /// Retrieves the cached configuration for a specific site.
    /// </summary>
    Task<SiteConfig?> GetSiteConfigAsync(int dropletId, string domain);
    
    /// <summary>
    /// Retrieves **all** cached site configurations for a droplet.
    /// </summary>
    Task<List<string>> GetAllSitesAsync(int dropletId);


    Task UpdateSiteConfigAsync(int dropletId, string domain, string nginxConfig, string supervisorConfig, string supervisorConfigFile);

    List<FirewallRule> GetFirewallRules(Droplet droplet);

    Task DeleteSiteConfigAsync(int dropletId, string domain);

    string GenerateDeployScript(Droplet? droplet, string? programName, string domain);
}