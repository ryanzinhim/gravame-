using gravameApi.src.Interface;

namespace gravameApi.src.Services
{
    public class ApontamentoServiceFactory
    {
        private readonly IServiceProvider _provider;
        private readonly IConfiguration _configuration;

        public ApontamentoServiceFactory(IServiceProvider provider, IConfiguration configuration)
        {
            _provider = provider;
            _configuration = configuration;
        }

        public async Task<IApontamentoService> CreateAsync()
        {
            var logger = _provider.GetRequiredService<ILogger<ApontamentoService>>();
            var authService = _provider.GetRequiredService<IAuthServicies>();
            var token = await authService.GetAccessToken();
            var UrlInclusao = _configuration["AuthConfig:UrlInclusao"];
            var ChaveIntegracao = _configuration["AuthConfig:ChaveIntegracao"];
            var graphQLService = _provider.GetRequiredService<GraphQLService>();

            return new ApontamentoService(UrlInclusao, token, ChaveIntegracao, logger, graphQLService);
        }
    }
}
