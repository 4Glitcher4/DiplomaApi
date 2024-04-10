using DiplomaApi.DataRepository.GenericRepository;
using DiplomaApi.DataRepository.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace DiplomaApi.Controllers
{
    [ApiController]
    [Route("/api/ip")]
    public class IpController : ControllerBase
    {
        private readonly IEntityRepository<IpAddress> _ipAddressRepository;
        public IpController(IEntityRepository<IpAddress> ipAddressRepository)
        {
            _ipAddressRepository = ipAddressRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post(dynamic obj)
        {
            try
            {
                IpAddress entity = JsonSerializer.Deserialize<IpAddress>(obj, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _ipAddressRepository.InsertOneAsync(entity);
                await _ipAddressRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
