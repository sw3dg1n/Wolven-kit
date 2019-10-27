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

            // TODO
            // List<string> relativeW2EntFilePathsForFires = GetRelativeW2EntFilePathsForFires(activeMod.Files);

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

        private List<string> GetRelativeW2EntFilePathsForFires(List<string> relativeModFilePaths)
        {
            List<string> relativeW2EntFilePathsForFires = new List<string>();
            List<string> relativeW2PFilePathsForFires = new List<string>();

            foreach (string relativeModFilePath in activeMod.Files)
            {
                try
                {
                    string absoluteModFilePath = activeMod.FileDirectory + Path.DirectorySeparatorChar + relativeModFilePath;

                    if (!File.Exists(absoluteModFilePath))
                    {
                        log.AddText("File '" + absoluteModFilePath + "' does not exist.\n", frmOutput.Logtype.Error);

                        continue;
                    }

                    if (Path.GetExtension(absoluteModFilePath).Equals(FileExtensionW2Ent))
                    {
                        CR2WFile w2EntFile = W2EntFilePatcher.ReadW2EntFile(absoluteModFilePath, MainController.Get());
                        List<SharedDataBuffer> sharedDataBuffersForFires = W2EntFilePatcher.ReadSharedDataBuffersForFires(w2EntFile);

                        if (sharedDataBuffersForFires == null || !sharedDataBuffersForFires.Any())
                        {
                            continue;
                        }

                        foreach (SharedDataBuffer sharedDataBufferForFire in sharedDataBuffersForFires)
                        {
                            if (W2EntFilePatcher.SharedDataBufferReferencesFireW2PFile(sharedDataBufferForFire, absoluteModFilePath))
                            {
                                relativeW2EntFilePathsForFires.Add(relativeModFilePath);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    log.AddText(e.Message + "\n", frmOutput.Logtype.Error);

                    continue;
                }
            }

            return relativeW2EntFilePathsForFires;
        }

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {

        }
    }
}
