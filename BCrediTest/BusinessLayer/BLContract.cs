using BCrediTest.Extensions;
using BCrediTest.Models;
using BCrediTest.Repositories;
using BCrediTest.Services.Email;
using BCrediTest.Viewmodels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.BusinessLayer
{
    /// <summary>
    /// Class containing all the contract related businnes rules for the application
    /// </summary>
    public class BLContract
    {
        private readonly IContractRepository _contractRepository;
        private readonly IEmailSender _emailSender;
        public BLContract(IContractRepository contractRepository, IEmailSender emailSender)
        {
            _contractRepository = contractRepository;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Formats the uploaded file to be stored on database according to the informed fileType
        /// </summary>
        /// <param name="file">The uploaded file</param>
        /// <param name="fileType">Type of the uploaded file: 1 for Contracts, 2 for Delayed Installments</param>
        /// <returns></returns>
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
                                ExternalId = values[0].Trim(),
                                CustomerName = values[1].Trim(),
                                CustomerEmail = values[2].Trim(),
                                CustomerCpf = values[3].Trim(),
                                LoanValue = decimal.Parse(values[4].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture),
                                PaymentTerm = int.Parse(values[5].Trim()),
                                RealtyAddress = values[6].Replace("\"", string.Empty).Trim()
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
                                ContractId = values[0].Trim(),
                                InstallmentIndex = values[1].Trim(),
                                DueDate = DateTime.Parse(values[2].Trim()),
                                Value = decimal.Parse(values[3].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture),
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

        /// <summary>
        /// Marks the informed bank slips and the related delayed installments as paid 
        /// </summary>
        /// <param name="slipIds"></param>
        /// <returns></returns>
        public bool MarkAsPaid(int[] slipIds)
        {
            return _contractRepository.MarkSlipsAsPaid(slipIds);
        }

        /// <summary>
        /// Creates a  bank slip for the given delayed  installments and contract and send it by email
        /// </summary>
        /// <param name="bankslipSchedule">Viewmodel containing all the data for generating the bank slip</param>
        /// <param name="contractId">Id of the related contract</param>
        public BankSlip CreateBankSlip(BankSlipScheduleViewModel bankslipSchedule, string contractId)
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

            bankSlip = _contractRepository.PersistBankSlip(bankSlip);

            if(bankSlip.BankslipId != 0)
            {
                try
                {
                    SendEmailWithBankslip(bankSlip, contractId);

                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

                return bankSlip;
            }
            return null;
        }

        private async void SendEmailWithBankslip(BankSlip bankSlip, string contractId)
        {
            string message = string.Format("Id: {0}\nDue Date: {1}\nValue: {2}\nStatus: {3}", bankSlip.BankslipId, bankSlip.DueDate, string.Format("{0:C}", bankSlip.Value), bankSlip.Status);
            string subject = string.Format("Bank slip for contract #{0}", contractId);
            Contract contract = _contractRepository.GetContract(contractId);


            await _emailSender.SendEmailAsync(contract.CustomerEmail, subject, message);
        }

        /// <summary>
        /// Loads a viewmodel with the selected installments details
        /// </summary>
        /// <param name="installmentsIds">A list with all the desired installments id</param>
        /// <returns></returns>
        public BankSlipScheduleViewModel GetDetailsToSchedule(List<int> installmentsIds)
        {

            BankSlipScheduleViewModel scheduleViewModel = new BankSlipScheduleViewModel();

            scheduleViewModel.Installments = _contractRepository.GetDelayedInstallments(installmentsIds);
            scheduleViewModel.DueDate = DateTime.Now;
            scheduleViewModel.FeeValue = 5;
            scheduleViewModel.InterestValue = 1;

            return scheduleViewModel;
        }


        /// <summary>
        /// Deletes a contract from the database
        /// </summary>
        /// <param name="id">Id of the contract to be deleted</param>
        public void DeleteContract(string id)
        {
            bool success = _contractRepository.DeleteContract(id);
        }
        
        /// <summary>
        /// Returns a list with every contract in the database
        /// </summary>
        /// <returns></returns>
        public List<Contract> ListContracts()
        {

            return _contractRepository.GetAllContracts();
        }

        /// <summary>
        /// Get all the details (delayed installments, bank slips, contract information) for a given contract
        /// </summary>
        /// <param name="contractId">The id of the desired contract</param>
        /// <returns></returns>
        public ContractDetailViewModel GetContractDetail(string contractId)
        {
            ContractDetailViewModel dt = _contractRepository.GetContractDetail(contractId);
            
            return dt;
        }
    }
}
