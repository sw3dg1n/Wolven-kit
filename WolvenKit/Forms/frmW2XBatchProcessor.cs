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

            foreach (String relativeModFilePath in activeMod.Files)
            {
                String absoluteModFilePath = activeMod.FileDirectory + Path.DirectorySeparatorChar + relativeModFilePath;

                if (!File.Exists(absoluteModFilePath))
                {
                    // TODO show in log
                    continue;
                }

                log.AddText(absoluteModFilePath + "\n", frmOutput.Logtype.Normal);

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
    }
}
