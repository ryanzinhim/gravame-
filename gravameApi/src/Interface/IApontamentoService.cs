namespace gravameApi.src.Interface
{
    public interface IApontamentoService
    {
        Task<string> IncluirApontamentoAsync(object payload);
        Task<string> IncluirApontamentoAsync(object payload, string cardId);

    }
}
