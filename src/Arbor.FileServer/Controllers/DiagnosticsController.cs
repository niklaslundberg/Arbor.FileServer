using Arbor.FileServer.Hashing;
using Microsoft.AspNetCore.Mvc;

namespace Arbor.FileServer.Controllers
{
    public class DiagnosticsController : Controller
    {
        [HttpGet]
        [Route("~/diagnostics")]
        public IActionResult Index([FromServices] FileServerSettings serverSettings)
        {
            if (!serverSettings.SettingsDiagnosticsEnabled)
            {
                return Redirect("/");
            }

            return new ObjectResult(serverSettings);
        }
    }
}
