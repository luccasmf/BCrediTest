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
    }
    public class ContractRepository : IContractRepository
    {
        private readonly BCrediDbContext _context;
        public ContractRepository(BCrediDbContext context)
        {
            _context = context;
        }

        public List<Contract> GetAllContracts()
        {
            return _context.Contracts.ToList();
        }

        public ContractDetailViewModel GetContractDetail(string contractId)
        {
            Contract c = _context.Contracts.Where(x => x.ExternalId == contractId).Include(x => x.Installments.Where(y => y.Delayed == true)).FirstOrDefault();

            return new ContractDetailViewModel();
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
