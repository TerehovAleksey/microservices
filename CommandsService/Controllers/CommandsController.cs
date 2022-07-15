using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[ApiController]
[Route("api/c/platforms/{platformId:int}/[controller]")]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepository _commandRepository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepository commandRepository, IMapper mapper)
    {
        _commandRepository = commandRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");

        if (_commandRepository.PlatformExists(platformId))
        {
            var result = _commandRepository.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(result));
        }

        return NotFound();
    }

    [HttpGet("{commandId:int}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

        var result = _commandRepository.GetCommand(platformId, commandId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<CommandReadDto>(result));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto dto)
    {
        Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

        if (!_commandRepository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var command = _mapper.Map<Command>(dto);
        _commandRepository.CreateCommand(platformId, command);
        _commandRepository.SaveChanges();
        var result = _mapper.Map<CommandReadDto>(command);
        return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = result.Id }, result);
    }
}