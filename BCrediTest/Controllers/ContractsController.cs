using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrediTest.BusinessLayer;
using BCrediTest.Models;
using BCrediTest.Viewmodels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BCrediTest.Controllers
{
    public class ContractsController : Controller
    {
        private readonly BLContract _blContract;
        public ContractsController(BLContract blContract)
        {
            _blContract = blContract;
        }
        public IActionResult Index()
        {
            return View(_blContract.ListContracts());
        }

        [HttpGet]
        public IActionResult UploadData()
        {
            object[] fileTypes = new object[] {
               new {Id= 1, Text= "Contracts" },
               new {Id= 2, Text= "Delayed Installments" }
            };

            ViewBag.FileTypes = new SelectList(fileTypes, "Id", "Text");
            return View();
        }

        [HttpPost]
        public IActionResult UploadData(IFormFile file, int fileType)
        {

            _blContract.ImportFile(file, fileType);


            return RedirectToAction("UploadData");
        }

        public IActionResult Details(string id)
        {
            return View(_blContract.GetContractDetail(id));
        }

        public IActionResult Delete(string id)
        {
            _blContract.DeleteContract(id);
            return RedirectToAction("Index");
        }


        public IActionResult Schedule(ContractDetailViewModel installments)
        {
            List<int> installmentsIds = installments.DelayedInstallments.Where(x => x.IsSelected == true).Select(x=>x.InstallmentId).ToList();
            string currentId = (TempData["currentId"]).ToString();
            ViewBag.CurrentId = currentId;

            BankSlipScheduleViewModel bankSlipSchedule = _blContract.GetDetailsToSchedule(installmentsIds);
            

            return View(bankSlipSchedule);
        }

        public IActionResult CreateSlip(BankSlipScheduleViewModel bankslipSchedule)
        {
            //List<int> installmentsIds = installments.DelayedInstallments.Where(x => x.IsSelected == true).Select(x => x.InstallmentId).ToList();
            string currentId = (TempData["currentId"]).ToString();

            _blContract.CreateBankSlip(bankslipSchedule,currentId);

            return RedirectToAction("Details", new { id = currentId });
        }
    }
}