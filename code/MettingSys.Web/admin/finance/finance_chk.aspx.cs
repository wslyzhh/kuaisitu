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
    public partial class finance_chk : Web.UI.ManagePage
    {
        protected int finid = 0;
        protected string oid = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            finid = DTRequest.GetQueryInt("finid");
            oid = DTRequest.GetQueryString("oid");
            ShowInfo(finid);
        }

        private void ShowInfo(int _id)
        {
            DataSet ds = new BLL.finance_chk().GetList(0, "fc_finid=" + _id + "", "fc_addDate desc");
            if (ds==null && ds.Tables[0].Rows.Count == 0)
            {
                JscriptMsg("传输参数不正确！", "back");
                return;
            }

            rptList.DataSource = ds;
            rptList.DataBind();

        }
    }
}