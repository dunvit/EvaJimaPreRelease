using System;
using System.Drawing;
using System.Net;
using System.Timers;
using EvaJimaCore;
using EveJimaUniverse;
using log4net;

namespace EveJimaCore.BLL
{
    public delegate void DelegateChangeSolarSystem(PilotEntity pilot, string systemFrom, string systemTo);
    public delegate void DelegateEnterToSolarSystem(string pilotName, string systemFrom, string systemTo);
    

    public class PilotEntity
    {
        private static readonly ILog Log = LogManager.GetLogger("All");

        readonly ILog _commandsLog = LogManager.GetLogger("CommandsMap");

        public DelegateChangeSolarSystem OnChangeSolarSystem;
        
        public event DelegateEnterToSolarSystem OnEnterToSolarSystem;

        public string Key { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }

        public Image Portrait { get; set; }

        public Map.Map SpaceMap { get; set; }

        public StarSystemEntity Location { get; set; }

        public string SelectedSolarSystem { get; set; }

        public CrestAuthorization CrestData { get; set; }

        private DateTime _lastTokenUpdate;

        public Map.Map SpaceMap1 { get; set; }

        public PilotEntity(string id, string refreshToken)
        {
            ReInitialization(id, refreshToken);

            ActivatePilot();
        }

        public PilotEntity(string token)
        {
            Initialization(token);

            ActivatePilot();
        }

        private Timer aTimer;

        private void ActivatePilot()
        {
            Key = Global.ApplicationSettings.GetPilotKey(Name);

            // TODO: Remove 
            // 1973
            //Location.SolarSystemName = "J213734";
            LocationCurrentSystemName = Location.SolarSystemName;

            // Pilot not are log in
            if (Location.SolarSystemName == "unknown") return;

            SpaceMap = new Map.Map { Key = Key, ActivePilot = Name, SelectedSolarSystemName = Location.SolarSystemName };
            Global.MapApiFunctions.PublishSolarSystem(Name, Key, null, LocationCurrentSystemName);
            SpaceMap.Update();

            SpaceMap.Activate(Name, Location.SolarSystemName);

            OnEnterToSolarSystem += SpaceMap.RelocatePilot;

            if (SpaceMap != null) ChangeLocation();

            aTimer = new Timer();
            aTimer.Elapsed += Event_Refresh;
            aTimer.Interval = 5000;
            aTimer.Enabled = true;
        }

        private void Event_Refresh(object sender, ElapsedEventArgs e)
        {
            RefreshInfo();
        }

        public string RefreshToken { get; set; }

        public string LocationCurrentSystemName { get; set; }

        public string LocationPreviousSystemName { get; set; }

        private void ReInitialization(string id, string refreshToken)
        {
            Log.DebugFormat("[Pilot.ReInitialization] starting for id = {0} refreshToken = {1}", id, refreshToken);

            CrestData = new CrestAuthorization(refreshToken, Global.Settings.CCPSSO_AUTH_CLIENT_ID, Global.Settings.CCPSSO_AUTH_CLIENT_SECRET);

            CrestData.Refresh(refreshToken);

            dynamic data = CrestData.ObtainingCharacterData();

            Id = data.CharacterID;
            Name = data.CharacterName;

            LoadLocationInfo();

            LoadCharacterInfo();

            _lastTokenUpdate = DateTime.Now;
        }

        private void Initialization(string token)
        {
            Log.DebugFormat("[Pilot.Initialization] starting for token = {0}", token);

            CrestData = new CrestAuthorization(token, Global.Settings.CCPSSO_AUTH_CLIENT_ID, Global.Settings.CCPSSO_AUTH_CLIENT_SECRET);

            RefreshToken = CrestData.RefreshToken;

            dynamic data = CrestData.ObtainingCharacterData();

            

            Id = data.CharacterID;
            Name = data.CharacterName;

            LoadLocationInfo();

            if(Key == null) Key = Name;

            if(Location.SolarSystemName != "unknown")
            {
                SpaceMap = new Map.Map { Key = Key, ActivePilot = Name, SelectedSolarSystemName = Location.SolarSystemName };
                Global.MapApiFunctions.PublishSolarSystem(Name, Key, null, LocationCurrentSystemName);
                SpaceMap.Update();

                SpaceMap.Activate(Name, Location.SolarSystemName);

                OnEnterToSolarSystem += SpaceMap.RelocatePilot;
            }

            LoadCharacterInfo();

            _lastTokenUpdate = DateTime.Now;

        }

