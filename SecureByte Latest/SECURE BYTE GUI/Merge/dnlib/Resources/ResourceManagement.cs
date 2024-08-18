using System.IO;
using dnlib.Load;

namespace dnlib.DotNet.Resources
{
    public class ResourceManagement
    {
        private AssemblyContext _context = null;

        public ResourceManagement(AssemblyContext context)
        {
            _context = context;
        }

        public void Add(string name, byte[] bytes)
        {
            EmbeddedResource resource = new EmbeddedResource(name, bytes, ManifestResourceAttributes.Public);
            _context.Module.Resources.Add(resource);
        }

        public void Add(string path)
        {
            string name = Path.GetFileName(path);
            byte[] bytes = File.ReadAllBytes(path);
            EmbeddedResource resource = new EmbeddedResource(name, bytes, ManifestResourceAttributes.Public);
            _context.Module.Resources.Add(resource);
        }

        public void Replace(string name, byte[] bytes)
        {
            for (var i = 0; i < _context.Module.Resources.Count; i++)
            {
                if (_context.Module.Resources[i].Name == name)
                {
                    _context.Module.Resources.RemoveAt(i);
                    EmbeddedResource resource = new EmbeddedResource(name, bytes, ManifestResourceAttributes.Public);
                    _context.Module.Resources.Add(resource);
                    break;
                }
            }
        }

        public void Replace(string path)
        {
            string name = Path.GetFileName(path);
            byte[] bytes = File.ReadAllBytes(path);
            for (var i = 0; i < _context.Module.Resources.Count; i++)
            {
                if (_context.Module.Resources[i].Name == name)
                {
                    _context.Module.Resources.RemoveAt(i);
                    EmbeddedResource resource = new EmbeddedResource(name, bytes, ManifestResourceAttributes.Public);
                    _context.Module.Resources.Add(resource);
                    break;
                }
            }
        }

        public MemoryStream GetStreamResource(string name)
        {
            foreach (var resource in _context.Module.Resources)
            {
                if (resource.Name == name)
                {
                    var embeddedResource = (EmbeddedResource)resource;
                    var stream = embeddedResource.CreateReader().AsStream();

                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);

                    var memStream = new MemoryStream();
                    memStream.Write(bytes, 0, bytes.Length);
                    memStream.Position = 0;
                    return memStream;
                }
            }
            return null;
        }

        public byte[] GetByteResource(string name)
        {
            foreach (var resource in _context.Module.Resources)
            {
                if (resource.Name == name)
                {
                    var embeddedResource = (EmbeddedResource)resource;
                    var stream = embeddedResource.CreateReader().AsStream();
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    return bytes;
                }
            }
            return null;
        }
    }
}
