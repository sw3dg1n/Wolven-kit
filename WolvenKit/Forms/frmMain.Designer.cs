﻿using System.ComponentModel;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace WolvenKit
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.newmodButton = new System.Windows.Forms.ToolStripButton();
            this.openmodButton = new System.Windows.Forms.ToolStripButton();
            this.openfileButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.saveallButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btPack = new System.Windows.Forms.ToolStripButton();
            this.rungameToolStrip = new System.Windows.Forms.ToolStripDropDownButton();
            this.launchGameForDebuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packProjectAndLaunchGameCustomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchWithCustomParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packProjectAndRunGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.iconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCr2wToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractCollisioncacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fbxWithCollisionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nvidiaClothFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ModscriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modwwise = new System.Windows.Forms.ToolStripMenuItem();
            this.ModchunkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dLCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DLCScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dlcwwise = new System.Windows.Forms.ToolStripMenuItem();
            this.DLCChunkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFileFromBundleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFileFromOtherModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createPackedInstallerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packageInstallerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stringsEncoderGUIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameDebuggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCreatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderW2meshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verifyFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joinOurDiscordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tutorialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.witcherIIIModdingToolLicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportABugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recordStepsToReproduceBugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutRedkit2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maximizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.statusLBL = new System.Windows.Forms.ToolStripLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.richpresenceworker = new System.ComponentModel.BackgroundWorker();
            this.visualStudioToolStripExtender1 = new WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender(this.components);
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.LeftToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newmodButton,
            this.openmodButton,
            this.openfileButton,
            this.toolStripSeparator1,
            this.saveButton,
            this.saveallButton,
            this.toolStripButton7,
            this.toolStripSeparator2,
            this.btPack,
            this.rungameToolStrip});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStrip1.Size = new System.Drawing.Size(41, 553);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "topTS";
            // 
            // newmodButton
            // 
            this.newmodButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newmodButton.Image = global::WolvenKit.Properties.Resources.NewSolutionFolder_16x;
            this.newmodButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newmodButton.Name = "newmodButton";
            this.newmodButton.Size = new System.Drawing.Size(37, 24);
            this.newmodButton.Text = "New mod";
            this.newmodButton.Click += new System.EventHandler(this.newModToolStripMenuItem_Click);
            // 
            // openmodButton
            // 
            this.openmodButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openmodButton.Image = global::WolvenKit.Properties.Resources.OpenFolder_16x;
            this.openmodButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openmodButton.Name = "openmodButton";
            this.openmodButton.Size = new System.Drawing.Size(37, 24);
            this.openmodButton.Text = "Open mod";
            this.openmodButton.Click += new System.EventHandler(this.openModToolStripMenuItem_Click);
            // 
            // openfileButton
            // 
            this.openfileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openfileButton.Image = global::WolvenKit.Properties.Resources.OpenFile_16x;
            this.openfileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openfileButton.Name = "openfileButton";
            this.openfileButton.Size = new System.Drawing.Size(37, 24);
            this.openfileButton.Text = "Open file";
            this.openfileButton.Click += new System.EventHandler(this.tbtOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(37, 6);
            // 
            // saveButton
            // 
            this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveButton.Image = global::WolvenKit.Properties.Resources.SaveStatusBar1_16x_c;
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(37, 24);
            this.saveButton.Text = "Save";
            this.saveButton.Click += new System.EventHandler(this.tbtSave_Click);
            // 
            // saveallButton
            // 
            this.saveallButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveallButton.Image = global::WolvenKit.Properties.Resources.SaveAll_16x;
            this.saveallButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveallButton.Name = "saveallButton";
            this.saveallButton.Size = new System.Drawing.Size(37, 24);
            this.saveallButton.Text = "toolStripButton5";
            this.saveallButton.ToolTipText = "Save all";
            this.saveallButton.Click += new System.EventHandler(this.tbtSaveAll_Click);
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton7.Image = global::WolvenKit.Properties.Resources.AddNodefromFile_354;
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(37, 24);
            this.toolStripButton7.Text = "toolStripButton7";
            this.toolStripButton7.ToolTipText = "Add file from bundle";
            this.toolStripButton7.Click += new System.EventHandler(this.addFileFromBundleToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(37, 6);
            // 
            // btPack
            // 
            this.btPack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btPack.Image = global::WolvenKit.Properties.Resources.package_16xLG;
            this.btPack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btPack.Name = "btPack";
            this.btPack.Size = new System.Drawing.Size(37, 24);
            this.btPack.Text = "btPack";
            this.btPack.ToolTipText = "Pack and install mod";
            this.btPack.Click += new System.EventHandler(this.btPack_Click);
            // 
            // rungameToolStrip
            // 
            this.rungameToolStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rungameToolStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.launchGameForDebuggingToolStripMenuItem,
            this.packProjectAndLaunchGameCustomToolStripMenuItem,
            this.launchWithCustomParametersToolStripMenuItem,
            this.packProjectToolStripMenuItem,
            this.packProjectAndRunGameToolStripMenuItem});
            this.rungameToolStrip.Image = global::WolvenKit.Properties.Resources.witcher3_101;
            this.rungameToolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rungameToolStrip.Name = "rungameToolStrip";
            this.rungameToolStrip.Size = new System.Drawing.Size(37, 24);
            this.rungameToolStrip.Text = "Launch game";
            // 
            // launchGameForDebuggingToolStripMenuItem
            // 
            this.launchGameForDebuggingToolStripMenuItem.Name = "launchGameForDebuggingToolStripMenuItem";
            this.launchGameForDebuggingToolStripMenuItem.Size = new System.Drawing.Size(546, 34);
            this.launchGameForDebuggingToolStripMenuItem.Text = "Launch game for debugging";
            this.launchGameForDebuggingToolStripMenuItem.Click += new System.EventHandler(this.LaunchGameForDebuggingToolStripMenuItem_Click);
            // 
            // packProjectAndLaunchGameCustomToolStripMenuItem
            // 
            this.packProjectAndLaunchGameCustomToolStripMenuItem.Name = "packProjectAndLaunchGameCustomToolStripMenuItem";
            this.packProjectAndLaunchGameCustomToolStripMenuItem.Size = new System.Drawing.Size(546, 34);
            this.packProjectAndLaunchGameCustomToolStripMenuItem.Text = "Pack project and launch game with custom parameters";
            this.packProjectAndLaunchGameCustomToolStripMenuItem.Click += new System.EventHandler(this.packProjectAndLaunchGameCustomToolStripMenuItem_Click);
            // 
            // launchWithCustomParametersToolStripMenuItem
            // 
            this.launchWithCustomParametersToolStripMenuItem.Name = "launchWithCustomParametersToolStripMenuItem";
            this.launchWithCustomParametersToolStripMenuItem.Size = new System.Drawing.Size(546, 34);
            this.launchWithCustomParametersToolStripMenuItem.Text = "Launch with custom parameters";
            this.launchWithCustomParametersToolStripMenuItem.Click += new System.EventHandler(this.launchWithCostumParametersToolStripMenuItem_Click);
            // 
            // packProjectToolStripMenuItem
            // 
            this.packProjectToolStripMenuItem.Name = "packProjectToolStripMenuItem";
            this.packProjectToolStripMenuItem.Size = new System.Drawing.Size(546, 34);
            this.packProjectToolStripMenuItem.Text = "Pack project";
            this.packProjectToolStripMenuItem.Click += new System.EventHandler(this.packProjectToolStripMenuItem_Click);
            // 
            // packProjectAndRunGameToolStripMenuItem
            // 
            this.packProjectAndRunGameToolStripMenuItem.Name = "packProjectAndRunGameToolStripMenuItem";
            this.packProjectAndRunGameToolStripMenuItem.Size = new System.Drawing.Size(546, 34);
            this.packProjectAndRunGameToolStripMenuItem.Text = "Pack project and run game";
            this.packProjectAndRunGameToolStripMenuItem.Click += new System.EventHandler(this.PackProjectAndRunGameToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iconToolStripMenuItem,
            this.fileToolStripMenuItem,
            this.modToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.buildDateToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.maximizeToolStripMenuItem,
            this.minimizeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1236, 35);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "topMS";
            this.menuStrip1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.menuStrip1_MouseDown_1);
            // 
            // iconToolStripMenuItem
            // 
            this.iconToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.iconToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.wolven_kit_icon;
            this.iconToolStripMenuItem.Name = "iconToolStripMenuItem";
            this.iconToolStripMenuItem.Size = new System.Drawing.Size(36, 29);
            this.iconToolStripMenuItem.Click += new System.EventHandler(this.iconToolStripMenuItem_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newModToolStripMenuItem,
            this.openModToolStripMenuItem,
            this.openFileToolStripMenuItem,
            this.recentFilesToolStripMenuItem,
            this.toolStripSeparator6,
            this.exportToolStripMenuItem,
            this.importToolStripMenuItem,
            this.newFileToolStripMenuItem,
            this.addFileFromBundleToolStripMenuItem,
            this.addFileFromOtherModToolStripMenuItem,
            this.addFileToolStripMenuItem,
            this.toolStripSeparator8,
            this.saveToolStripMenuItem,
            this.saveAllToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newModToolStripMenuItem
            // 
            this.newModToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.NewSolutionFolder_16x;
            this.newModToolStripMenuItem.Name = "newModToolStripMenuItem";
            this.newModToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.newModToolStripMenuItem.Text = "New mod";
            this.newModToolStripMenuItem.Click += new System.EventHandler(this.tbtNewMod_Click);
            // 
            // openModToolStripMenuItem
            // 
            this.openModToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.OpenFolder_16x;
            this.openModToolStripMenuItem.Name = "openModToolStripMenuItem";
            this.openModToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.openModToolStripMenuItem.Text = "Open mod";
            this.openModToolStripMenuItem.Click += new System.EventHandler(this.tbtOpenMod_Click);
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.OpenFile_16x;
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.openFileToolStripMenuItem.Text = "Open file";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // recentFilesToolStripMenuItem
            // 
            this.recentFilesToolStripMenuItem.Enabled = false;
            this.recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
            this.recentFilesToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.recentFilesToolStripMenuItem.Text = "Recent files";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(307, 6);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportCr2wToolStripMenuItem,
            this.extractCollisioncacheToolStripMenuItem});
            this.exportToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.wooden_box__arrow;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // exportCr2wToolStripMenuItem
            // 
            this.exportCr2wToolStripMenuItem.Name = "exportCr2wToolStripMenuItem";
            this.exportCr2wToolStripMenuItem.Size = new System.Drawing.Size(283, 34);
            this.exportCr2wToolStripMenuItem.Text = "Export Cr2w";
            this.exportCr2wToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // extractCollisioncacheToolStripMenuItem
            // 
            this.extractCollisioncacheToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.t_shirt_gray;
            this.extractCollisioncacheToolStripMenuItem.Name = "extractCollisioncacheToolStripMenuItem";
            this.extractCollisioncacheToolStripMenuItem.Size = new System.Drawing.Size(283, 34);
            this.extractCollisioncacheToolStripMenuItem.Text = "Extract collision.cache";
            this.extractCollisioncacheToolStripMenuItem.Click += new System.EventHandler(this.extractCollisioncacheToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fbxWithCollisionsToolStripMenuItem,
            this.nvidiaClothFileToolStripMenuItem});
            this.importToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.box__arrow;
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.importToolStripMenuItem.Text = "Import";
            // 
            // fbxWithCollisionsToolStripMenuItem
            // 
            this.fbxWithCollisionsToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.stickman;
            this.fbxWithCollisionsToolStripMenuItem.Name = "fbxWithCollisionsToolStripMenuItem";
            this.fbxWithCollisionsToolStripMenuItem.Size = new System.Drawing.Size(257, 34);
            this.fbxWithCollisionsToolStripMenuItem.Text = "Fbx with collisions";
            this.fbxWithCollisionsToolStripMenuItem.Click += new System.EventHandler(this.fbxWithCollisionsToolStripMenuItem_Click);
            // 
            // nvidiaClothFileToolStripMenuItem
            // 
            this.nvidiaClothFileToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.t_shirt_gray;
            this.nvidiaClothFileToolStripMenuItem.Name = "nvidiaClothFileToolStripMenuItem";
            this.nvidiaClothFileToolStripMenuItem.Size = new System.Drawing.Size(257, 34);
            this.nvidiaClothFileToolStripMenuItem.Text = "Nvidia cloth file";
            this.nvidiaClothFileToolStripMenuItem.Click += new System.EventHandler(this.nvidiaClothFileToolStripMenuItem_Click);
            // 
            // newFileToolStripMenuItem
            // 
            this.newFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modToolStripMenuItem1,
            this.dLCToolStripMenuItem});
            this.newFileToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.FileGroup_10135_16x;
            this.newFileToolStripMenuItem.Name = "newFileToolStripMenuItem";
            this.newFileToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.newFileToolStripMenuItem.Text = "New file";
            // 
            // modToolStripMenuItem1
            // 
            this.modToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ModscriptToolStripMenuItem,
            this.modwwise,
            this.ModchunkToolStripMenuItem});
            this.modToolStripMenuItem1.Name = "modToolStripMenuItem1";
            this.modToolStripMenuItem1.Size = new System.Drawing.Size(152, 34);
            this.modToolStripMenuItem1.Text = "Mod";
            // 
            // ModscriptToolStripMenuItem
            // 
            this.ModscriptToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.FileGroup_10135_16x;
            this.ModscriptToolStripMenuItem.Name = "ModscriptToolStripMenuItem";
            this.ModscriptToolStripMenuItem.Size = new System.Drawing.Size(269, 34);
            this.ModscriptToolStripMenuItem.Text = "Script";
            this.ModscriptToolStripMenuItem.Click += new System.EventHandler(this.ModscriptToolStripMenuItem_Click);
            // 
            // modwwise
            // 
            this.modwwise.Image = global::WolvenKit.Properties.Resources.bug;
            this.modwwise.Name = "modwwise";
            this.modwwise.Size = new System.Drawing.Size(269, 34);
            this.modwwise.Text = "Wwise sound(bank)";
            this.modwwise.Click += new System.EventHandler(this.ModWwiseNew_Click);
            // 
            // ModchunkToolStripMenuItem
            // 
            this.ModchunkToolStripMenuItem.Enabled = false;
            this.ModchunkToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.AddNodefromFile_354;
            this.ModchunkToolStripMenuItem.Name = "ModchunkToolStripMenuItem";
            this.ModchunkToolStripMenuItem.Size = new System.Drawing.Size(269, 34);
            this.ModchunkToolStripMenuItem.Text = "Chunk file";
            // 
            // dLCToolStripMenuItem
            // 
            this.dLCToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DLCScriptToolStripMenuItem,
            this.dlcwwise,
            this.DLCChunkToolStripMenuItem});
            this.dLCToolStripMenuItem.Name = "dLCToolStripMenuItem";
            this.dLCToolStripMenuItem.Size = new System.Drawing.Size(152, 34);
            this.dLCToolStripMenuItem.Text = "DLC";
            // 
            // DLCScriptToolStripMenuItem
            // 
            this.DLCScriptToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.FileGroup_10135_16x;
            this.DLCScriptToolStripMenuItem.Name = "DLCScriptToolStripMenuItem";
            this.DLCScriptToolStripMenuItem.Size = new System.Drawing.Size(269, 34);
            this.DLCScriptToolStripMenuItem.Text = "Script";
            this.DLCScriptToolStripMenuItem.Click += new System.EventHandler(this.DLCScriptToolStripMenuItem_Click);
            // 
            // dlcwwise
            // 
            this.dlcwwise.Image = global::WolvenKit.Properties.Resources.bug;
            this.dlcwwise.Name = "dlcwwise";
            this.dlcwwise.Size = new System.Drawing.Size(269, 34);
            this.dlcwwise.Text = "Wwise sound(bank)";
            this.dlcwwise.Click += new System.EventHandler(this.DLCWwise_Click);
            // 
            // DLCChunkToolStripMenuItem
            // 
            this.DLCChunkToolStripMenuItem.Enabled = false;
            this.DLCChunkToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.AddNodefromFile_354;
            this.DLCChunkToolStripMenuItem.Name = "DLCChunkToolStripMenuItem";
            this.DLCChunkToolStripMenuItem.Size = new System.Drawing.Size(269, 34);
            this.DLCChunkToolStripMenuItem.Text = "Chunk file";
            // 
            // addFileFromBundleToolStripMenuItem
            // 
            this.addFileFromBundleToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.AddNodefromFile_354;
            this.addFileFromBundleToolStripMenuItem.Name = "addFileFromBundleToolStripMenuItem";
            this.addFileFromBundleToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.addFileFromBundleToolStripMenuItem.Text = "Asset browser";
            this.addFileFromBundleToolStripMenuItem.Click += new System.EventHandler(this.addFileFromBundleToolStripMenuItem_Click);
            // 
            // addFileFromOtherModToolStripMenuItem
            // 
            this.addFileFromOtherModToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.AddNodefromFile_354;
            this.addFileFromOtherModToolStripMenuItem.Name = "addFileFromOtherModToolStripMenuItem";
            this.addFileFromOtherModToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.addFileFromOtherModToolStripMenuItem.Text = "Add file from other mod";
            this.addFileFromOtherModToolStripMenuItem.Click += new System.EventHandler(this.AddFileFromOtherModToolStripMenuItem_Click_1);
            // 
            // addFileToolStripMenuItem
            // 
            this.addFileToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.AddNodefromFile_354;
            this.addFileToolStripMenuItem.Name = "addFileToolStripMenuItem";
            this.addFileToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.addFileToolStripMenuItem.Text = "Add file";
            this.addFileToolStripMenuItem.Click += new System.EventHandler(this.addFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(307, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.SaveStatusBar1_16x_c;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // saveAllToolStripMenuItem
            // 
            this.saveAllToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.SaveAll_16x;
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.saveAllToolStripMenuItem.Text = "Save all";
            this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.SaveAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(307, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.exclamation_red;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(310, 34);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // modToolStripMenuItem
            // 
            this.modToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createPackedInstallerToolStripMenuItem,
            this.reloadProjectToolStripMenuItem,
            this.toolStripSeparator4,
            this.settingsToolStripMenuItem});
            this.modToolStripMenuItem.Name = "modToolStripMenuItem";
            this.modToolStripMenuItem.Size = new System.Drawing.Size(82, 29);
            this.modToolStripMenuItem.Text = "Project";
            // 
            // createPackedInstallerToolStripMenuItem
            // 
            this.createPackedInstallerToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.box__arrow;
            this.createPackedInstallerToolStripMenuItem.Name = "createPackedInstallerToolStripMenuItem";
            this.createPackedInstallerToolStripMenuItem.Size = new System.Drawing.Size(290, 34);
            this.createPackedInstallerToolStripMenuItem.Text = "Create Packed Installer";
            this.createPackedInstallerToolStripMenuItem.Click += new System.EventHandler(this.createPackedInstallerToolStripMenuItem_Click);
            // 
            // reloadProjectToolStripMenuItem
            // 
            this.reloadProjectToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.Refresh_16x;
            this.reloadProjectToolStripMenuItem.Name = "reloadProjectToolStripMenuItem";
            this.reloadProjectToolStripMenuItem.Size = new System.Drawing.Size(290, 34);
            this.reloadProjectToolStripMenuItem.Text = "Reload project";
            this.reloadProjectToolStripMenuItem.Click += new System.EventHandler(this.ReloadProjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(287, 6);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.Settings_Inverse_16x;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(290, 34);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.modSettingsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.packageInstallerToolStripMenuItem,
            this.saveExplorerToolStripMenuItem,
            this.stringsEncoderGUIToolStripMenuItem,
            this.gameDebuggerToolStripMenuItem,
            this.menuCreatorToolStripMenuItem,
            this.dumpFileToolStripMenuItem,
            this.renderW2meshToolStripMenuItem,
            this.verifyFileToolStripMenuItem,
            this.toolStripSeparator5,
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(69, 29);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // packageInstallerToolStripMenuItem
            // 
            this.packageInstallerToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.box;
            this.packageInstallerToolStripMenuItem.Name = "packageInstallerToolStripMenuItem";
            this.packageInstallerToolStripMenuItem.Size = new System.Drawing.Size(271, 34);
            this.packageInstallerToolStripMenuItem.Text = "Package Installer";
            this.packageInstallerToolStripMenuItem.Click += new System.EventHandler(this.packageInstallerToolStripMenuItem_Click);
            // 
            // saveExplorerToolStripMenuItem
            // 
            this.saveExplorerToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.properties_16xLG;
            this.saveExplorerToolStripMenuItem.Name = "saveExplorerToolStripMenuItem";
            this.saveExplorerToolStripMenuItem.Size = new System.Drawing.Size(271, 34);
            this.saveExplorerToolStripMenuItem.Text = "Save explorer";
            this.saveExplorerToolStripMenuItem.Click += new System.EventHandler(this.saveExplorerToolStripMenuItem_Click);
            // 
            // stringsEncoderGUIToolStripMenuItem
            // 
            this.stringsEncoderGUIToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.edit_letter_spacing;
            this.stringsEncoderGUIToolStripMenuItem.Name = "stringsEncoderGUIToolStripMenuItem";
            this.stringsEncoderGUIToolStripMenuItem.Size = new System.Drawing.Size(271, 34);
            this.stringsEncoderGUIToolStripMenuItem.Text = "Strings Encoder GUI";
            this.stringsEncoderGUIToolStripMenuItem.Click += new System.EventHandler(this.StringsGUIToolStripMenuItem_Click);
            // 
            // gameDebuggerToolStripMenuItem
            // 
            this.gameDebuggerToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.bug;
            this.gameDebuggerToolStripMenuItem.Name = "gameDebuggerToolStripMenuItem";
            this.gameDebuggerToolStripMenuItem.Size = new System.Drawing.Size(271, 34);
            this.gameDebuggerToolStripMenuItem.Text = "Game debugger";
            this.gameDebuggerToolStripMenuItem.Click += new System.EventHandler(this.GameDebuggerToolStripMenuItem_Click);
            // 
            // menuCreatorToolStripMenuItem
            // 
            this.menuCreatorToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.ui_menu_blue;
            this.menuCreatorToolStripMenuItem.Name = "menuCreatorToolStripMenuItem";
            this.menuCreatorToolStripMenuItem.Size = new System.Drawing.Size(271, 34);
            this.menuCreatorToolStripMenuItem.Text = "Menu creator";
            this.menuCreatorToolStripMenuItem.Click += new System.EventHandler(this.menuCreatorToolStripMenuItem_Click);
            // 
            // dumpFileToolStripMenuItem
            // 
            this.dumpFileToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.bug;
            this.dumpFileToolStripMenuItem.Name = "dumpFileToolStripMenuItem";
            this.dumpFileToolStripMenuItem.Size = new System.Drawing.Size(271, 34);
            this.dumpFileToolStripMenuItem.Text = "Dump game assets";
            this.dumpFileToolStripMenuItem.Click += new System.EventHandler(this.dumpFileToolStripMenuItem_Click);
            // 
            // renderW2meshToolStripMenuItem
            // 
            this.renderW2meshToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.ui_check_box_uncheck;
            this.renderW2meshToolStripMenuItem.Name = "renderW2meshToolStripMenuItem";
            this.renderW2meshToolStripMenuItem.Size = new System.Drawing.Size(271, 34);
            this.renderW2meshToolStripMenuItem.Tag = "false";
            this.renderW2meshToolStripMenuItem.Text = "Render w2mesh";
            this.renderW2meshToolStripMenuItem.Click += new System.EventHandler(this.renderW2meshToolStripMenuItem_Click);
            // 
            // verifyFileToolStripMenuItem
            // 
            this.verifyFileToolStripMenuItem.Name = "verifyFileToolStripMenuItem";
            this.verifyFileToolStripMenuItem.Size = new System.Drawing.Size(271, 34);
            this.verifyFileToolStripMenuItem.Text = "Verify File";
            this.verifyFileToolStripMenuItem.Click += new System.EventHandler(this.verifyFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(268, 6);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.Settings_Inverse_16x;
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(271, 34);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modExplorerToolStripMenuItem,
            this.outputToolStripMenuItem,
            this.toolStripSeparator9});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // modExplorerToolStripMenuItem
            // 
            this.modExplorerToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.RemoteServer_16x;
            this.modExplorerToolStripMenuItem.Name = "modExplorerToolStripMenuItem";
            this.modExplorerToolStripMenuItem.Size = new System.Drawing.Size(221, 34);
            this.modExplorerToolStripMenuItem.Text = "Mod explorer";
            this.modExplorerToolStripMenuItem.Click += new System.EventHandler(this.modExplorerToolStripMenuItem_Click);
            // 
            // outputToolStripMenuItem
            // 
            this.outputToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.Output_16x;
            this.outputToolStripMenuItem.Name = "outputToolStripMenuItem";
            this.outputToolStripMenuItem.Size = new System.Drawing.Size(221, 34);
            this.outputToolStripMenuItem.Text = "Output";
            this.outputToolStripMenuItem.Click += new System.EventHandler(this.OutputToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(218, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.joinOurDiscordToolStripMenuItem,
            this.tutorialsToolStripMenuItem,
            this.witcherIIIModdingToolLicenseToolStripMenuItem,
            this.reportABugToolStripMenuItem,
            this.recordStepsToReproduceBugToolStripMenuItem,
            this.toolStripSeparator7,
            this.aboutRedkit2ToolStripMenuItem,
            this.donateToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
            this.aboutToolStripMenuItem.Text = "Help";
            // 
            // joinOurDiscordToolStripMenuItem
            // 
            this.joinOurDiscordToolStripMenuItem.Image = global::WolvenKit.Properties.Resources._2c21aeda16de354ba5334551a883b481;
            this.joinOurDiscordToolStripMenuItem.Name = "joinOurDiscordToolStripMenuItem";
            this.joinOurDiscordToolStripMenuItem.Size = new System.Drawing.Size(372, 34);
            this.joinOurDiscordToolStripMenuItem.Text = "Join our discord";
            this.joinOurDiscordToolStripMenuItem.Click += new System.EventHandler(this.joinOurDiscordToolStripMenuItem_Click_1);
            // 
            // tutorialsToolStripMenuItem
            // 
            this.tutorialsToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.info_icon_23818;
            this.tutorialsToolStripMenuItem.Name = "tutorialsToolStripMenuItem";
            this.tutorialsToolStripMenuItem.Size = new System.Drawing.Size(372, 34);
            this.tutorialsToolStripMenuItem.Text = "Witcherscript documentation";
            this.tutorialsToolStripMenuItem.Click += new System.EventHandler(this.WitcherScriptToolStripMenuItem_Click);
            // 
            // witcherIIIModdingToolLicenseToolStripMenuItem
            // 
            this.witcherIIIModdingToolLicenseToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.witcher3_101;
            this.witcherIIIModdingToolLicenseToolStripMenuItem.Name = "witcherIIIModdingToolLicenseToolStripMenuItem";
            this.witcherIIIModdingToolLicenseToolStripMenuItem.Size = new System.Drawing.Size(372, 34);
            this.witcherIIIModdingToolLicenseToolStripMenuItem.Text = "Witcher III Modding Tool License";
            this.witcherIIIModdingToolLicenseToolStripMenuItem.Click += new System.EventHandler(this.witcherIIIModdingToolLicenseToolStripMenuItem_Click);
            // 
            // reportABugToolStripMenuItem
            // 
            this.reportABugToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.mail;
            this.reportABugToolStripMenuItem.Name = "reportABugToolStripMenuItem";
            this.reportABugToolStripMenuItem.Size = new System.Drawing.Size(372, 34);
            this.reportABugToolStripMenuItem.Text = "Report a bug";
            this.reportABugToolStripMenuItem.Click += new System.EventHandler(this.ReportABugToolStripMenuItem_Click);
            // 
            // recordStepsToReproduceBugToolStripMenuItem
            // 
            this.recordStepsToReproduceBugToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.player_record;
            this.recordStepsToReproduceBugToolStripMenuItem.Name = "recordStepsToReproduceBugToolStripMenuItem";
            this.recordStepsToReproduceBugToolStripMenuItem.Size = new System.Drawing.Size(372, 34);
            this.recordStepsToReproduceBugToolStripMenuItem.Text = "Record steps to reproduce bug";
            this.recordStepsToReproduceBugToolStripMenuItem.Click += new System.EventHandler(this.RecordStepsToReproduceBugToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(369, 6);
            // 
            // aboutRedkit2ToolStripMenuItem
            // 
            this.aboutRedkit2ToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.info_icon_23818;
            this.aboutRedkit2ToolStripMenuItem.Name = "aboutRedkit2ToolStripMenuItem";
            this.aboutRedkit2ToolStripMenuItem.Size = new System.Drawing.Size(372, 34);
            this.aboutRedkit2ToolStripMenuItem.Text = "About Wolven kit";
            this.aboutRedkit2ToolStripMenuItem.Click += new System.EventHandler(this.creditsToolStripMenuItem_Click);
            // 
            // donateToolStripMenuItem
            // 
            this.donateToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("donateToolStripMenuItem.Image")));
            this.donateToolStripMenuItem.Name = "donateToolStripMenuItem";
            this.donateToolStripMenuItem.Size = new System.Drawing.Size(372, 34);
            this.donateToolStripMenuItem.Text = "Donate";
            this.donateToolStripMenuItem.Click += new System.EventHandler(this.donateToolStripMenuItem_Click);
            // 
            // buildDateToolStripMenuItem
            // 
            this.buildDateToolStripMenuItem.Enabled = false;
            this.buildDateToolStripMenuItem.Name = "buildDateToolStripMenuItem";
            this.buildDateToolStripMenuItem.ShowShortcutKeys = false;
            this.buildDateToolStripMenuItem.Size = new System.Drawing.Size(107, 29);
            this.buildDateToolStripMenuItem.Text = "Build date";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.closeToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.closeToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.close;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(36, 29);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.ToolTipText = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // maximizeToolStripMenuItem
            // 
            this.maximizeToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.maximize1;
            this.maximizeToolStripMenuItem.Name = "maximizeToolStripMenuItem";
            this.maximizeToolStripMenuItem.Size = new System.Drawing.Size(36, 29);
            this.maximizeToolStripMenuItem.Text = "Maximize";
            this.maximizeToolStripMenuItem.ToolTipText = "Restore";
            this.maximizeToolStripMenuItem.Click += new System.EventHandler(this.RestoreToolStripMenuItem_Click);
            // 
            // minimizeToolStripMenuItem
            // 
            this.minimizeToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.minimizeToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.minimizeToolStripMenuItem.Image = global::WolvenKit.Properties.Resources.minimize;
            this.minimizeToolStripMenuItem.Name = "minimizeToolStripMenuItem";
            this.minimizeToolStripMenuItem.Size = new System.Drawing.Size(36, 29);
            this.minimizeToolStripMenuItem.Text = "Minimize";
            this.minimizeToolStripMenuItem.ToolTipText = "Minimize";
            this.minimizeToolStripMenuItem.Click += new System.EventHandler(this.MinimizeToolStripMenuItem_Click);
            // 
            // dockPanel
            // 
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.dockPanel.Location = new System.Drawing.Point(0, 0);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.ShowDocumentIcon = true;
            this.dockPanel.Size = new System.Drawing.Size(1195, 553);
            this.dockPanel.TabIndex = 9;
            this.dockPanel.ActiveDocumentChanged += new System.EventHandler(this.dockPanel_ActiveDocumentChanged);
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.SystemColors.Desktop;
            this.toolStrip2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLBL,
            this.toolStripProgressBar1});
            this.toolStrip2.Location = new System.Drawing.Point(0, 613);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStrip2.Size = new System.Drawing.Size(1236, 30);
            this.toolStrip2.TabIndex = 12;
            this.toolStrip2.Text = "bottomTS";
            // 
            // statusLBL
            // 
            this.statusLBL.Name = "statusLBL";
            this.statusLBL.Size = new System.Drawing.Size(60, 25);
            this.statusLBL.Text = "Ready";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(150, 31);
            this.toolStripProgressBar1.Visible = false;
            // 
            // richpresenceworker
            // 
            this.richpresenceworker.WorkerSupportsCancellation = true;
            this.richpresenceworker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.richpresenceworker_DoWork);
            this.richpresenceworker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.richpresenceworker_ProgressChanged);
            this.richpresenceworker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.richpresenceworker_RunWorkerCompleted);
            // 
            // visualStudioToolStripExtender1
            // 
            this.visualStudioToolStripExtender1.DefaultRenderer = null;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.dockPanel);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1195, 553);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            this.toolStripContainer1.LeftToolStripPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 35);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1236, 578);
            this.toolStripContainer1.TabIndex = 14;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 643);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(862, 562);
            this.Name = "frmMain";
            this.Text = "Wolven kit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.MdiChildActivate += new System.EventHandler(this.frmMain_MdiChildActivate);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.LeftToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.LeftToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStrip toolStrip1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newModToolStripMenuItem;
        private ToolStripMenuItem openModToolStripMenuItem;
        private ToolStripMenuItem modToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem saveExplorerToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem modExplorerToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem aboutRedkit2ToolStripMenuItem;
        private ToolStripMenuItem joinOurDiscordToolStripMenuItem;
        private ToolStripButton newmodButton;
        private ToolStripButton openmodButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton openfileButton;
        private ToolStripButton saveButton;
        private ToolStripButton saveallButton;
        private ToolStripButton toolStripButton7;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton btPack;
        private DockPanel dockPanel;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem outputToolStripMenuItem;
        private ToolStripMenuItem tutorialsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem reloadProjectToolStripMenuItem;
        private ToolStripMenuItem createPackedInstallerToolStripMenuItem;
        private ToolStripMenuItem witcherIIIModdingToolLicenseToolStripMenuItem;
        private ToolStripMenuItem buildDateToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem newFileToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripDropDownButton rungameToolStrip;
        private ToolStripMenuItem launchWithCustomParametersToolStripMenuItem;
        private ToolStripMenuItem launchGameForDebuggingToolStripMenuItem;
        private ToolStripMenuItem addFileFromBundleToolStripMenuItem;
        private ToolStripMenuItem addFileFromOtherModToolStripMenuItem;
        private ToolStripMenuItem openFileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
		private ToolStripMenuItem stringsEncoderGUIToolStripMenuItem;
        private ToolStripMenuItem menuCreatorToolStripMenuItem;
        private ToolStripMenuItem recordStepsToReproduceBugToolStripMenuItem;
        private ToolStripMenuItem reportABugToolStripMenuItem;
        private ToolStripMenuItem gameDebuggerToolStripMenuItem;
        private ToolStripMenuItem packageInstallerToolStripMenuItem;
        private ToolStrip toolStrip2;
        private ToolStripLabel statusLBL;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripMenuItem packProjectToolStripMenuItem;
        private ToolStripMenuItem modToolStripMenuItem1;
        private ToolStripMenuItem ModscriptToolStripMenuItem;
        private ToolStripMenuItem ModchunkToolStripMenuItem;
        private ToolStripMenuItem dLCToolStripMenuItem;
        private ToolStripMenuItem DLCScriptToolStripMenuItem;
        private ToolStripMenuItem DLCChunkToolStripMenuItem;
        private ToolStripMenuItem recentFilesToolStripMenuItem;
        private ToolStripMenuItem donateToolStripMenuItem;
        private ToolStripMenuItem importToolStripMenuItem;
        private ToolStripMenuItem fbxWithCollisionsToolStripMenuItem;
        private ToolStripMenuItem nvidiaClothFileToolStripMenuItem;
        private ToolStripMenuItem renderW2meshToolStripMenuItem;
        private ToolStripMenuItem dumpFileToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripMenuItem modwwise;
        private ToolStripMenuItem dlcwwise;
        private BackgroundWorker richpresenceworker;
        private ToolStripMenuItem packProjectAndLaunchGameCustomToolStripMenuItem;
        private ToolStripMenuItem packProjectAndRunGameToolStripMenuItem;
        private ToolStripMenuItem exportCr2wToolStripMenuItem;
        private ToolStripMenuItem extractCollisioncacheToolStripMenuItem;
        private VisualStudioToolStripExtender visualStudioToolStripExtender1;
        private ToolStripMenuItem verifyFileToolStripMenuItem;
        private ToolStripMenuItem addFileToolStripMenuItem;
        private ToolStripMenuItem iconToolStripMenuItem;
        private ToolStripMenuItem minimizeToolStripMenuItem;
        private ToolStripMenuItem maximizeToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripContainer toolStripContainer1;
        private ToolStripSeparator toolStripSeparator9;
    }
}