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
    public partial class bankDetails : Web.UI.ManagePage
    {
        protected DataTable dt = null;
        protected int cid = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            cid = DTRequest.GetQueryInt("cid", 0);
            if (!Page.IsPostBack)
            {
                DtBind();
            }
        }
        private void DtBind()
        {
            dt = new DataTable();
            BLL.customerBank bll = new BLL.customerBank();
            dt = bll.GetList(0, "cb_cid=" + cid + " and cb_flag=1", "cb_id asc", false).Tables[0];
        }
    }
}