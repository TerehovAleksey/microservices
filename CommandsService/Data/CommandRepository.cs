using CommandsService.Models;

namespace CommandsService.Data;

public class CommandRepository : ICommandRepository
{
    private readonly AppDbContext _context;

    public CommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public void CreateCommand(int platformId, Command command)
    {
        command.PlatformId = platformId;
        _context.Commands.Add(command);
    }

    public void CreatePlatform(Platform platform) =>
        _context.Platforms.Add(platform);

    public IEnumerable<Platform> GetAllPlatforms() =>
        _context.Platforms;

    public Command? GetCommand(int platformId, int commandId) =>
        _context.Commands
        .FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);

    public bool ExternalPlatformExists(int externalPlatformId) =>
        _context.Platforms.Any(p => p.ExternalId == externalPlatformId);

    public IEnumerable<Command> GetCommandsForPlatform(int platformId) =>
        _context.Commands
        .Where(c => c.PlatformId == platformId)
        .OrderBy(c => c.Platform.Name);

    public bool PlatformExists(int platformId) =>
        _context.Platforms.Any(p => p.Id == platformId);

    public bool SaveChanges() =>
        _context.SaveChanges() > 0;
}
