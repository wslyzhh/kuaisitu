using MettingSys.Common;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BorderStyle = NPOI.SS.UserModel.BorderStyle;

namespace MettingSys.Web.admin.finance
{
    public partial class FinancialOrderDetail : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小
        protected string _tag = "", _lastMonth = "", _smonth = "", _emonth = "", _defaultMonth = "", _type = "", _cid = "", _cusName = "", _area = "";
        BLL.finance bll = new BLL.finance();
        decimal _tmoney1 = 0, _tmoney2 = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            _smonth = DTRequest.GetString("txtsDate");
            _emonth = DTRequest.GetString("txteDate");
            _type = DTRequest.GetString("ddltype");
            _cid = DTRequest.GetString("hCusId");
            _cusName = DTRequest.GetString("txtCusName");
            _area = DTRequest.GetString("ddlarea");
            _tag = DTRequest.GetString("tag");
            this.pageSize = GetPageSize(10); //每页数量
            _lastMonth = bll.getLastFinancialMonth();
            if (string.IsNullOrEmpty(_lastMonth))
            {
                _lastMonth = DateTime.Now.ToString("yyyy-MM");
                _defaultMonth = _lastMonth;
            }
            else
            {
                _defaultMonth = _lastMonth;
                _lastMonth = ConvertHelper.toDate(_lastMonth + "-01").Value.AddMonths(1).ToString("yyyy-MM");
            }
            if (!IsPostBack)
            {
                if (_tag != "0")
                {
                    if (string.IsNullOrEmpty(_smonth))
                    {
                        _smonth = _defaultMonth;
                    }
                    if (string.IsNullOrEmpty(_emonth))
                    {
                        _emonth = _defaultMonth;
                    }
                }
                InitData();
                RptBind();
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

            ddlarea.DataSource = new BLL.department().getAreaDict();
            ddlarea.DataTextField = "value";
            ddlarea.DataValueField = "key";
            ddlarea.DataBind();
            ddlarea.Items.Insert(0, new ListItem("不限", ""));
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            string sqlWhere = "";
            if (!string.IsNullOrEmpty(_smonth))
            {
                sqlWhere += " and datediff(MONTH,fin_month+'-01','" + _smonth + "-01')<=0";
            }
            if (!string.IsNullOrEmpty(_emonth))
            {
                sqlWhere += " and datediff(MONTH,fin_month+'-01','" + _emonth + "-01')>=0";
            }
            if (!string.IsNullOrEmpty(_type))
            {
                sqlWhere += " and fin_type='" + _type + "'";
            }
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                sqlWhere += " and fin_cid=" + _cid + "";
            }
            if (!string.IsNullOrEmpty(_area))
            {
                sqlWhere += " and fin_area='" + _area + "'";
            }
            return sqlWhere;
        }
        #endregion

        #region 数据绑定=================================
        private void RptBind()
        {
            if (!this.isSearch)
            {
                this.page = DTRequest.GetQueryInt("page", 1);
            }
            else
            {
                this.page = 1;
            }
            DataTable dt = bll.getFinancialOrder(this.pageSize, this.page, CombSqlTxt(), "fin_oid asc", out this.totalCount, out _tmoney1, out _tmoney2).Tables[0];
            this.rptList.DataSource = dt;
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("FinancialOrderDetail.aspx", "txtsDate={0}&txteDate={1}&ddltype={2}&hCusId={3}&page={4}&ddlarea={5}", _smonth, _emonth, _type, _cid, "__id__",_area);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            pCount.Text = dt.Rows.Count.ToString();
            decimal _money1 = 0, _money2 = 0;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["fin_type"].ToString() == "True")
                    {
                        _money1 += Utils.ObjToDecimal(dr["fin_money"], 0);
                    }
                    else
                    {
                        _money2 += Utils.ObjToDecimal(dr["fin_money"], 0);
                    }
                }
            }
            pMoney1.Text = _money1.ToString();
            pMoney2.Text = _money2.ToString();
            tCount.Text = totalCount.ToString();
            tMoney1.Text = _tmoney1.ToString();
            tMoney2.Text = _tmoney2.ToString();

            txtsDate.Text = _smonth;
            txteDate.Text = _emonth;
            ddltype.SelectedValue = _type;
            hCusId.Value = _cid;
            txtCusName.Text = _cusName;
            ddlarea.SelectedValue = _area;
        }
        #endregion
        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("FinancialOrderDetail_page_size", "DTcmsPage"), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    return _pagesize;
                }
            }
            return _default_size;
        }
        #endregion
        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("FinancialOrderDetail_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("FinancialOrderDetail.aspx", "txtsDate={0}&txteDate={1}&ddltype={2}&hCusId={3}&page={4}&ddlarea={5}", _smonth, _emonth, _type, _cid, "__id__",_area));
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.isSearch = true;
            _smonth = DTRequest.GetFormString("txtsDate");
            _emonth = DTRequest.GetFormString("txteDate");
            _type = DTRequest.GetFormString("ddltype");
            _cid = DTRequest.GetFormString("hCusId");
            _area = DTRequest.GetFormString("ddlarea");
            RptBind();
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _smonth = DTRequest.GetFormString("txtsDate");
            _emonth = DTRequest.GetFormString("txteDate");
            _type = DTRequest.GetFormString("ddltype");
            _cid = DTRequest.GetFormString("hCusId");
            _area = DTRequest.GetFormString("ddlarea");
            DataTable dt = bll.getFinancialOrder(this.pageSize, this.page, CombSqlTxt(), "fin_oid asc", out this.totalCount, out _tmoney1, out _tmoney2, false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=应收付对象已结账订单地接明细.xlsx"); //HttpUtility.UrlEncode(fileName));
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

            headRow.CreateCell(0).SetCellValue("订单号");
            headRow.CreateCell(1).SetCellValue("活动名称");
            headRow.CreateCell(2).SetCellValue("活动日期");
            headRow.CreateCell(3).SetCellValue("活动地点");
            headRow.CreateCell(4).SetCellValue("业务日期");
            headRow.CreateCell(5).SetCellValue("业务性质/明细");
            headRow.CreateCell(6).SetCellValue("业务说明");
            headRow.CreateCell(7).SetCellValue("收付类别");
            headRow.CreateCell(8).SetCellValue("表达式");
            headRow.CreateCell(9).SetCellValue("应收付金额");

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

            sheet.SetColumnWidth(0, 15 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 20 * 256);
            sheet.SetColumnWidth(4, 20 * 256);
            sheet.SetColumnWidth(5, 20 * 256);
            sheet.SetColumnWidth(6, 20 * 256);
            sheet.SetColumnWidth(7, 10 * 256);
            sheet.SetColumnWidth(8, 15 * 256);
            sheet.SetColumnWidth(9, 10 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["o_id"].ToString());
                    row.CreateCell(1).SetCellValue(dt.Rows[i]["o_content"].ToString());
                    row.CreateCell(2).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["o_sdate"]).Value.ToString("yyyy-MM-dd")+"/"+ ConvertHelper.toDate(dt.Rows[i]["o_edate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(3).SetCellValue(dt.Rows[i]["o_address"].ToString());
                    row.CreateCell(4).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["fin_sdate"]).Value.ToString("yyyy-MM-dd") + "/" + ConvertHelper.toDate(dt.Rows[i]["fin_edate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(5).SetCellValue(dt.Rows[i]["na_name"].ToString() + "/" + dt.Rows[i]["fin_detail"].ToString());
                    row.CreateCell(6).SetCellValue(dt.Rows[i]["fin_illustration"].ToString());
                    row.CreateCell(7).SetCellValue(dt.Rows[i]["fin_type"].ToString() == "True" ? "收" : "付");
                    row.CreateCell(8).SetCellValue(dt.Rows[i]["fin_expression"].ToString());
                    row.CreateCell(9).SetCellValue(dt.Rows[i]["fin_money"].ToString());

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
                }
            }

            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }
    }
}