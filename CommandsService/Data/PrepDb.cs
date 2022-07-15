using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder builder)
    {
        using var serviceScope = builder.ApplicationServices.CreateScope();
        var grpcClient = serviceScope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
        var platforms = grpcClient.ReturnAllPlatforms();

        var commandRepository = serviceScope.ServiceProvider.GetRequiredService<ICommandRepository>();
        SeedData(commandRepository, platforms);
    }

    private static void SeedData(ICommandRepository commandRepository, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("--> Seeding new platforms...");

        foreach (var platform in platforms)
        {
            if (!commandRepository.ExternalPlatformExists(platform.ExternalId))
            {
                commandRepository.CreatePlatform(platform);
            }
            commandRepository.SaveChanges();
        }
    }
}
