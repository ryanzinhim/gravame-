using gravameApi.src.Interface;
using Microsoft.EntityFrameworkCore;

public class MunicipioService
{
    private readonly IRepositoryWrapper _repository;
    private readonly ILogger<MunicipioService> _logger;

    public MunicipioService(IRepositoryWrapper repository, ILogger<MunicipioService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<long> ObterCodigoTOM(string nomeMunicipio)
    {
        if (string.IsNullOrWhiteSpace(nomeMunicipio))
        {
            throw new ArgumentNullException(nameof(nomeMunicipio), "O nome do município não pode ser nulo ou vazio.");
        }

        try
        {
            long codigoTOM = await BuscarCodigoTOM(nomeMunicipio) ?? 6001; // Define 6001 se for null

            if (codigoTOM > 0)
            {
                return codigoTOM;
            }

            _logger.LogWarning($"O código TOM retornado não é válido: {codigoTOM}");
            return 6001; // Código padrão
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter código TOM para o município {Municipio}", nomeMunicipio);
            throw new Exception("Erro ao processar o código TOM.", ex);
        }
    }

    public async Task<long?> BuscarCodigoTOM(string nomeMunicipio)
    {
        long? codigoTom = await _repository.MunicipiosTom.FindByCondition(mt => mt.Municipio_tom == nomeMunicipio).Select(mt => mt.Codigo_tom).FirstOrDefaultAsync();

        return codigoTom;
    }
}
