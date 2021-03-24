using dcsdbeditor.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace dcsdbeditor
{
    public class MainVM : INotifyPropertyChanged
    {
        private const string AircraftDataFolder = "data/aircrafts";
        private const string WeaponDataFolder = "data/weapons";
        private const string AircraftJSON = AircraftDataFolder + "/aircrafts.json";
        private const string WeaponJSON = WeaponDataFolder + "/weapons.json";

        private List<Aircraft> _aircraft;
        private List<Weapon> _weapons;
        private List<TinyAircraft> _aircraftList;
        private List<TinyWeapon> _weaponList;
        private TinyAircraft _selectedTinyAircraft;

        public MainVM()
        {
            InitializeDataDirectory();
            Load();
        }

        public void Load()
        {
            var adata = File.ReadAllText(AircraftJSON);
            var aircraft = JsonConvert.DeserializeObject<List<TinyAircraft>>(adata);
            var wdata = File.ReadAllText(WeaponJSON);
            var weapons = JsonConvert.DeserializeObject<List<TinyWeapon>>(wdata);

            var aircraftFull = new List<Aircraft>();
            foreach (var a in aircraft)
            {
                var dir = $"{AircraftDataFolder}/{a.id}";
                var file = $"{dir}/data.json";
                Directory.CreateDirectory(dir);
                if (!File.Exists(file))
                {
                    File.WriteAllText(file, JsonConvert.SerializeObject(new Aircraft()));
                }

                var rawdata = File.ReadAllText(file);
                var craft = JsonConvert.DeserializeObject<Aircraft>(rawdata);
                craft.id = a.id;
                aircraftFull.Add(craft);
            }

            var weaponsFull = new List<Weapon>();
            foreach (var w in weapons)
            {
                var dir = $"{WeaponDataFolder}/{w.id}";
                var file = $"{dir}/data.json";
                Directory.CreateDirectory(dir);
                if (!File.Exists(file))
                {
                    File.WriteAllText(file, JsonConvert.SerializeObject(new Weapon()));
                }

                var rawdata = File.ReadAllText(file);
                var weap = JsonConvert.DeserializeObject<Weapon>(rawdata);
                weap.id = w.id;
                weaponsFull.Add(weap);
            }

            _aircraft = aircraftFull;
            _aircraftList = aircraft;
            _weapons = weaponsFull;
            _weaponList = weapons;
        }

        public void InitializeDataDirectory()
        {
            Directory.CreateDirectory(AircraftDataFolder);
            Directory.CreateDirectory(WeaponDataFolder);

            if (!File.Exists(AircraftJSON))
            {
                File.WriteAllText(AircraftJSON, "[]");
            }

            if (!File.Exists(WeaponJSON))
            {
                File.WriteAllText(WeaponJSON, "[]");
            }
        }

        public TinyAircraft SelectedTinyAircraft
        {
            get
            {
                return _selectedTinyAircraft;
            }

            set
            {
                _selectedTinyAircraft = value;
                Notify(nameof(SelectedTinyAircraft));
                Notify(nameof(SelectedAircraft));
            }
        }

        public Aircraft SelectedAircraft
        {
            get
            {
                return _aircraft.FirstOrDefault(x => x.id == _selectedTinyAircraft.id);
            }
        }

        public List<Aircraft> Aircraft
        {
            get
            {
                return _aircraft;
            }

            set
            {
                _aircraft = value;
                Notify(nameof(Aircraft));
            }
        }

        public List<Weapon> Weapons
        {
            get
            {
                return _weapons;
            }

            set
            {
                _weapons = value;
                Notify(nameof(Weapons));
            }
        }

        public List<TinyAircraft> AircraftList
        {
            get
            {
                return _aircraftList;
            }

            set
            {
                _aircraftList = value;
                Notify(nameof(AircraftList));
            }
        }

        public List<TinyWeapon> WeaponList
        {
            get
            {
                return _weaponList;
            }

            set
            {
                _weaponList = value;
                Notify(nameof(WeaponList));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
