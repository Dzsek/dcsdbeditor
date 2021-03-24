using System;
using System.Collections.Generic;
using System.Text;

namespace dcsdbeditor.Model
{
    public class WeaponGroups
    {
        public WeaponGroups()
        {
            aam = new List<TinyObject>();
            agm = new List<TinyObject>();
            bomb = new List<TinyObject>();
            fuel = new List<TinyObject>();
            pod = new List<TinyObject>();
            rocket = new List<TinyObject>();
        }

        public List<TinyObject> aam { get; set; }
        public List<TinyObject> agm { get; set; }
        public List<TinyObject> bomb { get; set; }
        public List<TinyObject> fuel { get; set; }
        public List<TinyObject> pod { get; set; }
        public List<TinyObject> rocket { get; set; }
    }
}
