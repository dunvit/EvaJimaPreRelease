using System;
using System.Collections.Generic;
using System.Net;
using EveJimaUniverse;
using log4net;
using Newtonsoft.Json;

namespace EveJimaCore.BLL.Map
{
    public class MapApiFunctions
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MapApiFunctions));

        readonly ILog _commandsLog = LogManager.GetLogger("CommandsMap");
        readonly ILog _errorsLog = LogManager.GetLogger("Errors");
        readonly ILog _apiCallsLog = LogManager.GetLogger("ApiCalls");
        //ApiCalls

        private string _mapServerAddress = "";//"http://www.evajima-maps.somee.com";// "http://localhost:51135"; //

        public void Initialization(string mapServerAddress)
        {
            _mapServerAddress = mapServerAddress;
        }

        public List<SolarSystem> PublishSolarSystem(string pilotName, string key, string systemFrom, string systemTo)
        {
            Log.DebugFormat("[MapApiFunctions.PublishSolarSystem] start");
            var address = _mapServerAddress + "/api/PublishSolarSystem?pilot=" + pilotName + "&mapKey=" + key + "&systemFrom=" + systemFrom + "&systemTo=" + systemTo;

            _apiCallsLog.Info(address);

            try
            {
                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    var updatedSystems = JsonConvert.DeserializeObject<List<SolarSystem>>(JsonConvert.DeserializeObject(dataVerification).ToString());

                    Log.DebugFormat("[MapApiFunctions.PublishSolarSystem] end");
                    return updatedSystems;
                }
            }
            catch(Exception ex)
            {
                _errorsLog.ErrorFormat("[.PublishSolarSystem] Critical error. Address '{0}' Exception {1}", address, ex);
            }

            return null;
        }

        public Map LoadMap(string key, string system, string pilot)
        {

            try
            {
                var address = _mapServerAddress + "/api/map?key=" + key + "&system=" + system + "&pilot=" + pilot + "";

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    return JsonConvert.DeserializeObject<Map>(JsonConvert.DeserializeObject(dataVerification).ToString());
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public string GetMapOwner(string key)
        {
            Log.DebugFormat("[MapApiFunctions.GetMapOwner] start");
            try
            {
                var address = _mapServerAddress + "/api/map?key=" + key + "";

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    Log.DebugFormat("[MapApiFunctions.GetMapOwner] end");

                    return dataVerification.Replace("\"","");
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        public string UpdateSolarSystemCoordinates(string key, string system, string pilot, int positionX, int positionY)
        {
            Log.DebugFormat("[MapApiFunctions.UpdateSolarSystemCoordinates] start");

            try
            {
                var address = _mapServerAddress + "/api/UpdateSolarSystemCoordinates?mapKey=" + key + "&system=" + system + "&pilot=" + pilot + "&positionX=" + positionX + "&positionY=" + positionY + "";

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    _commandsLog.DebugFormat("[MapApiFunctions.UpdateSolarSystemCoordinates] Change solar system coordinates complete. Point = {0} SolarSystemName = {1} ", system, key);

                    Log.DebugFormat("[MapApiFunctions.UpdateSolarSystemCoordinates] end");

                    return dataVerification;
                }
            }
            catch (Exception ex)
            {
                _errorsLog.ErrorFormat("[MapApiFunctions.UpdateSolarSystemCoordinates] Point = {1} SolarSystemName = {2} Critical error {0}", ex, system, key);
            }

            return string.Empty;
        }

        public List<SolarSystem> DeleteSolarSystem(string key, string system, string pilot)
        {
            Log.DebugFormat("[MapApiFunctions.DeleteSolarSystem] start");

            try
            {
                var address = _mapServerAddress + "/api/DeleteSolarSystem?mapKey=" + key + "&system=" + system + "&pilotName=" + pilot;

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    _commandsLog.InfoFormat("[Map.DeleteSolarSystem] Delete Solar System {2} with map key ='{0}' for pilot ='{1}'", key, pilot, system);

                    var updatedSystems = JsonConvert.DeserializeObject<List<SolarSystem>>(JsonConvert.DeserializeObject(dataVerification).ToString());

                    Log.DebugFormat("[MapApiFunctions.DeleteSolarSystem] end");

                    return updatedSystems;
                }
            }
            catch(Exception ex)
            {
                _commandsLog.ErrorFormat("[Map.DeleteSolarSystem] Critical error - Delete Solar System {2} with map key ='{0}' for pilot ='{1}' Exception {3}", key, pilot, system, ex);

                return new List<SolarSystem>();
            }
            
        }

        public List<SolarSystem> GetUpdates(string key, string pilot, long delta)
        {
            Log.DebugFormat($"[MapApiFunctions.GetUpdates] start for '{pilot}' and map '{key}'");

            var address = _mapServerAddress + "/api/GetUpdates?mapKey=" + key + "&pilot=" + pilot + "&ticks=" + delta + "";

            _apiCallsLog.Info(address);

            using (var client = new WebClient())
            {
                var dataVerification = client.DownloadString(address);

                Log.DebugFormat($"[MapApiFunctions.GetUpdates] end for '{pilot}' and map '{key}'");

                return JsonConvert.DeserializeObject<List<SolarSystem>>(JsonConvert.DeserializeObject(dataVerification).ToString());
            }
        }

        public List<SolarSystem> GetDeletes(string key, string pilot, long delta)
        {
            Log.DebugFormat("[MapApiFunctions.GetDeletes] start");

            try
            {
                var address = _mapServerAddress + "/api/GetUpdates?mapKey=" + key + "&pilot=" + pilot + "&ticks=" + delta + "&deleted=true";

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    Log.DebugFormat("[MapApiFunctions.GetDeletes] end");

                    return JsonConvert.DeserializeObject<List<SolarSystem>>(JsonConvert.DeserializeObject(dataVerification).ToString());
                }
            }
            catch(Exception)
            {
                
                throw;
            }


            
        }

        public List<PilotLocation> GetPilotes(string key, long delta, string pilot)
        {
            Log.DebugFormat("[MapApiFunctions.GetPilotes] start");

            var address = _mapServerAddress + "/api/GetUpdatesPilotes?mapKey=" + key + "&ticks=" + delta + "&pilot=" + pilot;

            _apiCallsLog.Info(address);

            using (var client = new WebClient())
            {
                var dataVerification = client.DownloadString(address);

                Log.DebugFormat("[MapApiFunctions.GetPilotes] end");

                return JsonConvert.DeserializeObject<List<PilotLocation>>(JsonConvert.DeserializeObject(dataVerification).ToString());
            }
        }

        public string PublishSignature(string pilotName, string key, string systemName, string type, string code, string name)
        {
            Log.DebugFormat("[MapApiFunctions.PublishSignature] start");

            var address = _mapServerAddress + "/api/Signatures?mapKey=" + key + "&systemName=" + systemName + "&type=" + type + "&code=" + code + "&name=" + name + "";

            _apiCallsLog.Info(address);

            using (var client = new WebClient())
            {
                var dataVerification = client.DownloadString(address);

                Log.DebugFormat("[MapApiFunctions.PublishSignature] end");

                return dataVerification;
            }
        }

        public string PublishDeadLetter(string mapKey, string pilot, string systemFrom, string systemTo)
        {
            Log.DebugFormat("[MapApiFunctions.PublishDeadLetter] start");

            try
            {
                var address = _mapServerAddress + "/api/Signatures?mapKey=" + mapKey + "&pilot=" + pilot + "&systemFrom=" + systemFrom + "&systemTo=" + systemTo + "";

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    _commandsLog.InfoFormat("[MapApiFunctions.PublishDeadLetter] PublishDeadLetter in system {2}, previous syste, {3} with map key ='{0}' for pilot ='{1}'", mapKey, pilot, systemTo, systemFrom);

                    Log.DebugFormat("[MapApiFunctions.PublishDeadLetter] end");

                    return dataVerification;
                }
            }
            catch (Exception ex)
            {
                _errorsLog.ErrorFormat("[MapApiFunctions.PublishDeadLetter] PublishDeadLetter in system {2}, previous syste, {3} with map key ='{0}' for pilot ='{1}' exception is {4}", mapKey, pilot, systemTo, systemFrom, ex);
                return "Failure";
            }
        }

        public List<SolarSystem> PublishSignatures(string pilotName, string key, string system, List<CosmicSignature> signatures)
        {
            Log.DebugFormat("[MapApiFunctions.PublishSignatures] start");

            try
            {
                var signaturesJson = JsonConvert.SerializeObject(signatures, Formatting.Indented);

                var address = _mapServerAddress + "/api/PublishSignatures?pilotName=" + pilotName + "&key=" + key + "&system=" + system + "&signatures=" + signaturesJson;

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    var updatedSystems = JsonConvert.DeserializeObject<List<SolarSystem>>(JsonConvert.DeserializeObject(dataVerification).ToString());

                    Log.DebugFormat("[MapApiFunctions.PublishSignatures] end");

                    return updatedSystems;
                }
            }
            catch(Exception ex)
            {
                _errorsLog.ErrorFormat("[MapApiFunctions.PublishSignatures] MapKey = {3} SolarSystemName = {2} Signatures = {1}  Critical error {0}", ex, signatures, system, key);
            }

            return null;
        }

        public string DeleteSignature(string pilotName, string key, string system, string code)
        {
            Log.DebugFormat("[MapApiFunctions.DeleteSignature] start");

            var address = _mapServerAddress + "/api/DeleteSignature?pilotName=" + pilotName + "&key=" + key + "&system=" + system + "&code=" + code;

            _apiCallsLog.Info(address);

            using (var client = new WebClient())
            {
                var dataVerification = client.DownloadString(address);

                Log.DebugFormat("[MapApiFunctions.DeleteSignature] end");

                return dataVerification;
            }
        }

        public List<SolarSystem> DeleteConnectionBetweenSolarSystems(string pilotName, string key, string systemFrom, string systemTo)
        {
            Log.DebugFormat("[MapApiFunctions.DeleteConnectionBetweenSolarSystems] start");

            var address = _mapServerAddress + "/api/DeathNotice?mapKey=" + key + "&pilot=" + pilotName + "&solarSystemFrom=" + systemFrom + "&solarSystemTo=" + systemTo;

            _apiCallsLog.Info(address);

            try
            {
                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    var updatedSystems = JsonConvert.DeserializeObject<List<SolarSystem>>(JsonConvert.DeserializeObject(dataVerification).ToString());

                    Log.DebugFormat("[MapApiFunctions.DeleteConnectionBetweenSolarSystems] end");

                    return updatedSystems;
                }
            }
            catch (Exception ex)
            {
                _errorsLog.ErrorFormat("[MapApiFunctions.DeleteConnectionBetweenSolarSystems] MapKey = {3} systemFrom = {2} systemTo = {1}  Critical error {0}", ex, systemTo, systemFrom, key);
            }

            return null;
        }
    }
}
