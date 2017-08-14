using System;
using System.Collections.Generic;
using System.Drawing;

namespace EveJimaUniverse
{
    public class StarSystemEntity1 : ICloneable
    {
        public System Information = new System();
        
        public int JumpsCountFromLocation { get; set; }

        public DateTime LastVisit { get; set; }

        public DateTime Created { get; set; }

        public List<string> ConnectedSolarSystems = new List<string>();

        public Point LocationInMap { get; set; }

        public StarSystemEntity1()
        {
            
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        
    }

    public class WormholeEntity
    {
        public string Name { get; set; }
        public string LeadsTo { get; set; }
        public string TotalMass { get; set; }
        public string SingleMass { get; set; }
        public string Regen { get; set; }
        public string Classification { get; set; }
        public string Lifetime { get; set; }

    }

    public class BasicSolarSystem
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class BasicCosmicSignature
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class BaseSolarSystem
    {
        public string System { get; set; }
        public string Region { get; set; }

        public string Coordinates { get; set; }
        public double SecurityRating { get; set; }

        public int AsteroidBelts { get; set; }
        public int Stations { get; set; }
        public int IceBelts { get; set; }


    }

    public class StarSystem 
    {
        public string Id { get; set; }
        public string SolarSystemName { get; set; }
        public string Class { get; set; }
        public string Static { get; set; }
        public string Static2 { get; set; }
        public string Sun { get; set; }
        public string Planets { get; set; }
        public string Moons { get; set; }
        public string Effect { get; set; }
        public string Region { get; set; }
        public string Constelation { get; set; }
    }

}
