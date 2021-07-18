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
    public partial class selectEmployee : Web.UI.ManagePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RptBind();
            }
        }
        #region 数据绑定=================================
        private void RptBind()
        {
            BLL.department bll = new BLL.department();
            this.rptList1.DataSource = bll.getAllEmployee("");
            this.rptList1.DataBind();

        }
        #endregion

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BLL.department bll = new BLL.department();
            this.rptList1.DataSource = bll.getAllEmployee("", txtPerson.Text.ToUpper());
            this.rptList1.DataBind();
        }
    }
}