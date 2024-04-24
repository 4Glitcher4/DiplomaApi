using DiplomaApi.DataRepository.GenericRepository;
using DiplomaApi.DataRepository.Models;
using DiplomaApi.Helpers.FilterHelper;
using DiplomaApi.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IUserService _userService;

        public LogsController(IEntityRepository<Log> logRepository,
            IUserService userService)
        {
            _logRepository = logRepository;
            _userService = userService;
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

        [HttpGet("/api/userLogs")]
        [FilterHelper<Log>]
        [Authorize]
        public ActionResult<IEnumerable<Log>> GetUserLog()
        {
            try
            {
                var userId = _userService.GetClaimValue(ClaimType.UserId);

                return Ok(_logRepository.AsQueryable().Where(doc => doc.UserId == int.Parse(userId)));
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
