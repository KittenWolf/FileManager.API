using FileManager.API.Abstractions;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FileManager.API.Services
{
    public partial class FileManagerService(IZipArchiveReaderService zipArchiveReaderService) : IFileManagerService
    {
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private string _filePath = string.Empty;

        public async Task<string> TryReadFileAsync(IFormFile file)
        {
            try
            {
                await UploadFileAsync(file);

                var json = string.Empty;
                var extention = FileExtentionRegex().Match(file.FileName).Value;

                switch (extention)
                {
                    case ".zip":
                        var node = zipArchiveReaderService.ReadArchive(_filePath);
                        json = JsonSerializer.Serialize(node, _options);
                        break;

                    default: 
                        throw new Exception($"{file.FileName} is not supported! Use .zip format files");
                }

                RemoveTemporalFiles();

                return json;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task UploadFileAsync(IFormFile file)
        {
            var tempPath = Path.GetTempPath();

            _filePath = Path.Combine(tempPath, file.FileName);

            await CopyFileToLocalStorageAsync(file);
        }

        private async Task CopyFileToLocalStorageAsync(IFormFile file)
        {
            using var stream = new FileStream(_filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        private void RemoveTemporalFiles()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        [GeneratedRegex(".\\w+$")]
        public static partial Regex FileExtentionRegex();
    }
}
