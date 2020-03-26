using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MettingSys.Common;
using System.Text;

namespace MettingSys.Web.admin.manager
{
    public partial class manager_edit : Web.UI.ManagePage
    {
        string defaultpassword = "0|0|0|0"; //默认显示密码
        private string action = DTEnums.ActionEnum.Add.ToString(); //操作类型
        private int id = 0;
        protected Model.manager manager = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            string _action = DTRequest.GetQueryString("action");
            if (!string.IsNullOrEmpty(_action) && _action == DTEnums.ActionEnum.Edit.ToString())
            {
                this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                if (!int.TryParse(Request.QueryString["id"] as string, out this.id))
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.manager().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("manager_list", DTEnums.ActionEnum.View.ToString()); //检查权限
                Model.manager model = GetAdminInfo(); //取得管理员信息
                RoleBind(ddlRoleId, model.role_type);
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
                else
                {
                    TreeBind();
                }
            }
        }
        #region 绑定导航菜单=============================
        private void TreeBind()
        {
            BLL.department bll = new BLL.department();
            DataTable dt = bll.GetList(0,"", false);

            this.ddlParentId.Items.Clear();
            this.ddlParentId.Items.Add(new ListItem("请选择", ""));
            foreach (DataRow dr in dt.Rows)
            {
                string Id = dr["de_id"].ToString();
                int ClassLayer = int.Parse(dr["class_layer"].ToString());
                string Title = dr["de_name"].ToString().Trim();

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

        #region 角色类型=================================
        private void RoleBind(DropDownList ddl, int role_type)
        {
            BLL.manager_role bll = new BLL.manager_role();
            DataTable dt = bll.GetList("").Tables[0];

            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("请选择角色...", ""));
            foreach (DataRow dr in dt.Rows)
            {
                if (Convert.ToInt32(dr["role_type"]) >= role_type)
                {
                    ddl.Items.Add(new ListItem(dr["role_name"].ToString(), dr["id"].ToString()));
                }
            }
        }
        #endregion

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            TreeBind();
            BLL.manager bll = new BLL.manager();
            Model.manager model = bll.GetModel(_id);
            ddlRoleId.SelectedValue = model.role_id.ToString();
            if (model.is_lock == 0)
            {
                cbIsLock.Checked = true;
            }
            else
            {
                cbIsLock.Checked = false;
            }
            if (model.is_audit == 0)
            {
                cbIsAudit.Checked = false;
            }
            else
            {
                cbIsAudit.Checked = true;
            }
            txtUserName.Text = model.user_name;
            txtUserName.ReadOnly = true;
            txtUserName.Attributes.Remove("ajaxurl");
            if (!string.IsNullOrEmpty(model.password))
            {
                txtPassword.Attributes["value"] = txtPassword1.Attributes["value"] = defaultpassword;
            }
            txtAvatar.Text = model.avatar;
            txtRealName.Text = model.real_name;
            txtTelephone.Text = model.telephone;
            txtEmail.Text = model.email;
            labdepartStr.Text = model.departTree;
            ddlParentId.SelectedValue = model.departID.ToString();
            hIDTree.Value = model.departTreeID;
            txtDetailDepart.Text = model.detaildepart;
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd()
        {
            Model.manager model = new Model.manager();
            BLL.manager bll = new BLL.manager();
            model.role_id = int.Parse(ddlRoleId.SelectedValue);
            model.role_type = new BLL.manager_role().GetModel(model.role_id).role_type;
            if (cbIsLock.Checked == true)
            {
                model.is_lock = 0;
            }
            else
            {
                model.is_lock = 1;
            }
            model.user_name = txtUserName.Text.Trim();
            //获得6位的salt加密字符串
            model.salt = Utils.GetCheckCode(6);
            //以随机生成的6位字符串做为密钥加密
            model.password = DESEncrypt.Encrypt(txtPassword.Text.Trim(), model.salt);
            model.avatar = txtAvatar.Text.Trim();
            model.real_name = txtRealName.Text.Trim();
            model.telephone = txtTelephone.Text.Trim();
            model.email = txtEmail.Text.Trim();
            model.add_time = DateTime.Now;
            model.departID = Convert.ToInt32(ddlParentId.SelectedValue);
            model.departTree = labdepartStr.Text;
            model.departTreeID = hIDTree.Value;
            model.detaildepart = txtDetailDepart.Text.Trim();
            manager = GetAdminInfo();
            return bll.Add(model, manager);
        }
        #endregion

        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.manager bll = new BLL.manager();
            Model.manager model = bll.GetModel(_id);
            bool updateName = false, updateContact = false;
            StringBuilder sb = new StringBuilder();
            if (model.role_id != int.Parse(ddlRoleId.SelectedValue))
            {
                sb.Append("用户角色ID：" + model.role_id + "→<font color='red'>" + ddlRoleId.SelectedValue + "</font><br/>");
            }
            model.role_id = int.Parse(ddlRoleId.SelectedValue);
            model.role_type = new BLL.manager_role().GetModel(model.role_id).role_type;
            if (cbIsLock.Checked == true)
            {
                model.is_lock = 0;
            }
            else
            {
                model.is_lock = 1;
            }
            //if (cbIsAudit.Checked == true)
            //{
            //    model.is_audit = 1;
            //}
            //else
            //{
            //    model.is_audit = 0;
            //}
            //判断密码是否更改
            if (txtPassword.Text.Trim() != defaultpassword)
            {
                //获取用户已生成的salt作为密钥加密
                model.password = DESEncrypt.Encrypt(txtPassword.Text.Trim(), model.salt);
            }
            model.avatar = txtAvatar.Text.Trim();
            if (model.real_name != txtRealName.Text.Trim())
            {
                updateName = true;
                sb.Append("姓名：" + model.real_name + "→<font color='red'>" + txtRealName.Text.Trim() + "</font><br/>");
            }
            model.real_name = txtRealName.Text.Trim();
            if (model.telephone != txtTelephone.Text.Trim())
            {
                updateContact = true;
                sb.Append("电话：" + model.telephone + "→<font color='red'>" + txtTelephone.Text.Trim() + "</font><br/>");
            }
            model.telephone = txtTelephone.Text.Trim();
            if (model.email != txtEmail.Text.Trim())
            {
                sb.Append("邮箱：" + model.email + "→<font color='red'>" + txtEmail.Text.Trim() + "</font><br/>");
            }
            model.email = txtEmail.Text.Trim();
            if (model.departID != Convert.ToInt32(ddlParentId.SelectedValue))
            {
                sb.Append("岗位：" + model.departTree + "→<font color='red'>" + labdepartStr.Text + "</font><br/>");
            }
            model.departID = Convert.ToInt32(ddlParentId.SelectedValue);
            model.departTree = hTextTree.Value;
            model.departTreeID = hIDTree.Value;
            if (model.detaildepart != txtDetailDepart.Text.Trim())
            {
                sb.Append("具体岗位：" + model.detaildepart + "→<font color='red'>" + txtDetailDepart.Text.Trim() + "</font><br/>");
            }
            model.detaildepart = txtDetailDepart.Text.Trim();
            manager = GetAdminInfo();
            return bll.Update(model, sb.ToString(), manager, true, updateName, updateContact);
        }
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                ChkAdminLevel("manager_list", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (!string.IsNullOrEmpty(result))
                {
                    JscriptMsg("保存过程中发生错误！", "");
                    return;
                }
                JscriptMsg("修改管理员信息成功！", "manager_list.aspx");
            }
            else //添加
            {
                ChkAdminLevel("manager_list", DTEnums.ActionEnum.Add.ToString()); //检查权限
                result = DoAdd();
                if (!string.IsNullOrEmpty(result))
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("添加管理员信息成功！", "manager_list.aspx");
            }
        }
        protected void ddlParentId_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            if (!string.IsNullOrEmpty(ddl.SelectedValue))
            {
                string textTree = "", idTree = "";
                new BLL.department().getDepartText(Convert.ToInt32(ddl.SelectedValue), out textTree, out idTree, out string area);
                labdepartStr.Text = textTree;
                hIDTree.Value = idTree;
            }
        }

    }
}