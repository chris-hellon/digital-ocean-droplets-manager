namespace DigitalOceanManager.Config;

public class SshSettings
{
    public string Username { get; set; } = "root";
    public string PrivateKeyPath { get; set; } = string.Empty;
}