﻿using BCrediTest.Extensions;
using BCrediTest.Models;
using BCrediTest.Repositories;
using BCrediTest.Viewmodels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.BusinessLayer
{
    public class BLContract
    {
        private readonly IContractRepository _contractRepository;
        public BLContract(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public bool ImportFile(IFormFile file, int fileType)
        {
            
            bool success = false;
            try
            {
                List<string> entries = file.ReadAsList();
                entries.RemoveAt(0);
                switch (fileType)
                {
                    case 1:
                        List<Contract> contracts = new List<Contract>();

                        foreach (string csvLine in entries)
                        {
                            string[] values = csvLine.Split(',');

                            contracts.Add(new Contract
                            {
                                ExternalId = values[0],
                                CustomerName = values[1],
                                CustomerEmail = values[2],
                                CustomerCpf = values[3],
                                LoanValue = decimal.Parse(values[4], NumberStyles.Any, CultureInfo.InvariantCulture),
                                PaymentTerm = int.Parse(values[5]),
                                RealtyAddress = values[6].Replace("\"", string.Empty)
                            });
                        }

                        success = _contractRepository.PersistContracts(contracts);
                        break;
                    case 2:
                        List<DelayedInstallment> installments = new List<DelayedInstallment>();

                        foreach (string csvLine in entries)
                        {
                            string[] values = csvLine.Split(',');

                            installments.Add(new DelayedInstallment
                            {
                                ContractId = values[0],
                                InstallmentIndex = values[1],
                                DueDate = DateTime.Parse(values[2]),
                                Value = decimal.Parse(values[3], NumberStyles.Any, CultureInfo.InvariantCulture),
                                Delayed = true
                            });
                        }

                        success = _contractRepository.PersistInstallments(installments);
                        break;
                }
            }
            catch
            {
                success = false;
            }



            return success;
            }

        public void CreateBankSlip(BankSlipScheduleViewModel bankslipSchedule, string contractId)
        {
            BankSlip bankSlip = new BankSlip();
            decimal baseValue = bankslipSchedule.Installments.Sum(x => x.Value);

            bankSlip.DueDate = bankslipSchedule.DueDate;
            bankSlip.Value += baseValue * (1 + (bankslipSchedule.FeeValue / 100));
            bankSlip.BankSlipInstallment = new List<BankSlipInstallment>();
                
            bankslipSchedule.Installments.ForEach(x =>
            {

                bankSlip.Value += x.Value * (decimal)Math.Pow((double)(bankslipSchedule.InterestValue / 100), x.DaysInDelay);
                bankSlip.BankSlipInstallment.Add(new BankSlipInstallment { InstallmentId = x.InstallmentId });

            });

            bankSlip.Status = BankSlipStatus.Pending;

            bool persisted = _contractRepository.PersistBankSlip(bankSlip);

            if(persisted)
            {
                //send email
            }
        }

        public BankSlipScheduleViewModel GetDetailsToSchedule(List<int> installmentsIds)
        {

            BankSlipScheduleViewModel scheduleViewModel = new BankSlipScheduleViewModel();

            scheduleViewModel.Installments = _contractRepository.GetDelayedInstallments(installmentsIds);
            scheduleViewModel.DueDate = DateTime.Now;
            scheduleViewModel.FeeValue = 5;
            scheduleViewModel.InterestValue = 1;

            return scheduleViewModel;
        }

        public void DeleteContract(string id)
        {
            bool success = _contractRepository.DeleteContract(id);
        }

        public object ImportDelayedInstallments(IFormFile file)
        {
            return true;
        }

        public List<Contract> ListContracts()
        {

            return _contractRepository.GetAllContracts();
        }

        public ContractDetailViewModel GetContractDetail(string contractId)
        {
            ContractDetailViewModel dt = _contractRepository.GetContractDetail(contractId);
            //List<DelayedInstallment> installments = new List<DelayedInstallment>();

            //installments.Add(new DelayedInstallment
            //{
            //    ContractId = "123",
            //    DueDate = DateTime.Parse("01/07/2019"),
            //    InstallmentId = 1,
            //    InstallmentIndex = "084/120",
            //    Value = (decimal)1450.00,
            //    Delayed = true
            //});
            //installments.Add(new DelayedInstallment
            //{
            //    ContractId = "123",
            //    DueDate = DateTime.Parse("01/08/2019"),
            //    InstallmentId = 1,
            //    InstallmentIndex = "085/120",
            //    Value = (decimal)1200.00,
            //    Delayed = true
            //});

            //Contract c = new Contract
            //{
            //    CustomerCpf = "088.985.269-38",
            //    CustomerEmail = "luccas_mf@live.com",
            //    CustomerName = "Luccas",
            //    ExternalId = "123",
            //    LoanValue = (decimal)1000.50,
            //    PaymentTerm = 120,
            //    RealtyAddress = "Endereço",
            //};

            //List<BankSlip> bankSlips = new List<BankSlip>();
            //bankSlips.Add(new BankSlip
            //{
            //    DueDate = DateTime.Parse("11/08/2019"),
            //    BankslipId = 1,
            //    IsSelected = false,
            //    Status = BankSlipStatus.Pending,
            //    Value = (decimal)1234.00
            //});
            //bankSlips.Add(new BankSlip
            //{
            //    DueDate = DateTime.Parse("13/08/2019"),
            //    BankslipId = 2,
            //    IsSelected = false,
            //    Status = BankSlipStatus.Pending,
            //    Value = (decimal)1245.00
            //});

            //ContractDetailViewModel details = new ContractDetailViewModel();

            //details.Contract = c;
            //details.DelayedInstallments = installments;
            //details.BankSlips = bankSlips;

            return dt;
        }
    }
}
