using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.unBusiness
{
    public partial class printCertificate : Web.UI.ManagePage
    {
        protected int _ubaid = 0;
        protected Model.manager manager = null;
        protected DataRow dr = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _ubaid = DTRequest.GetInt("ubaid",0);
            if (_ubaid == 0)
            {
                JscriptMsg("传输参数不正确！", "back");
                return;
            }
            DataTable dt = new BLL.unBusinessApply().GetList(0, "uba_id=" + _ubaid + "", "uba_id").Tables[0];
            if (dt == null || dt.Rows.Count == 0)
            {
                JscriptMsg("数据不存在！", "back");
                return;
            }
            dr = dt.Rows[0];
            manager = GetAdminInfo();
        }
    }
}