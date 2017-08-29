using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace IRPiWebApp.Models
{
    public class IREntity : TableEntity
    {
        public IREntity(string irMessageCode, string deviceName, string productName, string actionName)
        {
            this.PartitionKey = deviceName;
            this.RowKey = productName + ";" + actionName;
            this.IRMessageCode = irMessageCode;
        }

        public IREntity() { }

        public string IRMessageCode { get; set; }
    }
}