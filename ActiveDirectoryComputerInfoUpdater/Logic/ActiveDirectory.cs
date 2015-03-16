using System;
using System.DirectoryServices;
using System.Text;

namespace ActiveDirectoryComputerInfoUpdater.Logic
{
    public class ActiveDirectory
    {
        /// <summary>
        /// Update property on directory entry with new value.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns>true if property already existed and was updated.</returns>
        public static bool UpdateDirectoryEntry(DirectoryEntry entry, string property, object value)
        {
            bool exists = false;

            if (entry.Properties.Contains(property))
            {
                entry.Properties[property].Value = value;

                exists = true;
            }
            else
            {
                entry.Properties[property].Add(value);
            }

            entry.CommitChanges();

            return exists;
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
    }
}

