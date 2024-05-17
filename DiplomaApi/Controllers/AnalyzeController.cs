using DiplomaApi.DataRepository.GenericRepository;
using DiplomaApi.DataRepository.Models;
using DiplomaApi.ModelsDto;
using DiplomaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace DiplomaApi.Controllers
{
    [ApiController]
    [Route("/api/analyze")]
    [Authorize]
    public class AnalyzeController : ControllerBase
    {
        private readonly IEntityRepository<Log> _logRepository;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;

        public AnalyzeController(IEntityRepository<Log> logRepository,
            IFileService fileService,
            IUserService userService)
        {
            _logRepository = logRepository;
            _fileService = fileService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<List<string>>> Uploud(IFormFile pcapFile, int senderCount)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await pcapFile.CopyToAsync(memoryStream);
                    byte[] pcapBytes = memoryStream.ToArray();

                    // Далее используем сертификат для верификации подписи
                    string filePath = await _fileService.SaveFile(pcapBytes);

                    // Создание процесса для вызова Python скрипта
                    using (var client = new HttpClient())
                    {
                        var request = await client.PostAsync("http://ddos_detector_ai:5000/api/detect", new MultipartFormDataContent
                        {
                            { new StringContent(filePath), "filePath" },
                            { new StringContent(senderCount.ToString()), "senderCount" }
                        });
                        if (request.IsSuccessStatusCode)
                        {
                            var response = await request.Content.ReadAsStringAsync();
                            Console.WriteLine(response);
                            var ips = JsonSerializer.Deserialize<List<LogDto>>(response);
                            var userLogs = new List<Log>();
                            foreach (var ip in ips)
                            {
                                userLogs.Add(new Log
                                {
                                    Ip = ip.IpAddress,
                                    RequestCount = ip.RequestCount,
                                    CreatedAt = DateTime.UtcNow,
                                    UserId = int.Parse(_userService.GetClaimValue(ClaimType.UserId))
                                });
                            }
                            await _logRepository.InsertManyAsync(userLogs);
                            await _logRepository.SaveChangesAsync();

                            // Возвращаем сообщение о успешной верификации
                            return Ok(ips);
                        }
                        else
                        {
                            return Ok(new List<string>());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
