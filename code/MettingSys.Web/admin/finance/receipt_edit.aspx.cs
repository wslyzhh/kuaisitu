using MettingSys.Common;
using Newtonsoft.Json.Linq;
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
    public partial class receipt_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        protected int id = 0;
        protected string oidStr = "";//不为空时表示从客户对账明细过来
        protected string _cid = "", _cusName = "", _tMoney = "", _chk = "", _oStr = "";
        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            string _action = DTRequest.GetQueryString("action");
            oidStr = DTRequest.GetQueryString("oidStr");
            _cid = DTRequest.GetQueryString("cid");
            _cusName = DTRequest.GetQueryString("cusName");
            _tMoney = DTRequest.GetQueryString("tMoney");
            _chk = DTRequest.GetQueryString("chk");
            _oStr = DTRequest.GetQueryString("oStr");
            if (!string.IsNullOrEmpty(_action) && _action == DTEnums.ActionEnum.Edit.ToString())
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
            manager = GetAdminInfo();
            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(oidStr))
                {
                    hCusId.Value = _cid;
                    txtCusName.Text = _cusName;
                    topDiv.Visible = false;
                    floatHead.Visible = false;
                    btnReturn.Visible = false;
                    txtMoney.Text = _tMoney;
                }
                dlceNum.Visible = false;
                dlceDate.Visible = false;
                InitData();
                if (string.IsNullOrEmpty(oidStr))
                {
                    ChkAdminLevel("sys_payment_list1", DTEnums.ActionEnum.View.ToString()); //检查权限
                }
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
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
            ddlmethod.DataSource = new BLL.payMethod().GetList(0, "pm_isUse=1 "+ sqlwhere + "", "pm_sort asc,pm_id asc");
            ddlmethod.DataTextField = "pm_name";
            ddlmethod.DataValueField = "pm_id";
            ddlmethod.DataBind();
            ddlmethod.Items.Insert(0, new ListItem("请选择", ""));
        }
        #endregion
        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            DataSet ds = bll.GetList(0, "rp_id=" + _id + "", "");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                txtCusName.Text = dr["c_name"].ToString();
                hCusId.Value = dr["rp_cid"].ToString();
                txtMoney.Text = dr["rp_money"].ToString();
                if (dr["rp_foredate"] != null)
                {
                    txtforedate.Text = Convert.ToDateTime(dr["rp_foredate"]).ToString("yyyy-MM-dd");
                }
                ddlmethod.SelectedValue = dr["rp_method"].ToString();
                txtContent.Text = dr["rp_content"].ToString();
                if (dr["pm_type"].ToString()=="True")
                {
                    dlceDate.Visible = true;
                    dlceNum.Visible = true;
                    txtCenum.Text = dr["ce_num"].ToString();
                    txtCedate.Text = Utils.ObjectToDateTime(dr["ce_date"]).ToString("yyyy-MM-dd");
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
            model.rp_type = true;
            model.rp_cid = Utils.StrToInt(hCusId.Value,0);
            model.rp_money = Utils.StrToDecimal(txtMoney.Text.Trim(), 0);
            model.rp_foredate = ConvertHelper.toDate(txtforedate.Text.Trim());
            model.rp_method = Utils.StrToInt(ddlmethod.SelectedValue,0);
            model.rp_content = txtContent.Text.Trim();
            //在客户对账明细的账单打包时不是预收款
            bool flag = true;
            if (!string.IsNullOrEmpty(oidStr))
            {
                flag = false;
            }
            string result = bll.Add(model, manager,txtCenum.Text.Trim(),txtCedate.Text.Trim(),out rpid, flag);
            if (string.IsNullOrEmpty(result))
            {
                if (!string.IsNullOrEmpty(oidStr) && model.rp_money == Utils.StrToDecimal(_tMoney,0))
                {
                    JArray ja = JArray.Parse(oidStr);
                    foreach (JObject item in ja)
                    {
                        Model.ReceiptPayDetail rpdModel = new Model.ReceiptPayDetail();
                        rpdModel.rpd_type = true;
                        rpdModel.rpd_rpid = rpid;
                        rpdModel.rpd_oid = item["oid"].ToString();
                        rpdModel.rpd_cid = Utils.StrToInt(_cid, 0);
                        rpdModel.rpd_num = _chk;
                        rpdModel.rpd_money = Utils.StrToDecimal(item["money"].ToString(), 0);
                        rpdModel.rpd_foredate = model.rp_foredate;
                        rpdModel.rpd_method = model.rp_method;
                        rpdModel.rpd_personName = manager.real_name;
                        rpdModel.rpd_personNum = manager.user_name;
                        rpdModel.rpd_flag1 = 2;
                        rpdModel.rpd_flag2 = 2;
                        rpdModel.rpd_flag3 = 2;
                        rpdModel.rpd_adddate = DateTime.Now;
                        rpdModel.rpd_content = "";
                        new BLL.ReceiptPayDetail().AddOrCancleDistribution(rpdModel, manager);
                    }
                }
            }
            return result;
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
                _content += "收款对象ID：" + model.rp_cid + "→<font color='red'>" + hCusId.Value + "</font><br/>";
            }
            model.rp_cid = Utils.StrToInt(hCusId.Value,0);
            if (model.rp_money.ToString() != txtMoney.Text.Trim())
            {
                _content += "收款金额：" + model.rp_money + "→<font color='red'>" + txtMoney.Text.Trim() + "</font><br/>";
            }
            model.rp_money = Utils.StrToDecimal(txtMoney.Text.Trim(), 0);
            if (model.rp_foredate.Value.ToString("yyyy-MM-dd")!= txtforedate.Text.Trim())
            {
                _content += "预收日期：" + model.rp_foredate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + txtforedate.Text.Trim() + "</font><br/>";
            }
            model.rp_foredate = ConvertHelper.toDate(txtforedate.Text.Trim());
            if (model.rp_method.ToString() != ddlmethod.SelectedValue)
            {
                _content += "收款方式ID：" + model.rp_method.ToString() + "→<font color='red'>" + ddlmethod.SelectedValue + "</font><br/>";
            }
            model.rp_method = Utils.StrToInt(ddlmethod.SelectedValue, 0);
            if (model.rp_content != txtContent.Text.Trim())
            {
                _content += "收款内容：" + model.rp_content + "→<font color='red'>" + txtContent.Text.Trim() + "</font><br/>";
            }
            model.rp_content = txtContent.Text.Trim();
            return bll.Update(model, _content, manager, txtCenum.Text.Trim(), txtCedate.Text.Trim());
        }
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                //if (string.IsNullOrEmpty(oidStr))
                //{
                //    ChkAdminLevel("sys_payment_list1", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                //}
                result = DoEdit(this.id);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("修改收款通知成功！", "receipt_list.aspx");
            }
            else //添加
            {
                //if (string.IsNullOrEmpty(oidStr))
                //{
                //    ChkAdminLevel("sys_payment_list1", DTEnums.ActionEnum.Add.ToString()); //检查权限
                //}
                int rpid = 0;
                result = DoAdd(out rpid);
                if (result != "")
                {
                    //JscriptMsg(result, "");
                    PrintJscriptMsg(result, "");
                    return;
                }
                //JscriptMsg("添加收款通知成功！", "rpDistribution.aspx?id=" + rpid);
                if (!string.IsNullOrEmpty(oidStr))
                {
                    PrintMsg("添加收款通知成功！");
                }
                else
                {
                    JscriptMsg("添加收款通知成功！", "receipt_list.aspx");
                }
            }
        }

        protected void btnSubmitToDistribute_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                //if (string.IsNullOrEmpty(oidStr))
                //{
                //    ChkAdminLevel("sys_payment_list1", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                //}
                result = DoEdit(this.id);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("修改收款通知成功！", "rpDistribution.aspx?id=" + this.id);
            }
            else //添加
            {
                //if (string.IsNullOrEmpty(oidStr))
                //{
                //    ChkAdminLevel("sys_payment_list1", DTEnums.ActionEnum.Add.ToString()); //检查权限
                //}
                int rpid = 0;
                result = DoAdd(out rpid);
                if (result != "")
                {
                    //JscriptMsg(result, "");
                    PrintJscriptMsg(result, "");
                    return;
                }
                //JscriptMsg("添加收款通知成功！", "rpDistribution.aspx?id=" + rpid);
                if (!string.IsNullOrEmpty(oidStr))
                {
                    //PrintMsg("添加收款通知成功！"); 
                    Response.Redirect("rpDistribution.aspx?id=" + rpid + "&txtOrderId=" + _oStr);
                }
                else
                {
                    JscriptMsg("添加收款通知成功！", "rpDistribution.aspx?id=" + rpid);
                }
            }
        }
        protected void ddlmethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            if (!string.IsNullOrEmpty(ddl.SelectedValue))
            {
                Model.payMethod model = new BLL.payMethod().GetModel(Utils.StrToInt(ddl.SelectedValue,0));
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
    }
}