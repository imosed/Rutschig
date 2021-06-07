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
        private readonly AppConfig _appConfig;

        public ShortenController(RutschigContext context, AppConfig appConfig)
        {
            _context = context;
            _appConfig = appConfig;
        }

        [HttpPost]
        public AliasResponse Create([FromBody] AliasPost aliasData)
        {
            bool AllNumbers(string pin)
            {
                if (string.IsNullOrEmpty(pin)) return false;
                var pinSuccess = int.TryParse(pin.Trim(), out _);
                return pinSuccess;
            }

            bool ValidExpiration(string expiration)
            {
                if (string.IsNullOrEmpty(expiration)) return false;
                var expirationSuccess = DateTime.TryParse(expiration.Trim(), out _);
                return expirationSuccess;
            }

            var processedUrl = aliasData.Url.ToLowerInvariant().Trim();
            
            var processedPin = AllNumbers(aliasData.Pin) ? aliasData.Pin?.Trim() : null;
            if (processedPin?.Length > _appConfig.GetValue<int>(nameof(Config.MaxPinLength)))
                processedPin = processedPin[.._appConfig.GetValue<int>(nameof(Config.MaxPinLength))];

            bool SubmissionExists(Alias alias)
            {
                return alias.Url == processedUrl
                       && alias.Pin?.Trim() == processedPin
                       && (alias.Expiration == null
                           || Instant.FromDateTimeOffset(DateTimeOffset.Now) < alias.Expiration)
                       && alias.MaxHits == aliasData.MaxHits;
            }

            if (!processedUrl.StartsWith("http")) return new AliasResponse();

            if (_context.Aliases.AsEnumerable().Any(SubmissionExists))
                return new AliasResponse
                {
                    Shortened = _context.Aliases.AsEnumerable().First(SubmissionExists).Forward
                };

            var shortened = new Alias
            {
                Forward = ShortenUrl(processedUrl),
                Url = processedUrl,
                Pin = processedPin,
                Expiration = ValidExpiration(aliasData.Expiration)
                    ? Instant.FromDateTimeOffset(DateTimeOffset.Parse(aliasData.Expiration!))
                    : null,
                MaxHits = aliasData.MaxHits
            };
            _context.Aliases.Add(shortened);
            _context.SaveChanges();
            return new AliasResponse { Shortened = shortened.Forward };
        }

        private string ShortenUrl(string url)
        {
            var rand = new Random();
            var length = _appConfig.GetValue<byte>(nameof(Config.ShortenedLength));
            return BitConverter
                       .ToString(MakeBytesFromString(url, (byte) (length / 2 - 1)))
                       .Replace("-", string.Empty)
                       .ToLowerInvariant()
                   + (char) (DateTime.Now.Millisecond % 26 + 97)
                   + (char) (rand.Next() % 26 + 97);
        }

        private static byte[] MakeBytesFromString(string url, byte length)
        {
            var bytes = new byte[length];
            for (var i = 0; i < url.Length; i++) bytes[i % length] += (byte) url[i];

            return bytes;
        }
    }
}