using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace MettingSys.Web.admin.finance
{
    public partial class finance_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        protected int id = 0;

        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;
        protected bool type = true;//收付标识
        protected string typeText = "", oID = "";
        //protected string minDate = "", maxDate = "";
        protected string fromOrder = "";//true时表示从订单页面添加

        protected void Page_Load(object sender, EventArgs e)
        {
            action = DTRequest.GetQueryString("action");
            this.type = bool.Parse(DTRequest.GetQueryString("type"));
            this.oID = DTRequest.GetQueryString("oID");
            fromOrder = DTRequest.GetQueryString("fromOrder");
            if (this.type)
            {
                typeText = "应收";
            }
            else
            {
                typeText = "应付";
            }
            if (action == DTEnums.ActionEnum.Add.ToString())
            {
                //Model.Order order = new BLL.Order().GetModel(oID);
                //if (order == null)
                //{
                //    JscriptMsg("订单不存在！", "back");
                //    return;
                //}
                //minDate = order.o_sdate.Value.ToString("yyyy-MM-dd");
                //maxDate = order.o_edate.Value.ToString("yyyy-MM-dd");
            }
            if (!string.IsNullOrEmpty(action) && action == DTEnums.ActionEnum.Edit.ToString())
            {
                this.id = DTRequest.GetQueryInt("id");
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.finance().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            if (action == DTEnums.ActionEnum.View.ToString())
            {
                this.id = DTRequest.GetQueryInt("id");
                btnSave.Visible = false;
            }
            if (!Page.IsPostBack)
            {
                manager = GetAdminInfo();
                InitData();
                if (action == DTEnums.ActionEnum.Edit.ToString() || action==DTEnums.ActionEnum.View.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
                //if (this.type)
                //{
                //    ChkAdminLevel("sys_finance_list1", action); //检查权限
                //}
                //else
                //{
                //    ChkAdminLevel("sys_finance_list0", action); //检查权限
                //}
            }
        }
        #region 初始化数据=================================
        private void InitData()
        {
            string sqlwhere = "na_isUse=1";
            if (!new BLL.permission().checkHasPermission(manager, "0401"))            
            {
                sqlwhere += " and na_flag=0";
            }
            ddlnature.DataSource = new BLL.businessNature().GetList(0, sqlwhere, "na_sort asc,na_id desc");
            ddlnature.DataTextField = "na_name";
            ddlnature.DataValueField = "na_id";
            ddlnature.DataBind();
            ddlnature.Items.Insert(0, new ListItem("请选择", ""));
        }
        #endregion
        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.finance bll = new BLL.finance();
            DataSet ds = bll.GetList(0, "fin_id=" + _id + "", "");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                this.oID = dr["fin_oid"].ToString();
                this.type = bool.Parse(dr["fin_type"].ToString());
                if (this.type)
                {
                    typeText = "应收";
                }
                else
                {
                    typeText = "应付";
                }
                txtCusName.Text = dr["c_name"].ToString();
                hCusId.Value = dr["fin_cid"].ToString();
                ddlnature.Visible = false;
                //ddlnature.SelectedValue = dr["fin_nature"].ToString();
                labnature.Text = dr["na_name"].ToString();
                if (!Convert.ToBoolean(dr["na_type"]))
                {
                    detail1.Visible = true;
                    detail2.Visible = false;
                    ddldetail.DataSource = new BLL.businessDetails().GetList(0, " de_nid =" + dr["fin_nature"] + " and de_isUse=1 ", "de_sort asc,de_id desc");
                    ddldetail.DataTextField = "de_name";
                    ddldetail.DataValueField = "de_name";
                    ddldetail.DataBind();
                    ddldetail.Items.Insert(0, new ListItem("请选择", ""));
                    ddldetail.SelectedValue = dr["fin_detail"].ToString();
                }
                else
                {
                    detail1.Visible = false;
                    detail2.Attributes.Add("style","display:block;");
                    //林智斌|SY002|SY,林蕴钊|SY003|SY,段强|SY004|SY
                    if (!string.IsNullOrEmpty(dr["fin_detail"].ToString()))
                    {
                        string[] list = dr["fin_detail"].ToString().Split(',');
                        DataTable dt = new DataTable();
                        dt.Columns.Add("op_name");
                        dt.Columns.Add("op_number");
                        dt.Columns.Add("op_area");
                        DataRow drr = dt.NewRow();
                        foreach(string item in list)
                        {
                            string[] nlist = item.Split('|');
                            drr = dt.NewRow();
                            drr["op_name"] = nlist[0];
                            drr["op_number"] = nlist[1];
                            drr["op_area"] = nlist[2];
                            dt.Rows.Add(drr);
                        }
                        rptEmployee2.DataSource = dt;
                        rptEmployee2.DataBind();
                    }
                }
                //txtsDate.Text = Convert.ToDateTime(dr["fin_sdate"]).ToString("yyyy-MM-dd");
                //txteDate.Text = Convert.ToDateTime(dr["fin_edate"]).ToString("yyyy-MM-dd");
                txtIllustration.Text = dr["fin_illustration"].ToString();
                txtExpression.Text = dr["fin_expression"].ToString();
                txtMoney.Text = dr["fin_money"].ToString();

                //minDate = ConvertHelper.toDate(dr["o_sdate"]).Value.ToString("yyyy-MM-dd");
                //maxDate = ConvertHelper.toDate(dr["o_edate"]).Value.ToString("yyyy-MM-dd");
            }

        }
        #endregion

        #region 增加操作=================================
        //private string DoAdd()
        //{
        //    Model.finance model = new Model.finance();
        //    BLL.finance bll = new BLL.finance();
        //    manager = GetAdminInfo();
        //    model.fin_oid = oID;
        //    model.fin_type = this.type;
        //    model.fin_cid = string.IsNullOrEmpty(hCusId.Value) ? 0 : Convert.ToInt32(hCusId.Value);
        //    model.fin_nature = string.IsNullOrEmpty(ddlnature.SelectedValue) ? 0 : Convert.ToInt32(ddlnature.SelectedValue);
        //    model.fin_detail = ddldetail.SelectedItem.Text;
        //    model.fin_sdate = ConvertHelper.toDate(txtsDate.Text.Trim());
        //    model.fin_edate = ConvertHelper.toDate(txteDate.Text.Trim());
        //    model.fin_illustration = txtIllustration.Text.Trim();
        //    model.fin_expression = txtExpression.Text.Trim();
        //    model.fin_money = Convert.ToDecimal(new DataTable().Compute(model.fin_expression, "false"));
        //    model.fin_flag = 0;
        //    model.fin_area = manager.area;
        //    model.fin_personNum = manager.user_name;
        //    model.fin_personName = manager.real_name;
        //    model.fin_adddate = DateTime.Now;
        //    return bll.Add(model, manager);
        //}
        #endregion

        #region 修改操作=================================
        //private string DoEdit(int _id)
        //{
        //    BLL.finance bll = new BLL.finance();
        //    Model.finance model = new Model.finance();
        //    DataSet ds = bll.GetList(0, "fin_id=" + _id + "", "");
        //    if (ds == null || ds.Tables[0].Rows.Count == 0)
        //    {
        //        return "找不到记录";
        //    }
        //    DataRow dr = ds.Tables[0].Rows[0];

        //    manager = GetAdminInfo();
        //    model.fin_id = _id;
        //    model.fin_oid = oID;
        //    model.fin_type = Convert.ToBoolean(dr["fin_type"]);

        //    string _content = string.Empty;
        //    if (dr["fin_cid"].ToString() != hCusId.Value)
        //    {
        //        _content += "" + typeText + "对象：" + dr["c_name"] + "→<font color='red'>" + txtCusName.Text.Trim() + "</font><br/>";
        //    }
        //    model.fin_cid = string.IsNullOrEmpty(hCusId.Value) ? 0 : Convert.ToInt32(hCusId.Value);
        //    model.fin_markNum = dr["fin_markNum"].ToString();
        //    model.fin_nature = Convert.ToInt32(dr["fin_nature"]);
        //    //if (dr["fin_nature"].ToString() != ddlnature.SelectedValue)
        //    //{
        //    //    _content += "业务性质：" + dr["na_name"] + "→<font color='red'>" + ddlnature.SelectedItem.Text + "</font><br/>";
        //    //}
        //    //model.fin_nature = string.IsNullOrEmpty(ddlnature.SelectedValue) ? 0 : Convert.ToInt32(ddlnature.SelectedValue);
        //    if (dr["fin_detail"].ToString() != ddldetail.SelectedValue)
        //    {
        //        _content += "业务明细：" + dr["de_name"] + "→<font color='red'>" + ddldetail.SelectedItem.Text + "</font><br/>";
        //    }
        //    model.fin_detail = ddldetail.SelectedItem.Text;
        //    if (Convert.ToDateTime(dr["fin_sdate"].ToString()).ToString("yyyy-MM-dd") != txtsDate.Text.Trim())
        //    {
        //        _content += "业务开始日期：" + Convert.ToDateTime(dr["fin_sdate"].ToString()).ToString("yyyy-MM-dd") + "→<font color='red'>" + txtsDate.Text.Trim() + "</font><br/>";
        //    }
        //    model.fin_sdate = Convert.ToDateTime(txtsDate.Text.Trim());
        //    if (Convert.ToDateTime(dr["fin_edate"].ToString()).ToString("yyyy-MM-dd") != txteDate.Text.Trim())
        //    {
        //        _content += "业务结束日期：" + Convert.ToDateTime(dr["fin_edate"].ToString()).ToString("yyyy-MM-dd") + "→<font color='red'>" + txteDate.Text.Trim() + "</font><br/>";
        //    }
        //    model.fin_edate = Convert.ToDateTime(txteDate.Text.Trim());
        //    if (dr["fin_illustration"].ToString() != txtIllustration.Text.Trim())
        //    {
        //        _content += "业务说明：" + dr["fin_illustration"] + "→<font color='red'>" + txtIllustration.Text.Trim() + "</font><br/>";
        //    }
        //    model.fin_illustration = txtIllustration.Text.Trim();
        //    if (dr["fin_expression"].ToString() != txtExpression.Text.Trim())
        //    {
        //        model.fin_money = Convert.ToDecimal(new DataTable().Compute(txtExpression.Text.Trim(), "false"));
        //        _content += "金额表达式：" + dr["fin_expression"] + "=" + dr["fin_money"] + "→<font color='red'>" + txtExpression.Text.Trim() + "=" + model.fin_money + "</font><br/>";
        //    }
        //    model.fin_expression = txtExpression.Text.Trim();
        //    model.fin_month = dr["fin_month"].ToString();
        //    model.fin_flag = Utils.ObjToByte(dr["fin_flag"]);
        //    model.fin_area = dr["fin_area"].ToString();
        //    model.fin_personNum = dr["fin_personNum"].ToString();
        //    model.fin_personName = dr["fin_personName"].ToString();
        //    model.fin_adddate = Convert.ToDateTime(dr["fin_adddate"]);
        //    model.fin_remark = dr["fin_remark"].ToString();

        //    return bll.Update(model, _content, manager);
        //}
        #endregion

        //保存
        //protected void btnSubmit_Click(object sender, EventArgs e)
        //{
        //    string result = "";
        //    if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
        //    {
        //        if (this.type)
        //        {
        //            ChkAdminLevel("sys_finance_list1", DTEnums.ActionEnum.Edit.ToString()); //检查权限
        //        }
        //        else
        //        {
        //            ChkAdminLevel("sys_finance_list0", DTEnums.ActionEnum.Edit.ToString()); //检查权限
        //        }
        //        result = DoEdit(this.id);
        //        if (result != "")
        //        {
        //            PrintJscriptMsg(result, "");
        //            return;
        //        }
        //        if (!string.IsNullOrEmpty(oID))
        //        {
        //            PrintMsg("修改" + typeText + "成功！");
        //        }
        //        else
        //        {
        //            JscriptMsg("修改" + typeText + "记录成功！", "finance_list.aspx?type=" + type);
        //        }
        //    }
        //    else //添加
        //    {
        //        if (this.type)
        //        {
        //            ChkAdminLevel("sys_finance_list1", DTEnums.ActionEnum.Add.ToString()); //检查权限
        //        }
        //        else
        //        {
        //            ChkAdminLevel("sys_finance_list0", DTEnums.ActionEnum.Add.ToString()); //检查权限
        //        }
        //        result = DoAdd();
        //        if (result != "")
        //        {
        //            PrintJscriptMsg(result, "");
        //            return;
        //        }
        //        PrintMsg("添加" + typeText + "成功！");
        //    }
        //}

        //protected void ddlnature_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    DropDownList ddl = (DropDownList)sender;
        //    if (!string.IsNullOrEmpty(ddl.SelectedValue))
        //    {
        //        int nature = Convert.ToInt32(ddl.SelectedValue);
        //        ddldetail.DataSource = new BLL.businessDetails().GetList(0, " de_nid =" + nature + " and de_isUse=1 ", "de_sort asc,de_id desc");
        //        ddldetail.DataTextField = "de_name";
        //        ddldetail.DataValueField = "de_id";
        //        ddldetail.DataBind();
        //        ddldetail.Items.Insert(0, new ListItem("请选择", ""));
        //    }
        //}
    }
}