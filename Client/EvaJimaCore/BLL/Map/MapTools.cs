using System.Text.RegularExpressions;
using EvaJimaCore;
using EveJimaUniverse;

namespace EveJimaCore.BLL.Map
{
    public class MapTools
    {
        public static void Normalization(Map map)
        {
            foreach ( var solarSystem in map.Systems )
            {
                if (solarSystem.Name != null)
                {
                    solarSystem.Type = UpdateSystemType(map, solarSystem.Name);
                }
            }
        }

        private static bool IsWSpaceSystem(string systemName)
        {
            var numbersInSystemName = Regex.Match(systemName, @"\d+").Value;

            if (numbersInSystemName == "") return false;

            return systemName.Replace(numbersInSystemName, "") == "J";
        }

        private static string UpdateSystemType(Map map, string system)
        {
            if (IsWSpaceSystem(system))
            {
                return "A";
            }

            var systemPrevious = map.GetSystem(system);

            var isNeedAddSolarSystemToMap = false;

            foreach (var connected in systemPrevious.Connections)
            {
                if (IsWSpaceSystem(connected))
                {
                    return "B";
                }
            }

            foreach (var connected in systemPrevious.Connections)
            {
                var connectedSystem = map.GetSystem(connected);

                foreach (var connectedOfConnected in connectedSystem.Connections)
                {
                    if (IsWSpaceSystem(connectedOfConnected))
                    {
                        return "C";
                    }
                }
            }

            return "D";
        }

        public static SecurityStatus GetConnectionType(SolarSystem solarSystemFrom, SolarSystem solarSystemTo)
        {
            var _solarSystemFromInfo = Global.Space.GetSolarSystem(solarSystemFrom.Name);
            var _solarSystemToInfo = Global.Space.GetSolarSystem(solarSystemTo.Name);

            if ( _solarSystemFromInfo.Security == SecurityStatus.WSpace || _solarSystemToInfo.Security == SecurityStatus.WSpace )
            {
                return SecurityStatus.WSpace;
            }

            if (_solarSystemFromInfo.Security == SecurityStatus.Nullsec || _solarSystemToInfo.Security == SecurityStatus.Nullsec)
            {
                return SecurityStatus.Nullsec;
            }

            if (_solarSystemFromInfo.Security == SecurityStatus.Lowsec || _solarSystemToInfo.Security == SecurityStatus.Lowsec)
            {
                return SecurityStatus.Lowsec;
            }

            return SecurityStatus.Highsec;
        }
    }
}
