using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.baseData
{
    public partial class invUnit_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        private int id = 0;

        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            string _action = DTRequest.GetQueryString("action");

            if (!string.IsNullOrEmpty(_action))
            {
                if (_action == DTEnums.ActionEnum.Edit.ToString())
                {
                    this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                    this.id = DTRequest.GetQueryInt("id");
                    if (this.id == 0)
                    {
                        JscriptMsg("传输参数不正确！", "back");
                        return;
                    }
                    if (!new BLL.payMethod().Exists(this.id))
                    {
                        JscriptMsg("记录不存在或已被删除！", "back");
                        return;
                    }
                }
            }
            manager = GetAdminInfo();
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("pub_invUnit", DTEnums.ActionEnum.View.ToString()); //检查权限
                InitData();
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
            }
        }

        #region 初始化
        private void InitData()
        {
            ddlarea.DataSource = new BLL.department().getAreaDict();
            ddlarea.DataTextField = "value";
            ddlarea.DataValueField = "key";
            ddlarea.DataBind();
            ddlarea.Items.Insert(0, new ListItem("请选择", ""));
        }
        #endregion

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.invoiceUnit bll = new BLL.invoiceUnit();
            Model.invoiceUnit model = bll.GetModel(_id);

            ddlarea.SelectedValue = model.invU_area;
            txtUnit.Text = model.invU_name;
            txtContact.Text = model.invU_contact;
            txtPhone.Text = model.invU_contactPhone;
            cbIsUse.Checked = model.invU_flag.Value;
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd()
        {
            Model.invoiceUnit model = new Model.invoiceUnit();
            BLL.invoiceUnit bll = new BLL.invoiceUnit();
            model.invU_area = ddlarea.SelectedValue;
            model.invU_name = txtUnit.Text.Trim();
            model.invU_contact = txtContact.Text.Trim();
            model.invU_contactPhone = txtPhone.Text.Trim();
            model.invU_flag = cbIsUse.Checked;
            return bll.Add(model, manager);
        }
        #endregion
        
        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.invoiceUnit bll = new BLL.invoiceUnit();
            Model.invoiceUnit model = bll.GetModel(_id);
            string _content = string.Empty;
            if (model.invU_area != ddlarea.SelectedValue)
            {
                _content += "所属区域:" + model.invU_area + "→<font color='red'>" + ddlarea.SelectedValue + "</font><br/>";
            }
            model.invU_area = ddlarea.SelectedValue;
            if (model.invU_name != txtUnit.Text.Trim())
            {
                _content += "开票单位:" + model.invU_name + "→<font color='red'>" + txtUnit.Text.Trim() + "</font><br/>";
            }
            model.invU_name = txtUnit.Text.Trim();
            if (model.invU_contact != txtContact.Text.Trim())
            {
                _content += "联系人:" + model.invU_contact + "→<font color='red'>" + txtContact.Text.Trim() + "</font><br/>";
            }
            model.invU_contact = txtContact.Text.Trim();
            if (model.invU_contactPhone != txtPhone.Text.Trim())
            {
                _content += "联系电话:" + model.invU_contactPhone + "→<font color='red'>" + txtPhone.Text.Trim() + "</font><br/>";
            }
            model.invU_contactPhone = txtPhone.Text.Trim();
            if (model.invU_flag != cbIsUse.Checked)
            {
                _content += "启用状态:" + (model.invU_flag.Value ? "启用" : "禁用") + "→<font color='red'>" + (cbIsUse.Checked ? "启用" : "禁用") + "</font><br/>";
            }
            return bll.Update(model, _content, manager);
        }
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                ChkAdminLevel("pub_invUnit", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("修改收付款方式成功！", "invUnit_list.aspx");
            }
            else //添加
            {
                ChkAdminLevel("pub_invUnit", DTEnums.ActionEnum.Add.ToString()); //检查权限
                result = DoAdd();
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("添加收付款方式成功！", "invUnit_list.aspx");
            }
        }
    }
}