namespace gravameApi.src.Interface
{
    public interface IAuthServicies
    {
        Task<string> ObterTokenAsync();
        Task<string> GetAccessToken();


    }
}
