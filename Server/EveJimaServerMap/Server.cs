using System;
using System.Collections.Generic;
using System.Linq;
using EveJimaUniverse;
using log4net;

namespace EveJimaServerMap
{
    public class Server
    {
        public List<string> Messages = new List<string>();
        public List<Map> Maps = new List<Map>();
        public List<PilotLocation> Pilots = new List<PilotLocation>();

        private readonly MapType type;

        public Server(MapType type)
        {
            this.type = type;
        }

        public Map GetMap(string key, string pilotName, string systemFrom, string systemTo)
        {
            var map = GetMapByKey(key);

            if (map == null) return CreateMap(key, pilotName);

            if(map.Systems.Count == 0) return map;

            if(map.IsSystemConnectedToMap(systemFrom)) return map;
            
            if (map.IsSystemConnectedToMap(systemTo)) return MapsMergePostProcessing(map, pilotName, systemTo, systemFrom);

            if (map.IsSystemConnectedToMap(systemFrom)) return MapsMergePostProcessing(map, pilotName, systemFrom, systemFrom);

            var pilotMapKey = key + "_" + pilotName.Replace(" ", "_").Replace("-", "_");

            map = GetMapByKey(pilotMapKey);

            return map ?? CreateMap(pilotMapKey, pilotName);
        }

        private Map MapsMergePostProcessing(Map map, string pilotName, string connectedMapsSystem, string systemFrom)
        {
            var pilotMapKey = map.Key + "_" + pilotName.Replace(" ", "_").Replace("-", "_");

            var localMap = GetMapByKey(pilotMapKey);

            if (localMap == null) return map;

            localMap.AddSolarSystem(systemFrom, connectedMapsSystem);

            var connectedSystem = localMap.GetSystem(connectedMapsSystem);

            mergedSystems = new List<string>();

            MapsMergeAddSystem(map, localMap, connectedSystem);

            //foreach(var solarSystem in localMap.Systems)
            //{
            //    map.AddSolarSystem(null, solarSystem.Name);
            //}

            map.Save();

            var itemToRemove = Maps.Single(r => r.Key == pilotMapKey);
            Maps.Remove(itemToRemove);

            localMap.Delete();

            return map;
        }

        private List<string> mergedSystems; 

        private void MapsMergeAddSystem(Map map, Map localMap, SolarSystem system)
        {
            foreach(var connection in system.Connections)
            {
                var connectedSystem = localMap.GetSystem(connection);

                if(mergedSystems.Contains(connection) == false)
                {
                    mergedSystems.Add(connection);
                    
                    map.AddSolarSystem(system.Name, connectedSystem.Name, false);

                    MapsMergeAddSystem(map, localMap, connectedSystem);
                }
                
            }
        }

        private Map GetMapByKey(string key)
        {
            return Maps.FirstOrDefault(spaceMap => spaceMap.Key == key);
        }

        private Map CreateMap(string key, string pilotName)
        {
            var map = new Map();
            map.Initialization(key, type);
            map.Owner = pilotName;

            AddMap(map);

            return map;
        }

        public string GetMapOwner(string key)
        {
            foreach(var spaceMap in Maps.Where(spaceMap => spaceMap.Key == key))
            {
                return spaceMap.Owner;
            }

            return "";
        }

        public Map GetMap(string key, string pilotName)
        {
            foreach (var spaceMap in Maps)
            {
                if (spaceMap.Key == key)
                {

                    var location = Pilots.Find(x => x.MapKey == key && x.Name == pilotName);

                    if(location == null) continue;

                    if (spaceMap.IsSystemConnectedToMap(location.System))
                    {

                        return spaceMap;
                    }

                    var localMapKey = key + "_" + pilotName.Replace(" ", "_").Replace("-", "_");

                    foreach (var spaceMapLocal in Maps)
                    {
                        if (spaceMapLocal.Key == localMapKey)
                        {
                            return spaceMapLocal;
                        }
                    }

                    return CreateMap(localMapKey, pilotName);
                }
            }

            return CreateMap(key, pilotName);
        }

        public void AddMap(Map map)
        {
            Maps.Add(map);
        }

        public void AddMessage(string message)
        {
            Messages.Add(message);
        }

        public void RelocatePilot(string key, string pilot, string system)
        {
            try
            {
                var isExist = false;

                foreach (var pilotLocation in Pilots)
                {
                    if (pilotLocation.Name != pilot) continue;

                    isExist = true;
                    pilotLocation.MapKey = key;
                    pilotLocation.System = system;
                    pilotLocation.LastUpdate = DateTime.UtcNow;
                }

                if (isExist == false)
                {
                    Pilots.Add(new PilotLocation { MapKey = key, Name = pilot, System = system, LastUpdate = DateTime.UtcNow });
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("All").ErrorFormat("[RelocatePilot] Critical error map with key {0} exception {1}", key, ex);
            }

        }

        public List<PilotLocation> GetPilotes(string mapKey, DateTime lastUpdate)
        {
            var list = new List<PilotLocation>();

            foreach (var solarSystem in Pilots)
            {
                if (solarSystem.LastUpdate > lastUpdate && solarSystem.MapKey == mapKey)
                {
                    try
                    {
                        list.Add(solarSystem);
                    }
                    catch (Exception ex)
                    {
                        LogManager.GetLogger("All").ErrorFormat("[GetPilotes] Critical error map with key {0} exception {1}", mapKey, ex);
                    }
                }
            }

            return list;
        }

        
    }
}
