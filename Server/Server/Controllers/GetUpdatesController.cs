using System.Web.Http;

namespace Server.Controllers
{
    public class GetUpdatesController : ApiController
    {
        public string Get(string mapKey, string pilot, long ticks)
        {
            return GlobalData.MapRouter.GetUpdatedSystems(mapKey, pilot, ticks);
        }

        public string Get(string mapKey, string pilot, long ticks, bool deleted)
        {
            return GlobalData.MapRouter.GetDeletedSystems(mapKey, pilot, ticks, deleted);
        }
    }
}
