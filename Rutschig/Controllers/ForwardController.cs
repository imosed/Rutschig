#nullable enable
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Rutschig.Models;

namespace Rutschig.Controllers
{
    public class Forward : Controller
    {
        private readonly RutschigContext _context;

        public Forward(RutschigContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult? Redir(string alias)
        {
            var redir = _context.Aliases.SingleOrDefault(a => a.Forward == alias);
            
            if (redir == null) return RedirectToAction("Index", "Home");
            if ((redir.Expiration != null && NodaTime.Instant.FromDateTimeOffset(DateTimeOffset.Now) < redir.Expiration || redir.Expiration == null) && redir.Pin == null) return Redirect(redir.Url);
            if (redir.Expiration != null && NodaTime.Instant.FromDateTimeOffset(DateTimeOffset.Now) >= redir.Expiration) return RedirectToAction("Index", "Home");
            if (redir.Pin != null) return RedirectToAction("PinEntry", new {id = redir.Id});

            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        public IActionResult PinEntry(int id)
        {
            return View(id);
        }

        [HttpPost]
        public void CheckPin(ForwardPost forwardData)
        {
            Request.ContentType = "multipart/form-data";
            var pass = _context.Aliases.Find(forwardData.Id);
            if (pass != null)
                Response.Redirect(pass.Url);
        }
    }
}