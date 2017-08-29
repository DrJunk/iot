using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using IRPiWebApp.Models;


namespace IRPiWebApp.Controllers
{
    public class RPiController : Controller
    {
        // GET: RPi
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StartRecording()
        {

            return View();
        }

        public ActionResult EndRecording()
        {
            return View();
        }

        public ActionResult Transmit(string irMessageCode)
        {
            irMessageCode = irMessageCode.Substring(6, irMessageCode.Length - 10); // Parse from JSON object
            IoTHubCloud.InvokeTransmit("MainDevice", irMessageCode);
            ViewBag.Result = true;
            return View();
        }
    }
}