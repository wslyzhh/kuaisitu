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

namespace MettingSys.Web.admin.finance
{
    public partial class invoice_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string _cusName = "", _cid = "", _check1 = string.Empty, _check2 = string.Empty, _check3 = string.Empty, _isconfirm = string.Empty, _oid = string.Empty, _sign = "", _money = "", _sdate = "", _edate = "", _farea = "", _darea = "", _invType = "", _unit = "", _purchaserName;
        protected string _self = string.Empty, _check = "", _name = "", orderby = "inv_addDate desc,inv_id desc";
        protected Model.manager manager = null;
        decimal _tmoney = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            _check = DTRequest.GetString("check");
            if (string.IsNullOrEmpty(this._check))
            {
                this._check = "0";
            }
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _check1 = DTRequest.GetString("ddlcheck1");
            _check2 = DTRequest.GetString("ddlcheck2");
            _check3 = DTRequest.GetString("ddlcheck3");
            _isconfirm = DTRequest.GetString("ddlisConfirm");
            _oid = DTRequest.GetString("txtOid");
            _sign = DTRequest.GetString("ddlsign");
            _money = DTRequest.GetString("txtMoney");
            _sdate = DTRequest.GetString("txtsDate");
            _edate = DTRequest.GetString("txteDate");
            _farea = DTRequest.GetString("ddlfarea");
            _darea = DTRequest.GetString("ddldarea");
            _invType = DTRequest.GetString("ddlinvType");
            _name = DTRequest.GetString("txtName");
            _unit = DTRequest.GetString("txtUnit");
            _purchaserName = DTRequest.GetString("txtpurchaserName");
            _self = DTRequest.GetQueryString("self");//self=1表示个人页面
            switch (this._check)
            {
                case "1":
                    this._check1 = "0";
                    break;
                case "2":
                    this._check1 = "2";
                    this._check2 = "0";
                    orderby = "inv_checkTime1 asc,inv_id desc";//“开票区域未审批页签”的记录按“申请区域审批”的时间降序排列
                    break;
                case "3":
                    this._check1 = "2";
                    this._check2 = "2";
                    this._check3 = "0";
                    orderby = "inv_checkTime2 asc,inv_id desc";//“财务未审批页签”的记录按“开票区域审批”的时间降序排列
                    break;
                case "4":
                    this._check1 = "2";
                    this._check2 = "2";
                    this._check3 = "2";
                    _isconfirm = "False";
                    break;
                case "5":
                    this._check1 = "2";
                    this._check2 = "2";
                    this._check3 = "2";
                    _isconfirm = "True";
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
                if (this._self != "1")
                {
                    ChkAdminLevel("sys_invoice", DTEnums.ActionEnum.View.ToString()); //检查权限
                }
                RptBind("inv_id>0" + CombSqlTxt(), orderby);
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
            ddlcheck1.Items.Insert(0, new ListItem("不限", ""));
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

            ddlisConfirm.DataSource = Common.BusinessDict.invoiceConfirmStatus();
            ddlisConfirm.DataTextField = "value";
            ddlisConfirm.DataValueField = "key";
            ddlisConfirm.DataBind();
            ddlisConfirm.Items.Insert(0, new ListItem("不限", ""));

            ddlisConfirm1.DataSource = Common.BusinessDict.invoiceConfirmStatus();
            ddlisConfirm1.DataTextField = "value";
            ddlisConfirm1.DataValueField = "key";
            ddlisConfirm1.DataBind();
            ddlisConfirm1.Items.Insert(0, new ListItem("请选择", ""));
            ddlisConfirm1.SelectedValue = "True";

            ddlfarea.DataSource = new BLL.department().getAreaDict();
            ddlfarea.DataTextField = "value";
            ddlfarea.DataValueField = "key";
            ddlfarea.DataBind();
            ddlfarea.Items.Insert(0, new ListItem("不限", ""));

            ddldarea.DataSource = new BLL.department().getAreaDict();
            ddldarea.DataTextField = "value";
            ddldarea.DataValueField = "key";
            ddldarea.DataBind();
            ddldarea.Items.Insert(0, new ListItem("不限", ""));

            ddlinvType.DataSource = BusinessDict.invType();
            ddlinvType.DataTextField = "value";
            ddlinvType.DataValueField = "key";
            ddlinvType.DataBind();
            ddlinvType.Items.Insert(0, new ListItem("不限", ""));
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
            BLL.invoices bll = new BLL.invoices();
            DataTable dt= bll.GetList(this.pageSize, this.page, _strWhere, _orderby, manager, out this.totalCount,out _tmoney).Tables[0];
            this.rptList.DataSource = dt;
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = backUrl();
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            pCount.Text = dt.Rows.Count.ToString();
            decimal _pmoney = 0;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pmoney += Utils.ObjToDecimal(dr["inv_money"], 0);
                }
            }
            pMoney.Text = _pmoney.ToString();
            tCount.Text = totalCount.ToString();
            tMoney.Text = _tmoney.ToString();

            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            ddlchecktype.SelectedValue = this._check;
            ddlcheck1.SelectedValue = _check1;
            ddlcheck2.SelectedValue = _check2;
            ddlcheck3.SelectedValue = _check3;
            ddlisConfirm.SelectedValue = _isconfirm;
            txtOid.Text = _oid;
            ddlsign.SelectedValue = _sign;
            txtMoney.Text = _money;
            txtsDate.Text = _sdate;
            txteDate.Text = _edate;
            ddlfarea.SelectedValue = _farea;
            ddldarea.SelectedValue = _darea;
            ddlinvType.SelectedValue = _invType;
            txtName.Text = _name;
            txtUnit.Text = _unit;
            txtpurchaserName.Text = _purchaserName;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                strTemp.Append(" and inv_cid = " + _cid + "");
            }
            if (!string.IsNullOrEmpty(_oid))
            {
                strTemp.Append(" and inv_oid like  '%" + _oid + "%'");
            }
            if (!string.IsNullOrEmpty(_check1))
            {
                strTemp.Append(" and inv_flag1=" + _check1 + "");
            }
            if (!string.IsNullOrEmpty(_check2))
            {
                strTemp.Append(" and inv_flag2=" + _check2 + "");
            }
            if (!string.IsNullOrEmpty(_check3))
            {
                strTemp.Append(" and inv_flag3=" + _check3 + "");
            }
            if (!string.IsNullOrEmpty(_isconfirm))
            {
                strTemp.Append(" and inv_isConfirm='" + _isconfirm + "'");
            }
            if (this._self == "1")
            {
                strTemp.Append(" and inv_personNum='" + manager.user_name + "' and inv_personName='" + manager.real_name + "'");
            }
            if (!string.IsNullOrEmpty(_money))
            {
                strTemp.Append(" and inv_money " + _sign + " " + _money + "");
            }
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp.Append(" and datediff(d,inv_date,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp.Append(" and datediff(d,inv_date,'" + _edate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_farea))
            {
                strTemp.Append(" and inv_farea='"+_farea+"'");
            }
            if (!string.IsNullOrEmpty(_darea))
            {
                strTemp.Append(" and inv_darea='" + _darea + "'");
            }
            if (!string.IsNullOrEmpty(_invType))
            {
                strTemp.Append(" and inv_type='" + _invType + "'");
            }
            if (!string.IsNullOrEmpty(_name))
            {
                strTemp.Append(" and (inv_personNum='" + _name + "' or inv_personName like '%" + _name + "%')");
            }
            //列表权限控制
            if (manager.area != new BLL.department().getGroupArea())//如果不是总部的工号
            {
                if (new BLL.permission().checkHasPermission(manager, "0602"))
                {
                    //含有区域权限可以查看本区域添加的
                    strTemp.Append(" and (inv_farea='" + manager.area + "' or inv_darea='"+ manager.area + "')");
                }
                else
                {
                    //只能
                    strTemp.Append(" and inv_personNum='" + manager.user_name + "'");
                }
            }

            //所有页签下保留可以看到！只是针对HQ工号中有部门审批权限的，他的部门审批页签里只看本区域的
            if (new BLL.permission().checkHasPermission(manager, "0603"))
            {
                if (_check == "1")
                {
                    strTemp.Append(" and inv_farea='" + manager.area + "'");
                }
                else if (_check == "2")
                {
                    strTemp.Append(" and inv_darea='" + manager.area + "'");
                }
            }

            if (!string.IsNullOrEmpty(_unit))
            {
                strTemp.Append(" and invU_name like  '%" + _unit + "%'");
            }

            if (!string.IsNullOrEmpty(_purchaserName))
            {
                strTemp.Append(" and inv_purchaserName like  '%" + _purchaserName + "%'");
            }

            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("invoice_page_size", "DTcmsPage"), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    return _pagesize;
                }
            }
            return _default_size;
        }
        #endregion

        //关健字查询
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.isSearch = true;
            _cusName = DTRequest.GetFormString("txtCusName");
            _cid = DTRequest.GetFormString("hCusId");
            _check1 = DTRequest.GetFormString("ddlcheck1");
            _check2 = DTRequest.GetFormString("ddlcheck2");
            _check3 = DTRequest.GetFormString("ddlcheck3");
            _isconfirm = DTRequest.GetFormString("ddlisConfirm");
            _oid = DTRequest.GetFormString("txtOid");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _sdate = DTRequest.GetFormString("txtsDate");
            _edate = DTRequest.GetFormString("txteDate");
            _farea = DTRequest.GetFormString("ddlfarea");
            _darea = DTRequest.GetFormString("ddldarea");
            _invType = DTRequest.GetFormString("ddlinvType");
            _name = DTRequest.GetFormString("txtName");
            _unit = DTRequest.GetFormString("txtUnit");
            _purchaserName = DTRequest.GetFormString("txtpurchaserName");
            _self = DTRequest.GetFormString("self");//self=1表示个人页面
            RptBind("inv_id>0" + CombSqlTxt(), orderby);
            
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("invoice_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }
        private string backUrl()
        {
            return Utils.CombUrlTxt("invoice_list.aspx", "page={0}&txtCusName={1}&hCusId={2}&ddlcheck1={3}&ddlcheck2={4}&ddlcheck3={5}&ddlisConfirm={6}&txtOid={7}&self={8}&ddlsign={9}&txtMoney={10}&txtsDate={11}&txteDate={12}&ddlfarea={13}&ddldarea={14}&ddlinvType={15}&txtName={16}&check={17}&txtUnit={18}&txtpurchaserName={19}", "__id__", _cusName, _cid, _check1, _check2, _check3, _isconfirm, _oid, _self, _sign, _money, _sdate, _edate, _farea, _darea, _invType, _name, _check, _unit,_purchaserName);
        }
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetFormString("txtCusName");
            _cid = DTRequest.GetFormString("hCusId");
            _check1 = DTRequest.GetFormString("ddlcheck1");
            _check2 = DTRequest.GetFormString("ddlcheck2");
            _check3 = DTRequest.GetFormString("ddlcheck3");
            _isconfirm = DTRequest.GetFormString("ddlisConfirm");
            _oid = DTRequest.GetFormString("txtOid");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _sdate = DTRequest.GetFormString("txtsDate");
            _edate = DTRequest.GetFormString("txteDate");
            _farea = DTRequest.GetFormString("ddlfarea");
            _darea = DTRequest.GetFormString("ddldarea");
            _invType = DTRequest.GetFormString("ddlinvType");
            _name = DTRequest.GetFormString("txtName");
            _unit = DTRequest.GetFormString("txtUnit");
            _purchaserName = DTRequest.GetFormString("txtpurchaserName");
            _self = DTRequest.GetFormString("self");//self=1表示个人页面
            BLL.invoices bll = new BLL.invoices();
            DataTable dt = bll.GetList(this.pageSize, this.page, "inv_id>0" + CombSqlTxt(), "inv_addDate desc,inv_id desc", manager, out this.totalCount,out _tmoney, false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=发票列表.xlsx"); //HttpUtility.UrlEncode(fileName));
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

            headRow.CreateCell(0).SetCellValue("客户");
            headRow.CreateCell(1).SetCellValue("订单号");
            headRow.CreateCell(2).SetCellValue("开票项目");
            headRow.CreateCell(3).SetCellValue("开票金额");
            headRow.CreateCell(4).SetCellValue("超开");
            headRow.CreateCell(5).SetCellValue("送票方式");
            headRow.CreateCell(6).SetCellValue("开票区域");
            headRow.CreateCell(7).SetCellValue("申请人");
            headRow.CreateCell(8).SetCellValue("申请区域审批");
            headRow.CreateCell(9).SetCellValue("开票区域审批");
            headRow.CreateCell(10).SetCellValue("财务审批");
            headRow.CreateCell(11).SetCellValue("开票状态");
            headRow.CreateCell(12).SetCellValue("开票日期");
            headRow.CreateCell(13).SetCellValue("专普票");
            headRow.CreateCell(14).SetCellValue("开票单位");
            headRow.CreateCell(15).SetCellValue("购买方名称");

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
            sheet.SetColumnWidth(10, 20 * 256);
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
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["c_name"].ToString());
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["inv_oid"]));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["inv_serviceType"])+"/"+ Utils.ObjectToStr(dt.Rows[i]["inv_serviceName"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["inv_money"]));
                    row.CreateCell(4).SetCellValue("0");
                    row.CreateCell(5).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["inv_sentWay"]));
                    row.CreateCell(6).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["de_subname"]));
                    row.CreateCell(7).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["inv_personName"]));
                    row.CreateCell(8).SetCellValue(dt.Rows[i]["inv_flag1"].ToString() == "0" ? "待审批" : dt.Rows[i]["inv_flag1"].ToString() == "1" ? "审批未通过" : "审批通过");
                    row.CreateCell(9).SetCellValue(dt.Rows[i]["inv_flag2"].ToString() == "0" ? "待审批" : dt.Rows[i]["inv_flag2"].ToString() == "1" ? "审批未通过" : "审批通过");
                    row.CreateCell(10).SetCellValue(dt.Rows[i]["inv_flag3"].ToString() == "0" ? "待审批" : dt.Rows[i]["inv_flag3"].ToString() == "1" ? "审批未通过" : "审批通过");
                    row.CreateCell(11).SetCellValue(Utils.StrToBool(Utils.ObjectToStr(dt.Rows[i]["inv_isConfirm"]), false) ? "已开票" : "未开票");
                    row.CreateCell(12).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["inv_date"]) == null ? "" : ConvertHelper.toDate(dt.Rows[i]["inv_date"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(13).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["inv_type"]));
                    row.CreateCell(14).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["invU_name"]));
                    row.CreateCell(15).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["inv_purchaserName"]));

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
            if (this._self != "1")
            {
                ChkAdminLevel("sys_invoice", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            }
            BLL.invoices bll = new BLL.invoices();
            manager = GetAdminInfo();
            string result = "";
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                int id = Convert.ToInt32(((HiddenField)rptList.Items[i].FindControl("hidId")).Value);
                CheckBox cb = (CheckBox)rptList.Items[i].FindControl("chkId");
                if (cb.Checked)
                {
                    result = bll.Delete(Convert.ToInt32(id),manager);
                    if (result == "")
                    {
                        success++;
                    }
                    else
                    {
                        error++;
                        sb.Append(result + "<br/>");
                    }
                }
            }            
            JscriptMsg("共选择" + (success + error) + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), Utils.CombUrlTxt("invoice_list.aspx", "page={0}&txtCusName={1}&hCusId={2}&ddlcheck1={3}&ddlcheck2={4}&ddlcheck3={5}&ddlisConfirm={6}&txtOid={7}&self={8}&ddlsign={9}&txtMoney={10}&txtsDate={11}&txteDate={12}&ddlfarea={13}&ddldarea={14}&ddlinvType={15}&txtName={16}", "__id__", _cusName, _cid, _check1, _check2, _check3, _isconfirm, _oid, _self, _sign, _money, _sdate, _edate,_farea,_darea,_invType,_name));
        }
    }
}