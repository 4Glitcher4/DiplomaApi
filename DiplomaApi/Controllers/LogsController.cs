using DiplomaApi.DataRepository.GenericRepository;
using DiplomaApi.DataRepository.Models;
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
        public ActionResult<IEnumerable<Log>> Get()
        {
            try
            {
                return Ok(_logRepository.AsQueryable().ToList());
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

        [HttpGet("asdasdas")]
        public IActionResult ASD()
        {
            string pythonInterpreter = "python";

            // Путь к Python скрипту, который анализирует pcap файлы
            string pythonScript = "C:/Users/user/Desktop/dos-attack-detection-via-deep-learning-main/detector/src/pcap/analyzer.py";

            // Аргументы, передаваемые в Python скрипт
            string arguments = "C:/Users/user/Desktop/lab1.pcap";
            string arguments2 = "C:/Users/user/Desktop/opuidhjjksdafiasdfknasjkldnfuiasdf.txt";

            // Создание процесса для вызова Python скрипта
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pythonInterpreter,
                Arguments = $"{pythonScript} {arguments}",
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

                // Вывод результата и ошибок, если есть
                Console.WriteLine("Output:");
                Console.WriteLine(output);
                Console.WriteLine("Error:");
                Console.WriteLine(error);
            }
            return Ok();
        }
    }
}
