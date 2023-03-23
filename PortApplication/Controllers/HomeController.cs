using Ports.DAL;
using PortUpload;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PortApplication.Controllers
{
    public class HomeController : Controller
    {
        PortsEntities _db = new PortsEntities();
        PortHelper ufile = new PortHelper();

        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                
            }
            catch(Exception ex)
            {
                ufile.LogError(ex);
            }

            return View(_db.PORTDATAs);
        }

        public ActionResult Create()
        {
            List<string> CultureList = new List<string>();
            CultureList = ufile.getCountryList();

            ViewBag.Country = new SelectList(CultureList, "Country");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SaveConfig(FormCollection formCollection)
        {
            PORTDATA pData = new PORTDATA();
            string path = HttpContext.Server.MapPath("~/Data/Ports.json");
            string countryCode;
            countryCode = ufile.GetCultureInfo(formCollection["Country"].ToUpper());

            pData.Country = formCollection["Country"];
            pData.NAME = formCollection["NAME"];
            pData.UnctadPortCode = formCollection["UnctadPortCode"];
            pData.PortCode = countryCode + formCollection["UnctadPortCode"];
            pData.Longitude = formCollection["Longitude"];
            pData.Latitude = formCollection["Latitude"];
            pData.MainPortCode = formCollection["MainPortCode"];
            pData.Url = formCollection["Url"];

            try
            {
                var task = Task.Run(async () => await ufile.AddPort(pData));
                var response = task.Result;
            }

            catch (Exception ex)
            {
                ufile.LogError(ex);
                ViewBag.Message = ex.Message;
            }

            return View("Index", _db.PORTDATAs);
        }

        public ActionResult Delete(int id)
        {
            var task = Task.Run(async () => await ufile.Delete(id));
            var response = task.Result;

            return View("Index", _db.PORTDATAs);

        }

        public ActionResult Details(int id)
        {
            var pData = ufile.GetPortbyId(id);
            if (pData != null)
            {
                return View("Details", pData);
            }

            else
            {
                ViewBag.Message = "Port not found.";
                return View("Details");
            }

        }

        [HttpGet]
        public ActionResult BulkUpload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                if (file.FileName.Contains("json"))
                {
                    try
                    {
                        string path = HttpContext.Server.MapPath("~/Data");
                        var task = Task.Run(async () => await ufile.PortData(file, path));
                        var response = task.Result;
                        ViewBag.Message = response;
                    }
                    catch (Exception ex)
                    {
                        ufile.LogError(ex);
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                else
                {
                    ViewBag.Message = "Please select a JSON file to upload data";
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View("BulkUpload");
        }
    }
}