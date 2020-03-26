using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MettingSys.Common;

namespace MettingSys.Web
{
    public partial class main : Web.UI.ManagePage
    {
        protected Model.manager admin_info; //员工信息
        protected string DepartName = "暂无";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                admin_info = GetAdminInfo();
                DepartName = admin_info.detaildepart;
            }
        }

        //安全退出
        protected void lbtnExit_Click(object sender, EventArgs e)
        {
            Session[DTKeys.SESSION_ADMIN_INFO] = null;
            Utils.WriteCookie("AdminName", "hncbyhz", -14400);
            Utils.WriteCookie("AdminPwd", "hncbyhz", -14400);
            Response.Redirect("default.aspx");
        }
    }
}