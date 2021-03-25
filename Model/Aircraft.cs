using System;
using System.Collections.Generic;
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
            weapons = new WeaponGroups();
        }

        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public WeaponGroups weapons { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RemoveWeapon(string category, TinyObject weapon)
        {
            switch (category)
            {
                case "aam":
                    weapons.aam.Remove(weapon);
                    break;
                case "agm":
                    weapons.agm.Remove(weapon);
                    break;
                case "bomb":
                    weapons.bomb.Remove(weapon);
                    break;
                case "fuel":
                    weapons.fuel.Remove(weapon);
                    break;
                case "pod":
                    weapons.pod.Remove(weapon);
                    break;
                case "rocket":
                    weapons.rocket.Remove(weapon);
                    break;
            }
        }

        public void AddWeapon(string category, TinyObject weapon)
        {
            switch (category)
            {
                case "aam":
                    if(!weapons.aam.Any(x=>x.id==weapon.id))
                    {
                        weapons.aam.Add(new TinyObject { id = weapon.id, name = weapon.name });
                    }
                    break;
                case "agm":
                    if (!weapons.agm.Any(x => x.id == weapon.id))
                    {
                        weapons.agm.Add(new TinyObject { id = weapon.id, name = weapon.name });
                    }
                    break;
                case "bomb":
                    if (!weapons.bomb.Any(x => x.id == weapon.id))
                    {
                        weapons.bomb.Add(new TinyObject { id = weapon.id, name = weapon.name });
                    }
                    break;
                case "fuel":
                    if (!weapons.fuel.Any(x => x.id == weapon.id))
                    {
                        weapons.fuel.Add(new TinyObject { id = weapon.id, name = weapon.name });
                    }
                    break;
                case "pod":
                    if (!weapons.pod.Any(x => x.id == weapon.id))
                    {
                        weapons.pod.Add(new TinyObject { id = weapon.id, name = weapon.name });
                    }
                    break;
                case "rocket":
                    if (!weapons.rocket.Any(x => x.id == weapon.id))
                    {
                        weapons.rocket.Add(new TinyObject { id = weapon.id, name = weapon.name });
                    }
                    break;
            }
        }
    }
}
