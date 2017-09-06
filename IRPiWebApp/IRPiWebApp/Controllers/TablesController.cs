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
    public class TablesController : Controller
    {
        // GET: Tables
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult CreateTable()
        {
            // The code in this section goes here.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("irpistorageaccount_AzureStorageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("TestTable");

            ViewBag.Success = table.CreateIfNotExists();

            ViewBag.TableName = table.Name;

            return View();
        }

        [Authorize]
        public ActionResult GetPartition()
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

            List<IREntity> customers = new List<IREntity>();
            // Collect IR recordings for each device
            foreach (string deviceName in deviceNames)
            {
                CloudTable table = tableClient.GetTableReference("IRRecordingTable");
                TableQuery<IREntity> irQuery =
                    new TableQuery<IREntity>()
                        .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, deviceName)); // change

                token = null;
                do
                {
                    TableQuerySegment<IREntity> resultSegment = table.ExecuteQuerySegmented(irQuery, token);
                    token = resultSegment.ContinuationToken;

                    foreach (IREntity customer in resultSegment.Results)
                    {
                        customers.Add(customer);
                    }
                } while (token != null);
            }

            return View(customers);
        }

        [Authorize]
        public ActionResult GetSchedule()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("irpistorageaccount_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("IRScheduleTable");

            TableQuery<ScheduleEntity> query = new TableQuery<ScheduleEntity>();

            List<ScheduleEntity> customers = new List<ScheduleEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<ScheduleEntity> resultSegment = table.ExecuteQuerySegmented(query, token);
                token = resultSegment.ContinuationToken;

                foreach (ScheduleEntity customer in resultSegment.Results)
                {
                    if (customer.RowKey.StartsWith(User.Identity.GetUserId() + ";"))
                        customers.Add(customer);
                }
            } while (token != null);

            return View(customers);
        }

        [Authorize]
        public ActionResult SetNewSchedule(string deviceID, string productName, string actionName)
        {
            ViewBag.scheduleDeviceID = deviceID;
            ViewBag.scheduleProductName = productName;
            ViewBag.scheduleActionName = actionName;
            return View("SelectTimeSchedule");
        }

        [Authorize]
        public ActionResult AddSchedule(DateTime scheduleTime, string deviceID, string productName, string actionName, int timeZoneOffset)
        {
            DateTimeOffset scheduleTimeOffset = DateTime.SpecifyKind(scheduleTime, DateTimeKind.Utc);
            scheduleTimeOffset = scheduleTimeOffset.AddMinutes(timeZoneOffset);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("irpistorageaccount_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            //Open recording table and retrive the IREntity
            CloudTable recordTable = tableClient.GetTableReference("IRRecordingTable");
            IREntity irEntity = new IREntity("", deviceID, productName, actionName);
            TableOperation retrieveOperation = TableOperation.Retrieve<IREntity>(irEntity.PartitionKey, irEntity.RowKey);
            TableResult result = recordTable.Execute(retrieveOperation);
            irEntity = (IREntity)result.Result;
            if (irEntity == null)
            {
                ViewBag.Result = false;
                return View("ScheduleError");

            }

            //Open scheduling table and add the schedule
            CloudTable scheduleTable = tableClient.GetTableReference("IRScheduleTable");
            TableOperation addOperation = TableOperation.Insert(new ScheduleEntity(irEntity, scheduleTimeOffset, User.Identity.GetUserId()));
            result = scheduleTable.Execute(addOperation);

            ViewBag.Result = true;
            return Redirect("/Tables/GetSchedule");
        }

        [Authorize]
        public ActionResult AddDevice(string deviceID)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("irpistorageaccount_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
       

            //Open scheduling table and add the schedule
            CloudTable scheduleTable = tableClient.GetTableReference("UserDevicesTable");
            TableOperation addOperation = TableOperation.Insert(new TableEntity(User.Identity.GetUserId(), deviceID));
            TableResult result = scheduleTable.Execute(addOperation);

            ViewBag.Result = true;

            return Redirect("/Tables/Devices");
        }

        public ActionResult AddDeviceForm()
        {
            return View();
        }

        public ActionResult Devices()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("irpistorageaccount_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UserDevicesTable");
            
            TableQuery<TableEntity> query =
                new TableQuery<TableEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, User.Identity.GetUserId()));

            List<TableEntity> customers = new List<TableEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<TableEntity> resultSegment = table.ExecuteQuerySegmented(query, token);
                token = resultSegment.ContinuationToken;

                foreach (TableEntity customer in resultSegment.Results)
                {
                    customers.Add(customer);
                }
            } while (token != null);

            return View(customers);
        }
    }
}