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
        public string area = string.Empty, hasOrder = string.Empty, employeeType = string.Empty, ratio3 = string.Empty, ratio6 = string.Empty;
        protected bool IsshowNum = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            area = DTRequest.GetQueryString("area");
            IsshowNum = Utils.StrToBool(DTRequest.GetQueryString("showNum"), false);
            hasOrder = DTRequest.GetQueryString("hasOrder");
            employeeType = DTRequest.GetQueryString("type");
            ratio3 = DTRequest.GetQueryString("ratio3");
            ratio6 = DTRequest.GetQueryString("ratio6");
            if (!Page.IsPostBack)
            {
                RptBind();
            }
        }
        #region 数据绑定=================================
        private void RptBind()
        {
            BLL.department bll = new BLL.department();
            if (IsshowNum)
            {
                this.rptList.DataSource = bll.getAllEmployee(area, "", IsshowNum, hasOrder);
                this.rptList.DataBind();
            }
            else
            {
                this.rptList1.DataSource = bll.getAllEmployee(area, "");
                this.rptList1.DataBind();
            }
            
        }
        #endregion

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BLL.department bll = new BLL.department();
            if (IsshowNum)
            {
                this.rptList.DataSource = bll.getAllEmployee(area, txtPerson.Text.ToUpper(), IsshowNum, hasOrder);
                this.rptList.DataBind();
            }
            else
            {
                this.rptList1.DataSource = bll.getAllEmployee(area, txtPerson.Text.ToUpper());
                this.rptList1.DataBind();
            }
        }
    }
}