using ActiveDirectoryComputerInfoUpdater.Logic;
using Mach.Wpf.Mvvm;
using System;
using System.DirectoryServices;
using System.Windows.Input;

namespace ActiveDirectoryComputerInfoUpdater.ViewModel
{
    public class ComputerViewModel : NotifyPropertyBase
    {
        private DelegateCommand _assignOwnerCommand;
        private UserViewModel _owner;
        private UserViewModel _detectedUser;
        private DirectoryEntry _directoryEntry;

        public ComputerViewModel(DirectoryEntry directoryEntry)
        {
            _directoryEntry = directoryEntry;

            _assignOwnerCommand = new DelegateCommand(AssignOwner, CanExecuteAssignOwner);
        }

        public ComputerViewModel(DirectoryEntry directoryEntry, UserViewModel owner) : this(directoryEntry)
        {
            _owner = owner;
        }

        private bool CanExecuteAssignOwner(object parameter)
        {
            return _detectedUser != null && _owner != _detectedUser;
        }

        private void AssignOwner()
        {
            Owner = _detectedUser;
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
                    if (value != null && _directoryEntry != null)
                    {
                        _owner = value;

                        ActiveDirectory.UpdateDirectoryEntry(_directoryEntry, "managedBy", value.DistinguishedName);
                    }

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

        public ICommand AssignOwnerCommand
        {
            get
            {
                return _assignOwnerCommand;
            }
        }
    }
}
