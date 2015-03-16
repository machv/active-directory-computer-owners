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
        public string DN { get; set; }
        public ObservableCollection<OrganizationalUnitViewModel> ChildOrganizationalUnits
        {
            get { return _children; }
            set
            {
                _children = value;
                OnPropertyChanged();
            }
        }

        public OrganizationalUnitViewModel(DirectoryEntry entry)
        {
            ChildOrganizationalUnits = new ObservableCollection<OrganizationalUnitViewModel>();
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
                            unit.DN = GetDistinguishedName(child.Path);

                            if (recursive)
                                unit.LoadChildren();

                            ChildOrganizationalUnits.Add(unit);
                        }
                    }
                }
            }
        }

        private string GetDistinguishedName(string path)
        {
            int i = path.IndexOf('/', 7);
            if (i >= 0 && path.Length > i + 1)
            {
                return path.Substring(i + 1);
            }

            return path;
        }
    }
}
