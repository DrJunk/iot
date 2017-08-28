using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure.Devices;
using System.Threading.Tasks;


namespace IRPiWebApp.Controllers
{
    public class HomeController : Controller
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=MainIoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=f4RkS/lSdFcJcSIoRqaKKKgWnGhlYOe5GSHORqLIZxA=";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SendToRPI()
        {
            ViewBag.Message = "Sent message.";

            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            InvokeMethod();

            return View();
        }   
        private static async Task InvokeMethod()
        {
            var methodInvocation = new CloudToDeviceMethod("writeLine") { ResponseTimeout = TimeSpan.FromSeconds(30) };
            IRMessage msg =new IRMessage(new List<double>(new double[] { 1000, 2000, 1000, 2000, 3000 }), true);

            methodInvocation.SetPayloadJson("'" + msg.Encode() + "'");

            var response = await serviceClient.InvokeDeviceMethodAsync("MainDevice", methodInvocation);

            Console.WriteLine("Response status: {0}, payload:", response.Status);
            Console.WriteLine(response.GetPayloadAsJson());
        }
    }
}