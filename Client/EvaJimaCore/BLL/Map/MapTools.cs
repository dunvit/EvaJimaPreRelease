using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EvaJimaCore;
using EveJimaUniverse;
using log4net;

namespace EveJimaCore.BLL.Map
{
    public class MapTools
    {
        private static readonly ILog Log = LogManager.GetLogger("All");

        public static List<EveJimaUniverse.System> UpdateSolarSystems(Map map, List<EveJimaUniverse.System> updatedSystems)
        {
            Log.DebugFormat($"[MapTools.UpdateSolarSystems] start for '{map.ActivePilot}' and map '{map.Key}'");

            foreach (var updatedSystem in updatedSystems)
            {
                var system = map.GetSystem(updatedSystem.SolarSystemName);

                if (system != null)
                {
                    system.LocationInMap = updatedSystem.LocationInMap;
                    Log.DebugFormat("[MapTools.UpdateSolarSystems] For map with key {0} updated system {2} Coordinates {1}", map.Key, system.LocationInMap.X + ":" + system.LocationInMap.Y, system.SolarSystemName);
                    system.Signatures = updatedSystem.Signatures;
                    system.ConnectedSolarSystems = updatedSystem.ConnectedSolarSystems;
                }
                else
                {
                    map.Systems.Add(updatedSystem);
                    Log.DebugFormat("[MapTools.UpdateSolarSystems] For map with key {0} added system {2} Coordinates {1}", map.Key, updatedSystem.LocationInMap.X + ":" + updatedSystem.LocationInMap.Y, updatedSystem.SolarSystemName);
                }
            }

            if(updatedSystems.Count > 0) Normalization(map);

            return map.Systems;
        }

        public static void DeleteSolarSystems(Map map, List<EveJimaUniverse.System> deletedSystems)
        {
            Log.DebugFormat($"[MapTools.DeleteSolarSystems] start for '{map.ActivePilot}' and map '{map.Key}'");

            foreach (var updatedSystem in deletedSystems)
            {
                foreach (var solarSystem in map.Systems)
                {
                    var isNeedRemoveConnection = false;

                    foreach (var connection in solarSystem.ConnectedSolarSystems)
                    {
                        if (connection == updatedSystem.SolarSystemName) isNeedRemoveConnection = true;
                    }

                    if (isNeedRemoveConnection)
                    {
                        solarSystem.ConnectedSolarSystems.Remove(updatedSystem.SolarSystemName);
                    }
                }

                map.Systems.RemoveAll(x => x.SolarSystemName == updatedSystem.SolarSystemName);

                Log.InfoFormat("[MapTools.DeleteSolarSystems] Remove solar system {1} map with key ='{0}' for pilot {2}", map.Key, updatedSystem.SolarSystemName, map.ActivePilot);
            }
        }

        public static void RefreshPilots(Map map, List<PilotLocation> updatedPilotes)
        {
            Log.DebugFormat($"[MapTools.RefreshPilots] start for '{map.ActivePilot}' and map '{map.Key}'");

            if (map.Pilotes == null) map.Pilotes = new List<PilotLocation>();

            foreach (var updatedPilot in updatedPilotes)
            {
                var isExist = false;

                foreach (var pilotLocation in map.Pilotes)
                {
                    if (pilotLocation.Name != updatedPilot.Name) continue;

                    isExist = true;
                    pilotLocation.System = updatedPilot.System;
                }

                if (isExist == false)
                {
                    map.Pilotes.Add(new PilotLocation { Name = updatedPilot.Name, System = updatedPilot.System, LastUpdate = DateTime.UtcNow });
                }
            }
        }

        public static void Normalization(Map map)
        {
            Log.DebugFormat($"[MapTools.Normalization] start for '{map.ActivePilot}' and map '{map.Key}'");

            foreach ( var solarSystem in map.Systems )
            {
                if (solarSystem.SolarSystemName != null)
                {
                    solarSystem.Type = UpdateSystemType(map, solarSystem.SolarSystemName);
                }
            }
        }

        private static List<string> _systems = new List<string>();

        public static void HideUnconnectedSystems(Map map)
        {
            Log.DebugFormat($"[MapTools.HideUnconnectedSystems] for map {map.Key}. active pilot {map.ActivePilot}");

            if (map.LocationSolarSystemName == null) return;

            _systems = new List<string>();

            foreach (var solarSystem in map.Systems)
            {
                solarSystem.IsHidden = true;
            }

            CheckConnectionsForSystem(map, map.LocationSolarSystemName);

            var system = map.GetSystem(map.LocationSolarSystemName);

            system.IsHidden = false;
        }

        private static void CheckConnectionsForSystem(Map map, string locationSolarSystemName)
        {
            var system = map.GetSystem(locationSolarSystemName);

            _systems.Add(locationSolarSystemName);

            if (system == null) return;

            if (system.IsDeleted) return;

            system.IsHidden = false;

            foreach (var connection in system.ConnectedSolarSystems)
            {
                if (_systems.Contains(connection) == false)
                {
                    CheckConnectionsForSystem(map, connection);
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

            foreach (var connected in systemPrevious.ConnectedSolarSystems)
            {
                if (IsWSpaceSystem(connected))
                {
                    return "B";
                }
            }

            foreach (var connected in systemPrevious.ConnectedSolarSystems)
            {
                var connectedSystem = map.GetSystem(connected);

                foreach (var connectedOfConnected in connectedSystem.ConnectedSolarSystems)
                {
                    if (IsWSpaceSystem(connectedOfConnected))
                    {
                        return "C";
                    }
                }
            }

            return "D";
        }

        public static SecurityStatus GetConnectionType(EveJimaUniverse.System solarSystemFrom, EveJimaUniverse.System solarSystemTo)
        {
            var _solarSystemFromInfo = Global.Space.GetSystemByName(solarSystemFrom.SolarSystemName);
            var _solarSystemToInfo = Global.Space.GetSystemByName(solarSystemTo.SolarSystemName);

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
