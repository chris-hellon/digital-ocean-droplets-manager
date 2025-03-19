using DigitalOceanManager.Interfaces;

namespace DigitalOceanManager.Services;

public class LoaderService : ILoaderService
{
    public event Func<Task>? OnChange;

    private bool _isLoading;
    public bool IsLoading => _isLoading;

    public async Task Show()
    {
        _isLoading = true;
        if (OnChange != null) await OnChange.Invoke();
    }

    public async Task Hide()
    {
        _isLoading = false;
        if (OnChange != null) await OnChange.Invoke();
    }
}