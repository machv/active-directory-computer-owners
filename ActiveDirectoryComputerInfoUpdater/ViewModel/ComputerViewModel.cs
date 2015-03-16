using Mach.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;

namespace ActiveDirectoryComputerInfoUpdater.ViewModel
{
    public class ComputerViewModel : NotifyPropertyBase
    {
        private DirectoryEntry _directoryEntry;

        public ComputerViewModel(DirectoryEntry directoryEntry)
        {
            _directoryEntry = directoryEntry;
        }

        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        public DateTime LastLogon { get; set; }
        public int LogonCount { get; set; }
        public string OperatingSystem { get; set; }
        public string Description { get; set; }
        public string ManagedBy { get; set; }

    }
}
