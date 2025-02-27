using System.Text.Encodings.Web;
using System.Text.Unicode;
using gravameApi.Profile;
using gravameApi.src.Interface;
using gravameApi.src.Middleware;
using gravameApi.src.Services;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using gravameApi.src.DataReq;
using System.Net;
using Common.Database;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;

        builder.Services.AddAutoMapper(typeof(MappingProfile));
        builder.Services.AddMemoryCache();
        builder.Services.Configure<AuthConfigServices>(configuration.GetSection("AuthConfig"));
        builder.Services.AddScoped<GraphQLService>();
        builder.Services.ConfigureApontamentoService(builder.Configuration);
        builder.Services.ConfigureApontamentoService(configuration);
        builder.Services.ConfigureAuthServicies(configuration);
        builder.Services.ConfigureCertificaGravam();
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Services.AddRepositories();
        builder.Services.ConfigureServices();
        builder.Services.ConfigureGravame();
        builder.Services.AddDatabaseConnection(configuration);
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });

        if (builder.Environment.IsProduction())
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(address: IPAddress.Any, 5011);
            });


        }

        builder.Services.AddSwaggerGen();
        builder.Services.AddCancelarApontamentoService(builder.Configuration);

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
            try
            {
                dbContext.Database.OpenConnection();
                dbContext.Database.CloseConnection();
                Console.WriteLine("Conexão com o banco de dados estabelecida com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar ao banco de dados: {ex.Message}");
                // Aqui você pode tomar decisões sobre como lidar com a falha de conexão
            }
        }

        app.UseMiddleware<RequestLoggingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (var scope = app.Services.CreateScope())
        {
            var authService = scope.ServiceProvider.GetRequiredService<IAuthServicies>();
            try
            {
                await authService.GetAccessToken();
                Console.WriteLine("Token obtido com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar autenticacao: {ex.Message}");
            }
        }

        using (var scope = app.Services.CreateScope())
        {
            var certificaGravam = scope.ServiceProvider.GetRequiredService<ICertificaGravam>();
            certificaGravam.ConfigureCertificates();
        }

        app.MapControllers();
        app.Run();

    }
}