using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rutschig.Models;

namespace Rutschig.Controllers
{
    public class ShortenController : Controller
    {
        private readonly RutschigContext _context;

        public ShortenController(RutschigContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        public AliasResponse Create([FromBody] AliasPost aliasData)
        {
            if (_context.Aliases.Any(a => a.Url == aliasData.Url && a.Pin == aliasData.Pin &&
                                          (a.Expiration == null || NodaTime.Instant.FromDateTimeOffset(DateTimeOffset.Now) < a.Expiration)))
                    return new AliasResponse
                        { Shortened = _context.Aliases.Single(a => a.Url == aliasData.Url && a.Pin == aliasData.Pin).Forward };

            var shortened = new Alias
            {
                Forward = ShortenUrl(aliasData.Url),
                Url = aliasData.Url,
                Pin = aliasData.Pin,
                Expiration = aliasData.Expiration != null ? NodaTime.Instant.FromDateTimeOffset(DateTimeOffset.Parse(aliasData.Expiration)) : null
            };
            _context.Aliases.Add(shortened);
            _context.SaveChanges();
            return new AliasResponse { Shortened = shortened.Forward};
        }

        private static string ShortenUrl(string url)
        {
            var rand = new Random();
            return BitConverter.ToString(MakeBytesFromString(url))
                .Replace("-", string.Empty)
                .ToLowerInvariant()
                + (char)(DateTime.Now.Millisecond % 26 + 97)
                + (char)(rand.Next() % 26 + 97);
        }

        private static byte[] MakeBytesFromString(string url, short length = 4)
        {
            var bytes = new byte[length];
            for (var i = 0; i < url.Length; i++)
            {
                bytes[i % length] += (byte) url[i];
            }

            return bytes;
        }
    }
}