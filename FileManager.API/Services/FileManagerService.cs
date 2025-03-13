using FileManager.Models;
using System.IO.Compression;
using System.Text.Json;

namespace FileManager.API.Services
{
    public class FileManagerService
    {
        private readonly string _tempPath;
        private readonly string _filePath;
        private readonly string _extractPath;

        private IFormFile _file;

        private readonly JsonSerializerOptions _options;

        public FileManagerService(IFormFile file)
        {
            _file = file ?? throw new Exception("No file uploaded");

            _tempPath = Path.GetTempPath();
            _filePath = Path.Combine(_tempPath, _file.FileName);
            _extractPath = Path.Combine(Path.GetTempPath(), "Working project");

            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public void CheckFileExtention()
        {
            var extention = Path.GetExtension(_file.FileName);

            if (!extention.Equals(".zip"))
            {
                throw new Exception($"{extention} is not supported! Use .zip format files");
            }
        }

        internal async Task CopyToLocalStorage()
        {
            using var stream = new FileStream(_filePath, FileMode.Create);
            await _file.CopyToAsync(stream);
        }

        internal void ExtractArchive()
        {
            ZipFile.ExtractToDirectory(_filePath, _extractPath);
        }

        internal Node ReadCatalog()
        {
            var catalogReader = new CatalogReader(_extractPath);
            var node = catalogReader.TryRead();

            return node;
        }

        internal void RemoveTemporalFiles()
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

        internal async Task<string> TryReadFileAsync()
        {
            CheckFileExtention();

            await CopyToLocalStorage();

            ExtractArchive();

            var node = ReadCatalog();
            var json = JsonSerializer.Serialize(node, _options);

            return json;
        }
    }
}
