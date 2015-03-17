using ActiveDirectoryComputerInfoUpdater.Logic;
using Mach.Wpf.Mvvm;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Windows.Input;
using System;
using System.Threading.Tasks;

namespace ActiveDirectoryComputerInfoUpdater.ViewModel
{
    public class WindowViewModel : NotifyPropertyBase
    {
        private bool _usersLoading = false;
        private bool _organizationalUnitsLoading = false;
        private OrganizationalUnitsTreeViewModel _organizationalUnits;
        private DelegateCommand _loadOrganizationalUnitsCommand;
        private DelegateCommand _organizationalUnitChangedCommand;
        private DelegateCommand _loadUsersCommand;
        private DelegateCommand _queryLoggedUsersCommand;
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
        public ICommand QueryLoggedUsersCommand
        {
            get { return _queryLoggedUsersCommand; }
        }

        public WindowViewModel()
        {
            _loadOrganizationalUnitsCommand = new DelegateCommand(LoadOrganizationalUnits, CanLoadOrganizationalUnits);
            _organizationalUnitChangedCommand = new DelegateCommand(OrganizationalUnitChanged);
            _loadUsersCommand = new DelegateCommand(LoadUsers, CanExecuteLoadUsers);
            _queryLoggedUsersCommand = new DelegateCommand(QueryLoggedUsers, CanExecuteQueryLoggedUsers);

            _selectedOrganizationalUnit = new OrganizationalUnitViewModel(null); //dummy selected item


            LoadUsers();
            LoadOrganizationalUnits();
        }

        private bool CanLoadOrganizationalUnits(object parameter)
        {
            return _organizationalUnitsLoading == false;
        }

        private bool CanExecuteLoadUsers(object parameter)
        {
            return _usersLoading == false;
        }

        private bool CanExecuteQueryLoggedUsers(object parameter)
        {
            return _users != null && _selectedOrganizationalUnit != null && _selectedOrganizationalUnit.Entry != null;
        }

        private void QueryLoggedUsers()
        {
            if (_selectedOrganizationalUnit != null)
            {
                _selectedOrganizationalUnit.DetectLoggedUsers(_users);
            }
        }

        public void LoadUsers()
        {
            Task.Run(() =>
            {
                return DoLoadUsers();
            })
            .ContinueWith(users =>
            {
                Users = users.Result;

                CommandManager.InvalidateRequerySuggested();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private ObservableCollection<UserViewModel> DoLoadUsers()
        {
            _usersLoading = true;

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

            _usersLoading = false;

            return users;
        }

        private void OrganizationalUnitChanged()
        {
            _selectedOrganizationalUnit.LoadComputers(_users);
        }

        private void LoadOrganizationalUnits()
        {
            Task.Run(() =>
            {
                return DoLoadOrganizationalUnits();
            })
            .ContinueWith(units =>
            {
                OrganizationalUnits = units.Result;

                CommandManager.InvalidateRequerySuggested();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private OrganizationalUnitsTreeViewModel DoLoadOrganizationalUnits()
        {
            _organizationalUnitsLoading = true;

            OrganizationalUnitsTreeViewModel organizationalUnits = new OrganizationalUnitsTreeViewModel();
            organizationalUnits.LoadTree();

            _organizationalUnitsLoading = false;

            return organizationalUnits;
        }
    }
}
