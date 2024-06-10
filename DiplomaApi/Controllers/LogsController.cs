using DiplomaApi.DataRepository.GenericRepository;
using DiplomaApi.DataRepository.Models;
using DiplomaApi.Helpers.FilterHelper;
using DiplomaApi.ModelsDto;
using DiplomaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
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

        [HttpGet("/api/userLogs/weekly")]
        [Authorize]
        public ActionResult<IEnumerable<Log>> GetWeekly()
        {
            try
            {
                var userId = _userService.GetClaimValue(ClaimType.UserId);

                var weelkyLogs = _logRepository.AsQueryable()
                    .Where(doc => doc.UserId == int.Parse(userId))
                    .ToList();

                List<DayOfWeek> week = new List<DayOfWeek>
                {
                     DayOfWeek.Monday,
                     DayOfWeek.Tuesday,
                     DayOfWeek.Wednesday,
                     DayOfWeek.Thursday,
                     DayOfWeek.Friday,
                     DayOfWeek.Saturday,
                    DayOfWeek.Sunday
                };
                var query = week.AsQueryable()
                    .GroupJoin(weelkyLogs, day => day, request => request.CreatedAt.DayOfWeek, (day, requests) => new { day, requests })
                    .Select(joinResult => joinResult).ToList();

                var result = new List<WeeklyDto>();

                foreach(var dayOfWeek in query)
                {
                    var maxRequest = dayOfWeek.requests?.Max(doc => doc?.RequestCount);
                    var res = new WeeklyDto
                    {
                        Day = dayOfWeek.day.ToString(),
                        Ip = weelkyLogs.FirstOrDefault(doc => doc.RequestCount == maxRequest)?.Ip ?? "",
                        ReqestCount = maxRequest ?? 0,
                    };

                    result.Add(res);
                }

                return Ok(result);
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
