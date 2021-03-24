using System;
using System.Collections.Generic;
using System.Text;

namespace dcsdbeditor.Model
{
    public class TinyAircraftWithInstructions : TinyObject
    {
        public TinyAircraftWithInstructions(): base()
        {
            instructions = new List<string>();
        }

        public List<string> instructions { get; set; }
    }
}
