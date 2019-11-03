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
        public const string FileExtensionW2MeshBuffer = ".1.buffer";
        public const string FileExtensionW2P = ".w2p";

        public const string FileNameSuffixILOD = "_ilod";

        public const string PathBundle = "Bundle";
        public const string PathDLC = "dlc";
        public const string PathILOD = "ilod";

        public List<string> ErrorMessages { get; private set; }
        public List<string> W2EntFilePathsForFires { get; private set; }
        public List<string> W2MeshFilePathsForFires { get; private set; }
        public List<string> W2MeshBufferFilePathsForFires { get; private set; }
        public List<string> W2PFilePathsForFires { get; private set; }

        public W2XFileHandler()
        {
            ErrorMessages = new List<string>();
            W2EntFilePathsForFires = new List<string>();
            W2MeshFilePathsForFires = new List<string>();
            W2MeshBufferFilePathsForFires = new List<string>();
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

                        if (w2EntFile == null)
                        {
                            throw new System.InvalidOperationException("File '" + absoluteModFilePath + "' could not be loaded.");
                        }

                        (List<CByteArrayContainer> sharedDataBuffersForFires, CByteArrayContainer flatCompiledData) = W2EntFilePatcher.ReadSharedDataBuffersAndFlatCompiledDataForFires(w2EntFile);
                        bool w2EntFileContainsFires = false;

                        if (sharedDataBuffersForFires != null)
                        {
                            foreach (CByteArrayContainer sharedDataBufferForFire in sharedDataBuffersForFires)
                            {
                                List<string> w2PFilePathsForFires = W2EntFilePatcher.GetW2PFilePathsForFires(sharedDataBufferForFire, absoluteModFilePath, modDirectory, dlcDirectory);

                                if (w2PFilePathsForFires.Any())
                                {
                                    W2PFilePathsForFires.AddRange(w2PFilePathsForFires);
                                    W2EntFilePathsForFires.Add(absoluteModFilePath);

                                    w2EntFileContainsFires = true;
                                }
                            }
                        }

                        if (w2EntFileContainsFires)
                        {
                            CByteArrayContainer streamingDataBufferForFires = W2EntFilePatcher.ReadStreamingDataBufferForFires(w2EntFile);

                            if (streamingDataBufferForFires != null)
                            {
                                List<string> w2MeshFilePathsForFires = W2EntFilePatcher.GetW2MeshFilePathsForFires(streamingDataBufferForFires, absoluteModFilePath, modDirectory, dlcDirectory);

                                if (w2MeshFilePathsForFires.Any())
                                {
                                    foreach (string w2MeshFilePathForFire in w2MeshFilePathsForFires)
                                    {
                                        string w2MeshBufferFilePathForFire = w2MeshFilePathForFire + FileExtensionW2MeshBuffer;

                                        W2MeshFilePathsForFires.Add(w2MeshFilePathForFire);
                                        W2MeshBufferFilePathsForFires.Add(w2MeshBufferFilePathForFire);
                                    }
                                }
                                else
                                {
                                    ErrorMessages.Add("No mesh files could be found in file '" + absoluteModFilePath + "'.");
                                }
                            }
                            else
                            {
                                ErrorMessages.Add("No streaming data buffer could be found in file '" + absoluteModFilePath + "'.");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorMessages.Add("An unexpected exception occurred while processing file '" + relativeModFilePath + "': " + e.Message);

                    continue;
                }
            }

            // TODO this should simply be replaced with a set
            W2EntFilePathsForFires = W2EntFilePathsForFires.Distinct().ToList();
            W2MeshFilePathsForFires = W2MeshFilePathsForFires.Distinct().ToList();
            W2MeshBufferFilePathsForFires = W2MeshBufferFilePathsForFires.Distinct().ToList();
            W2PFilePathsForFires = W2PFilePathsForFires.Distinct().ToList();

            string w2entsource = "C:\\Users\\mkaltenb\\source\\repos\\fire_files\\w2ent";

            foreach (string fullPath in W2EntFilePathsForFires)
            {
                string newPath = w2entsource + fullPath.Substring(fullPath.IndexOf(PathBundle) + PathBundle.Length);

                if (File.Exists(fullPath) && !File.Exists(newPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    File.Copy(fullPath, newPath);
                }
            }

            string w2meshsource = "C:\\Users\\mkaltenb\\source\\repos\\fire_files\\w2mesh";

            foreach (string fullPath in W2MeshFilePathsForFires)
            {
                string newPath = w2meshsource + fullPath.Substring(fullPath.IndexOf(PathBundle) + PathBundle.Length);

                if (File.Exists(fullPath) && !File.Exists(newPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    File.Copy(fullPath, newPath);
                }
            }

            foreach (string fullPath in W2MeshBufferFilePathsForFires)
            {
                string newPath = w2meshsource + fullPath.Substring(fullPath.IndexOf(PathBundle) + PathBundle.Length);

                if (File.Exists(fullPath) && !File.Exists(newPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    File.Copy(fullPath, newPath);
                }
            }

            string w2psource = "C:\\Users\\mkaltenb\\source\\repos\\fire_files\\w2p";

            foreach (string fullPath in W2PFilePathsForFires)
            {
                string newPath = w2psource + fullPath.Substring(fullPath.IndexOf(PathBundle) + PathBundle.Length);

                if (File.Exists(fullPath) && !File.Exists(newPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    File.Copy(fullPath, newPath);
                }
            }
        }

        public static (Dictionary<string, string> relativeOriginalFilePathToRelativeRenamedFilePathMap, List<string> absoluteRenamedFilePaths) CopyAndRenameW2PFiles(List<string> absoluteW2PFilePaths, string dlcDirectory)
        {
            return CopyAndRenameFiles(absoluteW2PFilePaths, dlcDirectory, FileExtensionW2P);
        }

        private static (Dictionary<string, string> relativeOriginalFilePathToRelativeRenamedFilePathMap, List<string> absoluteRenamedFilePaths) CopyAndRenameFiles(List<string> absoluteFilePaths, string dlcDirectory, string fileExtension)
        {
            Dictionary<string, string> relativeOriginalFilePathToRelativeRenamedFilePathMap = new Dictionary<string, string>();
            List<string> absoluteRenamedFilePaths = new List<string>();

            string rootDirectoryDLCBundle = dlcDirectory + Path.DirectorySeparatorChar + PathBundle;
            string renamedFileNameSuffix = FileNameSuffixILOD + fileExtension;
            string filePathSeparator = Path.DirectorySeparatorChar + PathBundle + Path.DirectorySeparatorChar;

            foreach (string absoluteOriginalFilePath in absoluteFilePaths)
            {
                if (!File.Exists(absoluteOriginalFilePath))
                {
                    continue;
                }
                
                string relativeOriginalFilePath;
                string relativeRenamedFilePath;
                string absoluteRenamedFilePath;

                // If the file did already get renamed in a previous run, we still wanna add it the the map and list so that the file still gets processed
                if (absoluteOriginalFilePath.EndsWith(renamedFileNameSuffix))
                {
                    relativeOriginalFilePath = absoluteOriginalFilePath.Substring(absoluteOriginalFilePath.LastIndexOf(filePathSeparator) + filePathSeparator.Length);
                    relativeRenamedFilePath = relativeOriginalFilePath;
                    absoluteRenamedFilePath = absoluteOriginalFilePath;
                }
                else
                {
                    relativeOriginalFilePath = absoluteOriginalFilePath.Substring(absoluteOriginalFilePath.LastIndexOf(filePathSeparator) + filePathSeparator.Length);
                    relativeRenamedFilePath = PathDLC + Path.DirectorySeparatorChar + PathILOD + Path.DirectorySeparatorChar + relativeOriginalFilePath.Replace(fileExtension, renamedFileNameSuffix);
                    absoluteRenamedFilePath = rootDirectoryDLCBundle + Path.DirectorySeparatorChar + relativeRenamedFilePath;

                    if (!File.Exists(absoluteRenamedFilePath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(absoluteRenamedFilePath));
                        File.Copy(absoluteOriginalFilePath, absoluteRenamedFilePath);
                    }

                    // TODO probably delete the original file once everything is stable
                }

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
