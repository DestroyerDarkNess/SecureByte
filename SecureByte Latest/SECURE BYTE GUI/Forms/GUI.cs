using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using dnlib.DotNet;
using dnlib.Load;
using EmbedLibrary.Core.Library;
using Guna.UI2.WinForms;
using ICore;
using Inx.Ui;
using Microsoft.Win32;
using Protections.RefProxy;
using SECURE_BYTE_GUI.Global_for_Obfuscation;

namespace SECURE_BYTE_GUI
{
    public partial class GUI : Form
    {
        public GUI()
        {
            InitializeComponent();
            //if (Global_Checker.globalChecker.ints[0] != 1 || Global_Checker.globalChecker.ints[1] != 1 || Global_Checker.globalChecker.ints[2] != 1)
            //    authguard.authHelpers.SuspendProcess(Process.GetCurrentProcess().Id);
        }
        private void exitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(-1);
        }
        private void minButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void slidingPanel_MouseDown_1(object sender, MouseEventArgs e)
        {
            Movement.Move(this, e, gGlobals.transparency);
        }
        private void slideIT_Click(object sender, EventArgs e)
        {
            if (slidingPanel.Width == 300)
            {
                slidingPanel.Width = 56;
                guna2TabControl1.Location = new Point(51, -34);
                Size = new Size(556, 450);
                exitButton.Location = new Point(16, 105);
                minButton.Location = new Point(16, 75);
                guiSettings.Location = new Point(16, 45);
            }
            else
            {
                slidingPanel.Width = 300;
                guna2TabControl1.Location = new Point(295, -34);
                Size = new Size(800, 450);
                exitButton.Location = new Point(261, 15);
                minButton.Location = new Point(231, 15);
                guiSettings.Location = new Point(201, 15);
                logButton.Location = new Point(171, 15);
            }
        }
        private void GUI_Load(object sender, EventArgs e)
        {
            Opacity = 0.98;
            guna2ShadowForm1.SetShadowForm(this);
            x86runtimeName.Text = ICore.Safe.GenerateRandomLetters(5);
            x64runtimeName.Text = ICore.Safe.GenerateRandomLetters(5);
            oGlobals.x86RT = x86runtimeName.Text;
            oGlobals.x64RT = x64runtimeName.Text;
            saveProtections.Checked = true;
            guiTransparency.Checked = true;
            //Program.Auth.check();
        }      
        private void gotoMain_Click(object sender, EventArgs e)
        {
            InxUI.changeButtons((Guna2Button)sender, panel1); guna2VSeparator1.Location = new Point(0, 3); guna2TabControl1.SelectTab(mainPage);
        }
        private void gotoProtections_Click(object sender, EventArgs e)
        {
            InxUI.changeButtons((Guna2Button)sender, panel1); guna2VSeparator1.Location = new Point(0, 51); guna2TabControl1.SelectTab(protectionsPage);
            guna2TabControl1.SelectedTab.AutoScrollPosition = new Point(0, 10);
            guna2TabControl1.SelectedTab.AutoScrollMargin = new Size(0, 10);
            guna2TabControl1.SelectedTab.AutoScroll = true;
            guna2TabControl1.SelectedTab.VerticalScroll.Value = 0;
        }
        private void gotoCodeEnc_Click(object sender, EventArgs e)
        {
            InxUI.changeButtons((Guna2Button)sender, panel1); guna2VSeparator1.Location = new Point(0, 99); guna2TabControl1.SelectTab(codeencPage);
        }
        private void gotoJIT_Click(object sender, EventArgs e)
        {
            InxUI.changeButtons((Guna2Button)sender, panel1); guna2VSeparator1.Location = new Point(0, 147); guna2TabControl1.SelectTab(jitPage);
        }
        private void gotoDynamic_Click(object sender, EventArgs e)
        {
            InxUI.changeButtons((Guna2Button)sender, panel1); guna2VSeparator1.Location = new Point(0, 195); guna2TabControl1.SelectTab(dynPage);
        }
        private void gotoRenamer_Click(object sender, EventArgs e)
        {
            InxUI.changeButtons((Guna2Button)sender, panel1); guna2VSeparator1.Location = new Point(0, 243); guna2TabControl1.SelectTab(renamerPage);
        }
        private void logButton_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectTab(logPage);
        }
        #region Dyn Settings    
        public static int test = 0;
        private bool SearchRecursive(IEnumerable nodes, string searchFor)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.ToUpper().Contains(searchFor))
                {
                    treeView1.SelectedNode = node;
                    node.BackColor = Color.Black;
                }
                methodsearchTextbox.Focus();
                if (SearchRecursive(node.Nodes, searchFor))
                    return true;
            }
            return false;
        }
        private void Import()
        {
            this.treeView1.Nodes.Clear();
            foreach (TypeDef typeDef in oGlobals.ctx.Module.GetTypes())
            {
                VMUtils.Utils.hashSet.Add(typeDef.Namespace);
            }
            VMUtils.Utils.hashSet.Distinct<string>();
            foreach (TypeDef typeDef2 in oGlobals.ctx.Module.Types)
            {
                TreeNode treeNode = new TreeNode(typeDef2.Name, 0, 0)
                {
                    Tag = 1
                };
                foreach (MethodDef methodDef in typeDef2.Methods)
                {
                    if (methodDef != oGlobals.ctx.Module.GlobalType.FindOrCreateStaticConstructor())
                    {
                        TreeNode treeNode2 = new TreeNode(methodDef.FullName + " MDToken : " + methodDef.MDToken.ToString());
                        if (methodDef.IsPublic && methodDef.IsConstructor)
                        {

                            treeNode2.ImageIndex = 2;
                            treeNode2.SelectedImageIndex = 2;
                        }
                        else if (methodDef.IsPrivate && methodDef.IsConstructor)
                        {
                            treeNode2.ImageIndex = 3;
                            treeNode2.SelectedImageIndex = 3;
                        }
                        else if (methodDef.IsAssembly && methodDef.IsConstructor)
                        {
                            treeNode2.ImageIndex = 4;
                            treeNode2.SelectedImageIndex = 4;
                        }
                        else if (methodDef.IsFamily && methodDef.IsConstructor)
                        {
                            treeNode2.ImageIndex = 5;
                            treeNode2.SelectedImageIndex = 5;
                        }
                        else if (methodDef.IsFamilyOrAssembly && methodDef.IsConstructor)
                        {
                            treeNode2.ImageIndex = 6;
                            treeNode2.SelectedImageIndex = 6;
                        }
                        else if (methodDef.IsPublic)
                        {
                            treeNode2.ImageIndex = 2;
                            treeNode2.SelectedImageIndex = 2;
                        }
                        else if (methodDef.IsPrivate)
                        {
                            treeNode2.ImageIndex = 3;
                            treeNode2.SelectedImageIndex = 3;
                        }
                        else if (methodDef.IsAssembly)
                        {
                            treeNode2.ImageIndex = 4;
                            treeNode2.SelectedImageIndex = 4;
                        }
                        else if (methodDef.IsFamily)
                        {
                            treeNode2.ImageIndex = 5;
                            treeNode2.SelectedImageIndex = 5;
                        }
                        else if (methodDef.IsFamilyOrAssembly)
                        {
                            treeNode2.ImageIndex = 6;
                            treeNode2.SelectedImageIndex = 6;
                        }
                        treeNode2.Tag = 2;
                        treeNode.Nodes.Add(treeNode2);
                        VMUtils.Utils.tempMethodsList.Add(methodDef.FullName);
                    }
                }
                this.treeView1.Nodes.Add(treeNode);
            }
            TreeNode treeNode3 = null;
            foreach (string text in VMUtils.Utils.hashSet)
            {
                if (text != string.Empty)
                {
                    treeNode3 = new TreeNode(text, 0, 0)
                    {
                        Tag = 0
                    };
                    this.treeView1.Nodes.Add(treeNode3);
                    foreach (TypeDef typeDef3 in oGlobals.ctx.Module.Types)
                    {
                        if (treeNode3.Text == typeDef3.Namespace && typeDef3.Namespace != string.Empty && !typeDef3.IsValueType && !typeDef3.IsInterface)
                        {
                            string text2 = (typeDef3.Name.Contains("`") ? typeDef3.Name.Substring(0, typeDef3.Name.IndexOf('`')) : typeDef3.Name.Replace("`", string.Empty));
                            TreeNode treeNode4 = new TreeNode(text2, 0, 0)
                            {
                                Tag = 1
                            };
                            foreach (MethodDef methodDef2 in typeDef3.Methods)
                            {
                                if (methodDef2 != oGlobals.ctx.Module.GlobalType.FindOrCreateStaticConstructor())
                                {
                                    TreeNode treeNode5 = new TreeNode(methodDef2.FullName);
                                    if (methodDef2.IsPublic && methodDef2.IsConstructor)
                                    {
                                        treeNode5.ImageIndex = 2;
                                        treeNode5.SelectedImageIndex = 2;
                                    }
                                    else if (methodDef2.IsPrivate && methodDef2.IsConstructor)
                                    {
                                        treeNode5.ImageIndex = 3;
                                        treeNode5.SelectedImageIndex = 3;
                                    }
                                    else if (methodDef2.IsAssembly && methodDef2.IsConstructor)
                                    {
                                        treeNode5.ImageIndex = 4;
                                        treeNode5.SelectedImageIndex = 4;
                                    }
                                    else if (methodDef2.IsFamily && methodDef2.IsConstructor)
                                    {
                                        treeNode5.ImageIndex = 5;
                                        treeNode5.SelectedImageIndex = 5;
                                    }
                                    else if (methodDef2.IsFamilyOrAssembly && methodDef2.IsConstructor)
                                    {
                                        treeNode5.ImageIndex = 6;
                                        treeNode5.SelectedImageIndex = 6;
                                    }
                                    else if (methodDef2.IsPublic)
                                    {
                                        treeNode5.ImageIndex = 2;
                                        treeNode5.SelectedImageIndex = 2;
                                    }
                                    else if (methodDef2.IsPrivate)
                                    {
                                        treeNode5.ImageIndex = 3;
                                        treeNode5.SelectedImageIndex = 3;
                                    }
                                    else if (methodDef2.IsAssembly)
                                    {
                                        treeNode5.ImageIndex = 4;
                                        treeNode5.SelectedImageIndex = 4;
                                    }
                                    else if (methodDef2.IsFamily)
                                    {
                                        treeNode5.ImageIndex = 5;
                                        treeNode5.SelectedImageIndex = 5;
                                    }
                                    else if (methodDef2.IsFamilyOrAssembly)
                                    {
                                        treeNode5.ImageIndex = 6;
                                        treeNode5.SelectedImageIndex = 6;
                                    }
                                    treeNode5.Tag = 2;
                                    //treeNode4.Nodes.Add(treeNode5);
                                    VMUtils.Utils.tempMethodsList.Add(methodDef2.FullName);
                                }
                            }
                            treeNode3.Nodes.Add(treeNode4);
                        }
                    }
                }
            }
            try
            {
                int i = 0;
                while (i < this.treeView1.Nodes.Count)
                {
                    TreeNode treeNode6 = this.treeView1.Nodes[i];
                    if (treeNode6.Nodes.Count == 0)
                    {
                        this.treeView1.Nodes.Remove(treeNode6);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            catch
            {
            }
            this.treeView1.Sort();
        }
        private void dynselectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (dynselectAll.Checked)
            {
                VMUtils.Utils.ProtectAll = true;
                treeView1.Enabled = false;
            }
            else
            {
                VMUtils.Utils.ProtectAll = false;
                treeView1.Enabled = true;
            }
        }
        private void treeView1_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
            test = 1;
        }
        private void treeView1_DoubleClick_1(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.ForeColor == Color.FromArgb(171, 171, 171))
            {
                treeView1.SelectedNode.ForeColor = Color.FromArgb(240, 80, 80);
                VMUtils.Utils.SelectedMethods = VMUtils.Utils.SelectedMethods.Replace(treeView1.SelectedNode.Text, "");
            }
            else
            {
                if (test == 1)
                {
                    test = 0;
                    treeView1.SelectedNode.ForeColor = Color.FromArgb(171, 171, 171);
                    string meth = treeView1.SelectedNode.Text.ToString();
                    VMUtils.Utils.SelectedMethods = VMUtils.Utils.SelectedMethods + meth;
                }
            }
        }
        private void treeView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (e.KeyChar == (char)Keys.Enter && tn == null == false)
            {
                if (treeView1.SelectedNode.ForeColor == Color.FromArgb(171, 171, 171))
                {
                    treeView1.SelectedNode.ForeColor = Color.FromArgb(240, 80, 80);
                    VMUtils.Utils.SelectedMethods = VMUtils.Utils.SelectedMethods.Replace(treeView1.SelectedNode.Text, "");
                }
                else
                {
                    if (test == 1)
                    {
                        test = 0;
                        treeView1.SelectedNode.ForeColor = Color.FromArgb(171, 171, 171);
                        string meth = treeView1.SelectedNode.Text.ToString();
                        VMUtils.Utils.SelectedMethods = VMUtils.Utils.SelectedMethods + meth;
                    }
                }
            }
        }
        private void searchMethod_Click(object sender, EventArgs e)
        {
            treeView1.Focus();
            try
            {
                treeView1.SelectedNode.BackColor = Color.FromArgb(22, 22, 27);
                var searchFor = methodsearchTextbox.Text.Trim().ToUpper();
                if (searchFor != "")
                {
                    if (treeView1.Nodes.Count > 0)
                    {
                        if (SearchRecursive(treeView1.Nodes, searchFor))
                        {
                            treeView1.SelectedNode.Expand();
                            treeView1.Focus();
                        }
                    }
                }
            }
            catch { }
        }
        private void SetNodeBackColorRecursive(TreeNode parentNode)
        {
            foreach (TreeNode childNode in parentNode.Nodes)
            {
                childNode.BackColor = Color.FromArgb(22, 22, 27);
                SetNodeBackColorRecursive(childNode);
            }
        }
        private void treeView1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (TreeNode node in treeView1.Nodes)
                {
                    node.BackColor = Color.FromArgb(22, 22, 27);
                    SetNodeBackColorRecursive(node);
                }
            }
            catch { }
        }
        #endregion
        #region Embed
        public static List<string> _librarys = new List<string>();
        private void Clear()
        {
            Invoke((MethodInvoker)delegate
            {
                _librarys.Clear();
                dllsList.Items.Clear();
            });
        }
        private void Add(string path)
        {
            Invoke((MethodInvoker)delegate
            {
                //_librarys.Add(path);
                dllsList.Items.Add(path);
            });
        }
        private void GetRefs(string file)
        {
            using (AssemblyLoader assembly = new AssemblyLoader(file))
            {
                string dir = Path.GetDirectoryName(file);
                var refs = assembly.ModuleDefMD.GetAssemblyRefs();
                foreach (AssemblyRef asmRef in refs)
                {
                    if (!string.IsNullOrEmpty(dir))
                    {
                        string path = $"{Path.Combine(dir, asmRef.Name)}.dll";
                        if (File.Exists(path) && !dllsList.Items.Contains(path))
                        {
                            Add(path);
                            GetRefs(path);
                        }
                    }
                }
                assembly.Dispose();
            }
        }
        private string Address(string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!dir.EndsWith("\\"))
                dir += "\\";
            string file = Path.GetFileNameWithoutExtension(path) + Path.GetExtension(path);
            if (File.Exists(file))
                File.Delete(file);
            return file;
        }
        private void Merge(string path, bool writePdb)
        {
            //Clear();
            GetRefs(assemblyPath.Text);
            if (_librarys.Count > 0)
            {
                using (AssemblyLoader assembly = new AssemblyLoader(assemblyPath.Text))
                {
                    AssemblyContext context = assembly.GetAssemblyContext();
                    Embed.Execute(context, _librarys.ToArray());
                    assembly.Write(path.Replace(".exe", "-embed.exe"), writePdb);
                    context = null;
                    assembly.Dispose();
                }
            }
        }
        #endregion
        #region Load assembly
        private void addAssembly_Click(object sender, EventArgs e)
        {
            OpenFileDialog x = new OpenFileDialog
            {
                Title = "Load Assembly, dll or bat",
                Filter = ".NET Assembly (*.exe)|*.exe|(*.dll)|*.dll|(*.bat)|*.bat",
                Multiselect = false
            };
            if (x.ShowDialog() == DialogResult.OK)
            {
                string text = x.FileName;
                int num = text.LastIndexOf(".");
                if (num != -1)
                {
                    string text2 = text.Substring(num);
                    text2 = text2.ToLower();
                    if (text2 == ".bat")
                    {
                        assemblyPath.Text = text;
                        savetoPath.Text = text.Substring(0, text.LastIndexOf(".bat")) + "-obfuscated.bat";
                    }
                    else if (text2 == ".exe")
                    {
                        assemblyPath.Text = text;
                        savetoPath.Text = Path.Combine(Path.GetDirectoryName(assemblyPath.Text) + @"\Secured");
                        oGlobals.ctx = new Context(assemblyPath.Text)
                        {
                            FileName = Path.GetFileName(assemblyPath.Text),
                            DirPath = savetoPath.Text
                        };
                        oGlobals.ctx.OutPutPath = savetoPath.Text + @"\" + oGlobals.ctx.FileName;
                        oGlobals.browseDir = oGlobals.ctx.DirPath;
                        oGlobals.browseDir2 = Path.GetDirectoryName(assemblyPath.Text);
                        Import();
                        if (assemblyPath.Text == "")
                        {
                            return;
                        }
                        Clear();
                        GetRefs(assemblyPath.Text);
                    }
                    else if (text2 == ".dll")
                    {
                        mergingCbox.Checked = false;
                        assemblyPath.Text = text;
                        savetoPath.Text = Path.Combine(Path.GetDirectoryName(assemblyPath.Text) + @"\Secured");
                        oGlobals.ctx = new Context(assemblyPath.Text)
                        {
                            FileName = Path.GetFileName(assemblyPath.Text),
                            DirPath = savetoPath.Text
                        };
                        oGlobals.ctx.OutPutPath = savetoPath.Text + @"\" + oGlobals.ctx.FileName;
                        oGlobals.browseDir = oGlobals.ctx.DirPath;
                        oGlobals.browseDir2 = Path.GetDirectoryName(assemblyPath.Text);
                        Import();
                    }
                }
            }
        }
        private void assemblyPath_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        private void assemblyPath_DragEnter(object sender, DragEventArgs e)
        {
            Array array = (Array)e.Data.GetData(DataFormats.FileDrop);
            if (array != null)
            {
                string text = array.GetValue(0).ToString();
                int num = text.LastIndexOf(".");
                if (num != -1)
                {
                    string text2 = text.Substring(num);
                    text2 = text2.ToLower();
                    if (text2 == ".bat")
                    {
                        assemblyPath.Text = text;
                        savetoPath.Text = text.Substring(0, text.LastIndexOf(".bat")) + "-obfuscated.bat";
                    }
                    else if (text2 == ".exe")
                    {
                        assemblyPath.Text = text;
                        savetoPath.Text = Path.Combine(Path.GetDirectoryName(assemblyPath.Text) + @"\Secured");
                        oGlobals.ctx = new Context(assemblyPath.Text)
                        {
                            FileName = Path.GetFileName(assemblyPath.Text),
                            DirPath = savetoPath.Text
                        };
                        oGlobals.ctx.OutPutPath = savetoPath.Text + @"\" + oGlobals.ctx.FileName;
                        oGlobals.browseDir = oGlobals.ctx.DirPath;
                        oGlobals.browseDir2 = Path.GetDirectoryName(assemblyPath.Text);
                        Import();
                        if (assemblyPath.Text == "")
                        {
                            return;
                        }
                        Clear();
                        GetRefs(assemblyPath.Text);
                    }
                    else if (text2 == ".dll")
                    {
                        mergingCbox.Checked = false;
                        assemblyPath.Text = text;
                        savetoPath.Text = Path.Combine(Path.GetDirectoryName(assemblyPath.Text) + @"\Secured");
                        oGlobals.ctx = new Context(assemblyPath.Text)
                        {
                            FileName = Path.GetFileName(assemblyPath.Text),
                            DirPath = savetoPath.Text
                        };
                        oGlobals.ctx.OutPutPath = savetoPath.Text + @"\" + oGlobals.ctx.FileName;
                        oGlobals.browseDir = oGlobals.ctx.DirPath;
                        oGlobals.browseDir2 = Path.GetDirectoryName(assemblyPath.Text);
                        Import();
                    }
                }
            }
        }
        private void selectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog x = new FolderBrowserDialog();
            x.Description = "Select folder";
            if (x.ShowDialog() == DialogResult.OK)
                savetoPath.Text = x.SelectedPath;
        }
        #endregion
        #region Save & Load Protections
        private void SaveProt()
        {
            StreamWriter streamWriter = new StreamWriter(Directory.GetCurrentDirectory() + @"\Saved.txt");
            streamWriter.WriteLine("Saved protection:\n———————————");
            if (excludeCbox.Checked)
            {
                streamWriter.WriteLine("Exclude = true");
                File.WriteAllText(Directory.GetCurrentDirectory() + @"\Exclude.txt", excludeText.Text);
            }
            if (customrnCbox.Checked)
            {
                streamWriter.WriteLine("Custom Renaming = true");
                File.WriteAllText(Directory.GetCurrentDirectory() + @"\Custom.txt", customText.Text);
            }
            if (dynamiccctorCbox.Checked)
                streamWriter.WriteLine("Dynamic cctor = true");
            if (renamerCbox.Checked)
                streamWriter.WriteLine("Old Renamer = true");
            if (rnTypes.Checked)
                streamWriter.WriteLine("Types = true");
            if (rnProperties.Checked)
                streamWriter.WriteLine("Properties = true");
            if (rnFields.Checked)
                streamWriter.WriteLine("Fields = true");
            if (rnEvents.Checked)
                streamWriter.WriteLine("Events = true");
            if (rnMethods.Checked)
                streamWriter.WriteLine("Methods = true");
            if (rnParameters.Checked)
                streamWriter.WriteLine("Parameters = true");
            if (optimizecodeCbox.Checked)
                streamWriter.WriteLine("Code Optimization = true");
            //if (codehiderCbox.Checked)
            //    streamWriter.WriteLine("Code Hider = true");
            if (preserveallCbox.Checked)
                streamWriter.WriteLine("Preserve All = true");
            if (integrityCbox.Checked)
                streamWriter.WriteLine("Integrity = true");
            if (invalidmdCbox.Checked)
                streamWriter.WriteLine("Invalid Metadata = true");
            if (junkCbox.Checked)
                streamWriter.WriteLine("Junk = true");
            if (fakesignatureCbox.Checked)
                streamWriter.WriteLine("Fake Attributes = true");
            if (jitCbox.Checked)
                streamWriter.WriteLine("JIT = true");
            if (encoderesourcesCbox.Checked)
                streamWriter.WriteLine("Resources = true");
            if (antiildasmCbox.Checked)
                streamWriter.WriteLine("Anti ILDasm = true");
            if (referenceproxyCbox.Checked)
                streamWriter.WriteLine("Ref Proxy = true");
            if (encodestringsCbox.Checked)
                streamWriter.WriteLine("Strings Encoding = true");
            if (anticrackCbox.Checked)
                streamWriter.WriteLine("Main Anti Crack = true");
            if (normalacCbox.Checked)
                streamWriter.WriteLine("Anti Crack = true");
            if (dsacCbox.Checked)
            {
                streamWriter.WriteLine("Anti Crack Data Sending = true");
                if (webhookText.Text != "")
                {
                    streamWriter.WriteLine("WebHook = true");
                    File.WriteAllText(Directory.GetCurrentDirectory() + @"\Webhook.txt", webhookText.Text);
                }
                if (apiText.Text != "")
                {
                    streamWriter.WriteLine("API = true");
                    File.WriteAllText(Directory.GetCurrentDirectory() + @"\API.txt", apiText.Text);
                }
            }
            if (banCbox.Checked)
                streamWriter.WriteLine("Ban Cracker = true");
            if (capturessCbox.Checked)
                streamWriter.WriteLine("Capture Screen = true");
            if (silentmsgCbox.Checked)
            {
                streamWriter.WriteLine("Silent Message = true");
                File.WriteAllText(Directory.GetCurrentDirectory() + @"\CustomMessage.txt", msgText.Text);
            }
            if (normalmsgCbox.Checked)
            {
                streamWriter.WriteLine("Normal MessageBox = true");
                streamWriter.WriteLine("Custom Message = true");
                File.WriteAllText(Directory.GetCurrentDirectory() + @"\CustomMessage.txt", msgText.Text);
            }
            if (codeencCbox.Checked)
                streamWriter.WriteLine("Code Encryption = true");
            if (antidumpCbox.Checked)
                streamWriter.WriteLine("Anti Dump = true");
            if (antivmCbox.Checked)
                streamWriter.WriteLine("Anti VM = true");
            if (antidebugCbox.Checked)
                streamWriter.WriteLine("Anti Debug = true");
            if (encodeintsCbox.Checked)
                streamWriter.WriteLine("Mutation = true");
            if (controlflowCbox.Checked)
                streamWriter.WriteLine("Control Flow = true");
            if (localtofieldCbox.Checked)
                streamWriter.WriteLine("Local To Field = true");
            if (localtofieldLength.Value == 0)
                streamWriter.WriteLine("Local To Field Preset = Off");
            if (localtofieldLength.Value == 1)
                streamWriter.WriteLine("Local To Field Preset = Field");
            if (localtofieldLength.Value == 2)
                streamWriter.WriteLine("Local To Field Preset = Method");
            streamWriter.Dispose();
        }
        #endregion
        #region Obfuscation
        static void MoveAndRenameFile(string sourceFilePath, string destinationDirectory, string newFileName)
        {
            try
            {
                string fileName = Path.GetFileName(sourceFilePath);
                string newFilePath = Path.Combine(destinationDirectory, newFileName);
                File.Move(sourceFilePath, newFilePath);
            }
            catch { }
        }
        private void protectButton_Click(object sender, EventArgs e)
        {
            //if (Global_Checker.globalChecker.ints[0] != 1 || Global_Checker.globalChecker.ints[1] != 1 || Global_Checker.globalChecker.ints[2] != 1)
            //    authguard.authHelpers.SuspendProcess(Process.GetCurrentProcess().Id);
            guna2TabControl1.SelectTab(logPage);
            Logger.Logger.Starting(logBox);
            var procName = Path.GetFileNameWithoutExtension(assemblyPath.Text);
            var proc = Process.GetProcessesByName(procName);
            if (proc.Length == 1)
            {
                Logger.Logger.AppendToLog("[ ! ] Error : Please close the process before obfuscation !", logBox);
                return;
            }
            if (assemblyPath.Text == "")
            {
                Logger.Logger.AppendToLog("[ ! ] Error : Add file first !", logBox);
                return;
            }
            try
            {
                if (assemblyPath.Text.EndsWith(".bat"))
                {
                    Logger.Logger.AppendToLog("[ ! ] Warning : you are trying to obfuscate .bat file !\nSo i will auto enable bat options for you ;)", logBox);
                    Logger.Logger.AppendToLog("", logBox);
                    BatObfuscation.Bat.Encrypt(File.ReadAllText(assemblyPath.Text), savetoPath.Text);
                    Logger.Logger.Finish(logBox);
                    return;
                }
            }
            catch (Exception ex)
            {
                logBox.Text = ex.ToString();
            }
            try
            {
                if (File.Exists(assemblyPath.Text.Replace(".exe", "-embed.exe")))
                {
                    MoveAndRenameFile(assemblyPath.Text.Replace(".exe", "-embed.exe"), Path.GetTempPath(), Utils.MethodsRenamig());
                }
                if (File.Exists(oGlobals.ctx.OutPutPath.Replace(".dll", "-JIT.dll")))
                {
                    MoveAndRenameFile(oGlobals.ctx.OutPutPath.Replace(".dll", "-JIT.dll"), Path.GetTempPath() , Utils.MethodsRenamig());
                }
                if (File.Exists(oGlobals.ctx.OutPutPath.Replace(".exe", "-JIT.exe")))
                {
                    MoveAndRenameFile(oGlobals.ctx.OutPutPath.Replace(".exe", "-JIT.exe"), Path.GetTempPath(), Utils.MethodsRenamig());
                }
                if (File.Exists(oGlobals.ctx.OutPutPath.Replace(".dll", "-Dyn.dll")))
                {
                    MoveAndRenameFile(oGlobals.ctx.OutPutPath.Replace(".dll", "-Dyn.dll"), Path.GetTempPath(), Utils.MethodsRenamig());
                }
                if (File.Exists(oGlobals.ctx.OutPutPath.Replace(".exe", "-Dyn.exe")))
                {
                    MoveAndRenameFile(oGlobals.ctx.OutPutPath.Replace(".exe", "-Dyn.exe"), Path.GetTempPath(), Utils.MethodsRenamig());
                }
            }
            catch { }
            if (oGlobals.ctx.Path == null)
            {
                oGlobals.ctx = new Context(assemblyPath.Text)
                {
                    FileName = Path.GetFileName(assemblyPath.Text),
                    DirPath = savetoPath.Text
                };
                oGlobals.ctx.OutPutPath = savetoPath.Text + @"\" + oGlobals.ctx.FileName;
                oGlobals.browseDir = oGlobals.ctx.DirPath;
                oGlobals.browseDir2 = Path.GetDirectoryName(assemblyPath.Text);
            }      
            Task task = Task.Run(() =>
            {                    
                if (assemblyPath.Text.EndsWith(".dll"))
                {
                    Logger.Logger.AppendToLog("[ ! ] Warning : You are trying to obfuscate .dll file !\n So these protections will be disabled.", logBox);
                    Logger.Logger.AppendToLog(">> Anti Dump, Anti Virtual Machine, Anti Crack, Anti Http Debug, Anti Debug, Resources encoding\n And embed dlls.", logBox);
                    Logger.Logger.AppendToLog("", logBox);
                    antidumpCbox.Checked = false;
                    antivmCbox.Checked = false;
                    anticrackCbox.Checked = false;
                    antidebugCbox.Checked = false;
                    encoderesourcesCbox.Checked = false;
                    mergingCbox.Checked = false;
                }
                if (mergingCbox.Checked)
                {
                    Logger.Logger.AppendToLog("[ ! ] Embed references to resources ....", logBox);
                    if (dllsList.CheckedItems.Count == 0)
                    {
                        Logger.Logger.AppendToLog("[ ! ] There are no dlls to embed, embedding has been cancelled !", logBox);
                    }
                    else
                    {
                        foreach (string item in dllsList.CheckedItems)
                        {
                            _librarys.Add(item);
                        }
                        Merge(assemblyPath.Text, true);
                        oGlobals.ctx = new Context(assemblyPath.Text.Replace(".exe", "-embed.exe"))
                        {
                            FileName = Path.GetFileName(assemblyPath.Text),
                            DirPath = savetoPath.Text
                        };
                        oGlobals.ctx.OutPutPath = savetoPath.Text + @"\" + oGlobals.ctx.FileName;
                        oGlobals.browseDir = oGlobals.ctx.DirPath;
                        oGlobals.browseDir2 = Path.GetDirectoryName(assemblyPath.Text);

                        Logger.Logger.AppendToLog("Embed Done !", logBox);
                    }
                }
                //try
                //{
                    #region Code Optimization
                    if (optimizecodeCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("[ ! ] Optimizing Code ....", logBox);
                        Codes.Optimize.OptimizeCode.CodeOptimize(oGlobals.ctx);
                        Codes.Optimize.OptimizeCode.ReduceMD(oGlobals.ctx);
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    #endregion
                    #region Renaming
                    if (renamerCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Applying Renamer ....", logBox);
                        if (rnTypes.Checked)
                            Protections.Renamer.Globals.Globals.types = true;
                        else
                            Protections.Renamer.Globals.Globals.types = false;

                        if (rnProperties.Checked)
                            Protections.Renamer.Globals.Globals.props = true;
                        else
                            Protections.Renamer.Globals.Globals.props = false;

                        if (rnFields.Checked)
                            Protections.Renamer.Globals.Globals.flds = true;
                        else
                            Protections.Renamer.Globals.Globals.flds = false;

                        if (rnMethods.Checked)
                            Protections.Renamer.Globals.Globals.methods = true;
                        else
                            Protections.Renamer.Globals.Globals.methods = false;

                        if (rnEvents.Checked)
                            Protections.Renamer.Globals.Globals.events = true;
                        else
                            Protections.Renamer.Globals.Globals.events = false;

                        if (rnParameters.Checked)
                            Protections.Renamer.Globals.Globals.parameters = true;
                        else
                            Protections.Renamer.Globals.Globals.parameters = false;

                        if (customrnCbox.Checked)
                        {
                            Protections.Renamer.RNG.customstr = customText.Text;
                            Protections.Renamer.Renamer.Execute(oGlobals.ctx, Protections.Renamer.Schemes.Custom,
                            Protections.Renamer.Globals.Globals.props, Protections.Renamer.Globals.Globals.flds, Protections.Renamer.Globals.Globals.events, Protections.Renamer.Globals.Globals.methods, Protections.Renamer.Globals.Globals.parameters, Protections.Renamer.Globals.Globals.types);
                        }
                        else
                        {
                            Protections.Renamer.Renamer.Execute(oGlobals.ctx, Protections.Renamer.Schemes.Safe,
                            Protections.Renamer.Globals.Globals.props, Protections.Renamer.Globals.Globals.flds, Protections.Renamer.Globals.Globals.events, Protections.Renamer.Globals.Globals.methods, Protections.Renamer.Globals.Globals.parameters, Protections.Renamer.Globals.Globals.types);
                        }

                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    #endregion
                    #region Junk
                    if (junkCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Adding Junk ....", logBox);
                        Protections.Junk.Junk.Execute(oGlobals.ctx, Convert.ToInt32(junkLength.Value));
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    #endregion
                    if (antiildasmCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Applying Anti ILDasm ....", logBox);
                        Protections.Software.AntiSuspend.Execute(oGlobals.ctx);
                        //Protections.Software.AntiILDasm.Execute(oGlobals.ctx);
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    if (encodeintsCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Applying Mutation ....", logBox);
                        for (int i = 0; i < 2; i++)
                        {
                            Protections.Mutation.MutationConfusion.ExecuteNormal(oGlobals.ctx);
                        }
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    if (localtofieldCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Converting Local To Field / Method ....", logBox);
                        if (localtofieldLength.Value == 1)
                            Protections.Ints.LocalToField.ToField(oGlobals.ctx);
                        if (localtofieldLength.Value == 2)
                            Protections.Ints.LocalToField.ToMethod(oGlobals.ctx);
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    if (controlflowCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Applying Control Flow", logBox);
                        Protections.NormalCFlow.ControlFlow.Execute(oGlobals.ctx);
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    if (referenceproxyCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Applying Reference Proxy", logBox);
                        Protections.RefProxy.FixedReferenceProxy.Execute(oGlobals.ctx);
                        Logger.Logger.AppendToLog("Done !"  , logBox);
                    }
                    if (encodestringsCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Securing Strings ....", logBox);
                        if (assemblyPath.Text.EndsWith(".dll".ToLower()))
                        {
                            Protections.Strings.stillWorkingOn2.Encode(oGlobals.ctx);
                            new Protections.Strings.replaceObfuscator(oGlobals.ctx.Module,
                                Protections.Strings.replaceObfuscator.Mode.Simple).Execute();
                        }
                        else
                        {
                            //new Protections.Strings.replaceObfuscator(oGlobals.ctx.Module,
                            //    Protections.Strings.replaceObfuscator.Mode.Simple).Execute();
                            Protections.newStrings.eConstants.Encode(oGlobals.ctx.Module);
                            //Protections.Strings.test.Encode(oGlobals.ctx);
                        }
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    if (integrityCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Injecting Integrity Check ....", logBox);
                        Protections.Software.IntegrityCheck.Execute(oGlobals.ctx);
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    if (antivmCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Injecting Anti VM ....", logBox);
                        Protections.Software.InjectAntiVM.Inject(oGlobals.ctx);
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    if (antidebugCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Injecting Anti Debug ....", logBox);
                        Protections.Software.AntiDebug.Execute(oGlobals.ctx);
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    if (antidumpCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Injecting Anti Dump ....", logBox);
                        Protections.Software.AntiDump.Execute(oGlobals.ctx);
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    if (anticrackCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Injecting Anti Crack ....", logBox);
                        Protections.Software.Global.Global.api = apiText.Text;
                        Protections.Software.Global.Global.MSGC = msgText.Text;
                        if (excludeCbox.Checked && !string.IsNullOrEmpty(excludeText.Text))
                        {
                            Protections.Software.Global.Global.ExcludeList = excludeText.Text;
                        }
                        if (silentmsgCbox.Checked)
                        {
                            Protections.Software.Global.Global.MSG = "0";
                        }
                        if (normalmsgCbox.Checked)
                        {
                            Protections.Software.Global.Global.MSG = "1";
                        }
                        if (banCbox.Checked)
                        {
                            Protections.Software.Global.Global.Status = "1";
                        }
                        else
                        {
                            Protections.Software.Global.Global.Status = "0";
                        }
                        if (capturessCbox.Checked)
                        {
                            Protections.Software.Global.Global.rnd = string.Concat(ICore.Safe.GenerateRandomLetters(new Random().Next(2, 24)) , ".png");
                            Protections.Software.Global.Global.SIMG = "1";
                        }
                        else
                        {
                            Protections.Software.Global.Global.rnd = string.Concat(ICore.Safe.GenerateRandomLetters(new Random().Next(2, 24)), ".png");
                            Protections.Software.Global.Global.SIMG = "0";
                        }
                        //
                        if (normalacCbox.Checked)
                        {
                            Protections.Software.DetectCrackersNHook.Inject(oGlobals.ctx);
                        }
                        if (dsacCbox.Checked)
                        {
                            Protections.Software.Global.Global.webhookLink = webhookText.Text;
                            Protections.Software.Global.Global.ID = Protections.Software.Global.Global.webhookLink;
                            Protections.Software.DetectCrackersYHook.Inject(oGlobals.ctx);
                        }
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    if (fakesignatureCbox.Checked)
                    {
                        Protections.Software.AntiDe4dot.Execute(oGlobals.ctx);
                        Logger.Logger.AppendToLog("-> Injected Fake Attributes....", logBox);
                    }
                    if (encoderesourcesCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Encoding & Compressing Resources ....", logBox);
                        Protections.Software.ResourcesEncoder.Execute(oGlobals.ctx);
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    if (codeencCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Encrypting Code ....", logBox);
                        new ExAntiTamper.AntiTamperNormal().AntiTamper(oGlobals.ctx);
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    if (invalidmdCbox.Checked)
                    {
                        oGlobals.InvalidMD = true;
                        Logger.Logger.AppendToLog("-> Injected Invalid Metadata ....", logBox);
                    }
                    else
                    {
                        oGlobals.InvalidMD = false;
                    }
                    if (!dynamicmethodsCbox.Checked && !jitCbox.Checked && dynamiccctorCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("-> Convert cctor to dynamic ....", logBox);
                        new IlDyn.IL2Dynamic().Execute(oGlobals.ctx.Module);
                        Logger.Logger.AppendToLog("Done !", logBox);
                    }
                    oGlobals.ctx.SaveFile();
                    string DynSReplacer = string.Empty;
                    string JitPath = string.Empty;
                    if (dynamicmethodsCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("[ ! ] Executing Dynamic Method ....", logBox);
                        if (File.Exists(oGlobals.ctx.OutPutPath))
                        {
                            oGlobals.Virt = true;
                            oGlobals.FPath = oGlobals.ctx.OutPutPath;
                            if (assemblyPath.Text.EndsWith(".dll".ToLower()))
                            {
                                DynSReplacer = oGlobals.ctx.OutPutPath.Replace(".dll", "-Dyn.dll");
                                byte[] assemblyProtected = DynCore.Execute.Protect(File.ReadAllBytes(oGlobals.FPath));
                                File.WriteAllBytes(DynSReplacer, assemblyProtected);
                            }
                            else
                            {
                                DynSReplacer = oGlobals.ctx.OutPutPath.Replace(".exe", "-Dyn.exe");
                                byte[] assemblyProtected = DynCore.Execute.Protect(File.ReadAllBytes(oGlobals.FPath));
                                File.WriteAllBytes(DynSReplacer, assemblyProtected);
                            }
                            Logger.Logger.AppendToLog("Done !", logBox);
                        }
                    }
                    if (jitCbox.Checked)
                    {
                        Logger.Logger.AppendToLog("[ ! ] Applying JIT Protection ....", logBox);
                        if (File.Exists(oGlobals.ctx.OutPutPath))
                        {
                            string rtname = ICore.Utils.MethodsRenamig();
                            oGlobals.FPath = oGlobals.ctx.OutPutPath;
                            if (assemblyPath.Text.EndsWith(".dll".ToLower()))
                            {
                                JitPath = oGlobals.FPath.Replace(".dll", "-JIT.dll");
                                var jit = new JIT.Protection.Protection(ModuleDefMD.Load(oGlobals.ctx.OutPutPath));
                                var resultjit = jit.Protect();
                                File.WriteAllBytes(JitPath, resultjit);
                                if (oGlobals.dynCctor)
                                    Logger.Logger.AppendToLog("-> Convert cctor to dynamic ....", logBox);
                            }
                            else
                            {
                                JitPath = oGlobals.FPath.Replace(".exe", "-JIT.exe");
                                var jit = new JIT.Protection.Protection(ModuleDefMD.Load(oGlobals.ctx.OutPutPath));
                                var resultjit = jit.Protect();
                                File.WriteAllBytes(JitPath, resultjit);
                                if (oGlobals.dynCctor)
                                    Logger.Logger.AppendToLog("-> Convert cctor to dynamic ....", logBox);
                            }
                            Logger.Logger.AppendToLog("Done !", logBox);
                        }
                    }
                    if (saveProtections.Checked)
                        SaveProt();
                    clearRef();
                    Logger.Logger.Finish(logBox);
                    if (gGlobals.autoDir)
                        Process.Start(oGlobals.browseDir);
                    else
                    {
                        Global_for_links.globalLinks.type = "Dir";
                        customMessage.msg = "Obfuscated successfully, Press ok to go to file location .";
                        new customMessage().ShowDialog();
                    }
                    new Helpers.Mutations.MutationHelper().Dispose();
                //}
                //catch (Exception ex)
                //{
                //    this.BeginInvoke((Action)(() =>
                //    {
                //        logBox.Text = ex.ToString();
                //    }));
                   
                //}
                Thread.Sleep(1000);
            });
        }
        private void clearRef()
        {
            FixedReferenceProxy.ProxyMethods.Clear();
            Core.Protector.usedMethods.Clear();
        }
        #endregion
        private void normalacCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (normalacCbox.Checked)
            {
                dsacCbox.Checked = false;
                capturessCbox.Enabled = false;
            }
        }
        private void dsacCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (dsacCbox.Checked)
            {
                normalacCbox.Checked = false;
                capturessCbox.Enabled = true;
            }
        }
        private void integrityCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (integrityCbox.Checked)
            {
                dynamicmethodsCbox.Checked = false;
                jitCbox.Checked = false;
                oGlobals.Integrity = true;
            }
            else
            {
                oGlobals.Integrity = false;
            }
        }
        private void codeencCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (codeencCbox.Checked)
            {
                jitCbox.Checked = false;
                dynamicmethodsCbox.Checked = false;
                oGlobals.Atamper = true;
            }
            else
            {
                oGlobals.Atamper = false;
            }
        }
        private void jitCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (jitCbox.Checked)
            {
                oGlobals.Atamper = false;
                codeencCbox.Checked = false;
                dynamicmethodsCbox.Checked = false;
                integrityCbox.Checked = false;
            }
        }
        private void localtofieldLength_ValueChanged(object sender, EventArgs e)
        {
            switch (localtofieldLength.Value)
            {
                case 0:
                    localtofieldLabel.Text = "Off";
                    break;
                case 1:
                    localtofieldLabel.Text = "To field";
                    break;
                case 2:
                    localtofieldLabel.Text = "To field & method";
                    break;
            }
        }
        private void junkLength_ValueChanged(object sender, EventArgs e)
        {
            junkLabel.Text = junkLength.Value.ToString();
        }
        private void preserveallCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (preserveallCbox.Checked)
            {
                Context.preAll = true;
            }
            else
            {
                Context.preAll = false;
            }
        }
        private void acSettings_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectTab(acsPage);
        }
        private void dynamicmethodsCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (dynamicmethodsCbox.Checked)
            {
                oGlobals.Atamper = false;
                dynamiccctorCbox.Checked = false;
                integrityCbox.Checked = false;
                jitCbox.Checked = false;
                codeencCbox.Checked = false;
            }
        }
        private void unbanME_Click(object sender, EventArgs e)
        {
            banStatusTxt.Text = "You're not banned !";
            string keyPath32Bit = "SOFTWARE\\SBAC";
            RegistryKey cuu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
            RegistryKey key32Bit = cuu.OpenSubKey(keyPath32Bit, true);
            if (key32Bit != null)
            {
                var namesArray = key32Bit.GetValueNames();
                foreach (string valueName in namesArray)
                {
                    key32Bit.DeleteValue(valueName);
                    banStatusTxt.Text = "Unbanned !";
                }
                return;
            }
            string keyPath64Bit = "SOFTWARE\\SBAC";
            RegistryKey cu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            RegistryKey key64Bit = cu.OpenSubKey(keyPath64Bit, true);
            if (key64Bit != null)
            {
                var namesArray = key64Bit.GetValueNames();
                foreach (string valueName in namesArray)
                {
                    key64Bit.DeleteValue(valueName);
                    banStatusTxt.Text = "Unbanned !";
                }
                return;
            }
        }
        private void silentmsgCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (silentmsgCbox.Checked)
            {
                normalmsgCbox.Checked = false;
            }
        }
        private void normalmsgCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (normalmsgCbox.Checked)
            {
                silentmsgCbox.Checked = false;
            }
        }
        private void setAPI_Click(object sender, EventArgs e)
        {
            apiText.Text = "d8a121c71c1edfd0fa99d21031ee3623";
        }
        private void assemblyPath_TextChanged(object sender, EventArgs e)
        {
            oGlobals.asmPath = assemblyPath.Text;
            if (assemblyPath.Text.EndsWith(".bat"))
            {
                gotoProtections.Enabled = false;
                gotoCodeEnc.Enabled = false;
                gotoDynamic.Enabled = false;
                gotoJIT.Enabled = false;
                gotoRenamer.Enabled = false;
                invalidmdCbox.Enabled = false;
                preserveallCbox.Enabled = false;
                fakesignatureCbox.Enabled = false;
                optimizecodeCbox.Enabled = false;
                mergingCbox.Enabled = false;
            }
            else
            {
                gotoProtections.Enabled = true;
                gotoCodeEnc.Enabled = true;
                gotoDynamic.Enabled = true;
                gotoJIT.Enabled = true;
                gotoRenamer.Enabled = true;
                invalidmdCbox.Enabled = true;
                preserveallCbox.Enabled = true;
                fakesignatureCbox.Enabled = true;
                optimizecodeCbox.Enabled = true;
                mergingCbox.Enabled = true;
            }
        }
        private void backtoProtections_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectTab(protectionsPage);
        }
        private void dynamiccctorCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (dynamiccctorCbox.Checked)
            {
                oGlobals.dynCctor = true;
                dynamicmethodsCbox.Checked = false;
            }
            else
                oGlobals.dynCctor = false;
        }
        private void GUI_Shown(object sender, EventArgs e)
        {
            guna2HtmlToolTip1.SetToolTip(guna2HtmlLabel9, "Injecting invalid metadata this can stop some decompilers");
            guna2HtmlToolTip1.SetToolTip(guna2HtmlLabel5, "Injecting fake obfuscation attributes");
            guna2HtmlToolTip1.SetToolTip(guna2HtmlLabel6, "Optimizing your code by simplifying and optimize code instructions");
            guna2HtmlToolTip1.SetToolTip(label1, "Preserves all metadata, Enable this with mono apps or sensitives apps if some protections crush your app");
            guna2HtmlToolTip1.SetToolTip(guna2HtmlLabel1, "Embed dlls to resources so no need to keep them with exe in same folder");
            //
            guna2HtmlToolTip1.SetToolTip(guna2Panel2, "Prevents your assembly from being debugged");
            guna2HtmlToolTip1.SetToolTip(guna2Panel7, "Marks the module with a attribute that discourage ILDasm from disassembling it");
            guna2HtmlToolTip1.SetToolTip(guna2Panel3, "Prevents your assembly from being dumped from memory");
            guna2HtmlToolTip1.SetToolTip(guna2Panel4, "Prevents some tools that can be used to crack your app like: DnSpy, OllyDBG & etc...");
            guna2HtmlToolTip1.SetToolTip(guna2Panel6, "Prevents your assembly from being ran on Virtual machines");
            guna2HtmlToolTip1.SetToolTip(guna2Panel14, "Checks the integrity of the app to prevent modifing");
            guna2HtmlToolTip1.SetToolTip(guna2Panel10, "Encoding & compressing resources");
            guna2HtmlToolTip1.SetToolTip(guna2Panel8, "Encoding & hashing strings for better performance");
            guna2HtmlToolTip1.SetToolTip(guna2Panel9, "Mutates constants to confuse integers");
            guna2HtmlToolTip1.SetToolTip(guna2Panel11, "Mangles the code in the methods");
            guna2HtmlToolTip1.SetToolTip(guna2Panel13, "Convert all locals to fields");
            guna2HtmlToolTip1.SetToolTip(guna2Panel12, "Hide references with indirection method as proxy");
            guna2HtmlToolTip1.SetToolTip(guna2Panel18, "Adding random classes to confuse");
            guna2HtmlToolTip1.SetToolTip(junkLabel, "Junk length");
            guna2HtmlToolTip1.SetToolTip(guna2Panel15, "Converts cctor to dynamic and hide everything inside cctor");
            //
            guna2HtmlToolTip1.SetToolTip(guna2Panel19, "Encrypt & hash methods to hide code");
            //
            guna2HtmlToolTip1.SetToolTip(guna2Panel20, "Just in time ( JIT ) Hook");
            //
            guna2HtmlToolTip1.SetToolTip(guna2Panel21, "Converts code to dynamic");
            //
            guna2HtmlToolTip1.SetToolTip(guna2Panel24, "Obfuscate name symbols");
        }
        private void loadProtections_Click_1(object sender, EventArgs e)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + @"\Saved.txt"))
            {
                string lines = File.ReadAllText(Directory.GetCurrentDirectory() + @"\Saved.txt");
                if (lines.Contains("Exclude = true"))
                    excludeCbox.Checked = true;
                try
                {
                    if (lines.Contains("Exclude = true"))
                        excludeText.Text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\Exclude.txt");
                }              
                catch { }
                try
                {
                    if (lines.Contains("Custom Renaming = true"))
                    {
                        customrnCbox.Checked = true;
                        customText.Text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\Custom.txt");
                    }
                }
                catch { }
                if (lines.Contains("Dynamic cctor = true"))
                    dynamiccctorCbox.Checked = true;
                if (lines.Contains("Old Renamer = true"))
                    renamerCbox.Checked = true;
                if (lines.Contains("Types = true"))
                    rnTypes.Checked = true;
                if (lines.Contains("Properties = true"))
                    rnProperties.Checked = true;
                if (lines.Contains("Fields = true"))
                    rnFields.Checked = true;
                if (lines.Contains("Events = true"))
                    rnEvents.Checked = true;
                if (lines.Contains("Methods = true"))
                    rnMethods.Checked = true;
                if (lines.Contains("Parameters = true"))
                    rnParameters.Checked = true;
                if (lines.Contains("Code Optimization = true"))
                    optimizecodeCbox.Checked = true;
                if (lines.Contains("Preserve All = true"))
                    preserveallCbox.Checked = true;
                if (lines.Contains("Integrity = true"))
                    integrityCbox.Checked = true;
                if (lines.Contains("Junk = true"))
                    junkCbox.Checked = true;
                if (lines.Contains("Invalid Metadata = true"))
                    invalidmdCbox.Checked = true;
                if (lines.Contains("Fake Attributes = true"))
                    fakesignatureCbox.Checked = true;
                if (lines.Contains("JIT = true"))
                    jitCbox.Checked = true;
                if (lines.Contains("Resources = true"))
                    encoderesourcesCbox.Checked = true;
                if (lines.Contains("Anti ILDasm = true"))
                    antiildasmCbox.Checked = true;
                if (lines.Contains("Control Flow = true"))
                    controlflowCbox.Checked = true;
                if (lines.Contains("Ref Proxy = true"))
                    referenceproxyCbox.Checked = true;
                if (lines.Contains("Strings Encoding = true"))
                    encodestringsCbox.Checked = true;
                if (lines.Contains("Main Anti Crack = true"))
                    anticrackCbox.Checked = true;
                if (lines.Contains("Anti Crack = true"))
                    normalacCbox.Checked = true;
                if (lines.Contains("Anti Crack Data Sending = true"))
                    dsacCbox.Checked = true;
                try
                {
                    if (lines.Contains("WebHook = true"))
                        webhookText.Text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\Webhook.txt");
                }
                catch { }
                try
                {
                    if (lines.Contains("API = true"))
                        apiText.Text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\API.txt");
                }
                catch { }
                if (lines.Contains("Ban Cracker = true"))
                    banCbox.Checked = true;
                if (lines.Contains("Capture Screen = true"))
                    capturessCbox.Checked = true;
                if (lines.Contains("Silent MessageBox = true"))
                    silentmsgCbox.Checked = true;
                if (lines.Contains("Normal MessageBox = true"))
                    normalmsgCbox.Checked = true;
                try
                {
                    if (lines.Contains("Custom Message = true"))
                        msgText.Text = File.ReadAllText(Directory.GetCurrentDirectory() + @"\CustomMessage.txt");
                }
                catch { }
                if (lines.Contains("Anti Dump = true"))
                    antidumpCbox.Checked = true;
                if (lines.Contains("Code Encryption = true"))
                    codeencCbox.Checked = true;
                if (lines.Contains("Anti VM = true"))
                    antivmCbox.Checked = true;
                if (lines.Contains("Anti Debug = true"))
                    antidebugCbox.Checked = true;
                if (lines.Contains("Mutation = true"))
                    encodeintsCbox.Checked = true;
                if (lines.Contains("Local To Field = true"))
                    localtofieldCbox.Checked = true;
                if (lines.Contains("Local To Field Preset = Off"))
                    localtofieldLength.Value = 0;
                if (lines.Contains("Local To Field Preset = Field"))
                    localtofieldLength.Value = 1;
                if (lines.Contains("Local To Field Preset = Method"))
                    localtofieldLength.Value = 2;
            }
        }
        private void checkAll_Click(object sender, EventArgs e)
        {
            if (checkAll.Text == "Check all")
            {
                checkAll.Text = "Uncheck all";
                checkAll.Image = Properties.Resources.uncheck_all_20px;
                codeencCbox.Checked = true;
                antiildasmCbox.Checked = true;
                antidebugCbox.Checked = true;
                antidumpCbox.Checked = true;
                anticrackCbox.Checked = true;
                antivmCbox.Checked = true;
                integrityCbox.Checked = true;
                encoderesourcesCbox.Checked = true;
                encodestringsCbox.Checked = true;
                encodeintsCbox.Checked = true;
                controlflowCbox.Checked = true;
                localtofieldCbox.Checked = true;
                referenceproxyCbox.Checked = true;
                junkCbox.Checked = true;
                renamerCbox.Checked = true;
                rnEvents.Checked = true;
                rnFields.Checked = true;
                rnMethods.Checked = true;
                rnParameters.Checked = true;
                rnProperties.Checked = true;
                rnTypes.Checked = true;
                normalacCbox.Checked = true;
                silentmsgCbox.Checked = true;
                dynamiccctorCbox.Checked = true;
            }
            else
            {
                checkAll.Text = "Check all";
                checkAll.Image = Properties.Resources.check_all_20px;
                codeencCbox.Checked = false;
                antiildasmCbox.Checked = false;
                antidebugCbox.Checked = false;
                antidumpCbox.Checked = false;
                anticrackCbox.Checked = false;
                antivmCbox.Checked = false;
                integrityCbox.Checked = false;
                encoderesourcesCbox.Checked = false;
                encodestringsCbox.Checked = false;
                encodeintsCbox.Checked = false;
                controlflowCbox.Checked = false;
                localtofieldCbox.Checked = false;
                referenceproxyCbox.Checked = false;
                junkCbox.Checked = false;
                renamerCbox.Checked = false;
                rnEvents.Checked = false;
                rnFields.Checked = false;
                rnMethods.Checked = false;
                rnParameters.Checked = false;
                rnProperties.Checked = false;
                rnTypes.Checked = false;
                normalacCbox.Checked = false;
                silentmsgCbox.Checked = false;
                dynamiccctorCbox.Checked = false;
            }
        }
        private void randomrtNames_Click(object sender, EventArgs e)
        {
            x64runtimeName.Text = ICore.Safe.GenerateRandomLetters(5);
            x86runtimeName.Text = ICore.Safe.GenerateRandomLetters(5);
        }
        private void customrnCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (customrnCbox.Checked)
            {
                Protections.Renamer.GlobalName.custom = true;
            }
            else
            {
                Protections.Renamer.GlobalName.custom = false;
            }
        }
        private void excludeCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (excludeCbox.Checked)
                SECURE_BYTE_GUI.Global_for_Obfuscation.oGlobals.excludeforAC = true;
            else
                SECURE_BYTE_GUI.Global_for_Obfuscation.oGlobals.excludeforAC = false;
        }
        private void excludeText_TextChanged(object sender, EventArgs e)
        {
            SECURE_BYTE_GUI.Global_for_Obfuscation.oGlobals.excludeString = excludeText.Text;
        }
        private void guiSettings_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectTab(settingsPage);
        }
        private void backTo_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectTab(protectionsPage);
        }
        private void guiTransparency_CheckedChanged(object sender, EventArgs e)
        {
            if (guiTransparency.Checked)
            {
                Opacity = 0.98;
                gGlobals.transparency = true;
            }
            else
            {
                Opacity = 100;
                gGlobals.transparency = false;
            }
        }
        private void autoOpenDir_CheckedChanged(object sender, EventArgs e)
        {
            if (autoOpenDir.Checked)
                gGlobals.autoDir = true;
            else
                gGlobals.autoDir = false;
        }
    }
}
