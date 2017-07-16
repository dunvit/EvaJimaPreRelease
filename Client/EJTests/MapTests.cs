using System;
using System.Collections.Generic;
using System.Threading;
using EvaJimaCore;
using EveJimaCore;
using EveJimaCore.BLL.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EveJimaUniverse;

namespace EJTests
{
    [TestClass]
    public class MapTests
    {
        public string Server_MapAddress = "http://localhost:51135"; // "http://www.evajima-storage.somee.com";

        private string log = "";

        private void InitializationGlobalFunctions()
        {
            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);
            Global.ApplicationSettings = new ApplicationSettings();

            log4net.Config.XmlConfigurator.Configure();
        }

        private Map Initialization(string activePilotName, string Key)
        {
            InitializationGlobalFunctions();

            var map = new Map { ActivePilot = activePilotName, Key = Key };

            map.OnChangeStatus += AddLogMessage;

            return map;
        }

        [TestMethod]
        public void DeathNotice_Test()
        {
            var name = "threads_" + DateTime.UtcNow.Ticks;

            const string pilotFirst = "Scarlett Orwell";

            var map = Initialization(pilotFirst, name);

            map.ApiPublishSolarSystem(pilotFirst, name, "", "J213734");

            var updatedSystem = map.GetSystem("J213734");

            Assert.AreEqual(updatedSystem.LocationInMap.X, 5000);
            Assert.AreEqual(updatedSystem.LocationInMap.Y, 5000);

            Assert.AreEqual(map.ApiPublishSolarSystem(pilotFirst, name, "J213734", "J165920").UpdatedSystems, 2);

            Assert.AreEqual(map.ApiPublishSolarSystem(pilotFirst, name, "J165920", "Jita").UpdatedSystems, 2);

            map.RelocatePilot(pilotFirst, "J165920", "Jita");

            Assert.AreEqual(map.ApiPublishDeathNotice("Jita").UpdatedSystems, 2);

            Assert.AreEqual(map.GetSystem("Jita").Connections.Count, 0);

            Assert.AreEqual(map.GetSystem("J165920").Connections.Count, 1);

        }

        private void AddLogMessage(string obj)
        {
            log = log + Environment.NewLine + "[" + $"{DateTime.UtcNow:MM/dd/yyyy  hh:mm:ss}" + "] " + obj;
        }

        [TestMethod]
        public void LoadMapTest()
        {
            var name = "threads_" + DateTime.UtcNow.Ticks;
            const string pilotFirst = "Scarlett Orwell";

            var map = Initialization(pilotFirst, name);

            var retValue = map.ApiPublishSolarSystem(pilotFirst, name, "", "J213734");

            Assert.AreEqual(retValue.UpdatedSystems, 1);

            var owner = map.GetOwner();

            Assert.AreEqual(owner, pilotFirst);

            Global.MapApiFunctions.UpdateMap(map);
        }

        [TestMethod]
        public void LoadRealMap()
        {
            const string name = "CJQ_2000";
            const string pilotFirst = "Scarlett Orwell";

            var map = Initialization(pilotFirst, name);

            var retValue = map.ApiPublishSolarSystem(pilotFirst, name, "", "J213734");

            Assert.AreNotEqual(retValue.UpdatedSystems, 0);

            Global.MapApiFunctions.UpdateMap(map);
        }

        [TestMethod]
        public void GetMapOwnerTest()
        {
            var name = "threads_" + DateTime.UtcNow.Ticks;

            const string pilotFirst = "Scarlett Orwell";
            const string pilotSecond = "Dana Su-Shiloff";

            var map = Initialization(pilotFirst, name);

            Assert.AreEqual(map.ApiPublishSolarSystem(pilotFirst, name, "", "J213734").UpdatedSystems, 1);
            Assert.AreEqual(map.ApiPublishSolarSystem(pilotFirst, name, "J213734", "J165920").UpdatedSystems, 2);
            
            Assert.AreEqual(map.Pilotes.Count, 1);

            Assert.AreEqual(map.ApiPublishSolarSystem(pilotSecond, name, "", "J213734").UpdatedSystems, 2);

            Assert.AreEqual(map.Pilotes.Count, 2);

            var mapOwner = map.GetOwner();

            Assert.AreEqual(pilotFirst, mapOwner);
        }

