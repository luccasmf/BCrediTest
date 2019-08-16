using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.Models
{
    public class ContractDetailViewModel
    {
        public Contract Contract { get; set; }
        public List<DelayedInstallment> DelayedInstallments { get; set; }
        public List<BankSlip> BankSlips { get; set; }
    }
}
