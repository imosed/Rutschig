﻿using Microsoft.AspNetCore.Mvc;

namespace Rutschig.Controllers
{
    public class AppController : Controller
    {
        private readonly AppConfig _appConfig;
        
        public AppController(AppConfig appConfig)
        {
            _appConfig = appConfig;
        }
        
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