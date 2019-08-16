using BCrediTest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.Viewmodels
{
    public class BankSlipScheduleViewModel
    {
        public List<DelayedInstallment> Installments { get; set; }
        public decimal FeeValue { get; set; }
        public decimal InterestValue { get; set; }
        [Required(ErrorMessage = "Campo Obrigatorio")]
        public DateTime DueDate { get; set; }
    }
}
