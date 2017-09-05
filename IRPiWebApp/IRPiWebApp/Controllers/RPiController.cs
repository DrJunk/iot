using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using IRPiWebApp.Models;


namespace IRPiWebApp.Controllers
{
    public class RPiController : Controller
    {
        [Authorize]
        public ActionResult CreateRecording()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult StartRecording(IRRecordModel model)
        {
            if (ModelState.IsValid)
            {
                IoTHubCloud.InvokeStartRecording("MainDevice");

                ViewBag.ProductName = model.ProductName;
                ViewBag.ActionName = model.ActionName;

                return View("NowRecording");
            }

            return View("CreateRecording", model);
        }

        [Authorize]
        public ActionResult EndRecording(string productName, string actionName)
        {
            IoTHubCloud.InvokeEndRecording("MainDevice", productName, actionName);

            return View();
        }

        [Authorize]
        public ActionResult Transmit(string irPartitionKey, string irRowKey)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("irpistorageaccount_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("IRRecordingTable");
            TableOperation retrieveOperation = TableOperation.Retrieve<IREntity>(irPartitionKey, irRowKey);

            TableResult result = table.Execute(retrieveOperation);
            string irMessageCode = ((IREntity)result.Result).IRMessageCode;

            IoTHubCloud.InvokeTransmit("MainDevice", irMessageCode);
            ViewBag.Result = true;
            return View();
        }

        [Authorize]
        public ActionResult Delete(string irPartitionKey, string irRowKey)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("irpistorageaccount_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("IRRecordingTable");
            TableOperation retrieveOperation = TableOperation.Retrieve<IREntity>(irPartitionKey, irRowKey);
            TableResult retrieveResult = table.Execute(retrieveOperation);
            TableOperation deleteOperation = TableOperation.Delete((IREntity)retrieveResult.Result);
            TableResult deleteResult = table.Execute(deleteOperation);

            ViewBag.Result = true;
            return Redirect("/Tables/GetPartition");
        }
    }
}