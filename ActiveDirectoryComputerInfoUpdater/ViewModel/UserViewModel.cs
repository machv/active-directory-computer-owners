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
        private string _name;
        private string _samAccountName;
        private string _distinguishedName;
        private string _country;

        public UserViewModel(DirectoryEntry directoryEntry)
        {
            _directoryEntry = directoryEntry;
        }

        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }
        public string DistinguishedName { get { return _distinguishedName; } set { _distinguishedName = value; OnPropertyChanged(); } }
        public string SamAccountName { get { return _samAccountName; } set { _samAccountName = value; OnPropertyChanged(); } }
        public string Country { get { return _country; } set { _country = value; OnPropertyChanged(); } }
    }
}
