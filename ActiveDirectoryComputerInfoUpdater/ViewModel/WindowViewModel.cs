using Mach.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryComputerInfoUpdater.ViewModel
{
    public class WindowViewModel : NotifyPropertyBase
    {
        private OrganizationalUnitsTreeViewModel _organizationalUnits;

        public OrganizationalUnitsTreeViewModel OrganizationalUnits
        {
            get { return _organizationalUnits; }
            set
            {
                _organizationalUnits = value;
                OnPropertyChanged();
            }
        }

        public WindowViewModel()
        {

        }
    }
}
