using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenKit.CR2W
{
    public sealed class PathHashDecoder
    {
        private static readonly PathHashDecoder instance = new PathHashDecoder();
        private Dictionary<ulong, string> hashToPathMap = new Dictionary<ulong, string>();

        static PathHashDecoder()
        {
        }

        private PathHashDecoder()
        {
        }

        public static PathHashDecoder Instance
        {
            get
            {
                return instance;
            }
        }

        public void initialize(string mappingFilePath)
        {
            System.IO.FileInfo mappingFile = new System.IO.FileInfo(mappingFilePath);

            if (!mappingFile.Exists)
            {
                return;
            }

            foreach (string pathAndHashString in System.IO.File.ReadLines(mappingFile.FullName))
            {
                string[] pathAndHash = pathAndHashString.Split(',');

                if (pathAndHash.Length != 2)
                {
                    continue;
                }

                string path = pathAndHash[0];

                try
                {
                    ulong hash = ulong.Parse(pathAndHash[1]);

                    hashToPathMap.Add(hash, path);
                }
                catch { }
            }
        }

        public string getPath(ulong hash)
        {
            string path;

            if (hashToPathMap.TryGetValue(hash, out path))
            {
                return path;
            }
            else
            {
                return "n/a";
            }
        }
    }
}
