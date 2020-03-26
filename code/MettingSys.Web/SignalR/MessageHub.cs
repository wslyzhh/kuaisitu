using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MettingSys.Web.SignalR
{
    [HubName("messageService")]
    public class MessageHub:Hub
    {
        [HubMethodName("show")]
        public static void Show()
        {
            IHubContext contentext = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
            contentext.Clients.All.displayDatas();
        }
    }
}