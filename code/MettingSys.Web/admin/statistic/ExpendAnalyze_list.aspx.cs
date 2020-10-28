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

namespace MettingSys.Web.admin.statistic
{
    public partial class ExpendAnalyze_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        decimal _pFu = 0;
        decimal _tFu = 0;

        protected string action = "", _page = "", _excel = "", _sMonth = "", _eMonth = "", _cusName = "", _cid = "", _area = "", _nature = "", _person1 = "", _person3 = "", _group = "", _lockstatus = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            ChkAdminLevel("sys_RevenueAnalysis", DTEnums.ActionEnum.View.ToString()); //检查权限
            this.pageSize = GetPageSize(10); //每页数量
            _page = DTRequest.GetString("page");
            _sMonth = DTRequest.GetString("txtsDate");
            _eMonth = DTRequest.GetString("txteDate");
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _area = DTRequest.GetString("hide_place");
            _nature = DTRequest.GetString("hide_nature");
            _person1 = DTRequest.GetString("hide_employee1");
            _person3 = DTRequest.GetString("hide_employee3");
            _group = DTRequest.GetString("ddlGroup");
            _excel = DTRequest.GetString("Excel");
            action = DTRequest.GetString("action");
            _lockstatus = DTRequest.GetString("ddllock");
            InitData();
            if (!IsPostBack && _excel != "on" && string.IsNullOrEmpty(_page))
            {
                InitData();
                _lockstatus = "1";
                _sMonth = DateTime.Now.ToString("yyyy-MM");
                _eMonth = DateTime.Now.ToString("yyyy-MM");
                RptBind();
            }
            if (action == "Search")
            {
                RptBind();
            }
            if (_excel == "on")
            {
                Excel();
            }

