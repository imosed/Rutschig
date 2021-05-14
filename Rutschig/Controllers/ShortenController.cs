using System;
using Microsoft.AspNetCore.Mvc;
using Rutschig.Models;

namespace Rutschig.Controllers
{
    public class ShortenController : Controller
    {
        [HttpPost]
        public void Create([FromBody] Alias aliasData)
        {
            Console.WriteLine("HEY");
        }
    }
}