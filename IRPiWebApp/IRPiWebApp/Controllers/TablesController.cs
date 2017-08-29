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
        public ActionResult Index()
        {
            return View();
        }

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

        public ActionResult AddEntity()
        {
            // The code in this section goes here.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("irpistorageaccount_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("TestTable");

            CustomerEntity customer1 = new CustomerEntity("Harp", "Walter");
            customer1.Email = "Walter@contoso.com";

            TableOperation insertOperation = TableOperation.Insert(customer1);
            TableResult result = table.Execute(insertOperation);

            ViewBag.TableName = table.Name;
            ViewBag.Result = result.HttpStatusCode;
            return View();
        }

        public ActionResult GetPartition()
        {
            // The code in this section goes here.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("irpistorageaccount_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("deviceData");
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