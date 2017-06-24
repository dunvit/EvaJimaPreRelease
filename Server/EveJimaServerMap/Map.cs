using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using EveJimaUniverse;
using log4net;
using System.Collections.Concurrent;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;

namespace EveJimaServerMap
{
    public class Map
    {
        readonly ILog _log = LogManager.GetLogger("All");
        readonly ILog _commandsLog = LogManager.GetLogger("Commands");

        public MapInformation Information = new MapInformation();
            
        [JsonIgnore]
        public ConcurrentDictionary<string, SolarSystem> Systems { get; set; }

        [JsonIgnore]
        private ConcurrentDictionary<string, SolarSystem> DeletedSystems { get; set; }

        [JsonIgnore]
        private readonly Random _randomBase = new Random();

        public void Initialization(string key, MapType type)
        {
            Information.Key = key;
            Systems = new ConcurrentDictionary<string, SolarSystem>();
            DeletedSystems = new ConcurrentDictionary<string, SolarSystem>();

            switch(type)
            {
                case MapType.Client:
                break;

                case MapType.Server:
                break;
            }

            LoadFromFile(Information.Key);
        }



        public void GarbageCollector()
        {
            try
            {
                var removeCollection = new List<SolarSystem>();

                foreach (var visitedSolarSystem in Systems)
                {
                    var totalHours = (DateTime.UtcNow - visitedSolarSystem.Value.Created).TotalHours;

                    if (totalHours > Tools.SolarSystemTTL())
                    {
                        removeCollection.Add(visitedSolarSystem.Value);
                    }
                }


                foreach (var visitedSolarSystem in removeCollection)
                {
                    DeletedSystems.TryAdd(visitedSolarSystem.Name, visitedSolarSystem);

                    SolarSystem element;

                    Systems.TryRemove(visitedSolarSystem.Name, out element);
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[Map.GarbageCollector] Critical error {0}" , ex);
            }
        }

        public void DeleteSolarSystemConnection(string systemTo, string systemFrom)
        {
            try
            {
                foreach (var solarSystem in Systems)
                {
                    if(solarSystem.Value.Name == systemTo)
                    {
                        solarSystem.Value.Connections.Remove(systemFrom);
                        solarSystem.Value.LastUpdate = DateTime.UtcNow;
                    }

                    if (solarSystem.Value.Name == systemFrom)
                    {
                        solarSystem.Value.Connections.Remove(systemTo);
                        solarSystem.Value.LastUpdate = DateTime.UtcNow;
                    }
                }

                Save();
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }

        }

        public void DeleteSolarSystem(string system)
        {
            try
            {
                foreach (var solarSystem in Systems)
                {
                    if(solarSystem.Value.Connections.Contains(system))
                    {
                        solarSystem.Value.Connections.Remove(system);
                        solarSystem.Value.LastUpdate = DateTime.UtcNow;
                    }
                }

                var deletedSolarSystem = GetSystem(system);

                DeletedSystems.TryAdd(deletedSolarSystem.Name, deletedSolarSystem);

                SolarSystem element;

                Systems.TryRemove(deletedSolarSystem.Name, out element);

                Save();
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
            
        }

        private void LoadFromFile(string key)
        {
            try
            {
                var dataFile = HttpContext.Current.Server.MapPath("~/Data/Maps/Map_" + key);

                if (File.Exists(dataFile) == false) return;


                var json = File.ReadAllText(dataFile);

                var history = new MapInformation();
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(history.GetType());
                history = ser.ReadObject(ms) as MapInformation;
                ms.Close();


                //var history = Tools.DeSerializeObject<Map>(dataFile);

                if (history == null) return;

                foreach(var solarSystem in history.SystemsForSave)
                {
                    Systems.TryAdd(solarSystem.Name, solarSystem);
                }

                Information = history;

                // Remove old systems by TTL from web.config
                GarbageCollector();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[Map.LoadFromFile] Map: {1} Critical error {0}", ex, key);
            }

            
        }

        private static readonly object SyncAddSolarSystem = new object();

        public void AddSolarSystem(string systemFrom, string systemTo, bool isNeedSave = true)
        {
            try
            {
                lock (SyncAddSolarSystem)
                {
                    GarbageCollector();

                    var system = GetSystem(systemTo);

                    var isFirstStarSystemInMap = !Systems.Any();

                    if (system != null)
                    {
                        AddConnectionSolarSystem(systemFrom, systemTo);

                        if (system.LocationInMap == new Point(0, 0))
                        {
                            _commandsLog.InfoFormat("[AddSolarSystem] [AddSpaceMapCoordinates] For map with key {0} systemTo {2} systemFrom {3} coordinates before {1}", Information.Key, system.LocationInMap.X + ":" + system.LocationInMap.Y, systemTo, systemFrom);
                            AddSpaceMapCoordinates(systemTo, systemFrom, isFirstStarSystemInMap);
                        }

                        return;
                    }

                    InsertNewPoint(systemTo);

                    AddConnectionSolarSystem(systemFrom, systemTo);

                    _commandsLog.InfoFormat("[AddSolarSystem] [AddSpaceMapCoordinates] For map with key {0} systemTo {2} systemFrom {3} coordinates before {1}", Information.Key, 0 + ":" + 0, systemTo, systemFrom);
                    AddSpaceMapCoordinates(systemTo, systemFrom, isFirstStarSystemInMap);

                    if(isNeedSave) Save();
                }
            }
            catch (Exception)
            {

            }

            
        }

        

        

        private static readonly object SyncRoot = new object();
        public void Save()
        {

            lock (SyncRoot)
            {
                try
                {
                    var dataFile = HttpContext.Current.Server.MapPath("~/Data/Maps/Map_" + Information.Key);


                    Information.SystemsForSave = new List<SolarSystem>();

                    foreach (var solarSystem in Systems.Values)
                    {
                        Information.SystemsForSave.Add(solarSystem);
                    }

                    var jsonFormatter = new DataContractJsonSerializer(typeof(MapInformation));

                    File.Delete(dataFile);

                    using (var fs = new FileStream(dataFile, FileMode.Create))
                    {
                        jsonFormatter.WriteObject(fs, Information);
                    }
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat("[Map.Save] Critical error {0}", ex);
                }
                    
            }

            
        }

        public List<SolarSystem> GetUpdates(DateTime lastUpdate)
        {
            GarbageCollector();

            var list = new List<SolarSystem>();

            try
            {
                var updatePoint = lastUpdate.AddSeconds(-2);

                foreach (var solarSystem in Systems.Values)
                {
                    _commandsLog.InfoFormat("[GetUpdates] solarSystem {0} solarSystem.LastUpdate {1} lastUpdate {2} ", solarSystem, solarSystem.LastUpdate.Ticks, updatePoint.Ticks);

                    if (solarSystem.LastUpdate.Ticks > updatePoint.Ticks)
                    {
                        try
                        {
                            list.Add(solarSystem);
                        }
                        catch (Exception ex)
                        {
                            _log.ErrorFormat("[Map.GetUpdates] Critical error {0}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[Map.GetUpdates] Critical error {0}", ex);
            }
            
            return list;
        }

        public List<SolarSystem> GetDeleted(DateTime lastUpdate)
        {
            var list = new List<SolarSystem>();

            if(lastUpdate.Year == 1) return list;

            foreach (var solarSystem in DeletedSystems.Values)
            {
                if (solarSystem.LastUpdate > lastUpdate)
                {
                    try
                    {
                        list.Add(solarSystem);
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            return list;
        }

        public bool IsSystemConnectedToMap(string system)
        {
            foreach (var solarSystem in Systems.Values)
            {
                if (system == solarSystem.Name)
                {
                    return true;
                }
            }

            return false;
        }

        private void AddSpaceMapCoordinates(string systemTo, string systemFrom, bool isFirstStarSystemInMap)
        {
            try
            {
                var systemCurrent = GetSystem(systemTo);

                if (isFirstStarSystemInMap)
                {
                    if (systemCurrent != null)
                    {
                        systemCurrent.LocationInMap = new Point(5000, 5000);
                        _commandsLog.InfoFormat("[AddSpaceMapCoordinates] For map with key {0} system {2} set oordinates {1}", Information.Key, systemCurrent.LocationInMap.X + ":" + systemCurrent.LocationInMap.Y, systemTo);

                        return;
                    }
                }

                var systemPrevious = GetSystem(systemFrom);

                /*
                 * 
	                1. Тип А. ВХ система. Уникальные координаты.
	                2. Тип В. Империя с соседней ВХ Уникальные координаты
	                3. Тип С. Империя с соседней имперской системой имеющей соседнюю ВХ систему. Уникальные координаты. Дата обновления 2050 год
	                4. Тип D. Империя с соседними системами "С" и "D" типа. Координаты предыдущей системы. Дата обновления 2050 год
                 * 
                 */

                var systems = Systems; //(ConcurrentDictionary<string, SolarSystem>)Tools.CloneGenericList(this.Systems);

                switch (MapTools.GetSystemType(systems, systemTo))
                {
                    case "A":
                        systemCurrent.LocationInMap = GetLocationPoint(systemPrevious);
                        _commandsLog.InfoFormat("[AddSpaceMapCoordinates] For map with key {0} system {2} set oordinates {1} for type 'A'", Information.Key, systemCurrent.LocationInMap.X + ":" + systemCurrent.LocationInMap.Y, systemTo);

                        break;

                    case "B":
                        systemCurrent.LocationInMap = GetLocationPoint(systemPrevious);
                        _commandsLog.InfoFormat("[AddSpaceMapCoordinates] For map with key {0} system {2} set oordinates {1} for type 'B'", Information.Key, systemCurrent.LocationInMap.X + ":" + systemCurrent.LocationInMap.Y, systemTo);

                        break;

                    case "C":
                        systemCurrent.LocationInMap = GetLocationPoint(systemPrevious);
                        _commandsLog.InfoFormat("[AddSpaceMapCoordinates] For map with key {0} system {2} set oordinates {1} for type 'C'", Information.Key, systemCurrent.LocationInMap.X + ":" + systemCurrent.LocationInMap.Y, systemTo);

                        break;

                    case "D":
                        systemCurrent.LocationInMap = new Point(systemPrevious.LocationInMap.X, systemPrevious.LocationInMap.Y);
                        _commandsLog.InfoFormat("[AddSpaceMapCoordinates] For map with key {0} system {2} set oordinates {1} for type 'D'", Information.Key, systemCurrent.LocationInMap.X + ":" + systemCurrent.LocationInMap.Y, systemTo);

                        break;
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[Map.AddSpaceMapCoordinates] Critical error {0}", ex);
            }
        }
        



        private Point GetLocationPoint(SolarSystem fromSystem)
        {
            var deltaX = 0;
            var deltaY = 0;

            var isSearchingPositionInMap = true;

            while (isSearchingPositionInMap)
            {
                var point = GetPoint(fromSystem.LocationInMap.X, fromSystem.LocationInMap.Y);

                deltaX = point.X;
                deltaY = point.Y;

                isSearchingPositionInMap = isPositionUsedInMap(point.X, point.Y);
            }

            return new Point(deltaX, deltaY);
        }

        private Point GetPoint(int centreX, int centreY)
        {
            double angleOfLineInDegrees = _randomBase.Next(1, 360);
            var angleOfLineInRadians = (angleOfLineInDegrees / 180) * Math.PI;

            var x = _randomBase.Next(90, 200);
            var y = _randomBase.Next(90, 200);

            var lineVector = new Point((int)((float)Math.Cos(angleOfLineInRadians) * x), (int)((float)Math.Sin(angleOfLineInRadians) * y));

            var lineEndPoint = new Point((centreX + lineVector.X), (centreY + lineVector.Y));

            return lineEndPoint;
        }

        private bool isPositionUsedInMap(int positionX, int positionY)
        {
            foreach (var visitedSolarSystem in Systems.Values)
            {
                var locationX = Math.Abs(visitedSolarSystem.LocationInMap.X - positionX);
                var locationY = Math.Abs(visitedSolarSystem.LocationInMap.Y - positionY);

                if (locationX < 50 && locationY < 50)
                {
                    return true;
                }
            }

            return false;
        }

        private void AddConnectionSolarSystem(string systemFromName, string systemToName)
        {
            if (systemFromName == null) return;

            var system = GetSystem(systemFromName);

            if (system != null)
            {
                if (system.Connections.Contains(systemToName) == false)
                {
                    system.Connections.Add(systemToName);

                    system.LastUpdate = DateTime.UtcNow;
                }
            }

            var systemCurrentLocation = GetSystem(systemToName);

            if (systemCurrentLocation == null) return;

            if (string.IsNullOrEmpty(systemFromName) == false && systemCurrentLocation.Connections.Contains(systemFromName) == false)
            {
                systemCurrentLocation.Connections.Add(systemFromName);

                systemCurrentLocation.LastUpdate = DateTime.UtcNow;
            }
        }

        private void InsertNewPoint(string systemName)
        {
            var system = new SolarSystem { Name = systemName, Created = DateTime.UtcNow};

            Systems.TryAdd(system.Name, system);
        }

        public SolarSystem GetSystem(string systemName)
        {
            if(Systems.ContainsKey(systemName))
            {
                return Systems[systemName];
            }

            return null;
        }

        public void UpdateSignatures(string system, List<CosmicSignature> signatures)
        {
            var solarSystem = GetSystem(system);

            foreach (var cosmicSignature in signatures)
            {
                foreach (var signature in solarSystem.Signatures)
                {
                    if(signature.Type != SignatureType.Unknown && signature.Code == cosmicSignature.Code && cosmicSignature.Type == SignatureType.Unknown)
                    {
                        cosmicSignature.Type = signature.Type;
                        cosmicSignature.Name = signature.Name;
                    }
                }
            }

            solarSystem.Signatures = new List<CosmicSignature>();

            foreach (var cosmicSignature in signatures) solarSystem.AddSignature(cosmicSignature);
            

            solarSystem.LastUpdate = DateTime.UtcNow; 
        }

        public void DeleteSignature(string system, string code)
        {
            var solarSystem = GetSystem(system);

            solarSystem.Signatures.RemoveAll(item => item.Code == code);

            solarSystem.LastUpdate = DateTime.UtcNow;
        }

        public void Delete()
        {
            try
            {
                lock (SyncRoot)
                {
                    var dataFile = HttpContext.Current.Server.MapPath("~/Data/Maps/Map_" + Information.Key);

                    if (File.Exists(dataFile))
                    {
                        File.Delete(dataFile);
                    }

                }
            }
            catch (Exception)
            {

            }
        }
    }
}
