﻿using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.customer
{
    public partial class bank_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        protected int id = 0, cid = 0;
        protected DataRow dr = null;
        protected string fromPay = "";//true时表示从付款通知页面添加


        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            string _action = DTRequest.GetQueryString("action");
            cid= DTRequest.GetQueryInt("cid");
            fromPay = DTRequest.GetQueryString("fromPay");
            if (!string.IsNullOrEmpty(_action) && _action == DTEnums.ActionEnum.Edit.ToString())
            {
                this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                this.id = DTRequest.GetQueryInt("id");
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.customerBank().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
            }
        }

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.customerBank bll = new BLL.customerBank();
            DataSet ds = bll.GetList(0, "cb_id=" + _id, "");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                dr = ds.Tables[0].Rows[0];
                //txtCusName.Text = dr["c_name"].ToString();
                //hCusId.Value = dr["cb_cid"].ToString();
                txtBankName.Text = dr["cb_bankName"].ToString();
                txtBankNum.Text = dr["cb_bankNum"].ToString();
                txtBank.Text = dr["cb_bank"].ToString();
                txtBankAddress.Text = dr["cb_bankAddress"].ToString();
                if (Convert.ToBoolean(dr["cb_flag"]))
                {
                    cbIsUse.Checked = true;
                }
                else
                {
                    cbIsUse.Checked = false;
                }
            }
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd()
        {
            Model.customerBank model = new Model.customerBank();
            BLL.customerBank bll = new BLL.customerBank();
            model.cb_cid = cid;
            model.cb_bankName = txtBankName.Text.Trim();
            model.cb_bankNum = txtBankNum.Text.Trim();
            model.cb_bank = txtBank.Text.Trim();
            model.cb_bankAddress = txtBankAddress.Text.Trim();
            model.cb_flag = cbIsUse.Checked;
            return bll.Add(model, manager);
        }
        #endregion

        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.customerBank bll = new BLL.customerBank();
            Model.customerBank model = bll.GetModel(_id);
            string _content = string.Empty;
            
            if (model.cb_bankName != txtBankName.Text.Trim())
            {
                _content += "银行账户名称:" + model.cb_bankName + "→<font color='red'>" + txtBankName.Text.Trim() + "</font><br/>";
            }
            model.cb_bankName = txtBankName.Text.Trim();
            if (model.cb_bankNum != txtBankNum.Text.Trim())
            {
                _content += "客户银行账号:" + model.cb_bankNum + "→<font color='red'>" + txtBankNum.Text.Trim() + "</font><br/>";
            }
            model.cb_bankNum = txtBankNum.Text.Trim();
            if (model.cb_bank != txtBank.Text.Trim())
            {
                _content += "开户行:" + model.cb_bank + "→<font color='red'>" + txtBank.Text.Trim() + "</font><br/>";
            }
            model.cb_bank = txtBank.Text.Trim();
            if (model.cb_bankAddress != txtBankAddress.Text.Trim())
            {
                _content += "开户地址:" + model.cb_bankAddress + "→<font color='red'>" + txtBankAddress.Text.Trim() + "</font><br/>";
            }
            model.cb_bankAddress = txtBankAddress.Text.Trim();
            if (model.cb_flag != cbIsUse.Checked)
            {
                _content += "状态:" + (model.cb_flag.Value ? "启用" : "禁用") + "→<font color='red'>" + (cbIsUse.Checked ? "启用" : "禁用") + "</font><br/>";
            }
            model.cb_flag = cbIsUse.Checked;
            return bll.Update(model, _content, manager);
        }
        #endregion

        //保存
        //protected void btnSubmit_Click(object sender, EventArgs e)
        //{
        //    string result = "", msbox = "";
        //    manager = GetAdminInfo();
        //    if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
        //    {
        //        //ChkAdminLevel("sys_customerBank", DTEnums.ActionEnum.Edit.ToString()); //检查权限
        //        result = DoEdit(this.id);
        //        if (result != "")
        //        {
        //            msbox = "parent.parent.jsdialog(\"提示\",\"" + result + "\", \"\");";
        //            ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsDialog", msbox, true);                    
        //        }
        //        msbox = "parent.parent.jsprint(\"修改客户银行账号成功！\", \"customer_edit.aspx?action=Edit&id=" + this.cid + "\");";
        //        ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
                
        //    }
        //    else //添加
        //    {
        //        //ChkAdminLevel("sys_customerBank", DTEnums.ActionEnum.Add.ToString()); //检查权限
        //        result = DoAdd();
        //        if (result != "")
        //        {
        //            if (_tag == 1)
        //            {
        //                PrintMsg(result);
        //            }
        //            else
        //            {
        //                msbox = "parent.parent.jsdialog(\"提示\",\"" + result + "\", \"\");";
        //                ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsDialog", msbox, true);
        //                return;
        //            }
        //        }
        //        if (_tag == 1)
        //        {
        //            PrintMsg("添加银行账号成功！");
        //        }
        //        else
        //        {
        //            msbox = "parent.parent.jsprint(\"添加客户银行账号成功！\", \"customer_edit.aspx?action=Edit&id=" + this.cid + "\");";
        //            ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
        //        }
        //    }
        //}
    }
}