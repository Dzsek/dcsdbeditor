using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace dcsdbeditor.Model
{
    public class TinyAircraftWithInstructions : TinyObject
    {
        public TinyAircraftWithInstructions(): base()
        {
            instructions = new ObservableCollection<string>();
        }

        public ObservableCollection<string> instructions { get; set; }
    }
}
