using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Migration.Worker;
using Migration.Worker.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddMigrationLogicServices()
    .AddMigrationDataAccessServices(builder);

var host = builder.Build();

//host.Run();

// This application should listen for messages from a queue. For testing, I run the migration logic directly
using var scope = host.Services.CreateScope();

var processor = scope.ServiceProvider.GetRequiredService<MigrationProcessor>();

if (args.Length == 0 || !int.TryParse(args[0], out var legacyUserId))
{
    Console.WriteLine("Please provide a valid legacyUserId as the first argument.");
    return;
}

await processor.ProcessMigrationAsync(legacyUserId, CancellationToken.None);

Console.WriteLine($"Migration process for user {legacyUserId} completed.");
Console.ReadLine();