        [TestMethod]
        public void DeleteSolarSystemTest()
        {
            var name = "threads_" + DateTime.UtcNow.Ticks;

            const string pilotFirst = "Scarlett Orwell";
            const string pilotSecond = "Dana Su-Shiloff";

            var mapFirst = Initialization(pilotFirst, name);

            Assert.AreEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "", "Jita").UpdatedSystems, 1);
            Thread.Sleep(2000);

            Assert.AreEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "Jita", "Hek").UpdatedSystems, 2);
            Thread.Sleep(2000);

            var result = mapFirst.ApiDeleteSolarSystem(name, "Jita", pilotFirst);

            Assert.AreEqual(result.UpdatedSystems, 1);
            Assert.AreEqual(result.DeletedSustems, 1);
            Thread.Sleep(2000);
        }

        [TestMethod]
        public void FullFlowMapTest()
        {
            var name = "threads_" + DateTime.UtcNow.Ticks;

            RunFillMap(name);
        }

        private void RunFillMap(string name)
        {
            const string pilotFirst = "Scarlett Orwell";
            const string pilotSecond = "Dana Su-Shiloff";

            var mapFirst = Initialization(pilotFirst, name);

            Assert.AreEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "", "J213734").UpdatedSystems, 1);
            Thread.Sleep(2000);
            Assert.AreEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "J213734", "J165920").UpdatedSystems, 2);
            Thread.Sleep(2000);
            Assert.AreEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "J165920", "J165936").UpdatedSystems, 2);
            Thread.Sleep(2000);

            mapFirst.SetLastUpdate(new DateTime(2017, 5, 5));
            
            Global.MapApiFunctions.UpdateMap(mapFirst);

            Assert.AreEqual(mapFirst.Systems.Count, 3);

            var mapSecond = Initialization(pilotSecond, name);

            Assert.AreEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "", "J122635").UpdatedSystems, 1);
            Thread.Sleep(2000);

            Assert.AreEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "J122635", "J168936").UpdatedSystems, 2);
            Thread.Sleep(2000);

            
            Global.MapApiFunctions.UpdateMap(mapSecond);
            Global.MapApiFunctions.UpdateMap(mapFirst);

            Assert.AreEqual(mapSecond.Systems.Count, 2);

            Assert.AreEqual(mapFirst.Systems.Count, 3);

            Assert.AreEqual(mapFirst.Pilotes.Count, 2);
            Assert.AreEqual(mapSecond.Pilotes.Count, 2);

            Assert.AreEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "J168936", "J165920").UpdatedSystems, 5);
            Thread.Sleep(2000);

            Global.MapApiFunctions.UpdateMap(mapSecond);
            Global.MapApiFunctions.UpdateMap(mapFirst);

            Assert.AreEqual(mapSecond.Systems.Count, 5);
            Assert.AreEqual(mapFirst.Systems.Count, 5);

            var systemJ168936 = mapSecond.GetSystem("J168936");

            Assert.AreEqual(systemJ168936.Connections.Count, 2);

            var systemJ165920 = mapSecond.GetSystem("J165920");

            Assert.AreEqual(systemJ165920.Connections.Count, 3);

            Assert.AreEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "J213734", "J214318").UpdatedSystems, 2);
            Thread.Sleep(2000);

            //SignaturesTests(name, pilotFirst);
            //Thread.Sleep(2000);

            Assert.AreEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "J165920", "Jita").UpdatedSystems, 4);
            Thread.Sleep(2000);

            Assert.AreEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "Jita", "Hek").UpdatedSystems, 2);
            Thread.Sleep(2000);

            var deleteHistory = Global.MapApiFunctions.DeleteSolarSystem(mapFirst, "Jita");

            Assert.AreEqual(deleteHistory.DeletedSustems, 1);
            Assert.AreEqual(deleteHistory.UpdatedSystems, 2);
            Thread.Sleep(2000);

            mapSecond.ActivePilot = pilotFirst;

            mapSecond.LocationSolarSystemName = "J165920";

            Global.MapApiFunctions.UpdateMap(mapSecond);

            var systemHek = mapSecond.GetSystem("Hek");

            Assert.AreEqual(systemHek.IsHidden, true);

            Assert.AreEqual(mapSecond.GetSystem("J165920").IsHidden, false);
        }





        [TestMethod]
        public void SignaturesAdditionTests()
        {
            var key = "CJQ_2000";
            var pilot = "Scarlett Orwell";

            var map = Initialization(pilot, key);

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

           Global.MapApiFunctions.PublishSolarSystem(map, pilot, key, "", "J213734", new DateTime(2015, 5, 5).Ticks);
            Global.MapApiFunctions.PublishSolarSystem(map, pilot, key, "J213734", "J214318", new DateTime(2015, 5, 5).Ticks);

            SignaturesTests(key, pilot);
        }

        public void SignaturesTests(string mapKey, string pilot)
        {
            var name = mapKey;

            var mapFirst = Initialization(pilot, name);

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

            var signatures = new List<CosmicSignature>
            {
                new CosmicSignature { SolarSystemName = "J213734", Code = "KFG-768", Name = "To Hek", Type = SignatureType.WH },
                new CosmicSignature { SolarSystemName = "J213734", Code = "KFG-769", Name = "", Type = SignatureType.Relic },
                new CosmicSignature { SolarSystemName = "J213734", Code = "ABC-231", Name = "", Type = SignatureType.Gas }
            };

            Global.MapApiFunctions.PublishSignatures(mapFirst, pilot, name, "J213734", signatures);


            Assert.AreEqual(mapFirst.GetSystem("J213734").Signatures.Count, 3);

            signatures = new List<CosmicSignature>
            {
                new CosmicSignature { SolarSystemName = "J213734", Code = "UHT-116", Name = "", Type = SignatureType.Unknown },
                new CosmicSignature { SolarSystemName = "J213734", Code = "ABC-231", Name = "Bountiful Frontier Reservoir", Type = SignatureType.Gas }
            };

            Global.MapApiFunctions.PublishSignatures(mapFirst, pilot, name, "J213734", signatures);

            Assert.AreEqual(mapFirst.GetSystem("J213734").Signatures.Count, 2);

            signatures = new List<CosmicSignature>
            {
                new CosmicSignature { SolarSystemName = "J213734", Code = "KFG-769", Name = "", Type = SignatureType.Relic },
                new CosmicSignature { SolarSystemName = "J213734", Code = "UHT-116", Name = "", Type = SignatureType.Unknown },
                new CosmicSignature { SolarSystemName = "J213734", Code = "ABC-231", Name = "", Type = SignatureType.Unknown }
            };

            Global.MapApiFunctions.PublishSignatures(mapFirst, pilot, name, "J213734", signatures);

            Assert.AreEqual(mapFirst.GetSystem("J213734").Signatures.Count, 3);

            Global.MapApiFunctions.DeleteSignature(mapFirst, pilot, name, "J213734", "UHT-116");

            signatures = new List<CosmicSignature>
            {
                new CosmicSignature { SolarSystemName = "J214318", Code = "KFG-169", Name = "", Type = SignatureType.Relic },
                new CosmicSignature { SolarSystemName = "J214318", Code = "UHT-170", Name = "", Type = SignatureType.Unknown },
                new CosmicSignature { SolarSystemName = "J214318", Code = "ABC-171", Name = "", Type = SignatureType.Unknown }
            };

            Global.MapApiFunctions.PublishSignatures(mapFirst, pilot, name, "J214318", signatures);

            Assert.AreEqual(mapFirst.GetSystem("J214318").Signatures.Count, 2);
        }

        [TestMethod]
        public void CheckSystemTypeTests()
        {
            var name = DateTime.UtcNow.Ticks.ToString();

            name = "CJQ1_" + name;

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

            const string pilot = "Scarlett Orwell";

            var map = new Map { ActivePilot = pilot, Key = name };


            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "", "J213734", map.GetLastUpdate()), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "J213734", "Hek", map.GetLastUpdate()), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "Hek", "Jita", map.GetLastUpdate()), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "Jita", "Uedama", map.GetLastUpdate()), "\"Ok\"");

            

            Global.MapApiFunctions.UpdateMap(map);
        }

        [TestMethod]
        public void PilotRelocationTests()
        {
            var name = DateTime.UtcNow.Ticks.ToString();

            name = "PilotRelocation_" + name;

            const string pilot = "Scarlett Orwell";

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

            var map = new Map { ActivePilot = pilot, Key = name };

            var delta = DateTime.UtcNow.Ticks;

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map ,pilot, name, "", "J200000", delta), "\"Ok\"");

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "", "J300000", delta), "\"Ok\"");
        }

        [TestMethod]
        public void CreateMap()
        {
            var name = DateTime.UtcNow.Ticks.ToString();

            name = "CJQ_" + name;

            var pilot = "Scarlett Orwell";

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

            var map = new Map { ActivePilot = pilot, Key = name };

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "", "J213734", map.GetLastUpdate()), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "J213734", "J165920", map.GetLastUpdate()), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "J165920", "J165936", map.GetLastUpdate()), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "J165920", "J165943", map.GetLastUpdate()), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "J165943", "J165953", map.GetLastUpdate()), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "J165943", "J165954", map.GetLastUpdate()), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "J165943", "J165955", map.GetLastUpdate()), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "J165943", "J165956", map.GetLastUpdate()), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "J165943", "J165957", map.GetLastUpdate()), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "J165943", "J165958", map.GetLastUpdate()), "\"Ok\"");

            

            Global.MapApiFunctions.UpdateMap(map);

            Assert.AreEqual(map.Systems.Count, 10);

            Assert.AreEqual(Global.MapApiFunctions.DeleteSolarSystem(map, "J165953").UpdatedSystems, 1);

            Global.MapApiFunctions.UpdateMap(map);

            Assert.AreEqual(map.Systems.Count, 9);

            var system = map.GetSystem("J165943");

            Assert.AreEqual(system.Connections.Count, 6);

            var delta = DateTime.UtcNow.Ticks;

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(map, pilot, name, "J165943", "J165014", delta), "\"Ok\"");

            Assert.AreEqual(map.Systems, 2);
        }


    }
}
