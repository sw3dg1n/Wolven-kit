using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
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
            treeView.CanExpandGetter = x => ((VariableListNode) x).ChildCount > 0;
            treeView.ChildrenGetter = x => ((VariableListNode) x).Children;
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
                e.Control = ((VariableListNode) e.RowObject).Variable.GetEditor();
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

        private void copyVariable()
        {
            var tocopynodes = (from VariableListNode item in treeView.SelectedObjects where item?.Variable != null select item.Variable).ToList();
            if (tocopynodes.Count > 0)
            {
                CopyController.VariableTargets = tocopynodes;
            }
        }

        private void pasteVariable()
        {
            var node = (VariableListNode) treeView.SelectedObject;
            if (CopyController.VariableTargets == null || node?.Variable == null || !node.Variable.CanAddVariable(null))
            {
                return;
            }

            if (CopyController.VariableTargets.All(x => x is CVariable))
            {
                foreach (var newvar in from v in CopyController.VariableTargets.Select(x => (CVariable) x) let context = new CR2WCopyAction
                {
                    SourceFile = v.cr2w,
                    DestinationFile = node.Variable.CR2WOwner,
                    MaxIterationDepth = 0
                } select v.Copy(context))
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
            var node = (VariableListNode) treeView.SelectedObject;
            if (node?.Variable == null || !node.Variable.CanAddVariable(null))
            {
                return;
            }

            CVariable newvar = null;

            if (node.Variable is CArray)
            {
                var nodearray = (CArray) node.Variable;
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

                ((CHandle) newvar).ChunkHandle = result == DialogResult.Yes;
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
                    catch  {  } //TODO: Do this better, works now but it shouldn't be done like this. :p
                }
            }
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView.ExpandAll();
        }

        private void expandAllChildrenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = (VariableListNode) treeView.SelectedObject;
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
            var node = (VariableListNode) treeView.SelectedObject;
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
            var node = (VariableListNode) treeView.SelectedObject;
            if ((node?.Variable as CPtr)?.PtrTarget == null)
                return;

            Chunk = ((CPtr) node.Variable).PtrTarget;
        }

        private void copyTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = (VariableListNode) treeView.SelectedObject;
            if (node?.Parent == null || !node.Parent.Variable.CanRemoveVariable(node.Variable))
                return;
            if(node.Value != null)
                Clipboard.SetText(node.Value ?? (node.Type + ":??"));
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

            public int ChildCount => Children.Count;

            public List<VariableListNode> Children { get; set; }
            public VariableListNode Parent { get; set; }
            public IEditableVariable Variable { get; set; }
        }

        private void treeView_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            MainController.Get().ProjectUnsaved = true;
        }

        private void addCurveFromSBUILogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Vector3> positions = new List<Vector3>();
            List<Vector3> rotations = new List<Vector3>();
            Vector3 initialTransformPos = new Vector3();
            Vector3 initialTransformRot = new Vector3();

            string line;

            using (var fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\The Witcher 3\\scriptslog.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var stream = new StreamReader(fs, Encoding.Default))
                while ((line = stream.ReadLine()) != null)
                {
                    var str = line;

                    // Initial position
                    if (str.Contains("[W2SCENE]       #pos: [ "))
                    {
                        str = str.Remove(0, 24);
                        str = str.Replace("]", "");

                        initialTransformPos = StrToVec(str);
                        positions.Clear();
                        rotations.Clear();
                    }
                    // Initial rotation
                    else if (str.Contains("[W2SCENE]       #rot: [ "))
                    {
                        str = str.Remove(0, 24);
                        str = str.Replace("]", "");

                        initialTransformRot = StrToVec(str);
                    }
                    // All curve positions / rotations
                    else if (str.Contains("[W2SCENE]       - prop.placement: [0.0, "))
                    {
                        str = str.Remove(0, 40);
                        str = str.Replace("[", "");
                        str = str.Replace("]", "");

                        string[] data = str.Split(',');

                        Vector3 result = new Vector3(
                            float.Parse(data[1]),
                            float.Parse(data[2]),
                            float.Parse(data[3]));


                        Vector3 result2 = new Vector3(
                            float.Parse(data[4]),
                            float.Parse(data[6]),
                            float.Parse(data[5]));

                        positions.Add(result);
                        rotations.Add(result2);
                    }
                }

            // For curves that u want to be on the same level on Z axis
            bool sameZ = false;
            var boxresult = MessageBox.Show("Height value based on origin?", "Height", MessageBoxButtons.YesNo);
            if (boxresult == DialogResult.Yes)
            {
                sameZ = true;
            }

            // ------------------------------------------------------------------------
            List<Vector3> positionsV3 = new List<Vector3>();
            positionsV3 = positions;

            // calculate distances
            float dist = 0;
            float totalDist = 0;
            List<float> distanceArray = new List<float>();
            List<float> timeArray = new List<float>();
            for (int i = 0; i < positionsV3.Count; i++)
            {
                if (i == 0)
                {
                    totalDist += Vector3.Distance(positionsV3[i], positionsV3[positionsV3.Count - 1]);
                    distanceArray.Add(Vector3.Distance(positionsV3[i], positionsV3[positionsV3.Count - 1]));
                }
                else
                {
                    totalDist += Vector3.Distance(positionsV3[i - 1], positionsV3[i]);
                    distanceArray.Add(Vector3.Distance(positionsV3[i - 1], positionsV3[i]));
                }
            }
            for (int i = 0; i < distanceArray.Count; i++)
            {
                timeArray.Add(dist / totalDist);
                dist += distanceArray[i];
            }


            // ------------------------------------------------------------------------
            // Create the SCurveData Array
            var node = (VariableListNode)treeView.SelectedObject;
            if (node?.Variable == null || !node.Variable.CanAddVariable(null))
            {
                return;
            }

            var curveDataArray = CR2WTypeManager.Get().GetByName("array:2,0,SCurveData", "curves", Chunk.cr2w, false);

            curveDataArray.Name = "curves";
            curveDataArray.Type = "array:2,0,SCurveData";


            // ------------------------------------------------------------------------
            // Add 9 elements to SCurveData
            for (int i = 0; i < 9; i++)
            {
                // new curve array element
                var sCurveDataArrayVar = CR2WTypeManager.Get().GetByName("array:2,0,SCurveData", "", Chunk.cr2w, false);

                CVariable curveValues = null;
                curveValues = CR2WTypeManager.Get().GetByName("array:142,0,SCurveDataEntry", "Curve Values", Chunk.cr2w, false);
                curveValues.Name = "Curve Values";
                curveValues.Type = "array:142,0,SCurveDataEntry";

                for (int j = 0; j < positionsV3.Count; j++)
                {
                    //-----------------------------------------------------------------------------------------------
                    //-----------------------------------------------------------------------------------------------
                    // CURVE VALUE SPECIAL
                    CVariable curveValue = null;
                    curveValue = CR2WTypeManager.Get().GetByName("array:142,0,SCurveDataEntry", "", Chunk.cr2w, false);

                    curveValues.AddVariable(curveValue);

                    // me
                    CVariable me = null;
                    me = CR2WTypeManager.Get().GetByName("Float", "me", Chunk.cr2w, false);
                    me.Name = "me";
                    me.Type = "Float";
                    ((CFloat)me).val = timeArray[j];

                    // ntrolPoint
                    CVariable ntrolPoint = null;
                    ntrolPoint = CR2WTypeManager.Get().GetByName("Vector", "ntrolPoint", Chunk.cr2w, false);
                    ntrolPoint.Name = "ntrolPoint";
                    ntrolPoint.Type = "Vector";

                    CVariable x = null;
                    x = CR2WTypeManager.Get().GetByName("Float", "X", Chunk.cr2w, false);
                    x.Name = "X";
                    x.Type = "Float";
                    ((CFloat)x).val = (float)-0.1;
                    CVariable y = null;
                    y = CR2WTypeManager.Get().GetByName("Float", "Y", Chunk.cr2w, false);
                    y.Name = "Y";
                    y.Type = "Float";
                    ((CFloat)y).val = 0;
                    CVariable z = null;
                    z = CR2WTypeManager.Get().GetByName("Float", "Z", Chunk.cr2w, false);
                    z.Name = "Z";
                    z.Type = "Float";
                    ((CFloat)z).val = (float)0.1;
                    CVariable w = null;
                    w = CR2WTypeManager.Get().GetByName("Float", "W", Chunk.cr2w, false);
                    w.Name = "W";
                    w.Type = "Float";
                    ((CFloat)w).val = 0;

                    ((CVector)ntrolPoint).AddVariable(x);
                    ((CVector)ntrolPoint).AddVariable(y);
                    ((CVector)ntrolPoint).AddVariable(z);
                    ((CVector)ntrolPoint).AddVariable(w);

                    // lue
                    CVariable lue = null;
                    lue = CR2WTypeManager.Get().GetByName("Float", "lue", Chunk.cr2w, false);
                    lue.Name = "lue";
                    lue.Type = "Float";

                    if (i == 0)
                        ((CFloat)lue).val = positions[j].X;
                    else if (i == 1)
                        ((CFloat)lue).val = positions[j].Y;
                    else if (i == 2)
                    {
                        if (sameZ)
                            ((CFloat)lue).val = 0;
                        else
                            ((CFloat)lue).val = positions[j].Z;
                    }
                    else if (i == 3)
                        ((CFloat)lue).val = rotations[j].X;
                    else if (i == 4)
                        ((CFloat)lue).val = rotations[j].Y;
                    else if (i == 5)
                        ((CFloat)lue).val = rotations[j].Z;
                    else if (i > 5)
                        ((CFloat)lue).val = 1;


                    // rveTypeL
                    CVariable rveTypeL = null;
                    rveTypeL = CR2WTypeManager.Get().GetByName("Uint16", "rveTypeL", Chunk.cr2w, false);
                    rveTypeL.Name = "rveTypeL";
                    rveTypeL.Type = "Uint16";
                    ((CUInt16)rveTypeL).val = 3;

                    // rveTypeR
                    CVariable rveTypeR = null;
                    rveTypeR = CR2WTypeManager.Get().GetByName("Uint16", "rveTypeR", Chunk.cr2w, false);
                    rveTypeR.Name = "rveTypeR";
                    rveTypeR.Type = "Uint16";
                    ((CUInt16)rveTypeR).val = 3;

                    if (((CFloat)me).val != 0)
                        ((CVariable)curveValue).AddVariable(me);
                    ((CVariable)curveValue).AddVariable(ntrolPoint);
                    if (((CFloat)lue).val != 0)
                        ((CVariable)curveValue).AddVariable(lue);
                    ((CVariable)curveValue).AddVariable(rveTypeL);
                    ((CVariable)curveValue).AddVariable(rveTypeR);
                    // END OF CURVE SPECIAL
                    //-----------------------------------------------------------------------------------------------
                    //-----------------------------------------------------------------------------------------------
                }

                // curve value type
                CVariable valueType = null;
                valueType = CR2WTypeManager.Get().GetByName("ECurveValueType", "value type", Chunk.cr2w, false);
                valueType.Name = "value type";
                valueType.Type = "ECurveValueType";
                ((CName)valueType).Value = "CVT_Float";

                // curve base type
                CVariable baseType = null;
                baseType = CR2WTypeManager.Get().GetByName("ECurveBaseType", "type", Chunk.cr2w, false);
                baseType.Name = "type";
                baseType.Type = "ECurveBaseType";
                ((CName)baseType).Value = "CT_Smooth";

                // is looped
                CVariable isLooped = null;
                isLooped = CR2WTypeManager.Get().GetByName("Bool", "is looped", Chunk.cr2w, false);
                isLooped.Name = "is looped";
                isLooped.Type = "Bool";
                ((CBool)isLooped).val = true;

                ((CVariable)sCurveDataArrayVar).AddVariable(curveValues);
                ((CVariable)sCurveDataArrayVar).AddVariable(valueType);
                ((CVariable)sCurveDataArrayVar).AddVariable(baseType);
                ((CVariable)sCurveDataArrayVar).AddVariable(isLooped);

                ((CVariable)curveDataArray).AddVariable(sCurveDataArrayVar);
            }

            // Add Initial Parent Transform variable            
            node = (VariableListNode)treeView.SelectedObject;
            CVariable initialParentTransform = null;
            initialParentTransform = CR2WTypeManager.Get().GetByName("EngineTransform", "initialParentTransform", Chunk.cr2w, false);
            initialParentTransform.Name = "initialParentTransform";
            initialParentTransform.Type = "EngineTransform";
            ((CEngineTransform)initialParentTransform).x.val = initialTransformPos.X;
            ((CEngineTransform)initialParentTransform).y.val = initialTransformPos.Y;
            ((CEngineTransform)initialParentTransform).z.val = initialTransformPos.Z;
            ((CEngineTransform)initialParentTransform).pitch.val = initialTransformRot.X;
            ((CEngineTransform)initialParentTransform).roll.val = initialTransformRot.Y;
            ((CEngineTransform)initialParentTransform).yaw.val = initialTransformRot.Z;

            AddAndRefresh(curveDataArray, node);
            AddAndRefresh(initialParentTransform, node);
        }


        private void AddAndRefresh(CVariable varr, VariableListNode node)
        {
            node.Variable.AddVariable(varr);

            var subnode = AddListViewItems(varr, node);
            node.Children.Add(subnode);

            treeView.RefreshObject(node);
            treeView.RefreshObject(subnode);
        }


        private Vector3 StrToVec(string sVector)
        {
            string[] sArray = sVector.Split(',');
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }
    }
}