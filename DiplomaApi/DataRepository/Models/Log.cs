using DiplomaApi.DataRepository.GenericRepository;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DiplomaApi.DataRepository.Models
{
    public class Log : EntityDocument
    {
        [JsonPropertyName("ip")]
        public string Ip {  get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string CreatedAtString { get => CreatedAt.ToString(); } 
        [JsonPropertyName("ddos_probability")]
        public double DdosProbability { get; set; } = 0;
        [JsonPropertyName("request_count")]
        public int RequestCount { get; set; } = 0;
    }
}
