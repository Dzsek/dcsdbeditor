using dcsdbeditor.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
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

        private ObservableCollection<Aircraft> _aircraft;
        private ObservableCollection<Weapon> _weapons;
        private ObservableCollection<TinyAircraft> _aircraftList;
        private ObservableCollection<TinyWeapon> _weaponList;
        private TinyAircraft _selectedTinyAircraft;
        private TinyWeapon _selectedWeaponToAdd;
        private TinyWeapon _selectedAircraftWeapon;
        private TinyWeapon _selectedTinyWeapon;
        private TinyAircraft _selectedAircraftToAdd;
        private TinyAircraftWithInstructions _selectedWeaponAircraft;
        private int _selectedWeaponInstructionIndex;
        private string _aircraftWeaponFilterText = "";
        private string _weaponAircraftFilterText = "";
        private KeyValuePair<string, string>? _selectedWeaponData;

        public MainVM()
        {
            AddWeaponToAircraft = new MyCommand(AddWeaponToAircraftExecute);
            RemoveWeaponFromAircraft = new MyCommand(RemoveWeaponFromAircraftExecute);
            AddAircraftToWeapon = new MyCommand(AddAircraftToWeaponExecute);
            RemoveAircraftFromWeapon = new MyCommand(RemoveAircraftFromWeaponExecute);
            AddAircraftInstruction = new MyCommand(AddAircraftInstructionExecute);
            RemoveAircraftInstruction = new MyCommand(RemoveAircraftInstructionExecute);
            AddTagToAircraft = new MyCommand(AddTagToAircraftExecute);
            RemoveTagFromAircraft = new MyCommand(RemoveTagFromAircraftExecute);
            AddTagToWeapon = new MyCommand(AddTagToWeaponExecute);
            RemoveTagFromWeapon = new MyCommand(RemoveTagFromWeaponExecute);
            SaveDataToDisk = new MyCommand(SaveDataToDiskExecute);
            AddNewAircraft = new MyCommand(AddNewAircraftExecute);
            RemoveAircraft = new MyCommand(RemoveAircraftExecute);
            AddNewWeapon = new MyCommand(AddNewWeaponExecute);
            RemoveWeapon = new MyCommand(RemoveWeaponExecute);
            AddWeaponData = new MyCommand(AddWeaponDataExecute);
            RemoveWeaponData = new MyCommand(RemoveWeaponDataExecute);

            InitializeDataDirectory();
            Load();

            FilteredAircraft = new ListCollectionView(_aircraftList);
            FilteredAircraft.Filter += (item) =>
            {
                var plane = item as TinyObject;
                if (SelectedWeapon != null)
                {
                    if (SelectedWeapon.aircraft.Any(x => x.id == plane.id))
                    {
                        return false;
                    }
                }

                return plane.name.ToLowerInvariant().Replace("-", "").Replace("/", "").Contains(WeaponAircraftFilterText.ToLower());
            };

            FilteredWeapons = new ListCollectionView(_weaponList);
            FilteredWeapons.Filter += (item) =>
            {
                var weapon = item as TinyObject;
                if (SelectedAircraft != null)
                {
                    if (SelectedAircraft.weapons.Any(x => x.id == weapon.id))
                    {
                        return false;
                    }
                }

                return weapon.name.ToLowerInvariant().Replace("-", "").Replace("/", "").Contains(AircraftWeaponFilterText.ToLower());
            };

            WCProvider = new WeaponCategoryProvider();
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
            if (SelectedAircraft != null && SelectedWeaponToAdd != null)
            {
                SelectedAircraft.AddWeapon(SelectedWeaponToAdd);
                FilteredWeapons.Refresh();
            }
        }

        private void RemoveWeaponFromAircraftExecute(object obj)
        {
            if (SelectedAircraft != null && SelectedAircraftWeapon != null)
            {
                SelectedAircraft.RemoveWeapon(SelectedAircraftWeapon);
                SelectedAircraftWeapon = null;
            }
        }

        private void AddAircraftToWeaponExecute(object obj)
        {
            if (SelectedWeapon != null && SelectedAircraftToAdd != null)
            {
                SelectedWeapon.AddAircraft(SelectedAircraftToAdd);
                FilteredAircraft.Refresh();
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

        private void AddTagToAircraftExecute(object obj)
        {
            if (SelectedTinyAircraft != null)
            {
                SelectedTinyAircraft.tags.Add((string)obj);
            }
        }

        private void RemoveTagFromAircraftExecute(object obj)
        {
            if (SelectedTinyAircraft != null)
            {
                SelectedTinyAircraft.tags.Remove((string)obj);
            }
        }

        private void AddTagToWeaponExecute(object obj)
        {
            if (SelectedTinyWeapon != null)
            {
                SelectedTinyWeapon.tags.Add((string)obj);
            }
        }

        private void RemoveTagFromWeaponExecute(object obj)
        {
            if (SelectedTinyWeapon != null)
            {
                SelectedTinyWeapon.tags.Remove((string)obj);
            }
        }

        private void AddNewAircraftExecute(object obj)
        {
            var id = (string)obj;
            if (!string.IsNullOrWhiteSpace(id))
            {
                _aircraftList.Add(new TinyAircraft { id = id, name = id });
                _aircraft.Add(new Model.Aircraft { id = id, description = id, name = id });
            }
        }

        private void RemoveAircraftExecute(object obj)
        {
            var ta = obj as TinyAircraft;
            var ar = _aircraft.FirstOrDefault(x => x.id == ta.id);
            _aircraft.Remove(ar);
            _aircraftList.Remove(ta);
        }

        private void AddNewWeaponExecute(object obj)
        {
            var id = (string)obj;
            if (!string.IsNullOrWhiteSpace(id))
            {
                _weaponList.Add(new TinyWeapon { id = id, name = id });
                var wp = new Model.Weapon { id = id, description = id, name = id };
                wp.data.Add("Weight", "-");
                wp.data.Add("Range", "-");
                wp.data.Add("Guidance", "-");
                wp.data.Add("Type", "-");
                wp.data.Add("Warhead", "-");
                wp.data.Add("Targets", "-");
                _weapons.Add(wp);
            }
        }

        private void AddWeaponDataExecute(object obj)
        {
            if (SelectedWeapon != null)
            {
                SelectedWeapon.data.Add((string)obj, "new");
            }
        }

        private void RemoveWeaponDataExecute(object obj)
        {
            var data = (KeyValuePair<string, string>)obj;
            if (SelectedWeapon != null)
            {
                SelectedWeapon.data.Remove(data.Key);
            }
        }

        private void RemoveWeaponExecute(object obj)
        {
            var tw = obj as TinyWeapon;
            var wp = _weapons.FirstOrDefault(x => x.id == tw.id);
            _weapons.Remove(wp);
            _weaponList.Remove(tw);
        }

        public KeyValuePair<string, string>? SelectedWeaponData
        {
            get
            {
                return _selectedWeaponData;
            }

            set
            {
                _selectedWeaponData = value;
                Notify(nameof(SelectedWeaponData));
                Notify(nameof(SelectedWeaponDataValue));
            }
        }

        public string SelectedWeaponDataValue
        {
            get
            {
                if (_selectedWeaponData.HasValue)
                {
                    return _selectedWeaponData.Value.Value;
                }

                return "";
            }

            set
            {
                if (SelectedWeapon != null && _selectedWeaponData.HasValue)
                {
                    SelectedWeapon.data[_selectedWeaponData.Value.Key] = value;
                    SelectedWeaponData = SelectedWeapon.data.FirstOrDefault(x => x.Key == _selectedWeaponData.Value.Key);
                    SelectedWeapon.data.Refresh();
                }

                Notify(nameof(SelectedWeapon));
                Notify(nameof(SelectedWeaponDataValue));
            }
        }

        private void Consolidate()
        {
            foreach (var p in _aircraftList)
            {
                foreach (var w in _weapons)
                {
                    foreach (var wp in w.aircraft)
                    {
                        if (wp.id == p.id)
                        {
                            wp.name = p.name;
                        }
                    }
                }
            }

            foreach (var w in _weaponList)
            {
                foreach (var p in _aircraft)
                {
                    foreach (var pw in p.weapons)
                    {
                        if (pw.id == w.id)
                        {
                            pw.category = w.category;
                            pw.name = w.name;
                        }
                    }
                }
            }

            foreach (var wp in _weapons)
            {
                var tiny = _weaponList.FirstOrDefault(x => x.id == wp.id);
                foreach (var a in wp.aircraft)
                {
                    var air = _aircraft.FirstOrDefault(x => x.id == a.id);
                    if (air != null && tiny != null)
                    {
                        if (!air.weapons.Any(x => x.id == tiny.id))
                        {
                            air.weapons.Add(new TinyWeapon { id = tiny.id, name = tiny.name, category = tiny.category });
                        }
                    }
                }
            }

            foreach (var ap in _aircraft)
            {
                var tiny = _aircraftList.FirstOrDefault(x => x.id == ap.id);
                foreach (var w in ap.weapons)
                {
                    var weap = _weapons.FirstOrDefault(x => x.id == w.id);
                    if (weap != null && tiny != null)
                    {
                        if (!weap.aircraft.Any(x => x.id == tiny.id))
                        {
                            weap.aircraft.Add(new TinyAircraftWithInstructions { id = tiny.id, name = tiny.name });
                        }
                    }
                }
            }
        }

        private void SaveDataToDiskExecute(object obj)
        {
            Consolidate();

            var result = MessageBox.Show("Save?", "Save?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            var now = DateTime.Now;
            Directory.Move("data", $"data_backup_{now.Year}_{now.Month}_{now.Day}_{now.Hour}_{now.Minute}_{now.Second}");

            Directory.CreateDirectory(AircraftDataFolder);
            Directory.CreateDirectory(WeaponDataFolder);

            var weaponsfile = JsonConvert.SerializeObject(_weaponList.OrderBy(x => x.id), Formatting.Indented);
            var aircraftFile = JsonConvert.SerializeObject(_aircraftList.OrderBy(x => x.id), Formatting.Indented);
            File.WriteAllText(AircraftJSON, aircraftFile);
            File.WriteAllText(WeaponJSON, weaponsfile);

            foreach (var plane in _aircraft)
            {
                var planeFile = JsonConvert.SerializeObject(plane, Formatting.Indented);
                Directory.CreateDirectory($"{AircraftDataFolder}/{plane.id}");
                File.WriteAllText($"{AircraftDataFolder}/{plane.id}/data.json", planeFile);
            }

            foreach (var weapon in _weapons)
            {
                var weaponFile = JsonConvert.SerializeObject(weapon, Formatting.Indented);
                Directory.CreateDirectory($"{WeaponDataFolder}/{weapon.id}");
                File.WriteAllText($"{WeaponDataFolder}/{weapon.id}/data.json", weaponFile);
            }

            MessageBox.Show("Saved");
        }

        public void SelectNextWeaponData()
        {
            var found = false;
            if (SelectedWeaponData.HasValue && SelectedWeapon != null)
            {
                var list = SelectedWeapon.data.ToList();
                for (var i = 0; i < list.Count; i++)
                {
                    if (list[i].Key == SelectedWeaponData.Value.Key)
                    {
                        if (i + 1 >= list.Count)
                        {
                            SelectedWeaponData = list[0];
                        }
                        else
                        {
                            SelectedWeaponData = list[i + 1];
                        }

                        found = true;
                        break;
                    }
                }
            }

            if (!found && SelectedWeapon != null && SelectedWeapon.data.Count != 0)
            {
                SelectedWeaponData = SelectedWeapon.data.ToList()[0];
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
                ////if (weap.data.Count == 0)
                ////{
                ////    weap.data.Add("Weight", "-");
                ////    weap.data.Add("Range", "-");
                ////    weap.data.Add("Guidance", "-");
                ////    weap.data.Add("Type", "-");
                ////    weap.data.Add("Warhead", "-");
                ////    weap.data.Add("Targets", "-");
                ////}

                weaponsFull.Add(weap);
            }

            _aircraft = new ObservableCollection<Aircraft>(aircraftFull);
            _aircraftList = new ObservableCollection<TinyAircraft>(aircraft.OrderBy(x => x.name).ToList());
            _weapons = new ObservableCollection<Weapon>(weaponsFull);
            _weaponList = new ObservableCollection<TinyWeapon>(weapons.OrderBy(x => x.name).ToList());
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

        public WeaponCategoryProvider WCProvider { get; set; }

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

        public ICommand AddTagToAircraft { get; set; }

        public ICommand RemoveTagFromAircraft { get; set; }

        public ICommand AddTagToWeapon { get; set; }

        public ICommand RemoveTagFromWeapon { get; set; }

        public ICommand SaveDataToDisk { get; set; }

        public ICommand AddNewAircraft { get; set; }
        public ICommand RemoveAircraft { get; set; }
        public ICommand AddNewWeapon { get; set; }
        public ICommand RemoveWeapon { get; set; }

        public ICommand AddWeaponData { get; set; }
        public ICommand RemoveWeaponData { get; set; }

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

        public TinyWeapon SelectedAircraftWeapon
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

        public TinyAircraft SelectedTinyAircraft
        {
            get
            {
                return _selectedTinyAircraft;
            }

            set
            {
                _selectedTinyAircraft = value;
                WCProvider.SetAircraft(SelectedAircraft);
                WCProvider.Refresh();
                FilteredWeapons.Refresh();
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
                FilteredAircraft.Refresh();
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

        public ObservableCollection<Aircraft> Aircraft
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

        public ObservableCollection<Weapon> Weapons
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

        public ObservableCollection<TinyAircraft> AircraftList
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

        public ObservableCollection<TinyWeapon> WeaponList
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
