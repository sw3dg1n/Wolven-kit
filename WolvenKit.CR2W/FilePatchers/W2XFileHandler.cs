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
        public const string FileExtensionW2Ent = ".w2ent";
        public const string FileExtensionW2L = ".w2l";
        public const string FileExtensionW2Mesh = ".w2mesh";
        public const string FileExtensionW2P = ".w2p";

        public const string FileNameSuffixILOD = "_ilod";

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
                //if (relativeModFilePath.Contains("character") || relativeModFilePath.Contains("community"))
                //{
                //    continue;
                //}

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

            // TODO this should simply be replaced with a set
            W2PFilePathsForFires = W2PFilePathsForFires.Distinct().ToList();
            W2EntFilePathsForFires = W2EntFilePathsForFires.Distinct().ToList();

            //string w2psource = "C:\\Users\\mkaltenb\\source\\repos\\fire_files\\w2p";

            //foreach (string fullPath in W2PFilePathsForFires)
            //{
            //    string newPath = w2psource + fullPath.Substring(fullPath.IndexOf(PathBundle) + PathBundle.Length);

            //    if (File.Exists(fullPath) && !File.Exists(newPath))
            //    {
            //        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            //        File.Copy(fullPath, newPath);
            //    }
            //}

            //string w2entsource = "C:\\Users\\mkaltenb\\source\\repos\\fire_files\\w2ent";

            //foreach (string fullPath in W2EntFilePathsForFires)
            //{
            //    string newPath = w2entsource + fullPath.Substring(fullPath.IndexOf(PathBundle) + PathBundle.Length);

            //    if (File.Exists(fullPath) && !File.Exists(newPath))
            //    {
            //        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            //        File.Copy(fullPath, newPath);
            //    }
            //}


            //using (TextWriter tw = new StreamWriter("C:\\Users\\mkaltenb\\source\\repos\\w2pfiles.txt"))
            //{
            //    foreach (string s in W2PFilePathsForFires)
            //    {
            //        tw.WriteLine(s);
            //    }
            //}

            //using (TextWriter tw = new StreamWriter("C:\\Users\\mkaltenb\\source\\repos\\w2entfiles.txt"))
            //{
            //    foreach (string s in W2EntFilePathsForFires)
            //    {
            //        tw.WriteLine(s);
            //    }
            //}
        }

        public static (Dictionary<string, string> relativeOriginalFilePathToRelativeRenamedFilePathMap, List<string> absoluteRenamedFilePaths) CopyAndRenameW2PFiles(List<string> absoluteW2PFilePaths)
        {
            return CopyAndRenameFiles(absoluteW2PFilePaths, FileExtensionW2P);
        }

        private static (Dictionary<string, string> relativeOriginalFilePathToRelativeRenamedFilePathMap, List<string> absoluteRenamedFilePaths) CopyAndRenameFiles(List<string> absoluteFilePaths, string fileExtension)
        {
            Dictionary<string, string> relativeOriginalFilePathToRelativeRenamedFilePathMap = new Dictionary<string, string>();
            List<string> absoluteRenamedFilePaths = new List<string>();

            string renamedFileNameSuffix = FileNameSuffixILOD + fileExtension;
            string filePathSeparator = Path.DirectorySeparatorChar + PathBundle + Path.DirectorySeparatorChar;

            foreach (string absoluteFilePath in absoluteFilePaths)
            {
                if (!File.Exists(absoluteFilePath))
                {
                    continue;
                }

                string absoluteRenamedFilePath;
                string absoluteOriginalFilePath;

                // We also add entries for already renamed files as there might be new w2ent files which still contain references to the original files
                if (absoluteFilePath.EndsWith(renamedFileNameSuffix))
                {
                    absoluteRenamedFilePath = absoluteFilePath;
                    absoluteOriginalFilePath = absoluteRenamedFilePath.Replace(renamedFileNameSuffix, fileExtension);
                }
                else
                {
                    absoluteOriginalFilePath = absoluteFilePath;
                    absoluteRenamedFilePath = absoluteOriginalFilePath.Replace(fileExtension, renamedFileNameSuffix);

                    // As files might already have been renamed in a previous run, we only create ones which are not existing yet
                    if (!File.Exists(absoluteRenamedFilePath))
                    {
                        File.Copy(absoluteOriginalFilePath, absoluteRenamedFilePath);
                    }

                    // TODO maybe even delete the original file once everything is stable
                }

                string relativeOriginalFilePath = absoluteOriginalFilePath.Substring(absoluteOriginalFilePath.LastIndexOf(filePathSeparator) + filePathSeparator.Length);
                string relativeRenamedFilePath = absoluteRenamedFilePath.Substring(absoluteRenamedFilePath.LastIndexOf(filePathSeparator) + filePathSeparator.Length);

                if (!relativeOriginalFilePathToRelativeRenamedFilePathMap.ContainsKey(relativeOriginalFilePath))
                {
                    relativeOriginalFilePathToRelativeRenamedFilePathMap.Add(relativeOriginalFilePath, relativeRenamedFilePath);
                }

                absoluteRenamedFilePaths.Add(absoluteRenamedFilePath);
            }

            return (relativeOriginalFilePathToRelativeRenamedFilePathMap, absoluteRenamedFilePaths);
        }
    }
}
