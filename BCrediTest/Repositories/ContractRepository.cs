using BCrediTest.Data;
using BCrediTest.Models;
using BCrediTest.Viewmodels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.Repositories
{
    public interface IContractRepository
    {
        bool PersistContracts(List<Contract> contracts);
        List<Contract> GetAllContracts();
        ContractDetailViewModel GetContractDetail(string contractId);
        bool PersistInstallments(List<DelayedInstallment> installments);
        bool DeleteContract(string id);
        List<DelayedInstallment> GetDelayedInstallments(List<int> installmentsIds);
        BankSlip PersistBankSlip(BankSlip bankSlip);
        Contract GetContract(string contractId);
        bool MarkSlipsAsPaid(int[] slipIds);
    }
    public class ContractRepository : IContractRepository
    {
        private readonly BCrediDbContext _context;
        public ContractRepository(BCrediDbContext context)
        {
            _context = context;
        }

        public bool DeleteContract(string id)
        {
            _context.Contracts.Remove(_context.Contracts.FirstOrDefault(x => x.ExternalId == id));

            return _context.SaveChanges() > 0;
        }

        public List<Contract> GetAllContracts()
        {
            return _context.Contracts.ToList();
        }

        public Contract GetContract(string contractId)
        {
            return _context.Contracts.FirstOrDefault(x => x.ExternalId == contractId);
        }

        public ContractDetailViewModel GetContractDetail(string contractId)
        {
            ContractDetailViewModel details = new ContractDetailViewModel();
            Contract c = _context.Contracts.Where(x => x.ExternalId == contractId)
                .Include(x => x.Installments)
                .FirstOrDefault();

            details.Contract = c;
            details.DelayedInstallments = c.Installments.Where(x=>x.Delayed==true).ToList();

            int[] installmentsId = details.DelayedInstallments.Select(x => x.InstallmentId).ToArray();

            details.BankSlips = (from bk in _context.BankSlips
                                   where bk.BankSlipInstallment.Any(x => installmentsId.Contains(x.InstallmentId))
                                   && bk.Status == BankSlipStatus.Pending
                                   select bk).ToList();
            return details;
        }

        public List<DelayedInstallment> GetDelayedInstallments(List<int> installmentsIds)
        {
            return _context.DelayedInstallments.Where(x => installmentsIds.Contains(x.InstallmentId)).ToList();
        }

        public bool MarkSlipsAsPaid(int[] slipIds)
        {
            List<BankSlip> bankSlips = _context.BankSlips.Where(x => slipIds.Contains(x.BankslipId)).Include(x => x.BankSlipInstallment).ThenInclude(x => x.DelayedInstallment).ToList();
            List<DelayedInstallment> installments = new List<DelayedInstallment>();

           bankSlips.ForEach(x =>
            {
                x.Status = BankSlipStatus.Paid;
                installments.AddRange(x.BankSlipInstallment.Select(y => y.DelayedInstallment));
            });

            installments.ForEach(x => x.Delayed = false);

            return _context.SaveChanges() > 0;
        }

        public BankSlip PersistBankSlip(BankSlip bankSlip)
        {
            _context.BankSlips.Add(bankSlip);
            _context.SaveChanges();
            return bankSlip;
        }

        public bool PersistContracts(List<Contract> contractList)
        {
            _context.Contracts.AddRange(contractList);
            
            return _context.SaveChanges() == contractList.Count;
        }

        public bool PersistInstallments(List<DelayedInstallment> installments)
        {
            _context.DelayedInstallments.AddRange(installments);
            return _context.SaveChanges() == installments.Count;
        }
    }
}
