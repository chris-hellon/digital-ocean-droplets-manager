using DigitalOceanManager.Models;

namespace DigitalOceanManager.Interfaces;

public interface IBreadcrumbService
{
    event Func<Task>? OnBreadcrumbsChanged;
    IReadOnlyList<BreadcrumbItem> Breadcrumbs { get; }
    Task SetBreadcrumbs(List<BreadcrumbItem> breadcrumbs);
}