﻿//----------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.System.UserProfile;
using Microsoft.Identity.Client.Internal;

namespace Microsoft.Identity.Client
{
    internal class PlatformInformation : PlatformInformationBase
    {
        public override string GetProductName()
        {
            return "MSAL.WinRT";
        }

        public override string GetEnvironmentVariable(string variable)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            return localSettings.Values.ContainsKey(variable) ? localSettings.Values[variable].ToString() : null;
        }

        public override async Task<string> GetUserPrincipalNameAsync()
        {
            if (!UserInformation.NameAccessAllowed)
            {
                throw new MsalException(MsalErrorEx.CannotAccessUserInformation, MsalErrorMessageEx.CannotAccessUserInformation);
            }

            try
            {
                return await UserInformation.GetPrincipalNameAsync().AsTask().ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException ex)
            {
                PlatformPlugin.Logger.Error(null, ex);
                throw new MsalException(MsalErrorEx.UnauthorizedUserInformationAccess, MsalErrorMessageEx.UnauthorizedUserInformationAccess, ex);
            }
        }

        public override string GetProcessorArchitecture()
        {
            return NativeMethods.GetProcessorArchitecture();
        }

        public override string GetOperatingSystem()
        {
            // In WinRT, there is no way to reliably get OS version. All can be done reliably is to check 
            // for existence of specific features which does not help in this case, so we do not emit OS in WinRT.
            return null;
        }

        public override string GetDeviceModel()
        {
            var deviceInformation = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
            return deviceInformation.SystemProductName;
        }

        public override async Task<bool> IsUserLocalAsync(CallState callState)
        {
            if (!UserInformation.NameAccessAllowed)
            {
                // The access is not allowed and we cannot determine whether this is a local user or not. So, we do NOT add form auth parameter.
                // This is the case where we can advise customers to add extra query parameter if they want.

                PlatformPlugin.Logger.Information(callState, "Cannot access user information to determine whether it is a local user or not due to machine's privacy setting.");
                return false;
            }

            try
            {
                return string.IsNullOrEmpty(await UserInformation.GetDomainNameAsync().AsTask().ConfigureAwait(false));
            }
            catch (UnauthorizedAccessException ae)
            {
                PlatformPlugin.Logger.Warning(callState, ae.Message);
                PlatformPlugin.Logger.Information(callState, "Cannot try Windows Integrated Auth due to lack of Enterprise capability.");
                // This mostly means Enterprise capability is missing, so WIA cannot be used and
                // we return true to add form auth parameter in the caller.
                return true;
            }            
        }

        public override bool IsDomainJoined()
        {
            return NetworkInformation.GetHostNames().Any(entry => entry.Type == HostNameType.DomainName);
        }

        public override Uri ValidateRedirectUri(Uri redirectUri, CallState callState)
        {
            if (redirectUri == null)
            {
                redirectUri = Constant.SsoPlaceHolderUri;
                PlatformPlugin.Logger.Verbose(callState, "ms-app redirect Uri is used");
            }

            return redirectUri;
        }

        public override string GetRedirectUriAsString(Uri redirectUri, CallState callState)
        {
            return ReferenceEquals(redirectUri, Constant.SsoPlaceHolderUri) ?
                WebAuthenticationBroker.GetCurrentApplicationCallbackUri().OriginalString :
                redirectUri.OriginalString;
        }

        private static class NativeMethods
        {
            private const int PROCESSOR_ARCHITECTURE_AMD64 = 9;
            private const int PROCESSOR_ARCHITECTURE_ARM = 5;
            private const int PROCESSOR_ARCHITECTURE_IA64 = 6;
            private const int PROCESSOR_ARCHITECTURE_INTEL = 0;

            [DllImport("kernel32.dll")]
            private static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);

            public static string GetProcessorArchitecture()
            {
                try
                {
                    SYSTEM_INFO systemInfo = new SYSTEM_INFO();
                    GetNativeSystemInfo(ref systemInfo);
                    switch (systemInfo.wProcessorArchitecture)
                    {
                        case PROCESSOR_ARCHITECTURE_AMD64:
                        case PROCESSOR_ARCHITECTURE_IA64:
                            return "x64";

                        case PROCESSOR_ARCHITECTURE_ARM:
                            return "ARM";

                        case PROCESSOR_ARCHITECTURE_INTEL:
                            return "x86";

                        default:
                            return "Unknown";
                    }
                }
                catch(Exception ex)
                {
                    PlatformPlugin.Logger.Warning(null, ex.Message);
                    return "Unknown";
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct SYSTEM_INFO
            {
                public short wProcessorArchitecture;
                public short wReserved;
                public int dwPageSize;
                public IntPtr lpMinimumApplicationAddress;
                public IntPtr lpMaximumApplicationAddress;
                public IntPtr dwActiveProcessorMask;
                public int dwNumberOfProcessors;
                public int dwProcessorType;
                public int dwAllocationGranularity;
                public short wProcessorLevel;
                public short wProcessorRevision;
            }
        }
    }
}
