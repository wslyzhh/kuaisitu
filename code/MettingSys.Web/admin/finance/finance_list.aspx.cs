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
    public partial class finance_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小
        protected string typeText = "应收";
        protected string _ordernum = string.Empty, _cusname = string.Empty, _cusid = string.Empty, _status = string.Empty, _type = "", _smonth = "", _emonth = "", _sdate = "", _edate = "", _sign = "", _money = "", _nid = "", _detail = "", _ostatus = "", _lock = "", _area = "", _person1 = "", _person3 = "", _person5 = "",_orderarea="",_finarea="";
        protected string _isRemove = "";//员工业绩统计排除员工提成
        protected Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _ordernum = DTRequest.GetString("txtOrder");
            _cusname = DTRequest.GetString("txtCusName");
            _cusid = DTRequest.GetString("hCusId");
            _status = DTRequest.GetString("ddlcheck");
            _type = DTRequest.GetString("type");
            _smonth = DTRequest.GetString("txtsDate");
            _emonth = DTRequest.GetString("txteDate");
            _sdate = DTRequest.GetString("txtOsdate");
            _edate = DTRequest.GetString("txtOedate");
            _sign = DTRequest.GetString("ddlsign");
            _money = DTRequest.GetString("txtMoney");
            _nid = DTRequest.GetString("ddlnature");
            _detail = DTRequest.GetString("txtDetails");
            _ostatus = DTRequest.GetString("ddlstatus");
            _lock = DTRequest.GetString("ddllock");
            _area = DTRequest.GetString("ddlarea");
            _person1 = DTRequest.GetString("txtPerson1");
            _person3 = DTRequest.GetString("txtPerson3");
            _person5 = DTRequest.GetString("txtPerson5");
            _orderarea = DTRequest.GetString("ddlorderarea");
            _finarea = DTRequest.GetString("ddlfinarea");
            _isRemove = DTRequest.GetString("isRemove");
            Model.manager manager = null;
            this.pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                if (_type=="true")
                {
                    ChkAdminLevel("sys_finance_list1", DTEnums.ActionEnum.View.ToString()); //检查权限
                }
                else
                {
                    typeText = "应付";
                    ChkAdminLevel("sys_finance_list0", DTEnums.ActionEnum.View.ToString()); //检查权限
                }
                InitData();

                if (!string.IsNullOrEmpty(_sdate) && _sdate.Length<=7)
                {
                    _sdate = _sdate + "-01";
                }
                if (!string.IsNullOrEmpty(_edate) && _edate.Length <= 7)
                {
                    DateTime d = ConvertHelper.toDate(_edate + "-01").Value;
                    _edate = d.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                }
                RptBind("fin_type='" + (_type=="true"?"True":"False") + "'" + CombSqlTxt(), "fin_adddate desc,fin_id desc");
            }
        }
        #region 初始化数据=================================
        private void InitData()
        {
            //审批状态
            ddlcheck.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck.DataTextField = "value";
            ddlcheck.DataValueField = "key";
            ddlcheck.DataBind();
            ddlcheck.Items.Insert(0, new ListItem("不限", ""));

            //审批状态
            ddlcheck1.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck1.DataTextField = "value";
            ddlcheck1.DataValueField = "key";
            ddlcheck1.DataBind();
            ddlcheck1.Items.Insert(0, new ListItem("请选择", ""));

            ddlnature.DataSource = new BLL.businessNature().GetList(0, "na_isUse=1", "na_sort asc,na_id desc");
            ddlnature.DataTextField = "na_name";
            ddlnature.DataValueField = "na_id";
            ddlnature.DataBind();
            ddlnature.Items.Insert(0, new ListItem("不限", ""));

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

            ddlfinarea.DataSource = new BLL.department().getAreaDict();
            ddlfinarea.DataTextField = "value";
            ddlfinarea.DataValueField = "key";
            ddlfinarea.DataBind();
            ddlfinarea.Items.Insert(0, new ListItem("不限", ""));
        }
        #endregion
        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            manager = GetAdminInfo();
            if (!this.isSearch)
            {
                this.page = DTRequest.GetQueryInt("page", 1);
            }
            else
            {
                this.page = 1;
            }
            BLL.finance bll = new BLL.finance();
            this.rptList.DataSource = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, manager, out this.totalCount);
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = backUrl();
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            txtOrder.Text = _ordernum;
            txtCusName.Text = _cusname;
            hCusId.Value = _cusid;
            ddlcheck.SelectedValue = _status;
            txtsDate.Text = _smonth;
            txteDate.Text = _emonth;
            txtOsdate.Text = _sdate;
            txtOedate.Text = _edate;
            ddlsign.SelectedValue = _sign;
            txtMoney.Text = _money;
            ddlnature.SelectedValue = _nid;
            txtDetails.Text = _detail;
            ddlstatus.SelectedValue = _ostatus;
            ddllock.SelectedValue = _lock;
            ddlarea.SelectedValue = _area;
            txtPerson1.Text = _person1;
            txtPerson3.Text = _person3;
            txtPerson5.Text = _person5;
            ddlorderarea.SelectedValue = _orderarea;
            ddlfinarea.SelectedValue = _finarea;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_ordernum))
            {
                strTemp.Append(" and fin_oid='" + _ordernum + "'");
            }
            if (!string.IsNullOrEmpty(_cusid) && _cusid != "0")
            {
                strTemp.Append(" and fin_cid=" + _cusid + "");
            }
            if (!string.IsNullOrEmpty(_cusname))
            {
                strTemp.Append(" and c_name like '%" + _cusname + "%'");
            }
            if (!string.IsNullOrEmpty(_nid))
            {
                strTemp.Append(" and fin_nature = "+ _nid + "");
            }
            if (!string.IsNullOrEmpty(_detail))
            {
                strTemp.Append(" and fin_detail like '%" + _detail + "%'");
            }
            if (!string.IsNullOrEmpty(_status))
            {
                strTemp.Append(" and fin_flag=" + _status + "");
            }
            if (!string.IsNullOrEmpty(_smonth) || !string.IsNullOrEmpty(_emonth))
            {
                strTemp.Append(" and isnull(fin_month,'')<>''");
            }
            if (!string.IsNullOrEmpty(_smonth))
            {
                strTemp.Append(" and datediff(MONTH,fin_month+'-01','" + _smonth + "-01')<=0");
            }
            if (!string.IsNullOrEmpty(_emonth))
            {
                strTemp.Append(" and datediff(MONTH,fin_month+'-01','" + _emonth + "-01')>=0");
            }
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp.Append(" and datediff(d,o_edate,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp.Append(" and datediff(d,o_edate,'" + _edate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_money))
            {
                strTemp.Append(" and fin_money " + _sign + " " + _money + "");
            }
            if (!string.IsNullOrEmpty(_ostatus))
            {
                if (_ostatus == "3")
                {
                    strTemp.Append(" and (o_status=1 or o_status=2)");
                }
                else
                {
                    strTemp.Append(" and o_status=" + _ostatus + "");
                }
            }
            if (!string.IsNullOrEmpty(_lock))
            {
                if (_lock == "3")
                {
                    strTemp.Append(" and (o_lockStatus=0 or o_lockStatus=2)");
                }
                else
                {
                    strTemp.Append(" and o_lockStatus=" + _lock + "");
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
            if (!string.IsNullOrEmpty(_finarea))
            {
                strTemp.Append(" and fin_area='" + _finarea + "'");
            }
            if (!string.IsNullOrEmpty(_person1))
            {
                strTemp.Append(" and (op_number='" + _person1 + "' or op_name='" + _person1 + "' or exists(select * from MS_OrderPerson where op_oid=o_id and op_type =6 and (op_number ='" + _person1 + "' or op_name ='" + _person1 + "')))");
            }
            if (!string.IsNullOrEmpty(_person3))
            {
                strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =3 and (op_number ='" + _person3 + "' or op_name ='" + _person3 + "'))");
            }
            if (!string.IsNullOrEmpty(_person5))
            {
                strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =5 and (op_number ='" + _person5 + "' or op_name ='" + _person5 + "'))");
            }
            if (!string.IsNullOrEmpty(_isRemove))
            {
                strTemp.Append(" and (na_name<>'业务提成' and na_name<>'执行提成')");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("finance_page_size", "DTcmsPage"), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    return _pagesize;
                }
            }
            return _default_size;
        }
        #endregion

        private string backUrl()
        {
            return Utils.CombUrlTxt("finance_list.aspx", "page={0}&type={1}&txtOrder={2}&txtCusName={3}&hCusId={4}&ddlcheck={5}&txtsDate={6}&txteDate={7}&txtOsdate={8}&txtOedate={9}&ddlsign={10}&txtMoney={11}&ddlnature={12}&txtDetails={13}&ddlstatus={14}&ddllock={15}&ddlarea={16}&txtPerson1={17}&txtPerson3={18}&txtPerson5={19}&ddlorderarea={20}&ddlfinarea={21}&isRemove={22}", "__id__", _type, _ordernum, _cusname, _cusid, _status, _smonth, _emonth, _sdate, _edate, _sign, _money, _nid, _detail, _ostatus, _lock, _area, _person1,_person3,_person5,_orderarea,_finarea,_isRemove);
        }
        //关健字查询
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.isSearch = true;
            _ordernum = DTRequest.GetFormString("txtOrder");
            _cusname = DTRequest.GetFormString("txtCusName");
            _cusid = DTRequest.GetFormString("hCusId");
            _status = DTRequest.GetFormString("ddlcheck");
            _type = DTRequest.GetFormString("type");
            _smonth = DTRequest.GetFormString("txtsDate");
            _emonth = DTRequest.GetFormString("txteDate");
            _sdate = DTRequest.GetFormString("txtOsdate");
            _edate = DTRequest.GetFormString("txtOedate");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _nid = DTRequest.GetFormString("ddlnature");
            _detail = DTRequest.GetFormString("txtDetails");
            _ostatus = DTRequest.GetFormString("ddlstatus");
            _lock = DTRequest.GetFormString("ddllock");
            _area = DTRequest.GetFormString("ddlarea");
            _person1 = DTRequest.GetFormString("txtPerson1");
            _person3 = DTRequest.GetFormString("txtPerson3");
            _person5 = DTRequest.GetFormString("txtPerson5");
            _orderarea = DTRequest.GetFormString("ddlorderarea");
            _finarea = DTRequest.GetFormString("ddlfinarea");
            RptBind("fin_type='" + (_type == "true" ? "True" : "False") + "'" + CombSqlTxt(), "fin_adddate desc,fin_id desc");
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("finance_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _ordernum = DTRequest.GetFormString("txtOrder");
            _cusname = DTRequest.GetFormString("txtCusName");
            _cusid = DTRequest.GetFormString("hCusId");
            _status = DTRequest.GetFormString("ddlcheck");
            _type = DTRequest.GetFormString("type");
            _smonth = DTRequest.GetFormString("txtsDate");
            _emonth = DTRequest.GetFormString("txteDate");
            _sdate = DTRequest.GetFormString("txtOsdate");
            _edate = DTRequest.GetFormString("txtOedate");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _nid = DTRequest.GetFormString("ddlnature");
            _detail = DTRequest.GetFormString("txtDetails");
            _ostatus = DTRequest.GetFormString("ddlstatus");
            _lock = DTRequest.GetFormString("ddllock");
            _area = DTRequest.GetFormString("ddlarea");
            _person1 = DTRequest.GetFormString("txtPerson1");
            _person3 = DTRequest.GetFormString("txtPerson3");
            _person5 = DTRequest.GetFormString("txtPerson5");
            _orderarea = DTRequest.GetFormString("ddlorderarea");
            _finarea = DTRequest.GetFormString("ddlfinarea");
            BLL.finance bll = new BLL.finance();
            manager = GetAdminInfo();
            DataTable dt = bll.GetList(this.pageSize, this.page, "fin_type='" + (_type == "true" ? "True" : "False") + "'" + CombSqlTxt(), "fin_adddate desc,fin_id desc", manager, out this.totalCount,false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename="+((_type == "true" ? "应收列表" : "应付列表")) +".xlsx"); //HttpUtility.UrlEncode(fileName));
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
            headRow.CreateCell(1).SetCellValue(""+ (_type == "true" ? "应收对象" : "应付对象") + "");
            headRow.CreateCell(2).SetCellValue("对账凭证");
            headRow.CreateCell(3).SetCellValue("业务性质/明细");
            //headRow.CreateCell(4).SetCellValue("业务日期");
            headRow.CreateCell(4).SetCellValue("订单日期");
            headRow.CreateCell(5).SetCellValue("业务说明");
            headRow.CreateCell(6).SetCellValue("金额表达式");
            headRow.CreateCell(7).SetCellValue("金额");
            headRow.CreateCell(8).SetCellValue("区域");
            headRow.CreateCell(9).SetCellValue("结账月份");
            headRow.CreateCell(10).SetCellValue("审批状态");
            headRow.CreateCell(11).SetCellValue("添加人");

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
            sheet.SetColumnWidth(5, 15 * 256);
            sheet.SetColumnWidth(6, 20 * 256);
            sheet.SetColumnWidth(7, 20 * 256);
            sheet.SetColumnWidth(8, 20 * 256);
            sheet.SetColumnWidth(9, 20 * 256);
            sheet.SetColumnWidth(10, 15 * 256);
            sheet.SetColumnWidth(11, 15 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["fin_oid"].ToString());
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_name"]));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["chk"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["na_name"])+"/"+ Utils.ObjectToStr(dt.Rows[i]["fin_detail"]));
                    //row.CreateCell(4).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["fin_sdate"]).Value.ToString("yyyy-MM-dd") +"/"+ConvertHelper.toDate(dt.Rows[i]["fin_edate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(4).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["o_sdate"]).Value.ToString("yyyy-MM-dd") +"/"+ConvertHelper.toDate(dt.Rows[i]["o_edate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(5).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fin_illustration"]));
                    row.CreateCell(6).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fin_expression"]));
                    row.CreateCell(7).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fin_money"]));
                    row.CreateCell(8).SetCellValue(dt.Rows[i]["fin_area"].ToString());
                    row.CreateCell(9).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fin_month"]));
                    row.CreateCell(10).SetCellValue(dt.Rows[i]["fin_flag"].ToString() == "0" ? "待审批" : dt.Rows[i]["fin_flag"].ToString() == "1" ? "审批未通过" : "审批通过");
                    row.CreateCell(11).SetCellValue(dt.Rows[i]["fin_personName"].ToString());

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
        //批量删除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (_type=="true")
            {
                ChkAdminLevel("sys_finance_list1", DTEnums.ActionEnum.View.ToString()); //检查权限
            }
            else
            {
                ChkAdminLevel("sys_finance_list0", DTEnums.ActionEnum.View.ToString()); //检查权限
            }
            BLL.finance bll = new BLL.finance();
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
                    }
                    else
                    {
                        error++;
                        sb.Append(result + "<br/>");
                    }
                }
            }
            JscriptMsg("共选择" + (success + error) + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), backUrl());
        }
    }
}