            #region 绑定控件值
            txtsDate.Text = _sMonth;
            txteDate.Text = _eMonth;
            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            ddlGroup.SelectedValue = _group;
            ddllock.SelectedValue = _lockstatus;
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
            if (!string.IsNullOrEmpty(_nature))
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("na_id");
                dt.Columns.Add("na_name");
                string[] list = _nature.Split(',');
                foreach (string item in list)
                {
                    DataRow dr = dt.NewRow();
                    string[] lis = item.Split('|');
                    dr["na_id"] = lis[0];
                    dr["na_name"] = lis[1];
                    dt.Rows.Add(dr);
                }
                rptNatureList.DataSource = dt;
                rptNatureList.DataBind();
            }
            else
            {
                rptNatureList.DataSource = null;
                rptNatureList.DataBind();
            }
            if (!string.IsNullOrEmpty(_person1))
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("op_name");
                dt.Columns.Add("op_number");
                dt.Columns.Add("op_area");
                string[] list = _person1.Split(',');
                foreach (string item in list)
                {
                    DataRow dr = dt.NewRow();
                    string[] lis = item.Split('|');
                    dr["op_name"] = lis[0];
                    dr["op_number"] = lis[1];
                    dr["op_area"] = lis[2];
                    dt.Rows.Add(dr);
                }
                rptEmployee1.DataSource = dt;
                rptEmployee1.DataBind();
            }
            else
            {
                rptEmployee1.DataSource = null;
                rptEmployee1.DataBind();
            }
            if (!string.IsNullOrEmpty(_person3))
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("op_name");
                dt.Columns.Add("op_number");
                dt.Columns.Add("op_area");
                string[] list = _person3.Split(',');
                foreach (string item in list)
                {
                    DataRow dr = dt.NewRow();
                    string[] lis = item.Split('|');
                    dr["op_name"] = lis[0];
                    dr["op_number"] = lis[1];
                    dr["op_area"] = lis[2];
                    dt.Rows.Add(dr);
                }
                rptEmployee3.DataSource = dt;
                rptEmployee3.DataBind();
            }
            else
            {
                rptEmployee3.DataSource = null;
                rptEmployee3.DataBind();
            }
            #endregion
        }
        #region 初始化数据=================================
        private void InitData()
        {
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
            if (!string.IsNullOrEmpty(_cusName) || !string.IsNullOrEmpty(_cid))
            {
                dict.Add("cusname", _cusName);
                dict.Add("cid", _cid);
            }
            if (!string.IsNullOrEmpty(_area))
            {
                dict.Add("area", _area);
            }
            if (!string.IsNullOrEmpty(_lockstatus))
            {
                dict.Add("lockstatus", _lockstatus);
            }

            if (!string.IsNullOrEmpty(_nature))
            {
                string[] list = _nature.Split(',');
                string pStr = string.Empty;
                foreach (string item in list)
                {
                    string[] litem = item.Split('|');
                    pStr += litem[0] + ",";
                }
                dict.Add("nature", pStr.TrimEnd(','));
            }
            if (!string.IsNullOrEmpty(_person1))
            {
                string[] list = _person1.Split(',');
                string pStr = string.Empty;
                foreach (string item in list)
                {
                    string[] litem = item.Split('|');
                    pStr += litem[1] + ",";
                }
                dict.Add("person1", pStr.TrimEnd(','));
            }
            if (!string.IsNullOrEmpty(_person3))
            {
                string[] list = _person3.Split(',');
                string pStr = string.Empty;
                foreach (string item in list)
                {
                    string[] litem = item.Split('|');
                    pStr += litem[1] + ",";
                }
                dict.Add("person3", pStr.TrimEnd(','));
            }
            return dict;
        }

        #region 数据绑定=================================
        private void RptBind()
        {
            DataTable dt = null;
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.statisticBLL bll = new BLL.statisticBLL();
            Dictionary<string, string> dict = getDict();
            if (string.IsNullOrEmpty(_group))
            {
                dt = bll.getExpendAnalyzeData(dict, this.pageSize, this.page, "o_id asc", out this.totalCount, out _tFu).Tables[0];
                rptList.DataSource = dt;
                rptList.DataBind();
                rptList.Visible = true;
                rptList1.Visible = false;
                rptList2.Visible = false;
                rptList3.Visible = false;
                rptList4.Visible = false;
            }
            else
            {
                switch (_group)
                {
                    case "1"://供应商
                        dt = bll.getExpendAnalyzeData1(dict, this.pageSize, this.page, "c_name,c_type,c_business,op_area,na_name", "c_name,c_type,c_business,op_area,na_name", "c_name asc", out this.totalCount, out _tFu).Tables[0];
                        rptList1.DataSource = dt;
                        rptList1.DataBind();
                        rptList.Visible = false;
                        rptList1.Visible = true;
                        rptList2.Visible = false;
                        rptList3.Visible = false;
                        rptList4.Visible = false;
                        break;
                    case "2"://区域
                        dt = bll.getExpendAnalyzeData1(dict, this.pageSize, this.page, "op_area,na_name", "op_area,na_name", "op_area asc", out this.totalCount,out _tFu).Tables[0];
                        rptList2.DataSource = dt;
                        rptList2.DataBind();
                        rptList.Visible = false;
                        rptList1.Visible = false;
                        rptList2.Visible = true;
                        rptList3.Visible = false;
                        rptList4.Visible = false;
                        break;
                    case "3"://月份
                        dt = bll.getExpendAnalyzeData1(dict, this.pageSize, this.page, "datepart(year,o_edate) oYear,datepart(month,o_edate) oMonth,na_name", "datepart(year,o_edate),datepart(month,o_edate),na_name", "datepart(year,o_edate) asc,datepart(month,o_edate) asc", out this.totalCount,out _tFu).Tables[0];
                        rptList3.DataSource = dt;
                        rptList3.DataBind();
                        rptList.Visible = false;
                        rptList1.Visible = false;
                        rptList2.Visible = false;
                        rptList3.Visible = true;
                        rptList4.Visible = false;
                        break;
                    case "4"://业务性质
                        dt = bll.getExpendAnalyzeData1(dict, this.pageSize, this.page, "na_name,na_sort,fin_detail", "na_name,na_sort,fin_detail", "na_sort asc", out this.totalCount,out _tFu).Tables[0];
                        rptList4.DataSource = dt;
                        rptList4.DataBind();
                        rptList.Visible = false;
                        rptList1.Visible = false;
                        rptList2.Visible = false;
                        rptList3.Visible = false;
                        rptList4.Visible = true;
                        break;
                    default:
                        break;
                }
            }

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pFu += Utils.ObjToDecimal(dr["fu"], 0);
                }
            }

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("ExpendAnalyze_list.aspx", "page={0}&txtsDate={1}&txteDate={2}&action={3}&hide_place={4}&hide_nature={5}&txtCusName={6}&hCusId={7}&hide_employee1={8}&hide_employee3={9}&ddlGroup={10}&ddllock={11}", "__id__", _sMonth, _eMonth, action, _area, _nature, _cusName, _cid, _person1, _person3, _group,_lockstatus);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            pCount.Text = dt.Rows.Count.ToString();
            pFu.Text = _pFu.ToString();

            tCount.Text = totalCount.ToString();
            tFu.Text = _tFu.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("ExpendAnalyze_page_size", "DTcmsPage"), out _pagesize))
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
                    Utils.WriteCookie("ExpendAnalyze_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("ExpendAnalyze_list.aspx", "page={0}&txtsDate={1}&txteDate={2}&action={3}&hide_place={4}&hide_nature={5}&txtCusName={6}&hCusId={7}&hide_employee1={8}&hide_employee3={9}&ddlGroup={10}", "__id__", _sMonth, _eMonth, action, _area, _nature, _cusName, _cid, _person1, _person3, _group));
        }
        protected void Excel()
        {           
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

            string fileName = "";
            string[] strFieldsName = { };
            string[] strFields = new string[] { };
            DataTable dt = null;
            BLL.statisticBLL bll = new BLL.statisticBLL();
            Dictionary<string, string> dict = getDict();
            if (string.IsNullOrEmpty(_group))
            {
                #region
                fileName = "供应商支出分析-明细列表";                
                dt = bll.getExpendAnalyzeData(dict, this.pageSize, this.page, "o_id asc", out this.totalCount, out _tFu, false).Tables[0];

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename="+ fileName + ".xlsx"); //HttpUtility.UrlEncode(fileName));
                HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                headRow.CreateCell(0).SetCellValue("订单号");
                headRow.CreateCell(1).SetCellValue("供应商");
                headRow.CreateCell(2).SetCellValue("活动名称");
                headRow.CreateCell(3).SetCellValue("活动地点");
                headRow.CreateCell(4).SetCellValue("活动结束日期");
                headRow.CreateCell(5).SetCellValue("业务性质");
                headRow.CreateCell(6).SetCellValue("业务明细");
                headRow.CreateCell(7).SetCellValue("应付金额");
                headRow.CreateCell(8).SetCellValue("区域");
                headRow.CreateCell(9).SetCellValue("业务员");
                headRow.CreateCell(10).SetCellValue("地接添加人");

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
                sheet.SetColumnWidth(3, 30 * 256);
                sheet.SetColumnWidth(4, 20 * 256);
                sheet.SetColumnWidth(5, 20 * 256);
                sheet.SetColumnWidth(6, 15 * 256);
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
                        row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["o_content"]));
                        row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["o_address"]));
                        row.CreateCell(4).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["o_edate"]).Value.ToString("yyyy-mm-dd"));
                        row.CreateCell(5).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["na_name"]));
                        row.CreateCell(6).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fin_detail"]));
                        row.CreateCell(7).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fu"]));
                        row.CreateCell(8).SetCellValue(new MettingSys.BLL.department().getAreaText(Utils.ObjectToStr(dt.Rows[i]["op_area"])));
                        row.CreateCell(9).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["op_name"]));
                        row.CreateCell(10).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fin_personName"]));

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
                #endregion
            }
            else
            {
                switch (_group)
                {
                    case "1"://供应商
                        #region
                        fileName = "供应商支出分析-供应商分组";
                        dt = bll.getExpendAnalyzeData1(dict, this.pageSize, this.page, "c_name,c_type,c_business,op_area,na_name", "c_name,c_type,c_business,op_area,na_name", "c_name asc", out this.totalCount, out _tFu,false).Tables[0];
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".xlsx"); //HttpUtility.UrlEncode(fileName));
                        HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                        headRow.CreateCell(0).SetCellValue("供应商");
                        headRow.CreateCell(1).SetCellValue("客户类别");
                        headRow.CreateCell(2).SetCellValue("业务范围");
                        headRow.CreateCell(3).SetCellValue("区域");
                        headRow.CreateCell(4).SetCellValue("业务性质");
                        headRow.CreateCell(5).SetCellValue("应付金额");

                        headRow.GetCell(0).CellStyle = titleCellStyle;
                        headRow.GetCell(1).CellStyle = titleCellStyle;
                        headRow.GetCell(2).CellStyle = titleCellStyle;
                        headRow.GetCell(3).CellStyle = titleCellStyle;
                        headRow.GetCell(4).CellStyle = titleCellStyle;
                        headRow.GetCell(5).CellStyle = titleCellStyle;

                        sheet.SetColumnWidth(0, 15 * 256);
                        sheet.SetColumnWidth(1, 20 * 256);
                        sheet.SetColumnWidth(2, 20 * 256);
                        sheet.SetColumnWidth(3, 30 * 256);
                        sheet.SetColumnWidth(4, 20 * 256);
                        sheet.SetColumnWidth(5, 20 * 256);

                        if (dt != null)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                IRow row = sheet.CreateRow(i + 1);
                                row.HeightInPoints = 22;
                                row.CreateCell(0).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_name"]));
                                row.CreateCell(1).SetCellValue(MettingSys.Common.BusinessDict.customerType()[Utils.ObjToByte(dt.Rows[i]["c_type"])]);
                                row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_business"]));
                                row.CreateCell(3).SetCellValue(new MettingSys.BLL.department().getAreaText(Utils.ObjectToStr(dt.Rows[i]["op_area"])));
                                row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["na_name"]));
                                row.CreateCell(5).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fu"]));

                                row.GetCell(0).CellStyle = cellStyle;
                                row.GetCell(1).CellStyle = cellStyle;
                                row.GetCell(2).CellStyle = cellStyle;
                                row.GetCell(3).CellStyle = cellStyle;
                                row.GetCell(4).CellStyle = cellStyle;
                                row.GetCell(5).CellStyle = cellStyle;
                            }
                        }
                        #endregion
                        break;
                    case "2"://区域
                        #region
                        fileName = "供应商支出分析-区域分组";
                        dt = bll.getExpendAnalyzeData1(dict, this.pageSize, this.page, "op_area,na_name", "op_area,na_name", "op_area asc", out this.totalCount, out _tFu,false).Tables[0];
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".xlsx"); //HttpUtility.UrlEncode(fileName));
                        HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                        headRow.CreateCell(0).SetCellValue("区域");
                        headRow.CreateCell(1).SetCellValue("业务性质");
                        headRow.CreateCell(2).SetCellValue("应付金额");

                        headRow.GetCell(0).CellStyle = titleCellStyle;
                        headRow.GetCell(1).CellStyle = titleCellStyle;
                        headRow.GetCell(2).CellStyle = titleCellStyle;

                        sheet.SetColumnWidth(0, 15 * 256);
                        sheet.SetColumnWidth(1, 20 * 256);
                        sheet.SetColumnWidth(2, 20 * 256);

                        if (dt != null)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                IRow row = sheet.CreateRow(i + 1);
                                row.HeightInPoints = 22;
                                row.CreateCell(0).SetCellValue(new MettingSys.BLL.department().getAreaText(Utils.ObjectToStr(dt.Rows[i]["op_area"])));
                                row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["na_name"]));
                                row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fu"]));

                                row.GetCell(0).CellStyle = cellStyle;
                                row.GetCell(1).CellStyle = cellStyle;
                                row.GetCell(2).CellStyle = cellStyle;
                            }
                        }
                        #endregion
                        break;
                    case "3"://月份
                        #region
                        fileName = "供应商支出分析-月份分组";
                        dt = bll.getExpendAnalyzeData1(dict, this.pageSize, this.page, "datepart(year,o_edate) oYear,datepart(month,o_edate) oMonth,na_name", "datepart(year,o_edate),datepart(month,o_edate),na_name", "datepart(year,o_edate) asc,datepart(month,o_edate) asc", out this.totalCount, out _tFu, false).Tables[0];
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".xlsx"); //HttpUtility.UrlEncode(fileName));
                        HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                        headRow.CreateCell(0).SetCellValue("月份");
                        headRow.CreateCell(1).SetCellValue("业务性质");
                        headRow.CreateCell(2).SetCellValue("应付金额");

                        headRow.GetCell(0).CellStyle = titleCellStyle;
                        headRow.GetCell(1).CellStyle = titleCellStyle;
                        headRow.GetCell(2).CellStyle = titleCellStyle;

                        sheet.SetColumnWidth(0, 15 * 256);
                        sheet.SetColumnWidth(1, 20 * 256);
                        sheet.SetColumnWidth(2, 20 * 256);

                        if (dt != null)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                IRow row = sheet.CreateRow(i + 1);
                                row.HeightInPoints = 22;
                                row.CreateCell(0).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["oYear"])+"/"+ Utils.ObjectToStr(dt.Rows[i]["oMonth"]));
                                row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["na_name"]));
                                row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fu"]));

                                row.GetCell(0).CellStyle = cellStyle;
                                row.GetCell(1).CellStyle = cellStyle;
                                row.GetCell(2).CellStyle = cellStyle;
                            }
                        }
                        #endregion
                        break;
                    case "4"://业务性质
                        #region
                        fileName = "供应商支出分析-业务性质分组";
                        dt = bll.getExpendAnalyzeData1(dict, this.pageSize, this.page, "na_name,na_sort,fin_detail", "na_name,na_sort,fin_detail", "na_sort asc", out this.totalCount, out _tFu, false).Tables[0];
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".xlsx"); //HttpUtility.UrlEncode(fileName));
                        HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                        headRow.CreateCell(0).SetCellValue("业务性质");
                        headRow.CreateCell(1).SetCellValue("业务明细");
                        headRow.CreateCell(2).SetCellValue("应付金额");

                        headRow.GetCell(0).CellStyle = titleCellStyle;
                        headRow.GetCell(1).CellStyle = titleCellStyle;
                        headRow.GetCell(2).CellStyle = titleCellStyle;

                        sheet.SetColumnWidth(0, 15 * 256);
                        sheet.SetColumnWidth(1, 20 * 256);
                        sheet.SetColumnWidth(2, 20 * 256);

                        if (dt != null)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                IRow row = sheet.CreateRow(i + 1);
                                row.HeightInPoints = 22;
                                row.CreateCell(0).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["na_name"]));
                                row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fin_detail"]));
                                row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fu"]));

                                row.GetCell(0).CellStyle = cellStyle;
                                row.GetCell(1).CellStyle = cellStyle;
                                row.GetCell(2).CellStyle = cellStyle;
                            }
                        }
                        #endregion
                        break;
                    default:
                        break;
                }
            }
            //ExcelHelper.Write(HttpContext.Current, dt, fileName, fileName, strFields, strFieldsName, string.Format("{0}.xlsx", fileName));
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }
    }
}