using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EvaJimaCore;
using EveJimaUniverse;
using log4net;

namespace EveJimaCore.BLL.Map
{
    public class Map
    {
        public event Action<string> OnChangeStatus;

        private static readonly ILog Log = LogManager.GetLogger(typeof(Map));
        readonly ILog _commandsLog = LogManager.GetLogger("All");

        public string Key { get; set; }

        private string Owner { get; set; }

        public string ActivePilot { get; set; }

        private long _lastUpdate;

        public List<SolarSystem> Systems { get; set; }

        public List<PilotLocation> Pilotes { get; set; }

        private System.Timers.Timer aTimer;

        public string SelectedSolarSystemName { get; set; }

        public string LocationSolarSystemName { get; set; }

        public string PreviousLocationSolarSystemName { get; set; }

        private bool isStoppedUpdates;

        public Map()
        {
            Systems = new List<SolarSystem>();

            _lastUpdate = new DateTime(2015,5,5).Ticks;
        }

        public void Activate(string owner, string system)
        {
            ActivePilot = owner;
            SelectedSolarSystemName = system;
            LocationSolarSystemName = system;

            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += Event_Refresh;
            aTimer.Interval = 10000;
            aTimer.Enabled = true;
        }

        public string GetOwner()
        {
            if(Owner == null)
            {
                Log.DebugFormat("[Map.GetOwner] start");
                Owner = Global.MapApiFunctions.GetMapOwner(Key);
                Log.DebugFormat("[Map.GetOwner] end");
            }

            return Owner;
        }

        private void Event_Refresh(object sender, EventArgs e)
        {
            if(isRunUpdate)
            {
                _commandsLog.DebugFormat("[Map.Event_Refresh] Updates stopped because previous not ended for map with key ='{0}' for pilot ='{1}'", Key, ActivePilot);

                return;
            }

            if (isStoppedUpdates)
            {
                _commandsLog.DebugFormat("[Map.Event_Refresh] Updates stopped for map with key ='{0}' for pilot ='{1}'", Key, ActivePilot);
                return;
            }

            if (isGlobalReload)
            {
                _commandsLog.DebugFormat("[Map.Event_Refresh] Updates stopped for Global Reload for map with key ='{0}' for pilot ='{1}'", Key, ActivePilot);
                return;
            }

            if(Key == string.Empty) return;

            _commandsLog.DebugFormat("[Map.Event_Refresh] Start Refresh map with key ='{0}' for pilot ='{1}'", Key, ActivePilot);

            Update();

            _commandsLog.DebugFormat("[Map.Event_Refresh] End Refresh map with key ='{0}' for pilot ='{1}'", Key, ActivePilot);
        }

        private void UpdatesStop()
        {
            aTimer.Stop();
        }

        private void UpdatesStart()
        {
            aTimer.Start();
        }

        public SolarSystem GetSystem(string name)
        {
            return Systems.FirstOrDefault(solarSystem => solarSystem.Name == name);
        }

        private bool isGlobalReload;

        public void Reload(string key)
        {
            UpdatesStop();

            if (isGlobalReload)
            {
                _commandsLog.DebugFormat("[Map.Reload] Updates stopped for Global Reload for map with key ='{0}' for pilot ='{1}'", Key, ActivePilot);
                return;
            }


            isStoppedUpdates = true;
            isGlobalReload = true;

            
            _commandsLog.DebugFormat("[Map.Reload] Load systems for map with key ='{0}' for pilot ='{1}'  _lastUpdate = '{2}'", Key, ActivePilot, _lastUpdate);

            while (isRunUpdate)
            {
                _commandsLog.InfoFormat("[Map.Reload] Waiting for Reload map with key ='{0}' to key '{2}' for pilot ='{1}'", Key, ActivePilot, key);
                Thread.Sleep(1000);
            }

            _lastUpdate = new DateTime(2015, 5, 5).Ticks;
            Update();

            isStoppedUpdates = false;
            isGlobalReload = false;

            UpdatesStart();
        }

