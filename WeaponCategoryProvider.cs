using dcsdbeditor.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Data;

namespace dcsdbeditor
{
    public class WeaponCategoryProvider : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public WeaponCategoryProvider()
        {

        }

        public void SetAircraft(Aircraft aircraft)
        {
            aam = new ListCollectionView(aircraft.weapons);
            agm = new ListCollectionView(aircraft.weapons);
            bomb = new ListCollectionView(aircraft.weapons);
            fuel = new ListCollectionView(aircraft.weapons);
            pod = new ListCollectionView(aircraft.weapons);
            rocket = new ListCollectionView(aircraft.weapons);

            aam.Filter += (obj) =>
            {
                var w = obj as TinyWeapon;
                return w.category == "aam";
            };

            agm.Filter += (obj) =>
            {
                var w = obj as TinyWeapon;
                return w.category == "agm";
            };

            bomb.Filter += (obj) =>
            {
                var w = obj as TinyWeapon;
                return w.category == "bomb";
            };

            fuel.Filter += (obj) =>
            {
                var w = obj as TinyWeapon;
                return w.category == "fuel";
            };

            pod.Filter += (obj) =>
            {
                var w = obj as TinyWeapon;
                return w.category == "pod";
            };

            rocket.Filter += (obj) =>
            {
                var w = obj as TinyWeapon;
                return w.category == "rocket";
            };

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(aam)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(agm)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(bomb)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(fuel)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pod)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(rocket)));
        }

        public void Refresh()
        {
            aam.Refresh();
            agm.Refresh();
            bomb.Refresh();
            fuel.Refresh();
            pod.Refresh();
            rocket.Refresh();
        }

        public ICollectionView aam { get; set; }
        public ICollectionView agm { get; set; }
        public ICollectionView bomb { get; set; }
        public ICollectionView fuel { get; set; }
        public ICollectionView pod { get; set; }
        public ICollectionView rocket { get; set; }
    }
}
