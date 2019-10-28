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

            W2XFileHandler w2XFileHandler = new W2XFileHandler();

            w2XFileHandler.Initialize(activeMod.Files, activeMod.FileDirectory, activeMod.ModDirectory, activeMod.DlcDirectory, MainController.Get());

            foreach (string errorMessage in w2XFileHandler.ErrorMessages)
            {
                log.AddText(errorMessage + "\n", frmOutput.Logtype.Error);
            }

            List<string> w2EntFilePathsInMod = new List<string>();
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
                        // TODO add
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

            IEnumerable<string> w2PFilePathsInModButNotInFires = w2PFilePathsInMod.Except(w2XFileHandler.W2PFilePathsForFires);
            IEnumerable<string> w2PFilePathsInFiresButNotInMod = w2XFileHandler.W2PFilePathsForFires.Except(w2PFilePathsInMod);

            log.AddText("w2EntFilePathsInModButNotInFires:\n", frmOutput.Logtype.Important);
            foreach (string file in w2EntFilePathsInModButNotInFires)
            {
                log.AddText(file + "\n", frmOutput.Logtype.Error);
            }

            log.AddText("w2EntFilePathsInFiresButNotInMod:\n", frmOutput.Logtype.Important);
            foreach (string file in w2EntFilePathsInFiresButNotInMod)
            {
                log.AddText(file + "\n", frmOutput.Logtype.Error);
            }

            log.AddText("w2PFilePathsInModButNotInFires:\n", frmOutput.Logtype.Important);
            foreach (string file in w2PFilePathsInModButNotInFires)
            {
                log.AddText(file + "\n", frmOutput.Logtype.Error);
            }

            log.AddText("w2PFilePathsInFiresButNotInMod:\n", frmOutput.Logtype.Important);
            foreach (string file in w2PFilePathsInFiresButNotInMod)
            {
                log.AddText(file + "\n", frmOutput.Logtype.Error);
            }

            return;

            foreach (string relativeModFilePath in activeMod.Files)
            {
                string absoluteModFilePath = activeMod.FileDirectory + Path.DirectorySeparatorChar + relativeModFilePath;

                if (!File.Exists(absoluteModFilePath))
                {
                    log.AddText("File '" + absoluteModFilePath + "' does not exist.\n", frmOutput.Logtype.Error);

                    continue;
                }

                W2XFilePatcher filePatcher;

                switch (Path.GetExtension(absoluteModFilePath))
                {
                    case FileExtensionW2Ent:
                        filePatcher = new W2EntFilePatcher(absoluteModFilePath, MainController.Get());
                        break;
                    case FileExtensionW2L:
                        // TODO add
                        continue;
                    case FileExtensionW2Mesh:
                        // TODO add
                        continue;
                    case FileExtensionW2P:
                        filePatcher = new W2PFilePatcher(absoluteModFilePath, MainController.Get());
                        break;
                    default:
                        continue;
                }

                try
                {
                    filePatcher.PatchForIncreasedDrawDistance();
                }
                catch (Exception ex)
                {
                    log.AddText(ex.Message + "\n", frmOutput.Logtype.Error);

                    continue;
                }

                log.AddText("Successfully processed file '" + absoluteModFilePath + "\n", frmOutput.Logtype.Success);
            }
        }

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {

        }
    }
}
