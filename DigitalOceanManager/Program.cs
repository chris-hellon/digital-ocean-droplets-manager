using DigitalOceanManager.Components;
using DigitalOceanManager.Config;
using DigitalOceanManager.Interfaces;
using DigitalOceanManager.Services;

namespace DigitalOceanManager;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        
        builder.Services.Configure<SshSettings>(builder.Configuration.GetSection("SshSettings"));
        builder.Services.AddScoped<ISshService, SshService>();
        
        builder.Services.Configure<DigitalOceanApiSettings>(builder.Configuration.GetSection("DigitalOceanApi"));
        builder.Services.AddHttpClient<IDigitalOceanApiClient, DigitalOceanApiClient>();
        
        builder.Services.AddScoped<IDropletStateService, DropletStateService>();
        builder.Services.AddScoped<IToastService, ToastService>();
        builder.Services.AddScoped<ILoaderService, LoaderService>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}