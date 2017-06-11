
using System.Drawing;

namespace EveJimaCore.Domain.Map.Contracts
{
    public interface IMapModel
    {
        BLL.Map.Map SpaceMap { get; set; }

        string Activate();

        void SelectSolarSystem(string solarSystemName);

        void RelocateSolarSystem(Point solarSystemNewLocationInMap);
    }
}
