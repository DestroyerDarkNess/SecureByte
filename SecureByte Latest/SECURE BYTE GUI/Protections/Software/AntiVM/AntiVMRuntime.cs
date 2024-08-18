using Microsoft.Win32;
using System;
using System.Management;
using System.Runtime.InteropServices;

namespace Protections.Runtime
{
    internal static class AntiVM
    {
        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandle")]
        internal static extern IntPtr b(string c);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        internal static extern IntPtr B(string b, IntPtr c);
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        internal static extern IntPtr c(IntPtr c, string d);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "GetFileAttributes", SetLastError = true)]
        internal static extern uint C(string d);
        internal static void Initialize()
        {
            if (e())
            {
                Environment.Exit(0);
            }
        }
        internal static bool e()
        {
            try
            {
                if (E("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("VBOX"))
                {
                    return true;
                }
                if (E("HARDWARE\\Description\\System", "SystemBiosVersion").ToUpper().Contains("VBOX"))
                {
                    return true;
                }
                if (E("HARDWARE\\Description\\System", "VideoBiosVersion").ToUpper().Contains("VIRTUALBOX"))
                {
                    return true;
                }
                if (E("SOFTWARE\\Oracle\\VirtualBox Guest Additions", "") == "noValueButYesKey")
                {
                    return true;
                }
                if (C("C:\\WINDOWS\\system32\\drivers\\VBoxMouse.sys") != 4294967295u)
                {
                    return true;
                }
                if (E("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("VMWARE"))
                {
                    return true;
                }
                if (E("SOFTWARE\\VMware, Inc.\\VMware Tools", "") == "noValueButYesKey")
                {
                    return true;
                }
                if (E("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 1\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("VMWARE"))
                {
                    return true;
                }
                if (E("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 2\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("VMWARE"))
                {
                    return true;
                }
                if (E("SYSTEM\\ControlSet001\\Services\\Disk\\Enum", "0").ToUpper().Contains("vmware".ToUpper()))
                {
                    return true;
                }
                if (E("SYSTEM\\ControlSet001\\Control\\Class\\{4D36E968-E325-11CE-BFC1-08002BE10318}\\0000", "DriverDesc").ToUpper().Contains("VMWARE"))
                {
                    return true;
                }
                if (E("SYSTEM\\ControlSet001\\Control\\Class\\{4D36E968-E325-11CE-BFC1-08002BE10318}\\0000\\Settings", "Device Description").ToUpper().Contains("VMWARE"))
                {
                    return true;
                }
                if (E("SOFTWARE\\VMware, Inc.\\VMware Tools", "InstallPath").ToUpper().Contains("C:\\PROGRAM FILES\\VMWARE\\VMWARE TOOLS\\"))
                {
                    return true;
                }
                if (C("C:\\WINDOWS\\system32\\drivers\\vmmouse.sys") != 4294967295u)
                {
                    return true;
                }
                if (C("C:\\WINDOWS\\system32\\drivers\\vmhgfs.sys") != 4294967295u)
                {
                    return true;
                }
                if (c(b("kernel32.dll"), "wine_get_unix_file_name") != (IntPtr)0)
                {
                    return true;
                }
                if (E("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("QEMU"))
                {
                    return true;
                }
                if (E("HARDWARE\\Description\\System", "SystemBiosVersion").ToUpper().Contains("QEMU"))
                {
                    return true;
                }
                ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_VideoController");
                foreach (ManagementBaseObject managementBaseObject in new ManagementObjectSearcher(scope, query).Get())
                {
                    ManagementObject managementObject = (ManagementObject)managementBaseObject;
                    if (managementObject["Description"].ToString() == "VM Additions S3 Trio32/64")
                    {
                        return true;
                    }
                    if (managementObject["Description"].ToString() == "S3 Trio32/64")
                    {
                        return true;
                    }
                    if (managementObject["Description"].ToString() == "VirtualBox Graphics Adapter")
                    {
                        return true;
                    }
                    if (managementObject["Description"].ToString() == "VMware SVGA II")
                    {
                        return true;
                    }
                    if (managementObject["Description"].ToString().ToUpper().Contains("VMWARE"))
                    {
                        return true;
                    }
                    if (managementObject["Description"].ToString() == "")
                    {
                        return true;
                    }
                }
            }
            catch
            {

            }      
            return false;
        }
        internal static string E(string A_0, string A_1)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(A_0, false);
            if (registryKey == null)
            {
                return "noKey";
            }
            object value = registryKey.GetValue(A_1, "noValueButYesKey");
            if (value.GetType() == typeof(string))
            {
                return value.ToString();
            }
            if (registryKey.GetValueKind(A_1) == RegistryValueKind.String || registryKey.GetValueKind(A_1) == RegistryValueKind.ExpandString)
            {
                return value.ToString();
            }
            if (registryKey.GetValueKind(A_1) == RegistryValueKind.DWord)
            {
                return Convert.ToString((int)value);
            }
            if (registryKey.GetValueKind(A_1) == RegistryValueKind.QWord)
            {
                return Convert.ToString((long)value);
            }
            if (registryKey.GetValueKind(A_1) == RegistryValueKind.Binary)
            {
                return Convert.ToString((byte[])value);
            }
            if (registryKey.GetValueKind(A_1) == RegistryValueKind.MultiString)
            {
                return string.Join("", (string[])value);
            }
            return "noValueButYesKey";
        }
    }
}