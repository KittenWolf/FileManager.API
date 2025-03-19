using FileManager.API.Models;

namespace FileManager.API.Abstractions
{
    public interface IZipArchiveReaderService
    {
        Node ReadArchive(string path);
        Node ReadArchiveWithExtration(string path);
    }
}