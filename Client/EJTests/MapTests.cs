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

            var updatedSystems = map.ApiPublishSolarSystem(pilotFirst, name, "", "J213734");
            
            Assert.AreEqual(updatedSystems[0].LocationInMap.X, 5000);
            Assert.AreEqual(updatedSystems[0].LocationInMap.Y, 5000);

            Assert.AreEqual(map.ApiPublishSolarSystem(pilotFirst, name, "J213734", "J165920").Count, 2);

            map.Update();

            Assert.AreEqual(map.ApiPublishSolarSystem(pilotFirst, name, "J165920", "Jita").Count, 2);

            map.Update();

            map.RelocatePilot(pilotFirst, "J165920", "Jita");

            Assert.AreEqual(map.ApiPublishDeathNotice("Jita").Count, 2);

            map.Update();

            Assert.AreEqual(map.GetSystem("Jita").Connections.Count, 0);

            Assert.AreEqual(map.GetSystem("J165920").Connections.Count, 1);

        }

        private void AddLogMessage(string obj)
        {
            log = log + Environment.NewLine + "[" + string.Format("{0:MM/dd/yyyy  hh:mm:ss}", DateTime.UtcNow) + "] " + obj;
        }

        [TestMethod]
        public void LoadMapTest()
        {
            var name = "threads_" + DateTime.UtcNow.Ticks;
            const string pilotFirst = "Scarlett Orwell";

            var map = Initialization(pilotFirst, name);

            var retValue = map.ApiPublishSolarSystem(pilotFirst, name, "", "J213734");

            Assert.AreEqual(retValue.Count, 1);

            var owner = Global.MapApiFunctions.GetMapOwner(name);

            Assert.AreEqual(owner, pilotFirst);

            map.Update();
        }

        [TestMethod]
        public void LoadRealMap()
        {
            const string name = "CJQ_2000";
            const string pilotFirst = "Scarlett Orwell";

            var map = Initialization(pilotFirst, name);

            var retValue = map.ApiPublishSolarSystem(pilotFirst, name, "", "J213734");

            Assert.AreNotEqual(retValue.Count, 0);

            map.Update();
        }

        [TestMethod]
        public void GetMapOwnerTest()
        {
            var name = "threads_" + DateTime.UtcNow.Ticks;

            const string pilotFirst = "Scarlett Orwell";
            const string pilotSecond = "Dana Su-Shiloff";

            var map = Initialization(pilotFirst, name);

            Assert.AreEqual(map.ApiPublishSolarSystem(pilotFirst, name, "", "J213734").Count, 1);
            Assert.AreEqual(map.ApiPublishSolarSystem(pilotFirst, name, "J213734", "J165920").Count, 2);
            map.Update();
            Assert.AreEqual(map.Pilotes.Count, 1);

            Assert.AreEqual(map.ApiPublishSolarSystem(pilotSecond, name, "", "J213734").Count, 0);

            map.Update();

            Assert.AreEqual(map.Pilotes.Count, 2);

            var mapOwner = Global.MapApiFunctions.GetMapOwner(name);

            Assert.AreEqual(pilotFirst, mapOwner);
        }

        [TestMethod]
        public void DeleteSolarSystemTest()
        {
            var name = "threads_" + DateTime.UtcNow.Ticks;

            const string pilotFirst = "Scarlett Orwell";
            const string pilotSecond = "Dana Su-Shiloff";

            var mapFirst = Initialization(pilotFirst, name);

            Assert.AreEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "", "Jita").Count, 1);
            Thread.Sleep(2000);

            Assert.AreEqual(mapFirst.ApiPublishSolarSystem(pilotSecond, name, "Jita", "Hek").Count, 2);
            Thread.Sleep(2000);

            Assert.AreEqual(mapFirst.ApiDeleteSolarSystem(name, "Jita", pilotFirst).Count, 1);
            Thread.Sleep(2000);
        }

        private void RunFillMap(string name)
        {
            const string pilotFirst = "Scarlett Orwell";
            const string pilotSecond = "Dana Su-Shiloff";

            var mapFirst = Initialization(pilotFirst, name);

            Assert.AreEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "", "J213734").Count, 1);
            Thread.Sleep(2000);
            Assert.AreEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "J213734", "J165920").Count, 2);
            Thread.Sleep(2000);
            Assert.AreEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "J165920", "J165936").Count, 2);
            Thread.Sleep(2000);

            mapFirst.Update();

            Assert.AreEqual(mapFirst.Systems.Count, 3);

            var mapSecond = Initialization(pilotSecond, name);

            Assert.AreEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "", "J122635").Count, 1);
            Thread.Sleep(2000);

            Assert.AreEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "J122635", "J168936").Count, 2);
            Thread.Sleep(2000);

            mapSecond.Update();
            mapFirst.Update();

            Assert.AreEqual(mapSecond.Systems.Count, 2);

            Assert.AreEqual(mapFirst.Systems.Count, 3);

            Assert.AreEqual(mapFirst.Pilotes.Count, 2);
            Assert.AreEqual(mapSecond.Pilotes.Count, 2);

            Assert.AreEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "J168936", "J165920").Count, 5);
            Thread.Sleep(2000);

            mapSecond.Update();
            mapFirst.Update();

            Assert.AreEqual(mapSecond.Systems.Count, 5);
            Assert.AreEqual(mapFirst.Systems.Count, 5);

            var systemJ168936 = mapSecond.GetSystem("J168936");

            Assert.AreEqual(systemJ168936.Connections.Count, 2);

            var systemJ165920 = mapSecond.GetSystem("J165920");

            Assert.AreEqual(systemJ165920.Connections.Count, 3);

            Assert.AreEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "J213734", "J214318").Count, 2);
            Thread.Sleep(2000);

            //SignaturesTests(name, pilotFirst);
            //Thread.Sleep(2000);

            Assert.AreEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "J165920", "Jita").Count, 2);
            Thread.Sleep(2000);

            Assert.AreEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "Jita", "Hek").Count, 2);
            Thread.Sleep(2000);

            Assert.AreEqual(Global.MapApiFunctions.DeleteSolarSystem(name, "Jita", pilotSecond).Count, 7);
            Thread.Sleep(2000);

            mapSecond.ActivePilot = pilotFirst;

            mapSecond.LocationSolarSystemName = "J165920";

            mapSecond.Update();

            var systemHek = mapSecond.GetSystem("Hek");

            Assert.AreEqual(systemHek.IsHidden, true);

            Assert.AreEqual(mapSecond.GetSystem("J165920").IsHidden, false);
        }

        [TestMethod]
        public void FullFlowMapTest()
        {
            var name = "threads_" + DateTime.UtcNow.Ticks;

            RunFillMap(name);
        }

        [TestMethod]
        public void CurrentMapTest()
        {
            var name = "CJQ_2000";

            const string pilotFirst = "Scarlett Orwell";
            const string pilotSecond = "Dana Su-Shiloff";

            var mapFirst = Initialization(pilotFirst, name);

            Assert.AreNotEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "", "J213734").Count, 1000);
            Thread.Sleep(2000);
            Assert.AreNotEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "J213734", "J165920").Count, 1000);
            Thread.Sleep(2000);
            Assert.AreNotEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "J165920", "J165936").Count, 1000);
            Thread.Sleep(2000);

            mapFirst.Update();

            Assert.AreNotEqual(mapFirst.Systems.Count, 1000);

            var mapSecond = Initialization(pilotSecond, name);

            Assert.AreNotEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "", "J122635").Count, 1000);
            Thread.Sleep(2000);

            Assert.AreNotEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "J122635", "J168936").Count, 1000);
            Thread.Sleep(2000);

            mapSecond.Update();
            mapFirst.Update();

            Assert.AreNotEqual(mapSecond.Systems.Count, 1000);

            Assert.AreNotEqual(mapFirst.Systems.Count, 1000);

            Assert.AreNotEqual(mapFirst.Pilotes.Count, 1000);
            Assert.AreNotEqual(mapSecond.Pilotes.Count, 1000);

            Assert.AreNotEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "J168936", "J165920").Count, 1000);
            Thread.Sleep(2000);

            mapSecond.Update();
            mapFirst.Update();

            Assert.AreNotEqual(mapSecond.Systems.Count, 1000);
            Assert.AreNotEqual(mapFirst.Systems.Count, 1000);

            var systemJ168936 = mapSecond.GetSystem("J168936");

            Assert.AreNotEqual(systemJ168936.Connections.Count, 1000);

            var systemJ165920 = mapSecond.GetSystem("J165920");

            Assert.AreNotEqual(systemJ165920.Connections.Count, 1000);

            Assert.AreNotEqual(mapFirst.ApiPublishSolarSystem(pilotFirst, name, "J213734", "J214318").Count, 1000);
            Thread.Sleep(2000);

            //SignaturesTests(name, pilotFirst);
            //Thread.Sleep(2000);

            Assert.AreNotEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "J165920", "Jita").Count, 1000);
            Thread.Sleep(2000);

            Assert.AreNotEqual(mapSecond.ApiPublishSolarSystem(pilotSecond, name, "Jita", "Hek").Count, 1000);
            Thread.Sleep(2000);

            Assert.AreNotEqual(Global.MapApiFunctions.DeleteSolarSystem(name, "Jita", pilotSecond).Count, 1000);
            Thread.Sleep(2000);

            mapSecond.ActivePilot = pilotFirst;

            mapSecond.LocationSolarSystemName = "J165920";

            mapSecond.Update();

        }

        [TestMethod]
        public void SignaturesAdditionTests()
        {
            var name = "CJQ_2000" ;

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem("Scarlett Orwell", name, "", "J213734"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem("Scarlett Orwell", name, "J213734", "J214318"), "\"Ok\"");

            SignaturesTests(name, "Scarlett Orwell");
        }

        public void SignaturesTests(string mapKey, string pilot)
        {
            var name = mapKey;

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

            var _lastUpdate = DateTime.UtcNow.Ticks;

            var signatures = new List<CosmicSignature>
            {
                new CosmicSignature { SolarSystemName = "J213734", Code = "KFG-768", Name = "To Hek", Type = SignatureType.WH },
                new CosmicSignature { SolarSystemName = "J213734", Code = "KFG-769", Name = "", Type = SignatureType.Relic },
                new CosmicSignature { SolarSystemName = "J213734", Code = "ABC-231", Name = "", Type = SignatureType.Gas }
            };

            Global.MapApiFunctions.PublishSignatures(pilot, name, "J213734", signatures);

            var updates = Global.MapApiFunctions.GetUpdates(name, pilot, _lastUpdate);

            Assert.AreEqual(updates[0].Signatures.Count, 3);

            signatures = new List<CosmicSignature>
            {
                new CosmicSignature { SolarSystemName = "J213734", Code = "UHT-116", Name = "", Type = SignatureType.Unknown },
                new CosmicSignature { SolarSystemName = "J213734", Code = "ABC-231", Name = "Bountiful Frontier Reservoir", Type = SignatureType.Gas }
            };

            Global.MapApiFunctions.PublishSignatures(pilot, name, "J213734", signatures);

            updates = Global.MapApiFunctions.GetUpdates(name, pilot, _lastUpdate);

            Assert.AreEqual(updates[0].Signatures.Count, 2);

            _lastUpdate = DateTime.UtcNow.Ticks;

            signatures = new List<CosmicSignature>
            {
                new CosmicSignature { SolarSystemName = "J213734", Code = "KFG-769", Name = "", Type = SignatureType.Relic },
                new CosmicSignature { SolarSystemName = "J213734", Code = "UHT-116", Name = "", Type = SignatureType.Unknown },
                new CosmicSignature { SolarSystemName = "J213734", Code = "ABC-231", Name = "", Type = SignatureType.Unknown }
            };

            Global.MapApiFunctions.PublishSignatures(pilot, name, "J213734", signatures);

            updates = Global.MapApiFunctions.GetUpdates(name, pilot, _lastUpdate);

            Assert.AreEqual(updates[0].Signatures.Count, 3);

            _lastUpdate = DateTime.UtcNow.Ticks;


            Global.MapApiFunctions.DeleteSignature(pilot, name, "J213734", "UHT-116");

            updates = Global.MapApiFunctions.GetUpdates(name, pilot, _lastUpdate);

            Assert.AreEqual(updates[0].Signatures.Count, 2);

            signatures = new List<CosmicSignature>
            {
                new CosmicSignature { SolarSystemName = "J214318", Code = "KFG-169", Name = "", Type = SignatureType.Relic },
                new CosmicSignature { SolarSystemName = "J214318", Code = "UHT-170", Name = "", Type = SignatureType.Unknown },
                new CosmicSignature { SolarSystemName = "J214318", Code = "ABC-171", Name = "", Type = SignatureType.Unknown }
            };

            Global.MapApiFunctions.PublishSignatures(pilot, name, "J214318", signatures);

            var updates2 = Global.MapApiFunctions.GetUpdates(name, pilot, _lastUpdate);

            Assert.AreEqual(updates2[1].Signatures.Count, 2);
        }

        [TestMethod]
        public void CheckSystemTypeTests()
        {
            var name = DateTime.UtcNow.Ticks.ToString();

            name = "CJQ1_" + name;

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

            const string pilot = "Scarlett Orwell";

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "", "J213734"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "J213734", "Hek"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "Hek", "Jita"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "Jita", "Uedama"), "\"Ok\"");

            var map = new Map { ActivePilot = pilot, Key = name };

            map.Update();
        }

        [TestMethod]
        public void PilotRelocationTests()
        {
            var name = DateTime.UtcNow.Ticks.ToString();

            name = "PilotRelocation_" + name;

            const string pilot = "Scarlett Orwell";

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

            var delta = DateTime.UtcNow.Ticks;

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "", "J200000"), "\"Ok\"");

            var pilotesBefore = Global.MapApiFunctions.GetPilotes(name, delta, pilot);

            Assert.AreEqual(pilotesBefore[0].System, "J200000");

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "", "J300000"), "\"Ok\"");

            var pilotesAfter = Global.MapApiFunctions.GetPilotes(name, delta, pilot);

            Assert.AreEqual(pilotesAfter[0].System, "J300000");
        }

        [TestMethod]
        public void CreateMap()
        {
            var name = DateTime.UtcNow.Ticks.ToString();

            name = "CJQ_" + name;

            var pilot = "Scarlett Orwell";

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "", "J213734"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "J213734", "J165920"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "J165920", "J165936"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "J165920", "J165943"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "J165943", "J165953"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "J165943", "J165954"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "J165943", "J165955"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "J165943", "J165956"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "J165943", "J165957"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "J165943", "J165958"), "\"Ok\"");

            var map = new Map { ActivePilot = pilot, Key = name };

            map.Update();

            Assert.AreEqual(map.Systems.Count, 10);

            Assert.AreEqual(Global.MapApiFunctions.DeleteSolarSystem(name, "J165953", pilot).Count, 1);

            map.Update();

            Assert.AreEqual(map.Systems.Count, 9);

            var system = map.GetSystem("J165943");

            Assert.AreEqual(system.Connections.Count, 6);

            var delta = DateTime.UtcNow.Ticks;

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilot, name, "J165943", "J165014"), "\"Ok\"");

            var systems = Global.MapApiFunctions.GetUpdates(name, pilot, delta);

            Assert.AreEqual(systems.Count, 2);

            delta = DateTime.UtcNow.Ticks;

            systems = Global.MapApiFunctions.GetUpdates(name, pilot, delta);

            Assert.AreEqual(systems.Count, 0);

        }


    }
}
