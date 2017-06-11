using System;
using System.Collections.Generic;
using System.Drawing;
using EveJimaUniverse;
using log4net;
using Newtonsoft.Json;

namespace EveJimaServerMap
{
    public class Router
    {
        readonly ILog _log = LogManager.GetLogger("All");
        readonly ILog _commandsLog = LogManager.GetLogger("Commands");

        public Server Server { get; set; }

        public Router(string type)
        {
            switch(type)
            {
                case "server":
                    Server = new Server(MapType.Server);
                    break;

                case "client":
                    Server = new Server(MapType.Client);
                    break;

            }
        }

        public string LoadMap(string key, string system, string pilot)
        {
            try
            {
                Server.RelocatePilot(key, pilot, system);

                var mapObject = Server.GetMap(key, pilot);

                var map = JsonConvert.SerializeObject(mapObject);

                _commandsLog.InfoFormat("[Map] Load map with key {0}", key);

                return map;
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[Map] Load map with key {0} exception {1}", key, ex);
                return "Failure";
            }
        }

        public string GetMapOwner(string key)
        {
            try
            {
                var mapOwner = Server.GetMapOwner(key);

                _commandsLog.InfoFormat("[Map.GetMapOwner] Return map owner {1} with map key {0}", key, mapOwner);

                return mapOwner;
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[Map.GetMapOwner] With key {0} exception {1}", key, ex);
                return "Failure";
            }
        }

        public string DeleteSignature(string pilotName, string key, string system, string code)
        {
            try
            {
                var map = Server.GetMap(key, pilotName);

                map.DeleteSignature(system, code);

                _commandsLog.InfoFormat("[DeleteSignature] For map with key {0} system {1} signature code {2}", key, system, code);

                map.Save();

                return "Ok";
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[DeleteSignature] Critical error with map key {0} system {2} exception {1}", key, ex, system);
                return "Failure";
            }

        }

        public string DeleteSolarSystem(string mapKey, string system, string pilotName)
        {
            try
            {
                Server.GetMap(mapKey, pilotName).DeleteSolarSystem(system);

                _commandsLog.InfoFormat("[DeleteSolarSystem] Delete solar system {1} on map with key {0} ", mapKey, system);

                return "Ok";
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[DeleteSolarSystem] Delete solar system {2} on map with key {0} exception {1}", mapKey, ex, system);
                return "Failure";
            }
        }

        public string GetUpdatedSystems(string mapKey, string pilot, long ticks)
        {
            try
            {
                var dtTime = new DateTime(ticks);

                return JsonConvert.SerializeObject(Server.GetMap(mapKey, pilot).GetUpdates(dtTime));
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[GetUpdatedSystems] Get updates (systems) with key {0} ticks {2} exception {1}", mapKey, ex, ticks);
                return "Failure";
            }
        }

        //DeathNotice
        public string DeathNotice(string mapKey, string pilot, string systemFrom, string systemTo)
        {
            try
            {
                var map = Server.GetMap(mapKey, pilot, systemFrom, systemTo);

                map.DeleteSolarSystemConnection(systemTo, systemFrom);

                return "Ok";
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[DeathNotice] Death notice with key {0} systemFrom {2} systemTo {3} exception {1}", mapKey, ex, systemFrom, systemTo);
                return "Failure";
            }
        }

        public string GetDeletedSystems(string mapKey, string pilot, long ticks, bool deleted)
        {
            try
            {
                return JsonConvert.SerializeObject(Server.GetMap(mapKey, pilot).GetDeleted(new DateTime(ticks)));
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[GetDeletedSystems] Get updates (deleted systems) with key {0} ticks {2} exception {1}", mapKey, ex, ticks);

                return "Failure";
            }
        }

        public string GetUpdatedPilotes(string mapKey, long ticks, string pilot)
        {
            try
            {
                return JsonConvert.SerializeObject(Server.GetPilotes(mapKey, new DateTime(ticks)));
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[MapUpdates] Get updates (pilotes) with key {0} exception {1}", mapKey, ex);

                return "Failure";
            }
        }

        public string PublishSolarSystem(string pilot, string mapKey, string systemFrom, string systemTo)
        {
            try
            {
                // First login to EveJima
                if ( systemFrom == null )
                {
                    Server.RelocatePilot(mapKey, pilot, systemTo);
                }
                
                _commandsLog.InfoFormat("[PublishSolarSystem] Relocate pilot '{3}' system with key '{0}' from '{1}' to '{2}' ", mapKey, systemFrom, systemTo, pilot);

                var map = Server.GetMap(mapKey, pilot, systemFrom, systemTo);

                map.AddSolarSystem(systemFrom, systemTo);

                _commandsLog.InfoFormat("[PublishSolarSystem] Publish system with key {0} from {1} to {2} for pilot {3}", mapKey, systemFrom, systemTo, pilot);

                Server.RelocatePilot(mapKey, pilot, systemTo);


                return "Ok";
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[PublishSolarSystem] Publish system with key {0} from {1} to {2} exception {3}", mapKey, systemFrom, systemTo, ex);
                return "Failure";
            }

        }

        public string PublishSignatures(string pilotName, string key, string system, string signatures)
        {
            try
            {
                var map = Server.GetMap(key, pilotName);

                var listSignatures = JsonConvert.DeserializeObject<List<CosmicSignature>>(signatures);

                map.UpdateSignatures(system, listSignatures);

                _commandsLog.InfoFormat("[PublishSignatures] For map with key {0} system {1} ", key, system);

                map.Save();

                return "Ok";
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[PublishSignatures] Critical error with map key {0} system {2} exception {1}", key, ex, system);
                return "Failure";
            }
        }

        public string UpdateSolarSystemCoordinates(string mapKey, string system, string pilot, int positionX, int positionY)
        {
            try
            {
                var map = Server.GetMap(mapKey, pilot);

                var solarSystem = map.GetSystem(system);

                solarSystem.LocationInMap = new Point(positionX, positionY);

                _commandsLog.InfoFormat("[UpdateSolarSystemCoordinates] For map with key {0} system {2} set oordinates {1}", mapKey, positionX + ":" + positionY, system);

                map.Save();
                return "Ok";
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("[UpdateSolarSystemCoordinates] Load map with key {0} system {2} exception {1}", mapKey, ex, system);
                return "Failure";
            }
        }
    }
}
