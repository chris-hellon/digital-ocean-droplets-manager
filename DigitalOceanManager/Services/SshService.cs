using DigitalOceanManager.Config;
using DigitalOceanManager.Interfaces;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace DigitalOceanManager.Services;

public class SshService : ISshService
{
    private readonly string _username;
    private readonly string _privateKeyPath;
    private readonly IToastService _toastService;

    public SshService(IOptions<SshSettings> sshSettings, IToastService toastService)
    {
        _toastService = toastService;
        _privateKeyPath = sshSettings.Value.PrivateKeyPath;
        _username = sshSettings.Value.Username;
    }

    public string ExecuteCommand(string? host, string command)
    {
        try
        {
            using var client = new SshClient(host, _username, new PrivateKeyFile(_privateKeyPath));
            client.Connect();

            if (!client.IsConnected)
                throw new Exception($"SSH connection failed to {host}");
            
            var fullCommand = $"{command} 2>&1";
            var cmd = client.RunCommand(fullCommand);
            var result = cmd.Result.Trim();

            client.Disconnect();

            Console.WriteLine(string.IsNullOrWhiteSpace(result)
                ? $"Command executed on {host}, but returned no output: {command}"
                : $"Command executed on {host}: {command}\nOutput: {result}");

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing SSH command on {host}: {command}\nException: {ex.Message}");
            _toastService.ShowErrorToast($"Error executing SSH command on {host}: {command}\nException: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}