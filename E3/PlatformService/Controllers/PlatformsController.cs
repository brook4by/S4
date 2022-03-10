using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private IPlatformRepo _repository;
        private IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repository, 
            IMapper mapper, 
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("---> Getting Platforms");

            var platforms = _repository.GeAlltPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById([FromRoute]int id)
        {
            var platform =_repository.GetPlatformById(id);
            if (platform!=null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platform));
            }
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platform)
        {
            var platformModel = _mapper.Map<Platform>(platform);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();
            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);


            //Send Sync Message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"--> Could not send sync: {ex.Message}");
            }

            //Send Async Message

            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not send async: {e.Message}");
                throw;
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { platformReadDto.Id }, platformReadDto);
        }

    }
}
