using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MettingSys.Common;

namespace MettingSys.Web.admin.order
{
    public partial class mutliFinance : Web.UI.ManagePage
    {
        protected string oID = "", cid = "", cusName = "";
        //protected string minDate = "", maxDate = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            oID = DTRequest.GetString("oID");
            cid = DTRequest.GetString("cid");
            cusName = DTRequest.GetString("cusname");
            Model.Order order = new BLL.Order().GetModel(oID);
            if (order == null)
            {
                JscriptMsg("订单不存在！", "back");
                return;
            }
            //minDate = order.o_sdate.Value.ToString("yyyy-MM-dd");
            //maxDate = order.o_edate.Value.ToString("yyyy-MM-dd");
        }
    }
}