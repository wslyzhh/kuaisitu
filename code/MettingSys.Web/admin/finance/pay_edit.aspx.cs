using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.finance
{
    public partial class pay_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        protected int id = 0;

        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            action = DTRequest.GetQueryString("action");
            manager = GetAdminInfo();
            if (!string.IsNullOrEmpty(action) && action == DTEnums.ActionEnum.Edit.ToString())
            {
                this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                this.id = DTRequest.GetQueryInt("id");
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.ReceiptPay().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            //查看
            if (action == DTEnums.ActionEnum.View.ToString())
            {
                this.id = DTRequest.GetQueryInt("id");
                btnSubmit.Visible = false;
                btnSubmitToDistribute.Visible = false;
            }
            if (!Page.IsPostBack)
            {
                dlceNum.Visible = false;
                dlceDate.Visible = false;
                InitData();
                ChkAdminLevel("sys_payment_list0", DTEnums.ActionEnum.View.ToString()); //检查权限
                if (action == DTEnums.ActionEnum.Edit.ToString() || action == DTEnums.ActionEnum.View.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
            }
        }
        #region 初始化数据=================================
        private void InitData()
        {
            //收付款方式
            //判断是否是财务来显示不同的收付款方式
            string sqlwhere = "";
            if (!new BLL.permission().checkHasPermission(manager, "0401"))
            {
                sqlwhere = " and pm_type=0";
            }
            ddlmethod.DataSource = new BLL.payMethod().GetList(0, "pm_isUse=1 "+sqlwhere+"", "pm_sort asc,pm_id asc");
            ddlmethod.DataTextField = "pm_name";
            ddlmethod.DataValueField = "pm_id";
            ddlmethod.DataBind();
            ddlmethod.Items.Insert(0, new ListItem("请选择", ""));
        }
        #endregion
        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            btnAudit.Visible = false;
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            DataSet ds = bll.GetList(0, "rp_id=" + _id + "", "");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                txtCusName.Text = dr["c_name"].ToString();
                hCusId.Value = dr["rp_cid"].ToString();
                if (Utils.StrToInt(hCusId.Value, 0) > 0)
                {
                    //绑定客户银行账号
                    ddlbank.DataSource = new BLL.customerBank().GetList(Utils.StrToInt(hCusId.Value, 0));
                    ddlbank.DataTextField = "cbname";
                    ddlbank.DataValueField = "cb_id";
                    ddlbank.DataBind();
                    ddlbank.Items.Insert(0, new ListItem("请选择", ""));
                    ddlbank.SelectedValue = dr["rp_cbid"].ToString();
                }
                txtMoney.Text = dr["rp_money"].ToString();
                if (dr["rp_foredate"] != null)
                {
                    txtforedate.Text = Convert.ToDateTime(dr["rp_foredate"]).ToString("yyyy-MM-dd");
                }
                ddlmethod.SelectedValue = dr["rp_method"].ToString();
                txtContent.Text = dr["rp_content"].ToString();
                if (Utils.ObjToInt(dr["rp_method"],0)> 0 && new BLL.payMethod().GetModel(Utils.ObjToInt(dr["rp_method"], 0)).pm_type.Value)
                {
                    dlceDate.Visible = true;
                    dlceNum.Visible = true;

                    txtCenum.Text = Utils.ObjectToStr(dr["ce_num"]);
                    txtCedate.Text = ConvertHelper.toDate(dr["ce_date"]).Value.ToString("yyyy-MM-dd");
                }

                if (new MettingSys.BLL.permission().checkHasPermission(manager, "0402,0601"))
                {
                    btnAudit.Visible = true;
                    ddlflag.DataSource = Common.BusinessDict.checkStatus();
                    ddlflag.DataTextField = "value";
                    ddlflag.DataValueField = "key";
                    ddlflag.DataBind();
                    ddlflag.Items.Insert(0, new ListItem("请选择", ""));

                    if (new BLL.permission().checkHasPermission(manager, "0402"))//财务审批
                    {
                        ddlchecktype.SelectedValue = "1";
                        ddlflag.SelectedValue = dr["rp_flag"].ToString();
                        txtCheckRemark.Text = dr["rp_checkRemark"].ToString();
                    }
                    else if (new BLL.permission().checkHasPermission(manager, "0601"))//总经理审批
                    {
                        ddlchecktype.SelectedValue = "2";
                        ddlflag.SelectedValue = dr["rp_flag1"].ToString();
                        txtCheckRemark.Text = dr["rp_checkRemark1"].ToString();
                    }
                }
            }
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd(out int rpid)
        {
            rpid = 0;
            Model.ReceiptPay model = new Model.ReceiptPay();
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            manager = GetAdminInfo();
            model.rp_type = false;
            model.rp_cid = Utils.StrToInt(hCusId.Value, 0);
            model.rp_money = Utils.StrToDecimal(txtMoney.Text.Trim(), 0);
            model.rp_foredate = ConvertHelper.toDate(txtforedate.Text.Trim());
            model.rp_method = Utils.StrToInt(ddlmethod.SelectedValue, 0);
            model.rp_content = txtContent.Text.Trim();            
            return bll.Add(model, manager, txtCenum.Text.Trim(), txtCedate.Text.Trim(),out rpid);
        }
        #endregion

        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            Model.ReceiptPay model = bll.GetModel(_id);
            manager = GetAdminInfo();

            string _content = string.Empty;
            if (model.rp_cid.ToString() != hCusId.Value)
            {
                _content += "付款对象ID：" + model.rp_cid + "→<font color='red'>" + hCusId.Value + "</font><br/>";
            }
            model.rp_cid = Utils.StrToInt(hCusId.Value, 0);
            bool updateMoney = false;
            if (model.rp_money.ToString() != txtMoney.Text.Trim())
            {
                if ((model.rp_money < 0 && Utils.ObjToDecimal(txtMoney.Text.Trim(), 0) >= 0) || (model.rp_money >= 0 && Utils.ObjToDecimal(txtMoney.Text.Trim(), 0) < 0))
                {
                    updateMoney = true;//表示金额从负数改为正数，或从正数改为负数
                }
                _content += "付款金额：" + model.rp_money + "→<font color='red'>" + txtMoney.Text.Trim() + "</font><br/>";
            }
            model.rp_money = Utils.StrToDecimal(txtMoney.Text.Trim(), 0);
            if (model.rp_foredate.Value.ToString("yyyy-MM-dd") != txtforedate.Text.Trim())
            {
                _content += "预付日期：" + model.rp_foredate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + txtforedate.Text.Trim() + "</font><br/>";
            }
            model.rp_foredate = ConvertHelper.toDate(txtforedate.Text.Trim());
            if (model.rp_method.ToString() != ddlmethod.SelectedValue)
            {
                _content += "付款方式ID：" + model.rp_method + "→<font color='red'>" + ddlmethod.SelectedValue + "</font><br/>";
            }
            model.rp_method = Utils.StrToInt(ddlmethod.SelectedValue, 0);
            if (model.rp_content != txtContent.Text.Trim())
            {
                _content += "付款内容：" + model.rp_content + "→<font color='red'>" + txtContent.Text.Trim() + "</font><br/>";
            }
            model.rp_content = txtContent.Text.Trim();
            return bll.Update(model, _content, manager, txtCenum.Text.Trim(), txtCedate.Text.Trim(), updateMoney);
        }
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                ChkAdminLevel("sys_payment_list0", DTEnums.ActionEnum.View.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("修改付款通知成功！", "pay_list.aspx");
            }
            else //添加
            {
                ChkAdminLevel("sys_payment_list0", DTEnums.ActionEnum.View.ToString()); //检查权限
                int rpid = 0;
                result = DoAdd(out rpid);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("添加付款通知成功！", "pay_list.aspx");
            }
        }
        protected void ddlmethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            if (!string.IsNullOrEmpty(ddl.SelectedValue))
            {
                Model.payMethod model = new BLL.payMethod().GetModel(Utils.StrToInt(ddl.SelectedValue, 0));
                if (model.pm_type.Value)
                {
                    dlceDate.Visible = true;
                    dlceNum.Visible = true;
                }
                else
                {
                    dlceDate.Visible = false;
                    dlceNum.Visible = false;
                }
            }
        }

        protected void btnSubmitToDistribute_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                ChkAdminLevel("sys_payment_list0", DTEnums.ActionEnum.View.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("修改付款通知成功！", "rpDistribution.aspx?id="+ this.id);
            }
            else //添加
            {
                ChkAdminLevel("sys_payment_list0", DTEnums.ActionEnum.View.ToString()); //检查权限
                int rpid = 0;
                result = DoAdd(out rpid);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("添加付款通知成功！", "rpDistribution.aspx?id="+ rpid);
            }
        }

    }
}