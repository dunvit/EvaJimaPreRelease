using System.Web.Http;

namespace Server.Controllers
{
    public class GetUpdatesPilotesController : ApiController
    {

        public string Get(string mapKey, long ticks, string pilot)
        {
            return GlobalData.MapRouter.GetUpdatedPilotes(mapKey, ticks, pilot);
        }

    }
}
