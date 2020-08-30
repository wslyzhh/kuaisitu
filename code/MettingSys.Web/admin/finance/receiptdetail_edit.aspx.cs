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
    public partial class receiptdetail_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        protected int id = 0, finid = 0, cid = 0;

        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;
        protected string oID = string.Empty, cname = string.Empty, contentText=string.Empty;
        protected string fromOrder = "";//true时表示从订单页面添加
        protected bool isChongzhang = false, isFushu = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            action = DTRequest.GetQueryString("action");
            this.oID = DTRequest.GetQueryString("oID");
            fromOrder = DTRequest.GetQueryString("fromOrder");
            //ChkAdminLevel("sys_payment_detail1", action); //检查权限
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
                if (!new BLL.ReceiptPayDetail().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            //查看
            if (action == DTEnums.ActionEnum.View.ToString())
            {
                this.action = DTEnums.ActionEnum.View.ToString();//修改类型
                this.id = DTRequest.GetQueryInt("id");
                btnSubmit.Visible = false;
            }
            if (!Page.IsPostBack)
            {
                finid = DTRequest.GetQueryInt("id", 0);
                cid = DTRequest.GetQueryInt("cid", 0);
                cname = DTRequest.GetQueryString("cname");
                contentText = DTRequest.GetQueryString("contentText");
                if (action == DTEnums.ActionEnum.Add.ToString())
                {
                    hCusId.Value = cid.ToString();
                    txtCusName.Text = cname;
                    txtContent.Text = contentText;
                }
                InitData();
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
            if (!new BLL.permission().checkHasPermission(manager,"0401"))
            {
                sqlwhere = " and pm_type=0";
            }
            ddlmethod.DataSource = new BLL.payMethod().GetList(0, "pm_isUse=1 "+ sqlwhere + "", "pm_sort asc,pm_id asc");
            ddlmethod.DataTextField = "pm_name";
            ddlmethod.DataValueField = "pm_id";
            ddlmethod.DataBind();
            ddlmethod.Items.Insert(0, new ListItem("请选择", ""));
        }

        protected void ddlmethod_DataBound(object sender, EventArgs e)
        {
            DropDownList ddl = sender as DropDownList;
            DataTable dt = ((DataSet)ddl.DataSource).Tables[0];
            ddl.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                ddl.Items.Add(new ListItem(Utils.ObjectToStr(dr["pm_name"]), Utils.ObjectToStr(dr["pm_id"])));
                ddl.Items.FindByValue(Utils.ObjectToStr(dr["pm_id"])).Attributes.Add("py", Utils.ObjectToStr(dr["pm_type"]));
            }
            ddl.Items.Insert(0, new ListItem("请选择", ""));
        }
        #endregion

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            DataSet ds = bll.GetList(0, "rpd_id=" + _id + "", "");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                txtCusName.Text = dr["c_name"].ToString();
                hCusId.Value = dr["rpd_cid"].ToString();
                txtMoney.Text = dr["rpd_money"].ToString();
                if (Utils.StrToFloat(dr["rp_money"].ToString(), 0) < 0)
                {
                    isFushu = true;
                }
                if (dr["rpd_foredate"] != null)
                {
                    txtforedate.Text = Convert.ToDateTime(dr["rpd_foredate"]).ToString("yyyy-MM-dd");
                }
                txtBank.Text = Utils.ObjectToStr(dr["cb_bank"]) + "(" + Utils.ObjectToStr(dr["cb_bankName"]) + "/" + Utils.ObjectToStr(dr["cb_bankNum"]) + ")";
                hBankId.Value = Utils.ObjectToStr(dr["rp_cbid"]);

                ddlmethod.SelectedValue = dr["rpd_method"].ToString();
                txtContent.Text = dr["rpd_content"].ToString();
                if (dr["pm_type"].ToString() == "True")
                {
                    isChongzhang = true;
                }
            }

        }
        #endregion

        #region 增加操作=================================
        private string DoAdd(out int id)
        {
            id = 0;
            Model.ReceiptPayDetail model = new Model.ReceiptPayDetail();
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();            
            manager = GetAdminInfo();
            model.rpd_type = true;
            model.rpd_oid = oID;
            model.rpd_cid = Utils.StrToInt(hCusId.Value,0);
            model.rpd_content = txtContent.Text.Trim();
            model.rpd_money = Utils.StrToDecimal(txtMoney.Text.Trim(), 0);
            model.rpd_foredate = ConvertHelper.toDate(txtforedate.Text.Trim());
            model.rpd_method = Utils.StrToInt(ddlmethod.SelectedValue,0);
            //model.rpd_content = txtContent.Text.Trim();
            model.rpd_personNum = manager.user_name;
            model.rpd_personName = manager.real_name;
            model.rpd_adddate = DateTime.Now;
            model.rpd_flag1 = 2;
            //model.rpd_area = manager.area;    
            model.rpd_cbid = 0;
            if (model.rpd_money < 0)
            {
                model.rpd_cbid = Utils.StrToInt(hBankId.Value, 0);
            }
            return bll.AddReceiptPay(model, manager,out id);
        }
        #endregion

        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            Model.ReceiptPayDetail model = bll.GetModel(_id);            
            manager = GetAdminInfo();            
            string _content = string.Empty;
            if (model.rpd_cid.ToString() != hCusId.Value)
            {
                _content += "收款对象ID：" + model.rpd_cid + "→<font color='red'>" + hCusId.Value + "</font><br/>";
            }
            model.rpd_cid = Utils.StrToInt(hCusId.Value, 0);
            bool updateMoney = false;
            if (model.rpd_money.ToString() != txtMoney.Text.Trim())
            {
                updateMoney = true;
                _content += "收款金额：" + model.rpd_money + "→<font color='red'>" + txtMoney.Text.Trim() + "</font><br/>";
            }
            model.rpd_money = Utils.StrToDecimal(txtMoney.Text.Trim(), 0);
            if (model.rpd_foredate.Value.ToString("yyyy-MM-dd") != txtforedate.Text.Trim())
            {
                _content += "预收日期：" + model.rpd_foredate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + txtforedate.Text.Trim() + "</font><br/>";
            }
            model.rpd_foredate = ConvertHelper.toDate(txtforedate.Text.Trim());
            if (model.rpd_method.ToString() != ddlmethod.SelectedValue)
            {
                _content += "收款方式ID：" + model.rpd_method + "→<font color='red'>" + ddlmethod.SelectedItem.Text + "</font><br/>";
            }
            model.rpd_method = Utils.StrToInt(ddlmethod.SelectedValue, 0);
            if (model.rpd_content != txtContent.Text.Trim())
            {
                _content += "收款内容：" + model.rpd_content + "→<font color='red'>" + txtContent.Text.Trim() + "</font><br/>";
            }
            model.rpd_content = txtContent.Text.Trim();
            if (model.rpd_cbid != Utils.StrToInt(hBankId.Value, 0))
            {
                _content += "客户银行账号：" + model.rpd_cbid + "→<font color='red'>" + hBankId.Value + "</font><br/>";
            }
            model.rpd_cbid = Utils.StrToInt(hBankId.Value, 0);
            return bll.Update(model, _content, manager, updateMoney);
        }
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                //ChkAdminLevel("sys_payment_detail1", DTEnums.ActionEnum.View.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    PrintJscriptMsg(result, "");
                    return;
                }
                if (fromOrder == "true")
                {
                    PrintMsg("修改收款明细成功！");
                }
                else
                {
                    JscriptMsg("修改收款明细成功！", "receiptdetail_list.aspx");
                }
            }
            else //添加
            {
                //ChkAdminLevel("sys_payment_detail1", DTEnums.ActionEnum.View.ToString()); //检查权限
                int id = 0;
                result = DoAdd(out id);
                if (result != "")
                {
                    PrintJscriptMsg(result, "");
                    return;
                }
                if (fromOrder == "true")
                {
                    PrintMsg("添加收款明细成功！");
                }
                else
                {
                    JscriptMsg("添加收款明细成功！", "receiptdetail_list.aspx");
                }
            }
        }
    }
}