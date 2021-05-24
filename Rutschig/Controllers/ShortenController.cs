using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
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
            bool validPin(string pin)
            {
                if (string.IsNullOrEmpty(pin)) return false;
                var pinSuccess = int.TryParse(pin.Trim(), out _);
                return pinSuccess;
            }

            bool validExpiration(string expiration)
            {
                if (string.IsNullOrEmpty(expiration)) return false;
                var expirationSuccess = DateTime.TryParse(expiration.Trim(), out _);
                return expirationSuccess;
            }

            var processedPin = validPin(aliasData.Pin) ? aliasData.Pin : null;

            if (_context.Aliases.AsEnumerable().Any(a => a.Url == aliasData.Url && a.Pin == processedPin &&
                                                         (a.Expiration == null ||
                                                          Instant.FromDateTimeOffset(DateTimeOffset.Now) <
                                                          a.Expiration)))
                return new AliasResponse
                {
                    Shortened = _context.Aliases.AsEnumerable().First(a =>
                            a.Url == aliasData.Url
                            && a.Pin == processedPin
                            && (a.Expiration == null || Instant.FromDateTimeOffset(DateTimeOffset.Now) < a.Expiration))
                        .Forward
                };

            var shortened = new Alias
            {
                Forward = ShortenUrl(aliasData.Url),
                Url = aliasData.Url,
                Pin = processedPin,
                Expiration = validExpiration(aliasData.Expiration)
                    ? Instant.FromDateTimeOffset(DateTimeOffset.Parse(aliasData.Expiration))
                    : null
            };
            _context.Aliases.Add(shortened);
            _context.SaveChanges();
            return new AliasResponse {Shortened = shortened.Forward};
        }

        private static string ShortenUrl(string url)
        {
            var rand = new Random();
            return BitConverter.ToString(MakeBytesFromString(url))
                       .Replace("-", string.Empty)
                       .ToLowerInvariant()
                   + (char) (DateTime.Now.Millisecond % 26 + 97)
                   + (char) (rand.Next() % 26 + 97);
        }

        private static byte[] MakeBytesFromString(string url, short length = 4)
        {
            var bytes = new byte[length];
            for (var i = 0; i < url.Length; i++) bytes[i % length] += (byte) url[i];

            return bytes;
        }
    }
}