        private void LoadCharacterInfo()
        {
            Log.DebugFormat("[Pilot.LoadCharacterInfo] starting for Id = {0}", Id);

            dynamic characterInfo = CrestData.GetCharacterInfo(Id);

            var portraitAddress = characterInfo.SelectToken("portrait.64x64.href").Value;

            var request = WebRequest.Create(portraitAddress);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                Portrait = Image.FromStream(stream);
            }
        }

        private void LoadLocationInfo()
        {
            Log.DebugFormat("[Pilot.LoadLocationInfo] starting for Id = {0}", Id);

            _isBusy = true;

            var isNeedChangeLocation = false;

            try
            {
                if (Location == null)
                {
                    Location = new StarSystemEntity();
                }

                _commandsLog.DebugFormat("[Pilot {0}] Call CrestData.GetLocation with ID={1}", Name, Id);

                dynamic locationInfo = CrestData.GetLocation(Id);

                if (Location.SolarSystemName == locationInfo.SelectToken("solarSystem.name").Value)
                {
                    _commandsLog.DebugFormat("[Pilot {0}] No need change location {1}", Name, Location.SolarSystemName);
                    return;
                }

                if (locationInfo.SelectToken("solarSystem.id").Value != null)
                {
                    if (LocationCurrentSystemName != locationInfo.SelectToken("solarSystem.name").Value)
                    {
                        LocationPreviousSystemName = LocationCurrentSystemName;
                        LocationCurrentSystemName = locationInfo.SelectToken("solarSystem.name").Value;

                        isNeedChangeLocation = true;
                    }

                }

                if (Global.Space.SolarSystems.ContainsKey(locationInfo.SelectToken("solarSystem.name").Value.ToString()))
                {
                    var location = (StarSystemEntity)Global.Space.SolarSystems[locationInfo.SelectToken("solarSystem.name").Value.ToString()];

                    Location = location.Clone() as StarSystemEntity;

                    if (Location != null)
                    {
                        Location.Id = locationInfo.SelectToken("solarSystem.id").Value.ToString();
                    }
                }
                else
                {
                    Location.Region = "";
                    Location.Constelation = "";
                    Location.Effect = "";
                    Location.Class = "";
                    Location.Static2 = "";
                    Location.Static = "";

                    Location.Id = locationInfo.SelectToken("solarSystem.id").Value.ToString();

                    Location.SolarSystemName = locationInfo.SelectToken("solarSystem.name").Value;

                }

                if ( isNeedChangeLocation )
                {
                    ChangeLocation();
                }

            }
            catch (Exception ex)
            {
                Log.DebugFormat("[Pilot.LoadLocationInfo] pilot Id = {0} not login in game. Exception {1}", Id, ex);

                if (Location != null)
                {
                    Location.SolarSystemName = "unknown";
                }
            }
            finally
            {
                _isBusy = false;
            }
        }

        private bool _isBusy;

        private void RefreshInfo()
        {
            Log.DebugFormat("[Pilot.RefreshInfo] starting for Id = {0}", Id);

            var span = DateTime.Now - _lastTokenUpdate;
            var ms = (int)span.TotalMilliseconds;

            if (ms > CrestData.ExpiresIn * 1000 - 20000)
            {
                CrestData.Refresh();

                _lastTokenUpdate = DateTime.Now;

                Log.DebugFormat("[Pilot.RefreshInfo] set LastTokenUpdate for Id = {0}", Id);
            }

            if (_isBusy == false)
            {
                _commandsLog.DebugFormat("[Pilot '{0}'] Load location info.", Name);
                LoadLocationInfo();
            }
        }


        public void ChangeLocation()
        {
            if (Key == null) return;

            _commandsLog.InfoFormat("[Pilot '{3}'] Publish system with key {0} from {1} to {2} ", Name, LocationPreviousSystemName, LocationCurrentSystemName, Name);

            SpaceMap.Publish(Name, LocationPreviousSystemName, LocationCurrentSystemName);

            if (OnChangeSolarSystem == null) return;

            _commandsLog.InfoFormat("[Pilot '{3}'] Call OnChangeSolarSystem with key after publish {0} from {1} to {2} ", Name, LocationPreviousSystemName, LocationCurrentSystemName, Name);

            try
            {
                SpaceMap.SelectedSolarSystemName = LocationCurrentSystemName;
                Global.Presenter.ActivatePilot(Name);
                if(OnChangeSolarSystem != null) OnChangeSolarSystem(this, LocationPreviousSystemName, LocationCurrentSystemName);
                if(OnEnterToSolarSystem != null)  OnEnterToSolarSystem(Name, LocationPreviousSystemName, LocationCurrentSystemName);
            }
            catch (Exception exception)
            {
                Log.ErrorFormat("[PilotEntity.ChangeLocation] Critical error = {0}", exception);
            }
        }
    }
}
