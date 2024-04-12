using DiplomaApi.DataRepository.GenericRepository;
using DiplomaApi.DataRepository.Models;
using DiplomaApi.ModelsDto;
using DiplomaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaApi.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IEntityRepository<User> _userRepository;
        private readonly IUserService _userService;
        private readonly ISmtpService _smtpService;
        public AuthController(IEntityRepository<User> userRepository,
            IUserService userService,
            ISmtpService smtpService)
        {
            _userRepository = userRepository;
            _userService = userService;
            _smtpService = smtpService;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                var user = await _userRepository.FindOneAsync(doc => doc.Login == userDto.Login);
                if (user != null)
                    return BadRequest($"Пользователь с {userDto.Login} уже существует.");
                user = new User
                {
                    Login = userDto.Login,
                    Password = _userService.Encrypt(userDto.Password),
                };

                await _userRepository.InsertOneAsync(user);
                await _userRepository.SaveChangesAsync();

                var token = _userService.CreateToken(new ClaimSettings
                {
                    UserId = user.Id.ToString(),
                    TokenLifeTime = DateTime.Now.AddMinutes(10),
                });

                _smtpService.SendMessage($"http://localhost:6743/api/auth/callback/{token}", new SendDto
                {
                    Login = user.Login,
                    Subject = "DiplomaApi"
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            try
            {
                var user = await _userRepository.FindOneAsync(doc => doc.Login == userDto.Login);
                if (user != null)
                    return BadRequest($"Пользователь с {userDto.Login} уже существует.");
                if (user.Password != _userService.Encrypt(userDto.Password))
                    return BadRequest();
                if (!user.IsVerify)
                    return BadRequest("Этот пользователь не подтвержден");

                var token = _userService.CreateToken(new ClaimSettings
                {
                    UserId = user.Id.ToString(),
                    TokenLifeTime = DateTime.Now.AddDays(1),
                });

                return Ok(new { accessToken = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("resendRegister")]
        public async Task<IActionResult> ResendRegister(string login)
        {
            try
            {
                var user = await _userRepository.FindOneAsync(doc => doc.Login == login);
                if (user != null)
                    return BadRequest($"Пользователь с {login} уже существует.");

                var token = _userService.CreateToken(new ClaimSettings
                {
                    UserId = user.Id.ToString(),
                    TokenLifeTime = DateTime.Now.AddMinutes(10),
                });

                _smtpService.SendMessage($"http://localhost:6743/api/auth/callback/{token}", new SendDto
                {
                    Login = user.Login,
                    Subject = "DiplomaApi"
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("callback/{token}")]
        public async Task<IActionResult> Callback(string token)
        {
            try
            {
                var userId = _userService.GetClaimValue(token, ClaimType.UserId);
                var user = await _userRepository.FindByIdAsync(int.Parse(userId));

                if (user == null)
                    return NotFound("Пользователь не найден.");

                user.IsVerify = true;
                await _userRepository.ReplaceOneAsync(user);
                await _userRepository.SaveChangesAsync();

                return Redirect("http://localhost:6743/swagger");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
