using MettingSys.Common;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BorderStyle = NPOI.SS.UserModel.BorderStyle;

namespace MettingSys.Web.admin.unBusiness
{
    public partial class unBusinessPay_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string keywords = string.Empty, _check1 = string.Empty, _check2 = string.Empty, _check3 = string.Empty, _payStatus = string.Empty, _sforedate = string.Empty, _eforedate = string.Empty, _sdate = "", _edate = "", _area = "", _method = "", _sign = "", _money = "", _bankName = "";
        protected string _check = string.Empty, _self = string.Empty, _type = string.Empty, _function = string.Empty,_owner=string.Empty;
        protected string orderby = "isnull(uba_date,'3000-01-01') desc,isnull(pm_sort,-1) asc,uba_id desc";

        decimal _tmoney = 0;
        protected Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _check = DTRequest.GetString("check");
            if (string.IsNullOrEmpty(this._check))
            {
                this._check = "0";
            }
            _check1 = DTRequest.GetString("ddlcheck1");
            _check2 = DTRequest.GetString("ddlcheck2");
            _check3 = DTRequest.GetString("ddlcheck3");
            _payStatus = DTRequest.GetString("ddlConfirm");
            _sforedate = DTRequest.GetString("txtsforedate");
            _eforedate = DTRequest.GetString("txteforedate");
            _sdate = DTRequest.GetString("txtsdate");
            _edate = DTRequest.GetString("txtedate");
            _area = DTRequest.GetString("ddlarea");
            _type = DTRequest.GetString("ddltype");
            _function = DTRequest.GetString("txtfunction");
            _owner = DTRequest.GetString("txtOwner");
            _self = DTRequest.GetString("self");//self=1表示个人页面
            _method = DTRequest.GetString("ddlPayMethod1");
            _sign = DTRequest.GetString("ddlsign");
            _money = DTRequest.GetString("txtMoney");
            _bankName = DTRequest.GetString("txtBankName");
            switch (this._check)
            {
                case "1":
                    this._check1 = "0";
                    break;
                case "2":
                    this._check1 = "2";
                    this._check2 = "0";
                    orderby = "uba_checkTime1 asc,uba_id desc";//财务未审批页签按“部门审批”的时间降序排列
                    break;
                case "3":
                    this._check1 = "2";
                    this._check2 = "2";
                    this._check3 = "0";
                    orderby = "uba_checkTime2 asc,uba_id desc";//总经理未审批页签按“财务审批”的时间降序排列
                    break;
                case "4":
                    this._check1 = "2";
                    this._check2 = "2";
                    this._check3 = "2";
                    this._payStatus = "False";
                    break;
                case "5":
                    this._check1 = "2";
                    this._check2 = "2";
                    this._check3 = "2";
                    this._payStatus = "True";
                    break;
                default:
                    break;
            }
            if (this._self == "1")
            {
                titleDiv.Visible = false;
                liCheck.Visible = false;
                liConfirm.Visible = false;
            }
            this.pageSize = GetPageSize(10); //每页数量
            manager = GetAdminInfo();
            if (!Page.IsPostBack)
            {
                InitData();
                ChkAdminLevel("sys_unBusiness_list", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("uba_id>0" + CombSqlTxt(), orderby);
            }
        }
        #region 初始化数据=================================
        private void InitData()
        {
            //部门审批状态
            ddlcheck1.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck1.DataTextField = "value";
            ddlcheck1.DataValueField = "key";
            ddlcheck1.DataBind();
            ddlcheck1.Items.Insert(0,new ListItem("不限", ""));
            //财务审批状态
            ddlcheck2.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck2.DataTextField = "value";
            ddlcheck2.DataValueField = "key";
            ddlcheck2.DataBind();
            ddlcheck2.Items.Insert(0, new ListItem("不限", ""));
            //总经理审批状态
            ddlcheck3.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck3.DataTextField = "value";
            ddlcheck3.DataValueField = "key";
            ddlcheck3.DataBind();
            ddlcheck3.Items.Insert(0, new ListItem("不限", ""));

            ddlcheck.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck.DataTextField = "value";
            ddlcheck.DataValueField = "key";
            ddlcheck.DataBind();
            ddlcheck.Items.Insert(0, new ListItem("请选择", ""));
            //收付款方式
            //判断是否是财务来显示不同的收付款方式
            string sqlwhere = "";
            if (!new BLL.permission().checkHasPermission(manager, "0401"))
            {
                sqlwhere = " and pm_type=0";
            }
            ddlPayMethod.DataSource = new BLL.payMethod().GetList(0, "pm_isUse=1 "+ sqlwhere + "", "pm_sort asc,pm_id asc");
            ddlPayMethod.DataTextField = "pm_name";
            ddlPayMethod.DataValueField = "pm_id";
            ddlPayMethod.DataBind();
            ddlPayMethod.Items.Insert(0, new ListItem("请选择", ""));

            ddlPayMethod1.DataSource = new BLL.payMethod().GetList(0, "pm_isUse=1 " + sqlwhere + "", "pm_sort asc,pm_id asc");
            ddlPayMethod1.DataTextField = "pm_name";
            ddlPayMethod1.DataValueField = "pm_id";
            ddlPayMethod1.DataBind();
            ddlPayMethod1.Items.Insert(0, new ListItem("不限", ""));

            ddlConfirm.DataSource = Common.BusinessDict.financeConfirmStatus();
            ddlConfirm.DataTextField = "value";
            ddlConfirm.DataValueField = "key";
            ddlConfirm.DataBind();
            ddlConfirm.Items.Insert(0, new ListItem("不限", ""));

            ddlPayStatus.DataSource = Common.BusinessDict.financeConfirmStatus();
            ddlPayStatus.DataTextField = "value";
            ddlPayStatus.DataValueField = "key";
            ddlPayStatus.DataBind();
            ddlPayStatus.Items.Insert(0, new ListItem("请选择", ""));

            ddlarea.DataSource = new BLL.department().getAreaDict();
            ddlarea.DataTextField = "value";
            ddlarea.DataValueField = "key";
            ddlarea.DataBind();
            ddlarea.Items.Insert(0, new ListItem("不限", ""));

            ddltype.DataSource = BusinessDict.unBusinessNature(1);
            ddltype.DataTextField = "value";
            ddltype.DataValueField = "key";
            ddltype.DataBind();
            ddltype.Items.Insert(0, new ListItem("不限", ""));
        }
        #endregion

        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            if (!this.isSearch)
            {
                this.page = DTRequest.GetQueryInt("page", 1);
            }
            else
            {
                this.page = 1;
            }
            BLL.unBusinessApply bll = new BLL.unBusinessApply();
            DataTable dt = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, manager, out this.totalCount,out _tmoney).Tables[0];
            this.rptList.DataSource = dt;
            this.rptList.DataBind();

            tCount.Text = totalCount.ToString();
            pCount.Text = dt.Rows.Count.ToString();
            decimal _pmoney = 0;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pmoney += Utils.ObjToDecimal(dr["uba_money"], 0);
                }
            }
            pMoney.Text = _pmoney.ToString();
            tMoney.Text = _tmoney.ToString();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = backUrl();
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            ddlchecktype.SelectedValue = this._check;
            ddlcheck1.SelectedValue = this._check1;
            ddlcheck2.SelectedValue = this._check2;
            ddlcheck3.SelectedValue = this._check3;
            ddlConfirm.SelectedValue = _payStatus;
            txtsforedate.Text = _sforedate;
            txteforedate.Text = _eforedate;
            txtsdate.Text = _sdate;
            txtedate.Text = _edate;
            ddlarea.SelectedValue = _area;
            ddltype.SelectedValue = _type;
            txtfunction.Text = _function;
            txtOwner.Text = _owner;
            ddlsign.SelectedValue = _sign;
            txtMoney.Text = _money;
            txtBankName.Text = _bankName;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            //所有页签下保留可以看到！只是针对HQ工号中有部门审批权限的，他的部门审批页签里只看本区域的
            if (_check == "1" && new BLL.permission().checkHasPermission(manager, "0603"))
            {
                strTemp.Append(" and uba_area='" + manager.area + "'");
            }

            if (!string.IsNullOrEmpty(_check1))
            {
                strTemp.Append(" and uba_flag1=" + _check1 + "");
            }
            if (!string.IsNullOrEmpty(_check2))
            {
                strTemp.Append(" and uba_flag2=" + _check2 + "");
            }
            if (!string.IsNullOrEmpty(_check3))
            {
                strTemp.Append(" and uba_flag3=" + _check3 + "");
            }
            if (!string.IsNullOrEmpty(_payStatus))
            {
                strTemp.Append(" and uba_isConfirm='"+ _payStatus + "'");
            }
            if (!string.IsNullOrEmpty(_sforedate))
            {
                strTemp.Append(" and datediff(d,uba_foreDate,'" + _sforedate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_eforedate))
            {
                strTemp.Append(" and datediff(d,uba_foreDate,'" + _eforedate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp.Append(" and datediff(d,uba_date,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp.Append(" and datediff(d,uba_date,'" + _edate + "')>=0");
            }
            if (this._self == "1")
            {
                strTemp.Append(" and uba_PersonNum='" + manager.user_name + "' and uba_personName='" + manager.real_name + "'");
            }
            if (!string.IsNullOrEmpty(_area))
            {
                strTemp.Append(" and uba_area='" + _area + "'");
            }
            if (!string.IsNullOrEmpty(_type))
            {
                strTemp.Append(" and uba_type=" + _type + "");
            }
            if (!string.IsNullOrEmpty(_function))
            {
                strTemp.Append(" and uba_function like '%" + _function + "%'");
            }
            if (!string.IsNullOrEmpty(_owner.ToUpper()))
            {
                strTemp.Append(" and (uba_PersonNum like '%" + _owner + "%' or uba_personName like '%" + _owner + "%')");
            }
            if (!string.IsNullOrEmpty(_method))
            {
                strTemp.Append(" and uba_payMethod=" + _method + "");
            }
            if (!string.IsNullOrEmpty(_money))
            {
                strTemp.Append(" and uba_money " + _sign + " " + _money + "");
            }
            if (!string.IsNullOrEmpty(_bankName))
            {
                if (_bankName.Trim() == "0")
                {
                    strTemp.Append(" and isnull(uba_receiveBankName,'')=''");
                }
                else
                {
                    strTemp.Append(" and uba_receiveBankName like '%" + _bankName + "%'");
                }
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("unBusinessPay_page_size", "DTcmsPage"), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    return _pagesize;
                }
            }
            return _default_size;
        }
        #endregion

        //查询
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.isSearch = true;
            _check = DTRequest.GetFormString("check");
            _check1 = DTRequest.GetFormString("ddlcheck1");
            _check2 = DTRequest.GetFormString("ddlcheck2");
            _check3 = DTRequest.GetFormString("ddlcheck3");
            _payStatus = DTRequest.GetFormString("ddlConfirm");
            _sforedate = DTRequest.GetFormString("txtsforedate");
            _eforedate = DTRequest.GetFormString("txteforedate");
            _sdate = DTRequest.GetFormString("txtsdate");
            _edate = DTRequest.GetFormString("txtedate");
            _area = DTRequest.GetFormString("ddlarea");
            _type = DTRequest.GetFormString("ddltype");
            _function = DTRequest.GetFormString("txtfunction");
            _owner = DTRequest.GetFormString("txtOwner");
            _self = DTRequest.GetFormString("self");//self=1表示个人页面
            _method = DTRequest.GetFormString("ddlPayMethod1");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _bankName = DTRequest.GetFormString("txtBankName");
            RptBind("uba_id>0" + CombSqlTxt(), orderby);
        }

        private string backUrl()
        {
            return Utils.CombUrlTxt("unBusinessPay_list.aspx", "page={0}&ddlcheck1={1}&ddlcheck2={2}&ddlcheck3={3}&ddlConfirm={4}&txtsforedate={5}&txteforedate={6}&txtsdate={7}&txtedate={8}&ddlarea={9}&ddltype={10}&txtfunction={11}&txtOwner={12}&ddlPayMethod1={13}&ddlsign={14}&txtmoney={15}&txtBankName={16}&check={17}", "__id__", _check1, _check2, _check3, _payStatus, _sforedate, _eforedate, _sdate, _edate, _area, _type, _function, _owner, _method, _sign, _money, _bankName,_check);
        }
        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("unBusinessPay_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _check = DTRequest.GetFormString("check");
            _check1 = DTRequest.GetFormString("ddlcheck1");
            _check2 = DTRequest.GetFormString("ddlcheck2");
            _check3 = DTRequest.GetFormString("ddlcheck3");
            _payStatus = DTRequest.GetFormString("ddlConfirm");
            _sforedate = DTRequest.GetFormString("txtsforedate");
            _eforedate = DTRequest.GetFormString("txteforedate");
            _sdate = DTRequest.GetFormString("txtsdate");
            _edate = DTRequest.GetFormString("txtedate");
            _area = DTRequest.GetFormString("ddlarea");
            BLL.unBusinessApply bll = new BLL.unBusinessApply();
            DataTable dt = bll.GetList(this.pageSize, this.page, "uba_id>0" + CombSqlTxt(), "isnull(uba_date,'3000-01-01') desc,isnull(pm_sort,-1) asc,uba_id desc", manager, out this.totalCount, out _tmoney,false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=非业务支付申请列表.xlsx"); //HttpUtility.UrlEncode(fileName));
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet("明细");
            IFont font = hssfworkbook.CreateFont();
            font.Boldweight = short.MaxValue;
            font.FontHeightInPoints = 11;

            #region 表格样式
            //设置单元格的样式：水平垂直对齐居中
            ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BottomBorderColor = HSSFColor.Black.Index;
            cellStyle.LeftBorderColor = HSSFColor.Black.Index;
            cellStyle.RightBorderColor = HSSFColor.Black.Index;
            cellStyle.TopBorderColor = HSSFColor.Black.Index;
            cellStyle.WrapText = true;//自动换行

            //设置表头的样式：水平垂直对齐居中，加粗
            ICellStyle titleCellStyle = hssfworkbook.CreateCellStyle();
            titleCellStyle.Alignment = HorizontalAlignment.Center;
            titleCellStyle.VerticalAlignment = VerticalAlignment.Center;
            titleCellStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index; //图案颜色
            titleCellStyle.FillPattern = FillPattern.SparseDots; //图案样式
            titleCellStyle.FillBackgroundColor = HSSFColor.Grey25Percent.Index; //背景颜色
            //设置边框
            titleCellStyle.BorderBottom = BorderStyle.Thin;
            titleCellStyle.BorderLeft = BorderStyle.Thin;
            titleCellStyle.BorderRight = BorderStyle.Thin;
            titleCellStyle.BorderTop = BorderStyle.Thin;
            titleCellStyle.BottomBorderColor = HSSFColor.Black.Index;
            titleCellStyle.LeftBorderColor = HSSFColor.Black.Index;
            titleCellStyle.RightBorderColor = HSSFColor.Black.Index;
            titleCellStyle.TopBorderColor = HSSFColor.Black.Index;
            //设置字体
            titleCellStyle.SetFont(font);
            #endregion
            //表头
            IRow headRow = sheet.CreateRow(0);
            headRow.HeightInPoints = 25;

            headRow.CreateCell(0).SetCellValue("支付类别");
            headRow.CreateCell(1).SetCellValue("支付用途");
            headRow.CreateCell(2).SetCellValue("订单号");
            headRow.CreateCell(3).SetCellValue("用途说明");
            headRow.CreateCell(4).SetCellValue("收款账户");
            headRow.CreateCell(5).SetCellValue("收款账号");
            headRow.CreateCell(6).SetCellValue("收款银行");
            headRow.CreateCell(7).SetCellValue("金额");
            headRow.CreateCell(8).SetCellValue("预付日期");
            headRow.CreateCell(9).SetCellValue("实付日期");
            headRow.CreateCell(10).SetCellValue("付款方式");
            headRow.CreateCell(11).SetCellValue("申请人");
            headRow.CreateCell(12).SetCellValue("部门审批");
            headRow.CreateCell(13).SetCellValue("财务审批");
            headRow.CreateCell(14).SetCellValue("总经理审批");
            headRow.CreateCell(15).SetCellValue("支付确认");

            headRow.GetCell(0).CellStyle = titleCellStyle;
            headRow.GetCell(1).CellStyle = titleCellStyle;
            headRow.GetCell(2).CellStyle = titleCellStyle;
            headRow.GetCell(3).CellStyle = titleCellStyle;
            headRow.GetCell(4).CellStyle = titleCellStyle;
            headRow.GetCell(5).CellStyle = titleCellStyle;
            headRow.GetCell(6).CellStyle = titleCellStyle;
            headRow.GetCell(7).CellStyle = titleCellStyle;
            headRow.GetCell(8).CellStyle = titleCellStyle;
            headRow.GetCell(9).CellStyle = titleCellStyle;
            headRow.GetCell(10).CellStyle = titleCellStyle;
            headRow.GetCell(11).CellStyle = titleCellStyle;
            headRow.GetCell(12).CellStyle = titleCellStyle;
            headRow.GetCell(13).CellStyle = titleCellStyle;
            headRow.GetCell(14).CellStyle = titleCellStyle;
            headRow.GetCell(15).CellStyle = titleCellStyle;

            sheet.SetColumnWidth(0, 15 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 20 * 256);
            sheet.SetColumnWidth(4, 20 * 256);
            sheet.SetColumnWidth(5, 15 * 256);
            sheet.SetColumnWidth(6, 20 * 256);
            sheet.SetColumnWidth(7, 20 * 256);
            sheet.SetColumnWidth(8, 20 * 256);
            sheet.SetColumnWidth(9, 20 * 256);
            sheet.SetColumnWidth(10, 15 * 256);
            sheet.SetColumnWidth(11, 20 * 256);
            sheet.SetColumnWidth(12, 20 * 256);
            sheet.SetColumnWidth(13, 20 * 256);
            sheet.SetColumnWidth(14, 20 * 256);
            sheet.SetColumnWidth(15, 20 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(BusinessDict.unBusinessNature()[Utils.ObjToByte(dt.Rows[i]["uba_type"])]);
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["uba_function"]));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["uba_oid"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["uba_instruction"]));
                    row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["uba_receiveBankName"]));
                    row.CreateCell(5).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["uba_receiveBankNum"]));
                    row.CreateCell(6).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["uba_receiveBank"]));
                    row.CreateCell(7).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["uba_money"]));
                    row.CreateCell(8).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["uba_foreDate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(9).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["uba_Date"])==null?"":ConvertHelper.toDate(dt.Rows[i]["uba_Date"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(10).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["pm_name"]));
                    row.CreateCell(11).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["uba_personName"]));
                    row.CreateCell(12).SetCellValue(dt.Rows[i]["uba_flag1"].ToString() == "0" ? "待审批" : dt.Rows[i]["uba_flag1"].ToString() == "1" ? "审批未通过" : "审批通过");
                    row.CreateCell(13).SetCellValue(dt.Rows[i]["uba_flag2"].ToString() == "0" ? "待审批" : dt.Rows[i]["uba_flag2"].ToString() == "1" ? "审批未通过" : "审批通过");
                    row.CreateCell(14).SetCellValue(dt.Rows[i]["uba_flag3"].ToString() == "0" ? "待审批" : dt.Rows[i]["uba_flag3"].ToString() == "1" ? "审批未通过" : "审批通过");
                    row.CreateCell(15).SetCellValue(Utils.StrToBool(Utils.ObjectToStr(dt.Rows[i]["uba_isConfirm"]),false)? "已支付" : "待支付");

                    row.GetCell(0).CellStyle = cellStyle;
                    row.GetCell(1).CellStyle = cellStyle;
                    row.GetCell(2).CellStyle = cellStyle;
                    row.GetCell(3).CellStyle = cellStyle;
                    row.GetCell(4).CellStyle = cellStyle;
                    row.GetCell(5).CellStyle = cellStyle;
                    row.GetCell(6).CellStyle = cellStyle;
                    row.GetCell(7).CellStyle = cellStyle;
                    row.GetCell(8).CellStyle = cellStyle;
                    row.GetCell(9).CellStyle = cellStyle;
                    row.GetCell(10).CellStyle = cellStyle;
                    row.GetCell(11).CellStyle = cellStyle;
                    row.GetCell(12).CellStyle = cellStyle;
                    row.GetCell(13).CellStyle = cellStyle;
                    row.GetCell(14).CellStyle = cellStyle;
                    row.GetCell(15).CellStyle = cellStyle;
                }
            }

            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }
        //批量删除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            PrintLoad();
            ChkAdminLevel("sys_unBusiness_list", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.unBusinessApply bll = new BLL.unBusinessApply();
            string result = "";
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            manager = GetAdminInfo();
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                int id = Convert.ToInt32(((HiddenField)rptList.Items[i].FindControl("hidId")).Value);
                CheckBox cb = (CheckBox)rptList.Items[i].FindControl("chkId");
                if (cb.Checked)
                {
                    result = bll.Delete(Convert.ToInt32(id), manager);
                    if (result == "")
                    {
                        success++;
                        //删除文件
                        if (Directory.Exists(Server.MapPath("~/uploadPay/2/"+ id + "/")))
                        {
                            Directory.Delete(Server.MapPath("~/uploadPay/2/" + id + "/"),true);
                        }
                    }
                    else
                    {
                        error++;
                        sb.Append(result + "<br/>");
                    }
                }
            }
            JscriptMsg("共选择" + (success + error) + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), Utils.CombUrlTxt("unBusinessPay_list.aspx", "page={0}&ddlcheck1={1}&ddlcheck2={2}&ddlcheck3={3}&ddlConfirm={4}&txtsforedate={5}&txteforedate={6}&txtsdate={7}&txtedate={8}&ddlarea={9}&ddltype={10}&txtfunction={11}&txtOwner={12}&ddlPayMethod1={13}&ddlsign={14}&txtmoney={15}&txtBankName={16}", "__id__", _check1, _check2, _check3, _payStatus, _sforedate, _eforedate, _sdate, _edate,_area,_type,_function,_owner,_method,_sign,_money,_bankName));
        }

    }
}