using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvilTemple.NativeEngineInterop;
using EvilTemple.NativeEngineInterop.Generated;
using EvilTemple.Runtime;

namespace EvilTemple
{
    class ResourceManagerAdapter : IResourceManager
    {
        public bool MountArchive(string path)
        {
            ResourceManager.addZipArchive("General", path);
            return true;
        }

        public byte[] ReadResource(string path)
        {
            return ResourceManager.ReadFile(path);
        }
    }
}
