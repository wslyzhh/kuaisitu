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
    public partial class pay_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string _cusName = "", _cid = "", _method = string.Empty, _check = string.Empty, _check2 = string.Empty, _isconfirm = string.Empty, _sforedate = string.Empty, _eforedate = string.Empty, _sdate = string.Empty, _edate = string.Empty, _num = "", _chk = "", _numdate = "", _moneyType = "1", _sign = "", _money = "", _type = "", _flag = "";
        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;
        decimal _tmoney = 0, _tunmoney = 0;
        protected string _fromOtherPage = "0";//是否从其他页面链接过来的:0否，1是
        protected string orderby = "isnull(rp_date,'3000-01-01') desc,isnull(pm_sort,-1) asc,rp_id desc";
        protected void Page_Load(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _method = DTRequest.GetString("ddlmethod");
            _check = DTRequest.GetString("ddlcheck");
            _check2 = DTRequest.GetString("ddlcheck2");
            _isconfirm = DTRequest.GetString("ddlisConfirm");
            _sforedate = DTRequest.GetString("txtsforedate");
            _eforedate = DTRequest.GetString("txteforedate");
            _sdate = DTRequest.GetString("txtsdate");
            _edate = DTRequest.GetString("txtedate");
            _num = DTRequest.GetString("txtNum");
            _chk = DTRequest.GetString("txtChk");
            _numdate = DTRequest.GetString("txtNumDate");
            _moneyType = DTRequest.GetString("ddlmoneyType");
            _sign = DTRequest.GetString("ddlsign");
            _money = DTRequest.GetString("txtMoney");
            _type = DTRequest.GetString("ddlType");
            _flag = DTRequest.GetString("flag");
            if (string.IsNullOrEmpty(this._flag))
            {
                this._flag = "0";
            }
            switch (this._flag)
            {
                case "1":
                    this._check = "0";
                    break;
                case "2":
                    this._check = "2";
                    this._check2 = "0";
                    orderby = "rp_checkTime asc,rp_id desc";//“总经理未审批”页签中的记录按“财务审批”的时间降序排列
                    break;
                case "3":
                    this._check = "2";
                    this._check2 = "2";
                    _isconfirm = "False";
                    break;
                case "4":
                    this._check = "2";
                    this._check2 = "2";
                    _isconfirm = "True";
                    break;
                default:
                    break;
            }
            _fromOtherPage = DTRequest.GetString("fromOtherPage");
            if (string.IsNullOrEmpty(_fromOtherPage))
            {
                _fromOtherPage = "0";
            }
            this.pageSize = GetPageSize(10); //每页数量
            manager = GetAdminInfo();
            if (!Page.IsPostBack)
            {
                if (string.IsNullOrEmpty(DTRequest.GetString("page")) && string.IsNullOrEmpty(_isconfirm))
                {
                    if (_fromOtherPage == "0")
                    {
                        _isconfirm = "False";
                    }
                }
                initData();
                labUnPayCount.Text = new BLL.ReceiptPay().getCheckUnPaycount().ToString();
                labUnCheckCount.Text = new BLL.ReceiptPay().getUnPaycount().ToString();
                ChkAdminLevel("sys_payment_list0", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("rp_type=0 " + CombSqlTxt(), orderby);
            }
        }
        #region 初始化=================================
        private void initData()
        {
            //收付款方式
            //判断是否是财务来显示不同的收付款方式
            string sqlwhere = "";
            if (!new BLL.permission().checkHasPermission(manager, "0401"))
            {
                sqlwhere = " and pm_type=0";
            }
            ddlmethod.DataSource = new BLL.payMethod().GetList(0, "pm_isUse=1 "+ sqlwhere + "", "pm_sort asc,pm_id asc");
            ddlmethod.DataTextField = "pm_name";
            ddlmethod.DataValueField = "pm_id";
            ddlmethod.DataBind();
            ddlmethod.Items.Insert(0, new ListItem("不限", ""));
                        
            ddlmethod1.DataSource = new BLL.payMethod().GetList(0, "pm_isUse=1 "+ sqlwhere + "", "pm_sort asc,pm_id asc");
            ddlmethod1.DataTextField = "pm_name";
            ddlmethod1.DataValueField = "pm_id";
            ddlmethod1.DataBind();
            ddlmethod1.Items.Insert(0, new ListItem("请选择", ""));

            //收款状态
            ddlisConfirm.DataSource = Common.BusinessDict.financeConfirmStatus(0);
            ddlisConfirm.DataTextField = "value";
            ddlisConfirm.DataValueField = "key";
            ddlisConfirm.DataBind();
            ddlisConfirm.Items.Insert(0, new ListItem("不限", ""));


            ddlisConfirm1.DataSource = Common.BusinessDict.financeConfirmStatus(0);
            ddlisConfirm1.DataTextField = "value";
            ddlisConfirm1.DataValueField = "key";
            ddlisConfirm1.DataBind();
            ddlisConfirm1.Items.Insert(0, new ListItem("请选择", ""));
            ddlisConfirm1.SelectedValue = "True";

            ddlcheck.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck.DataTextField = "value";
            ddlcheck.DataValueField = "key";
            ddlcheck.DataBind();
            ddlcheck.Items.Insert(0, new ListItem("不限", ""));

            ddlcheck1.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck1.DataTextField = "value";
            ddlcheck1.DataValueField = "key";
            ddlcheck1.DataBind();
            ddlcheck1.Items.Insert(0, new ListItem("请选择", ""));

            ddlcheck2.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck2.DataTextField = "value";
            ddlcheck2.DataValueField = "key";
            ddlcheck2.DataBind();
            ddlcheck2.Items.Insert(0, new ListItem("不限", ""));
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
            string pageUrl = backUrl();
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
            ddlcheck.SelectedValue = _check;
            ddlcheck2.SelectedValue = _check2;
            ddlisConfirm.SelectedValue = _isconfirm;
            txtsforedate.Text = _sforedate;
            txteforedate.Text = _eforedate;
            txtsdate.Text = _sdate;
            txtedate.Text = _edate;
            txtNum.Text = _num;
            txtChk.Text = _chk;
            txtNumDate.Text = _numdate;
            ddlsign.SelectedValue = _sign;
            txtMoney.Text = _money;
            ddlmoneyType.SelectedValue = _moneyType;

            ddlchecktype.SelectedValue = _flag;
        }
        #endregion

        private string backUrl()
        {
            return Utils.CombUrlTxt("pay_list.aspx", "page={0}&ddlmethod={1}&ddlisConfirm={2}&txtsforedate={3}&txteforedate={4}&txtsdate={5}&txtedate={6}&txtNum={7}&ddlcheck={8}&txtCusName={9}&hCusId={10}&txtChk={11}&ddlcheck2={12}&txtNumDate={13}&ddlsign={14}&txtmoney={15}&ddlmoneyType={16}&ddlType={17}", "__id__", _method, _isconfirm, _sforedate, _eforedate, _sdate, _edate, _num, _check, _cusName, _cid, _chk, _check2, _numdate, _sign, _money, _moneyType,_type);
        }

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                strTemp.Append(" and rp_cid = " + _cid + "");
            }
            if (!string.IsNullOrEmpty(_cusName))
            {
                strTemp.Append(" and c_name like '%" + _cusName + "%'");
            }
            if (!string.IsNullOrEmpty(_num))
            {
                if (_num == "0")
                {
                    strTemp.Append(" and isnull(ce_num,'')=''");
                }
                else if (_num == "1")
                {
                    strTemp.Append(" and isnull(ce_num,'')<>''");
                }
                else
                {
                    strTemp.Append(" and ce_num='" + _num + "'");
                }
            }
            if (!string.IsNullOrEmpty(_numdate))
            {
                strTemp.Append(" and ce_date='" + _numdate + "'");
            }
            if (!string.IsNullOrEmpty(_method))
            {
                strTemp.Append(" and rp_method=" + _method + "");
            }
            if (!string.IsNullOrEmpty(_check))
            {
                strTemp.Append(" and rp_flag=" + _check + "");
            }
            if (!string.IsNullOrEmpty(_check2))
            {
                strTemp.Append(" and rp_flag1=" + _check2 + "");
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
            if (!string.IsNullOrEmpty(_money))
            {
                if (_moneyType == "1")
                {
                    strTemp.Append(" and rp_money " + _sign + " " + _money + "");
                }
                else if (_moneyType == "2")
                {
                    strTemp.Append(" and undistribute " + _sign + " " + _money + "");
                }
            }
            if (!string.IsNullOrEmpty(_type))
            {
                strTemp.Append(" and rp_isExpect='" + _type + "'");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("pay_list_size", "DTcmsPage"), out _pagesize))
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
            _check = DTRequest.GetFormString("ddlcheck");
            _check2 = DTRequest.GetFormString("ddlcheck2");
            _isconfirm = DTRequest.GetFormString("ddlisConfirm");
            _sforedate = DTRequest.GetFormString("txtsforedate");
            _eforedate = DTRequest.GetFormString("txteforedate");
            _sdate = DTRequest.GetFormString("txtsdate");
            _edate = DTRequest.GetFormString("txtedate");
            _num = DTRequest.GetFormString("txtNum");
            _chk = DTRequest.GetFormString("txtChk");
            _numdate = DTRequest.GetFormString("txtNumDate");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _moneyType = DTRequest.GetFormString("ddlmoneyType");
            _type = DTRequest.GetFormString("ddlType");
            RptBind("rp_type=0 " + CombSqlTxt(), orderby);
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("pay_list_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _method = DTRequest.GetFormString("ddlmethod");
            _check = DTRequest.GetFormString("ddlcheck");
            _check2 = DTRequest.GetFormString("ddlcheck2");
            _isconfirm = DTRequest.GetFormString("ddlisConfirm");
            _sforedate = DTRequest.GetFormString("txtsforedate");
            _eforedate = DTRequest.GetFormString("txteforedate");
            _sdate = DTRequest.GetFormString("txtsdate");
            _edate = DTRequest.GetFormString("txtedate");
            _num = DTRequest.GetFormString("txtNum");
            _chk = DTRequest.GetFormString("txtChk");
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            DataTable dt = bll.GetList(this.pageSize, this.page, "rp_type=0 " + CombSqlTxt(), "isnull(rp_date,'3000-01-01') desc,isnull(pm_sort,-1) asc,rp_id desc", out this.totalCount, out _tmoney, out _tunmoney, false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=付款通知列表.xlsx"); //HttpUtility.UrlEncode(fileName));
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

            headRow.CreateCell(0).SetCellValue("付款对象");
            headRow.CreateCell(1).SetCellValue("凭证");
            headRow.CreateCell(2).SetCellValue("付款内容");
            headRow.CreateCell(3).SetCellValue("客户银行账号");
            headRow.CreateCell(4).SetCellValue("付款金额");
            headRow.CreateCell(5).SetCellValue("未分配金额");
            headRow.CreateCell(6).SetCellValue("预付日期");
            headRow.CreateCell(7).SetCellValue("付款方式");
            headRow.CreateCell(8).SetCellValue("实付日期");
            headRow.CreateCell(9).SetCellValue("申请人");
            headRow.CreateCell(10).SetCellValue("财务审批");
            headRow.CreateCell(11).SetCellValue("总经理审批");
            headRow.CreateCell(12).SetCellValue("确认收款");

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

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["c_name"].ToString() + (Utils.StrToBool(Utils.ObjectToStr(dt.Rows[i]["rp_isExpect"]), false) ? "[预]" : ""));
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["ce_num"]));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rp_content"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["cb_bankName"]) + "\r\n" + Utils.ObjectToStr(dt.Rows[i]["cb_bankNum"]) + "\r\n" + Utils.ObjectToStr(dt.Rows[i]["cb_bank"]));
                    row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rp_money"]));
                    row.CreateCell(5).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["undistribute"]));
                    row.CreateCell(6).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["rp_foredate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(7).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["pm_name"]));
                    row.CreateCell(8).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["rp_date"]) == null ? "" : ConvertHelper.toDate(dt.Rows[i]["rp_date"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(9).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rp_personName"]));
                    row.CreateCell(10).SetCellValue(dt.Rows[i]["rp_flag"].ToString() == "0" ? "待审批" : dt.Rows[i]["rp_flag"].ToString() == "1" ? "审批未通过" : "审批通过");
                    row.CreateCell(11).SetCellValue(dt.Rows[i]["rp_flag1"].ToString() == "0" ? "待审批" : dt.Rows[i]["rp_flag1"].ToString() == "1" ? "审批未通过" : "审批通过");
                    row.CreateCell(12).SetCellValue(Utils.StrToBool(Utils.ObjectToStr(dt.Rows[i]["rp_isConfirm"]), false) ? "已收款" : "待收款");

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
            ChkAdminLevel("sys_payment_list0", DTEnums.ActionEnum.Delete.ToString()); //检查权限
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
            JscriptMsg("共选择" + (success + error) + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), backUrl(), "");
            
        }
    }
}