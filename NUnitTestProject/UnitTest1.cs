using NUnit.Framework;
using BCrediTest.Models;
using BCrediTest.BusinessLayer;
using Microsoft.EntityFrameworkCore;
using BCrediTest.Data;
using BCrediTest.Repositories;
using System.Collections.Generic;
using BCrediTest.Services.Email;
using System.Linq;

namespace Tests
{
    public class Tests
    {
        DbContextOptions<BCrediDbContext> options;
        BLContract _bLContract;
        IEmailSender _emailSender;
        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<BCrediDbContext>()
                  .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                  .Options;

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
                LoanValue = (decimal)2000.50,
                PaymentTerm = 120,
                RealtyAddress = "Endereço",
            });

            using (var context = new BCrediDbContext(options))
            {
                var repository = new ContractRepository(context);
                contractsPersisted = repository.PersistContracts(contracts);

            }

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
                using (var context = new BCrediDbContext(options))
                {
                    var repository = new ContractRepository(context);
                    installmentsPersisted = repository.PersistInstallments(installments);
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

            BankSlip bankSlip = _bLContract.CreateBankSlip(detailsToSchedule, contractDetail.Contract.ExternalId);

            if(bankSlip != null)
            {
                if (bankSlip.Value > contractDetail.DelayedInstallments.Sum(x => x.Value))
                    Assert.Pass();
                else
                    Assert.Fail();
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}