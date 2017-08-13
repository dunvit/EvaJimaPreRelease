using System.Collections.Generic;
using System.Linq;

namespace EveJimaUniverse
{
    public class Universe
    {
        public List<System> Systems = new List<System>();

        public System GetSystemById(string id)
        {
            return Systems.FirstOrDefault(system => system.Id == id);
        }

        public System GetSystemByName(string name)
        {
            return Systems.FirstOrDefault(system => system.SolarSystemName == name);
        }
    }
}
