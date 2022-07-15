using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/platforms")]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(IPlatformRepository platformRepository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
    {
        _platformRepository = platformRepository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public IEnumerable<PlatformReadDto> GetPlatforms()
    {
        var result = _platformRepository.GetAllPlatforms();
        return _mapper.Map<IEnumerable<PlatformReadDto>>(result);
    }

    [HttpGet("{id:int}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        var result = _platformRepository.GetPlatformById(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<PlatformReadDto>(result));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto dto)
    {
        var model = _mapper.Map<Platform>(dto);
        _platformRepository.CreatePlatform(model);
        _platformRepository.SaveChanges();

        var platform = _mapper.Map<PlatformReadDto>(model);

        // send sync message
        try
        {
           await _commandDataClient.SendPlatformToCommand(platform);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send synchronously: {e.Message}");
        }
        
        // send async message
        try
        {
            var message = _mapper.Map<PlatformPublishedDto>(platform);
            message.Event = "Platform_Published";
            _messageBusClient.PublishNewPlatform(message);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send asynchronously: {e.Message}");
        }
        
        return CreatedAtRoute(nameof(GetPlatformById), new { Id = platform.Id }, platform);
    }
}