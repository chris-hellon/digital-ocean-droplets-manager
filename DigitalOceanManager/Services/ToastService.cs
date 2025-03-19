using System.Collections.Concurrent;
using DigitalOceanManager.Interfaces;
using DigitalOceanManager.Models;

namespace DigitalOceanManager.Services;

public class ToastService : IToastService
{
    public event Func<Task>? OnChange;
    
    private readonly ConcurrentQueue<ToastMessage> _toasts = new();

    public IEnumerable<ToastMessage> GetToasts() => _toasts.ToArray();

    public async Task ShowToast(string message, string type = "primary", string title = "Notification", int duration = 5000)
    {
        var toast = new ToastMessage(message, type, title, duration, DateTime.Now);
        _toasts.Enqueue(toast);
        await InvokeStateChanged();
        await Task.Delay(100);
        
        _ = Task.Run(async () =>
        {
            await Task.Delay(duration);
            RemoveToast(toast);
        });
    }

    public async Task ShowSuccessToast(string message, string title = "Success", int duration = 5000)
    {
        await ShowToast(message, "success", title, duration);
    }

    public async Task ShowErrorToast(string message, string title = "Error", int duration = 5000)
    {
        await ShowToast(message, "danger", title, duration);
    }

    public void RemoveToast(ToastMessage toast)
    {
        if (_toasts.TryDequeue(out _))
        {
            _ = InvokeStateChanged();
        }
    }

    private async Task InvokeStateChanged()
    {
        if (OnChange is not null)
        {
            await OnChange.Invoke();
        }
    }
}