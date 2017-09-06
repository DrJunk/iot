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
using Microsoft.AspNet.Identity;

namespace IRPiWebApp.Controllers
{
    public class RPiController : Controller
    {
        [Authorize]
        public ActionResult CreateRecording()
        {
            // The code in this section goes here.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("irpistorageaccount_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Collect user devices
            CloudTable devicesTable = tableClient.GetTableReference("UserDevicesTable");
            TableQuery<TableEntity> query =
                new TableQuery<TableEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, User.Identity.GetUserId())); // change

            List<string> deviceNames = new List<string>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<TableEntity> resultSegment = devicesTable.ExecuteQuerySegmented(query, token);
                token = resultSegment.ContinuationToken;

                foreach (TableEntity entity in resultSegment.Results)
                {
                    deviceNames.Add(entity.RowKey);
                }
            } while (token != null);

            ViewBag.Devices = deviceNames;

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult StartRecording(IRRecordModel model)
        {
            if (ModelState.IsValid)
            {
                IoTHubCloud.InvokeStartRecording(model.DeviceID);

                ViewBag.ProductName = model.ProductName;
                ViewBag.ActionName = model.ActionName;
                ViewBag.DeviceID = model.DeviceID;

                return View("NowRecording");
            }

            return Redirect("/RPi/CreateRecording");
        }

        [Authorize]
        public ActionResult EndRecording(string productName, string actionName,string deviceID)
        {
            IoTHubCloud.InvokeEndRecording(deviceID, productName, actionName);

            return Redirect("/Tables/GetPartition");
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

            IoTHubCloud.InvokeTransmit(irPartitionKey, irMessageCode);
            ViewBag.Result = true;

            return Redirect("/Tables/GetPartition");
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