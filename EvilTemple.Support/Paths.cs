using System;
using System.IO;
using System.Reflection;
using EvilTemple.Runtime;

namespace EvilTemple.Support
{
    public class Paths : IPaths
    {
        public string UserDataPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EvilTemple");
            }
        }

        public string GeneratedDataPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EvilTemple");
            }
        }

        public string InstallationPath
        {
            get { var assembly = Assembly.GetExecutingAssembly();
                var binDir = Path.GetDirectoryName(assembly.Location);
                binDir += Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar;
                return Path.GetFullPath(binDir);
            }
        }
    }
}
