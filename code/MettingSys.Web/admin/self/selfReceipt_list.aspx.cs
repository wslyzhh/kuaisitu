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

namespace MettingSys.Web.admin.self
{
    public partial class selfReceipt_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string _cusName = "", _cid = "", _method = string.Empty, _isconfirm = string.Empty, _sforedate = string.Empty, _eforedate = string.Empty, _sdate = string.Empty, _edate = string.Empty, _num = "", _chk = "", _numdate = "";
        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;
        decimal _tmoney = 0, _tunmoney = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _method = DTRequest.GetString("ddlmethod");
            _isconfirm = DTRequest.GetString("ddlisConfirm");
            _sforedate = DTRequest.GetString("txtsforedate");
            _eforedate = DTRequest.GetString("txteforedate");
            _sdate = DTRequest.GetString("txtsdate");
            _edate = DTRequest.GetString("txtedate");
            _num = DTRequest.GetString("txtNum");
            _chk = DTRequest.GetString("txtChk");
            _numdate = DTRequest.GetString("txtNumDate");
            manager = GetAdminInfo();
            this.pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                if (string.IsNullOrEmpty(DTRequest.GetString("page")))
                {
                    _isconfirm = "False";
                }
                initData();
                RptBind("rp_type=1 " + CombSqlTxt(), "isnull(rp_date,'3000-01-01') desc,isnull(pm_sort,-1) asc,rp_id desc");
            }
        }
        #region 初始化=================================
        private void initData()
        {
            //根据权限显示付款方式
            string sqlwhere = "";
            if (!new BLL.permission().checkHasPermission(manager, "0401"))
            {
                sqlwhere = " and pm_type=0";
            }
            ddlmethod.DataSource = new BLL.payMethod().GetList(0, "pm_isUse=1 " + sqlwhere + "", "pm_sort asc,pm_id asc");
            ddlmethod.DataTextField = "pm_name";
            ddlmethod.DataValueField = "pm_id";
            ddlmethod.DataBind();
            ddlmethod.Items.Insert(0, new ListItem("不限", ""));

            //收款状态
            ddlisConfirm.DataSource = Common.BusinessDict.financeConfirmStatus(1);
            ddlisConfirm.DataTextField = "value";
            ddlisConfirm.DataValueField = "key";
            ddlisConfirm.DataBind();
            ddlisConfirm.Items.Insert(0, new ListItem("不限", ""));

            ddlisConfirm1.DataSource = Common.BusinessDict.financeConfirmStatus(1);
            ddlisConfirm1.DataTextField = "value";
            ddlisConfirm1.DataValueField = "key";
            ddlisConfirm1.DataBind();
            ddlisConfirm1.Items.Insert(0, new ListItem("请选择", ""));
            ddlisConfirm1.SelectedValue = "True";
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
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            DataTable dt = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, out this.totalCount, out _tmoney, out _tunmoney).Tables[0];
            this.rptList.DataSource = dt;
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("selfReceipt_list.aspx", "page={0}&ddlmethod={1}&ddlisConfirm={2}&txtsforedate={3}&txteforedate={4}&txtsdate={5}&txtedate={6}&txtNum={7}&txtCusName={8}&hCusId={9}&txtChk={10}&txtNumDate={11}", "__id__", _method, _isconfirm, _sforedate, _eforedate, _sdate, _edate, _num, _cusName, _cid, _chk, _numdate);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            pCount.Text = dt.Rows.Count.ToString();
            decimal _pmoney = 0, _punmoney = 0;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pmoney += Utils.ObjToDecimal(dr["rp_money"], 0);
                    _punmoney += Utils.ObjToDecimal(dr["undistribute"], 0);
                }
            }
            pMoney.Text = _pmoney.ToString();
            pUnMoney.Text = _punmoney.ToString();
            tCount.Text = totalCount.ToString();
            tMoney.Text = _tmoney.ToString();
            tUnMoney.Text = _tunmoney.ToString();

            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            ddlmethod.SelectedValue = _method;
            ddlisConfirm.SelectedValue = _isconfirm;
            txtsforedate.Text = _sforedate;
            txteforedate.Text = _eforedate;
            txtsdate.Text = _sdate;
            txtedate.Text = _edate;
            txtNum.Text = _num;
            txtChk.Text = _chk;
            txtNumDate.Text = _numdate;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            strTemp.Append(" and exists (select * from MS_ReceiptPayDetail left join MS_Order on rpd_oid=o_id left join MS_OrderPerson on rpd_oid=op_oid and op_type=1 where rpd_rpid=rp_id and op_number='" + manager.user_name + "')");
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                strTemp.Append(" and rp_cid = " + _cid + "");
            }
            if (!string.IsNullOrEmpty(_num))
            {
                strTemp.Append(" and ce_num='" + _num + "'");
            }
            if (!string.IsNullOrEmpty(_numdate))
            {
                strTemp.Append(" and ce_date='" + _numdate + "'");
            }
            if (!string.IsNullOrEmpty(_method))
            {
                strTemp.Append(" and rp_method=" + _method + "");
            }
            if (!string.IsNullOrEmpty(_isconfirm))
            {
                strTemp.Append(" and rp_isConfirm='" + _isconfirm + "'");
            }
            if (!string.IsNullOrEmpty(_sforedate))
            {
                strTemp.Append(" and datediff(d,rp_foredate,'" + _sforedate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_eforedate))
            {
                strTemp.Append(" and datediff(d,rp_foredate,'" + _eforedate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp.Append(" and datediff(d,rp_date,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp.Append(" and datediff(d,rp_date,'" + _edate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_chk))
            {
                strTemp.Append(" and exists(select * from MS_ReceiptPayDetail where rpd_rpid=rp_id and rpd_num='" + _chk + "')");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("selfReceipt_list_size", "DTcmsPage"), out _pagesize))
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
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _method = DTRequest.GetFormString("ddlmethod");
            _isconfirm = DTRequest.GetFormString("ddlisConfirm");
            _sforedate = DTRequest.GetFormString("txtsforedate");
            _eforedate = DTRequest.GetFormString("txteforedate");
            _sdate = DTRequest.GetFormString("txtsdate");
            _edate = DTRequest.GetFormString("txtedate");
            _num = DTRequest.GetFormString("txtNum");
            _chk = DTRequest.GetFormString("txtChk");
            _numdate = DTRequest.GetFormString("txtNumDate");
            RptBind("rp_type=1 " + CombSqlTxt(), "isnull(rp_date,'3000-01-01') desc,isnull(pm_sort,-1) asc,rp_id desc");

        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("selfReceipt_list_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("selfReceipt_list.aspx", "page={0}&ddlmethod={1}&ddlisConfirm={2}&txtsforedate={3}&txteforedate={4}&txtsdate={5}&txtedate={6}&txtNum={7}&txtCusName={8}&hCusId={9}&txtChk={10}&txtNumDate={11}", "__id__", _method, _isconfirm, _sforedate, _eforedate, _sdate, _edate, _num, _cusName, _cid, _chk, _numdate));
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _method = DTRequest.GetFormString("ddlmethod");
            _isconfirm = DTRequest.GetFormString("ddlisConfirm");
            _sforedate = DTRequest.GetFormString("txtsforedate");
            _eforedate = DTRequest.GetFormString("txteforedate");
            _sdate = DTRequest.GetFormString("txtsdate");
            _edate = DTRequest.GetFormString("txtedate");
            _num = DTRequest.GetFormString("txtNum");
            _chk = DTRequest.GetFormString("txtChk");
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            DataTable dt = bll.GetList(this.pageSize, this.page, "rp_type=1 " + CombSqlTxt(), "isnull(rp_date,'3000-01-01') desc,isnull(pm_sort,-1) asc,rp_id desc", out this.totalCount, out _tmoney, out _tunmoney, false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=收款通知列表.xlsx"); //HttpUtility.UrlEncode(fileName));
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

            headRow.CreateCell(0).SetCellValue("收款对象");
            headRow.CreateCell(1).SetCellValue("凭证");
            headRow.CreateCell(2).SetCellValue("收款内容");
            headRow.CreateCell(3).SetCellValue("收款金额");
            headRow.CreateCell(4).SetCellValue("未分配金额");
            headRow.CreateCell(5).SetCellValue("预收日期");
            headRow.CreateCell(6).SetCellValue("收款方式");
            headRow.CreateCell(7).SetCellValue("实收日期");
            headRow.CreateCell(8).SetCellValue("申请人");
            headRow.CreateCell(9).SetCellValue("确认收款");

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
            sheet.SetColumnWidth(5, 15 * 256);
            sheet.SetColumnWidth(6, 20 * 256);
            sheet.SetColumnWidth(7, 20 * 256);
            sheet.SetColumnWidth(8, 20 * 256);
            sheet.SetColumnWidth(9, 20 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["c_name"].ToString() + (Utils.StrToBool(Utils.ObjectToStr(dt.Rows[i]["rp_isExpect"]), false) ? "[预]" : ""));
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["ce_num"]));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rp_content"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rp_money"]));
                    row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["undistribute"]));
                    row.CreateCell(5).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["rp_foredate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(6).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["pm_name"]));
                    row.CreateCell(7).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["rp_date"]) == null ? "" : ConvertHelper.toDate(dt.Rows[i]["rp_date"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(8).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rp_personName"]));
                    row.CreateCell(9).SetCellValue(Utils.StrToBool(Utils.ObjectToStr(dt.Rows[i]["rp_isConfirm"]), false) ? "已收款" : "待收款");

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

        //批量删除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
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
            JscriptMsg("共选择" + (success + error) + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), Utils.CombUrlTxt("selfReceipt_list.aspx", "page={0}&ddlmethod={1}&ddlisConfirm={2}&txtsforedate={3}&txteforedate={4}&txtsdate={5}&txtedate={6}&txtNum={7}&txtCusName={8}&hCusId={9}&txtChk={10}&txtNumDate={11}", "__id__", _method, _isconfirm, _sforedate, _eforedate, _sdate, _edate, _num, _cusName, _cid, _chk, _numdate), "");

        }
    }
}