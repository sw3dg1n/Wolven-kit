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
    public partial class frmW2MeshLODBatchProcessor : Form
    {
        private const string FileExtensionW2Mesh = ".w2mesh";

        private W3Mod activeMod;
        private frmOutput log;

        public frmW2MeshLODBatchProcessor(W3Mod activeMod)
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

            int autoHideDistance;

            if (!int.TryParse(textBoxAutoHideDistance.Text, out autoHideDistance))
            {
                log.AddText("The specified autoHideDistance is not an integer.\n", frmOutput.Logtype.Error);
                return;
            }

            log.AddText("Initializing file lists...\n", frmOutput.Logtype.Normal);

            List<string> w2MeshFilePathsInMod = new List<string>();

            foreach (string relativeModFilePath in activeMod.Files)
            {
                string absoluteModFilePath = activeMod.FileDirectory + Path.DirectorySeparatorChar + relativeModFilePath;

                if (Path.GetExtension(absoluteModFilePath).Equals(FileExtensionW2Mesh))
                {
                    w2MeshFilePathsInMod.Add(absoluteModFilePath);
                }
            }

            log.AddText("Patching w2mesh files for increased draw distance and LOD...\n", frmOutput.Logtype.Normal);

            patchW2MeshFilesForFires(w2MeshFilePathsInMod, autoHideDistance);
        }

        private void patchW2MeshFilesForFires(List<string> w2MeshFilePathsForFires, float autoHideDistance)
        {
            W2MeshFileLODPatcher w2MeshFilePatcher = new W2MeshFileLODPatcher(MainController.Get());

            foreach (string w2MeshFilePathForFire in w2MeshFilePathsForFires)
            {
                if (!File.Exists(w2MeshFilePathForFire))
                {
                    log.AddText("File '" + w2MeshFilePathForFire + "' does not exist.\n", frmOutput.Logtype.Error);

                    continue;
                }

                try
                {
                    w2MeshFilePatcher.PatchDrawDistanceAndLOD(w2MeshFilePathForFire, autoHideDistance);
                }
                catch (Exception e)
                {
                    log.AddText("An unexpected exception occurred while processing file '" + w2MeshFilePathForFire + "': " + e.Message + "\n", frmOutput.Logtype.Error);

                    continue;
                }

                log.AddText("Successfully processed file '" + w2MeshFilePathForFire + "\n", frmOutput.Logtype.Success);
            }
        }

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {

        }
    }
}
