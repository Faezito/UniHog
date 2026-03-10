using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using Z2.Services.Externo;

public class ManterAPIOnlineServicos : BackgroundService
{
    private readonly IHttpClientFactory _http;
    private readonly ILogger<ManterAPIOnlineServicos> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ManterAPIOnlineServicos(
        IHttpClientFactory http,
        ILogger<ManterAPIOnlineServicos> logger,
        IServiceScopeFactory scopeFactory)
    {
        _http = http;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = _http.CreateClient();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var apiService = scope.ServiceProvider
                    .GetRequiredService<IAPIsServicos>();

                var api = await apiService.Obter(null, 151);

                var url = $"{api.Url}/health";

                var auth = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{api.Usuario}:{api.Senha}"));

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", auth);

                var response = await client.GetAsync(url, stoppingToken);

                _logger.LogInformation(
                    "KeepAlive enviado: {time} - Status: {status}",
                    DateTime.Now,
                    response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no KeepAlive");
            }

            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}
