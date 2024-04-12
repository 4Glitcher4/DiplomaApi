using DiplomaApi.ModelsDto;

namespace DiplomaApi.Services
{
    public interface ISmtpService
    {
        bool SendMessage(string url, SendDto smtp);
    }
}
