using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using WolvenKit.CR2W;
using WolvenKit.CR2W.BatchProcessors;
using WolvenKit.CR2W.FilePatchers;
using WolvenKit.CR2W.Types;
using WolvenKit.Mod;

namespace WolvenKit.Forms
{
    public partial class frmW2XBatchProcessor : Form
    {
        private const string FileExtensionW2Ent = ".w2ent";
        private const string FileExtensionW2L = ".w2l";
        private const string FileExtensionW2Mesh = ".w2mesh";
        private const string FileExtensionW2P = ".w2p";

        private W3Mod activeMod;
        private frmOutput log;

        public frmW2XBatchProcessor(W3Mod activeMod)
        {
            this.activeMod = activeMod;

            InitializeComponent();

            ShowLog();
        }

        private void frmW2XBatchProcessor_Load(object sender, EventArgs e)
        {

        }

        private void ShowLog()
        {
            if (log == null || log.IsDisposed)
            {
                log = new frmOutput();
                log.Show(dockPanel, DockState.DockBottom);
            }

            log.Focus();
        }

        private void startBatch_Click(object sender, EventArgs e)
        {
            ShowLog();

            log.AddText("Initializing file lists...\n", frmOutput.Logtype.Normal);

            W2XFileHandler w2XFileHandler = new W2XFileHandler();

            w2XFileHandler.Initialize(activeMod.Files, activeMod.FileDirectory, activeMod.ModDirectory, activeMod.DlcDirectory, MainController.Get());

            foreach (string errorMessage in w2XFileHandler.ErrorMessages)
            {
                log.AddText(errorMessage + "\n", frmOutput.Logtype.Error);
            }

            List<string> w2EntFilePathsInMod = new List<string>();
            List<string> w2MeshFilePathsInMod = new List<string>();
            List<string> w2PFilePathsInMod = new List<string>();

            foreach (string relativeModFilePath in activeMod.Files)
            {
                string absoluteModFilePath = activeMod.FileDirectory + Path.DirectorySeparatorChar + relativeModFilePath;

                switch (Path.GetExtension(absoluteModFilePath))
                {
                    case FileExtensionW2Ent:
                        w2EntFilePathsInMod.Add(absoluteModFilePath);
                        break;
                    case FileExtensionW2L:
                        // TODO add
                        continue;
                    case FileExtensionW2Mesh:
                        w2MeshFilePathsInMod.Add(absoluteModFilePath);
                        continue;
                    case FileExtensionW2P:
                        w2PFilePathsInMod.Add(absoluteModFilePath);
                        break;
                    default:
                        continue;
                }
            }

            IEnumerable<string> w2EntFilePathsInModButNotInFires = w2EntFilePathsInMod.Except(w2XFileHandler.W2EntFilePathsForFires);
            IEnumerable<string> w2EntFilePathsInFiresButNotInMod = w2XFileHandler.W2EntFilePathsForFires.Except(w2EntFilePathsInMod);

            IEnumerable<string> w2MeshFilePathsInModButNotInFires = w2MeshFilePathsInMod.Except(w2XFileHandler.W2MeshFilePathsForFires);
            IEnumerable<string> w2MeshFilePathsInFiresButNotInMod = w2XFileHandler.W2MeshFilePathsForFires.Except(w2MeshFilePathsInMod);

            IEnumerable<string> w2PFilePathsInModButNotInFires = w2PFilePathsInMod.Except(w2XFileHandler.W2PFilePathsForFires);
            IEnumerable<string> w2PFilePathsInFiresButNotInMod = w2XFileHandler.W2PFilePathsForFires.Except(w2PFilePathsInMod);

            //log.AddText("w2EntFilePathsInModButNotInFires:\n", frmOutput.Logtype.Important);
            //foreach (string file in w2EntFilePathsInModButNotInFires)
            //{
            //    log.AddText(file + "\n", frmOutput.Logtype.Error);
            //}

            log.AddText("w2EntFilePathsInFiresButNotInMod:\n", frmOutput.Logtype.Important);
            foreach (string file in w2EntFilePathsInFiresButNotInMod)
            {
                log.AddText(file + "\n", frmOutput.Logtype.Error);
            }

            //log.AddText("w2MeshFilePathsInModButNotInFires:\n", frmOutput.Logtype.Important);
            //foreach (string file in w2MeshFilePathsInModButNotInFires)
            //{
            //    log.AddText(file + "\n", frmOutput.Logtype.Error);
            //}

            log.AddText("w2MeshFilePathsInFiresButNotInMod:\n", frmOutput.Logtype.Important);
            foreach (string file in w2MeshFilePathsInFiresButNotInMod)
            {
                log.AddText(file + "\n", frmOutput.Logtype.Error);
            }

            //log.AddText("w2PFilePathsInModButNotInFires:\n", frmOutput.Logtype.Important);
            //foreach (string file in w2PFilePathsInModButNotInFires)
            //{
            //    log.AddText(file + "\n", frmOutput.Logtype.Error);
            //}

            log.AddText("w2PFilePathsInFiresButNotInMod:\n", frmOutput.Logtype.Important);
            foreach (string file in w2PFilePathsInFiresButNotInMod)
            {
                log.AddText(file + "\n", frmOutput.Logtype.Error);
            }

            //return;

            // TODO maybe also patch fire w2p files which are not found in w2ent files
            // TODO maybe also check w2l files for fires as apparently they can also reference w2p files directly

            (Dictionary<string, string> relativeOriginalW2MeshFilePathToRelativeRenamedW2MeshFilePathMap, List<string> absoluteRenamedW2MeshFilePaths)
                = W2XFileHandler.CopyAndRenameW2MeshFiles(w2XFileHandler.W2MeshFilePathsForFiresRenamed, activeMod.DlcDirectory);

            W2XFileHandler.CopyAndRenameW2MeshBufferFiles(w2XFileHandler.W2MeshBufferFilePathsForFiresRenamed, activeMod.DlcDirectory);

            absoluteRenamedW2MeshFilePaths.AddRange(w2XFileHandler.W2MeshFilePathsForFires);

            (Dictionary<string, string> relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap, List<string> absoluteRenamedW2PFilePaths)
                = W2XFileHandler.CopyAndRenameW2PFiles(w2XFileHandler.W2PFilePathsForFires, activeMod.DlcDirectory);

            log.AddText("Patching w2ent files for increased draw distance...\n", frmOutput.Logtype.Normal);
            List<string> absoluteCollisionMeshFilePaths = patchW2EntFilesForFires(w2XFileHandler.W2EntFilePathsForFires, activeMod.ModDirectory, activeMod.DlcDirectory, relativeOriginalW2MeshFilePathToRelativeRenamedW2MeshFilePathMap,
                relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap);

            log.AddText("Patching w2mesh files for increased draw distance...\n", frmOutput.Logtype.Normal);
            patchW2MeshFilesForFires(absoluteRenamedW2MeshFilePaths, null);

            log.AddText("Patching w2ent files used for collisions...\n", frmOutput.Logtype.Normal);
            patchW2MeshFilesForFires(absoluteCollisionMeshFilePaths, new W2MeshSettings(1, 1, 1));

            log.AddText("Patching w2p files for increased draw distance...\n", frmOutput.Logtype.Normal);
            patchW2PFilesForFires(absoluteRenamedW2PFilePaths);

            log.AddText("Cleaning up a few things...\n", frmOutput.Logtype.Normal);
            cleanUp(activeMod.ModDirectory, activeMod.DlcDirectory, w2XFileHandler.W2PFilePathsForFires);
        }

