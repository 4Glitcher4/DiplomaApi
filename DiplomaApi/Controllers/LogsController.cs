using DiplomaApi.DataRepository.GenericRepository;
using DiplomaApi.DataRepository.Models;
using DiplomaApi.Helpers.FilterHelper;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiplomaApi.Controllers
{
    [ApiController]
    [Route("/api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly IEntityRepository<Log> _logRepository;
        public LogsController(IEntityRepository<Log> logRepository)
        {
            _logRepository = logRepository;
        }


        [HttpGet]
        [FilterHelper<Log>]
        public ActionResult<IEnumerable<Log>> Get()
        {
            try
            {
                return Ok(_logRepository.AsQueryable());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(dynamic obj)
        {
            try
            {
                Log entity = JsonSerializer.Deserialize<Log>(obj, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _logRepository.InsertOneAsync(entity);
                await _logRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