        private readonly object _updateLock = new object();
        private readonly object _resetLock = new object();

        public void Reset(string key)
        {
            UpdatesStop();

            if (isGlobalReload)
            {
                _commandsLog.DebugFormat("[Map.Reset] Updates stopped for Global Reload for map with key ='{0}' for pilot ='{1}'", Key, ActivePilot);
                return;
            }

            isGlobalReload = true;
            isStoppedUpdates = true;

            lock (_resetLock)
            {
                while(isRunUpdate)
                {
                    
                    _commandsLog.InfoFormat("[Map.Reset] Waiting for reset map with key ='{0}' to key '{2}' for pilot ='{1}'", Key, ActivePilot, key);
                    Thread.Sleep(1000);
                }

                _commandsLog.InfoFormat("[Map.Reset] Reset map with key ='{0}' to key '{2}' for pilot ='{1}'", Key, ActivePilot, key);
                _lastUpdate = new DateTime(2015, 5, 5).Ticks;
                Key = key;

                ApiPublishSolarSystem(ActivePilot, Key, null, LocationSolarSystemName);

                Global.Pilots.Selected.Key = key;
                Global.ApplicationSettings.UpdatePilotInStorage(Global.Pilots.Selected.Name, Global.Pilots.Selected.Id.ToString(), Global.Pilots.Selected.CrestData.RefreshToken, Key);

                Update();
            }

            isStoppedUpdates = false;
            isGlobalReload = false;

            UpdatesStart();
        }

        public List<SolarSystem> ApiPublishSolarSystem(string pilotName, string key, string systemFrom, string systemTo)
        {
            OnChangeStatus?.Invoke($"Start Publish Solar System for map '{Key}' with pilot '{pilotName}'. Relocated from '{systemFrom}' to '{systemTo}'");

            var updatedSystems = Global.MapApiFunctions.PublishSolarSystem(pilotName, Key, systemFrom, systemTo);

            OnChangeStatus?.Invoke($"End get updates for map '{Key}' after PublishSolarSystem. Updated {updatedSystems.Count} solar systems.");

            UpdateSolarSystems(updatedSystems);

            return updatedSystems;
        }

        public List<SolarSystem> ApiDeleteSolarSystem(string key , string solarSystemName, string pilotName)
        {
            OnChangeStatus?.Invoke($"Start Delete Solar System for map '{key}' with pilot '{pilotName}'. Deleted solar system name is '{solarSystemName}'");

            var updatedSystems = Global.MapApiFunctions.DeleteSolarSystem(key, solarSystemName, pilotName);

            UpdateSolarSystems(updatedSystems);

            RemoveSystem(solarSystemName);

            return updatedSystems;

            

        }

        public List<SolarSystem> ApiPublishSignatures(string key, string solarSystemName, string pilotName, List<CosmicSignature> signatures)
        {
            OnChangeStatus?.Invoke($"Start Publish Signatures Solar System '{solarSystemName}' for map '{key}' with pilot '{pilotName}'. signatures count is '{signatures.Count}'");

            var updatedSystems = Global.MapApiFunctions.PublishSignatures(pilotName, key, solarSystemName, signatures);

            UpdateSolarSystems(updatedSystems);

            RemoveSystem(solarSystemName);

            return updatedSystems;
        }

        public long GetLastUpdate()
        {
            return _lastUpdate;
        }

        private bool isRunUpdate = false;

        public string Update()
        {

            var message = "";

            

            lock(_updateLock)
            {
                isRunUpdate = true;

                message = UpdateMap();

                isRunUpdate = false;
            }

            return message;
        }

