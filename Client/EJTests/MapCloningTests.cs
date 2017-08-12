using System;
using System.Collections.Generic;
using System.Drawing;
using EveJimaCore.BLL.Map;
using EveJimaUniverse;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EJTests
{
    [TestClass]
    public class MapCloningTests
    {
        private Map GenerateMapObject()
        {
            var map = new Map();

            map.Systems.Add(new SolarSystem { Name = "J213734", Connections = new List<string> { "J213735", "J213736" }, LocationInMap = new Point(5000, 5000) });
            map.Systems.Add(new SolarSystem { Name = "J213735", Connections = new List<string> { "J213734" }, LocationInMap = new Point(5200, 5300) });
            map.Systems.Add(new SolarSystem { Name = "J213736", Connections = new List<string> { "J213734" }, LocationInMap = new Point(4800, 4990) });

            map.LocationSolarSystemName = "J213734";

            return map;
        }

        [TestMethod]
        public void MapSelfCloningTest()
        {
            var map = GenerateMapObject();

            var mapForView = map.Clone();

            Assert.IsTrue(map.Systems.Count == mapForView.Systems.Count);

            map.Systems.Add(new SolarSystem { Name = "J213737", Connections = new List<string> { "J213736" }, LocationInMap = new Point(4700, 4790) });

            Assert.IsFalse(map.Systems.Count == mapForView.Systems.Count);
        }
    }
}
