#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace Rutschig.Models
{
    public class AliasResponse
    {
        public string Shortened { get; init; }
    }

    public class AliasPost
    {
        public string Url { get; set; }
        public string? Pin { get; set; }
        public string? Expiration { get; set; }
        public uint? MaxHits { get; set; }
    }

    public class Alias
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        public string Forward { get; init; }
        public string Url { get; init; }
        public string? Pin { get; init; }
        public Instant? Expiration { get; init; }
        public uint Hits { get; set; }
        public uint? MaxHits { get; init; }
    }
}
