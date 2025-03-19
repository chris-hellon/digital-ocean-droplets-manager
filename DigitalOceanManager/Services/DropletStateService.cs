using DigitalOceanManager.Interfaces;
using DigitalOceanManager.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DigitalOceanManager.Services;

public class DropletStateService : IDropletStateService
{
    private readonly IDigitalOceanApiClient _apiClient;
    private readonly ISshService _sshService;
    private readonly ProtectedSessionStorage _sessionStorage;

    public DropletStateService(IDigitalOceanApiClient apiClient, ISshService sshService, ProtectedSessionStorage sessionStorage)
    {
        _apiClient = apiClient;
        _sshService = sshService;
        _sessionStorage = sessionStorage;
    }

    public async Task<List<Droplet>> GetDropletsAsync()
    {
        var sessionData = await _sessionStorage.GetAsync<List<Droplet>>("droplets");
        
        if (sessionData is {Success: true, Value: not null})
        {
            return sessionData.Value;
        }
        
        var droplets = await _apiClient.GetDropletsAsync();
        
        await _sessionStorage.SetAsync("droplets", droplets);

        return droplets;
    }

    public async Task<Droplet?> GetDropletById(int id)
    {
        var droplets = await GetDropletsAsync();
        var droplet = droplets.FirstOrDefault(d => d.Id == id);
        
        if (droplet is not null)
            droplet.FirewallRules = GetFirewallRules(droplet);

        return droplet;
    }

    public async Task LoadAllSitesAsync(Droplet droplet)
    {
        var sessionData = await _sessionStorage.GetAsync<Dictionary<string, SiteConfig>>($"sites_{droplet.Id}");

        if (sessionData is {Success: true, Value: not null})
        {
            return;
        }

        Console.WriteLine($"🔄 Loading all sites for Droplet {droplet.Name}");

        var siteResults = _sshService.ExecuteCommand(droplet.IpAddress, "ls /etc/nginx/sites-available/");
        var siteNames = siteResults.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var siteConfigs = new Dictionary<string, SiteConfig>();

        foreach (var site in siteNames)
        {
            var nginxConfig = _sshService.ExecuteCommand(droplet.IpAddress, $"cat /etc/nginx/sites-available/{site} 2>/dev/null");

            var supervisorFiles = _sshService.ExecuteCommand(droplet.IpAddress, "ls /etc/supervisor/conf.d/ 2>/dev/null")
                .Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var matchedSupervisorFile = supervisorFiles
                .FirstOrDefault(file => site.Contains(file.Replace(".conf", ""), StringComparison.OrdinalIgnoreCase))
                ?? string.Empty;

            var supervisorConfig = !string.IsNullOrWhiteSpace(matchedSupervisorFile)
                ? _sshService.ExecuteCommand(droplet.IpAddress, $"cat /etc/supervisor/conf.d/{matchedSupervisorFile} 2>/dev/null")
                : "Supervisor Config Not Found";

            siteConfigs[site] = new SiteConfig
            {
                Domain = site,
                NginxConfig = nginxConfig,
                SupervisorConfigFile = matchedSupervisorFile,
                SupervisorConfig = supervisorConfig
            };
        }
        
        await _sessionStorage.SetAsync($"sites_{droplet.Id}", siteConfigs);

        Console.WriteLine($"✅ Loaded {siteConfigs.Count} site configurations for {droplet.Name}.");
    }

    public async Task<SiteConfig?> GetSiteConfigAsync(int dropletId, string domain)
    {
        var sessionData = await _sessionStorage.GetAsync<Dictionary<string, SiteConfig>>($"sites_{dropletId}");
        return sessionData is {Success: true, Value: not null} ? sessionData.Value.GetValueOrDefault(domain) : null;
    }
    
    public async Task<List<string>> GetAllSitesAsync(int dropletId)
    {
        var sessionData = await _sessionStorage.GetAsync<Dictionary<string, SiteConfig>>($"sites_{dropletId}");
        if (sessionData is not {Success: true, Value: not null}) return [];

        var keys= sessionData.Value.Keys.ToList();
        return keys;
    }

    public async Task UpdateSiteConfigAsync(int dropletId, string domain, string nginxConfig, string supervisorConfig, string supervisorConfigFile)
    {
        var sessionData = await _sessionStorage.GetAsync<Dictionary<string, SiteConfig>>($"sites_{dropletId}");
        var siteConfigs = sessionData is {Success: true, Value: not null} ? sessionData.Value : new Dictionary<string, SiteConfig>();

        siteConfigs[domain] = new SiteConfig
        {
            Domain = domain,
            NginxConfig = nginxConfig,
            SupervisorConfig = supervisorConfig,
            SupervisorConfigFile = supervisorConfigFile
        };

        await _sessionStorage.SetAsync($"sites_{dropletId}", siteConfigs);

        Console.WriteLine($"✅ Updated session storage for {domain} in Droplet {dropletId}.");
    }

    public async Task DeleteSiteConfigAsync(int dropletId, string domain)
    {
        var sessionData = await _sessionStorage.GetAsync<Dictionary<string, SiteConfig>>($"sites_{dropletId}");
        var siteConfigs = sessionData is {Success: true, Value: not null} ? sessionData.Value : new Dictionary<string, SiteConfig>();
        
        siteConfigs.Remove(domain);
        
        await _sessionStorage.SetAsync($"sites_{dropletId}", siteConfigs);
    }
    
    public List<FirewallRule> GetFirewallRules(Droplet droplet)
    {
        var firewallRules = new List<FirewallRule>();
        var rulesOutput = _sshService.ExecuteCommand(droplet.IpAddress, "ufw status numbered");
        
        var lines = rulesOutput.Split('\n');
        foreach (var line in lines)
        {
            var parts = line.Trim().Split(']');

            if (parts.Length <= 1) continue;
            if (!int.TryParse(parts[0].Trim('[', ' '), out var ruleNum)) continue;

            var ruleDetails = parts[1].Trim().Split(["ALLOW IN", "DENY IN"], StringSplitOptions.None);
            if (ruleDetails.Length <= 1) continue;
            
            var action = parts[1].Contains("ALLOW IN") ? "ALLOW IN" : "DENY IN";
            var ruleParts = ruleDetails[0].Trim().Split([' '], StringSplitOptions.RemoveEmptyEntries);

            var to = ruleParts.Length > 0 ? ruleParts[0].Trim() : "Any";
            var from = ruleDetails.Length > 1 ? ruleDetails[1].Trim() : "Anywhere";

            firewallRules.Add(new FirewallRule
            {
                RuleNumber = ruleNum,
                To = to,
                Action = action,
                From = from
            });
        }

        return firewallRules;
    }
    
    public string GenerateDeployScript(Droplet? droplet, string? programName, string domain)
    {
        var serverIp = droplet?.IpAddress ?? "UNKNOWN_IP";
        programName = string.IsNullOrWhiteSpace(programName) ? domain.Split('.')[0] : programName;

        return $@"#!/bin/bash

# Step 1: Build and Publish the Application
dotnet publish -c Release -o ./publish

# Step 2: Remove all files on the server
ssh root@{serverIp} ""rm -rf /var/www/{programName}/*""

# Step 3: Copy the new files to the server
rsync -av ./publish/ root@{serverIp}:/var/www/{programName}

# Step 4: Restart the application using Supervisor
ssh root@{serverIp} ""sudo supervisorctl restart {programName}""

# Step 5: Delete the local publish folder
rm -rf ./publish";
    }
}