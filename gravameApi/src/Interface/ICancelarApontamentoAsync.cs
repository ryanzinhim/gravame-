namespace gravameApi.src.Interface
{
    public interface ICancelarApontamentoAsync
    {
        Task<string> CancelarApontamentoAsync(object payload);

    }
}
