using Microsoft.AspNetCore.Mvc;

namespace Rutschig.Models
{
    public class ForwardPost
    {
        [FromForm(Name = "id")]
        public int Id { get; set; }
        [FromForm(Name = "pin")]
        public string Pin { get; set; }
    }
}