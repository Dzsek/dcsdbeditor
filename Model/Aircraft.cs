using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace dcsdbeditor.Model
{
    public class Aircraft : INotifyPropertyChanged
    {

        public Aircraft()
        {
            name = "";
            description = "";
            weapons = new ObservableCollection<TinyWeapon>();
        }

        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public ObservableCollection<TinyWeapon> weapons { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RemoveWeapon(TinyWeapon weapon)
        {
            weapons.Remove(weapon);
        }

        public void AddWeapon(TinyWeapon weapon)
        {
            weapons.Add(new TinyWeapon { id = weapon.id, name = weapon.name, category = weapon.category });
        }
    }
}
