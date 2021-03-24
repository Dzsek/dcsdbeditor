using System;
using System.Collections.Generic;
using System.Text;

namespace dcsdbeditor.Model
{
    public class TinyWeapon : TinyObject
    {
        public TinyWeapon():base()
        {
            tags = new List<string>();
        }

        public List<string> tags { get; set; }
    }
}
