using System;
using System.ComponentModel;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ActiveDirectoryComputerInfoUpdater.Logic
{
    public class ActiveDirectory
    {
        /// <summary>
        /// Update property on directory entry with new value.
        /// </summary>
        /// <param name="directoryEntry"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns>true if property already existed and was updated.</returns>
        public static bool UpdateDirectoryEntry(DirectoryEntry directoryEntry, string property, object value)
        {
            bool exists = false;

            if (directoryEntry.Properties.Contains(property))
            {
                directoryEntry.Properties[property].Value = value;

                exists = true;
            }
            else
            {
                directoryEntry.Properties[property].Add(value);
            }

            directoryEntry.CommitChanges();

            return exists;
        }

        public static void RemoveDirectoryEntryProperty(DirectoryEntry directoryEntry, string property)
        {
            if (directoryEntry.Properties.Contains(property))
            {
                object value = directoryEntry.Properties[property][0];
                directoryEntry.Properties[property].Remove(value);
            }
        }

        public static void RemoveDirectoryEntryProperty(DirectoryEntry directoryEntry, string property, object value)
        {
            if (directoryEntry.Properties.Contains(property))
            {
                directoryEntry.Properties[property].Remove(value);
            }
        }

        public static SearchResultCollection GetSingleLevelSubOrganizationalUnits(DirectoryEntry searchRoot)
        {
            string searchFilter = "(|(objectcategory=organizationalunit)(objectcategory=container))";

            SearchResultCollection results = FindObjects(searchFilter, searchRoot, SearchScope.OneLevel);

            return results;
        }

        public static SearchResultCollection GetComputersInOrganizationalUnit(DirectoryEntry searchRoot)
        {
            string searchFilter = "(objectcategory=computer)";

            SearchResultCollection results = FindObjects(searchFilter, searchRoot, SearchScope.OneLevel);

            return results;
        }

        public static SearchResultCollection GetUsers(DirectoryEntry searchRoot)
        {
            string searchFilter = "(&(objectClass=user)(objectCategory=person))";

            SearchResultCollection results = FindObjects(searchFilter, searchRoot, SearchScope.Subtree);

            return results;
        }

        public static SearchResultCollection GetBitlockerRecoveryKeys(DirectoryEntry searchRoot)
        {
            string searchFilter = "(objectClass=msFVE-RecoveryInformation)";

            SearchResultCollection results = FindObjects(searchFilter, searchRoot, SearchScope.Subtree);

            return results;
        }

        public static SearchResultCollection FindObjects(string filter, DirectoryEntry searchRoot, SearchScope scope)
        {
            DirectoryEntry entry = null;
            DirectorySearcher searcher = null;

            if (filter == null)
                throw new ArgumentNullException("filter", "The search filter cannot be null.");

            if (searchRoot == null)
            {
                entry = GetDirectoryEntry();
                searcher = new DirectorySearcher(entry);
            }
            else
            {
                searcher = new DirectorySearcher(searchRoot);
            }

            searcher.Filter = filter;
            searcher.SearchScope = scope;

            SearchResultCollection results = searcher.FindAll();

            return results;
        }

        /// <summary>
        /// This method retreives a new directory entry object.
        /// </summary>
        /// <returns>A DirectoryEntry object.</returns>
        public static DirectoryEntry GetDirectoryEntry()
        {
            DirectoryEntry entry;

            //if (preferredServer != null)
            //{
            //    entry = new DirectoryEntry("LDAP://" + preferredServer, appAccountId, appAccountPwd, AuthenticationTypes.ServerBind | AuthenticationTypes.Secure);
            //}
            //else
            //{
                entry = new DirectoryEntry("LDAP://" + GetDomainName(), null, null, AuthenticationTypes.Secure);
            //}
            return entry;
        }

        /// <summary>
        /// This method returns the simple Domain Name of the AD Domain
        /// </summary>
        /// <returns>The first value of the DC=* domain name from the default naming context.</returns>
        public static string GetDomainName()
        {
            DirectoryEntry rootDse = new DirectoryEntry("LDAP://rootDSE");

            //Get the defaultNamingContext and parse in into a properly formatted string
            //to use for binding with the global catalog
            string ldapDomain = rootDse.Properties["defaultNamingContext"][0].ToString();

            rootDse.Close();
            rootDse.Dispose();

            return ConvertDNToUpnSuffix(ldapDomain);
        }

        /// <summary>
        /// Takes a DN-formatted domain name and returns a domai name in UPN-suffix format
        /// For example: DC=ads,DC=uscg,DC=mil --> ads.uscg.mil
        /// </summary>
        /// <param name="domainDN">The raw distinguished name for a domain.</param>
        /// <returns>UPN suffix.</returns>
        private static string ConvertDNToUpnSuffix(string domainDN)
        {
            string delimStr = "=,";
            char[] delimiter = delimStr.ToCharArray();
            string[] split = null;

            split = domainDN.Split(delimiter);
            StringBuilder buf = new StringBuilder();
            foreach (string str in split)
            {
                if (str.Equals("DC"))
                {
                    continue;
                }
                else
                {
                    buf.Append(str);
                    buf.Append(".");
                }
            }

            buf.Remove(buf.Length - 1, 1);
            return buf.ToString();
        }

        public static string GetDistinguishedNameFromPath(string path)
        {
            int i = path.IndexOf('/', 7);
            if (i >= 0 && path.Length > i + 1)
            {
                return path.Substring(i + 1);
            }

            return path;
        }

        public static string GetNetbiosNameForDomain(string dns)
        {
            IntPtr pDomainInfo;
            int result = DsGetDcName(null, dns, IntPtr.Zero, null,
                DSGETDCNAME_FLAGS.DS_IS_DNS_NAME | DSGETDCNAME_FLAGS.DS_RETURN_FLAT_NAME,
                out pDomainInfo);
            try
            {
                if (result != ERROR_SUCCESS)
                    throw new Win32Exception(result);

                var dcinfo = new DomainControllerInfo();
                Marshal.PtrToStructure(pDomainInfo, dcinfo);

                return dcinfo.DomainName;
            }
            finally
            {
                if (pDomainInfo != IntPtr.Zero)
                    NetApiBufferFree(pDomainInfo);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class DomainControllerInfo
        {
            public string DomainControllerName;
            public string DomainControllerAddress;
            public int DomainControllerAddressType;
            public Guid DomainGuid;
            public string DomainName;
            public string DnsForestName;
            public int Flags;
            public string DcSiteName;
            public string ClientSiteName;
        }

        [Flags]
        private enum DSGETDCNAME_FLAGS : uint
        {
            DS_FORCE_REDISCOVERY = 0x00000001,
            DS_DIRECTORY_SERVICE_REQUIRED = 0x00000010,
            DS_DIRECTORY_SERVICE_PREFERRED = 0x00000020,
            DS_GC_SERVER_REQUIRED = 0x00000040,
            DS_PDC_REQUIRED = 0x00000080,
            DS_BACKGROUND_ONLY = 0x00000100,
            DS_IP_REQUIRED = 0x00000200,
            DS_KDC_REQUIRED = 0x00000400,
            DS_TIMESERV_REQUIRED = 0x00000800,
            DS_WRITABLE_REQUIRED = 0x00001000,
            DS_GOOD_TIMESERV_PREFERRED = 0x00002000,
            DS_AVOID_SELF = 0x00004000,
            DS_ONLY_LDAP_NEEDED = 0x00008000,
            DS_IS_FLAT_NAME = 0x00010000,
            DS_IS_DNS_NAME = 0x00020000,
            DS_RETURN_DNS_NAME = 0x40000000,
            DS_RETURN_FLAT_NAME = 0x80000000
        }

        [DllImport("Netapi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "DsGetDcNameW", CharSet = CharSet.Unicode)]
        private static extern int DsGetDcName(
            [In] string computerName,
            [In] string domainName,
            [In] IntPtr domainGuid,
            [In] string siteName,
            [In] DSGETDCNAME_FLAGS flags,
            [Out] out IntPtr domainControllerInfo);

        [DllImport("Netapi32.dll")]
        private static extern int NetApiBufferFree(
            [In] IntPtr buffer);

        private const int ERROR_SUCCESS = 0;
    }
}

