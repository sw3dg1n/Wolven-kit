﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using WeifenLuo.WinFormsUI.Docking;
using WolvenKit.CR2W;
using WolvenKit.CR2W.Editors;
using WolvenKit.CR2W.Types;

namespace WolvenKit
{
    public partial class frmChunkProperties : DockContent
    {
        private CR2WChunk chunk;

        public frmChunkProperties()
        {
            InitializeComponent();
            treeView.CanExpandGetter = x => ((VariableListNode)x).ChildCount > 0;
            treeView.ChildrenGetter = x => ((VariableListNode)x).Children;
        }

        public CR2WChunk Chunk
        {
            get { return chunk; }
            set
            {
                chunk = value;
                CreatePropertyLayout(chunk);
            }
        }

        public IEditableVariable EditObject { get; set; }
        public object Source { get; set; }

        public void CreatePropertyLayout(IEditableVariable v)
        {
            if (EditObject != v)
            {
                EditObject = v;


                if (v == null)
                {
                    treeView.Roots = null;
                    return;
                }

                var root = AddListViewItems(v);

                treeView.Roots = root.Children;
                treeView.RefreshObjects(root.Children);

                for (var depth = 0; ExpandOneLevel(depth, root.Children); depth++)
                {
                }
            }
        }

        private bool ExpandOneLevel(int depth, List<VariableListNode> children, int currentLevel = 0)
        {
            var expandedSomething = false;

            foreach (var c in children)
            {
                if (currentLevel == depth)
                {
                    treeView.Expand(c);

                    if ((NativeMethods.GetWindowLong(treeView.Handle, NativeMethods.GWL_STYLE) & NativeMethods.WS_VSCROLL) == NativeMethods.WS_VSCROLL)
                    {
                        treeView.Collapse(c);
                        return false;
                    }

                    expandedSomething = true;
                }
                else
                {
                    if (ExpandOneLevel(depth, c.Children, currentLevel + 1))
                        expandedSomething = true;
                }
            }

            return expandedSomething;
        }

        private VariableListNode AddListViewItems(IEditableVariable v, VariableListNode parent = null,
            int arrayindex = 0)
        {
            var node = new VariableListNode()
            {
                Variable = v,
                Children = new List<VariableListNode>(),
                Parent = parent
            };
            var vars = v.GetEditableVariables();
            if (vars != null)
            {
                for (var i = 0; i < vars.Count; i++)
                {
                    node.Children.Add(AddListViewItems(vars[i], node, i));
                }
            }

            return node;
        }

