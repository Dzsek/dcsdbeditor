using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace dcsdbeditor.Model
{
    public class TinyWeapon : TinyObject
    {
        public TinyWeapon():base()
        {
            tags = new ObservableCollection<string>();
        }

        public ObservableCollection<string> tags { get; set; }
    }
}