        private string UpdateMap()
        {
            Log.DebugFormat($"[Map.UpdateMap] start for {ActivePilot}");

            var message = "";

            if (string.IsNullOrEmpty(Owner))
            {
                OnChangeStatus?.Invoke($"Get map owner for map {Key} active pilot {ActivePilot}...");
                Owner = GetOwner();
            }

            OnChangeStatus?.Invoke($"Start get updates for map {Key} active pilot {ActivePilot}...");

            var updatedSystems = Global.MapApiFunctions.GetUpdates(Key, ActivePilot, _lastUpdate);

            OnChangeStatus?.Invoke($"End get updates for map {Key}. Updated {updatedSystems.Count} solar systems. active pilot {ActivePilot}");

            _commandsLog.DebugFormat("[Map.Update] Load systems for map with key ='{0}' for pilot ='{1}' Updated Systems = '{2}' _lastUpdate = '{3}'", Key, ActivePilot, updatedSystems.Count, _lastUpdate);

            if (updatedSystems.Count > 0)
            {
                message = string.Format("{0:HH:mm:ss}", DateTime.UtcNow) + " Updated " + updatedSystems.Count + $" systems. active pilot {ActivePilot}";

                UpdateSolarSystems(updatedSystems);
            }

            var deletedSystems = Global.MapApiFunctions.GetDeletes(Key, ActivePilot, _lastUpdate);

            OnChangeStatus?.Invoke($"End get deleted systems for map {Key}. Removed {deletedSystems.Count} solar systems. active pilot {ActivePilot}");

            if (deletedSystems.Count > 0)
            {
                message = message + Environment.NewLine;
                message = message + string.Format("{0:HH:mm:ss}", DateTime.UtcNow) + " Deleted " + deletedSystems.Count + $" systems. active pilot {ActivePilot}";

                GarbageCollector(deletedSystems);
            }

            var updatePilotes = Global.MapApiFunctions.GetPilotes(Key, _lastUpdate, ActivePilot);

            OnChangeStatus?.Invoke($"End get active pilotes for map {Key}. Updated {updatePilotes.Count} pilotes. active pilot {ActivePilot}");

            if (updatePilotes.Count > 0)
            {
                UpdatePilots(updatePilotes);
            }

            _lastUpdate = DateTime.UtcNow.Ticks;

            HideUnconnectedSystems();

            OnChangeStatus?.Invoke($"End remove old connection for map {Key}. active pilot {ActivePilot}");

            Log.DebugFormat($"[Map.UpdateMap] end for {ActivePilot}");

            return message;
        }

        List<string> _systems = new List<string>();

        private void HideUnconnectedSystems()
        {
            _systems = new List<string>();

            foreach (var solarSystem in Systems)
            {
                solarSystem.IsHidden = true;
            }

            CheckConnectionsForSystem(LocationSolarSystemName);
        }

        private void CheckConnectionsForSystem(string locationSolarSystemName)
        {
            var system = GetSystem(locationSolarSystemName);

            _systems.Add(locationSolarSystemName);

            if (system == null) return;

            if (system.IsDeleted) return;

            system.IsHidden = false;

            foreach(var connection in system.Connections)
            {
                if (_systems.Contains(connection) == false)
                {
                    CheckConnectionsForSystem(connection);
                }
            }
        }


        public void Publish(string pilotName, string systemFrom, string systemTo)
        {
            ApiPublishSolarSystem(pilotName, Key, systemFrom, systemTo);

            Update();

            _lastUpdate = DateTime.UtcNow.Ticks;
        }

        private void UpdateSolarSystems(List<SolarSystem> updatedSystems)
        {
            foreach (var updatedSystem in updatedSystems)
            {
                var system = GetSystem(updatedSystem.Name);

                if (system != null)
                {
                    system.LocationInMap = updatedSystem.LocationInMap;
                    _commandsLog.InfoFormat("[UpdateSolarSystemCoordinates] For map with key {0} updated system {2} Coordinates {1}", Key, system.LocationInMap.X + ":" + system.LocationInMap.Y, system.Name);
                    system.Signatures = updatedSystem.Signatures;
                    system.Connections = updatedSystem.Connections;
                }
                else
                {
                    Systems.Add(updatedSystem);
                    _commandsLog.InfoFormat("[UpdateSolarSystemCoordinates] For map with key {0} added system {2} Coordinates {1}", Key, updatedSystem.LocationInMap.X + ":" + updatedSystem.LocationInMap.Y, updatedSystem.Name);
                }


            }

            MapTools.Normalization(this);
        }

