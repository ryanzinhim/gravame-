using gravameApi.src.Interface;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using static gravameApi.src.Services.AuthConfigServices;

namespace gravameApi.src.Services
{
    public class AuthServicies : IAuthServicies
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _chaveIntegracao;
        private readonly string _urlAuth;
        private readonly ILogger<AuthServicies> _logger;
        private string _accessToken;
        private DateTime _tokenExpiration;
        private readonly string _senha;
        private const int TOKEN_REFRESH_BUFFER_MINUTES = 5; 



        public AuthServicies(
            string clientId, 
            string clientSecret,
            string chaveIntegracao,
            string urlAuth,
            ILogger<AuthServicies> logger,
            string senha
            )
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _chaveIntegracao = chaveIntegracao;
            _urlAuth = urlAuth;
            _logger = logger;
            _senha = senha;

            var handler = new HttpClientHandler();
            try
            {
                // Obtendo o caminho completo do arquivo .pfx
                string certificatePath = Path.Combine(Directory.GetCurrentDirectory(), _local);

                // Imprimindo o caminho para saber onde o código está procurando o arquivo
                Console.WriteLine("Procurando o certificado em: " + certificatePath);
                var certificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), _local), _senha);


                var caCertificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), _local), _senha);

                handler.ClientCertificates.Add(certificate);
                handler.ClientCertificates.Add(caCertificate);
                _logger.LogInformation("Certificados carregados com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar os certificados.");
                throw;
            }

            _httpClient = new HttpClient(handler);
            _senha = senha;
        }

        public async Task<string> ObterTokenAsync()
        {
            _logger.LogInformation("Iniciando processo para obter token de acesso...");
            try
            {
                if (string.IsNullOrEmpty(_urlAuth))
                {
                    throw new InvalidOperationException("A URL de autenticação não foi configurada.");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("chave", _chaveIntegracao);

                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret)
            });

                var response = await _httpClient.PostAsync(_urlAuth, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao autenticar: {response.StatusCode} - {response.ReasonPhrase}. Detalhes: {errorDetails}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    throw new Exception("Resposta da API não contém um token válido.");
                }

                _accessToken = tokenResponse.AccessToken;
                _tokenExpiration = DateTime.UtcNow.AddSeconds(Convert.ToDouble(tokenResponse.ExpiresIn));

                _logger.LogInformation($"Novo token obtido. Expira em {_tokenExpiration}.");
                return _accessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o token.");
                throw;
            }
        }

        public async Task<string> GetAccessToken()
        {
            _logger.LogInformation("Recuperando token de acesso atual.");

            if (NeedsRefresh())
            {
                _logger.LogInformation("Token próximo da expiração. Solicitando novo token...");
                return await ObterTokenAsync();
            }

            return _accessToken;
        }

        private bool NeedsRefresh()
        {
            if (string.IsNullOrEmpty(_accessToken))
                return true;

            var timeUntilExpiration = _tokenExpiration - DateTime.UtcNow;
            return timeUntilExpiration.TotalMinutes <= TOKEN_REFRESH_BUFFER_MINUTES;
        }
    }
}
