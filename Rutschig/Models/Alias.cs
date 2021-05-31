#nullable enable
using NodaTime;

namespace Rutschig.Models
{
    public class AliasResponse
    {
        public string Shortened { get; set; }
    }

    public class AliasPost
    {
        public string Url { get; set; }
        public string? Pin { get; set; }
        public string? Expiration { get; set; }
    }

    public class Alias
    {
        public int Id { get; set; }
        public string Forward { get; set; }
        public string Url { get; set; }
        public string? Pin { get; set; }
        public Instant? Expiration { get; set; }
    }
}