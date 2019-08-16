using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.Models
{
    public class BankSlip
    {
        public int Id { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Value { get; set; }
        public BankSlipStatus Status { get; set; }
        [NotMapped]
        public int Delay
        {
            get
            {

                return DateTime.Now > DueDate? (DateTime.Now - DueDate).Days : 0;
            }
        }
        [NotMapped]
        public bool IsSelected { get; set; }
        public virtual ICollection<DelayedInstallment> Installments { get; set; }

    }

    public enum BankSlipStatus : byte
    {
        Pending = 1,
        Paid = 2
    }
}
