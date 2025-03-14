namespace FileManager.API.Abstractions
{
    public interface IFileManagerService
    {
        Task<string> TryReadFileAsync(IFormFile file);
    }
}