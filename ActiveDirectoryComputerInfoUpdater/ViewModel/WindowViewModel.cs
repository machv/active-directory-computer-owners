using Mach.Wpf.Mvvm;
using System.Windows.Input;

namespace ActiveDirectoryComputerInfoUpdater.ViewModel
{
    public class WindowViewModel : NotifyPropertyBase
    {
        private OrganizationalUnitsTreeViewModel _organizationalUnits;
        private DelegateCommand _loadOrganizationalUnitsCommand;
        private DelegateCommand _organizationalUnitChangedCommand;
        private OrganizationalUnitViewModel _selectedOrganizationalUnit;

        public OrganizationalUnitsTreeViewModel OrganizationalUnits
        {
            get { return _organizationalUnits; }
            set
            {
                _organizationalUnits = value;
                OnPropertyChanged();
            }
        }
        public OrganizationalUnitViewModel SelectedOrganizationalUnit
        {
            get { return _selectedOrganizationalUnit; }
            set
            {
                _selectedOrganizationalUnit = value;
                OnPropertyChanged();
            }
        }
        public ICommand LoadOrganizationalUnitsCommand
        {
            get { return _loadOrganizationalUnitsCommand; }
        }
        public ICommand OrganizationalUnitChangedCommand
        {
            get { return _organizationalUnitChangedCommand; }
        }

        public WindowViewModel()
        {
            _loadOrganizationalUnitsCommand = new DelegateCommand(LoadOrganizationalUnits);
            _organizationalUnitChangedCommand = new DelegateCommand(OrganizationalUnitChanged);

            _selectedOrganizationalUnit = new OrganizationalUnitViewModel(null); //dummy selected item
        }

        private void OrganizationalUnitChanged()
        {
            _selectedOrganizationalUnit.LoadComputers();
        }

        private void LoadOrganizationalUnits()
        {
            OrganizationalUnitsTreeViewModel organizationalUnits = new OrganizationalUnitsTreeViewModel();
            organizationalUnits.LoadTree();

            OrganizationalUnits = organizationalUnits;
        }
    }
}
