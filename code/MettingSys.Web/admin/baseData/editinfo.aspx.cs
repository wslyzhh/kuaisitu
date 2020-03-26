using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.baseData
{
    public partial class editinfo : Web.UI.ManagePage
    {
        protected Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Model.manager model = GetAdminInfo();
                ShowInfo(model.id);
            }
        }

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.manager bll = new BLL.manager();
            Model.manager model = bll.GetModel(_id);
            txtAvatar.Text = model.avatar;
            txtUserName.Text = model.user_name;
            txtRealName.Text = model.real_name;
            txtTelephone.Text = model.telephone;
            txtEmail.Text = model.email;
        }
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            BLL.manager bll = new BLL.manager();
            Model.manager model = GetAdminInfo();
            bool updateName = false, updateContact = false;
            StringBuilder sb = new StringBuilder();
            if (DESEncrypt.Encrypt(txtOldPassword.Text.Trim(), model.salt) != model.password)
            {
                JscriptMsg("旧密码不正确！", "");
                return;
            }
            if (!string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                if (txtPassword.Text.Trim().Length < 6 || txtPassword.Text.Trim().Length > 20)
                {
                    JscriptMsg("密码长度须为6-20", "");
                    return;
                }
                if (txtPassword.Text.Trim() != txtPassword1.Text.Trim())
                {
                    JscriptMsg("两次密码不一致！", "");
                    return;
                }
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
            manager = GetAdminInfo();
            string result = bll.Update(model, sb.ToString(), manager,false, updateName,updateContact);
            if (!string.IsNullOrEmpty(result))
            {
                JscriptMsg(result, "");
                return;
            }
            Session[DTKeys.SESSION_ADMIN_INFO] = null;
            JscriptMsg("密码修改成功！", "editinfo.aspx");
        }
    }
}