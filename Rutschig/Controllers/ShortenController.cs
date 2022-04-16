using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NodaTime;
using Rutschig.Config;
using Rutschig.Data;
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

            var processedUrl = aliasData.Url.Trim();

            var processedPin = AllNumbers(aliasData.Pin) ? aliasData.Pin?.Trim() : null;
            if (processedPin?.Length > _appConfig.GetValue<int>(nameof(Config.Config.MaxPinLength)))
                processedPin = processedPin[.._appConfig.GetValue<int>(nameof(Config.Config.MaxPinLength))];

            string SummedPin()
            {
                return processedPin?.Select(c => (int) c).Sum().ToString() ?? string.Empty;
            }

            bool SubmissionExists(Alias alias)
            {
                return alias.Url == processedUrl
                       && alias.Pin?.Trim() == processedPin
                       && (alias.Expiration == null
                           || Instant.FromDateTimeOffset(DateTimeOffset.Now) < alias.Expiration)
                       && (alias.MaxHits == null
                           || alias.Hits < aliasData.MaxHits);
            }

            if (!processedUrl.StartsWith("http")) return new AliasResponse();

            try
            {
                if (_context.AliasList.AsEnumerable().Any(SubmissionExists))
                    return new AliasResponse
                    {
                        Shortened = _context.AliasList.AsEnumerable().First(SubmissionExists).Forward
                    };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            var shortened = new Alias
            {
                Forward = ShortenUrl(processedUrl + SummedPin()),
                Url = processedUrl,
                Pin = processedPin,
                Expiration = ValidExpiration(aliasData.Expiration)
                    ? Instant.FromDateTimeOffset(DateTimeOffset.Parse(aliasData.Expiration!))
                    : null,
                MaxHits = aliasData.MaxHits
            };

            try
            {
                _context.AliasList.Add(shortened);
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
            var now = DateTime.Now;
            var length = _appConfig.GetValue<byte>(nameof(Config.Config.ShortenedLength));

            var shortened =
                CreateStringSignature(url, (byte)(length - 2)).Select(GetCharConstrained)
                    .Append(GetCharConstrained(now.Millisecond)).Append(GetCharConstrained(now.Hour));
            return string.Join(string.Empty, shortened);
        }

        private static IEnumerable<int> CreateStringSignature(string url, byte length)
        {
            var signature = new int[length];
            for (var i = 0; i < url.Length; i++) signature[i % length] += url[i];

            return signature;
        }

        private static char GetCharConstrained(int input)
        {
            var pool = new[] {
                Enumerable.Range(49, 8).ToArray(), // 1-9 (intentionally exclude 0, as it could be mistaken for 'o')
                Enumerable.Range(65, 26).ToArray(), // A-Z
                Enumerable.Range(97, 26).ToArray() // a-z
            };

            var range = pool[input % 3];
            return (char)range[input % range.Length];
        }
    }
}
