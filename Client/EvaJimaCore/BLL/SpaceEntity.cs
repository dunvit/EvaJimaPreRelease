using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using EvaJimaCore;
using EveJimaUniverse;
using log4net;

namespace EveJimaCore.BLL
{
    public class SpaceEntity1
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SpaceEntity1));

        public Dictionary<string, WormholeEntity> WormholeTypes = new Dictionary<string, WormholeEntity>();

        public readonly Dictionary<string, EveJimaUniverse.System> SolarSystems = new Dictionary<string, EveJimaUniverse.System>();

        public readonly Dictionary<string, string> BasicSolarSystems = new Dictionary<string, string>();

        public SpaceEntity1()
        {

            LoadWormholeTypes();

            LoadSolarSystems();
        }

        private void LoadSolarSystems()
        {
            
        }

        public EveJimaUniverse.System GetSolarSystem(string systemName)
        {
            if (SolarSystems.ContainsKey(systemName))
            {
                var location = SolarSystems[systemName];

                return location.Clone() as EveJimaUniverse.System;
            }

            return null;
        }


        public EveJimaUniverse.System SolarSystem(string systemName)
        {
            if (SolarSystems.ContainsKey(systemName))
            {
                return SolarSystems[systemName];
            }

            return null;
        }

       

        private void LoadWormholeTypes()
        {
            Log.Debug("[SpaceEntity.LoadWormholes] Read csv file \"Data/WSpaceSystemInfo - Wormholes.csv\". ");

            try
            {
                var json = File.ReadAllText(@"Data/Wormholes.dat");
                var WormholeTypesAfterLoad = new Dictionary<string, WormholeEntity>();

                var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
                var ser = new DataContractJsonSerializer(WormholeTypesAfterLoad.GetType());
                WormholeTypes = ser.ReadObject(ms) as Dictionary<string, WormholeEntity>;
                ms.Close();

            }
            catch (Exception ex)
            {
                Log.ErrorFormat("[SpaceEntity.LoadWormholes] Critical error = {0}", ex);
            }
        }

        public string GetTitle(EveJimaUniverse.System solarSystem)
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
