using ActiveDirectoryComputerInfoUpdater.Logic;
using Mach.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ActiveDirectoryComputerInfoUpdater.ViewModel
{
    public class WindowViewModel : NotifyPropertyBase
    {
        private OrganizationalUnitsTreeViewModel _organizationalUnits;
        private DelegateCommand _loadOrganizationalUnitsCommand;

        public OrganizationalUnitsTreeViewModel OrganizationalUnits
        {
            get { return _organizationalUnits; }
            set
            {
                _organizationalUnits = value;
                OnPropertyChanged();
            }
        }
        public ICommand LoadOrganizationalUnitsCommand
        {
            get { return _loadOrganizationalUnitsCommand; }
        }

        public WindowViewModel()
        {
            _loadOrganizationalUnitsCommand = new DelegateCommand(LoadOrganizationalUnits);
        }

        private void LoadOrganizationalUnits()
        {
            OrganizationalUnitsTreeViewModel organizationalUnits = new OrganizationalUnitsTreeViewModel();
            organizationalUnits.LoadTree();

            OrganizationalUnits = organizationalUnits;
        }
    }
}
