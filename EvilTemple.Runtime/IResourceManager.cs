using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EvilTemple.Runtime
{
    public interface IResourceManager
    {

        bool MountArchive(string path);

        /// <summary>
        /// Reads a resource from the virtual file system.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The resource's content.</returns>
        /// <exception cref="FileNotFoundException">If the file doesn't exist.</exception>
        byte[] ReadResource(string path);
        
    }
}
