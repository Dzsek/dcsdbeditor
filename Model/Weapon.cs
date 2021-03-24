using System;
using System.Collections.Generic;
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
            aircraft = new List<TinyAircraftWithInstructions>();
        }

        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public Dictionary<string, string> data { get; set; }

        public List<TinyAircraftWithInstructions> aircraft { get; set; }
    }
}
