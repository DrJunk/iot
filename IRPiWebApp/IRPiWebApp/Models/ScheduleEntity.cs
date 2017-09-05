using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace IRPiWebApp.Models
{
    public class ScheduleEntity : TableEntity
    {
        public ScheduleEntity(string PartitionKey, string RowKey, string ProductName, string ActionName, string IRMessageCode)
        {
            this.PartitionKey = PartitionKey;
            this.RowKey = RowKey;
            this.DeviceID = DeviceID;
            this.ProductName = ProductName;
            this.ActionName = ActionName;
            this.IRMessageCode = IRMessageCode;
        }

        public ScheduleEntity(IREntity irEntity)
        {
            this.PartitionKey = null;
            this.RowKey = null;
            this.DeviceID = irEntity.PartitionKey;
            this.ProductName = irEntity.RowKey.Split(';')[0];
            this.ActionName = irEntity.RowKey.Split(';')[1];
            this.IRMessageCode = irEntity.IRMessageCode;
        }

        public ScheduleEntity() { }

        public string DeviceID { get; set; }

        public string IRMessageCode { get; set; }

        public string ProductName { get; set; }

        public string ActionName { get; set; }
    }
}