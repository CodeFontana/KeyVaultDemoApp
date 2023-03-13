using Azure.Core;
using Azure.Identity;
using KeyVaultDemo;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.Services.AddAzureClients(config => 
{
    // https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/extensions/Microsoft.Extensions.Azure/README.md
    config.AddCertificateClient(new Uri(builder.Configuration.GetValue<string>("KeyVault:VaultUri")));
    config.UseCredential(new EnvironmentCredential());
    config.ConfigureDefaults(builder.Configuration.GetSection("AzureId"));
    config.ConfigureDefaults(options => options.Retry.Mode = RetryMode.Exponential);
});
builder.Services.AddHostedService<App>();
IHost app = builder.Build();
app.Run();
