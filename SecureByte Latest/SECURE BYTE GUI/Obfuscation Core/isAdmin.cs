using System.Runtime.InteropServices;
using System;

namespace SecureByte.IAdmin
{
    internal class isAdmin
    {
        public static bool IsRunningWithElevatedPrivileges()
        {
            IntPtr hToken;
            int sizeofTokenElevationType = Marshal.SizeOf(typeof(int));
            IntPtr pElevationType =
                Marshal.AllocHGlobal(sizeofTokenElevationType);
            if (OpenProcessToken(GetCurrentProcess(), TokenQuery, out hToken))
            {
                uint dwSize;
                if (GetTokenInformation(hToken,
                    TokenInformationClass.TokenElevationType, pElevationType,
                    (uint)sizeofTokenElevationType, out dwSize))
                {
                    TokenElevationType elevationType = (TokenElevationType)Marshal.ReadInt32(pElevationType);
                    Marshal.FreeHGlobal(pElevationType);
                    switch (elevationType)
                    {
                        case TokenElevationType.TokenElevationTypeFull:
                            return true;
                        default:
                            return false;
                    }
                }
            }
            return false;
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool OpenProcessToken(
            IntPtr processHandle,
            uint desiredAccess,
            out IntPtr tokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetTokenInformation(
            IntPtr tokenHandle,
            TokenInformationClass tokenInformationClass,
            IntPtr tokenInformation,
            uint tokenInformationLength,
            out uint returnLength);

        const UInt32 TokenQuery = 0x0008;

        enum TokenElevationType
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }

        enum TokenInformationClass
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }
    }
}
