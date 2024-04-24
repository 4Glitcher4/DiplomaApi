﻿using DiplomaApi.DataRepository.GenericRepository;
using DiplomaApi.DataRepository.Models;
using DiplomaApi.ModelsDto;
using DiplomaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
                string pythonInterpreter = "python";

                // Путь к Python скрипту, который анализирует pcap файлы
                string pythonScript = "C:/Users/user/Desktop/dos-attack-detection-via-deep-learning-main/detector/src/api_entrypoint.py";

                using (var memoryStream = new MemoryStream())
                {
                    await pcapFile.CopyToAsync(memoryStream);
                    byte[] pcapBytes = memoryStream.ToArray();

                    // Далее используем сертификат для верификации подписи
                    string filePath = await _fileService.SaveFile(pcapBytes);

                    // Создание процесса для вызова Python скрипта
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = pythonInterpreter,
                        Arguments = $"{pythonScript} {filePath} {senderCount}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(startInfo))
                    {
                        // Чтение вывода Python скрипта
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();

                        var ips = JsonSerializer.Deserialize<List<LogDto>>(output);
                        var userLogs = new List<Log>();
                        foreach(var ip in ips)
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
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
