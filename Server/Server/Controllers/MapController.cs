using System.Web.Http;

namespace Server.Controllers
{
    public class MapController : ApiController
    {
        public string Get(string key, string system, string pilot)
        {
            return GlobalData.MapRouter.LoadMap(key, system, pilot);
        }

        public string Get(string key)
        {
            return GlobalData.MapRouter.GetMapOwner(key);
        }

    }
}
