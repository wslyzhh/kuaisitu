using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.manage
{
    public partial class permission_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString(); //操作类型
        private int id = 0;
        protected Model.manager manager = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            string _action = DTRequest.GetQueryString("action");
            this.id = DTRequest.GetQueryInt("id");
            ChkAdminLevel("sys_permission", _action); //检查权限
            if (!string.IsNullOrEmpty(_action) && _action == DTEnums.ActionEnum.Edit.ToString())
            {
                this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.permission().Exists(this.id))
                {
                    JscriptMsg("导航不存在或已被删除！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("sys_permission", DTEnums.ActionEnum.View.ToString()); //检查权限              
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
                else
                {
                    TreeBind();
                    if (this.id > 0)
                    {
                        this.ddlParentId.SelectedValue = this.id.ToString();
                    }
                }
            }
        }

        #region 绑定导航菜单=============================
        private void TreeBind()
        {
            BLL.permission bll = new BLL.permission();
            DataTable dt = bll.GetList(0);

            this.ddlParentId.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                string Id = dr["pe_id"].ToString();
                int ClassLayer = int.Parse(dr["class_layer"].ToString());
                string Title = dr["pe_code"].ToString().Trim()+ dr["pe_name"].ToString().Trim();

                if (ClassLayer == 1)
                {
                    this.ddlParentId.Items.Add(new ListItem(Title, Id));
                }
                else
                {
                    Title = "├ " + Title;
                    Title = Utils.StringOfChar(ClassLayer - 1, "　") + Title;
                    this.ddlParentId.Items.Add(new ListItem(Title, Id));
                }
            }
        }
        #endregion


        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.permission bll = new BLL.permission();
            Model.permission model = bll.GetModel(_id);

            TreeBind();
            ddlParentId.Enabled = false;
            ddlParentId.SelectedValue = model.pe_parentid.ToString();
            txtCode.Text = model.pe_code;
            txtName.Text = model.pe_name;
            txtRemark.Text = model.pe_remark;
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd()
        {
            try
            {
                Model.permission model = new Model.permission();
                BLL.permission bll = new BLL.permission();
                manager = GetAdminInfo();
                model.pe_parentid = int.Parse(ddlParentId.SelectedValue);
                model.pe_name = txtName.Text.Trim();
                model.pe_code = txtCode.Text.Trim();
                model.pe_remark = txtRemark.Text.Trim();
                return bll.Add(model, manager.user_name, manager.real_name);
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }
        #endregion

        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            try
            {
                BLL.permission bll = new BLL.permission();
                Model.permission model = bll.GetModel(_id);
                manager = GetAdminInfo();
                string content = string.Empty;
                if (model.pe_code != txtCode.Text.Trim())
                {
                    content += "权限代码：" + model.pe_code + "→<font color='red'>" + txtCode.Text.Trim() + "</font><br/>";
                }
                model.pe_code = txtCode.Text.Trim();
                if (model.pe_name != txtName.Text.Trim())
                {
                    content += "权限名称：" + model.pe_name + "→<font color='red'>" + txtName.Text.Trim() + "</font><br/>";
                }
                model.pe_name = txtName.Text.Trim();
                if (model.pe_remark != txtRemark.Text.Trim())
                {
                    content += "备注：" + model.pe_remark + "→<font color='red'>" + txtRemark.Text.Trim() + "</font><br/>";
                }
                model.pe_remark = txtRemark.Text.Trim();
                return bll.Update(model, content, manager.user_name, manager.real_name);
            }
            catch (Exception e)
            {
                return e.Message;
            }           
        }
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = string.Empty;
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                ChkAdminLevel("sys_permission", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (!string.IsNullOrEmpty(result))
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("修改权限成功！", "permission_list.aspx");
            }
            else //添加
            {
                ChkAdminLevel("sys_permission", DTEnums.ActionEnum.Add.ToString()); //检查权限
                result = DoAdd();
                if (!string.IsNullOrEmpty(result))
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("添加权限成功！", "permission_list.aspx");
            }
        }        
    }
}