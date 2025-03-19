namespace DigitalOceanManager.Models;

public class SiteConfig
{
    public string Domain { get; set; }  = null!;
    public string NginxConfig { get; set; } = null!;
    public string SupervisorConfigFile { get; set; } = null!;
    public string SupervisorConfig { get; set; } = null!;
}