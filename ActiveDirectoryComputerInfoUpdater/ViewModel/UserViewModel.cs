using Mach.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryComputerInfoUpdater.ViewModel
{
    public class UserViewModel : NotifyPropertyBase
    {
        private DirectoryEntry _directoryEntry;

        public UserViewModel(DirectoryEntry directoryEntry)
        {
            _directoryEntry = directoryEntry;
        }

        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        public string SamAccountName { get; internal set; }
        public object Country { get; internal set; }
    }
}
