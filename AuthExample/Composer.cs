using System.Security.Claims;
using AuthExample.Abstractions;
using AuthExample.Configurations;
using AuthExample.Database;
using AuthExample.Politics;
using AuthExample.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

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

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetRequiredSection(nameof(JwtOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration
                    .GetRequiredSection(nameof(JwtOptions))
                    .Get<JwtOptions>()!;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtOptions.Secret)),
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var authService =
                            context.HttpContext
                                .RequestServices
                                .GetRequiredService<IAuthService>();

                        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        if (
                            userId is null
                            || context.SecurityToken.ValidTo < DateTime.UtcNow
                            || !authService.VerifyToken(Guid.Parse(userId), context.SecurityToken.UnsafeToString()))
                        {
                            context.Fail("Unauthorized");
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        services.AddScoped<IAuthorizationHandler, PostOwnerRequirementHandler>();
        services.AddHttpContextAccessor();
        services.AddAuthorization(options =>
        {
            var defaultAuthorizationPolicyBuilder =
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
            defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

            options.AddPolicy("PostsOwner", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new PostOwnerRequirement());
            });
        });

        services.AddEndpointsApiExplorer();
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(
                JwtBearerDefaults.AuthenticationScheme,
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                });

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = JwtBearerDefaults.AuthenticationScheme,
                                Type = ReferenceType.SecurityScheme,
                            },
                        },
                        Array.Empty<string>()
                    },
                });
        });

        services.AddValidatorsFromAssembly(typeof(Composer).Assembly);
        services.AddFluentValidationAutoValidation();
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
