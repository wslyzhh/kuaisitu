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

namespace MettingSys.Web.admin.statistic
{
    public partial class unReceiveAnalyze : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string _type = "", _sdate = "", _edate = "", _sdate1 = "", _edate1 = "", _status = "", _sign = "", _money1 = "", _tag = "", _self = "", _lockstatus = "", _area = "", _person1 = "";
        private Model.manager manager = null;
        decimal money1 = 0, money2 = 0, money3 = 0, money4 = 0, money5 = 0, money6 = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.pageSize = GetPageSize(10); //每页数量
            _type = DTRequest.GetString("ddltype");
            _sdate = DTRequest.GetString("txtsDate");
            _edate = DTRequest.GetString("txteDate");
            _sdate1 = DTRequest.GetString("txtsDate1");
            _edate1 = DTRequest.GetString("txteDate1");
            _status = DTRequest.GetString("ddlstatus");
            _sign = DTRequest.GetString("ddlsign");
            _money1 = DTRequest.GetString("txtMoney1");
            _tag = DTRequest.GetString("tag");
            _self = DTRequest.GetString("self");
            _lockstatus = DTRequest.GetString("ddllock");
            _area = DTRequest.GetString("ddlarea");
            _person1 = DTRequest.GetString("txtPerson1").ToUpper();
            if (_tag == "1")
            {
                _type = "True";
                ddltype.Enabled = false;
                _sign = ">";
                _money1 = "0";
            }
            else if (_tag == "0")
            {
                _type = "False";
                ddltype.Enabled = false;
                _sign = "<";
                _money1 = "0";
            }
            manager = GetAdminInfo();
            if (!Page.IsPostBack)
            {
                InitData();
                if (_self != "1")
                {
                    ChkAdminLevel("sys_settlementCustomer", DTEnums.ActionEnum.View.ToString()); //检查权限
                }
                RptBind("1=1 " + CombSqlTxt(), "isnull(fin_type,rp_type) desc,c_name asc");
            }

        }
        #region 初始化数据=================================
        private void InitData()
        {
            ddltype.DataSource = Common.BusinessDict.financeType();
            ddltype.DataTextField = "value";
            ddltype.DataValueField = "key";
            ddltype.DataBind();
            ddltype.Items.Insert(0, new ListItem("不限", ""));

            ddlstatus.DataSource = Common.BusinessDict.fStatus(1);
            ddlstatus.DataTextField = "value";
            ddlstatus.DataValueField = "key";
            ddlstatus.DataBind();
            ddlstatus.Items.Insert(0, new ListItem("不限", ""));

            ddllock.DataSource = Common.BusinessDict.lockStatus(1);
            ddllock.DataTextField = "value";
            ddllock.DataValueField = "key";
            ddllock.DataBind();
            ddllock.Items.Insert(0, new ListItem("不限", ""));

            string mArea = manager.area == "HQ" ? "" : manager.area;
            ddlarea.DataSource = new BLL.department().getAreaDict(mArea);
            ddlarea.DataTextField = "value";
            ddlarea.DataValueField = "key";
            ddlarea.DataBind();
            if (manager.area == "HQ")
            {
                ddlarea.Items.Insert(0, new ListItem("不限", ""));
            }
            else
            {
                _area = manager.area;
            }
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
            BLL.finance bll = new BLL.finance();
            DataTable dt = null;
            dt = bll.getSettleCustomerDetailListByUser(this.pageSize, this.page, _type, "", "", _sdate, _edate, _sdate1, _edate1, "", "", _status, _sign, _money1, _self == "1" ? manager.user_name : "", _lockstatus, _area, _person1, "op_name asc", out this.totalCount, out money1, out money2, out money3).Tables[0];
            this.rptPersonList.DataSource = dt;
            this.rptPersonList.DataBind();
            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = backUrl();
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            pCount.Text = dt.Rows.Count.ToString();
            decimal _pmoney1 = 0, _pmoney2 = 0, _pmoney3 = 0, _pmoney4 = 0, _pmoney5 = 0, _pmoney6 = 0;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pmoney1 += Utils.ObjToDecimal(dr["orderFinMoney"], 0);
                    _pmoney2 += Utils.ObjToDecimal(dr["orderRpdMoney"], 0);
                    _pmoney3 += Utils.ObjToDecimal(dr["orderUnMoney"], 0);                    
                }
            }
            tCount.Text = totalCount.ToString();
            pMoney1.Text = _pmoney1.ToString();
            pMoney2.Text = _pmoney2.ToString();
            pMoney3.Text = _pmoney3.ToString();

            tMoney1.Text = money1.ToString();
            tMoney2.Text = money2.ToString();
            tMoney3.Text = money3.ToString();

            ddltype.SelectedValue = _type;
            txtsDate.Text = _sdate;
            txteDate.Text = _edate;
            txtsDate1.Text = _sdate1;
            txteDate1.Text = _edate1;
            ddlstatus.SelectedValue = _status;
            ddlsign.SelectedValue = _sign;
            txtMoney1.Text = _money1;
            ddllock.SelectedValue = _lockstatus;
            ddlarea.SelectedValue = _area;
            txtPerson1.Text = _person1;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_type))
            {
                strTemp.Append(" and fin_type='" + _type + "'");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("settleCustomerDetail_page_size", "DTcmsPage"), out _pagesize))
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
            _type = DTRequest.GetFormString("ddltype");
            _sdate = DTRequest.GetFormString("txtsDate");
            _edate = DTRequest.GetFormString("txteDate");
            _sdate1 = DTRequest.GetFormString("txtsDate1");
            _edate1 = DTRequest.GetFormString("txteDate1");
            _status = DTRequest.GetFormString("ddlstatus");
            _sign = DTRequest.GetFormString("ddlsign");
            _money1 = DTRequest.GetFormString("txtMoney1");
            _tag = DTRequest.GetFormString("tag");
            _self = DTRequest.GetFormString("self");
            _lockstatus = DTRequest.GetFormString("ddllock");
            _area = DTRequest.GetFormString("ddlarea");
            _person1 = DTRequest.GetFormString("txtPerson1").ToUpper();
            if (_tag == "1")
            {
                _type = "True";
                ddltype.Enabled = false;
            }
            else if (_tag == "0")
            {
                _type = "False";
                ddltype.Enabled = false;
            }
            RptBind("1=1 " + CombSqlTxt(), "isnull(fin_type,rp_type) desc,c_name asc");
        }
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _type = DTRequest.GetFormString("ddltype");
            _sdate = DTRequest.GetFormString("txtsDate");
            _edate = DTRequest.GetFormString("txteDate");
            _sdate1 = DTRequest.GetFormString("txtsDate1");
            _edate1 = DTRequest.GetFormString("txteDate1");
            _status = DTRequest.GetFormString("ddlstatus");
            _sign = DTRequest.GetFormString("ddlsign");
            _money1 = DTRequest.GetFormString("txtMoney1");
            _tag = DTRequest.GetFormString("tag");
            _self = DTRequest.GetFormString("self");
            _lockstatus = DTRequest.GetFormString("ddllock");
            _area = DTRequest.GetFormString("ddlarea");
            _person1 = DTRequest.GetFormString("txtPerson1").ToUpper();
            BLL.finance bll = new BLL.finance();
            DataTable dt = bll.getSettleCustomerDetailList(this.pageSize, this.page, _type, "", "", _sdate, _edate, _sdate1, _edate1, "", "", _status, _sign, _money1, _self == "1" ? manager.user_name : "", _lockstatus, _area, _person1, "isnull(fin_type,rp_type) desc,c_name asc", out this.totalCount, out money1, out money2, out money3, out money4, out money5, out money6, false).Tables[0];

            string filename = "往来客户明细列表.xlsx";
            if (_tag == "1")
            {
                filename = "未收款列表.xlsx";
            }
            if (_tag == "2")
            {
                filename = "多付款列表.xlsx";
            }

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ""); //HttpUtility.UrlEncode(fileName));
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

            headRow.CreateCell(0).SetCellValue("应收付对象");
            headRow.CreateCell(1).SetCellValue("收付类别");
            headRow.CreateCell(2).SetCellValue("应收付款");
            headRow.CreateCell(3).SetCellValue("订单已收付款");
            headRow.CreateCell(4).SetCellValue("未收付款");
            headRow.CreateCell(5).SetCellValue("已收付款");
            headRow.CreateCell(6).SetCellValue("已分配款");
            headRow.CreateCell(7).SetCellValue("未分配款");

            headRow.GetCell(0).CellStyle = titleCellStyle;
            headRow.GetCell(1).CellStyle = titleCellStyle;
            headRow.GetCell(2).CellStyle = titleCellStyle;
            headRow.GetCell(3).CellStyle = titleCellStyle;
            headRow.GetCell(4).CellStyle = titleCellStyle;
            headRow.GetCell(5).CellStyle = titleCellStyle;
            headRow.GetCell(6).CellStyle = titleCellStyle;
            headRow.GetCell(7).CellStyle = titleCellStyle;

            sheet.SetColumnWidth(0, 15 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 20 * 256);
            sheet.SetColumnWidth(4, 20 * 256);
            sheet.SetColumnWidth(5, 15 * 256);
            sheet.SetColumnWidth(6, 20 * 256);
            sheet.SetColumnWidth(7, 20 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_name"]));
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fin_type"]) == "True" ? "收" : "付");
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["orderFinMoney"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["orderRpdMoney"]));
                    row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["orderUnMoney"]));
                    row.CreateCell(5).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rpmoney"]));
                    row.CreateCell(6).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rpdmoney"]));
                    row.CreateCell(7).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["unmoney"]));

                    row.GetCell(0).CellStyle = cellStyle;
                    row.GetCell(1).CellStyle = cellStyle;
                    row.GetCell(2).CellStyle = cellStyle;
                    row.GetCell(3).CellStyle = cellStyle;
                    row.GetCell(4).CellStyle = cellStyle;
                    row.GetCell(5).CellStyle = cellStyle;
                    row.GetCell(6).CellStyle = cellStyle;
                    row.GetCell(7).CellStyle = cellStyle;
                }
            }

            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }

        private string backUrl()
        {
            return Utils.CombUrlTxt("settleCustomerDetail.aspx", "page={0}&ddltype={1}&txtsDate={2}&txteDate={3}&txtsDate1={4}&txteDate1={5}&ddlstatus={6}&ddlsign={7}&txtMoney1={8}&tag={9}&self={10}&ddllock={11}&ddlarea={12}&txtPerson1={13}", "__id__", _type, _sdate, _edate, _sdate1, _edate1, _status, _sign, _money1, _tag, _self, _lockstatus, _area, _person1);
        }
        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("settleCustomerDetail_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }
    }
}