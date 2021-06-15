using Microsoft.AspNetCore.Mvc;

namespace Rutschig.Controllers
{
    public class AppController : Controller
    {
        public IActionResult Robots()
        {
            return Content(
                "User-agent: *\n" +
                "Allow: /$\n" +
                "Disallow: /\n"
            );
        }

        public IActionResult SiteMap()
        {
            return Content(
                "<url>\n" +
                "  <loc>" +
                Request.Host +
                "/</loc>\n" +
                "</url>\n"
            );
        }
    }
}