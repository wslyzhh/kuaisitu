using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.baseData
{
    public partial class businessDetails_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        private int id = 0;

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
                if (!new BLL.businessDetails().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                initData();
                ChkAdminLevel("pub_details", DTEnums.ActionEnum.View.ToString()); //检查权限
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
            }
        }

        private void initData()
        {
            ddlnature.DataSource = new BLL.businessNature().GetList(0, "na_isUse=1", "na_sort asc,na_id desc");
            ddlnature.DataTextField = "na_name";
            ddlnature.DataValueField = "na_id";
            ddlnature.DataBind();
            ddlnature.Items.Insert(0, new ListItem("请选择", ""));
        }

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.businessDetails bll = new BLL.businessDetails();
            Model.businessDetails model = bll.GetModel(_id);

            ddlnature.SelectedValue = model.de_nid.ToString();
            txtTitle.Text = model.de_name;
            txtSortId.Text = model.de_sort.ToString();
            if (model.de_isUse.Value)
            {
                cbIsUse.Checked = true;
            }
            else
            {
                cbIsUse.Checked = false;
            }
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd()
        {
            Model.businessDetails model = new Model.businessDetails();
            BLL.businessDetails bll = new BLL.businessDetails();
            if (string.IsNullOrEmpty(ddlnature.SelectedValue))
            {
                return "请选择业务性质";
            }
            model.de_nid = Convert.ToInt32(ddlnature.SelectedValue);
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
            {
                return "请填写业务明细";
            }
            if (bll.Exists(txtTitle.Text.Trim(), model.de_nid))
            {
                return "该业务明细已存在";
            }
            model.de_name = txtTitle.Text.Trim();
            model.de_sort = Utils.StrToInt(txtSortId.Text.Trim(), 0);
            model.de_isUse = cbIsUse.Checked;
            int _naid = bll.Add(model);
            if (_naid > 0)
            {
                //更新一下域名缓存
                //CacheHelper.Remove(DTKeys.CACHE_SITE_HTTP_DOMAIN);
                //Undo
                logmodel = new Model.business_log();
                logmodel.ol_relateID = _naid;
                logmodel.ol_title = "添加业务明细";
                logmodel.ol_content = model.de_name;
                logmodel.ol_operateDate = DateTime.Now;
                AddBusinessLog(DTEnums.ActionEnum.Add.ToString(), logmodel); //记录日志
                return "";
            }

            return "添加失败";
        }
        #endregion

        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.businessDetails bll = new BLL.businessDetails();
            Model.businessDetails model = bll.GetModel(_id);
            if (string.IsNullOrEmpty(ddlnature.SelectedValue))
            {
                return "请选择业务性质";
            }
            model.de_nid = Convert.ToInt32(ddlnature.SelectedValue);
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
            {
                return "请填写业务明细";
            }
            if (bll.Exists(txtTitle.Text.Trim(),_id))
            {
                return "该业务明细已存在";
            }
            string _content = string.Empty;
            if (model.de_name != txtTitle.Text.Trim())
            {
                _content+= "业务明细:" + model.de_name+ "→<font color='red'>" + txtTitle.Text.Trim()+ "</font><br/>";
            }
            model.de_name = txtTitle.Text.Trim();
            model.de_sort = Utils.StrToInt(txtSortId.Text.Trim(), 0);
            model.de_isUse = cbIsUse.Checked;
            if (bll.Update(model))
            {
                //CacheHelper.Remove(DTKeys.CACHE_SITE_HTTP_DOMAIN); //更新一下域名缓存
                //Undo
                if (!string.IsNullOrEmpty(_content))
                {
                    logmodel = new Model.business_log();
                    logmodel.ol_relateID = _id;
                    logmodel.ol_title = "修改业务明细";
                    logmodel.ol_content = _content;
                    logmodel.ol_operateDate = DateTime.Now;
                    AddBusinessLog(DTEnums.ActionEnum.Edit.ToString(), logmodel); //记录日志
                }
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
                ChkAdminLevel("pub_details", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("修改业务明细成功！", "businessDetails_list.aspx");
            }
            else //添加
            {
                ChkAdminLevel("pub_details", DTEnums.ActionEnum.Add.ToString()); //检查权限
                result = DoAdd();
                if (result!="")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("添加业务明细成功！", "businessDetails_list.aspx");
            }
        }
    }
}