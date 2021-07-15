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
    public partial class AreaAchievementStatistic_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小
        int _pOrderCount = 0, _tOrderCount = 0;
        decimal _pShou = 0, _pUnIncome = 0, _pFu = 0, _pUnCost = 0, _pCust = 0, _pTicheng = 0, _pProfit1 = 0, _pProfit2 = 0;
        decimal _tShou = 0, _tUnIncome = 0, _tFu = 0, _tUnCost = 0, _tCust = 0, _tTicheng = 0, _tProfit1 = 0, _tProfit2 = 0;
        Model.manager manager = null;
        protected string action = "", _page = "", _sMonth = "", _eMonth = "", _status = "", _lockstatus = "", _area = "", _isCust = "", _excel="", _ordertype = "", _order = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            this.pageSize = GetPageSize(10); //每页数量            
            _page = DTRequest.GetString("page");
            _sMonth = DTRequest.GetString("txtsDate");
            _eMonth = DTRequest.GetString("txteDate");
            _status = DTRequest.GetString("ddlstatus");
            _lockstatus = DTRequest.GetString("ddllock");
            _area = DTRequest.GetString("hide_place");
            _isCust = DTRequest.GetString("cbIsCust");
            action = DTRequest.GetString("action");
            _excel = DTRequest.GetString("Excel"); 
            _ordertype = DTRequest.GetString("ddlorderType");
            if (string.IsNullOrEmpty(_ordertype))
            {
                _ordertype = "(shou-fu-oCust)";
            }
            _order = DTRequest.GetString("ddlorder");
            if (string.IsNullOrEmpty(_order))
            {
                _order = "Desc";
            }
            ChkAdminLevel("sys_AreaAchievementStatistics", DTEnums.ActionEnum.View.ToString()); //检查权限
            if (!IsPostBack && _excel != "on" && string.IsNullOrEmpty(_page))
            {
                _sMonth = DateTime.Now.ToString("yyyy-MM");
                _eMonth = DateTime.Now.ToString("yyyy-MM");
                _status = "3";
                _lockstatus = "1";
                _isCust = "on";
                RptBind("1=1 " + CombSqlTxt(), _ordertype + " " + _order);
            }
            InitData();
            if (action == "Search")
            {
                RptBind("1=1 " + CombSqlTxt(), _ordertype + " " + _order);
            }
            if (_excel == "on")
            {
                Excel();
            }

            #region 绑定控件值
            txtsDate.Text = _sMonth;
            txteDate.Text = _eMonth;
            ddlstatus.SelectedValue = _status;
            ddllock.SelectedValue = _lockstatus;
            cbIsCust.Checked = _isCust == "on" ? true : false;
            ddlorderType.SelectedValue = _ordertype;
            ddlorder.SelectedValue = _order;
            string placeStr = _area;
            if (!string.IsNullOrEmpty(placeStr))
            {
                Dictionary<string, string> areaDic = new BLL.department().getAreaDict();
                Dictionary<string, string> orderAreaDic = new Dictionary<string, string>();
                string[] list = placeStr.Split(',');
                foreach (string item in list)
                {
                    if (areaDic.ContainsKey(item))
                    {
                        orderAreaDic.Add(item, areaDic[item]);
                    }
                }
                rptAreaList.DataSource = orderAreaDic;
                rptAreaList.DataBind();
            }
            else
            {
                rptAreaList.DataSource = null;
                rptAreaList.DataBind();
            }
            #endregion
        }
        #region 初始化数据=================================
        private void InitData()
        {
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

        }
        #endregion

        private Dictionary<string, string> getDict()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(_sMonth))
            {
                dict.Add("smonth", _sMonth);
            }
            if (!string.IsNullOrEmpty(_eMonth))
            {
                dict.Add("emonth", _eMonth);
            }
            if (!string.IsNullOrEmpty(_status))
            {
                dict.Add("status", _status);
            }
            if (!string.IsNullOrEmpty(_lockstatus))
            {
                dict.Add("lockstatus", _lockstatus);
            }
            if (!string.IsNullOrEmpty(_area))
            {
                dict.Add("area", _area);
            }
            dict.Add("isCust", _isCust.ToString());
            return dict;
        }

        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.statisticBLL bll = new BLL.statisticBLL();
            Dictionary<string, string> dict = getDict();            
            DataTable dt = bll.getAreaAchievementStatisticData(dict, this.pageSize, this.page, _orderby, out this.totalCount, out _tOrderCount, out _tShou,out _tUnIncome,out _tFu, out _tUnCost, out _tCust,out _tTicheng,out _tProfit1,out _tProfit2).Tables[0];
            rptList.DataSource = dt;
            rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = backUrl();
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pOrderCount += Utils.ObjToInt(dr["oCount"]);
                    _pShou += Utils.ObjToDecimal(dr["shou"], 0);
                    _pUnIncome += Utils.ObjToDecimal(dr["unIncome"], 0);
                    _pFu += Utils.ObjToDecimal(dr["fu"], 0);
                    _pUnCost += Utils.ObjToDecimal(dr["unCost"], 0);
                    _pCust += Utils.ObjToDecimal(dr["oCust"], 0);
                    _pTicheng += Utils.ObjToDecimal(dr["ticheng"], 0);
                    _pProfit1 += Utils.ObjToDecimal(dr["profit1"], 0);
                    _pProfit2 += Utils.ObjToDecimal(dr["profit2"], 0);
                }
            }

            pCount.Text = dt.Rows.Count.ToString();
            pOrderCount.Text = _pOrderCount.ToString();
            pShou.Text = _pShou.ToString();
            pUnIncome.Text = _pUnIncome.ToString();
            pFu.Text = _pFu.ToString();
            pUnCost.Text = _pUnCost.ToString();
            pCust.Text = _pCust.ToString();
            pTicheng.Text = _pTicheng.ToString();
            pProfit1.Text = _pProfit1.ToString();
            pProfit2.Text = _pProfit2.ToString();

            tCount.Text = totalCount.ToString();
            tOrderCount.Text = _tOrderCount.ToString();
            tShou.Text = _tShou.ToString();
            tUnIncome.Text = _tUnIncome.ToString();
            tFu.Text = _tFu.ToString();
            tUnCost.Text = _tUnCost.ToString();
            tCust.Text = _tCust.ToString();
            tTicheng.Text = _tTicheng.ToString();
            tProfit1.Text = _tProfit1.ToString();
            tProfit2.Text = _tProfit2.ToString();
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();

            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("AreaAchievementStatistic_page_size", "DTcmsPage"), out _pagesize))
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
            if (int.TryParse(DTRequest.GetFormString("txtPageNum"), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("AreaAchievementStatistic_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }

        private string backUrl()
        {
            return Utils.CombUrlTxt("AreaAchievementStatistic_list.aspx", "page={0}&txtsDate={1}&txteDate={2}&ddlstatus={3}&ddllock={4}&hide_place={5}&cbIsCust={6}&action={7}&ddlorderType={8}&ddlorder={9}", "__id__", _sMonth, _eMonth, _status, _lockstatus, _area, _isCust, action,_ordertype,_order);
        }

        protected void Excel()
        {
            BLL.statisticBLL bll = new BLL.statisticBLL();
            Dictionary<string, string> dict = getDict();
            DataTable dt = bll.getAreaAchievementStatisticData(dict, this.pageSize, this.page, _ordertype + " " + _order, out this.totalCount, out _tOrderCount, out _tShou,out _tUnIncome, out _tFu,out _tUnCost, out _tCust,out _tTicheng, out _tProfit1,out _tProfit2,false).Tables[0];

            string fileName = "区域业绩统计";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".xlsx"); //HttpUtility.UrlEncode(fileName));
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
            //区域	订单数量	应收	非考核收入	应付	非考核成本	提成	税费	提成前业绩	提成前业绩率	提成后业绩	提成后业绩率
            headRow.CreateCell(0).SetCellValue("区域");
            headRow.CreateCell(1).SetCellValue("订单数量");
            headRow.CreateCell(2).SetCellValue("应收");
            headRow.CreateCell(3).SetCellValue("非考核收入");
            headRow.CreateCell(4).SetCellValue("应付");
            headRow.CreateCell(5).SetCellValue("非考核成本");
            headRow.CreateCell(6).SetCellValue("提成");
            headRow.CreateCell(7).SetCellValue("税费");
            headRow.CreateCell(8).SetCellValue("提成前业绩");
            headRow.CreateCell(9).SetCellValue("提成前业绩率");
            headRow.CreateCell(10).SetCellValue("提成后业绩");
            headRow.CreateCell(11).SetCellValue("提成后业绩率");

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

            sheet.SetColumnWidth(0, 15 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 20 * 256);
            sheet.SetColumnWidth(4, 20 * 256);
            sheet.SetColumnWidth(5, 20 * 256);
            sheet.SetColumnWidth(6, 20 * 256);
            sheet.SetColumnWidth(7, 20 * 256);
            sheet.SetColumnWidth(8, 20 * 256);
            sheet.SetColumnWidth(9, 20 * 256);
            sheet.SetColumnWidth(10, 20 * 256);
            sheet.SetColumnWidth(11, 20 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["p_name"]) + "-" + Utils.ObjectToStr(dt.Rows[i]["p_chnName"]));
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["oCount"]));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["shou"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["unIncome"]));
                    row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fu"]));
                    row.CreateCell(5).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["unCost"]));
                    row.CreateCell(6).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["ticheng"]));
                    row.CreateCell(7).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["oCust"]));
                    row.CreateCell(8).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["profit1"]));
                    row.CreateCell(9).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["profitRatio1"]) + "%");
                    row.CreateCell(10).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["profit2"]));
                    row.CreateCell(11).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["profitRatio2"]) + "%");

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
                }
            }

            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }

    }
}