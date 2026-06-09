using Convertly.Application.Auth;
using Convertly.Application.Common;
using Convertly.Application.Conversions;
using Convertly.Application.Files;
using Convertly.Application.Subscriptions;
using Convertly.Infrastructure.Auth;
using Convertly.Infrastructure.Conversions;
using Convertly.Infrastructure.Jobs;
using Convertly.Infrastructure.Persistence;
using Convertly.Infrastructure.Storage;
using Convertly.Infrastructure.Subscriptions;
using Convertly.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Hangfire;
using Hangfire.PostgreSql;

namespace Convertly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' was not configured.");
        }

        services.AddDbContext<ConvertlyDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddHangfire(configuration =>
        {
            configuration.UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(connectionString);
            });
        });

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        var supabaseStorageOptions = configuration.GetSection("Supabase").Get<SupabaseStorageOptions>()
            ?? new SupabaseStorageOptions();
        supabaseStorageOptions.Validate();

        services.AddSingleton(Options.Create(supabaseStorageOptions));
        services.AddHttpClient<IFileStorageService, SupabaseFileStorageService>(client =>
        {
            client.BaseAddress = new Uri(supabaseStorageOptions.Url.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Authorization = new(
                "Bearer",
                supabaseStorageOptions.ServiceRoleKey);
        });

        services.AddHttpContextAccessor();
        services.AddScoped<IAuthService, AuthService>();
        services.Configure<ConversionOptions>(configuration.GetSection("Conversion"));
        services.AddScoped<IFileConverter, DocxToPdfConverter>();
        services.AddScoped<IFileConverterResolver, FileConverterResolver>();
        services.AddScoped<IConversionJobProcessor, ConversionJobProcessor>();
        services.AddScoped<IConversionJobQueue, HangfireConversionJobQueue>();
        services.AddScoped<IConversionService, ConversionService>();
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IMonthlyUsageService, MonthlyUsageService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();

        return services;
    }
}