        private List<string> patchW2EntFilesForFires(List<string> w2EntFilePathsForFires, string modDirectory, string dlcDirectory, Dictionary<string, string> relativeOriginalW2MeshFilePathToRelativeRenamedW2MeshFilePathMap,
            Dictionary<string, string> relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap)
        {
            List<string> absoluteCollisionMeshFilePaths = new List<string>();

            W2EntFilePatcher w2EntFilePatcher = new W2EntFilePatcher(MainController.Get());

            foreach (string w2EntFilePathForFire in w2EntFilePathsForFires)
            {
                if (!File.Exists(w2EntFilePathForFire))
                {
                    log.AddText("File '" + w2EntFilePathForFire + "' does not exist.\n", frmOutput.Logtype.Error);

                    continue;
                }

                try
                {
                    (List<string> relativeCollisionMeshFilePaths, List<string> relativeMeshFilePathsToDelete) = w2EntFilePatcher.PatchForIncreasedDrawDistance(w2EntFilePathForFire,
                        relativeOriginalW2MeshFilePathToRelativeRenamedW2MeshFilePathMap, relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap, new W2EntSettings(Path.GetFileName(w2EntFilePathForFire)));

                    foreach (string relativeCollisionMeshFilePath in relativeCollisionMeshFilePaths)
                    {
                        string initialPath = relativeCollisionMeshFilePath.StartsWith(W2XFileHandler.PathDLC) ? dlcDirectory : modDirectory;

                        absoluteCollisionMeshFilePaths.Add(initialPath + Path.DirectorySeparatorChar + W2XFileHandler.PathBundle + Path.DirectorySeparatorChar + relativeCollisionMeshFilePath);
                    }

                    foreach (string relativeMeshFilePathToDelete in relativeMeshFilePathsToDelete)
                    {
                        string initialPath = relativeMeshFilePathToDelete.StartsWith(W2XFileHandler.PathDLC) ? dlcDirectory : modDirectory;

                        File.Delete(initialPath + Path.DirectorySeparatorChar + W2XFileHandler.PathBundle + Path.DirectorySeparatorChar + relativeMeshFilePathToDelete);
                        File.Delete(initialPath + Path.DirectorySeparatorChar + W2XFileHandler.PathBundle + Path.DirectorySeparatorChar + relativeMeshFilePathToDelete + W2XFileHandler.FileExtensionSuffixW2MeshBuffer);
                    }
                }
                catch (Exception e)
                {
                    log.AddText("An unexpected exception occurred while processing file '" + w2EntFilePathForFire + "': " + e.Message + "\n", frmOutput.Logtype.Error);

                    continue;
                }

                log.AddText("Successfully processed file '" + w2EntFilePathForFire + "\n", frmOutput.Logtype.Success);
            }

            return absoluteCollisionMeshFilePaths.Distinct().ToList();
        }

