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
    public partial class certificate_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string _num = string.Empty, _sdate = string.Empty, _edate = string.Empty, _check = string.Empty, _group = "";
        protected Model.business_log logmodel = null;
        decimal _tmoney = 0, _tunmoney = 0;

        protected Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _num = DTRequest.GetString("txtNum");
            _sdate = DTRequest.GetString("txtsdate");
            _edate = DTRequest.GetString("txtedate");
            _check = DTRequest.GetString("ddlcheck");
            _group = DTRequest.GetString("ddlgroup");
            this.pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                InitData();
                ChkAdminLevel("sys_cetificate", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("ce_id>0" + CombSqlTxt(), "ce_addDate desc,ce_id desc");
            }
            txtNum.Text = _num;
            txtsdate.Text = _sdate;
            txtedate.Text = _edate;
            ddlcheck.SelectedValue = _check;
            ddlgroup.SelectedValue = _group;
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
        }
        #endregion
        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.certificates bll = new BLL.certificates();
            DataTable dt = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, _group, out this.totalCount,out _tmoney,out _tunmoney).Tables[0];
            if (string.IsNullOrEmpty(_group))
            {                
                rptList.Visible = true;
                rptList1.Visible = false;
                this.rptList.DataSource = dt;
                this.rptList.DataBind();
            }
            else
            {
                rptList.Visible = false;
                rptList1.Visible = true;
                this.rptList1.DataSource = dt;
                this.rptList1.DataBind();
            }

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("certificate_list.aspx", "page={0}&txtNum={1}&txtsdate={2}&txtedate={3}&ddlcheck={4}&ddlgroup={5}", "__id__", _num, _sdate, _edate, _check,_group);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            
            pCount.Text = dt.Rows.Count.ToString();
            decimal _pmoney = 0, _punmoney = 0;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pmoney += Utils.ObjToDecimal(dr["receipt"], 0);
                    _punmoney += Utils.ObjToDecimal(dr["pay"], 0);
                }
            }
            pMoney.Text = _pmoney.ToString();
            pUnMoney.Text = _punmoney.ToString();
            tCount.Text = totalCount.ToString();
            tMoney.Text = _tmoney.ToString();
            tUnMoney.Text = _tunmoney.ToString();
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_num))
            {
                strTemp.Append(" and ce_num like  '%" + _num + "%'");
            }
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp.Append(" and datediff(d,ce_date,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp.Append(" and datediff(d,ce_date,'" + _edate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_check))
            {
                strTemp.Append(" and ce_flag ="+_check+"");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("certificates_page_size", "DTcmsPage"), out _pagesize))
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
            _num = DTRequest.GetFormString("txtNum");
            _sdate = DTRequest.GetFormString("txtsdate");
            _edate = DTRequest.GetFormString("txtedate");
            _check = DTRequest.GetFormString("ddlcheck");
            _group = DTRequest.GetFormString("ddlgroup");
            RptBind("ce_id>0" + CombSqlTxt(), "ce_addDate desc,ce_id desc");
            txtNum.Text = _num;
            txtsdate.Text = _sdate;
            txtedate.Text = _edate;
            ddlcheck.SelectedValue = _check;
            ddlgroup.SelectedValue = _group;
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("certificates_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("certificate_list.aspx", "page={0}&txtNum={1}&txtsdate={2}&txtedate={3}&ddlcheck={4}&ddlgroup={5}", "__id__", _num, _sdate, _edate, _check,_group));
        }
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _num = DTRequest.GetFormString("txtNum");
            _sdate = DTRequest.GetFormString("txtsdate");
            _edate = DTRequest.GetFormString("txtedate");
            _check = DTRequest.GetFormString("ddlcheck");
            _group = DTRequest.GetFormString("ddlgroup");
            BLL.certificates bll = new BLL.certificates();
            DataTable dt = bll.GetList(this.pageSize, this.page, "ce_id>0" + CombSqlTxt(), "ce_addDate desc,ce_id desc", _group, out this.totalCount, out _tmoney, out _tunmoney, false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=凭证列表.xlsx"); //HttpUtility.UrlEncode(fileName));
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

            headRow.CreateCell(0).SetCellValue("凭证号");
            headRow.CreateCell(1).SetCellValue("凭证日");
            headRow.CreateCell(2).SetCellValue("已收总额");
            headRow.CreateCell(3).SetCellValue("已付总额");
            headRow.CreateCell(4).SetCellValue("备注");
            headRow.CreateCell(5).SetCellValue("状态");

            headRow.GetCell(0).CellStyle = titleCellStyle;
            headRow.GetCell(1).CellStyle = titleCellStyle;
            headRow.GetCell(2).CellStyle = titleCellStyle;
            headRow.GetCell(3).CellStyle = titleCellStyle;
            headRow.GetCell(4).CellStyle = titleCellStyle;
            headRow.GetCell(5).CellStyle = titleCellStyle;

            sheet.SetColumnWidth(0, 15 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 20 * 256);
            sheet.SetColumnWidth(4, 40 * 256);
            sheet.SetColumnWidth(5, 15 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["ce_num"].ToString());
                    row.CreateCell(1).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["ce_date"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["receipt"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["pay"]));
                    row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["ce_remark"]));
                    row.CreateCell(5).SetCellValue(dt.Rows[i]["ce_flag"].ToString() == "0" ? "待审批" : dt.Rows[i]["ce_flag"].ToString() == "1" ? "审批未通过" : "审批通过");

                    row.GetCell(0).CellStyle = cellStyle;
                    row.GetCell(1).CellStyle = cellStyle;
                    row.GetCell(2).CellStyle = cellStyle;
                    row.GetCell(3).CellStyle = cellStyle;
                    row.GetCell(4).CellStyle = cellStyle;
                    row.GetCell(5).CellStyle = cellStyle;
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
            ChkAdminLevel("sys_cetificate", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.certificates bll = new BLL.certificates();
            manager = GetAdminInfo();
            string idstr = string.Empty;
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                int id = Convert.ToInt32(((HiddenField)rptList.Items[i].FindControl("hidId")).Value);
                CheckBox cb = (CheckBox)rptList.Items[i].FindControl("chkId");
                if (cb.Checked)
                {
                    idstr += id.ToString() + ",";
                }
            }
            string[] idlist = idstr.TrimEnd(',').Split(',');
            string result = "";
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            foreach (string id in idlist)
            {
                result = bll.deleteCertificate(Convert.ToInt32(id),manager.user_name,manager.real_name);
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
            JscriptMsg("共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), Utils.CombUrlTxt("certificate_list.aspx", "page={0}&txtNum={1}&txtsdate={2}&txtedate={3}&ddlcheck={4}&ddlgroup={5}", "__id__", _num, _sdate, _edate, _check,_group));
        }
    }
}