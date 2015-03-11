using ActiveDirectoryComputerInfoUpdater.Logic;
using Mach.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryComputerInfoUpdater.ViewModel
{
    public class OrganizationalUnitsTreeViewModel : NotifyPropertyBase
    {
        private OrganizationalUnitViewModel _root;

        public OrganizationalUnitViewModel Root
        {
            get { return _root; }
            set
            {
                _root = value;
                OnPropertyChanged();
            }
        }

        public void LoadTree()
        {
            DirectoryEntry rootEntry = ActiveDirectory.GetDirectoryEntry();
            OrganizationalUnitViewModel root = new OrganizationalUnitViewModel(rootEntry);
            root.Name = "AD";
            root.LoadChildren(true);

            Root = root;
        }
    }
}
