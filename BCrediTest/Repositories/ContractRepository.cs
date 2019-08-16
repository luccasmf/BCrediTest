using BCrediTest.Data;
using BCrediTest.Models;
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
