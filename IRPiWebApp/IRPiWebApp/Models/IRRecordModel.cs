using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace IRPiWebApp.Models
{
    public class IRRecordModel
    {
        [Required]
        public string ProductName { get; set; }

        [Required]
        public string ActionName { get; set; }
    }
}