        private void UpdatePilots(List<PilotLocation> updatedPilotes)
        {
            if (Pilotes == null) Pilotes = new List<PilotLocation>();

            foreach (var updatedPilot in updatedPilotes)
            {
                var isExist = false;

                foreach (var pilotLocation in Pilotes)
                {
                    if (pilotLocation.Name != updatedPilot.Name) continue;

                    isExist = true;
                    pilotLocation.System = updatedPilot.System;
                }

                if (isExist == false)
                {
                    Pilotes.Add(new PilotLocation { Name = updatedPilot.Name, System = updatedPilot.System, LastUpdate = DateTime.UtcNow });
                }
            }
        }

        private void GarbageCollector(IEnumerable<SolarSystem> deletedSystems)
        {
            foreach (var updatedSystem in deletedSystems)
            {

                foreach (var solarSystem in Systems)
                {
                    var isNeedRemoveConnection = false;

                    foreach(var connection in solarSystem.Connections)
                    {
                        if(connection == updatedSystem.Name) isNeedRemoveConnection = true;
                    }

                    if(isNeedRemoveConnection)
                    {
                        solarSystem.Connections.Remove(updatedSystem.Name);
                    }
                }

                Systems.RemoveAll(x => x.Name == updatedSystem.Name);

                _commandsLog.InfoFormat("[Map.GarbageCollector] Remove solar system {1} map with key ='{0}' for pilot {2}", Key, updatedSystem.Name, ActivePilot);
            }

        }

        public string GetPilotesBySolarSystem(string name)
        {
            var pilotesInSolarSystem = "";

            foreach ( var pilotLocation in Pilotes )
            {
                if ( pilotLocation.System == name )
                {
                    if ( pilotesInSolarSystem == "" )
                    {
                        pilotesInSolarSystem = pilotLocation.Name;
                    }
                    else
                    {
                        pilotesInSolarSystem = pilotesInSolarSystem + Environment.NewLine + "" + pilotLocation.Name;
                    }
                }
            }

            return pilotesInSolarSystem;
        }

        public void RelocatePilot(string pilotname, string systemfrom, string systemto)
        {
            PreviousLocationSolarSystemName = systemfrom;
            LocationSolarSystemName = systemto;
        }

        public void RemoveSystem(string solarSystem)
        {
            OnChangeStatus?.Invoke($"Check is removed solar system '{solarSystem}' current for selected pilot {Global.Pilots.Selected.Name}...");

            if (solarSystem == LocationSolarSystemName) return;

            var deletedSystem = GetSystem(solarSystem);

            deletedSystem.IsDeleted = true;

            SelectedSolarSystemName = LocationSolarSystemName;

            Update();
        }

        public List<SolarSystem> ApiPublishDeathNotice(string locationSolarSystem)
        {
            var updatedSystems = Global.MapApiFunctions.DeleteConnectionBetweenSolarSystems(ActivePilot, Key, PreviousLocationSolarSystemName, locationSolarSystem);

            OnChangeStatus?.Invoke($"End get updates for map '{Key}' after DeathNotice delete connection from system {PreviousLocationSolarSystemName} to system {locationSolarSystem}. Updated {updatedSystems.Count} solar systems.");

            UpdateSolarSystems(updatedSystems);

            _commandsLog.InfoFormat("[DeathNotice] For map with key {0} delete connection from system {2} to system {1}", Key, locationSolarSystem, PreviousLocationSolarSystemName);

            return updatedSystems;
        }
    }
}
