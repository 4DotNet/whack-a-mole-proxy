using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Wam.Proxy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok("Hello from the Whack-A-Mole Proxy API");
        }
    }
}
