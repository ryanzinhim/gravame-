using gravameApi.src.Interface;
using gravameApi.src.Services;
using RestSharp;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using static gravameApi.src.Services.AuthConfigServices;

public class ApontamentoService : IApontamentoService
{
    private readonly ILogger<ApontamentoService> _logger;
    private readonly RestClient _client;
    private readonly string _uri;
    private readonly string _token;
    private readonly string _chave;
    private readonly GraphQLService _graphQLService; 
    private readonly string _senha ;

    public ApontamentoService(string UrlInclusao, string token, string ChaveIntegracao, ILogger<ApontamentoService> logger, GraphQLService graphQLService)
    {

        var certificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), _local), _senha);

        var caCertificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), _local), _senha);

        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(certificate);
        handler.ClientCertificates.Add(caCertificate);

        var clientOptions = new RestClientOptions(UrlInclusao)
        {
            ConfigureMessageHandler = _ => handler
        };

        _client = new RestClient(clientOptions);
        _token = token;
        _chave = ChaveIntegracao;
        _logger = logger;
        _graphQLService = graphQLService;

        _logger.LogInformation("ApontamentoService inicializado com URL: {UrlInclusao}.", UrlInclusao);
    }

    // Método original
    public async Task<string> IncluirApontamentoAsync(object payload)
    {
        return await IncluirApontamentoAsync(payload, null); // Chama o método sobrecarregado com cardId como null
    }

    public async Task<string> IncluirApontamentoAsync(object payload, string cardId)
    {
        if (string.IsNullOrEmpty(_token))
        {
            _logger.LogError("Token não foi definido.");
            throw new InvalidOperationException("Token não foi definido.");
        }

        var jsonPayload = JsonSerializer.Serialize(payload);
        var request = new RestRequest(_uri, Method.Post)
        {
            RequestFormat = DataFormat.Json
        };

        request.AddHeader("Chave", _chave);
        request.AddHeader("Authorization", $"Bearer {_token}");
        request.AddJsonBody(payload);


        try
        {
            // Enviando a requisição
            var response = await _client.ExecuteAsync(request);
            var responseText = response.Content;

            _logger.LogInformation("Status Code: {StatusCode}", response.StatusCode);
            _logger.LogInformation("Response: {Content}", responseText);

            if (!response.IsSuccessful)
            {
                _logger.LogError("Erro na requisição: {StatusCode}, Resposta: {Content}",
                                  response.StatusCode, responseText);
                throw new Exception($"Erro na API: {response.StatusCode} - {responseText}");
            }


            return responseText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao incluir apontamento.");

            throw;
        }
    }
}
