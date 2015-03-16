using Mach.Wpf.Mvvm;
using System;
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
        public int BitlockerRecoveryKeys { get; set; }
        public string Location { get; set; }
    }
}
