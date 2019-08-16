using BCrediTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.Data
{
    public class BCrediDbContext : DbContext
    {
        public BCrediDbContext(DbContextOptions options) :
            base(options)
        {
        }

        public DbSet<Contract> Contracts { get; set; }
        public DbSet<DelayedInstallment> DelayedInstallments { get; set; }
        public DbSet<BankSlip> BankSlips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contract>(entity =>
            {
                entity.HasKey(p => p.ExternalId);
                entity.HasMany(p => p.Installments)
                .WithOne(d => d.Contracts);
            });
            modelBuilder.Entity<DelayedInstallment>(entity =>
            {
                entity.HasKey(p => p.InstallmentId);

            });
            modelBuilder.Entity<BankSlip>(entity =>
            {
                entity.HasKey(p => p.BankslipId);
                
            });

            modelBuilder.Entity<BankSlipInstallment>()
                .HasKey(bi => new { bi.BankslipId, bi.InstallmentId });
            modelBuilder.Entity<BankSlipInstallment>()
                .HasOne(bi => bi.BankSlip)
                .WithMany(bi => bi.BankSlipInstallment)
                .HasForeignKey(bi => bi.BankslipId);

            modelBuilder.Entity<BankSlipInstallment>()
               .HasOne(bi => bi.DelayedInstallment)
               .WithMany(bi => bi.BankSlipInstallment)
               .HasForeignKey(bi => bi.InstallmentId);

        }
    }
}
