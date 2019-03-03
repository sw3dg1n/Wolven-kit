using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WolvenKit
{
    enum EDisplayNameType
    {
        VAR,
        GROUP
    }

    /// <summary>
    /// The main strings encoder form.
    /// </summary>
    public partial class frmStringsGui : Form
    {
        int counter = 0;
        List<int> modIDs = new List<int> { 0 };

        bool fileOpened = false;
        bool multipleIDs = false;
        bool rowAddedAutomatically = false;
        bool abortedSwitchingBackToAllLanguages = false;
        bool fileIsSaved = false;

        string currentModID = "";
        List<string> groups = new List<string>();

        int idsLimit = 1000;

        IEnumerable<W3Strings.W3Language> languages = W3Strings.W3Language.languages;
        string languageTabSelected = "ar";

        List<LanguageStringsCollection> languagesStrings = new List<LanguageStringsCollection>();

        DataTable dataTableGridViewSource;

        Mod.W3Mod activeMod = MainController.Get().Window.ActiveMod;

        /// <summary>
        /// Initializes the form and reads CSV files if it finds any in \\strings\\CSV.
        /// </summary>
        public frmStringsGui()
        {
            InitializeComponent();

            comboBoxLanguagesMode.SelectedIndex = 0;
            CreateDataTable();

            if (activeMod != null)
            {
                var csvDir = (activeMod.ProjectDirectory + "\\strings\\CSV");
                if (!Directory.Exists(csvDir))
                    return;

                string[] fileNames = Directory.GetFiles(csvDir, "*.csv*", SearchOption.AllDirectories).Select(x => Path.GetFullPath(x)).ToArray();
                if (fileNames.Length == 0)
                    return;

                comboBoxLanguagesMode.SelectedIndex = 1;
                languagesStrings.Clear();

                rowAddedAutomatically = true;

                {
                    List<string[]> rows = ParseCSV(fileNames[0]);
                    GetCSVIDs(rows);
                }
                
                foreach (var file in fileNames)
                {
                    List<string[]> rows = ParseCSV(file);

                    var firstLine = File.ReadLines(file, Encoding.UTF8).First();
                    var language = Regex.Match(firstLine, "language=([a-zA-Z]+)]").Groups[1].Value;

                    var strings = new List<List<string>>();

                    rows.ForEach(x =>
                    {
                        strings.Add(new List<string>() { x[0], x[1], x[2], x[3] });
                    });

                    languagesStrings.Add(new LanguageStringsCollection(language, strings));

                    foreach (var lang in languagesStrings)
                    {
                        if (lang.language == "ar")
                        {
                            dataTableGridViewSource.Clear();

                            foreach (var str in lang.strings)
                                dataTableGridViewSource.Rows.Add(str[0], str[1], str[2], str[3]);
                            break;
                        }
                    }
                }

                fileOpened = true;
                HashStringKeys();
                UpdateModID();
                dataGridViewStrings.Visible = true;
                rowAddedAutomatically = false;
            }

        }

        /*
            Events
        */

        /*
            toolStrip Buttons
        */

        /// <summary>
        /// Saves strings to CSV if there are any, shows an error message otherwise.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            if (dataGridViewStrings.Rows.Count != 1)
                SaveCSV();
            else
                MessageBox.Show("Current file is empty.", "Wolven Kit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        /// <summary>
        /// Opens a CSV file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            OpenCSV();
        }

        /// <summary>
        /// Generates strings from menu xml. The menu should not contain custom localization tags.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonGenerateXML_Click(object sender, EventArgs e)
        {
            GenerateFromXML();
        }

        /// <summary>
        /// Generates strings from script files - grabs string keys with a specified prefix.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonGenerateScripts_Click(object sender, EventArgs e)
        {
            ReadScripts();
        }

        /// <summary>
        /// Encodes strings to w3string file(s).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonEncode_Click(object sender, EventArgs e)
        {
            if (dataGridViewStrings.Rows.Count != 1)
                Encode();
            else
                MessageBox.Show("Current file is empty.", "Wolven Kit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        /// <summary>
        /// Opens the importer, which can be used to explore existing w3string files and import from them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonImport_Click(object sender, EventArgs e)
        {
            ImportW3Strings();
        }

        /*
            toolStrip Menus
        */

        /// <summary>
        /// Switches visibility of the ID column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void idToolStripMenuItem_Click(object sender, EventArgs e)
        {
            idToolStripMenuItem.Checked = dataGridViewStrings.Columns[0].Visible = !idToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Switches visibility of the hex key column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hexKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hexKeyToolStripMenuItem.Checked = dataGridViewStrings.Columns[1].Visible = !hexKeyToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Switches visibility of the string key column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stringKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stringKeyToolStripMenuItem.Checked = dataGridViewStrings.Columns[2].Visible = !stringKeyToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Creates a new strings project, asks to save changes if necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewStrings.Visible == false)
                return;
            if (!fileIsSaved)
            {
                var result = MessageBox.Show("File is not saved. Do you want to continue anyway?", "Wolven Kit", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                if (result == DialogResult.Cancel)
                    return;
            }

            dataTableGridViewSource.Clear();
            CreateDataTable();
            modIDs.Clear();
            textBoxModID.Text = "";
            dataGridViewStrings.Visible = false;
            languagesStrings.Clear();
        }

        /// <summary>
        /// Opens a CSV file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCSV();
        }

        /// <summary>
        /// Opens the importer, which can be used to explore existing w3string files and import from them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportW3Strings();
        }

        /// <summary>
        /// Generates strings from menu xml. The menu should not contain custom localization tags.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fromXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateFromXML();
        }

        /// <summary>
        /// Generates strings from script files - grabs string keys with a specified prefix.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fromScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadScripts();
        }

        /*
            Other 
        */

        /// <summary>
        /// Verifies the entered mod ID and uses it if valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxModID_Leave(object sender, EventArgs e)
        {
            FillModIDIfValid();
            currentModID = textBoxModID.Text;
        }

        /// <summary>
        /// Event to save ID changes with Enter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxModID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                splitContainerMain.Focus();
            }
        }

        /// <summary>
        /// Fires up when the cell is validated, checks for mod ID, calculates final IDs and hex keys.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewStrings_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (rowAddedAutomatically)
                return;

            if (textBoxModID.Text == "")
            {
                AskForModID();
                return;
            }

            if (dataGridViewStrings.Rows.Count >= 3)
                dataGridViewStrings.Rows[dataGridViewStrings.Rows.Count - 2].Cells[0].Value =
                    Convert.ToInt32(dataGridViewStrings.Rows[dataGridViewStrings.Rows.Count - 3].Cells[0].Value) + 1;
            else
                dataGridViewStrings.Rows[0].Cells[0].Value = modIDs[0] * 1000 + 2110000000;

            HashStringKeys();
            fileIsSaved = false;
        }

        /// <summary>
        /// Recalculates IDs for rows when row is deleted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewStrings_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            UpdateModID();
        }

        /// <summary>
        /// Switches languages mode between English and separate languages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxLanguagesMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (abortedSwitchingBackToAllLanguages)
            {
                abortedSwitchingBackToAllLanguages = false;
                //return;
            }
            if (comboBoxLanguagesMode.SelectedIndex == 1)
            {
                tabControlLanguages.Controls.Clear();

                var allLanguagesStrings = new List<List<string>>();

                foreach (DataGridViewRow row in dataGridViewStrings.Rows)
                {
                    if (row.Cells[0].Value != null)
                        allLanguagesStrings.Add(new List<string>() { row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString() });
                }

                languagesStrings.Add(new LanguageStringsCollection("all", allLanguagesStrings));


                foreach (var language in languages)
                {
                    var languageStrings = new List<List<string>>();

                    foreach (var str in allLanguagesStrings)
                        languageStrings.Add(new List<string>() { str[0], str[1], str[2], str[3] });

                    languagesStrings.Add(new LanguageStringsCollection(language.Handle, languageStrings));

                    var newTabPage = new System.Windows.Forms.TabPage();
                    newTabPage.Location = new System.Drawing.Point(4, 22);
                    newTabPage.Name = "tabPage" + language.Handle;
                    newTabPage.Padding = new System.Windows.Forms.Padding(3);
                    newTabPage.Size = new System.Drawing.Size(998, 0);
                    newTabPage.TabIndex = 0;
                    newTabPage.Text = language.Handle;
                    newTabPage.UseVisualStyleBackColor = true;
                    tabControlLanguages.Controls.Add(newTabPage);
                }
            }
            else if (dataTableGridViewSource != null)
            {
                var result = MessageBox.Show("Are you sure? English strings will be used for all languages.", "Wolven Kit", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                if (result == DialogResult.Cancel)
                {
                    comboBoxLanguagesMode.SelectedIndex = 1;
                    abortedSwitchingBackToAllLanguages = true;
                    return;
                }

                tabControlLanguages.Controls.Clear();

                dataTableGridViewSource.Rows.Clear();
                foreach (var str in languagesStrings[7].strings)
                    dataTableGridViewSource.Rows.Add(str[0], str[1], str[2], str[3]);
                languagesStrings.Clear();

                var newTabPage = new System.Windows.Forms.TabPage();
                newTabPage.Location = new System.Drawing.Point(4, 22);
                newTabPage.Name = "tabPageAllLanguages";
                newTabPage.Padding = new System.Windows.Forms.Padding(3);
                newTabPage.Size = new System.Drawing.Size(998, 0);
                newTabPage.TabIndex = 0;
                newTabPage.Text = "All Languages";
                newTabPage.UseVisualStyleBackColor = true;
                tabControlLanguages.Controls.Add(newTabPage);
            }
        }

        /*
            End of events
        */

        /// <summary>
        /// Calculates Fowler–Noll–Vo based hash of string keys, used as hex key.
        /// Similar to FNV-1 with addition instead of XOR.
        /// </summary>
        private void HashStringKeys()
        {
            foreach (DataGridViewRow row in dataGridViewStrings.Rows)
            {
                if (row.Cells[2].Value == null)
                    return;
                string key = row.Cells[2].Value.ToString();
                if (key == "")
                    return;
                char[] keyConverted = key.ToCharArray();
                uint hash = 0;

                for (int i = 0; i < keyConverted.Count(); ++i)
                {
                    char c = keyConverted[i];
                    c = Char.ToLower(c);
                    hash *= 31;
                    hash += (uint)c;
                }
                string hex_key = hash.ToString("X");

                const int targetLength = 8;
                int currentLength = hex_key.Count();
                for (int i = 0; i < targetLength - currentLength; ++i)
                {
                    hex_key = hex_key.Insert(0, "0");
                }
                row.Cells[1].Value = hex_key;
            }
        }

        /// <summary>
        /// Gets strings from strings manager that were selected in importer.
        /// </summary>
        private void ImportW3Strings()
        {
            if (textBoxModID.Text == "")
            {
                AskForModID();
                return;
            }

            var guiStrings = new List<string>();

            foreach (DataGridViewRow row in dataGridViewStrings.Rows)
                if (row.Cells[3].Value != null)
                    guiStrings.Add(row.Cells[3].Value.ToString());

            var importer = new frmStringsGuiImporter(guiStrings);

            importer.ShowDialog();
            var stringsManager = MainController.Get().W3StringManager;
            var strings = stringsManager.GetImportedStrings();
            if (strings == null)
                return;

            foreach (var str in strings)
            {
                dataTableGridViewSource.Rows.Add(str[0], str[1], "", str[2]);
            }

            stringsManager.ClearImportedStrings();
            UpdateModID();
        }

        /// <summary>
        /// Checks for valid mod ID, asks for one if needed.
        /// If ID is valid it generates strings from menu xml. 
        /// The menu should not contain custom localization tags.
        /// </summary>
        private void GenerateFromXML()
        {
            if (textBoxModID.Text != "" && FillModIDIfValid())
                ReadXML();
            else
                AskForModID();
        }

        /// <summary>
        /// Shows a message asking to enter mod ID.
        /// </summary>
        private void AskForModID()
        {
            MessageBox.Show("Enter mod ID.", "Wolven Kit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            textBoxModID.Text = "";
        }

        /// <summary>
        /// Dialog that gets the prefix used for grabbing keys from scripts.
        /// </summary>
        /// <returns>The selected prefix, "Cancelled" if user cancells.</returns>
        public string ShowScriptPrefixDialog()
        {
            frmStringsGuiScriptsPrefixDialog testDialog = new frmStringsGuiScriptsPrefixDialog();
            var prefix = "";
            if (testDialog.ShowDialog(this) == DialogResult.OK)
                prefix = testDialog.prefix;
            else
                prefix = "Cancelled";

            testDialog.Dispose();

            return prefix;
        }

        /// <summary>
        /// Walks recursively through scripts, grabs the key IDs and generates string rows from them.
        /// </summary>
        private void ReadScripts()
        {
            if (textBoxModID.Text == "")
            {
                AskForModID();
                return;
            }
            
            string scriptsDir = "";

            if (activeMod != null)
                scriptsDir = (activeMod.FileDirectory + "\\scripts");

            var prefix = ShowScriptPrefixDialog();

            if (prefix == "Cancelled")
                return;
            else if (prefix == "")
            {
                MessageBox.Show("Empty prefix.", "Wolven Kit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }


            string contents = "";
            if (!Directory.Exists(scriptsDir))
            {
                FolderBrowserDialog fbw = new FolderBrowserDialog()
                {
                    Description = "Please select the scripts directory!",
                   
                };
                if (fbw.ShowDialog() == DialogResult.OK) scriptsDir = fbw.SelectedPath;
            }

            string[] filenames = Directory.GetFiles(scriptsDir, "*.ws*", SearchOption.AllDirectories).Select(x => Path.GetFullPath(x)).ToArray();
            foreach (var file in filenames)
                contents += (File.ReadAllText(file));

            Regex regex = new Regex("\"(" + prefix + ".+)\"");
            MatchCollection matches = regex.Matches(contents);

            if (matches.Count == 0)
            {
                MessageBox.Show("No matches.", "Wolven Kit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            List<string> strings = new List<string>();
            var convertToLower = false;

            int counter = 0;
            foreach (Match match in matches)
            {
                if (match.Groups[1].Value.ToLower() != match.Groups[1].Value && !convertToLower)
                {
                    var result = MessageBox.Show("Found uppercase string keys. String keys called in scripts must be all lowercase. " +
                        "Do you want to read them as lowercase? You will need to change the string keys in the scripts, or pass them to StrLower() function.", "Wolven Kit", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                    if (result == DialogResult.OK)
                        convertToLower = true;
                }

                ++counter;
            }

            if (convertToLower)
                foreach (Match match in matches)
                    strings.Add((counter + 2110000000 + modIDs[0] * 1000).ToString() + "||" + match.Groups[1].Value + "|" + match.Groups[1].Value.ToLower());
            else
                foreach (Match match in matches)
                    strings.Add((counter + 2110000000 + modIDs[0] * 1000).ToString() + "||" + match.Groups[1].Value + "|" + match.Groups[1].Value);

            List<string[]> rows = strings.Select(x => x.Split('|')).ToList();

            rowAddedAutomatically = true;

            currentModID = textBoxModID.Text;
            rows.ForEach(x =>
            {
                dataTableGridViewSource.Rows.Add(x);
            });

            rowAddedAutomatically = false;

            dataGridViewStrings.Visible = true;
            UpdateModID();
            HashStringKeys();
        }

        /// <summary>
        /// Gets the menu file path and reads it, generates string rows.
        /// </summary>
        private void ReadXML()
        {
            rowAddedAutomatically = true;
            //TODO check tags for custom display names, add prefixes to keys
            string path = GetXMLPath();

            // to prevent ID being 0 when Leave event for text box wasn't triggered
            FillModIDIfValid();

            if (path != "")
            {
                //Fix encoding
                File.WriteAllLines(path, new string[] { "<?xml version=\"1.0\" encoding=\"utf-8\"?>" }.ToList().Concat(File.ReadAllLines(path).Skip(1).ToArray()));

                //dataTableGridViewSource = (DataTable)dataGridViewStrings.DataSource;
                XDocument doc = XDocument.Load(path);

                // vars displayNames
                foreach (var vars in doc.Descendants("UserConfig").Descendants("Group").Descendants("VisibleVars"))
                {
                    foreach (var var in vars.Descendants("Var"))
                    {
                        String name = var.Attribute("displayName").Value;
                        if (counter > idsLimit)
                            dataTableGridViewSource.Rows.Add(counter + 2110000000 + modIDs[1] * 1000, "", DisplayNameToKey(name), name);
                        else
                            dataTableGridViewSource.Rows.Add(counter + 2110000000 + modIDs[0] * 1000, "", DisplayNameToKey(name), name);

                        ++counter;
                    }
                }
                // optionsArray vars displayNames
                foreach (var vars in doc.Descendants("UserConfig").Descendants("Group").Descendants("VisibleVars").Descendants("OptionsArray"))
                {
                    foreach (var var in vars.Descendants("Option"))
                    {
                        String name = var.Attribute("displayName").Value;
                        if (counter > idsLimit)
                            dataTableGridViewSource.Rows.Add(counter + 2110000000 + modIDs[1] * 1000, "", DisplayNameToKey(name, true), name);
                        else
                            dataTableGridViewSource.Rows.Add(counter + 2110000000 + modIDs[0] * 1000, "", DisplayNameToKey(name, true), name);

                        ++counter;
                    }
                }

                // groups displayNames
                foreach (var vars in doc.Descendants("UserConfig"))
                {
                    foreach (var var in vars.Descendants("Group"))
                    {
                        String name = var.Attribute("displayName").Value;

                        List<string> groupNames = DisplayNameToKeyGroup(name);

                        foreach (var groupName in groupNames)
                        {
                            List<string> splitGroupName = groupName.Split('_').ToList();
                            splitGroupName.RemoveAt(0);
                            string localisationName = String.Join("", splitGroupName);

                            if (counter > idsLimit)
                                dataTableGridViewSource.Rows.Add(counter + 2110000000 + modIDs[1] * 1000, "", groupName, localisationName);
                            else
                                dataTableGridViewSource.Rows.Add(counter + 2110000000 + modIDs[0] * 1000, "", groupName, localisationName);
                            ++counter;
                        }
                    }
                }
            }
            rowAddedAutomatically = false;

            HashStringKeys();
            UpdateModID();
        }

        /// <summary>
        /// Dialog that gets menu XML path.
        /// </summary>
        /// <returns>The XML path, empty string if user cancelled.</returns>
        private string GetXMLPath()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML | *.xml";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return ofd.FileName;
            }
            else
                return "";
        }

        /// <summary>
        /// Gets string key for option group.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private List<string> DisplayNameToKeyGroup(string name)
        {
            char[] nameConverted = name.ToCharArray(0, name.Length);
            List<string> stringKeys = new List<string>();
            string stringKey = "";

            for (int i = 0; i < nameConverted.Length; ++i)
            {
                if (nameConverted[i] == ' ')
                    nameConverted[i] = '_';

                stringKey += nameConverted[i];
            }
                
            string[] stringKeySplitted = stringKey.Split('.');

            if (groups.Count() == 0)
            {
                groups.Add(stringKeySplitted[stringKeySplitted.Length - 1]);
                stringKeys.Add("panel_" + stringKeySplitted[stringKeySplitted.Length - 1]);
            }

            for (int i = 0; i < stringKeySplitted.Length; ++i)
            {
                for (int j = 0; j < groups.Count(); ++j)
                    if (!groups.Contains(stringKeySplitted[i]))
                    {
                        groups.Add(stringKeySplitted[i]);
                        stringKeys.Add("panel_" + stringKeySplitted[i]);
                    }
            }

            return stringKeys;
        }

        /// <summary>
        /// Gets string key for option.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string DisplayNameToKey(string name, bool optionsArray = false)
        {
            char[] nameConverted = name.ToCharArray(0, name.Length);
            string stringKey = "";

            if (!optionsArray)
            {
                stringKey += "option_";
            }
            else
            {
                stringKey += "preset_value_";
            }
            

            for (int i = 0; i < nameConverted.Length; ++i)
            {
                if (nameConverted[i] == ' ')
                {
                    nameConverted[i] = '_';
                    stringKey += nameConverted[i];
                }
                else
                {
                    stringKey += nameConverted[i];
                }
            }

            return stringKey;
        }

        /// <summary>
        /// Checks if mod ID is valid.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsIDValid(string id)
        {
            char[] digits = new char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', };
            char[] convertedId = id.ToCharArray();
            int validCharCount = 0;
            int idLength = id.Length;
            for (int i = 0; i < idLength; ++i)
            {
                for (int j = 0; j < 10; ++j)
                    if (convertedId[i] == digits[j])
                    {
                        ++validCharCount;
                        break;
                    }
                    else if (convertedId[i] == ';')
                    {
                        multipleIDs = true;
                        ++validCharCount;
                        break;
                    }
            }
            if (!multipleIDs && idLength > 4)
                return false;
            else if (validCharCount == idLength && validCharCount != 0)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if multiple mod IDs are valid.
        /// </summary>
        /// <param name="splittedIDs"></param>
        /// <returns></returns>
        private bool AreAllIDsValid(string[] splittedIDs)
        {
            foreach (string modID in splittedIDs)
            {
                int count = splittedIDs.Count(f => f == modID);
                if (modID.Length > 4)
                    return false;
                else if (count > 1)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Verifies the entered mod ID and uses it if valid.
        /// </summary>
        /// <returns></returns>
        private bool FillModIDIfValid()
        {
            modIDs.Clear();
            string[] splittedIDs;

            if (!IsIDValid(textBoxModID.Text))
            {
                MessageBox.Show("Invalid Mod ID.", "Wolven Kit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                if (currentModID != "")
                    textBoxModID.Text = currentModID;
                else
                    textBoxModID.Text = "";

                return false;
            }
            else
            {
                if (!multipleIDs)
                {
                    modIDs.Add(Convert.ToInt32(textBoxModID.Text));
                    dataGridViewStrings.Visible = true;
                    UpdateModID();
                }
                else
                {
                    splittedIDs = textBoxModID.Text.Split(';');

                    if (!AreAllIDsValid(splittedIDs))
                    {
                        MessageBox.Show("Invalid Mod ID.", "Wolven Kit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                        if (currentModID != "")
                            textBoxModID.Text = currentModID;
                        else
                            textBoxModID.Text = "";

                        return false;
                    }
                    else if (!dataGridViewStrings.Visible)
                        dataGridViewStrings.Visible = true;
                    foreach (var id in splittedIDs)
                        modIDs.Add(Convert.ToInt32(id));
                }
            }

            return true;
        }

        /// <summary>
        /// Recalculates IDs for rows when row is deleted.
        /// </summary>
        private void UpdateModID()
        {
            rowAddedAutomatically = true;
            //TODO - fix for empty dataGridView
            if (dataTableGridViewSource == null)
                return;

            int counter = 0;
            int newModID = modIDs[0] * 1000 + 2110000000;
            foreach (DataRow row in dataTableGridViewSource.Rows)
            {
                int newModIDRow = newModID + counter;
                row[0] = newModIDRow.ToString();
                var test = row.ItemArray[0];
                ++counter;
                if (counter / idsLimit >= modIDs.Count)
                {
                    MessageBox.Show("Number of strings exceeds " + counter + ", number of IDs: " + modIDs.Count
                        + "\nStrings Limit per one modID is " + idsLimit + " Enter more modIDs.", "Wolven Kit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    dataTableGridViewSource.Clear();
                    break;
                }
            }
            rowAddedAutomatically = false;
        }

        /// <summary>
        /// Saves strings to CSV.
        /// </summary>
        private void SaveCSV()
        {
            dataGridViewStrings.EndEdit();
            HashStringKeys();

            var outputPath = "";
            if (activeMod != null)
            {
                outputPath = (activeMod.ProjectDirectory + "\\strings\\CSV");
                if (!Directory.Exists(outputPath))
                    Directory.CreateDirectory(activeMod.ProjectDirectory + "\\strings\\CSV");
            }
            else
                outputPath = GetPath();
            if (outputPath == "")
                return;
            var sb = new StringBuilder();

            if (comboBoxLanguagesMode.SelectedIndex == 0)
            {
                if (dataGridViewStrings.Rows.Count >= 3)
                    dataGridViewStrings.Rows[dataGridViewStrings.Rows.Count - 2].Cells[0].Value =
                        Convert.ToInt32(dataGridViewStrings.Rows[dataGridViewStrings.Rows.Count - 3].Cells[0].Value) + 1;
                else
                    dataGridViewStrings.Rows[0].Cells[0].Value = modIDs[0] * 1000 + 2110000000;

                foreach (DataGridViewRow row in dataGridViewStrings.Rows)
                {
                    if (row.Cells[0].Value == DBNull.Value)
                        continue;
                    var cells = row.Cells.Cast<DataGridViewCell>();


                    sb.AppendLine(string.Join("|", cells.Select(cell => cell.Value).ToArray()));
                }

                int languagesCount = languages.Count();

                foreach (var language in languages)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(outputPath + "\\" + language.Handle + ".csv"))
                    {
                        file.WriteLine(";meta[language=" + language.Handle + "]", Encoding.UTF8);
                        file.WriteLine("; id | key(hex) | key(str) | text", Encoding.UTF8);

                        string csv = sb.ToString();

                        // leaving space for the hex key empty
                        if (!fileOpened)
                        {
                            List<string> splittedCsv = csv.Split('\n').ToList();
                            int splittedCsvLength = splittedCsv.Count();
                            for (int j = 0; j < splittedCsvLength; ++j)
                                if (splittedCsv[j] == "\r" || splittedCsv[j] == "")
                                {
                                    // remove empty rows
                                    splittedCsv.RemoveAt(j);
                                    --splittedCsvLength;
                                    --j;
                                }

                            csv = String.Join("\n", splittedCsv);
                        }

                        file.WriteLine(csv, Encoding.UTF8);
                    }
                }
            }
            else
            {
                foreach (var language in languagesStrings)
                {
                    foreach (var line in language.strings)
                        sb.AppendLine(string.Join("|", line.ToArray()));

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(outputPath + "\\" + language.language + ".csv"))
                    {
                        file.WriteLine(";meta[language=" + language.language + "]", Encoding.UTF8);
                        file.WriteLine("; id | key(hex) | key(str) | text", Encoding.UTF8);

                        string csv = sb.ToString();

                        // leaving space for the hex key empty
                        if (!fileOpened)
                        {
                            List<string> splittedCsv = csv.Split('\n').ToList();
                            int splittedCsvLength = splittedCsv.Count();
                            for (int j = 0; j < splittedCsvLength; ++j)
                                if (splittedCsv[j] == "\r" || splittedCsv[j] == "")
                                {
                                    // remove empty rows
                                    splittedCsv.RemoveAt(j);
                                    --splittedCsvLength;
                                    --j;
                                }

                            csv = String.Join("\n", splittedCsv);
                        }

                        file.WriteLine(csv, Encoding.UTF8);
                    }
                    sb.Clear();
                }
            }

            WriteHash("csv");
            fileIsSaved = true;
        }

        /// <summary>
        /// Dialog that gets folder path.
        /// </summary>
        /// <returns></returns>
        private string GetPath()
        {
            FolderBrowserDialog fbw = new FolderBrowserDialog();
            if (fbw.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return fbw.SelectedPath + "//";
            }
            else
                return "";
        }

        /// <summary>
        /// Creates the source for dataGridViewStrings.
        /// </summary>
        private void CreateDataTable()
        {
            if (dataTableGridViewSource == null)
            {
                dataTableGridViewSource = new DataTable();

                dataGridViewStrings.Columns.Clear();

                dataTableGridViewSource.Columns.Add("Id");
                dataTableGridViewSource.Columns.Add("Hex Key");
                dataTableGridViewSource.Columns.Add("String Key");
                dataTableGridViewSource.Columns.Add("Localisation");

                dataGridViewStrings.DataSource = dataTableGridViewSource;

                dataGridViewStrings.Columns[0].ReadOnly = true;
                dataGridViewStrings.Columns[1].ReadOnly = true;

                foreach (DataGridViewColumn column in dataGridViewStrings.Columns)
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary>
        /// Parses the string IDs from CSV.
        /// </summary>
        /// <param name="rows"></param>
        private void GetCSVIDs(List<string[]> rows)
        {
            modIDs.Clear();
            modIDs.Add((Convert.ToInt32(rows[0][0]) - 2110000000) / 1000);

            // get multiple ids
            foreach (var row in rows)
            {
                int currentRowID = Convert.ToInt32((Convert.ToInt32(row[0]) - 2110000000) / 1000);
                foreach (var addedID in modIDs.ToList()) // to prevent modified collection exception
                    if (currentRowID != addedID)
                        if (!multipleIDs)
                        {
                            multipleIDs = true;
                            modIDs.Add(currentRowID);
                        }
            }

            textBoxModID.Text = "";
            if (multipleIDs)
            {
                foreach (var id in modIDs)
                    textBoxModID.Text += Convert.ToString(id) + ";";
                // delete last ;
                textBoxModID.Text = textBoxModID.Text.Remove(textBoxModID.Text.Length - 1);
            }
            else
                textBoxModID.Text = Convert.ToString(modIDs[0]);
        }

        /// <summary>
        /// Parses the CSV, removes invalid rows.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Rows as a List of string[]</returns>
        private List<string[]> ParseCSV(string path)
        {
            List<string[]> rows = File.ReadAllLines(path, Encoding.UTF8).Select(x => x.Split('|')).ToList();

            for (int i = 0; i < rows.Count(); ++i)
                if (rows[i].Length == 1)
                {
                    rows.RemoveAt(i);
                    --i;
                }
                else if (rows[i][0][0] == ';') // need to be separated in two statements so this won't compare an empty row
                {
                    rows.RemoveAt(i);
                    --i;
                }

            return rows;
        }

        /// <summary>
        /// Opens a CSV file.
        /// </summary>
        private void OpenCSV()
        {
            rowAddedAutomatically = true;
            string filePath;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV | *.csv;";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = ofd.FileName;

                List<string[]> rows = ParseCSV(filePath);
                GetCSVIDs(rows);

                if (comboBoxLanguagesMode.SelectedIndex == 1)
                {
                    var firstLine = File.ReadLines(filePath, Encoding.UTF8).First();
                    var language = Regex.Match(firstLine, "language=([a-zAZ]+)]").Groups[1].Value;
                    var strings = new List<List<string>>();

                    rows.ForEach(row =>
                    {
                        strings.Add(new List<string>() { row[0], row[1], row[2], row[3] });
                    });

                    foreach (var lang in languagesStrings)
                    {
                        if (lang.language == language)
                        {
                            lang.strings = strings;

                            if (lang.language == "ar" && languageTabSelected == "ar")
                                foreach (var str in lang.strings)
                                    dataTableGridViewSource.Rows.Add(str[0], str[1], str[2], str[3]);
                            break;
                        }
                    }
                }
                else
                {
                    currentModID = textBoxModID.Text;
                    rows.ForEach(row =>
                    {
                        dataTableGridViewSource.Rows.Add(row);
                    });
                }
            }
            else
                return;

            fileOpened = true;
            HashStringKeys();
            UpdateModID();
            dataGridViewStrings.Visible = true;
            rowAddedAutomatically = false;
        }

        /// <summary>
        /// Encodes strings to w3string file(s).
        /// </summary>
        private void Encode()
        {
            dataGridViewStrings.EndEdit();
            HashStringKeys();

            string stringsDir = "";
            if (activeMod != null)
            {
                stringsDir = (activeMod.ProjectDirectory + "\\strings");
                if (!Directory.Exists(stringsDir))
                    Directory.CreateDirectory(activeMod.ProjectDirectory + "\\strings");
            }
            else
                stringsDir = GetPath();

            if (stringsDir == "")
                return;
            if (comboBoxLanguagesMode.SelectedIndex == 0)
            {
                var strings = new List<List<string>>();
                foreach (DataGridViewRow row in dataGridViewStrings.Rows)
                {
                    if (row.Cells[0].Value == DBNull.Value || row.Cells[0].Value == null)
                        continue;

                    var str = new List<string>();
                    var cells = row.Cells.Cast<DataGridViewCell>();
                    foreach (var cell in cells)
                    {
                        if (cell == cells.ElementAt(1))
                            continue;
                        str.Add(cell.Value.ToString());
                    }
                    strings.Add(str);
                }
                foreach (var lang in languages)
                {
                    var w3tringFile = new W3Strings.W3StringFile();
                    w3tringFile.Create(strings, lang.Handle);
                    using (var bw = new BinaryWriter(File.OpenWrite(stringsDir + "\\" + lang.Handle + ".w3strings")))
                    {
                        w3tringFile.Write(bw);
                    }
                }
            }
            else
                foreach (var lang in languagesStrings)
                {
                    if (lang.language == "all")
                        continue;
                    else if (lang.language == languageTabSelected)
                    {
                        lang.strings.Clear();

                        foreach (DataGridViewRow row in dataGridViewStrings.Rows)
                        {
                            if (row.Cells[0].Value != null)
                            {
                                lang.strings.Add(new List<string>() { row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString() });
                            }
                        }
                    }
                    var w3tringFile = new W3Strings.W3StringFile();
                    var stringsBlock1Strings = new List<List<string>>();
                    foreach (var str in lang.strings)
                    {
                        stringsBlock1Strings.Add(new List<string>() { str[0], str[2], str[3] });
                    }
                    w3tringFile.Create(stringsBlock1Strings, lang.language);
                    using (var bw = new BinaryWriter(File.OpenWrite(stringsDir + "\\" + lang.language + ".w3strings")))
                    {
                        w3tringFile.Write(bw);
                    }
                }

            WriteHash("encoded");

            MessageBox.Show("Strings encoded.", "Wolven Kit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        /// <summary>
        /// Hash used to check for not encoded csvs, function from Sound_Cache.cs (FNV1A64)
        /// </summary>
        /// <param name="bytes">Bytes to hash.</param>
        /// <returns></returns>
        private ulong CalculateHash(byte[] bytes)
        {
            const ulong fnv64Offset = 0xcbf29ce484222325;
            const ulong fnv64Prime = 0x100000001b3;
            ulong hash = fnv64Offset;

            foreach (var b in bytes)
            {
                hash = hash ^ b;
                hash = (hash * fnv64Prime) % 0xFFFFFFFFFFFFFFFF;
            }
            return hash;
        }

        /// <summary>
        /// Wrotes encoded strings/csv hash.
        /// </summary>
        /// <param name="type">"encoded" or "csv"</param>
        private void WriteHash(string type) // 
        {
            var toHash = "";
            var outputPath = "";

            if (type == "encoded")
            {
                var stringsDir = activeMod.ProjectDirectory + "\\strings";
                if (!Directory.Exists(stringsDir))
                    return;

                outputPath = stringsDir;

                if (comboBoxLanguagesMode.SelectedIndex == 1)
                    foreach (var lang in languagesStrings)
                        foreach (var str in lang.strings)
                            foreach (var column in str)
                                toHash += column;

                else
                    foreach (DataGridViewRow row in dataGridViewStrings.Rows)
                        foreach (DataGridViewCell cell in row.Cells)
                            if (cell.Value != null)
                                toHash += cell.Value.ToString();
            }

            if (type == "csv")
            {
                var csvDir = activeMod.ProjectDirectory + "\\strings\\CSV";
                if (!Directory.Exists(csvDir))
                    return;

                string[] fileNames = Directory.GetFiles(csvDir, "*.csv*", SearchOption.AllDirectories).Select(x => Path.GetFullPath(x)).ToArray();
                if (fileNames.Length == 0)
                    return;

                outputPath = csvDir;

                var cells = new List<string>();

                foreach (var file in fileNames)
                {
                    var content = File.ReadAllLines(file);
                    //var splittedContent = content.Split('|');

                    foreach (var line in content)
                    {
                        if (line.Contains(";"))
                            continue;
                        var splitted = line.Split('|');

                        foreach(var cell in splitted)
                        {
                            if (cell == "")
                                continue;

                            cells.Add(cell);
                        }
                    }
                }

                foreach (var cell in cells)
                    toHash += cell;
            }

            var hash = CalculateHash(Encoding.ASCII.GetBytes(toHash));

            using (var bw = new BinaryWriter(File.OpenWrite(outputPath + "\\hash")))
            {
                bw.Write(hash);
            }
        }

        /// <summary>
        /// Compares hashes of encoded strings and CSV.
        /// </summary>
        /// <returns>Comparison result.</returns>
        public bool AreHashesDifferent()
        {
            if (activeMod == null)
                return false;

            var stringsHashPath = activeMod.ProjectDirectory + "\\strings\\hash";
            if (!File.Exists(stringsHashPath))
                return false;

            byte[] hash;
            using (var br = new BinaryReader(File.OpenRead(stringsHashPath)))
            {
                hash = br.ReadBytes(32);
            }

            ulong hashStringsBytesSum = 0;

            foreach (var b in hash)
                hashStringsBytesSum += b;

            var csvHashPath = activeMod.ProjectDirectory + "\\strings\\CSV\\hash";
            if (!File.Exists(csvHashPath))
                return false;

            using (var br = new BinaryReader(File.OpenRead(csvHashPath)))
            {
                hash = br.ReadBytes(32);
            }

            ulong hashCsvBytesSum = 0;

            foreach (var b in hash)
                hashCsvBytesSum += b;

            if (hashStringsBytesSum == hashCsvBytesSum)
                return true;

            return false;
        }

        /// <summary>
        /// Fires up when languages mode is selected. Populates strings for each language.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControlLanguages_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == null)
                return;

            dataGridViewStrings.EndEdit();

            foreach (var language in languagesStrings)
                if (language.language == languageTabSelected)
                {
                    language.strings.Clear();

                    foreach (DataGridViewRow row in dataGridViewStrings.Rows)
                    {
                        if (row.Cells[0].Value != null)
                        {
                            language.strings.Add(new List<string>() { row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString() });
                        }
                    }
                    break;
                }

            foreach (var language in languagesStrings)
            {
                if (language.language == e.TabPage.Text)
                {
                    dataTableGridViewSource.Clear();

                    foreach (var str in language.strings)
                        dataTableGridViewSource.Rows.Add(str[0], str[1], str[2], str[3]);
                    break;
                }
            }

            if (e.TabPage != null)
                languageTabSelected = e.TabPage.Text;

            HashStringKeys();
            UpdateModID();
        }
    }

    /// <summary>
    /// Encapsulates a language with its strings.
    /// </summary>
    class LanguageStringsCollection
    {
        public string language;
        public List<List<string>> strings;

        public LanguageStringsCollection(string language, List<List<string>> strings)
        {
            this.language = language;
            this.strings = strings;
        }
    }
}
