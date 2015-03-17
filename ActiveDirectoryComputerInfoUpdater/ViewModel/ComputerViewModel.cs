using ActiveDirectoryComputerInfoUpdater.Logic;
using Mach.Wpf.Mvvm;
using System;
using System.DirectoryServices;

namespace ActiveDirectoryComputerInfoUpdater.ViewModel
{
    public class ComputerViewModel : NotifyPropertyBase
    {
        private UserViewModel _owner;
        private UserViewModel _detectedUser;
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
        public UserViewModel Owner
        {
            get { return _owner; }
            set
            {
                if (_owner != value)
                {
                    _owner = value;

                    if (value != null && _directoryEntry != null)
                        ActiveDirectory.UpdateDirectoryEntry(_directoryEntry, "managedBy", value.DistinguishedName);

                    OnPropertyChanged();
                }
            }
        }
        public UserViewModel DetectedUser
        {
            get { return _detectedUser; }
            set
            {
                _detectedUser = value;
                OnPropertyChanged();
            }
        }
    }
}
