using DigitalOceanManager.Extensions;

namespace DigitalOceanManager.Utils;

public static class StringUtils
{
    public static string GetBlazorNginxConfig(string domain, int port, bool isSubdomain = false) => $@"server {{
    server_name {domain}{(!isSubdomain ? $" www.{domain}" : "")};

    location / {{
        proxy_pass http://localhost:{port}; # Your .NET app's port
        proxy_http_version 1.1;

        # ✅ Proper Header Configuration
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection Upgrade;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;

        # ✅ Increased Buffering for Large Payloads
        proxy_buffer_size 128k;
        proxy_buffers 4 256k;
        proxy_busy_buffers_size 256k;

        # ✅ Increased Timeouts to Prevent Premature Disconnects
        proxy_read_timeout 3600;
        proxy_send_timeout 3600;
    }}

    # ✅ Increased Header Buffers for Large Cookies and Headers
    large_client_header_buffers 4 32k;

    listen 443 ssl;
    ssl_certificate /etc/letsencrypt/live/{domain}/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/{domain}/privkey.pem;
    include /etc/letsencrypt/options-ssl-nginx.conf;
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem;
}}

server {{
    listen 80;
    server_name {domain} www.{domain};
    return 301 https://{domain};
}}";

    public static string GetNonBlazorNginxConfig(string domain, int port, bool isSubdomain = false) => $@"server {{
    server_name {domain}{(!isSubdomain ? $" www.{domain}" : "")};

    location / {{
        proxy_pass http://localhost:{port}; # Your .NET app's port
        proxy_http_version 1.1;

        # ✅ Proper Header Configuration
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;

        # ✅ Increased Buffering for Large Payloads
        proxy_buffer_size 128k;
        proxy_buffers 4 256k;
        proxy_busy_buffers_size 256k;

        # ✅ Increased Timeouts to Prevent Premature Disconnects
        proxy_read_timeout 3600;
        proxy_send_timeout 3600;
    }}

    # ✅ Increased Header Buffers for Large Cookies and Headers
    large_client_header_buffers 4 32k;

    listen 443 ssl;
    ssl_certificate /etc/letsencrypt/live/{domain}/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/{domain}/privkey.pem;
    include /etc/letsencrypt/options-ssl-nginx.conf;
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem;
}}

server {{
    listen 80;
    server_name {domain} www.{domain};
    return 301 https://{domain};
}}";

    public static string GetSupervisorConfig(string programName, int port) => $@"[program:{programName.CapitalizeFirstLetter()}]
    command=/usr/bin/dotnet /var/www/{programName.CapitalizeFirstLetter()}/{programName.CapitalizeFirstLetter()}.dll --urls ""http://localhost:{port}""
    directory=/var/www/{programName}
    autostart=true
    autorestart=true
    stderr_logfile=/var/log/{programName}.err.log
    stdout_logfile=/var/log/{programName}.out.log
    user=www-data
    environment=ASPNETCORE_ENVIRONMENT=Production";
    
    public static string GetTemporaryNginxConfig(string domain, int port, bool isSubdomain = false) => $@"server {{
    listen 80;
    server_name {domain}{(!isSubdomain ? $" www.{domain}" : "")};

    location / {{
        proxy_pass http://localhost:{port};
        proxy_http_version 1.1;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \\$host;
        proxy_set_header X-Real-IP \\$remote_addr;
        proxy_set_header X-Forwarded-For \\$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \\$scheme;
        proxy_cache_bypass \\$http_upgrade;
    }}
}}".EscapeForShell(); 
}