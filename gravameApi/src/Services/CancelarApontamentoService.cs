using gravameApi.src.Interface;
using RestSharp;
using System.Security.Cryptography.X509Certificates;

namespace gravameApi.src.Services
{
    public class CancelarApontamentoService : ICancelarApontamentoAsync
    {
        private readonly ILogger<CancelarApontamentoService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IAuthServicies _authServicies;
        private readonly RestClient _client;
        private readonly string _accessToken;
        private readonly string _chave;
        private readonly string[] _certificados;
        private readonly string _senha;

        // Adicionando a URL de inclusão como parâmetro
        private readonly string _urlInclusao;

        public CancelarApontamentoService(
            ILogger<CancelarApontamentoService> logger,
            IAuthServicies authServicies,
            string chaveIntegracao,
            string[] certificados,
            string senha,
            string urlInclusao) // Adicionando o parâmetro urlInclusao
        {
            _logger = logger;
            _authServicies = authServicies;
            _chave = chaveIntegracao;
            _certificados = certificados;
            _senha = senha;
            _urlInclusao = urlInclusao; // Inicializando a URL de inclusão

            // Criando o HttpClient com os certificados
            _httpClient = new HttpClient(new HttpClientHandler
            {
                ClientCertificates =
            {
                new X509Certificate2(_certificados[0], _senha), // Primeiro certificado
                new X509Certificate2(_certificados[1], _senha)  // Segundo certificado
            }
            });

            // Configurando o RestClient com a URL do AuthConfig
            var handler = new HttpClientHandler
            {
                ClientCertificates =
            {
                new X509Certificate2(_certificados[0], _senha),
                new X509Certificate2(_certificados[1], _senha)
            }
            };

            var clientOptions = new RestClientOptions(_urlInclusao) // Usando _urlInclusao
            {
                ConfigureMessageHandler = _ => handler
            };

            _client = new RestClient(clientOptions);

            _logger.LogInformation("Certificados carregados com sucesso.");
        }



        public async Task<string> CancelarApontamentoAsync(object payload)
        {
            try
            {
                var accessToken = await _authServicies.ObterTokenAsync(); // Obtém o token antes da requisição

                if (string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogError("Token não foi definido.");
                    throw new InvalidOperationException("Token não foi definido.");
                }

                var request = new RestRequest("api/rsng/v2/apontamentos/transacoes/cancelamentos", Method.Post)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddHeader("Chave", _chave);
                request.AddHeader("Authorization", $"Bearer {accessToken}");
                request.AddJsonBody(payload);

                _logger.LogInformation("Enviando requisição de cancelamento...");
                _logger.LogInformation("Payload enviado: {0}", System.Text.Json.JsonSerializer.Serialize(payload));

                try
                {
                    var response = await _client.ExecuteAsync(request);

                    if (!response.IsSuccessful)
                    {
                        _logger.LogError("Erro no cancelamento: {0} - {1}", response.StatusCode, response.Content);
                        throw new Exception($"Erro na API: {response.StatusCode} - {response.Content}");
                    }

                    _logger.LogInformation("Cancelamento realizado com sucesso. Resposta: {0}", response.Content);
                    return response.Content;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao cancelar apontamento.");
                    throw;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}


