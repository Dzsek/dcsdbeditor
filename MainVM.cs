using dcsdbeditor.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
        private TinyWeapon _selectedWeaponToAdd;
        private TinyObject _selectedAircraftWeapon;
        private TabItem _selectedTab;
        private TinyWeapon _selectedTinyWeapon;
        private TinyAircraft _selectedAircraftToAdd;
        private TinyAircraftWithInstructions _selectedWeaponAircraft;
        private int _selectedWeaponInstructionIndex;
        private string _aircraftWeaponFilterText = "";
        private string _weaponAircraftFilterText = "";

        public MainVM()
        {
            AddWeaponToAircraft = new MyCommand(AddWeaponToAircraftExecute);
            RemoveWeaponFromAircraft = new MyCommand(RemoveWeaponFromAircraftExecute);
            AddAircraftToWeapon = new MyCommand(AddAircraftToWeaponExecute);
            RemoveAircraftFromWeapon = new MyCommand(RemoveAircraftFromWeaponExecute);
            AddAircraftInstruction = new MyCommand(AddAircraftInstructionExecute);
            RemoveAircraftInstruction = new MyCommand(RemoveAircraftInstructionExecute);

            InitializeDataDirectory();
            Load();

            FilteredAircraft = new ListCollectionView(_aircraftList);
            FilteredAircraft.Filter += (item) =>
            {
                var plane = item as TinyObject;
                return plane.name.ToLowerInvariant().Replace("-", "").Replace("/", "").Contains(WeaponAircraftFilterText.ToLower());
            };

            FilteredWeapons = new ListCollectionView(_weaponList);
            FilteredWeapons.Filter += (item) =>
            {
                var weapon = item as TinyObject;
                return weapon.name.ToLowerInvariant().Replace("-", "").Replace("/", "").Contains(AircraftWeaponFilterText.ToLower());
            };
        }

        private void AddAircraftInstructionExecute(object obj)
        {
            if (SelectedWeaponAircraft != null)
            {
                SelectedWeaponAircraft.instructions.Add("new");
            }
        }

        private void RemoveAircraftInstructionExecute(object obj)
        {
            if (SelectedWeaponInstructionIndex >= 0)
            {
                SelectedWeaponAircraft.instructions.RemoveAt(SelectedWeaponInstructionIndex);
                SelectedWeaponInstructionIndex = -2;
            }
        }


        private void AddWeaponToAircraftExecute(object obj)
        {
            if (SelectedTab != null && SelectedAircraft != null && SelectedWeaponToAdd != null)
            {
                SelectedAircraft.AddWeapon((string)SelectedTab.Header, SelectedWeaponToAdd);
            }
        }

        private void RemoveWeaponFromAircraftExecute(object obj)
        {
            if (SelectedTab != null && SelectedAircraft != null && SelectedAircraftWeapon != null)
            {
                SelectedAircraft.RemoveWeapon((string)SelectedTab.Header, SelectedAircraftWeapon);
                SelectedAircraftWeapon = null;
            }
        }

        private void AddAircraftToWeaponExecute(object obj)
        {
            if (SelectedWeapon != null && SelectedAircraftToAdd != null)
            {
                SelectedWeapon.AddAircraft(SelectedAircraftToAdd);
            }
        }

        private void RemoveAircraftFromWeaponExecute(object obj)
        {
            if (SelectedWeapon != null && SelectedWeaponAircraft != null)
            {
                SelectedWeapon.RemoveAircraft(SelectedWeaponAircraft);
                SelectedWeaponAircraft = null;
            }
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
            _aircraftList = aircraft.OrderBy(x => x.name).ToList();
            _weapons = weaponsFull;
            _weaponList = weapons.OrderBy(x => x.name).ToList();
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

        public ICollectionView FilteredWeapons { get; set; }

        public ICollectionView FilteredAircraft { get; set; }

        public string WeaponAircraftFilterText
        {
            get
            {
                return _weaponAircraftFilterText;
            }

            set
            {
                _weaponAircraftFilterText = value;
                FilteredAircraft.Refresh();
                Notify(nameof(WeaponAircraftFilterText));
            }
        }

        public string AircraftWeaponFilterText
        {
            get
            {
                return _aircraftWeaponFilterText;
            }

            set
            {
                _aircraftWeaponFilterText = value;
                FilteredWeapons.Refresh();
                Notify(nameof(AircraftWeaponFilterText));
            }
        }

        public ICommand AddAircraftInstruction { get; set; }

        public ICommand RemoveAircraftInstruction { get; set; }

        public ICommand AddWeaponToAircraft { get; set; }

        public ICommand RemoveWeaponFromAircraft { get; set; }

        public ICommand AddAircraftToWeapon { get; set; }

        public ICommand RemoveAircraftFromWeapon { get; set; }

        public TinyWeapon SelectedWeaponToAdd
        {
            get
            {
                return _selectedWeaponToAdd;
            }

            set
            {
                _selectedWeaponToAdd = value;
                Notify(nameof(SelectedWeaponToAdd));
            }
        }

        public int SelectedWeaponInstructionIndex
        {
            get
            {
                return _selectedWeaponInstructionIndex;
            }

            set
            {
                if (value == -1)
                {
                    return;
                }

                _selectedWeaponInstructionIndex = value;
                Notify(nameof(SelectedWeaponInstructionIndex));
                Notify(nameof(SelectedWeaponInstruction));
            }
        }

        public string SelectedWeaponInstruction
        {
            get
            {
                if (_selectedWeaponInstructionIndex == -2)
                {
                    return "";
                }

                return _selectedWeaponAircraft?.instructions[_selectedWeaponInstructionIndex];
            }

            set
            {
                if (_selectedWeaponInstructionIndex == -2)
                {
                    return;
                }

                if (_selectedWeaponAircraft != null)
                {
                    _selectedWeaponAircraft.instructions[_selectedWeaponInstructionIndex] = value;
                }
                Notify(nameof(SelectedWeaponInstruction));
            }
        }


        public TinyAircraft SelectedAircraftToAdd
        {
            get
            {
                return _selectedAircraftToAdd;
            }

            set
            {
                _selectedAircraftToAdd = value;
                Notify(nameof(SelectedAircraftToAdd));
            }
        }

        public TinyObject SelectedAircraftWeapon
        {
            get
            {
                return _selectedAircraftWeapon;
            }

            set
            {
                _selectedAircraftWeapon = value;
                Notify(nameof(SelectedAircraftWeapon));
            }
        }

        public TinyAircraftWithInstructions SelectedWeaponAircraft
        {
            get
            {
                return _selectedWeaponAircraft;
            }

            set
            {
                _selectedWeaponAircraft = value;
                SelectedWeaponInstructionIndex = -2;
                Notify(nameof(SelectedWeaponAircraft));
            }
        }

        public TabItem SelectedTab
        {
            get
            {
                return _selectedTab;
            }

            set
            {
                _selectedTab = value;
                Notify(nameof(SelectedTab));
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

        public TinyWeapon SelectedTinyWeapon
        {
            get
            {
                return _selectedTinyWeapon;
            }

            set
            {
                _selectedTinyWeapon = value;
                SelectedWeaponInstructionIndex = -2;
                SelectedWeaponAircraft = null;
                Notify(nameof(SelectedTinyWeapon));
                Notify(nameof(SelectedWeapon));
            }
        }

        public Weapon SelectedWeapon
        {
            get
            {
                if (_selectedTinyWeapon == null)
                {
                    return null;
                }

                return _weapons.FirstOrDefault(x => x.id == _selectedTinyWeapon.id);
            }
        }


        public Aircraft SelectedAircraft
        {
            get
            {
                if (_selectedTinyAircraft == null)
                {
                    return null;
                }

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
