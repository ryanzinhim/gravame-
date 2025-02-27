using gravameApi.src.Interface;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using gravameApi.src.DataReq;
using Microsoft.EntityFrameworkCore;
using Common.Database;
using gravameApi.src.Repository;

namespace gravameApi.src.Services
{
    public static class ServiceExtensions
    {
        public static void AddAuthConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthConfig:Authority"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = configuration["AuthConfig:Issuer"],
                        ValidAudience = configuration["AuthConfig:Audience"]
                    };
                });

            services.AddAuthorization();
        }

        public static void ConfigureAuthServicies(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthConfigServices>(configuration.GetSection("AuthConfig"));

            services.AddSingleton<IAuthServicies>(provider =>
            {
                var authConfig = provider.GetRequiredService<IOptions<AuthConfigServices>>().Value;
                var logger = provider.GetRequiredService<ILogger<AuthServicies>>();
                return new AuthServicies(
                    authConfig.ClientId,
                    authConfig.ClientSecret,
                    authConfig.ChaveIntegracao,
                    authConfig.UrlAuth,
                    logger,
                    authConfig.Senha
                );
            });
        }

        public static void ConfigureCertificaGravam(this IServiceCollection services)
        {
            services.AddScoped<ICertificaGravam>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ICertificaGravam>>();
                var authConfig = provider.GetRequiredService<IOptions<AuthConfigServices>>(); 
                return new CertificaGravam(logger, authConfig);
            });
        }

        public static void ConfigureApontamentoService(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar a fábrica como um Singleton
            services.AddScoped<ApontamentoServiceFactory>();

            // Registrar o serviço ApontamentoService de forma correta
            services.AddScoped<IApontamentoService>(provider =>
            {
                var factory = provider.GetRequiredService<ApontamentoServiceFactory>();
                return factory.CreateAsync().GetAwaiter().GetResult(); // Bloqueia de forma segura
            });
        }
        public static IServiceCollection AddCancelarApontamentoService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICancelarApontamentoAsync>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<CancelarApontamentoService>>();
                var authServicies = provider.GetRequiredService<IAuthServicies>();
                var config = provider.GetRequiredService<IOptions<AuthConfigServices>>();

                var chaveIntegracao = configuration["AuthConfig:ChaveIntegracao"]
                    ?? null;
                var senha = configuration["AuthConfig:Senha"]
                    ?? null;

                string[] certificados =
                {
            configuration["AuthConfig:Certificados:CertificatePath"]
                ?? null,
            configuration["AuthConfig:Certificados:CertificateCaPath"]
                ?? null
        };

                // Aqui, estamos obtendo a URL de inclusão do arquivo de configuração
                var urlInclusao = configuration["AuthConfig:UrlInclusao"]
                    ?? null;

                return new CancelarApontamentoService(
                    logger,
                    authServicies,
                    chaveIntegracao,
                    certificados,
                    senha,
                    urlInclusao); // Passando a URL de inclusão
            });

            return services;
        }

        public static void AddDatabaseConnection(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<Context>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("gravameApi"))); // Define que as migrações ficam na API
        }

        public static void AddRepositories(this IServiceCollection services)
            => services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<MunicipioService>();
        }

        public static void ConfigureGravame(this IServiceCollection services)
        {
            services.AddScoped<GravameRequestBuilder>();
        }
    }
}

  