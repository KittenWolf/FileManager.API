namespace FileManager.Models
{
    public class CatalogReader
    {
        private readonly string _path;

        public CatalogReader(string path)
        {
            _path = path;
        }

        public Node TryRead()
        {
            var directoryInfo = new DirectoryInfo(_path);
            var node = new Node(directoryInfo);

            try
            {
                foreach (var dir in Directory.GetDirectories(node.Path))
                {
                    CreateDirectoryNode(node);
                }

                CreateFileNode(node);
            }
            catch (Exception)
            {
                throw;
            }

            return node;
        }

        private Node CreateDirectoryNode(Node parent)
        {
            foreach (var dir in Directory.GetDirectories(parent.Path))
            {
                var directoryInfo = new DirectoryInfo(dir);
                var node = new Node(directoryInfo, parent);

                CreateDirectoryNode(node);
                CreateFileNode(node);

                parent.Children.Add(node);
            }

            return parent;
        }

        private Node CreateFileNode(Node parent)
        {
            foreach (var file in Directory.GetFiles(parent.Path))
            {
                var fileInfo = new FileInfo(file);
                var node = new Node(fileInfo, parent);

                parent.Children.Add(node);
            }

            return parent;
        }
    }
}
