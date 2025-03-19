namespace DigitalOceanManager.Interfaces;

public interface ISshService
{
    string ExecuteCommand(string? host, string command);
}