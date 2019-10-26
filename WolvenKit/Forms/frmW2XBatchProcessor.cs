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
        private W3Mod activeMod;
        private frmOutput log;

        public frmW2XBatchProcessor(W3Mod activeMod)
        {
            this.activeMod = activeMod;

            InitializeComponent();

            showLog();
        }

        private void frmW2XBatchProcessor_Load(object sender, EventArgs e)
        {

        }

        private void showLog()
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
            showLog();

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
                    case ".w2ent":
                        filePatcher = new W2EntFilePatcher(absoluteModFilePath, MainController.Get());
                        break;
                    case ".w2l":
                        // TODO add
                        continue;
                    case ".w2mesh":
                        // TODO add
                        continue;
                    case ".w2p":
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
