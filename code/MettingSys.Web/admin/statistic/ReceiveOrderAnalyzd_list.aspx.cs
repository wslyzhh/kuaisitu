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
    public partial class ReceiveOrderAnalyzd_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string _sdate = "", _edate = "",_person="",_depart="", _status = "", _dstatus = "", _lockstatus = "", _area = "", _orderarea = "";
        private Model.manager manager = null;
        int _tcount3 = 0, _tcount5 = 0, _tcount = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            _sdate = DTRequest.GetString("txtsDate");
            _edate = DTRequest.GetString("txteDate");
            _person = DTRequest.GetString("txtPerson");
            _depart = DTRequest.GetString("txtDepart");
            _status = DTRequest.GetString("ddlstatus");
            _dstatus = DTRequest.GetString("ddldstatus");
            _lockstatus = DTRequest.GetString("ddllock");
            _area = DTRequest.GetString("ddlarea");
            _orderarea = DTRequest.GetString("ddlorderarea");
            pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                if (string.IsNullOrEmpty(DTRequest.GetString("page")))
                {
                    _dstatus = "5";
                }
                InitData();
                ChkAdminLevel("sys_ReceiveOrderAnalyze", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind(CombSqlTxt(), "op_number asc");
            }
        }

        #region 初始化
        private void InitData()
        {
            ddlstatus.DataSource = Common.BusinessDict.fStatus(1);
            ddlstatus.DataTextField = "value";
            ddlstatus.DataValueField = "key";
            ddlstatus.DataBind();
            ddlstatus.Items.Insert(0, new ListItem("不限", ""));

            ddldstatus.DataSource = Common.BusinessDict.dStatus(3);
            ddldstatus.DataTextField = "value";
            ddldstatus.DataValueField = "key";
            ddldstatus.DataBind();
            ddldstatus.Items.Insert(0, new ListItem("不限", ""));

            ddllock.DataSource = Common.BusinessDict.lockStatus(1);
            ddllock.DataTextField = "value";
            ddllock.DataValueField = "key";
            ddllock.DataBind();
            ddllock.Items.Insert(0, new ListItem("不限", ""));

            ddlarea.DataSource = new BLL.department().getAreaDict();
            ddlarea.DataTextField = "value";
            ddlarea.DataValueField = "key";
            ddlarea.DataBind();
            ddlarea.Items.Insert(0, new ListItem("不限", ""));

            ddlorderarea.DataSource = new BLL.department().getAreaDict();
            ddlorderarea.DataTextField = "value";
            ddlorderarea.DataValueField = "key";
            ddlorderarea.DataBind();
            ddlorderarea.Items.Insert(0, new ListItem("不限", ""));
        }
        #endregion

        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.statisticBLL bll = new BLL.statisticBLL();
            DataTable dt = bll.getReceiveOrderAnalyzeData(this.pageSize, this.page, _strWhere, _orderby,out _tcount3,out _tcount5,out _tcount, out this.totalCount).Tables[0];
            this.rptList.DataSource = dt;
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = backUrl();
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);
            int _pcount3 = 0, _pcount5 = 0, _pcount = 0;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pcount3 += Utils.ObjToInt(dr["type3"], 0);
                    _pcount5 += Utils.ObjToInt(dr["type5"], 0);
                    _pcount += Utils.ObjToInt(dr["sumType"], 0);
                }
            }
            pCount.Text = dt.Rows.Count.ToString();
            pOrder3Count.Text = _pcount3.ToString();
            pOrder5Count.Text = _pcount5.ToString();
            pOrderCount.Text = _pcount.ToString();

            tCount.Text = this.totalCount.ToString();
            tOrder3Count.Text = _tcount3.ToString();
            tOrder5Count.Text = _tcount5.ToString();
            tOrderCount.Text = _tcount.ToString();

            txtsDate.Text = _sdate;
            txteDate.Text = _edate;
            txtPerson.Text = _person;
            txtDepart.Text = _depart;
            ddlstatus.SelectedValue = _status;
            ddldstatus.SelectedValue = _dstatus;
            ddllock.SelectedValue = _lockstatus;
            ddlarea.SelectedValue = _area;
            ddlorderarea.SelectedValue = _orderarea;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp.Append(" and datediff(s,o_edate,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp.Append(" and datediff(s,o_edate,'" + _edate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_person))
            {
                strTemp.Append(" and (op_number like '%"+ _person + "%' or op_name like '%" + _person + "%')");
            }
            if (!string.IsNullOrEmpty(_depart))
            {
                strTemp.Append(" and detaildepart like '%" + _depart + "%'");
            }
            if (!string.IsNullOrEmpty(_status))
            {
                switch (_status)
                {
                    case "3":
                        strTemp.Append(" and (o_status=1 or o_status=2)");
                        break;
                    default:
                        strTemp.Append(" and o_status=" + _status + "");
                        break;
                }
            }
            if (!string.IsNullOrEmpty(_dstatus))
            {
                switch (_dstatus)
                {
                    case "5":
                        strTemp.Append(" and op_dstatus<>2 and (op_type=3 or op_type=5)");
                        break;
                    default:
                        strTemp.Append(" and op_dstatus=" + _dstatus + " and (op_type=3 or op_type=5)");
                        break;
                }
            }

            if (!string.IsNullOrEmpty(_lockstatus))
            {
                if (_lockstatus == "3")
                {
                    strTemp.Append(" and (o_lockStatus=0 or o_lockStatus=2)");
                }
                else
                {
                    strTemp.Append(" and o_lockStatus=" + _lockstatus + "");
                }
            }
            if (!string.IsNullOrEmpty(_area))
            {
                strTemp.Append(" and o_place like '%" + _area + "%'");
            }
            if (!string.IsNullOrEmpty(_orderarea))
            {
                strTemp.Append(" and op_area='" + _orderarea + "'");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("receiveOrder_page_size", "DTcmsPage"), out _pagesize))
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
            _sdate = DTRequest.GetString("txtsDate");
            _edate = DTRequest.GetString("txteDate");
            _person = DTRequest.GetString("txtPerson");
            _depart = DTRequest.GetString("txtDepart");
            _status = DTRequest.GetString("ddlstatus");
            _dstatus = DTRequest.GetString("ddldstatus");
            _lockstatus = DTRequest.GetString("ddllock");
            _area = DTRequest.GetString("ddlarea");
            _orderarea = DTRequest.GetString("ddlorderarea");
            RptBind(CombSqlTxt(), " op_number asc");
        }

        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("receiveOrder_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }
        private string backUrl()
        {
            return Utils.CombUrlTxt("ReceiveOrderAnalyzd_list.aspx", "page={0}&txtsDate={1}&txteDate={2}&txtPerson={3}&txtDepart={4}&ddlstatus={5}&ddldstatus={6}&ddllock={7}&ddlarea={8}&ddlorderarea={9}", "__id__", _sdate,_edate,_person,_depart,_status,_dstatus,_lockstatus,_area,_orderarea);
        }
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _sdate = DTRequest.GetString("txtsDate");
            _edate = DTRequest.GetString("txteDate");
            _person = DTRequest.GetString("txtPerson");
            _depart = DTRequest.GetString("txtDepart");
            _status = DTRequest.GetString("ddlstatus");
            _dstatus = DTRequest.GetString("ddldstatus");
            _lockstatus = DTRequest.GetString("ddllock");
            _area = DTRequest.GetString("ddlarea");
            _orderarea = DTRequest.GetString("ddlorderarea");
            BLL.statisticBLL bll = new BLL.statisticBLL();
            DataTable dt = bll.getReceiveOrderAnalyzeData(this.pageSize, this.page, CombSqlTxt(), " op_number asc", out _tcount3, out _tcount5, out _tcount, out this.totalCount,false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=策划与设计.xlsx"); //HttpUtility.UrlEncode(fileName));
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

            headRow.CreateCell(0).SetCellValue("人员");
            headRow.CreateCell(1).SetCellValue("岗位");
            headRow.CreateCell(2).SetCellValue("策划订单数");
            headRow.CreateCell(3).SetCellValue("设计订单数");
            headRow.CreateCell(4).SetCellValue("合计订单数");

            headRow.GetCell(0).CellStyle = titleCellStyle;
            headRow.GetCell(1).CellStyle = titleCellStyle;
            headRow.GetCell(2).CellStyle = titleCellStyle;
            headRow.GetCell(3).CellStyle = titleCellStyle;
            headRow.GetCell(4).CellStyle = titleCellStyle;

            sheet.SetColumnWidth(0, 15 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 20 * 256);
            sheet.SetColumnWidth(4, 20 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["op_number"].ToString()+"("+ dt.Rows[i]["op_name"].ToString() + ")");
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["detaildepart"]));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["type3"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["type5"]));
                    row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["sumType"]));

                    row.GetCell(0).CellStyle = cellStyle;
                    row.GetCell(1).CellStyle = cellStyle;
                    row.GetCell(2).CellStyle = cellStyle;
                    row.GetCell(3).CellStyle = cellStyle;
                    row.GetCell(4).CellStyle = cellStyle;
                }
            }

            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }
    }
}