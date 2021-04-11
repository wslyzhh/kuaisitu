using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.customer
{
    public partial class contact_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        private int id = 0;
        private int cid = 0;

        protected Model.business_log log = null;
        protected Model.manager manager = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            string _action = DTRequest.GetQueryString("action");
            this.cid = DTRequest.GetQueryInt("cid");

            if (!string.IsNullOrEmpty(_action) && _action == DTEnums.ActionEnum.Edit.ToString())
            {
                this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                this.id = DTRequest.GetQueryInt("id");
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.Contacts().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                //ChkAdminLevel("sys_customer_list", DTEnums.ActionEnum.View.ToString()); //检查权限
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
            }
        }
        
        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.Contacts bll = new BLL.Contacts();
            Model.Contacts model = bll.GetModel(_id);

            txtName.Text = model.co_name;
            txtPhone.Text = model.co_number;
            if (model.co_flag.Value)
            {
                labflag.Text = "主要联系人";
            }
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd()
        {
            Model.Contacts model = new Model.Contacts();
            BLL.Contacts bll = new BLL.Contacts();
            manager = GetAdminInfo();            
            model.co_cid = cid;
            model.co_flag = false;
            model.co_name = txtName.Text.Trim();
            model.co_number = txtPhone.Text.Trim();
            return bll.Add(model,manager);
        }
        #endregion

        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.Contacts bll = new BLL.Contacts();
            Model.Contacts model = bll.GetModel(_id);
            manager = GetAdminInfo();
            string _content = string.Empty;
            model.co_cid = cid;
            if (model.co_name != txtName.Text.Trim())
            {
                _content += "联系人:" + model.co_name + "→<font color='red'>" + txtName.Text.Trim() + "<font><br/>";
            }
            model.co_name = txtName.Text.Trim();
            if (model.co_number != txtPhone.Text.Trim())
            {
                _content += "联系号码:" + model.co_number + "→<font color='red'>" + txtPhone.Text.Trim() + "<font><br/>";
            }
            model.co_number = txtPhone.Text.Trim();
            return bll.Update(model, manager, _content);            
        }
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = "", msbox = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                //ChkAdminLevel("sys_customer_list", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    msbox = "parent.parent.jsprint(\"" + result + "\", \"\");";
                    ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
                    return;
                }
                msbox = "parent.parent.jsprint(\"修改联系人成功！\", \"customer_edit.aspx?action=Edit&id=" + this.cid + "\");";
                ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
            }
            else //添加
            {
                //ChkAdminLevel("sys_customer_list", DTEnums.ActionEnum.Add.ToString()); //检查权限
                result = DoAdd();
                if (result != "")
                {
                    msbox = "parent.parent.jsprint(\"" + result + "\", \"\");";
                    ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
                    return;
                }
                msbox = "parent.parent.jsprint(\"添加联系人成功！\", \"customer_edit.aspx?action=Edit&id=" + this.cid + "\");";
                ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
            }
        }
    }
}