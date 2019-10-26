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

                string fileExtension = Path.GetExtension(absoluteModFilePath);

                if (!fileExtension.Equals(".w2ent") && !fileExtension.Equals(".w2p") && !fileExtension.Equals(".w2mesh") && !fileExtension.Equals(".w2l")  )
                {
                    continue;
                }

                CR2WFile file = loadFile(absoluteModFilePath);

                if (file == null)
                {
                    log.AddText("File '" + absoluteModFilePath + "' could not be loaded.\n", frmOutput.Logtype.Error);

                    continue;
                }

                log.AddText("Successfully loaded file '" + absoluteModFilePath + "\n", frmOutput.Logtype.Success);

                
                //var doc = new frmCR2WDocument();

                //try
                //{
                //    doc.LoadFile(absoluteModFilePath);
                //}
                //catch (InvalidFileTypeException ex)
                //{

                //    // TODO log ex.Message

                //    doc.Dispose();

                //    continue;
                //}
                //catch (MissingTypeException ex)
                //{
                //    // TODO log ex.Message

                //    doc.Dispose();

                //    continue;
                //}
            }
        }

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {

        }

        private CR2WFile loadFile(string filePath)
        {
            CR2WFile file = null;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    file = new CR2WFile(reader)
                    {
                        FileName = filePath,
                        LocalizedStringSource = MainController.Get()
                    };
                }
            }

            return file;
        }
    }
}
