using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.ServiceClients;
using Microsoft.AspNetCore.Authorization;

namespace WebAdvert.Web.Controllers
{
    [Route("api")]
    [ApiController]
    [Produces("application/json")]
    public class InternalApis : Controller
    {
        private readonly IAdvertApiClient _advertApiClient;

        public InternalApis(IAdvertApiClient advertApiClient)
        {
            _advertApiClient = advertApiClient;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAsync(string id)
        {
            var record = await _advertApiClient.GetAsync(id).ConfigureAwait(false);

            return Json(record);
        }
    }
}