        private void treeView_CellEditStarting(object sender, CellEditEventArgs e)
        {
            var variable = (e.RowObject as VariableListNode).Variable;
            if (e.Column.AspectName == "Value")
            {
                e.Control = ((VariableListNode)e.RowObject).Variable.GetEditor();
                if (e.Control != null)
                {
                    e.Control.Location = new Point(e.CellBounds.Location.X, e.CellBounds.Location.Y - 1);
                    e.Control.Width = e.CellBounds.Width;
                }
                e.Cancel = e.Control == null;
            }
            else if (e.Column.AspectName == "Name")
            {
                //Normal textbox is good for this.
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void frmChunkProperties_Resize(object sender, EventArgs e)
        {
        }

        private void frmChunkProperties_Shown(object sender, EventArgs e)
        {
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            var sNodes = treeView.SelectedObjects.Cast<VariableListNode>().Where(item => item?.Variable != null).ToList();
            if (sNodes.ToArray().Length <= 0)
            {
                e.Cancel = true;
                return;
            }

            addVariableToolStripMenuItem.Enabled = sNodes.All(x => x.Variable.CanAddVariable(null));
            removeVariableToolStripMenuItem.Enabled = sNodes.All(x => x.Parent != null && x.Parent.Variable.CanRemoveVariable(x.Variable));
            pasteToolStripMenuItem.Enabled = CopyController.VariableTargets != null && sNodes.All(x => x.Variable != null && CopyController.VariableTargets.Any(z => x.Variable.CanAddVariable(z)));
            ptrPropertiesToolStripMenuItem.Visible = sNodes.All(x => x.Variable is CPtr) && sNodes.Count == 1;
        }

        public void copyVariable()
        {
            var tocopynodes = (from VariableListNode item in treeView.SelectedObjects where item?.Variable != null select item.Variable).ToList();
            if (tocopynodes.Count > 0)
            {
                CopyController.VariableTargets = tocopynodes;
            }
        }

        public void pasteVariable()
        {
            var node = (VariableListNode)treeView.SelectedObject;
            if (CopyController.VariableTargets == null || node?.Variable == null || !node.Variable.CanAddVariable(null))
            {
                return;
            }

            if (CopyController.VariableTargets.All(x => x is CVariable))
            {
                foreach (var newvar in from v in CopyController.VariableTargets.Select(x => (CVariable)x)
                                       let context = new CR2WCopyAction
                                       {
                                           SourceFile = v.cr2w,
                                           DestinationFile = node.Variable.CR2WOwner,
                                           MaxIterationDepth = 0
                                       }
                                       select v.Copy(context))
                {
                    node.Variable.AddVariable(newvar);

                    var subnode = AddListViewItems(newvar, node);
                    node.Children.Add(subnode);

                    treeView.RefreshObject(node);
                    treeView.RefreshObject(subnode);
                }
            }
        }

        private void addVariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = (VariableListNode)treeView.SelectedObject;
            if (node?.Variable == null || !node.Variable.CanAddVariable(null))
            {
                return;
            }

            CVariable newvar = null;

            if (node.Variable is CArray)
            {
                var nodearray = (CArray)node.Variable;
                newvar = CR2WTypeManager.Get().GetByName(nodearray.elementtype, "", Chunk.cr2w, false);
                if (newvar == null)
                    return;
            }
            else
            {
                var frm = new frmAddVariable();
                if (frm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                newvar = CR2WTypeManager.Get().GetByName(frm.VariableType, frm.VariableName, Chunk.cr2w, false);
                if (newvar == null)
                    return;

                newvar.Name = frm.VariableName;
                newvar.Type = frm.VariableType;
            }

            if (newvar is CHandle)
            {
                var result = MessageBox.Show("Add as chunk handle? (Yes for chunk handle, No for normal handle)",
                    "Adding handle.", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                    return;

                ((CHandle)newvar).ChunkHandle = result == DialogResult.Yes;
            }

            node.Variable.AddVariable(newvar);

            var subnode = AddListViewItems(newvar, node);
            node.Children.Add(subnode);

            treeView.RefreshObject(node);
            treeView.RefreshObject(subnode);
        }

        private void removeVariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (VariableListNode node in treeView.SelectedObjects)
            {
                if (node?.Parent != null && node.Parent.Variable.CanRemoveVariable(node.Variable))
                {
                    node.Parent.Variable.RemoveVariable(node.Variable);
                    node.Parent.Children.Remove(node);
                    try
                    {
                        treeView.RefreshObject(node.Parent);
                    }
                    catch { } //TODO: Do this better, works now but it shouldn't be done like this. :p
                }
            }
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView.ExpandAll();
        }

        private void expandAllChildrenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = (VariableListNode)treeView.SelectedObject;
            if (node != null)
            {
                treeView.Expand(node);
                foreach (var c in node.Children)
                {
                    treeView.Expand(c);
                }
            }
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView.CollapseAll();
        }

        private void collapseAllChildrenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = (VariableListNode)treeView.SelectedObject;
            if (node != null)
            {
                foreach (var c in node.Children)
                {
                    treeView.Collapse(c);
                }
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyVariable();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteVariable();
        }

        private void treeView_CellClick(object sender, CellClickEventArgs e)
        {
            if (e.Column == null || e.Item == null)
                return;

            if (e.ClickCount == 2 && e.Column.AspectName == "Name")
            {
                treeView.StartCellEdit(e.Item, 0);
            }
            else if (e.Column.AspectName == "Value")
            {
                treeView.StartCellEdit(e.Item, 1);
            }
        }

        private void ptrPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = (VariableListNode)treeView.SelectedObject;
            if ((node?.Variable as CPtr)?.PtrTarget == null)
                return;

            Chunk = ((CPtr)node.Variable).PtrTarget;
        }

        private void copyTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = (VariableListNode)treeView.SelectedObject;
            if (node?.Parent == null || !node.Parent.Variable.CanRemoveVariable(node.Variable))
                return;
            if (node.Value != null)
            {
                if (node.Value == "")
                    Clipboard.SetText(node.Type + ":??");
                else
                    Clipboard.SetText(node.Value);

            }
        }

        private void copyInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = (VariableListNode)treeView.SelectedObject;

            if (node.Info != null)
            {
                Clipboard.SetText(node.Info);
            }
        }

        internal class VariableListNode
        {
            public string Name
            {
                get
                {
                    if (Variable.Name != null)
                        return Variable.Name;

                    return Parent?.Children.IndexOf(this).ToString() ?? "";
                }
                set
                {
                    if (Variable.Name != null)
                    {
                        Variable.Name = value;
                    }
                }
            }

            public string Value => Variable.ToString();

            public string Type => Variable.Type;

            public string Info => Variable.Info;

            public int ChildCount => Children.Count;

            public List<VariableListNode> Children { get; set; }
            public VariableListNode Parent { get; set; }
            public IEditableVariable Variable { get; set; }
        }

        private void treeView_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            MainController.Get().ProjectUnsaved = true;
        }
    }
}