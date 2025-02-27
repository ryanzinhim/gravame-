using RestSharp;

namespace gravameApi.src.Interface
{
    public interface ICertificaGravam
    {
        void ConfigureCertificates();
        RestClient GetRestClient();
    }
}
