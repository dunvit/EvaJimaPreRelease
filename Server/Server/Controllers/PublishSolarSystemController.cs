using System.Web.Http;

namespace Server.Controllers
{
    public class PublishSolarSystemController : ApiController
    {

        public string Get(string pilot, string mapKey, string systemFrom, string systemTo)
        {
            return GlobalData.MapRouter.PublishSolarSystem(pilot, mapKey, systemFrom, systemTo);

        }
    }
}
