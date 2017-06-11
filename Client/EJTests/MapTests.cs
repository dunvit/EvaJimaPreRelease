using System;
using System.Collections.Generic;
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

        [TestMethod]
        public void DeathNotice_Test()
        {
            var name = "threads_" + DateTime.UtcNow.Ticks;

            const string pilotFirst = "Scarlett Orwell";

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);
            Global.ApplicationSettings = new ApplicationSettings();

            var map = new Map { ActivePilot = pilotFirst, Key = name };

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotFirst, name, "", "J213734"), "\"Ok\"");
            map.Update();
            Assert.AreEqual(map.Systems[0].LocationInMap.X, 5000);
            Assert.AreEqual(map.Systems[0].LocationInMap.Y, 5000);
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotFirst, name, "J213734", "J165920"), "\"Ok\"");
            map.Update();
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotFirst, name, "J165920", "Jita"), "\"Ok\"");
            map.Update();
            Assert.AreEqual(Global.MapApiFunctions.DeleteConnectionBetweenSolarSystems(pilotFirst, name, "J165920", "Jita"), "\"Ok\"");
            map.Update();

            Assert.AreEqual(map.Systems[2].Connections.Count, 0);

        }

        [TestMethod]
        public void LoadMapTest()
        {
            var name = "threads_636324973316144820";
            const string pilotFirst = "Scarlett Orwell";

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

            var map = new Map {ActivePilot = pilotFirst, Key = name};

            var retValue = Global.MapApiFunctions.PublishSolarSystem(pilotFirst, name, "", "J213734");

            Assert.AreEqual(retValue, "\"Ok\"");

            map.Update();
        }

        [TestMethod]
        public void GetMapOwnerTest()
        {
            var name = "threads_" + DateTime.UtcNow.Ticks;

            const string pilotFirst = "Scarlett Orwell";
            const string pilotSecond = "Dana Su-Shiloff";

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotFirst, name, "", "J213734"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotFirst, name, "J213734", "J165920"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotSecond, name, "", "J213734"), "\"Ok\"");

            var mapOwner = Global.MapApiFunctions.GetMapOwner(name);

            Assert.AreEqual(pilotFirst, mapOwner);
        }

        [TestMethod]
        public void FullFlowMapTest()
        {
            var name = "threads_" + DateTime.UtcNow.Ticks;

            var someString = $"Some data: {name}, some more data: {name}";

            Global.MapApiFunctions = new MapApiFunctions();
            Global.MapApiFunctions.Initialization(Server_MapAddress);

            const string pilotFirst = "Scarlett Orwell";
            const string pilotSecond = "Dana Su-Shiloff";


            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotFirst, name, "", "J213734"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotFirst, name, "J213734", "J165920"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotFirst, name, "J165920", "J165936"), "\"Ok\"");

            var mapFirst = new Map { ActivePilot = pilotFirst, Key = name };

            mapFirst.Update();
            //var mapFirst = mapApiFunctions.LoadMap(name, "J213734", pilotFirst);

            Assert.AreEqual(mapFirst.Systems.Count, 3);

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotSecond, name, "", "J122635"), "\"Ok\"");
            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotSecond, name, "J122635", "J168936"), "\"Ok\"");

            var mapSecond = new Map { ActivePilot = pilotSecond, Key = name };

            mapSecond.Update();

            Assert.AreEqual(mapSecond.Systems.Count, 2);

            Assert.AreEqual(mapFirst.Systems.Count, 3);

            var updatesFirst = Global.MapApiFunctions.GetUpdates(name, pilotFirst, 0);

            Assert.AreEqual(updatesFirst.Count, 3);

            var updatesSecond = Global.MapApiFunctions.GetUpdates(name, pilotSecond, 0);

            Assert.AreEqual(updatesSecond.Count, 2);

            var pilots = Global.MapApiFunctions.GetPilotes(name,0, pilotSecond);

            Assert.AreEqual(pilots.Count, 2);

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotSecond, name, "J168936", "J165920"), "\"Ok\"");

            mapSecond.Update();

            Assert.AreEqual(mapSecond.Systems.Count, 3);

            var systemJ168936 = mapSecond.GetSystem("J168936");

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotFirst, name, "J213734", "J214318"), "\"Ok\"");

            SignaturesTests(name, pilotFirst);

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotSecond, name, "J165920", "Jita"), "\"Ok\"");

            Assert.AreEqual(Global.MapApiFunctions.PublishSolarSystem(pilotSecond, name, "Jita", "Hek"), "\"Ok\"");

            Assert.AreEqual(Global.MapApiFunctions.DeleteSolarSystem(name, "Jita", pilotSecond), "\"Ok\"");

            mapSecond.ActivePilot = pilotFirst;

            mapSecond.LocationSolarSystemName = "J165920";

            mapSecond.Update();

            var systemHek = mapSecond.GetSystem("Hek");

            Assert.AreEqual(systemHek.IsHidden, true);

            Assert.AreEqual(mapSecond.GetSystem("J165920").IsHidden, false);

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

            Assert.AreEqual(Global.MapApiFunctions.DeleteSolarSystem(name, "J165953", pilot), "\"Ok\"");

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
