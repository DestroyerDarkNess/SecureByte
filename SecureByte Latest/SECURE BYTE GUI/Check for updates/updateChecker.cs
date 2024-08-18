using System.Net.Http;
using System.Net;

namespace SECURE_BYTE_GUI.Check_for_updates
{
    public static class updateChecker
    {
        public static void checkforUpdates()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string ver = client.GetStringAsync("https://raw.githubusercontent.com/ItIsInx/Sbyte-Updates/main/Check").Result;
                    if (!ver.Contains("74"))
                    {
                        customMessage.msg = "New update detected, You can get it from server !";
                        new customMessage().ShowDialog();
                    }
                }
                catch
                {
                    customMessage.msg = "Failed to check for updates !";
                    new customMessage().ShowDialog();
                }
            }
        }
    }
}