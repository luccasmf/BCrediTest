using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentAPIController : ControllerBase
    {
        [HttpPost("Test")]
        public IActionResult Test()
        {
            return Ok();
        }
    }
}
