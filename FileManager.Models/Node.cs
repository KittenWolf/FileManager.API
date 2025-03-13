using System.Globalization;
using System.Text.Json.Serialization;

namespace FileManager.Models
{
    public class Node
    {
        [JsonIgnore]
        private readonly string[] _suffixes = ["B", "KB", "MB", "GB", "TB"];

        [JsonIgnore]
        public Node? Parent { get; private set; }

        [JsonIgnore]
        public string Path { get; private set; } = string.Empty;

        public string Name { get; private set; } = string.Empty;
        public string Size { get; private set; } = string.Empty;
        
        public long Bytes { get; private set; }

        [JsonPropertyName("date")]
        public string CreationDate { get; private set; } = string.Empty;

        [JsonPropertyName("type")]
        public string FileExtention { get; private set; } = string.Empty;

        public List<Node> Children { get; private set; } = [];

        public Node(FileInfo fileInfo)
        {
            InitAsFile(fileInfo);
        }

        public Node(DirectoryInfo directoryInfo)
        {
            InitAsDirectory(directoryInfo);
        }

        public Node(FileInfo fileInfo, Node parent)
            : this(fileInfo)
        {
            InitParentRelation(parent);
        }

        public Node(DirectoryInfo directoryInfo, Node parent)
            : this(directoryInfo)
        {
            InitParentRelation(parent);
        }

        private void InitAsDirectory(DirectoryInfo directoryInfo)
        {
            Path = directoryInfo.FullName;
            Name = directoryInfo.Name;
            FileExtention = "File folder";
            CreationDate = directoryInfo.CreationTime.ToString("G", CultureInfo.CurrentCulture);
        }

        private void InitAsFile(FileInfo fileInfo)
        {
            Path = fileInfo.FullName;
            Name = fileInfo.Name;
            FileExtention = fileInfo.Extension;
            CreationDate = fileInfo.CreationTime.ToString("G", CultureInfo.CurrentCulture);

            SetSize(fileInfo.Length);
        }

        private void InitParentRelation(Node parent)
        {
            Parent = parent;
        }

        private void SetSize(long size)
        {
            var index = 0;

            while (size >= 1024)
            {
                size /= 1024;
                index++;
            }

            Size = $"{size} {_suffixes[index]}";
            Bytes = size;
        }
    }
}
