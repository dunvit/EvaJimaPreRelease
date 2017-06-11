using EveJimaCore.Domain.MapToolbar.Contracts;

namespace EveJimaCore.Domain.MapToolbar.Modes
{
    public class MapInformationModel : IMapInformationModel
    {
        public BLL.Map.Map SpaceMap { get; set; }

        public void ChangeMapKey(string newMapKey)
        {
            SpaceMap.Reload(newMapKey);
        }

        public void DeathNotice(string locationSolarSystem)
        {
            SpaceMap.DeathNotice(locationSolarSystem);
        }
    }
}
