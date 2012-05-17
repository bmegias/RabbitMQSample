using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Server.Messages;

namespace AsyncWeb.Controllers
{
    public class HomeController : AsyncController
    {
        [AsyncTimeout(5000)]
        [AcceptVerbs(HttpVerbs.Post)]
        public void SayHelloAsync(string name)
        {
            AsyncManager.OutstandingOperations.Increment();

            var pub = RabbitUtils.Publisher;
            pub
                .PublishRequest<HelloRequest, HelloResponse>(
                    new HelloRequest() { Name = name }
                    , response =>
                    {
                        AsyncManager.Parameters["msg"] = response.HelloMessage;
                        AsyncManager.OutstandingOperations.Decrement();
                    });
        }

        public JsonResult SayHelloCompleted(string msg)
        {
            return Json(msg);
        }
    }
}
