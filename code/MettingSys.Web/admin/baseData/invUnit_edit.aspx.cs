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
                if (!new BLL.payMethod().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("pub_paymethod", DTEnums.ActionEnum.View.ToString()); //检查权限
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
            }
        }

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.payMethod bll = new BLL.payMethod();
            Model.payMethod model = bll.GetModel(_id);

            txtTitle.Text = model.pm_name;
            txtSortId.Text = model.pm_sort.ToString();
            cbIsUse.Checked = model.pm_isUse.Value;
            cbIsType.Checked = model.pm_type.Value;
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd()
        {
            Model.payMethod model = new Model.payMethod();
            BLL.payMethod bll = new BLL.payMethod();
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
            {
                return "请填写收付款方式";
            }
            if (bll.Exists(txtTitle.Text.Trim()))
            {
                return "该收付款方式已存在";
            }
            model.pm_name = txtTitle.Text.Trim();
            model.pm_isUse = cbIsUse.Checked;
            model.pm_sort = Utils.StrToInt(txtSortId.Text.Trim(), 0);
            model.pm_type = cbIsType.Checked;
            int _naid = bll.Add(model);
            if (_naid > 0)
            {
                //更新一下域名缓存
                //CacheHelper.Remove(DTKeys.CACHE_SITE_HTTP_DOMAIN);
                //Undo
                logmodel = new Model.business_log();
                logmodel.ol_relateID = _naid;
                logmodel.ol_title = "添加收付款方式";
                logmodel.ol_content = model.pm_name + "，状态：" + (cbIsUse.Checked ? "启用" : "禁用");
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
            BLL.payMethod bll = new BLL.payMethod();
            Model.payMethod model = bll.GetModel(_id);
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
            {
                return "请填写收付款方式";
            }
            if (bll.Exists(txtTitle.Text.Trim(), _id))
            {
                return "该收付款方式已存在";
            }
            string _content = string.Empty;
            if (model.pm_name != txtTitle.Text.Trim())
            {
                _content += "收付款方式:" + model.pm_name + "→<font color='red'>" + txtTitle.Text.Trim() + "</font><br/>";
            }
            model.pm_name = txtTitle.Text.Trim();
            if (model.pm_isUse != cbIsUse.Checked)
            {
                _content += "启用状态:" + (model.pm_isUse.Value ? "启用" : "禁用") + "→<font color='red'>" + (cbIsUse.Checked ? "启用" : "禁用") + "</font><br/>";
            }
            model.pm_isUse = cbIsUse.Checked;
            if (model.pm_type != cbIsType.Checked)
            {
                _content += "仅限财务使用:" + (model.pm_type.Value ? "是" : "否") + "→<font color='red'>" + (cbIsType.Checked ? "是" : "否") + "</font><br/>";
            }
            model.pm_type = cbIsType.Checked;
            model.pm_sort = Utils.StrToInt(txtSortId.Text.Trim(), 0);
            if (bll.Update(model))
            {
                //CacheHelper.Remove(DTKeys.CACHE_SITE_HTTP_DOMAIN); //更新一下域名缓存
                //Undo
                if (!string.IsNullOrEmpty(_content))
                {
                    logmodel = new Model.business_log();
                    logmodel.ol_relateID = _id;
                    logmodel.ol_title = "修改收付款方式";
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
                ChkAdminLevel("pub_paymethod", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("修改收付款方式成功！", "payMethod_list.aspx");
            }
            else //添加
            {
                ChkAdminLevel("pub_paymethod", DTEnums.ActionEnum.Add.ToString()); //检查权限
                result = DoAdd();
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("添加收付款方式成功！", "payMethod_list.aspx");
            }
        }
    }
}