using RestSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Azure;
using gravameApi.src.Controllers;

namespace gravameApi.src.Services
{
    public class GraphQLService
    {
        private readonly RestClient _client;
        private readonly string _url = "https://app.pipefy.com/graphql";
        private readonly string _token = ""; // Substitua pelo seu token real 
        private readonly ILogger<GraphQLService> _logger;

        public GraphQLService(ILogger<GraphQLService> logger)
        {
            _client = new RestClient(_url);
            _logger = logger; // Agora o logger é inicializado corretamente
        }


        // Método comum para enviar requisição
        private async Task<string> SendGraphQLRequestAsync(string mutation)
        {
            var request = new RestRequest("/", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_token}");
            request.AddHeader("Content-Type", "application/json");

            var requestBody = new JObject { ["query"] = mutation };
            request.AddJsonBody(requestBody.ToString());


            try
            {
                var response = await _client.ExecuteAsync(request);
                if (!response.IsSuccessful)
                {

                    throw new Exception($"Erro na requisição GraphQL: {response.StatusCode}");
                }
                return response.Content;
            }
            catch (Exception ex)
            {
                // Logar erro na comunicação
                // _logger.LogError($"Erro na comunicação com o Pipefy: {ex.Message}");
                throw new Exception("Erro ao enviar requisição para o Pipefy.", ex);
            }
        }

        // Método para atualizar o campo no Pipefy
        public async Task UpdateCardFieldAsync(string cardId,string field_id, string newValue)
        {
            var escapedNewValue = newValue.Replace("\"", "\\\"");
            var mutation = $@"
                mutation {{
                    updateCardField(input: {{
                        card_id: ""{cardId}"", 
                        field_id: ""numero_do_apontamento"", 
                        new_value: ""{escapedNewValue}""
                    }}) {{
                        clientMutationId 
                    }}
                }}";

            await SendGraphQLRequestAsync(mutation);
            _logger.LogInformation($"Sending GraphQL mutation for card {cardId} with new value: {newValue}");

            var response = await SendGraphQLRequestAsync(mutation);

            _logger.LogInformation($"GraphQL Response received: {response}");

            

            _logger.LogInformation($"Successfully updated card {cardId} field {field_id}");
        }


        public async Task RegistrarRespostaNoPipefy(string cardId, string status, string message)
        {
            // Formatar a mensagem de erro
            var formattedMessage = TransformErrorMessage(message);

            // Escape da mensagem formatada
            var escapedMessage = formattedMessage.Replace("\"", "\\\"");

            var mutation = $@"
        mutation {{
            updateCardField(input: {{
                card_id: ""{cardId}"", 
                field_id: ""response"",
                new_value: ""{status}: {escapedMessage}""
            }}) {{
                clientMutationId 
            }}
        }}";

            var responseString = await SendGraphQLRequestAsync(mutation);

            // Processamento da resposta JSON
            try
            {
                var jsonData = JObject.Parse(responseString);

                // Verifica se o card foi movimentado
                // Aqui você pode processar a resposta conforme a lógica necessária
            }
            catch (Exception ex)
            {
                // Se necessário, logar o erro aqui ou lançar uma exceção personalizada
                throw new Exception("Erro ao processar a resposta do Pipefy.", ex);
            }
        }

        public string TransformErrorMessage(string errorMessage)
        {
            // Verifica se a mensagem contém o erro em formato JSON
            if (errorMessage.Contains("{\"erros\":"))
            {
                // Extrai o JSON da string (a parte após o "UnprocessableEntity - ")
                var jsonStartIndex = errorMessage.IndexOf("{\"erros\":");
                var jsonResponse = errorMessage.Substring(jsonStartIndex);

                try
                {
                    // Parse do JSON
                    var errorObj = JObject.Parse(jsonResponse);
                    var errors = errorObj["erros"]?.ToArray();

                    if (errors != null && errors.Length > 0)
                    {
                        var error = errors[0];
                        var codigo = error["codigo"]?.ToString();
                        var titulo = error["titulo"]?.ToString();
                        var detalhe = error["detalhe"]?.ToString();

                        // Formata a mensagem como você deseja
                        return $"Código: {codigo}\nTítulo: {titulo}\nDetalhe: {detalhe}";
                    }
                }
                catch (Exception ex)
                {
                    // Se ocorrer um erro no processamento do JSON, retorna a mensagem original
                    return $"Erro ao processar a resposta: {ex.Message}";
                }
            }

            return errorMessage; // Retorna a mensagem original se não for o formato esperado
        }
    }
}

