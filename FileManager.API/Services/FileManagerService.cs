using FileManager.API.Abstractions;
using FileManager.Models;
using System.IO.Compression;
using System.Text.Json;

namespace FileManager.API.Services
{
    public class FileManagerService : IFileManagerService
    {
        private string _tempPath = string.Empty;
        private string _filePath = string.Empty;
        private string _extractPath = string.Empty;

        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public async Task<string> TryReadFileAsync(IFormFile file)
        {
            await UploadFile(file);

            var node = ReadCatalog();
            var json = JsonSerializer.Serialize(node, _options);

            RemoveTemporalFiles();

            return json;
        }

        private async Task UploadFile(IFormFile file)
        {
            if (file == null)
            {
                throw new Exception("No file uploaded");
            }

            _tempPath = Path.GetTempPath();
            _filePath = Path.Combine(_tempPath, file.FileName);
            _extractPath = Path.Combine(Path.GetTempPath(), "Working project");

            CheckFileExtention(file);

            await CopyFileToLocalStorage(file);

            ExtractArchive();
        }

        private void CheckFileExtention(IFormFile file)
        {
            var extention = Path.GetExtension(file.FileName);

            if (!extention.Equals(".zip"))
            {
                throw new Exception($"{extention} is not supported! Use .zip format files");
            }
        }

        private async Task CopyFileToLocalStorage(IFormFile file)
        {
            using var stream = new FileStream(_filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        private void ExtractArchive()
        {
            //using ZipArchive archive = ZipFile.OpenRead(_filePath);

            //foreach (var entry in archive.Entries)
            //{
            //    var name = entry.Name;
            //    var size = entry.Length;
            //}

            ZipFile.ExtractToDirectory(_filePath, _extractPath);
        }

        private Node ReadCatalog()
        {
            var catalogReader = new CatalogReader(_extractPath);
            var node = catalogReader.TryRead();

            return node;
        }

        private void RemoveTemporalFiles()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            if (Directory.Exists(_extractPath))
            {
                Directory.Delete(_extractPath, true);
            }
        }
    }
}
