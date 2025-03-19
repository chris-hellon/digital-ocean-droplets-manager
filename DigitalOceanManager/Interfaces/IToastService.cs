using DigitalOceanManager.Models;

namespace DigitalOceanManager.Interfaces;

public interface IToastService
{
    /// <summary>
    /// Event triggered when a toast notification is added or removed.
    /// </summary>
    event Func<Task>? OnChange;

    /// <summary>
    /// Retrieves all active toast notifications.
    /// </summary>
    IEnumerable<ToastMessage> GetToasts();

    /// <summary>
    /// Displays a general toast notification.
    /// </summary>
    /// <param name="message">The message content.</param>
    /// <param name="type">The toast type (e.g., "primary", "success", "danger").</param>
    /// <param name="title">The title of the toast.</param>
    /// <param name="duration">The duration in milliseconds before the toast disappears.</param>
    Task ShowToast(string message, string type = "primary", string title = "Notification", int duration = 5000);

    /// <summary>
    /// Displays a success toast notification.
    /// </summary>
    /// <param name="message">The message content.</param>
    /// <param name="title">The title of the toast.</param>
    /// <param name="duration">The duration in milliseconds before the toast disappears.</param>
    Task ShowSuccessToast(string message, string title = "Success", int duration = 5000);

    /// <summary>
    /// Displays an error toast notification.
    /// </summary>
    /// <param name="message">The message content.</param>
    /// <param name="title">The title of the toast.</param>
    /// <param name="duration">The duration in milliseconds before the toast disappears.</param>
    Task ShowErrorToast(string message, string title = "Error", int duration = 5000);

    /// <summary>
    /// Removes a specific toast notification.
    /// </summary>
    /// <param name="toast">The toast message to remove.</param>
    void RemoveToast(ToastMessage toast);
}