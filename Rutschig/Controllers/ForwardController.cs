#nullable enable
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Rutschig.Data;
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
            static bool EndpointAccessible(Alias? alias)
            {
                return (alias?.Expiration != null
                        && Instant.FromDateTimeOffset(DateTimeOffset.Now) < alias.Expiration
                        || alias?.Expiration == null)
                       && alias?.Pin == null
                       && (alias?.Hits < alias?.MaxHits
                           || alias?.MaxHits == null);
            }

            static bool EndpointInaccessible(Alias? alias)
            {
                return alias?.Expiration != null
                       && Instant.FromDateTimeOffset(DateTimeOffset.Now) >= alias.Expiration
                       || alias?.MaxHits != null
                       && alias.Hits >= alias.MaxHits;
            }

            var redir = _context.Aliases.SingleOrDefault(a => a.Forward == alias);

            if (redir == null) return RedirectToAction("Index", "Home");
            if (EndpointAccessible(redir))
            {
                redir.Hits++;
                _context.SaveChanges();
                return Redirect(redir.Url);
            }

            if (EndpointInaccessible(redir))
                return RedirectToAction("Index", "Home");
            return redir.Pin != null
                ? RedirectToAction("PinEntry", new {shortened = redir.Forward})
                : RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult PinEntry(string shortened)
        {
            return View((object) shortened);
        }

        [HttpPost]
        public IActionResult CheckPin(ForwardPost forwardData)
        {
            Request.ContentType = "multipart/form-data";
            var pass = _context.Aliases.AsEnumerable()
                .SingleOrDefault(
                    a => a.Forward == forwardData.Forward && a.Pin == forwardData.Pin && a.Hits < a.MaxHits);
            if (pass == null) return RedirectToAction("Index", "Home");
            pass.Hits++;
            _context.SaveChanges();
            return Redirect(pass.Url);
        }
    }
}