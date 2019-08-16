using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.Models
{
    public class BankSlipInstallment
    {
        public int BankslipId { get; set; }
        public BankSlip BankSlip { get; set; }
        public int InstallmentId { get; set; }
        public DelayedInstallment DelayedInstallment { get; set; }
    }
}
