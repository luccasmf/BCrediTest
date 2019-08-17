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
        public IActionResult UploadData(bool? success)
        {
            object[] fileTypes = new object[] {
               new {Id= 1, Text= "Contracts" },
               new {Id= 2, Text= "Delayed Installments" }
            };

            ViewBag.SuccessUpload = success;
            ViewBag.FileTypes = new SelectList(fileTypes, "Id", "Text");
            return View();
        }

        [HttpPost]
        public IActionResult UploadData(IFormFile file, int fileType)
        {

                return RedirectToAction("UploadData",new { success = _blContract.ImportFile(file, fileType) });


        }

        public IActionResult Details(string id, bool success = true)
        {
            ViewBag.Success = success;
            return View(_blContract.GetContractDetail(id));
        }

        public IActionResult Delete(string id)
        {
            _blContract.DeleteContract(id);
            return RedirectToAction("Index");
        }

        public IActionResult Schedule(ContractDetailViewModel installments, bool success = true)
        {
            List<int> installmentsIds = installments.DelayedInstallments.Where(x => x.IsSelected == true).Select(x => x.InstallmentId).ToList();
            string currentId = (TempData["currentId"]).ToString();
            ViewBag.CurrentId = currentId;

            if (!installmentsIds.Any())
                return RedirectToAction("Details", new { id = currentId,success = false });

            BankSlipScheduleViewModel bankSlipSchedule = _blContract.GetDetailsToSchedule(installmentsIds);
            ViewBag.Success = success;

            return View(bankSlipSchedule);
        }

        public IActionResult CreateSlip(BankSlipScheduleViewModel bankslipSchedule)
        {
            string currentId = (TempData["currentId"]).ToString();
            var data = _blContract.GetContractDetail(currentId);
            if (bankslipSchedule.DueDate.Date < DateTime.Now.Date)
                return RedirectToAction("Details", new { id = currentId, success = false });


            _blContract.CreateBankSlip(bankslipSchedule, currentId);

            return RedirectToAction("Details", new { id = currentId });
        }

        public IActionResult MarkAsPaid(ContractDetailViewModel detailViewModel)
        {
            string currentId = (TempData["currentId"]).ToString();

            bool success = _blContract.MarkAsPaid(detailViewModel.BankSlips.Where(x=>x.IsSelected == true).Select(x=>x.BankslipId).ToArray());

            return RedirectToAction("Details", new { id = currentId });
        }
    }
}