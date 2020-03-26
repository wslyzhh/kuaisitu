using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin
{
    public partial class selectEmployee : Web.UI.ManagePage
    {
        private string area = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            area = DTRequest.GetQueryString("area");
            if (!Page.IsPostBack)
            {
                RptBind();
            }
        }
        #region 数据绑定=================================
        private void RptBind()
        {
            BLL.department bll = new BLL.department();
            this.rptList.DataSource = bll.getAllEmployee(area);
            this.rptList.DataBind();
            
        }
        #endregion

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BLL.department bll = new BLL.department();
            this.rptList.DataSource = bll.getAllEmployee(area,txtPerson.Text.ToUpper());
            this.rptList.DataBind();
        }
    }
}