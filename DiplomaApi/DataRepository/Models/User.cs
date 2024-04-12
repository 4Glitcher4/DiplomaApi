using DiplomaApi.DataRepository.GenericRepository;

namespace DiplomaApi.DataRepository.Models
{
    public class User : EntityDocument
    {
        public string UserName { get; set; }= string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsVerify { get; set; }
    }
}
