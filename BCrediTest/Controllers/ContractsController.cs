using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrediTest.BusinessLayer;
using BCrediTest.Models;
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
            return View(contracts);
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
        public IActionResult UploadData(IFormFile file, string fileType)
        {

            return View();
        }

        public IActionResult Details(string id)
        {

            return View();
        }

        public IActionResult Delete(string id)
        {

            return RedirectToAction("Index");
        }
    }
}