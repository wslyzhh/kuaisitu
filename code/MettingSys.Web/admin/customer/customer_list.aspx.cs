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

namespace MettingSys.Web.admin.customer
{
    public partial class customer_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string _cusName = "", _cid = "", _check1 = string.Empty, _type = string.Empty, _isUse = string.Empty, _owner1, _business, _contact = "";
        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _check1 = DTRequest.GetString("ddlcheck1");
            _type = DTRequest.GetString("ddltype");
            _isUse = DTRequest.GetString("ddlisUse");
            _owner1 = DTRequest.GetString("txtOwner1");
            _business = DTRequest.GetString("txtBusiness");
            _contact = DTRequest.GetString("txtContact");

            this.pageSize = GetPageSize(10); //每页数量
            manager = GetAdminInfo();
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("sys_customer_list", DTEnums.ActionEnum.View.ToString()); //检查权限
                InitData();
                RptBind("c_id>0" + CombSqlTxt(), "c_isUse desc,c_addDate desc,c_id desc");
            }
        }
        #region 初始化数据=================================
        private void InitData()
        {
            //审批状态
            ddlcheck1.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck1.DataTextField = "value";
            ddlcheck1.DataValueField = "key";
            ddlcheck1.DataBind();
            ddlcheck1.Items.Insert(0, new ListItem("不限", ""));
            //客户类别
            ddltype.DataSource = Common.BusinessDict.customerType();
            ddltype.DataTextField = "value";
            ddltype.DataValueField = "key";
            ddltype.DataBind();
            ddltype.Items.Insert(0, new ListItem("不限", ""));
            //启用状态
            ddlisUse.DataSource = Common.BusinessDict.isUseStatus(1);
            ddlisUse.DataTextField = "value";
            ddlisUse.DataValueField = "key";
            ddlisUse.DataBind();
            ddlisUse.Items.Insert(0, new ListItem("不限", ""));
            //审批状态
            ddlcheck.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck.DataTextField = "value";
            ddlcheck.DataValueField = "key";
            ddlcheck.DataBind();
            ddlcheck.Items.Insert(0, new ListItem("请选择", ""));

            if (!new BLL.permission().checkHasPermission(manager,"0301"))
            {
                li1.Visible = false;
                li2.Visible = false;
                li3.Visible = false;
                li4.Visible = false;
            }
        }
        #endregion
        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.Customer bll = new BLL.Customer();
            this.rptList.DataSource = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, manager, out this.totalCount);
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = backUrl();
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            ddlcheck1.SelectedValue = _check1;
            ddltype.SelectedValue = _type;
            ddlisUse.SelectedValue = _isUse;
            txtOwner1.Text = _owner1;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                strTemp.Append(" and c_id=" + _cid + "");
            }
            if (!string.IsNullOrEmpty(_cusName))
            {
                strTemp.Append(" and c_name like '%" + _cusName + "%'");
            }
            if (!string.IsNullOrEmpty(_check1))
            {
                strTemp.Append(" and c_flag=" + _check1 + "");
            }
            if (!string.IsNullOrEmpty(_type))
            {
                strTemp.Append(" and c_type='" + _type + "'");
            }
            if (!string.IsNullOrEmpty(_isUse))
            {
                strTemp.Append(" and c_isuse='" + _isUse + "'");
            }
            if (!string.IsNullOrEmpty(_owner1))
            {
                strTemp.Append(" and (c_owner like '%" + _owner1 + "%' or c_ownerName like '%" + _owner1 + "%')");
            }
            if (!string.IsNullOrEmpty(_business))
            {
                strTemp.Append(" and c_business like '%" + _business + "%'");
            }
            if (!string.IsNullOrEmpty(_contact))
            {
                strTemp.Append(" and co_name like '%" + _contact + "%'");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("customer_page_size", "DTcmsPage"), out _pagesize))
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
            _cusName = DTRequest.GetFormString("txtCusName");
            _cid = DTRequest.GetFormString("hCusId");
            _check1 = DTRequest.GetFormString("ddlcheck1");
            _type = DTRequest.GetFormString("ddltype");
            _isUse = DTRequest.GetFormString("ddlisUse");
            _owner1 = DTRequest.GetFormString("txtOwner1");
            _business = DTRequest.GetFormString("txtBusiness");
            _contact = DTRequest.GetFormString("txtContact");
            RptBind("c_id>0" + CombSqlTxt(), "c_isUse desc,c_addDate desc,c_id desc");
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("customer_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }
        private string backUrl()
        {
            return Utils.CombUrlTxt("customer_list.aspx", "page={0}&txtCusName={1}&hCusId={2}&ddlcheck1={3}&ddltype={4}&ddlisUse={5}&txtOwner1={6}&txtBusiness={7}&txtContact={8}", "__id__", _cusName, _cid, _check1, _type, _isUse, _owner1, _business, _contact);
        }
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetFormString("txtCusName");
            _cid = DTRequest.GetFormString("hCusId");
            _check1 = DTRequest.GetFormString("ddlcheck1");
            _type = DTRequest.GetFormString("ddltype");
            _isUse = DTRequest.GetFormString("ddlisUse");
            _owner1 = DTRequest.GetFormString("txtOwner1");
            _business = DTRequest.GetFormString("txtBusiness");
            _contact = DTRequest.GetFormString("txtContact");
            BLL.Customer bll = new BLL.Customer();
            DataTable dt = bll.GetList(this.pageSize, this.page, "c_id>0" + CombSqlTxt(), "c_isUse desc,c_addDate desc,c_id desc", manager, out this.totalCount,false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=客户列表.xlsx"); //HttpUtility.UrlEncode(fileName));
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

            headRow.CreateCell(0).SetCellValue("客户ID");
            headRow.CreateCell(1).SetCellValue("客户名称");
            headRow.CreateCell(2).SetCellValue("客户类别");
            headRow.CreateCell(3).SetCellValue("业务范围");
            headRow.CreateCell(4).SetCellValue("信用代码(税号)");
            headRow.CreateCell(5).SetCellValue("所属人");
            headRow.CreateCell(6).SetCellValue("联系人");
            headRow.CreateCell(7).SetCellValue("审批状态");
            headRow.CreateCell(8).SetCellValue("启用状态");

            headRow.GetCell(0).CellStyle = titleCellStyle;
            headRow.GetCell(1).CellStyle = titleCellStyle;
            headRow.GetCell(2).CellStyle = titleCellStyle;
            headRow.GetCell(3).CellStyle = titleCellStyle;
            headRow.GetCell(4).CellStyle = titleCellStyle;
            headRow.GetCell(5).CellStyle = titleCellStyle;
            headRow.GetCell(6).CellStyle = titleCellStyle;
            headRow.GetCell(7).CellStyle = titleCellStyle;
            headRow.GetCell(8).CellStyle = titleCellStyle;

            sheet.SetColumnWidth(0, 15 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 30 * 256);
            sheet.SetColumnWidth(4, 20 * 256);
            sheet.SetColumnWidth(5, 20 * 256);
            sheet.SetColumnWidth(6, 15 * 256);
            sheet.SetColumnWidth(7, 20 * 256);
            sheet.SetColumnWidth(8, 20 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["c_id"].ToString());
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_name"]));
                    row.CreateCell(2).SetCellValue(BusinessDict.customerType()[Utils.ObjToByte(dt.Rows[i]["c_type"])]);
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_business"]));
                    row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_num"]));
                    row.CreateCell(5).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_ownerName"]));
                    row.CreateCell(6).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["co_name"]));
                    row.CreateCell(7).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_flag"])=="0"?"待审批": Utils.ObjectToStr(dt.Rows[i]["c_flag"])=="1"?"审批未通过":"审批通过");
                    row.CreateCell(8).SetCellValue(BusinessDict.isUseStatus(1)[Convert.ToBoolean(dt.Rows[i]["c_isUse"])]);

                    row.GetCell(0).CellStyle = cellStyle;
                    row.GetCell(1).CellStyle = cellStyle;
                    row.GetCell(2).CellStyle = cellStyle;
                    row.GetCell(3).CellStyle = cellStyle;
                    row.GetCell(4).CellStyle = cellStyle;
                    row.GetCell(5).CellStyle = cellStyle;
                    row.GetCell(6).CellStyle = cellStyle;
                    row.GetCell(7).CellStyle = cellStyle;
                    row.GetCell(8).CellStyle = cellStyle;
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
            ChkAdminLevel("sys_customer_list", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.Customer bll = new BLL.Customer();
            logmodel = new Model.business_log();
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
            manager = GetAdminInfo();
            foreach (string id in idlist)
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
            JscriptMsg("共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), Utils.CombUrlTxt("customer_list.aspx", "page={0}&txtCusName={1}&hCusId={2}&ddlcheck1={3}&ddltype={4}&ddlisUse={5}&txtOwner1={6}", "__id__", _cusName, _cid, _check1, _type, _isUse,_owner1));

            
        }
    }
}