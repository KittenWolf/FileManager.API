using FileManager.API.Abstractions;
using FileManager.API.Models;
using System.IO.Compression;

namespace FileManager.API.Services
{
    public class ZipArchiveReaderService(IFileCatalogReaderService fileCatalogReaderService) : IZipArchiveReaderService
    {
        private string _extractPath = string.Empty;

        public Node ReadArchive(string path)
        {
            using ZipArchive archive = ZipFile.OpenRead(path);

            var archiveName = Path.GetFileName(path).Split('.')[0];
            var dictionary = new Dictionary<string, Node>();

            foreach (var entry in archive.Entries)
            {
                if (!dictionary.ContainsKey(entry.FullName))
                {
                    var node = new Node(entry);

                    var potentialParentPath = !string.IsNullOrEmpty(entry.Name)
                        ? entry.FullName.Replace($"{node.Name}", "")
                        : entry.FullName.Replace($"{node.Name}/", "");

                    if (dictionary.TryGetValue(potentialParentPath, out Node? value))
                    {
                        value.Children.Add(node);
                    }

                    dictionary[entry.FullName] = node;
                }
            }

            return dictionary[$"{archiveName}/"];
        }

        public Node ReadArchiveWithExtration(string path)
        {
            ExtractArchive(path);

            var node = fileCatalogReaderService.TryRead(_extractPath);

            RemoveTemporalFiles();

            return node;
        }

        private void ExtractArchive(string path)
        {
            _extractPath = Path.Combine(Path.GetTempPath(), "Working project");

            ZipFile.ExtractToDirectory(path, _extractPath);
        }

        private void RemoveTemporalFiles()
        {
            if (Directory.Exists(_extractPath))
            {
                Directory.Delete(_extractPath, true);
            }
        }
    }
}
