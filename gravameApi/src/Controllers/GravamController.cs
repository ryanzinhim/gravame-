using Microsoft.AspNetCore.Mvc;
using gravameApi.src.Interface;
using gravameApi.src.DataReq;
using gravameApi.src.Models.DTOs;
using System.Globalization;
using gravameApi.src.Services;
using Microsoft.IdentityModel.Tokens;
using Azure;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Security.AccessControl;
using Microsoft.Extensions.Logging;
using static gravameApi.src.Controllers.GravamController;
using gravameApi.src.Models;
using System.Reflection;

namespace gravameApi.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GravamController : Controller
    {
        private readonly IApontamentoService _apontamentoService;
        private readonly MunicipioService _municipioService;
        private readonly GraphQLService _graphQLService;
        private readonly ICancelarApontamentoAsync _cancelaApontamentoService;
        private readonly ILogger<GravamController> _logger;
        private readonly GravameRequestBuilder _gravameRequestBuilder;

        public GravamController(IApontamentoService apontamentoService, MunicipioService municipioService, GraphQLService graphQLService, ICancelarApontamentoAsync cancelaApontamentoService, ILogger<GravamController> logger, GravameRequestBuilder gravameRequestBuilder)
        {
            _apontamentoService = apontamentoService;
            _municipioService = municipioService;
            _graphQLService = graphQLService;
            _cancelaApontamentoService = cancelaApontamentoService;
            _logger = logger;
            _gravameRequestBuilder = gravameRequestBuilder;


        }

        [HttpPost("incluir-gravam")]
        public async Task<IActionResult> IncluirGravam([FromBody] DataRequest dataRequest)
        {
            var cardId = dataRequest.Data.Contrato.CardId;

            if (dataRequest?.Data == null)
            {
                return BadRequest(new { message = "Dados do contrato, financiado e veículo são obrigatórios." });
            }

            var campoFaltante = VerificarCamposNulos(dataRequest.Data);

            if (!string.IsNullOrEmpty(campoFaltante) && campoFaltante == "Financiado.numDddTelefone")
            {
                string mensagemErro = $"telefone do cliente é obrigatorio";

                _logger.LogWarning(mensagemErro);
                await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", mensagemErro);

                return BadRequest(new { message = mensagemErro });
            }
            if (!string.IsNullOrEmpty(campoFaltante))
            {
                string mensagemErro = $"O campo {campoFaltante} é obrigatório.";

                _logger.LogWarning(mensagemErro);
                await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", mensagemErro);

                return BadRequest(new { message = mensagemErro });
            }
            {

            }
            if (string.IsNullOrEmpty(cardId))
            {
                return BadRequest(new { message = "CardId é obrigatório." });
            }

            try
            {
                dataRequest.Data.Financiado.codMunicipioEndereco = await ObterCodigoTOM(dataRequest.Data.Financiado.nomeMunicipioEndereco);

                bool converterPlaca = false;
                string response = string.Empty;
                int maxTentativas = 2; // Limita o número de tentativas
                int tentativaAtual = 0;

                string placaLimpa = dataRequest.Data.Veiculo.numPlaca.Replace(".", "").Replace("-", "").Replace("/", "").Substring(0, 7);

                while (tentativaAtual < maxTentativas)
                {
                    // Cria os filtros com base no estado atual de converterPlaca

                    var filtros = _gravameRequestBuilder.Build(dataRequest, converterPlaca, placaLimpa);
                    Console.WriteLine(dataRequest);
                    try
                    {                       
                        response = await _apontamentoService.IncluirApontamentoAsync(filtros);

                        _logger.LogInformation($"Tentativa {tentativaAtual + 1}: Resposta com placa {(converterPlaca ? "convertida" : "original")}: {response}");

                        if (response.Contains("sucesso", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                    catch (ArgumentNullException argEx)
                    {
                        await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", $"Erro: {argEx.Message}");
                        _logger.LogError($"ArgumentNullException na tentativa {tentativaAtual + 1}: {argEx.Message}");

                        if (tentativaAtual == maxTentativas - 1)
                        {
                            return StatusCode(500, new { message = $"Erro interno: {argEx.Message}" });
                        }
                    }
                    catch (FormatException formatEx)
                    {
                        await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", $"Erro de formatação: {formatEx.Message}");
                        _logger.LogError($"FormatException na tentativa {tentativaAtual + 1}: {formatEx.Message}");

                        if (tentativaAtual == maxTentativas - 1)
                        {
                            return BadRequest(new { message = $"Erro de formatação: {formatEx.Message}" });
                        }
                    }
                    catch (InvalidOperationException invOpEx)
                    {
                        await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", $"Operação inválida: {invOpEx.Message}");
                        _logger.LogError($"InvalidOperationException na tentativa {tentativaAtual + 1}: {invOpEx.Message}");

                        if (tentativaAtual == maxTentativas - 1)
                        {
                            return BadRequest(new { message = $"Operação inválida: {invOpEx.Message}" });
                        }
                    }
                    catch (Exception ex)
                    {
                        await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", $"Erro inesperado: {ex.Message}");
                        _logger.LogError($"Exception inesperada na tentativa {tentativaAtual + 1}: {ex.Message}");

                        if (tentativaAtual == maxTentativas - 1)
                        {
                            return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
                        }
                    }

                    // Prepara para próxima tentativa
                    converterPlaca = true;
                    tentativaAtual++;
                }

                // Se todas as tentativas falharem
                if (tentativaAtual >= maxTentativas)
                {
                    await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", "Falha em todas as tentativas de incluir apontamento");
                    _logger.LogError("Falha em todas as tentativas de incluir apontamento");
                    return StatusCode(500, new { message = "Erro ao processar apontamento após múltiplas tentativas" });
                }

                // Processa a resposta final
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var resultado = apiResponse.Data;
                _logger.LogInformation($"NumApontamento antes de atualizar: {resultado.NumApontamento}");

                await _graphQLService.UpdateCardFieldAsync(cardId, "numero_do_apontamento", resultado.NumApontamento.ToString());
                await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", "gravame incluído");


                return Ok(new { message = "Inclusão de Apontamento realizada com sucesso.", numApontamento = resultado.NumApontamento });
            }
            catch (Exception ex)
            {
                await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", $"Erro inesperado final: {ex.Message}");
                return StatusCode(500, new { message = $"Erro interno final: {ex.Message}" });
            }
        }
        private string VerificarCamposNulos(object obj, string prefix = "")
        {
            if (obj == null)
            {
                _logger.LogWarning($"Campo obrigatório ausente: {prefix}");
                return prefix;
            }

            Type tipo = obj.GetType();
            PropertyInfo[] propriedades = tipo.GetProperties();

            foreach (var propriedade in propriedades)
            {
                object valor = propriedade.GetValue(obj);

                // Verifica se é null ou string vazia
                if (valor == null || (valor is string str && string.IsNullOrWhiteSpace(str)))
                {
                    string campoFaltante = string.IsNullOrEmpty(prefix) ? propriedade.Name : $"{prefix}.{propriedade.Name}";
                    _logger.LogWarning($"Campo obrigatório ausente: {campoFaltante}");
                    return campoFaltante;
                }

                if (!propriedade.PropertyType.IsPrimitive && propriedade.PropertyType != typeof(string))
                {
                    string resultado = VerificarCamposNulos(valor, string.IsNullOrEmpty(prefix) ? propriedade.Name : $"{prefix}.{propriedade.Name}");
                    if (!string.IsNullOrEmpty(resultado))
                    {
                        return resultado;
                    }
                }
            }
            return null;
        }
        public async Task<long> ObterCodigoTOM(string nomeMunicipio)
        {
            return await _municipioService.ObterCodigoTOM(nomeMunicipio);
        }
    }
}


[ApiController]
[Route("api/[controller]")]
public class MunicipioController : ControllerBase
{
    private readonly MunicipioService _service;
    private readonly MunicipioService _municipioService;

    public MunicipioController(MunicipioService service, MunicipioService municipioService)
    {
        _service = service;
        _municipioService = municipioService;

    }

    [HttpGet("codigo/{nomeMunicipio}")]
    public async Task<IActionResult> ObterCodigoTOM(string nomeMunicipio)
    {
        try
        {
            var codigoTOM = await _municipioService.BuscarCodigoTOM(nomeMunicipio);
            return Ok(new { CodigoTOM = codigoTOM });
        }
        catch (Exception ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}

[ApiController]
[Route("api/[controller]")]
public class CancelaController : ControllerBase
{
    private readonly ICancelarApontamentoAsync _cancelaApontamentoService;
    private readonly GraphQLService _graphQLService;

    public CancelaController(ICancelarApontamentoAsync cancelaApontamentoService, GraphQLService graphQLService)
    {
        _cancelaApontamentoService = cancelaApontamentoService;
        _graphQLService = graphQLService;

    }

    [HttpPost("cancelar-gravam")]
    public async Task<IActionResult> CancelarGravam([FromBody] DataCance dataCance)
    {
        if (dataCance == null || dataCance.Data == null)
        {
            return BadRequest(new { message = "Dados do contrato, financiado e veículo são obrigatórios." });
        }
        var cardId = dataCance.Data.dadosValidacao.cardID;

        try
        {
            // Criar os filtros e chamar o serviço
            var filtros = CriarFiltro(dataCance);
            var resultado = await _cancelaApontamentoService.CancelarApontamentoAsync(filtros);
            await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", "Apontamento cancelado com sucesso!");

            return Ok(new { message = "Gravam (Apontamento) cancelado com sucesso!", data = resultado });
        }
        catch (ArgumentNullException argEx)
        {
            await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", $"Erro: {argEx.Message}");
            return StatusCode(500, new { message = $"Erro interno: {argEx.Message}" });
        }
        catch (FormatException formatEx)
        {
            await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", $"Erro de formatação: {formatEx.Message}");
            return BadRequest(new { message = $"Erro de formatação: {formatEx.Message}" });
        }
        catch (InvalidOperationException invOpEx)
        {
            await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", $"Operação inválida: {invOpEx.Message}");
            return BadRequest(new { message = $"Operação inválida: {invOpEx.Message}" });
        }
        catch (Exception ex)
        {
            await _graphQLService.RegistrarRespostaNoPipefy(cardId, "response", $"Erro inesperado: {ex.Message}");
            return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
        }

    }

    private object CriarFiltro(DataCance dataCance)
    {
        // Corrigindo a criação do objeto
        return new
        {
            data = new
            {
                dadosValidacao = new
                {
                    numChassiVeiculo = dataCance.Data.dadosValidacao.numChassiVeiculo,
                    numDocumentoFinanciado = dataCance.Data.dadosValidacao.numDocumentoFinanciado.Replace(".", "").Replace("-", "").Replace("/", ""),
                    numApontamento = dataCance.Data.dadosValidacao.numApontamento,
                    cardID = dataCance.Data.dadosValidacao.cardID
                }
            }
        };
    }
}