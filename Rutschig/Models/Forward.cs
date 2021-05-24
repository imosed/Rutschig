using Microsoft.AspNetCore.Mvc;

namespace Rutschig.Models
{
    public class ForwardPost
    {
        [FromForm(Name = "forward")] public string Forward { get; set; }

        [FromForm(Name = "pin")] public string Pin { get; set; }
    }
}