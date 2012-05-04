using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace Web.Hubs
{
    public class Notifications : Hub
    {
        public void Send(string msg)
        {
            Clients.addMessage(msg);
        }
    }
}
