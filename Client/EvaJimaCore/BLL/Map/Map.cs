using System;
using System.Collections.Generic;
using System.Linq;
using EvaJimaCore;
using EveJimaUniverse;
using log4net;

namespace EveJimaCore.BLL.Map
{
    public class Map
    {
        public event Action<string> OnChangeStatus;

        readonly ILog _commandsLog = LogManager.GetLogger("CommandsMap");

        public string Key { get; set; }

        public string Owner { get; set; }

        public string ActivePilot { get; set; }

        private long _lastUpdate;

        public List<SolarSystem> Systems { get; set; }

        public List<PilotLocation> Pilotes { get; set; }

        private System.Timers.Timer aTimer;

        public string SelectedSolarSystemName { get; set; }

        public string LocationSolarSystemName { get; set; }

        public string PreviousLocationSolarSystemName { get; set; }

        public Map()
        {
            Systems = new List<SolarSystem>();

            //_lastUpdate = DateTime.UtcNow.Ticks;
        }

        public void Activate(string owner, string system)
        {
            ActivePilot = owner;
            SelectedSolarSystemName = system;
            LocationSolarSystemName = system;

            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += Event_Refresh;
            aTimer.Interval = 5000;
            aTimer.Enabled = true;
        }

        private void Event_Refresh(object sender, EventArgs e)
        {
            _commandsLog.DebugFormat("[Map.Event_Refresh] Refresh map with key ='{0}' for pilot ='{1}'", Key, ActivePilot);

            if (Key == string.Empty) return;

            Update();
        }

        public SolarSystem GetSystem(string name)
        {
            return Systems.FirstOrDefault(solarSystem => solarSystem.Name == name);
        }

        public void Reload(string key)
        {
            _isUpdateInProgress = true;

            _commandsLog.InfoFormat("[Map.Reload] Reload map with key ='{0}' to key '{2}' for pilot ='{1}'", Key, ActivePilot, key);
            _lastUpdate = 0;
            Key = key;

            Global.MapApiFunctions.PublishSolarSystem(ActivePilot, Key, null, LocationSolarSystemName);
            Global.Pilots.Selected.Key = key;
            Global.ApplicationSettings.UpdatePilotInStorage(Global.Pilots.Selected.Name, Global.Pilots.Selected.Id.ToString(), Global.Pilots.Selected.CrestData.RefreshToken, Key);

            _isUpdateInProgress = false;

            Update();
        }

        private bool _isUpdateInProgress;

        public string Update()
        {
            if(_isUpdateInProgress) return string.Empty;

            _isUpdateInProgress = true;

            var message = "";

            if(string.IsNullOrEmpty(Owner))
            {
                OnChangeStatus?.Invoke($"Get map owner for map {Key}...");
                Owner = Global.MapApiFunctions.GetMapOwner(Key);
            }

            OnChangeStatus?.Invoke($"Start get updates for map {Key}...");

            var updatedSystems = Global.MapApiFunctions.GetUpdates(Key, ActivePilot, _lastUpdate);

            OnChangeStatus?.Invoke($"End get updates for map {Key}. Updated {updatedSystems.Count} solar systems.");

            _commandsLog.InfoFormat("[Map.Update] Load systems for map with key ='{0}' for pilot ='{1}' Updated Systems = '{2}' _lastUpdate = '{3}'", Key, ActivePilot, updatedSystems.Count, _lastUpdate);

            if (updatedSystems.Count > 0)
            {
                message = string.Format("{0:HH:mm:ss}", DateTime.UtcNow) + " Updated " + updatedSystems.Count + " systems.";

                UpdateSolarSystems(updatedSystems);
            }

            var deletedSystems = Global.MapApiFunctions.GetDeletes(Key, ActivePilot, _lastUpdate);

            OnChangeStatus?.Invoke($"End get deleted systems for map {Key}. Removed {deletedSystems.Count} solar systems.");

            if (deletedSystems.Count > 0)
            {
                message = message + Environment.NewLine;
                message = message + string.Format("{0:HH:mm:ss}", DateTime.UtcNow) + " Deleted " + deletedSystems.Count + " systems.";

                GarbageCollector(deletedSystems);
            }

            //List<PilotLocation> GetPilotes

            var updatePilotes = Global.MapApiFunctions.GetPilotes(Key, _lastUpdate, ActivePilot);

            OnChangeStatus?.Invoke($"End get active pilotes for map {Key}. Updated {updatePilotes.Count} pilotes.");

            if (updatePilotes.Count > 0)
            {
                UpdatePilots(updatePilotes);
            }

            _lastUpdate = DateTime.UtcNow.Ticks;

            HideUnconnectedSystems();

            OnChangeStatus?.Invoke($"End remove old connection for map {Key}.");

            _isUpdateInProgress = false;

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
            Global.MapApiFunctions.PublishSolarSystem(pilotName, Key, systemFrom, systemTo);

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
                    _commandsLog.InfoFormat("[UpdateSolarSystemCoordinates] For map with key {0} updated system {2} oordinates {1}", Key, system.LocationInMap.X + ":" + system.LocationInMap.Y, system.Name);
                    system.Signatures = updatedSystem.Signatures;
                    system.Connections = updatedSystem.Connections;
                }
                else
                {
                    Systems.Add(updatedSystem);
                    _commandsLog.InfoFormat("[UpdateSolarSystemCoordinates] For map with key {0} added system {2} oordinates {1}", Key, updatedSystem.LocationInMap.X + ":" + updatedSystem.LocationInMap.Y, updatedSystem.Name);
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

        public void DeathNotice(string locationSolarSystem)
        {
            Global.MapApiFunctions.DeleteConnectionBetweenSolarSystems(ActivePilot, Key, PreviousLocationSolarSystemName, locationSolarSystem);

            _commandsLog.InfoFormat("[DeathNotice] For map with key {0} delete connection from system {2} to system {1}", Key, locationSolarSystem, PreviousLocationSolarSystemName);

            Update();
        }
    }
}
