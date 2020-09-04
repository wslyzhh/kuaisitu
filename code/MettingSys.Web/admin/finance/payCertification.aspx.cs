using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.finance
{
    public partial class payCertification : Web.UI.ManagePage
    {
        protected int rpid = 0, rptype = 0;
        protected Model.manager manager = null;
        protected DataRow dr = null;
        protected DataTable detailDT = null;
        protected string typeName="付款";

        protected void Page_Load(object sender, EventArgs e)
        {
            rpid = DTRequest.GetInt("rpid",0);
            if (rpid == 0)
            {
                JscriptMsg("传输参数不正确！", "back");
                return;
            }
            rptype = DTRequest.GetInt("rptype", 0);
            if (rptype == 1)
            {
                typeName = "收款";
            }
            RptBind("rpd_rpid=" + rpid + "", "rpd_oid");
        }
        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            manager = GetAdminInfo();
            DataTable dt = new BLL.ReceiptPay().GetList(0, "rp_id=" + rpid, "rp_id").Tables[0];
            if (dt == null || dt.Rows.Count == 0)
            {
                JscriptMsg("数据不存在！", "back");
                return;
            }
            dr = dt.Rows[0];
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            detailDT = bll.GetPayCertificationList(_strWhere).Tables[0];
            
            //if (dt!=null)
            //{
            //    labCusName.Text = dt.Rows[0]["c_name"].ToString();
            //    labPayStatus.Text = Utils.StrToBool(Utils.ObjectToStr(dt.Rows[0]["rp_isConfirm"]), false) ? "已付款" : "未付款";
            //    labPayDate.Text = Utils.StrToBool(Utils.ObjectToStr(dt.Rows[0]["rp_isConfirm"]), false) ? ConvertHelper.toDate(dt.Rows[0]["rp_date"]).Value.ToString("yyyy-MM-dd") : "无";
            //    labPayMethod.Text = Utils.StrToBool(Utils.ObjectToStr(dt.Rows[0]["rp_isConfirm"]), false) ? dt.Rows[0]["pm_name"].ToString() : "无";
            //    labText.Text = dt.Rows[0]["rp_money"].ToString();
            //    if (unMoney > 0)
            //    {
            //        labText.Text += "，其中未分配金额为：" + unMoney;
            //    }
            //}
            
        }
        #endregion
    }
}