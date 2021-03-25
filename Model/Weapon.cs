using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace dcsdbeditor.Model
{
    public class Weapon
    {
        public Weapon()
        {
            name = "";
            description = "";
            data = new Dictionary<string, string>();
            aircraft = new ObservableCollection<TinyAircraftWithInstructions>();
        }

        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public Dictionary<string, string> data { get; set; }

        public ObservableCollection<TinyAircraftWithInstructions> aircraft { get; set; }

        public void AddAircraft(TinyObject plane)
        {
            if(!aircraft.Any(x=>x.id==plane.id))
            {
                aircraft.Add(new TinyAircraftWithInstructions { id = plane.id, name = plane.name });
            }
        }

        public void RemoveAircraft(TinyAircraftWithInstructions plane)
        {
            aircraft.Remove(plane);
        }
    }
}
