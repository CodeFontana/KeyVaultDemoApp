using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace KeyVaultDemo;

public class App : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<App> _logger;
    private readonly IConfiguration _config;
    private readonly CertificateClient _certificateClient;

    public App(IHostApplicationLifetime hostApplicationLifetime,
               ILogger<App> logger,
               IConfiguration config,
               CertificateClient certificateClient)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
        _config = config;
        _certificateClient = certificateClient;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _hostApplicationLifetime.ApplicationStarted.Register(async () =>
        {
            try
            {
                await Task.Yield(); // https://github.com/dotnet/runtime/issues/36063
                await Task.Delay(1000); // Additional delay for Microsoft.Hosting.Lifetime messages
                await ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception!");
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task ExecuteAsync()
    {
        // GET certifcate
        KeyVaultCertificateWithPolicy certificateWithPolicy = await _certificateClient.GetCertificateAsync(_config.GetValue<string>("Certificate:Name"));
        X509Certificate2 certificateWithPrivateKey = new X509Certificate2(
            certificateWithPolicy.Cer, 
            _config.GetValue<string>("Certificate:Password"), 
            X509KeyStorageFlags.MachineKeySet);
        _logger.LogInformation(certificateWithPrivateKey.Subject);
    }
}
