using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.Models
{
    public class DelayedInstallment
    {
        [Key]
        public int Id { get; set; }
        public string ContractId { get; set; }
        public string InstallmentIndex { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Value { get; set; }
        public bool Delayed { get; set; }
        [NotMapped]
        public int DaysInDelay
        {
            get
            {
                return DateTime.Now > DueDate ? (DateTime.Now - DueDate).Days : 0;

            }
        }

        [NotMapped]
        public bool IsSelected { get; set; }
        public virtual Contract Contracts { get; set; }
        public virtual ICollection<BankSlip> BankSlips { get; set; }

    }
}
