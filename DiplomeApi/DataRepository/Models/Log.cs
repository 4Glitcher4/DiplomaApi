using DiplomeApi.DataRepository.GenericRepository;
using System.Text.Json.Serialization;

namespace DiplomeApi.DataRepository.Models
{
    public class Log : EntityDocument
    {
        [JsonPropertyName("ip")]
        public string Ip {  get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;
        [JsonPropertyName("ddos_probability")]
        public double DdosProbability { get; set; } = 0;
    }
}
