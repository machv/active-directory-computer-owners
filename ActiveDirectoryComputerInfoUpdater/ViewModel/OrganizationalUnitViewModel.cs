using ActiveDirectoryComputerInfoUpdater.Logic;
using ActiveDirectoryComputerInfoUpdater.Properties;
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
        private UserDetectionProgressViewModel _detectionProgress;

        public UserDetectionProgressViewModel DetectionProgress
        {
            get { return _detectionProgress; }
            set
            {
                _detectionProgress = value;
                OnPropertyChanged();
            }
        }
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
            _detectionProgress = new UserDetectionProgressViewModel();
            _computers = new ObservableCollection<ComputerViewModel>();
            _children = new ObservableCollection<OrganizationalUnitViewModel>();

            Entry = entry;
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

        public void DetectLoggedUsers(ObservableCollection<UserViewModel> users)
        {
            string domain = Settings.Default.Domain;
            if (string.IsNullOrEmpty(Settings.Default.Domain))
            {
                string fqdn = ActiveDirectory.GetDomainName();
                domain = ActiveDirectory.GetNetbiosNameForDomain(fqdn);
            }

            DetectionProgress = new UserDetectionProgressViewModel();
            DetectionProgress.ComputersToProcess = _computers.Count;

            foreach (ComputerViewModel computer in _computers)
            {
                Task.Run(() =>
                {
                    return Remote.DetectLogin(computer.Name, Settings.Default.User, Settings.Default.Password, domain);
                })
                .ContinueWith(result =>
                {
                    string login = result.Result;

                    if (!string.IsNullOrEmpty(login))
                    {
                        if (login.Contains("\\"))
                        {
                            login = login.Substring(login.IndexOf("\\") + 1);
                        }

                        computer.DetectedUser = users.Where(u => u.SamAccountName == login).FirstOrDefault();
                    }

                    DetectionProgress.ComputersProcessed += 1;
                });
            }
        }

        public void LoadComputers()
        {
            LoadComputers(null);
        }

        public void LoadComputers(ObservableCollection<UserViewModel> users)
        {
            Computers.Clear();

            using (SearchResultCollection computers = ActiveDirectory.GetComputersInOrganizationalUnit(Entry))
            {
                foreach (SearchResult result in computers)
                {
                    UserViewModel owner = null;
                    if (result.Properties.Contains("managedBy"))
                    {
                        if (users != null)
                        {
                            owner = users.Where(u => u.DistinguishedName == result.Properties["managedBy"][0] as string).FirstOrDefault();
                        }
                    }

                    ComputerViewModel computer = new ComputerViewModel(result.GetDirectoryEntry(), owner);
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
