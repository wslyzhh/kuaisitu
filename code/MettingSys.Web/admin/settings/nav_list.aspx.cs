using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MettingSys.Common;

namespace MettingSys.Web.admin.settings
{
    public partial class nav_list : Web.UI.ManagePage
    {
        protected string navtype = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.navtype = DTRequest.GetQueryStringValue("navtype", "OA");

            if (!Page.IsPostBack)
            {
                ChkAdminLevel("sys_navigation", DTEnums.ActionEnum.View.ToString()); //检查权限
                this.ddlNavType.SelectedValue = this.navtype;
                RptBind(this.navtype);
            }
        }

        //数据绑定
        private void RptBind(string nav_type)
        {
            BLL.navigation bll = new BLL.navigation();
            DataTable dt = bll.GetList(0, nav_type);
            this.rptList.DataSource = dt;
            this.rptList.DataBind();
        }

        //保存排序
        protected void btnSave_Click(object sender, EventArgs e)
        {
            ChkAdminLevel("sys_navigation", DTEnums.ActionEnum.Edit.ToString()); //检查权限
            BLL.navigation bll = new BLL.navigation();
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                int id = Convert.ToInt32(((HiddenField)rptList.Items[i].FindControl("hidId")).Value);
                int sortId;
                if (!int.TryParse(((TextBox)rptList.Items[i].FindControl("txtSortId")).Text.Trim(), out sortId))
                {
                    sortId = 99;
                }
                bll.UpdateField(id, "sort_id=" + sortId.ToString());
            }
            AddAdminLog(DTEnums.ActionEnum.Edit.ToString(), "保存导航排序"); //记录日志
            JscriptMsg("保存排序成功！", "nav_list.aspx");
        }

        //删除导航
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ChkAdminLevel("sys_navigation", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.navigation bll = new BLL.navigation();
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                int id = Convert.ToInt32(((HiddenField)rptList.Items[i].FindControl("hidId")).Value);
                CheckBox cb = (CheckBox)rptList.Items[i].FindControl("chkId");
                if (cb.Checked)
                {
                    bll.Delete(id);
                }
            }
            AddAdminLog(DTEnums.ActionEnum.Delete.ToString(), "删除导航菜单"); //记录日志
            JscriptMsg("删除数据成功！", "nav_list.aspx", "parent.loadMenuTree");
        }

        protected void ddlNavType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.navtype = ddlNavType.SelectedValue;
            RptBind(this.navtype);
        }

    }
}