using FileManager.API.Models;

namespace FileManager.API.Abstractions
{
    public interface IFileCatalogReaderService
    {
        Node TryRead(string path);
    }
}