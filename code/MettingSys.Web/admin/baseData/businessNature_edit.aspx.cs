using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.baseData
{
    public partial class businessNature_edit : Web.UI.ManagePage
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
                if (!new BLL.businessNature().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("pub_nature", DTEnums.ActionEnum.View.ToString()); //检查权限
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
            }
        }

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.businessNature bll = new BLL.businessNature();
            Model.businessNature model = bll.GetModel(_id);

            txtTitle.Text = model.na_name;
            txtSortId.Text = model.na_sort.ToString();
            cbIsUse.Checked = model.na_isUse.Value;
            cbIsFinance.Checked = model.na_flag.Value;
            cbType.Checked = model.na_type.Value;
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd()
        {
            Model.businessNature model = new Model.businessNature();
            BLL.businessNature bll = new BLL.businessNature();
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
            {
                return "请填写业务性质";
            }
            if (bll.Exists(txtTitle.Text.Trim()))
            {
                return "该业务性质名称已存在";
            }
            model.na_name = txtTitle.Text.Trim();
            model.na_sort = Utils.StrToInt(txtSortId.Text.Trim(), 0);
            model.na_isUse = cbIsUse.Checked;
            model.na_flag = cbIsFinance.Checked;
            model.na_type = cbType.Checked;
            int _naid = bll.Add(model);
            if (_naid > 0)
            {
                //更新一下域名缓存
                //CacheHelper.Remove(DTKeys.CACHE_SITE_HTTP_DOMAIN);
                //Undo
                logmodel = new Model.business_log();
                logmodel.ol_relateID = _naid;
                logmodel.ol_title = "添加业务性质";
                logmodel.ol_content =model.na_name;
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
            BLL.businessNature bll = new BLL.businessNature();
            Model.businessNature model = bll.GetModel(_id);
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
            {
                return "请填写业务性质";
            }
            if (bll.Exists(txtTitle.Text.Trim(),_id))
            {
                return "该业务性质已存在";
            }
            string _content = string.Empty;
            if (model.na_name != txtTitle.Text.Trim())
            {
                _content += "业务性质:" + model.na_name + "→<font color='red'>" + txtTitle.Text.Trim() + "</font><br/>";
            }
            model.na_name = txtTitle.Text.Trim();
            model.na_sort = Utils.StrToInt(txtSortId.Text.Trim(), 0);
            model.na_isUse = cbIsUse.Checked;
            model.na_flag = cbIsFinance.Checked;
            model.na_type = cbType.Checked;
            if (bll.Update(model))
            {
                //CacheHelper.Remove(DTKeys.CACHE_SITE_HTTP_DOMAIN); //更新一下域名缓存
                //Undo
                if (!string.IsNullOrEmpty(_content))
                {
                    logmodel = new Model.business_log();
                    logmodel.ol_relateID = _id;
                    logmodel.ol_title = "修改业务性质";
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
                ChkAdminLevel("pub_nature", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("修改业务性质成功！", "businessNature_list.aspx");
            }
            else //添加
            {
                ChkAdminLevel("pub_nature", DTEnums.ActionEnum.Add.ToString()); //检查权限
                result = DoAdd();
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("添加业务性质成功！", "businessNature_list.aspx");
            }
        }
    }
}