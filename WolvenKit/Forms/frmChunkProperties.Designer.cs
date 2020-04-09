﻿using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace WolvenKit
{
    partial class frmChunkProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChunkProperties));
            this.treeView = new BrightIdeasSoftware.TreeListView();
            this.colName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colValue = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colInfo = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandAllChildrenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllChildrenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addVariableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeVariableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolSplitPtr = new System.Windows.Forms.ToolStripSeparator();
            this.ptrPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.treeView)).BeginInit();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.AllColumns.Add(this.colName);
            this.treeView.AllColumns.Add(this.colValue);
            this.treeView.AllColumns.Add(this.colType);
            this.treeView.AllColumns.Add(this.colInfo);
            this.treeView.AlternateRowBackColor = System.Drawing.Color.LightCyan;
            this.treeView.CellEditUseWholeCell = false;
            this.treeView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colValue,
            this.colType,
            this.colInfo});
            this.treeView.ContextMenuStrip = this.contextMenu;
            this.treeView.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.FullRowSelect = true;
            this.treeView.HideSelection = false;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.ShowGroups = false;
            this.treeView.Size = new System.Drawing.Size(1412, 493);
            this.treeView.TabIndex = 1;
            this.treeView.UseAlternatingBackColors = true;
            this.treeView.UseCompatibleStateImageBehavior = false;
            this.treeView.View = System.Windows.Forms.View.Details;
            this.treeView.VirtualMode = true;
            this.treeView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.treeView_CellEditStarting);
            this.treeView.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.treeView_CellClick);
            this.treeView.ItemsChanged += new System.EventHandler<BrightIdeasSoftware.ItemsChangedEventArgs>(this.treeView_ItemsChanged);
            this.treeView.SelectedIndexChanged += new System.EventHandler(this.treeView_SelectedIndexChanged);
            // 
            // colName
            // 
            this.colName.AspectName = "Name";
            this.colName.Text = "Name";
            this.colName.Width = 300;
            // 
            // colValue
            // 
            this.colValue.AspectName = "Value";
            this.colValue.Text = "Value";
            this.colValue.Width = 650;
            // 
            // colType
            // 
            this.colType.AspectName = "Type";
            this.colType.Text = "Type";
            this.colType.Width = 150;
            // 
            // colInfo
            // 
            this.colInfo.AspectName = "Info";
            this.colInfo.Text = "Info";
            this.colInfo.Width = 650;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllToolStripMenuItem,
            this.expandAllChildrenToolStripMenuItem,
            this.collapseAllToolStripMenuItem,
            this.collapseAllChildrenToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addVariableToolStripMenuItem,
            this.removeVariableToolStripMenuItem,
            this.toolStripMenuItem2,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.copyTextToolStripMenuItem,
            this.copyInfoToolStripMenuItem,
            this.toolSplitPtr,
            this.ptrPropertiesToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(185, 264);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // expandAllChildrenToolStripMenuItem
            // 
            this.expandAllChildrenToolStripMenuItem.Name = "expandAllChildrenToolStripMenuItem";
            this.expandAllChildrenToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.expandAllChildrenToolStripMenuItem.Text = "Expand All Children";
            this.expandAllChildrenToolStripMenuItem.Click += new System.EventHandler(this.expandAllChildrenToolStripMenuItem_Click);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.collapseAllToolStripMenuItem.Text = "Collapse All";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
            // 
            // collapseAllChildrenToolStripMenuItem
            // 
            this.collapseAllChildrenToolStripMenuItem.Name = "collapseAllChildrenToolStripMenuItem";
            this.collapseAllChildrenToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.collapseAllChildrenToolStripMenuItem.Text = "Collapse All Children";
            this.collapseAllChildrenToolStripMenuItem.Click += new System.EventHandler(this.collapseAllChildrenToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(181, 6);
            // 
            // addVariableToolStripMenuItem
            // 
            this.addVariableToolStripMenuItem.Name = "addVariableToolStripMenuItem";
            this.addVariableToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.addVariableToolStripMenuItem.Text = "Add Variable";
            this.addVariableToolStripMenuItem.Click += new System.EventHandler(this.addVariableToolStripMenuItem_Click);
            // 
            // removeVariableToolStripMenuItem
            // 
            this.removeVariableToolStripMenuItem.Name = "removeVariableToolStripMenuItem";
            this.removeVariableToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.removeVariableToolStripMenuItem.Text = "Remove Variable";
            this.removeVariableToolStripMenuItem.Click += new System.EventHandler(this.removeVariableToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(181, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.copyToolStripMenuItem.Text = "Copy Variable";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.pasteToolStripMenuItem.Text = "Paste Variable";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // copyTextToolStripMenuItem
            // 
            this.copyTextToolStripMenuItem.Name = "copyTextToolStripMenuItem";
            this.copyTextToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.copyTextToolStripMenuItem.Text = "Copy Text";
            this.copyTextToolStripMenuItem.Click += new System.EventHandler(this.copyTextToolStripMenuItem_Click);
            // 
            // copyInfoToolStripMenuItem
            // 
            this.copyInfoToolStripMenuItem.Name = "copyInfoToolStripMenuItem";
            this.copyInfoToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.copyInfoToolStripMenuItem.Text = "Copy Info";
            this.copyInfoToolStripMenuItem.Click += new System.EventHandler(this.copyInfoToolStripMenuItem_Click);
            // 
            // toolSplitPtr
            // 
            this.toolSplitPtr.Name = "toolSplitPtr";
            this.toolSplitPtr.Size = new System.Drawing.Size(181, 6);
            // 
            // ptrPropertiesToolStripMenuItem
            // 
            this.ptrPropertiesToolStripMenuItem.Name = "ptrPropertiesToolStripMenuItem";
            this.ptrPropertiesToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.ptrPropertiesToolStripMenuItem.Text = "Ptr Properties";
            this.ptrPropertiesToolStripMenuItem.Click += new System.EventHandler(this.ptrPropertiesToolStripMenuItem_Click);
            // 
            // frmChunkProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1412, 493);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.treeView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmChunkProperties";
            this.Text = "Properties";
            this.Shown += new System.EventHandler(this.frmChunkProperties_Shown);
            this.Resize += new System.EventHandler(this.frmChunkProperties_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.treeView)).EndInit();
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TreeListView treeView;
        private OLVColumn colName;
        private OLVColumn colValue;
        private OLVColumn colType;
        private OLVColumn colInfo;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem addVariableToolStripMenuItem;
        private ToolStripMenuItem removeVariableToolStripMenuItem;
        private ToolStripMenuItem expandAllToolStripMenuItem;
        private ToolStripMenuItem expandAllChildrenToolStripMenuItem;
        private ToolStripMenuItem collapseAllToolStripMenuItem;
        private ToolStripMenuItem collapseAllChildrenToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripSeparator toolSplitPtr;
        private ToolStripMenuItem ptrPropertiesToolStripMenuItem;
        private ToolStripMenuItem copyTextToolStripMenuItem;
        private ToolStripMenuItem copyInfoToolStripMenuItem;
    }
}