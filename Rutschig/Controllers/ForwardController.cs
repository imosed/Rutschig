using Microsoft.AspNetCore.Mvc;

namespace Rutschig.Controllers
{
    public class Forward : Controller
    {
        [HttpGet]
        public void Redirect()
        {
            
        }
        
        [HttpGet]
        public IActionResult PinEntry()
        {
            return View();
        }

        [HttpPost]
        public void CheckPin()
        {
            Response.Redirect("/");
        }
    }
}