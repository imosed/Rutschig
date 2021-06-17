using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NodaTime;
using Rutschig.Models;

namespace Rutschig.Controllers
{
    public class ShortenController : Controller
    {
        private readonly AppConfig _appConfig;
        private readonly RutschigContext _context;
        private readonly ILogger<ShortenController> _logger;

        public ShortenController(RutschigContext context, AppConfig appConfig, ILogger<ShortenController> logger)
        {
            _context = context;
            _appConfig = appConfig;
            _logger = logger;
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

            try
            {
                if (_context.Aliases.AsEnumerable().Any(SubmissionExists))
                    return new AliasResponse
                    {
                        Shortened = _context.Aliases.AsEnumerable().First(SubmissionExists).Forward
                    };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

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

            _logger.LogInformation($"{shortened.Url} -> {shortened.Forward}");

            try
            {
                _context.Aliases.Add(shortened);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return new AliasResponse {Shortened = shortened.Forward};
        }

        private string ShortenUrl(string url)
        {
            static string IntToAlpha(int b) => ((char) (b % 26 + 97)).ToString();

            var now = DateTime.Now;
            var length = _appConfig.GetValue<byte>(nameof(Config.ShortenedLength));
            var bytes = MakeBytesFromString(url, (byte) (length - 2));
            var result = bytes.Select(b => b > 64 ? IntToAlpha(b) : (b % 10).ToString())
                .Append(IntToAlpha(now.Millisecond))
                .Append(IntToAlpha(now.Hour));
            return string.Join(string.Empty, result);
        }

        private static byte[] MakeBytesFromString(string url, byte length)
        {
            var bytes = new byte[length];
            for (var i = 0; i < url.Length; i++) bytes[i % length] += (byte) url[i];

            return bytes;
        }
    }
}
