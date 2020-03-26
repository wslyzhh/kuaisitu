using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.finance
{
    public partial class certificate_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        protected int id = 0;

        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;

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
                if (!new BLL.certificates().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("sys_cetificate", DTEnums.ActionEnum.View.ToString()); //检查权限
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
            }
        }

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.certificates bll = new BLL.certificates();
            Model.certificates model = bll.GetModel(_id);

            txtNum.Text = model.ce_num;
            txtDate.Text = model.ce_date.Value.ToString("yyyy-MM-dd");
            txtRemark.Text = model.ce_remark;
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd()
        {
            Model.certificates model = new Model.certificates();
            BLL.certificates bll = new BLL.certificates();
            manager = GetAdminInfo();

            model.ce_num = txtNum.Text.Trim();
            model.ce_date = ConvertHelper.toDate(txtDate.Text.Trim());
            model.ce_remark = txtRemark.Text.Trim();
            model.ce_personNum = manager.user_name;
            model.ce_personName = manager.real_name;
            int _naid = 0;
            string result = bll.Add(model,out _naid);
            if (_naid > 0)
            {                               
                return "";
            }

            return "添加失败";
        }
        #endregion

        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.certificates bll = new BLL.certificates();
            Model.certificates model = bll.GetModel(_id);
            if (model.ce_flag == 2)
            {
                return "凭证号已经审批通过，不能编辑";
            }
            if (string.IsNullOrEmpty(txtNum.Text.Trim()))
            {
                return "请填写凭证号";
            }
            if (string.IsNullOrEmpty(txtDate.Text.Trim()))
            {
                return "请填写凭证日期";
            }
            string _content = string.Empty;
            bool _update = false;
            if (model.ce_num != txtNum.Text.Trim())
            {
                _update = true;
                _content += "凭证号:" + model.ce_num + "→<font color='red'>" + txtNum.Text.Trim() + "</font><br/>";
            }
            model.ce_num = txtNum.Text.Trim();
            if (model.ce_date != Convert.ToDateTime(txtDate.Text.Trim()))
            {
                _update = true;
                _content += "凭证日期:" + model.ce_date.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + txtNum.Text.Trim() + "</font><br/>";
            }
            model.ce_date = Convert.ToDateTime(txtDate.Text.Trim());
            if (_update)
            {
                //判断是否被使用
                DataSet ds = new BLL.ReceiptPay().GetList(0, "rp_ceid=" + id, "");
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return "该凭证已被使用，不能修改凭证号和凭证日期，只能修改备注";
                }
            }

            if (model.ce_remark != txtRemark.Text.Trim())
            {
                _content += "备注:" + model.ce_remark + "→<font color='red'>" + txtRemark.Text.Trim() + "</font><br/>";
            }
            model.ce_remark = txtRemark.Text.Trim();
            if (bll.Exists(model.ce_num, model.ce_date,_id))
            {
                return "凭证号、凭证日期重复";
            }
            if (bll.Update(model))
            {
                if (!string.IsNullOrEmpty(_content))
                {
                    logmodel = new Model.business_log();
                    logmodel.ol_relateID = _id;
                    logmodel.ol_title = "修改凭证";
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
                ChkAdminLevel("sys_cetificate", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("修改凭证成功！", "certificate_list.aspx");
            }
            else //添加
            {
                ChkAdminLevel("sys_cetificate", DTEnums.ActionEnum.Add.ToString()); //检查权限
                result = DoAdd();
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("添加凭证成功！", "certificate_list.aspx");
            }
        }
    }
}