using BCrediTest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCrediTest.Repositories
{
    public interface IContractRepository
    {

    }
    public class ContractRepository: IContractRepository
    {
        private readonly BCrediDbContext _context;
        public ContractRepository(BCrediDbContext context)
        {
            _context = context;
        }
    }
}
