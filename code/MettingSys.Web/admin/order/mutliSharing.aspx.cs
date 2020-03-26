using MettingSys.Common;
using MettingSys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.order
{
    public partial class mutliSharing : Web.UI.ManagePage
    {
        protected string oID = "", trHtml = "", sdate = "", edate = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            oID = DTRequest.GetString("oID");
            Model.Order order = new BLL.Order().GetModel(oID);
            if (order == null)
            {
                JscriptMsg("订单不存在！", "back");
                return;
            }
            if (order.o_lockStatus==1)
            {
                PrintJscriptMsg("订单已锁定，不能再添加合作分成", "");
                return;
            }
            sdate = order.o_sdate.Value.ToString("yyyy-MM-dd");
            edate = order.o_edate.Value.ToString("yyyy-MM-dd");
            OrderPerson op = order.personlist.Where(p => p.op_type == 1).ToArray()[0];
            string place = ("," + order.o_place + ",").Replace(","+ op.op_area + ",", ",");
            string area = "";
            if (!string.IsNullOrEmpty(place))
            {
                string[] plist = place.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in plist)
                {
                    area = new BLL.department().getAreaText(item);
                    trHtml += "<tr style=\"text-align: center;\" class=\"computeTR\">";
                    trHtml += "<td>" + area + "</td>";
                    trHtml += "<td>业绩分成(" + area + ")</td>";
                    trHtml += "<td><input type=\"text\" name=\"txtExpression1\" data-area=\""+ item + "\" style=\"width: 200px; \" class=\"input\" onblur=\"computeResult(this,1)\" /><span></span></td>";
                    trHtml += "<td><span class=\"computeInput1\">0</span></td>";
                    trHtml += "<td><input type=\"text\" name=\"txtExpression0\" data-area=\"" + item + "\" style=\"width: 200px; \" class=\"input\" onblur=\"computeResult(this,0)\" /><span></span></td>";
                    trHtml += "<td><span class=\"computeInput0\">0</span></td>";
                    trHtml += "</tr>";
                }
            }
            area = new BLL.department().getAreaText(op.op_area);
            trHtml += "<tr style=\"text-align: center;\">";
            trHtml += "<td>"+ area + "</td>";
            trHtml += "<td>业绩分成(" + area + ")</td>";
            trHtml += "<td><input type=\"text\" id=\"txtArea1\" name=\"txtExpression1\" data-area=\"" + op.op_area + "\"  disabled=\"disabled\" style=\"width: 200px; \" class=\"input\" value=\"0\" /><span></span></td>";
            trHtml += "<td>0</td>";
            trHtml += "<td><input type=\"text\" id=\"txtArea0\" name=\"txtExpression0\" data-area=\"" + op.op_area + "\" disabled=\"disabled\" style=\"width: 200px; \" class=\"input\" value=\"0\" /><span></span></td>";
            trHtml += "<td>0</td>";
            trHtml += "</tr>";
        }
    }
}