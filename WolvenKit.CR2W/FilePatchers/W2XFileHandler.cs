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
        public const string FileExtensionSuffixW2MeshBuffer = ".1.buffer";

        public const string FileExtensionW2Ent = ".w2ent";
        public const string FileExtensionW2L = ".w2l";
        public const string FileExtensionW2Mesh = ".w2mesh";
        public const string FileExtensionW2MeshBuffer = FileExtensionW2Mesh + FileExtensionSuffixW2MeshBuffer;
        public const string FileExtensionW2P = ".w2p";

        public const string FileNameSuffixILOD = "_ilod";

        public const string PathBundle = "Bundle";
        public const string PathDLC = "dlc";
        public const string PathILOD = "ilod";

        public List<string> ErrorMessages { get; private set; }
        public List<string> W2EntFilePathsForFires { get; private set; }
        public List<string> W2MeshFilePathsForFires { get; private set; }
        public List<string> W2MeshBufferFilePathsForFires { get; private set; }
        public List<string> W2MeshFilePathsForFiresRenamed { get; private set; }
        public List<string> W2MeshBufferFilePathsForFiresRenamed { get; private set; }
        public List<string> W2PFilePathsForFires { get; private set; }

        public W2XFileHandler()
        {
            ErrorMessages = new List<string>();
            W2EntFilePathsForFires = new List<string>();
            W2MeshFilePathsForFires = new List<string>();
            W2MeshBufferFilePathsForFires = new List<string>();
            W2MeshFilePathsForFiresRenamed = new List<string>();
            W2MeshBufferFilePathsForFiresRenamed = new List<string>();
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

                        List<string> w2PFilePathsForFires = new List<string>();

                        (List<CByteArrayContainer> sharedDataBuffersForFires, CByteArrayContainer flatCompiledData) = W2EntFilePatcher.ReadSharedDataBuffersAndFlatCompiledDataForFires(w2EntFile);

                        if (sharedDataBuffersForFires != null)
                        {
                            foreach (CByteArrayContainer sharedDataBufferForFire in sharedDataBuffersForFires)
                            {
                                w2PFilePathsForFires.AddRange(W2EntFilePatcher.GetW2PFilePathsForFires(sharedDataBufferForFire, flatCompiledData, absoluteModFilePath, modDirectory, dlcDirectory));
                            }
                        }
                        else
                        {
                            w2PFilePathsForFires.AddRange(W2EntFilePatcher.GetW2PFilePathsForFires(null, flatCompiledData, absoluteModFilePath, modDirectory, dlcDirectory));
                        }

                        if (w2PFilePathsForFires.Any() || relativeModFilePath.Contains("hanging_lamp") || relativeModFilePath.Contains("lantern"))
                        {
                            W2EntFilePathsForFires.Add(absoluteModFilePath);

                            if (w2PFilePathsForFires.Any())
                            {
                                W2PFilePathsForFires.AddRange(w2PFilePathsForFires);
                            }

                            List<string> w2MeshFilePathsForFires = new List<string>();

                            CByteArrayContainer streamingDataBufferForFires = W2EntFilePatcher.ReadStreamingDataBufferForFires(w2EntFile);

                            if (streamingDataBufferForFires != null)
                            {
                                w2MeshFilePathsForFires.AddRange(W2EntFilePatcher.GetW2MeshFilePathsForFires(streamingDataBufferForFires.Content.chunks, absoluteModFilePath, modDirectory, dlcDirectory));
                            }

                            w2MeshFilePathsForFires.AddRange(W2EntFilePatcher.GetW2MeshFilePathsForFires(w2EntFile.chunks, absoluteModFilePath, modDirectory, dlcDirectory));

                            if (w2MeshFilePathsForFires.Any())
                            {
                                foreach (string w2MeshFilePathForFire in w2MeshFilePathsForFires)
                                {
                                    string w2MeshFileName = Path.GetFileName(w2MeshFilePathForFire);
                                    string w2MeshBufferFilePathForFire = w2MeshFilePathForFire + FileExtensionSuffixW2MeshBuffer;

                                    if (w2MeshFileName.Contains("braziers_floor") || w2MeshFileName.Contains("braziers_wall") || w2MeshFileName.Contains("torch_hand") || w2MeshFileName.Contains("pile_of_bodies")
                                        || (w2MeshFileName.Contains("hanging_lamp") && !w2MeshFileName.Contains("holder"))
                                        || (w2MeshFileName.Contains("shrine_of_ethernal_fire_altar") && !w2MeshFileName.Contains("small"))
                                        || w2MeshFileName.Contains("shipyard_pole_support") || w2MeshFileName.Contains("chandelier_small") || w2MeshFileName.Contains("bonfire_large")
                                        || (w2MeshFileName.Contains("candle") && (!w2MeshFileName.Contains("holder") || w2MeshFileName.Contains("small")))
                                        || w2MeshFileName.Contains("torch_wall") || w2MeshFileName.Contains("lantern_red.w2mesh") || w2MeshFileName.Contains("lantern_red_table.w2mesh"))
                                    {
                                        W2MeshFilePathsForFiresRenamed.Add(w2MeshFilePathForFire);
                                        W2MeshBufferFilePathsForFiresRenamed.Add(w2MeshBufferFilePathForFire);
                                    }
                                    else
                                    {
                                        W2MeshFilePathsForFires.Add(w2MeshFilePathForFire);
                                        W2MeshBufferFilePathsForFires.Add(w2MeshBufferFilePathForFire);
                                    }
                                }
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

            W2EntFilePathsForFires = W2EntFilePathsForFires.Distinct().ToList();
            W2MeshFilePathsForFires = W2MeshFilePathsForFires.Distinct().ToList();
            W2MeshBufferFilePathsForFires = W2MeshBufferFilePathsForFires.Distinct().ToList();
            W2MeshFilePathsForFiresRenamed = W2MeshFilePathsForFiresRenamed.Distinct().ToList();
            W2MeshBufferFilePathsForFiresRenamed = W2MeshBufferFilePathsForFiresRenamed.Distinct().ToList();
            W2PFilePathsForFires = W2PFilePathsForFires.Distinct().ToList();

            //string w2entsource = "D:\\_\\w2ent";

            //foreach (string fullPath in W2EntFilePathsForFires)
            //{
            //    string newPath = w2entsource + fullPath.Substring(fullPath.IndexOf(PathBundle) + PathBundle.Length);

            //    if (File.Exists(fullPath) && !File.Exists(newPath))
            //    {
            //        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            //        File.Copy(fullPath, newPath);
            //    }
            //}

            //string w2meshsource = "D:\\_\\w2mesh";

            //foreach (string fullPath in W2MeshFilePathsForFires)
            //{
            //    string newPath = w2meshsource + fullPath.Substring(fullPath.IndexOf(PathBundle) + PathBundle.Length);

            //    if (File.Exists(fullPath) && !File.Exists(newPath))
            //    {
            //        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            //        File.Copy(fullPath, newPath);
            //    }
            //}

            //foreach (string fullPath in W2MeshBufferFilePathsForFires)
            //{
            //    string newPath = w2meshsource + fullPath.Substring(fullPath.IndexOf(PathBundle) + PathBundle.Length);

            //    if (File.Exists(fullPath) && !File.Exists(newPath))
            //    {
            //        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            //        File.Copy(fullPath, newPath);
            //    }
            //}

            //string w2psource = "D:\\_\\w2p";

            //foreach (string fullPath in W2PFilePathsForFires)
            //{
            //    string newPath = w2psource + fullPath.Substring(fullPath.IndexOf(PathBundle) + PathBundle.Length);

            //    if (File.Exists(fullPath) && !File.Exists(newPath))
            //    {
            //        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            //        File.Copy(fullPath, newPath);
            //    }
            //}
        }

        public static (Dictionary<string, string> relativeOriginalFilePathToRelativeRenamedFilePathMap, List<string> absoluteRenamedFilePaths) CopyAndRenameW2MeshFiles(List<string> absoluteW2MeshFilePaths, string dlcDirectory)
        {
            return CopyAndRenameFiles(absoluteW2MeshFilePaths, dlcDirectory, FileExtensionW2Mesh);
        }
        public static (Dictionary<string, string> relativeOriginalFilePathToRelativeRenamedFilePathMap, List<string> absoluteRenamedFilePaths) CopyAndRenameW2MeshBufferFiles(List<string> absoluteW2MeshBufferFilePaths, string dlcDirectory)
        {
            return CopyAndRenameFiles(absoluteW2MeshBufferFilePaths, dlcDirectory, FileExtensionW2MeshBuffer);
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

                    // TODO delete the original file once everything is stable, only applicable for w2p files
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
