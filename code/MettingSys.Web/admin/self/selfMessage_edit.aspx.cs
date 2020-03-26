using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.self
{
    public partial class selfMessage_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        protected int id = 0;

        protected Model.business_log logmodel = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            string _action = DTRequest.GetQueryString("action");

            if (!string.IsNullOrEmpty(_action) && _action == DTEnums.ActionEnum.Edit.ToString())
            {
                this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                this.id = DTRequest.GetQueryInt("id");
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("sys_myself_message", DTEnums.ActionEnum.View.ToString()); //检查权限
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
            }
        }

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.selfMessage bll = new BLL.selfMessage();
            Model.selfMessage model = bll.GetModel(_id);

            labTitle.Text = model.me_title;
            labContent.Text = model.me_content;
            if (model.me_isRead.Value)
            {
                cbIsRead.Checked = true;
            }
            else
            {
                cbIsRead.Checked = false;
            }
        }
        #endregion
        
        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.selfMessage bll = new BLL.selfMessage();
            Model.selfMessage model = bll.GetModel(_id);            
            model.me_isRead = cbIsRead.Checked;
            if (bll.Update(model))
            {                
                return "";
            }

            return "修改失败";
        }
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                ChkAdminLevel("pub_nature", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("操作成功！", "selfMessage.aspx", "");
            }
        }
    }
}