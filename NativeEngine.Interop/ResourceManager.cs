using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilTemple.NativeEngineInterop.Generated
{
    public partial class ResourceManager
    {
    
        public static byte[] ReadFile(string path)
        {
            using (var byteArray = read(path))
                return byteArray.constData();
        }

    }
}
