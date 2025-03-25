# DigitalOcean Manager

A Blazor Web App for managing DigitalOcean droplets, configuring Nginx, and handling firewall settings, all without needing to use the terminal.

## Features

- **Droplet Management**: View and manage multiple sites on a single droplet.
- **Automated Nginx Configuration**: Generates and applies Nginx configs for Blazor and ASP.NET apps.
- **SSL Setup**: Automates Let's Encrypt SSL certificate installation.
- **Port Management**: Checks for port conflicts and supports selecting existing ports.
- **Supervisor Process Control**: Manages background services via Supervisor.
- **State Management**: Uses a `LoaderService` for a smooth user experience.
- **UI Components**: Includes generic modals, toasts, breadcrumb navigation, and spinners.

## Getting Started

### Prerequisites

- .NET 9 SDK
- Blazor Server Web App
- DigitalOcean account with API access
- SSH key configured for the droplet
- Nginx installed on the server
- Supervisor installed for managing processes

### Installation

1. **Clone the repository**

   ```sh
   git clone https://github.com/chris-hellon/digital-ocean-droplets-manager.git
   cd digital-ocean-droplets-manager
   ```

2. **Restore dependencies**

   ```sh
   dotnet restore
   ```

3. **Configure app settings**

   Rename `appsettings.example.json` to `appsettings.json` and update DigitalOcean API credentials:

   ```json
   {
   "SshSettings": {
        "Username" : "username",
        "PrivateKeyPath": "path-to-ssh-key"
    },
    "DigitalOceanApi": {
        "PersonalAccessToken": "personal-access-token",
        "BaseUrl": "https://api.digitalocean.com/v2/"
    }
   }
   ```

4. **Run the application**

   ```sh
   dotnet run
   ```

5. Open your browser and navigate to `https://localhost:5001`

## Usage

### Creating a New Website

1. Navigate to **Droplets**.
2. Select a droplet.
3. Click **Create New Website**.
4. Enter domain name and port.
5. Select whether it's a Blazor app or a standard ASP.NET site.
6. Click **Create Website**.
7. The system generates Nginx and Supervisor configurations, applies them, and sets up SSL.

### Managing Existing Websites

- View all sites running on a droplet.
- Update Nginx or Supervisor configurations.
- Restart services.
- Delete sites and clean up configurations.

## Deployment to DigitalOcean

To deploy your website to the DigitalOcean droplet, you can use the **Deploy Script Modal** within the web app:

1. Navigate to the **Edit Website** section of the application.
2. Click on **View Deploy Script**.
3. Copy the generated deployment script using the **Copy to Clipboard** button.
4. You can then run the generated script within your Web App or add it as a GitHub action within your project repo.

## Known Issues & Troubleshooting

- **Nginx Config Not Applying?**
  - Run `sudo nginx -t` to check for errors.
  - Restart Nginx: `sudo systemctl restart nginx`.
- **SSL Issues?**
  - Ensure ports `80` and `443` are open in UFW.
  - Re-run `certbot --nginx -d yourdomain.com`.
- **Site Redirects to Another Website?**
  - Check `/etc/nginx/sites-enabled/` for conflicting configs.

## Contributing

1. Fork the repo
2. Create a new branch (`git checkout -b feature-name`)
3. Commit changes (`git commit -m 'Add new feature'`)
4. Push to branch (`git push origin feature-name`)
5. Create a pull request

## License

This project is licensed under the MIT License.
