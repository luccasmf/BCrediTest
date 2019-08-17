using BCrediTest.BusinessLayer;
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
        private readonly BLContract _blContract;

        public PaymentAPIController(BLContract blContract)
        {
            _blContract = blContract;
        }

        /// <summary>
        /// Marks a list of bank slips are paid by informing its ids
        /// </summary>
        /// <param name="bankSlipIds">An array of ids of paid bank slips.</param>
        /// <returns></returns>
        [HttpPost("InformPayment")]
        public IActionResult InformPayment([FromBody]int[] bankSlipIds)
        {
            if (_blContract.MarkAsPaid(bankSlipIds))
                return Ok(true);

            return Ok(false);
        }
    }
}
