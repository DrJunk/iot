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
            CloudTable table = tableClient.GetTableReference("IRRecordingTable");
            TableQuery<IREntity> query =
                new TableQuery<IREntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "MainDevice")); // change

            List<IREntity> customers = new List<IREntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<IREntity> resultSegment = table.ExecuteQuerySegmented(query, token);
                token = resultSegment.ContinuationToken;

                foreach (IREntity customer in resultSegment.Results)
                {
                    customers.Add(customer);
                }
            } while (token != null);

            return View(customers);
        }
    }


}