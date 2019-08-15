using BCrediTest.Repositories;
using System;
using System.Collections.Generic;
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
    }
}
