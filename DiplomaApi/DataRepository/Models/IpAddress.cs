using DiplomaApi.DataRepository.GenericRepository;
using System.Text.Json.Serialization;

namespace DiplomaApi.DataRepository.Models
{
    public class IpAddress : EntityDocument
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; } = string.Empty;
        [JsonPropertyName("intent")]
        public IntentStatus Intent { get; set; }
    }
}
