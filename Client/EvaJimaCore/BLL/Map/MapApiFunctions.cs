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
            var address = _mapServerAddress + "/api/PublishSolarSystem?pilot=" + pilotName + "&mapKey=" + key + "&systemFrom=" + systemFrom + "&systemTo=" + systemTo;

            _apiCallsLog.Info(address);

            try
            {
                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    var updatedSystems = JsonConvert.DeserializeObject<List<SolarSystem>>(JsonConvert.DeserializeObject(dataVerification).ToString());

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
            try
            {
                var address = _mapServerAddress + "/api/map?key=" + key + "";

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

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
            try
            {
                var address = _mapServerAddress + "/api/UpdateSolarSystemCoordinates?mapKey=" + key + "&system=" + system + "&pilot=" + pilot + "&positionX=" + positionX + "&positionY=" + positionY + "";

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    _commandsLog.DebugFormat("[MapApiFunctions.UpdateSolarSystemCoordinates] Change solar system coordinates complete. Point = {0} SolarSystemName = {1} ", system, key);

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
            try
            {
                var address = _mapServerAddress + "/api/DeleteSolarSystem?mapKey=" + key + "&system=" + system + "&pilotName=" + pilot;

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    _commandsLog.InfoFormat("[Map.DeleteSolarSystem] Delete Solar System {2} with map key ='{0}' for pilot ='{1}'", key, pilot, system);

                    var updatedSystems = JsonConvert.DeserializeObject<List<SolarSystem>>(JsonConvert.DeserializeObject(dataVerification).ToString());

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
            var address = _mapServerAddress + "/api/GetUpdates?mapKey=" + key + "&pilot=" + pilot + "&ticks=" + delta + "";

            _apiCallsLog.Info(address);

            using (var client = new WebClient())
            {
                var dataVerification = client.DownloadString(address);

                return JsonConvert.DeserializeObject<List<SolarSystem>>(JsonConvert.DeserializeObject(dataVerification).ToString());
            }
        }

        public List<SolarSystem> GetDeletes(string key, string pilot, long delta)
        {
            try
            {
                var address = _mapServerAddress + "/api/GetUpdates?mapKey=" + key + "&pilot=" + pilot + "&ticks=" + delta + "&deleted=true";

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

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
            var address = _mapServerAddress + "/api/GetUpdatesPilotes?mapKey=" + key + "&ticks=" + delta + "&pilot=" + pilot;

            _apiCallsLog.Info(address);

            using (var client = new WebClient())
            {
                var dataVerification = client.DownloadString(address);

                return JsonConvert.DeserializeObject<List<PilotLocation>>(JsonConvert.DeserializeObject(dataVerification).ToString());
            }
        }

        public string PublishSignature(string pilotName, string key, string systemName, string type, string code, string name)
        {
            var address = _mapServerAddress + "/api/Signatures?mapKey=" + key + "&systemName=" + systemName + "&type=" + type + "&code=" + code + "&name=" + name + "";

            _apiCallsLog.Info(address);

            using (var client = new WebClient())
            {
                var dataVerification = client.DownloadString(address);

                return dataVerification;
            }
        }

        public string PublishDeadLetter(string mapKey, string pilot, string systemFrom, string systemTo)
        {
            try
            {
                var address = _mapServerAddress + "/api/Signatures?mapKey=" + mapKey + "&pilot=" + pilot + "&systemFrom=" + systemFrom + "&systemTo=" + systemTo + "";

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    _commandsLog.InfoFormat("[MapApiFunctions.PublishDeadLetter] PublishDeadLetter in system {2}, previous syste, {3} with map key ='{0}' for pilot ='{1}'", mapKey, pilot, systemTo, systemFrom);

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
            try
            {
                var signaturesJson = JsonConvert.SerializeObject(signatures, Formatting.Indented);

                var address = _mapServerAddress + "/api/PublishSignatures?pilotName=" + pilotName + "&key=" + key + "&system=" + system + "&signatures=" + signaturesJson;

                _apiCallsLog.Info(address);

                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    var updatedSystems = JsonConvert.DeserializeObject<List<SolarSystem>>(JsonConvert.DeserializeObject(dataVerification).ToString());

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
            var address = _mapServerAddress + "/api/DeleteSignature?pilotName=" + pilotName + "&key=" + key + "&system=" + system + "&code=" + code;

            _apiCallsLog.Info(address);

            using (var client = new WebClient())
            {
                var dataVerification = client.DownloadString(address);

                return dataVerification;
            }
        }

        public List<SolarSystem> DeleteConnectionBetweenSolarSystems(string pilotName, string key, string systemFrom, string systemTo)
        {
            var address = _mapServerAddress + "/api/DeathNotice?mapKey=" + key + "&pilot=" + pilotName + "&solarSystemFrom=" + systemFrom + "&solarSystemTo=" + systemTo;

            _apiCallsLog.Info(address);

            try
            {
                using (var client = new WebClient())
                {
                    var dataVerification = client.DownloadString(address);

                    var updatedSystems = JsonConvert.DeserializeObject<List<SolarSystem>>(JsonConvert.DeserializeObject(dataVerification).ToString());

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
