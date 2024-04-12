namespace DiplomaApi.Services
{
    public interface IFileService
    {
        Task<string> SaveFile(byte[] pcapBytes);
    }
}
