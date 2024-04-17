using DiplomaApi.DataRepository.GenericRepository;
using DiplomaApi.DataRepository.Models;
using DiplomaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaApi.Controllers
{
    [ApiController]
    [Route("/api/userLogs")]
    [Authorize]
    public class UserLogsController : ControllerBase
    {
        private readonly IEntityRepository<UserLog> _userLogRepository;
        private readonly IUserService _userService;

        public UserLogsController(IEntityRepository<UserLog> userLogRepository,
            IUserService userService)
        {
            _userLogRepository = userLogRepository;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<UserLog>> Get()
        {
            try
            {
                var userLogs = _userLogRepository.AsQueryable()
                    .Where(doc => doc.UserId == int.Parse(_userService.GetClaimValue(ClaimType.UserId)));
                return Ok(userLogs.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
