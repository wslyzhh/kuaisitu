using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.order
{
    public partial class chkDetails : Web.UI.ManagePage
    {
        protected DataTable dt = null;
        protected int finid = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            finid = DTRequest.GetQueryInt("finid",0);
            if (!Page.IsPostBack)
            {
                DtBind();
            }
        }

        private void DtBind()
        {
            dt = new DataTable();
            dt = new BLL.finance_chk().GetList(0, "fc_finid=" + finid + "", "fc_addDate desc").Tables[0];
        }
    }
}