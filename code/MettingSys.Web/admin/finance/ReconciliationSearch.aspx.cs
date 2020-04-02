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
    public partial class ReconciliationSearch : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小
        protected string _orderid = "", _cusName = "", _cid = "", _type = "", _num = "";

        decimal _pMoney = 0, _tMoney = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            _orderid = DTRequest.GetString("txtOrderID");
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _type = DTRequest.GetString("ddltype");
            _num = DTRequest.GetString("txtNum");
            
            this.pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                InitData();
                ChkAdminLevel("sys_ReconciliationSearch", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("1=1" + CombSqlTxt(), "fc_addDate desc,fin_adddate desc");
            }
        }

        #region 初始化
        private void InitData()
        {
            ddltype.DataSource = Common.BusinessDict.financeType();
            ddltype.DataTextField = "value";
            ddltype.DataValueField = "key";
            ddltype.DataBind();
            ddltype.Items.Insert(0, new ListItem("不限", ""));
        }
        #endregion

        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.finance_chk bll = new BLL.finance_chk();
            DataTable dt= bll.GetList(this.pageSize, this.page, _strWhere, _orderby, out this.totalCount, out _tMoney).Tables[0];
            this.rptList.DataSource = dt;
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("ReconciliationSearch.aspx", "page={0}&txtOrderID={1}&txtCusName={2}&hCusId={3}&ddltype={4}&txtNum={5}", "__id__", _orderid, _cusName, _cid, _type, _num);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pMoney += Utils.ObjToDecimal(dr["fc_money"], 0);
                }
            }

            pCount.Text = dt.Rows.Count.ToString();
            pMoney.Text = _pMoney.ToString();

            tCount.Text = totalCount.ToString();
            tMoney.Text = _tMoney.ToString();

            txtOrderID.Text = _orderid;
            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            ddltype.SelectedValue = _type;
            txtNum.Text = _num;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                strTemp.Append(" and fin_cid=" + _cid + "");
            }
            if (!string.IsNullOrEmpty(_orderid))
            {
                strTemp.Append(" and o_id='" + _orderid + "'");
            }
            if (!string.IsNullOrEmpty(_type))
            {
                strTemp.Append(" and fin_type='" + _type + "'");
            }
            if (!string.IsNullOrEmpty(_num))
            {
                strTemp.Append(" and fc_num='" + _num + "'");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("ReconciliationSearch_page_size", "DTcmsPage"), out _pagesize))
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
            _orderid = DTRequest.GetFormString("txtOrderID");
            _cusName = DTRequest.GetFormString("txtCusName");
            _cid = DTRequest.GetFormString("hCusId");
            _type = DTRequest.GetFormString("ddltype");
            _num = DTRequest.GetFormString("txtNum");
            RptBind("1=1" + CombSqlTxt(), "o_addDate desc,o_id desc");
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            //var fileName = "对账查询";
            //string[] strFieldsName = { "订单号", "应收付对象","客户", "活动日期", "活动地点", "活动名称",  "收付性质", "业务性质", "业务明细", "对账标识", "对账金额" };
            //string[] strFields = { "o_id","c_name","cname", "o_sdate/o_edate", "o_address", "o_content",  "fin_type", "na_name", "fin_detail", "fc_num", "fc_money" };
            //DataTable dt = new BLL.finance_chk().GetList(0,0, "1=1" + CombSqlTxt(), "fc_addDate desc,fin_adddate desc", out totalCount, out _tMoney, false).Tables[0];
            //ExcelHelper.Write(HttpContext.Current, dt, fileName, fileName, strFields, strFieldsName, string.Format("{0}.xlsx", fileName));

            DataTable dt = new BLL.finance_chk().GetList(0, 0, "1=1" + CombSqlTxt(), "fc_addDate desc,fin_adddate desc", out totalCount, out _tMoney, false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=对账查询列表.xlsx"); //HttpUtility.UrlEncode(fileName));
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
            headRow.CreateCell(1).SetCellValue("应收付对象");
            headRow.CreateCell(2).SetCellValue("客户");
            headRow.CreateCell(3).SetCellValue("活动日期");
            headRow.CreateCell(4).SetCellValue("活动地点");
            headRow.CreateCell(5).SetCellValue("活动名称");
            headRow.CreateCell(6).SetCellValue("收付性质");
            headRow.CreateCell(7).SetCellValue("业务性质");
            headRow.CreateCell(8).SetCellValue("业务明细");
            headRow.CreateCell(9).SetCellValue("对账标识");
            headRow.CreateCell(10).SetCellValue("对账金额");

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

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["o_id"].ToString());
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_name"]));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["cname"]));
                    row.CreateCell(3).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["o_sdate"]).Value.ToString("yyyy-MM-dd") + "/" + ConvertHelper.toDate(dt.Rows[i]["o_edate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["o_address"]));
                    row.CreateCell(5).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["o_content"]));
                    row.CreateCell(6).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fin_type"]));
                    row.CreateCell(7).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["na_name"]));
                    row.CreateCell(8).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fin_detail"]));
                    row.CreateCell(9).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fc_num"]));
                    row.CreateCell(10).SetCellValue(dt.Rows[i]["fc_money"].ToString());

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
                }
            }

            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }
        //设置分页数量 
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("ReconciliationSearch_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("ReconciliationSearch.aspx", "page={0}&txtOrderID={1}&txtCusName={2}&hCusId={3}&ddltype={4}&txtNum={5}", "__id__", _orderid, _cusName, _cid, _type, _num));
        }


    }
}