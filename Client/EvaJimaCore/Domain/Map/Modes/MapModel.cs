using System;
using System.Drawing;
using EvaJimaCore;
using EveJimaCore.Domain.Map.Contracts;
using log4net;

namespace EveJimaCore.Domain.Map.Modes
{
    public class MapModel : IMapModel
    {
        readonly ILog _commandsLog = LogManager.GetLogger("Errors");

        public BLL.Map.Map SpaceMap { get; set; }

        public string Activate()
        {
            return SpaceMap.ActivePilot + " " + SpaceMap.Key;
        }

        public void SelectSolarSystem(string solarSystemName)
        {
        }

        public void RelocateSolarSystem(Point solarSystemNewLocationInMap)
        {
            try
            {
                Global.MapApiFunctions.UpdateSolarSystemCoordinates(SpaceMap.Key, SpaceMap.SelectedSolarSystemName, SpaceMap.ActivePilot, solarSystemNewLocationInMap.X, solarSystemNewLocationInMap.Y);
            }
            catch(Exception ex)
            {
                _commandsLog.ErrorFormat("[MapModel.RelocateSolarSystem] Point = {1} SolarSystemName = {2} Critical error {0}", ex, solarSystemNewLocationInMap, SpaceMap.SelectedSolarSystemName);
            }
        }
    }
}
