using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.Models
{
    public class DelayedInstallments
    {
        public string ContractId { get; set; }
        public string InstallmentIndex { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Value { get; set; }
    }
}
