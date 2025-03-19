namespace DigitalOceanManager.Interfaces;

public interface ILoaderService
{
    event Func<Task>? OnChange;
    bool IsLoading { get; } 
    Task Show();
    Task Hide();
}