using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using EvaJimaCore;
using EveJimaUniverse;
using log4net;

namespace EveJimaCore.BLL
{
    public class SpaceEntity
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SpaceEntity));

        public readonly Dictionary<string, WormholeEntity> WormholeTypes = new Dictionary<string, WormholeEntity>();

        public readonly Dictionary<string, StarSystemEntity> SolarSystems = new Dictionary<string, StarSystemEntity>();

        public readonly Dictionary<string, string> BasicSolarSystems = new Dictionary<string, string>();

        public SpaceEntity()
        {
            LoadEmpireSolarSystems();

            LoadWormholeTypes();

            LoadWormholeStarSystems();

            LoadSolarSystemsIDs();
        }

        public StarSystemEntity GetSolarSystem(string systemName)
        {
            if (SolarSystems.ContainsKey(systemName))
            {
                var location = SolarSystems[systemName];

                return location.Clone() as StarSystemEntity;
            }

            return new StarSystemEntity { Id = "-1", SolarSystemName = systemName };
        }


        public StarSystemEntity SolarSystem(string systemName)
        {
            if (SolarSystems.ContainsKey(systemName))
            {
                return SolarSystems[systemName];
            }

            return new StarSystemEntity { Id = "-1", SolarSystemName = systemName };
        }

        private void LoadEmpireSolarSystems()
        {
            Log.Debug("[SpaceEntity.LoadBasicSolarSystems] Read csv file \"Data/WSpaceSystemInfo - Basic Solar Systems.csv\". ");

            try
            {
                using (var sr = new StreamReader(@"Data/WSpaceSystemInfo - Base Solar Systems.csv"))
                {
                    var records = new CsvReader(sr).GetRecords<BaseSolarSystem>();

                    foreach (var record in records)
                    {
                        var solarSystem = new StarSystemEntity
                        {
                            SolarSystemName = record.System,
                            Class = null,
                            ConnectedSolarSystems = new List<string>(),
                            Constelation = null,
                            Effect = null,
                            Moons = null,
                            Planets = null,
                            Region = record.Region,
                            Static = null,
                            Static2 = null,
                            Sun = null
                        };

                        solarSystem.Security = Tools.GetStatus(record.SecurityRating);

                        SolarSystems.Add(solarSystem.SolarSystemName.Trim(), solarSystem);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("[SpaceEntity.LoadBasicSolarSystems] Critical error = {0}", ex);
            }
        }

        private void LoadSolarSystemsIDs()
        {
            Log.Debug("[SpaceEntity.LoadBasicSolarSystems] Read csv file \"Data/WSpaceSystemInfo - Basic Solar Systems.csv\". ");

            try
            {
                using (var sr = new StreamReader(@"Data/WSpaceSystemInfo - Basic Solar Systems.csv"))
                {
                    var records = new CsvReader(sr).GetRecords<BasicSolarSystem>();

                    foreach (var record in records)
                    {
                        var solarSystem = SolarSystem(record.Name.ToUpper());

                        solarSystem.Id = record.Id;

                        


                        BasicSolarSystems.Add(record.Name.Trim().ToUpper(), record.Id.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("[SpaceEntity.LoadBasicSolarSystems] Critical error = {0}", ex);
            }


            
        }

        private void LoadWormholeTypes()
        {
            Log.Debug("[SpaceEntity.LoadWormholes] Read csv file \"Data/WSpaceSystemInfo - Wormholes.csv\". ");

            try
            {
                using (var sr = new StreamReader(@"Data/WSpaceSystemInfo - Wormholes.csv"))
                {
                    var records = new CsvReader(sr).GetRecords<WormholeEntity>();

                    foreach (var record in records)
                    {
                        WormholeTypes.Add(record.Name.Trim(), record);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("[SpaceEntity.LoadWormholes] Critical error = {0}", ex);
            }

            
        }

        private void LoadWormholeStarSystems()
        {
            Log.Debug("[SpaceEntity.LoadStarSystems] Read csv file \"Data/WSpaceSystemInfo - Systems.csv\". ");

            try
            {
                using (var sr = new StreamReader(@"Data/WSpaceSystemInfo - Systems.csv"))
                {
                    var records = new CsvReader(sr).GetRecords<StarSystem>();

                    foreach (var record in records)
                    {

                        var solarSystem = new StarSystemEntity
                        {
                            Id =  record.Id,
                            SolarSystemName = record.SolarSystemName,
                            Class = record.Class,
                            ConnectedSolarSystems = new List<string>(),
                            Constelation = record.Constelation,
                            Effect = record.Effect,
                            Moons = record.Moons,
                            Planets = record.Planets,
                            Region = record.Region,
                            Static = record.Static,
                            Static2 = record.Static2,
                            Sun = record.Sun,
                            Security = SecurityStatus.WSpace
                        };


                        SolarSystems.Add(record.SolarSystemName.Trim(), solarSystem);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("[SpaceEntity.LoadStarSystems] Critical error = {0}", ex);
            }

            
        }

        public string GetTitle(StarSystemEntity solarSystem)
        {
            var title = solarSystem.SolarSystemName + "";

            if (string.IsNullOrEmpty(solarSystem.Class) == false)
            {
                title = title + "[C" + solarSystem.Class + "]";
            }

            if (string.IsNullOrEmpty(solarSystem.Static) == false)
            {
                var wormholeI = Global.Space.WormholeTypes[solarSystem.Static.Trim()];

                title = title + " " + wormholeI.Name + "[" + wormholeI.LeadsTo + "]";
            }

            if (string.IsNullOrEmpty(solarSystem.Static2) == false)
            {
                var wormholeII = Global.Space.WormholeTypes[solarSystem.Static2.Trim()];

                title = title + " " + wormholeII.Name + "[" + wormholeII.LeadsTo + "]";
            }

            return title;
        }
    }
}
