using DiplomaApi.DataRepository.GenericRepository;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaApi.DataRepository.Models
{
    public class UserLog : EntityDocument
    {
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        
        public string IpAddress { get; set; }
        public int RequestCount { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}
