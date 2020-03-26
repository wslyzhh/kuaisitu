using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.manage
{
    public partial class selectPermission : Web.UI.ManagePage
    {
        private string area = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                area = DTRequest.GetQueryString("area");
                RptBind();
            }
        }
        #region 数据绑定=================================
        private void RptBind()
        {
            BLL.permission bll = new BLL.permission();
            this.rptList.DataSource = bll.GetList(0);
            this.rptList.DataBind();

        }
        #endregion
    }
}