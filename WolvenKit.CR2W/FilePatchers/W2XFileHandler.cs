using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenKit.CR2W.FilePatchers
{
    public sealed class W2XFileHandler
    {
        private const string FileExtensionW2Ent = ".w2ent";
        private const string FileExtensionW2L = ".w2l";
        private const string FileExtensionW2Mesh = ".w2mesh";
        private const string FileExtensionW2P = ".w2p";

        private const string PathBundle = "Bundle";
        private const string PathDLC = "dlc";

        public List<string> ErrorMessages { get; private set; }
        public List<string> W2EntFilePathsForFires { get; private set; }
        public List<string> W2PFilePathsForFires { get; private set; }

        public W2XFileHandler()
        {
            ErrorMessages = new List<string>();
            W2EntFilePathsForFires = new List<string>();
            W2PFilePathsForFires = new List<string>();
        }

        public void Initialize(List<string> relativeModFilePaths, string fileDirectory, string modDirectory, string dlcDirectory, ILocalizedStringSource localizedStringSource)
        {
            InitializeW2EntAndW2PFilePathsForFires(relativeModFilePaths, fileDirectory, modDirectory, dlcDirectory, localizedStringSource);
        }

        private void InitializeW2EntAndW2PFilePathsForFires(List<string> relativeModFilePaths, string fileDirectory, string modDirectory, string dlcDirectory, ILocalizedStringSource localizedStringSource)
        {
            foreach (string relativeModFilePath in relativeModFilePaths)
            {
                try
                {
                    string absoluteModFilePath = fileDirectory + Path.DirectorySeparatorChar + relativeModFilePath;

                    if (!File.Exists(absoluteModFilePath))
                    {
                        throw new System.InvalidOperationException("File '" + absoluteModFilePath + "' does not exist.");
                    }

                    if (Path.GetExtension(absoluteModFilePath).Equals(FileExtensionW2Ent))
                    {
                        CR2WFile w2EntFile = W2EntFilePatcher.ReadW2EntFile(absoluteModFilePath, localizedStringSource);
                        List<SharedDataBuffer> sharedDataBuffersForFires = W2EntFilePatcher.ReadSharedDataBuffersForFires(w2EntFile);

                        if (sharedDataBuffersForFires == null || !sharedDataBuffersForFires.Any())
                        {
                            continue;
                        }

                        foreach (SharedDataBuffer sharedDataBufferForFire in sharedDataBuffersForFires)
                        {
                            List<string> w2PFilePathsForFires = W2EntFilePatcher.GetW2PFilePathsForFires(sharedDataBufferForFire, absoluteModFilePath, modDirectory, dlcDirectory, localizedStringSource);

                            if (w2PFilePathsForFires.Any())
                            {
                                W2PFilePathsForFires.AddRange(w2PFilePathsForFires);
                                W2EntFilePathsForFires.Add(absoluteModFilePath);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorMessages.Add(e.Message);

                    continue;
                }
            }
        }
    }
}