        private void patchW2MeshFilesForFires(List<string> w2MeshFilePathsForFires, W2MeshSettings w2MeshSettings)
        {
            W2MeshFilePatcher w2MeshFilePatcher = new W2MeshFilePatcher(MainController.Get());

            foreach (string w2MeshFilePathForFire in w2MeshFilePathsForFires)
            {
                if (!File.Exists(w2MeshFilePathForFire))
                {
                    log.AddText("File '" + w2MeshFilePathForFire + "' does not exist.\n", frmOutput.Logtype.Error);

                    continue;
                }

                try
                {
                    w2MeshFilePatcher.PatchDrawDistance(w2MeshFilePathForFire, w2MeshSettings != null ? w2MeshSettings : new W2MeshSettings(Path.GetFileName(w2MeshFilePathForFire)));
                }
                catch (Exception e)
                {
                    log.AddText("An unexpected exception occurred while processing file '" + w2MeshFilePathForFire + "': " + e.Message + "\n", frmOutput.Logtype.Error);

                    continue;
                }

                log.AddText("Successfully processed file '" + w2MeshFilePathForFire + "\n", frmOutput.Logtype.Success);
            }
        }

        private void patchW2PFilesForFires(List<string> w2PFilePathsForFires)
        {
            W2AFilePatcher w2PFilePatcher = new W2AFilePatcher(MainController.Get());

            foreach (string w2PFilePathForFire in w2PFilePathsForFires)
            {
                if (!File.Exists(w2PFilePathForFire))
                {
                    log.AddText("File '" + w2PFilePathForFire + "' does not exist.\n", frmOutput.Logtype.Error);

                    continue;
                }

                try
                {
                    w2PFilePatcher.PatchForIncreasedDrawDistance(w2PFilePathForFire, new W2PSettings(Path.GetFileName(w2PFilePathForFire)));
                }
                catch (Exception e)
                {
                    log.AddText("An unexpected exception occurred while processing file '" + w2PFilePathForFire + "': " + e.Message + "\n", frmOutput.Logtype.Error);

                    continue;
                }

                log.AddText("Successfully processed file '" + w2PFilePathForFire + "\n", frmOutput.Logtype.Success);
            }
        }

        private static void cleanUp(string modDirectory, string dlcDirectory, List<string> absolutOriginalW2PFilePathsForFires)
        {
            foreach (string absolutOriginalW2PFilePathForFires in absolutOriginalW2PFilePathsForFires)
            {
                if (!absolutOriginalW2PFilePathForFires.EndsWith("_ilod.w2p") && !absolutOriginalW2PFilePathForFires.EndsWith("h_shrine_eternal_fire_wide_materialswap.w2p")
                     && !absolutOriginalW2PFilePathForFires.EndsWith("torch_moving.w2p") && !absolutOriginalW2PFilePathForFires.EndsWith("cs001_fireplace.w2p")
                      && !absolutOriginalW2PFilePathForFires.EndsWith("p_fire_medium_subuv_spot_smaller.w2p") && !absolutOriginalW2PFilePathForFires.EndsWith("fireplace_looped.w2p")
                       && !absolutOriginalW2PFilePathForFires.EndsWith("fireplace_looped_3.w2p") && !absolutOriginalW2PFilePathForFires.EndsWith("fireplace_smoke_embers_distort_bathhouse.w2p")
                       && !absolutOriginalW2PFilePathForFires.EndsWith("torch_hand_fire.w2p") && !absolutOriginalW2PFilePathForFires.EndsWith("torch_fx1.w2p")
                       && !absolutOriginalW2PFilePathForFires.EndsWith("mq1046_fire_house.w2p") && !absolutOriginalW2PFilePathForFires.EndsWith("q103_barons_stable_fire_4.w2p")
                       && !absolutOriginalW2PFilePathForFires.EndsWith("medium_fire_anim.w2p"))
                    File.Delete(absolutOriginalW2PFilePathForFires);
            }

            deleteEmptyDirectories(modDirectory);
            deleteEmptyDirectories(dlcDirectory);
        }

        private static void deleteEmptyDirectories(string rootDirectory)
        {
            foreach (var directory in Directory.GetDirectories(rootDirectory))
            {
                deleteEmptyDirectories(directory);

                if (Directory.GetDirectories(directory).Length == 0 && Directory.GetFiles(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {

        }
    }
}
