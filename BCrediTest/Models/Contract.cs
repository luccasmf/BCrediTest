using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.Models
{
    public class Contract
    {
        public string ExternalId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerCpf { get; set; }
        public decimal LoanValue { get; set; }
        public int  PaymentTerm { get; set; }
        public string RealtyAddress { get; set; }

        public List<DelayedInstallments> Installments { get; set; }
    }
}
