using Mach.Wpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryComputerInfoUpdater.ViewModel
{
    public class UserDetectionProgressViewModel : NotifyPropertyBase
    {
        private int _computersToProcess;
        private int _computersProcessed;

        public int ComputersToProcess
        {
            get { return _computersToProcess; }
            set
            {
                _computersToProcess = value;
                OnPropertyChanged();
            }
        }
        public int ComputersProcessed
        {
            get { return _computersProcessed; }
            set
            {
                _computersProcessed = value;
                OnPropertyChanged();
            }
        }
    }
}
