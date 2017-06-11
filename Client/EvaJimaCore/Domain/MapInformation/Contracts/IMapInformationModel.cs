
namespace EveJimaCore.Domain.MapToolbar.Contracts
{
    public interface IMapInformationModel
    {
        BLL.Map.Map SpaceMap { get; set; }

        void ChangeMapKey(string newMapKey);

        void DeathNotice(string locationSolarSystem);
    }
}
