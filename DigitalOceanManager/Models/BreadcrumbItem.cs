namespace DigitalOceanManager.Models;

public class BreadcrumbItem
{
    public required string Label { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsActive => string.IsNullOrEmpty(Url);
    public string? IconClass { get; set; }
}