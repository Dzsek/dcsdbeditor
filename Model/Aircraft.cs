using System;
using System.Collections.Generic;
using System.Text;

namespace dcsdbeditor.Model
{
    public class Aircraft
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
    }
}
