using AuthExample.Abstractions;
using AuthExample.Configurations;
using AuthExample.Database;
using AuthExample.Services;

namespace AuthExample;

public static class Composer
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetRequiredSection(nameof(DatabaseOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddDbContext<AppDbContext>();
        services.AddEndpointsApiExplorer();
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();

        return services;
    }

    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services
    )
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPostRepository, PostRepository>();

        return services;
    }
}
