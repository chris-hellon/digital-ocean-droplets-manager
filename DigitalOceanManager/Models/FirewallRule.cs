namespace DigitalOceanManager.Models;

public class FirewallRule
{
    public int RuleNumber { get; set; }
    public string To { get; set; } = string.Empty; 
    public string Action { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
}