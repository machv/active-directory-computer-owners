using ActiveDirectoryComputerInfoUpdater.Logic;
using Mach.Wpf.Mvvm;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Windows.Input;

namespace ActiveDirectoryComputerInfoUpdater.ViewModel
{
    public class WindowViewModel : NotifyPropertyBase
    {
        private OrganizationalUnitsTreeViewModel _organizationalUnits;
        private DelegateCommand _loadOrganizationalUnitsCommand;
        private DelegateCommand _organizationalUnitChangedCommand;
        private DelegateCommand _loadUsersCommand;
        private OrganizationalUnitViewModel _selectedOrganizationalUnit;
        private ObservableCollection<UserViewModel> _users;

        public OrganizationalUnitsTreeViewModel OrganizationalUnits
        {
            get { return _organizationalUnits; }
            set
            {
                _organizationalUnits = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<UserViewModel> Users
        {
            get { return _users; }
            set
            {
                _users = value;
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
        public ICommand LoadUsersCommand
        {
            get { return _loadUsersCommand; }
        }

        public WindowViewModel()
        {
            _loadOrganizationalUnitsCommand = new DelegateCommand(LoadOrganizationalUnits);
            _organizationalUnitChangedCommand = new DelegateCommand(OrganizationalUnitChanged);
            _loadUsersCommand = new DelegateCommand(LoadUsers);

            _selectedOrganizationalUnit = new OrganizationalUnitViewModel(null); //dummy selected item
        }

        public void LoadUsers()
        {
            ObservableCollection<UserViewModel> users = new ObservableCollection<UserViewModel>();

            DirectoryEntry rootEntry = ActiveDirectory.GetDirectoryEntry();
            using (SearchResultCollection usersResult = ActiveDirectory.GetUsers(rootEntry))
            {
                foreach (SearchResult result in usersResult)
                {
                    UserViewModel user = new UserViewModel(result.GetDirectoryEntry());
                    user.Name = result.Properties["name"][0] as string;
                    user.SamAccountName = result.Properties["samaccountname"][0] as string;
                    user.DistinguishedName = ActiveDirectory.GetDistinguishedNameFromPath(result.Path);
                    if (result.Properties.Contains("c"))
                        user.Country = result.Properties["c"][0] as string;

                    users.Add(user);
                }
            }

            Users = users;
        }

        private void OrganizationalUnitChanged()
        {
            _selectedOrganizationalUnit.LoadComputers(Users);
        }

        private void LoadOrganizationalUnits()
        {
            OrganizationalUnitsTreeViewModel organizationalUnits = new OrganizationalUnitsTreeViewModel();
            organizationalUnits.LoadTree();

            OrganizationalUnits = organizationalUnits;
        }
    }
}
