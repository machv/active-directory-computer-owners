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
    public class OrganizationalUnitViewModel : NotifyPropertyBase
    {
        private string _name;
        private ObservableCollection<OrganizationalUnitViewModel> _children;
        private ObservableCollection<ComputerViewModel> _computers;

        public DirectoryEntry Entry { get; set; }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public string DistinguishedName { get; set; }
        public ObservableCollection<OrganizationalUnitViewModel> ChildOrganizationalUnits
        {
            get { return _children; }
            set
            {
                _children = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ComputerViewModel> Computers
        {
            get { return _computers; }
            set
            {
                _computers = value;
                OnPropertyChanged();
            }
        }

        public OrganizationalUnitViewModel(DirectoryEntry entry)
        {
            ChildOrganizationalUnits = new ObservableCollection<OrganizationalUnitViewModel>();
            Entry = entry;
            Computers = new ObservableCollection<ComputerViewModel>();
        }

        public void LoadChildren()
        {
            LoadChildren(true);
        }

        public void LoadChildren(bool recursive)
        {
            using (SearchResultCollection children = ActiveDirectory.GetSingleLevelSubOrganizationalUnits(Entry))
            {
                foreach (SearchResult child in children)
                {
                    if (child.Properties.Contains("Name"))
                    {
                        foreach (string name in child.Properties["Name"])
                        {
                            OrganizationalUnitViewModel unit = new OrganizationalUnitViewModel(child.GetDirectoryEntry());
                            unit.Name = name;
                            unit.DistinguishedName = ActiveDirectory.GetDistinguishedNameFromPath(child.Path);

                            if (recursive)
                                unit.LoadChildren();

                            ChildOrganizationalUnits.Add(unit);
                        }
                    }
                }
            }
        }

        public void LoadComputers()
        {
            Computers.Clear();

            using (SearchResultCollection computers = ActiveDirectory.GetComputersInOrganizationalUnit(Entry))
            {
                foreach (SearchResult result in computers)
                {
                    ComputerViewModel computer = new ComputerViewModel(result.GetDirectoryEntry());
                    computer.DistinguishedName = ActiveDirectory.GetDistinguishedNameFromPath(result.Path);

                    if (result.Properties.Contains("lastLogon"))
                        computer.LastLogon = DateTime.FromFileTime((long)(result.Properties["lastLogon"][0]));

                    if (result.Properties.Contains("logonCount"))
                        computer.LogonCount = (int)result.Properties["logonCount"][0];

                    if (result.Properties.Contains("name"))
                        computer.Name = result.Properties["name"][0] as string;

                    if (result.Properties.Contains("operatingSystem"))
                        computer.OperatingSystem = result.Properties["operatingSystem"][0] as string;

                    if (result.Properties.Contains("description"))
                        computer.Description = result.Properties["description"][0] as string;

                    if (result.Properties.Contains("location"))
                        computer.Location = result.Properties["location"][0] as string;
                    
                    if (result.Properties.Contains("managedBy"))
                        computer.ManagedBy = result.Properties["managedBy"][0] as string;

                    using (SearchResultCollection results = ActiveDirectory.GetBitlockerRecoveryKeys(result.GetDirectoryEntry()))
                        computer.BitlockerRecoveryKeys = results.Count;

                    Computers.Add(computer);
                }
            }
        }
    }
}
