using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace dcsdbeditor.Model
{
    public class WeaponGroups
    {
        public WeaponGroups()
        {
            aam = new ObservableCollection<TinyObject>();
            agm = new ObservableCollection<TinyObject>();
            bomb = new ObservableCollection<TinyObject>();
            fuel = new ObservableCollection<TinyObject>();
            pod = new ObservableCollection<TinyObject>();
            rocket = new ObservableCollection<TinyObject>();
        }

        public ObservableCollection<TinyObject> aam { get; set; }
        public ObservableCollection<TinyObject> agm { get; set; }
        public ObservableCollection<TinyObject> bomb { get; set; }
        public ObservableCollection<TinyObject> fuel { get; set; }
        public ObservableCollection<TinyObject> pod { get; set; }
        public ObservableCollection<TinyObject> rocket { get; set; }
    }
}
