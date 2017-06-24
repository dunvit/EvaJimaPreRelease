using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EveJimaUniverse;

namespace EveJimaServerMap
{
    public class MapInformation
    {
        public string Key { get; set; }

        public string Owner { get; set; }

        public List<SolarSystem> SystemsForSave { get; set; }
    }
}
