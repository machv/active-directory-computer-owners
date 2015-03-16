using ActiveDirectoryComputerInfoUpdater.Logic;
using Mach.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryComputerInfoUpdater.ViewModel
{
    public class OrganizationalUnitsTreeViewModel : NotifyPropertyBase
    {
        private ObservableCollection<OrganizationalUnitViewModel> _root;
        private OrganizationalUnitViewModel _selectedOrganizationalUnit;

        public ObservableCollection<OrganizationalUnitViewModel> Root
        {
            get { return _root; }
            set
            {
                _root = value;
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

        public void LoadTree()
        {
            DirectoryEntry rootEntry = ActiveDirectory.GetDirectoryEntry();
            OrganizationalUnitViewModel root = new OrganizationalUnitViewModel(rootEntry);
            root.Name = "AD";
            root.LoadChildren(true);

            ObservableCollection<OrganizationalUnitViewModel> units = new ObservableCollection<OrganizationalUnitViewModel>();
            units.Add(root);

            Root = units;
        }
    }
}
