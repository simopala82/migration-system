using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Migration.Core.Services;
using Migration.DataAccess;
using Migration.DataAccess.Services;

namespace Migration.Worker.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMigrationLogicServices(this IServiceCollection services)
    {
        services.AddSingleton<ISlotManager>(sp => new SlotManager(maxSlots: 5));
        
        services.AddScoped<IDataTransformer, DataTransformer>();
        services.AddScoped<MigrationProcessor, MigrationProcessor>();

        return services;
    }

    public static IServiceCollection AddMigrationDataAccessServices(this IServiceCollection services,
        IHostApplicationBuilder builder)
    {
        services.AddDbContext<MigrationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MigrationDB")));
        services.AddScoped<IMigrationStatusRepository, MigrationStatusRepository>();
        
        services.AddDbContext<UserOldDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("OldDB")));
        services.AddScoped<IUserOldRepository, UserOldRepository>();
        
        services.AddDbContext<UserNewDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("NewDB")));
        services.AddScoped<IUserNewRepository, UserNewRepository>();
        
        return services;
    }
}