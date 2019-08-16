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
                entity.HasKey(p => p.Id);
                entity.HasMany(p => p.BankSlips);

            });
            modelBuilder.Entity<BankSlip>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasMany(p => p.Installments);
                
            });

        }
    }
}
