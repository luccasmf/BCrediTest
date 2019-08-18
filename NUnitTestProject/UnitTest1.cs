using NUnit.Framework;
using BCrediTest.Models;
using BCrediTest.BusinessLayer;
using Microsoft.EntityFrameworkCore;
using BCrediTest.Data;
using BCrediTest.Repositories;
using System.Collections.Generic;
using BCrediTest.Services.Email;
using System.Linq;
using System;

namespace Tests
{
    public class Tests
    {

        BLContract _bLContract;
        IEmailSender _emailSender;
        BCrediDbContext _context;
        [SetUp]
        public void Setup()
        {
            DbContextOptions<BCrediDbContext> options = new DbContextOptionsBuilder<BCrediDbContext>()
                  .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                  .Options;
            _context = new BCrediDbContext(options);

            IContractRepository contractRepository = new ContractRepository(new BCrediDbContext(options));
            _bLContract = new BLContract(contractRepository, _emailSender);
        }

        [Test]
        public void AddingContractsToDatabase()
        {
            bool contractsPersisted = false;
            List<Contract> contracts = new List<Contract>();

            contracts.Add(new Contract
            {
                CustomerCpf = "088.985.269-38",
                CustomerEmail = "email@email.com",
                CustomerName = "Luccas",
                ExternalId = "123",
                LoanValue = (decimal)1000.50,
                PaymentTerm = 120,
                RealtyAddress = "Endereço"
            });

            contracts.Add(new Contract
            {
                CustomerCpf = "088.985.269-38",
                CustomerEmail = "email@email.com",
                CustomerName = "Luccas2",
                ExternalId = "234",
                LoanValue = (decimal)2000.50,
                PaymentTerm = 120,
                RealtyAddress = "Endereço",
            });


            var repository = new ContractRepository(_context);
            contractsPersisted = repository.PersistContracts(contracts);



            List<DelayedInstallment> installments = new List<DelayedInstallment>();
            bool installmentsPersisted = false;
            installments.Add(new DelayedInstallment
            {
                ContractId = "123",
                DueDate = new System.DateTime(2019, 07, 10),
                Value = (decimal)500.25,
                InstallmentIndex = "01/02",
            });
            installments.Add(new DelayedInstallment
            {
                ContractId = "123",
                DueDate = new System.DateTime(2019, 08, 10),
                Value = (decimal)500.25,
                InstallmentIndex = "02/02",
            });

            installments.Add(new DelayedInstallment
            {
                ContractId = "234",
                DueDate = new System.DateTime(2019, 08, 10),
                Value = (decimal)200.10,
                InstallmentIndex = "07/10",
            });


            if (contractsPersisted)
            {
                var repo = new ContractRepository(_context);
                installmentsPersisted = repo.PersistInstallments(installments);
            }

            if (installmentsPersisted)
                Assert.Pass();
            else
                Assert.Fail();
        }

        [Test]
        public void GeneratingBankSlips()
        {
            List<int> installments = new List<int>();

            BCrediTest.Viewmodels.ContractDetailViewModel contractDetail = _bLContract.GetContractDetail("123");


            installments = contractDetail.DelayedInstallments.Select(x => x.InstallmentId).ToList();

            BCrediTest.Viewmodels.BankSlipScheduleViewModel detailsToSchedule = _bLContract.GetDetailsToSchedule(installments);

            detailsToSchedule.FeeValue = 5;
            detailsToSchedule.InterestValue = 1;

            detailsToSchedule.DueDate = DateTime.Now.AddDays(2);

            BankSlip bankSlip = _bLContract.CreateBankSlip(detailsToSchedule, "123");


            if (bankSlip != null)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}