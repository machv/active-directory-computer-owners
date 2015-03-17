using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryComputerInfoUpdater
{
    public static class Remote
    {
        public static string DetectLogin(string computerName, string username = null, string password = null, string domain = null)
        {
            try
            {
                ManagementScope scope;

                if (!computerName.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                {
                    ConnectionOptions connection = new ConnectionOptions();
                    connection.Username = username;
                    connection.Password = password;
                    if(domain != null)
                        connection.Authority = "ntlmdomain:" + domain;

                    scope = new ManagementScope(string.Format("\\\\{0}\\root\\CIMV2", computerName), connection);
                }
                else
                    scope = new ManagementScope(string.Format("\\\\{0}\\root\\CIMV2", computerName), null);

                scope.Connect();

                ObjectQuery query = new ObjectQuery("Select UserName from Win32_ComputerSystem");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                foreach (ManagementObject wmiObject in searcher.Get())
                {
                    if (!string.IsNullOrEmpty(wmiObject["UserName"] as string))
                        return wmiObject["UserName"] as string;
                }
            }
            catch { }

            return null;
        }
    }
}
