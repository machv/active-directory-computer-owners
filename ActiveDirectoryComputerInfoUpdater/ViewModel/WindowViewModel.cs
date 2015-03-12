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
        private ObservableCollection<OrganizationalUnitViewModel> _organizationalUnits;
        private DelegateCommand _loadOrganizationalUnitsCommand;

        public ObservableCollection<OrganizationalUnitViewModel> OrganizationalUnits
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
            ObservableCollection<OrganizationalUnitViewModel> units = new ObservableCollection<OrganizationalUnitViewModel>();

            DirectoryEntry rootEntry = ActiveDirectory.GetDirectoryEntry();
            OrganizationalUnitViewModel root = new OrganizationalUnitViewModel(rootEntry);
            root.Name = "AD";
            root.LoadChildren(true);

            units.Add(root);

            OrganizationalUnits = units;

           // OrganizationalUnitsTreeViewModel organizationalUnits = new OrganizationalUnitsTreeViewModel();
           // organizationalUnits.LoadTree();

           // OrganizationalUnits = organizationalUnits;
        }
    }
}
