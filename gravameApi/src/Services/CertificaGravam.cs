using gravameApi.src.Interface;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace gravameApi.src.Services
{
    public class CertificaGravam : ICertificaGravam
    {
        private readonly ILogger<ICertificaGravam> _logger;
        private readonly string _certificatePath;
        private readonly string _certificateCaPath;
        private readonly string _senha;
        private RestClient _client;

        public CertificaGravam(ILogger<ICertificaGravam> logger, IOptions<AuthConfigServices> config)
        {
            _logger = logger;

            var certificados = config.Value.Certificados;

            if (certificados == null)
            {
                _logger.LogError("Configuração de certificados não fornecida.");
                throw new ArgumentException("Configuração de certificados não fornecida.");
            }

            _senha = config.Value.Senha;

            _certificatePath = Path.Combine(Directory.GetCurrentDirectory(), certificados.CertificatePath);

            _certificateCaPath = Path.Combine(Directory.GetCurrentDirectory(), certificados.CertificateCaPath);

            if (string.IsNullOrWhiteSpace(_certificatePath) || string.IsNullOrWhiteSpace(_certificateCaPath))
            {
                _logger.LogError("Os caminhos dos certificados não foram configurados corretamente.");
                throw new ArgumentException("Os caminhos dos certificados não foram configurados corretamente.");
            }
        }

        public void ConfigureCertificates()
        {
            try
            {
                if (!File.Exists(_certificatePath))
                {
                    _logger.LogError($"Certificado não encontrado: {_certificatePath}");
                    throw new FileNotFoundException($"Certificado não encontrado: {_certificatePath}");
                }

                if (!File.Exists(_certificateCaPath))
                {
                    _logger.LogError($"Certificado CA não encontrado: {_certificateCaPath}");
                    throw new FileNotFoundException($"Certificado CA não encontrado: {_certificateCaPath}");
                }

                var certificate = new X509Certificate2(_certificatePath, _senha);
                var caCertificate = new X509Certificate2(_certificateCaPath, _senha);

                var options = new RestClientOptions("https://api-revolucaosng.b3.com.br/")
                {
                    ClientCertificates = new X509CertificateCollection { certificate, caCertificate }
                };

                _client = new RestClient(options);
                _logger.LogInformation("Certificados configurados com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao configurar certificados.");
                throw;
            }
        }

        public RestClient GetRestClient()
        {
            if (_client == null)
            {
                _logger.LogError("Cliente REST não configurado. Execute ConfigureCertificates primeiro.");
                throw new InvalidOperationException("Cliente REST não configurado. Execute ConfigureCertificates primeiro.");
            }

            _logger.LogInformation("Retornando cliente REST configurado.");
            return _client;
        }
    }
}
