using NUnit.Framework;
using BCrediTest.Models;
using BCrediTest.BusinessLayer;
using Microsoft.EntityFrameworkCore;
using BCrediTest.Data;
using BCrediTest.Repositories;
using System.Collections.Generic;

namespace Tests
{
    public class Tests
    {
        DbContextOptions<BCrediDbContext> options;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<BCrediDbContext>()
                  .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                  .Options;
        }

        [Test]
        public void AddingContractssToDatabase()
        {
            bool addedRegisters = false;
            List<Contract> contracts = new List<Contract>();

            contracts.Add(new Contract
            {
                CustomerCpf = "088.985.269-38",
                CustomerEmail = "luccas_mf@live.com",
                CustomerName = "Luccas",
                ExternalId = "123",
                LoanValue = (decimal)1000.50,
                PaymentTerm = 120,
                RealtyAddress = "Endereço"
            });

            contracts.Add(new Contract
            {
                CustomerCpf = "088.985.269-38",
                CustomerEmail = "luccas_mf@live.com",
                CustomerName = "Luccas2",
                ExternalId = "234",
                LoanValue = (decimal)1000.50,
                PaymentTerm = 120,
                RealtyAddress = "Endereço",
            });

            using (var context = new BCrediDbContext(options))
            {
                var repository = new ContractRepository(context);
                addedRegisters = repository.PersistContracts(contracts);

            }
            if (addedRegisters)
                Assert.Pass();
            else
                Assert.Fail();
        }
    }
}