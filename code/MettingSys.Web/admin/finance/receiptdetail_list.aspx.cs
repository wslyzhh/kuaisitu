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
    public partial class receiptdetail_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string _cusName = "", _cid = "", _oID = string.Empty, _area = string.Empty, _sforedate = string.Empty, _eforedate = string.Empty, _method = string.Empty, _person1 = "", _sdate = "", _edate = "", _addperson = "";
        protected Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _oID = DTRequest.GetString("txtorderid");
            _area = DTRequest.GetString("ddlarea");
            _sforedate = DTRequest.GetString("txtsforedate");
            _eforedate = DTRequest.GetString("txteforedate");
            _method = DTRequest.GetString("ddlmethod");
            _person1 = DTRequest.GetString("txtPerson1");
            _sdate = DTRequest.GetString("txtsdate");
            _edate = DTRequest.GetString("txtedate");
            _addperson = DTRequest.GetString("txtAddPerson");
            manager = GetAdminInfo();
            this.pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                initData();
                ChkAdminLevel("sys_payment_detail1", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("rpd_type=1" + CombSqlTxt(), "rpd_adddate desc,rpd_id desc");
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
            ddlmethod.DataSource = new BLL.payMethod().GetList(0, "pm_isUse=1 "+ sqlwhere + "", "pm_sort asc,pm_id asc");
            ddlmethod.DataTextField = "pm_name";
            ddlmethod.DataValueField = "pm_id";
            ddlmethod.DataBind();
            ddlmethod.Items.Insert(0, new ListItem("不限", ""));

            ddlarea.DataSource = new BLL.department().getAreaDict();
            ddlarea.DataTextField = "value";
            ddlarea.DataValueField = "key";
            ddlarea.DataBind();
            ddlarea.Items.Insert(0, new ListItem("不限", ""));
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
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            DataTable dt = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, manager, out this.totalCount,out decimal _tmoney).Tables[0];
            this.rptList.DataSource = dt;
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = backUrl();
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            pCount.Text = dt.Rows.Count.ToString();
            decimal _pmoney = 0;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pmoney += Utils.ObjToDecimal(dr["rpd_money"], 0);
                }
            }
            pMoney.Text = _pmoney.ToString();
            tCount.Text = totalCount.ToString();
            tMoney.Text = _tmoney.ToString();


            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            txtorderid.Text = _oID;
            ddlarea.SelectedValue = _area;
            txtsforedate.Text = _sforedate;
            txteforedate.Text = _eforedate;
            ddlmethod.SelectedValue = _method;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                strTemp.Append(" and rpd_cid = " + _cid + "");
            }
            if (!string.IsNullOrEmpty(_cusName))
            {
                strTemp.Append(" and c_name like '%" + _cusName + "%'");
            }
            if (!string.IsNullOrEmpty(_oID))
            {
                strTemp.Append(" and rpd_oid='" + _oID + "'");
            }
            if (!string.IsNullOrEmpty(_area))
            {
                strTemp.Append(" and rpd_area='" + _area + "'");
            }
            if (!string.IsNullOrEmpty(_sforedate))
            {
                strTemp.Append(" and datediff(d,rpd_foredate,'" + _sforedate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_eforedate))
            {
                strTemp.Append(" and datediff(d,rpd_foredate,'" + _eforedate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_method))
            {
                strTemp.Append(" and rpd_method=" + _method + "");
            }
            if (!string.IsNullOrEmpty(_person1))
            {
                strTemp.Append(" and (rp_confirmerNum like '%" + _person1.ToUpper() + "%' or rp_confirmerName like '%" + _person1.ToUpper() + "%')");
            }
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp.Append(" and datediff(day,rp_date,'" + _sdate + "')<=0 ");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp.Append(" and datediff(day,rp_date,'" + _edate + "')>=0 ");
            }
            if (!string.IsNullOrEmpty(_addperson))
            {
                strTemp.Append(" and (rpd_personNum like '%" + _addperson + "%' or rpd_personName like '%" + _addperson + "%')");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("receiptdetail_page_size", "DTcmsPage"), out _pagesize))
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
            _oID = DTRequest.GetFormString("txtorderid");
            _area = DTRequest.GetFormString("ddlarea");
            _sforedate = DTRequest.GetFormString("txtsforedate");
            _eforedate = DTRequest.GetFormString("txteforedate");
            _method = DTRequest.GetFormString("ddlmethod");
            _person1 = DTRequest.GetFormString("txtPerson1");
            _sdate = DTRequest.GetFormString("txtsdate");
            _edate = DTRequest.GetFormString("txtedate");
            _addperson = DTRequest.GetFormString("txtAddPerson");
            RptBind("rpd_type=1" + CombSqlTxt(), "rpd_adddate desc,rpd_id desc");
        }

        private string backUrl()
        {
            return Utils.CombUrlTxt("Receiptdetail_list.aspx", "page={0}&txtorderid={1}&txtsforedate={2}&txteforedate={3}&ddlmethod={4}&txtCusName={5}&hCusId={6}&ddlarea={7}&txtPerson1={8}&txtsdate={9}&txtedate={10}&txtAddPerson={11}", "__id__", _oID, _sforedate, _eforedate, _method, _cusName, _cid, _area, _person1, _sdate, _edate, _addperson);
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("receiptdetail_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }

        //批量删除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ChkAdminLevel("sys_payment_detail1", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
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
            JscriptMsg("共选择" + (success + error) + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), Utils.CombUrlTxt("Receiptdetail_list.aspx", "page={0}&txtorderid={1}&txtsforedate={2}&txteforedate={3}&ddlmethod={4}&txtCusName={5}&hCusId={6}&ddlarea={7}&txtPerson1={8}&txtsdate={9}&txtedate={10}", "__id__", _oID, _sforedate, _eforedate, _method, _cusName, _cid, _area,_person1,_sdate,_edate));
            
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _oID = DTRequest.GetFormString("txtorderid");
            _area = DTRequest.GetFormString("ddlarea");
            _sforedate = DTRequest.GetFormString("txtsforedate");
            _eforedate = DTRequest.GetFormString("txteforedate");
            _method = DTRequest.GetFormString("ddlmethod");
            _person1 = DTRequest.GetFormString("txtPerson1");
            _sdate = DTRequest.GetFormString("txtsdate");
            _edate = DTRequest.GetFormString("txtedate");
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            DataTable dt = bll.GetList(this.pageSize, this.page, "rpd_type=1" + CombSqlTxt(), "rpd_adddate desc,rpd_id desc", manager, out this.totalCount, out decimal _tmoney,false,false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=收款明细列表.xlsx"); //HttpUtility.UrlEncode(fileName));
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
            headRow.CreateCell(1).SetCellValue("收款对象");
            headRow.CreateCell(2).SetCellValue("收款内容");
            headRow.CreateCell(3).SetCellValue("收款金额");
            headRow.CreateCell(4).SetCellValue("预收日期");
            headRow.CreateCell(5).SetCellValue("收款方式");
            headRow.CreateCell(6).SetCellValue("申请人");
            headRow.CreateCell(7).SetCellValue("状态");
            headRow.CreateCell(8).SetCellValue("收款人");
            headRow.CreateCell(9).SetCellValue("实收日期");

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
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["rpd_oid"].ToString());
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_name"]));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rpd_content"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rpd_money"]));
                    row.CreateCell(4).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["rpd_foredate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(5).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["pm_name"]));
                    row.CreateCell(6).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rpd_personNum"])+"-"+Utils.ObjectToStr(dt.Rows[i]["rpd_personName"]));
                    row.CreateCell(7).SetCellValue(BusinessDict.checkStatus()[Convert.ToByte(dt.Rows[i]["rpd_flag1"].ToString())]);
                    row.CreateCell(8).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rp_confirmerName"]));
                    row.CreateCell(9).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["rp_date"])==null?"":ConvertHelper.toDate(dt.Rows[i]["rp_date"]).Value.ToString("yyyy-MM-dd"));

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