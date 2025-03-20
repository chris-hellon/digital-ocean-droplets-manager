using DigitalOceanManager.Interfaces;
using DigitalOceanManager.Models;

namespace DigitalOceanManager.Services;

public class BreadcrumbService : IBreadcrumbService
{
    public event Func<Task>? OnBreadcrumbsChanged;
    private List<BreadcrumbItem> _breadcrumbs = [];

    public IReadOnlyList<BreadcrumbItem> Breadcrumbs => _breadcrumbs;

    public async Task SetBreadcrumbs(List<BreadcrumbItem> breadcrumbs)
    {
        if (breadcrumbs.SequenceEqual(_breadcrumbs) && breadcrumbs.Count != 0)
            return;

        _breadcrumbs = breadcrumbs;
        
        if (_breadcrumbs.Count == 0)
            _breadcrumbs.Insert(0, new BreadcrumbItem {Label = "Droplets", IconClass = "fa-droplet"});
        else
            _breadcrumbs.Insert(0, new BreadcrumbItem {Label = "Droplets", Url = "/", IconClass = "fa-droplet"});
        
        await NotifyBreadcrumbsChanged();
    }

    private async Task NotifyBreadcrumbsChanged()
    {
        if (OnBreadcrumbsChanged is not null)
        {
            await OnBreadcrumbsChanged.Invoke();
        }
    }
}