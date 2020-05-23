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
    public partial class orderApproval_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小
        protected string flag = "", _orderid = "", _cusName = "", _cid = "", _contractPrice = "", _sdate = "", _edate = "", _sdate1 = "", _edate1 = "", _status = "", _dstatus = "", _pushstatus = "", _flag = "", _lockstatus = "", _oid = "", _area = "", _orderarea = "";


        Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            flag = DTRequest.GetString("flag");
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _contractPrice = DTRequest.GetString("ddlContractPrice");
            _sdate = DTRequest.GetString("txtsDate");
            _edate = DTRequest.GetString("txteDate");
            _sdate1 = DTRequest.GetString("txtsDate1");
            _edate1 = DTRequest.GetString("txteDate1");
            _status = DTRequest.GetString("ddlstatus");
            _dstatus = DTRequest.GetString("ddldstatus");
            _pushstatus = DTRequest.GetString("ddlispush");
            _flag = DTRequest.GetString("ddlflag");
            _lockstatus = DTRequest.GetString("ddllock");
            _oid = DTRequest.GetString("txtOrderID");
            _area = DTRequest.GetString("ddlarea");
            _orderarea = DTRequest.GetString("ddlorderarea");
            if (string.IsNullOrEmpty(flag))
            {
                flag = "0";
            }
            manager = GetAdminInfo();
            this.pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                InitData();
                if (flag == "0")
                {  
                    _edate1 = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    _status = "3";
                    _dstatus = "4";
                    _flag = "2";
                    _lockstatus = "0";
                }
                labUnCheckCount.Text = new BLL.Order().getUnCheckOrderCount(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), "3", "4", "2").ToString();
                labUnLockCount.Text = new BLL.Order().getUnLockOrderCount().ToString();
                labUnDealCount.Text = new BLL.Order().getUnDealOrderCount().ToString();
                ChkAdminLevel("sys_checkOrder", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("1=1" + CombSqlTxt(), "o_addDate desc,o_id desc");
            }
            
        }

        #region 初始化
        private void InitData()
        {
            ddlContractPrice.DataSource = Common.BusinessDict.ContractPriceType();
            ddlContractPrice.DataTextField = "value";
            ddlContractPrice.DataValueField = "key";
            ddlContractPrice.DataBind();
            ddlContractPrice.Items.Insert(0, new ListItem("不限", ""));

            ddlstatus.DataSource = Common.BusinessDict.fStatus(1);
            ddlstatus.DataTextField = "value";
            ddlstatus.DataValueField = "key";
            ddlstatus.DataBind();
            ddlstatus.Items.Insert(0, new ListItem("不限", ""));

            ddldstatus.DataSource = Common.BusinessDict.dStatus(1);
            ddldstatus.DataTextField = "value";
            ddldstatus.DataValueField = "key";
            ddldstatus.DataBind();
            ddldstatus.Items.Insert(0, new ListItem("不限", ""));

            ddlispush.DataSource = Common.BusinessDict.pushStatus();
            ddlispush.DataTextField = "value";
            ddlispush.DataValueField = "key";
            ddlispush.DataBind();
            ddlispush.Items.Insert(0, new ListItem("不限", ""));

            ddlflag.DataSource = Common.BusinessDict.checkStatus();
            ddlflag.DataTextField = "value";
            ddlflag.DataValueField = "key";
            ddlflag.DataBind();
            ddlflag.Items.Insert(0, new ListItem("不限", ""));

            ddllock.DataSource = Common.BusinessDict.lockStatus();
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
            if (!this.isSearch)
            {
                this.page = DTRequest.GetQueryInt("page", 1);
            }
            else
            {
                this.page = 1;
            }
            BLL.finance bll = new BLL.finance();
            this.rptList.DataSource = bll.GetApprovalList(this.pageSize, this.page, _strWhere, _orderby, out this.totalCount);
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = backUrl();
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);


            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            ddlContractPrice.SelectedValue = _contractPrice;
            txtsDate.Text = _sdate;
            txteDate.Text = _edate;
            txtsDate1.Text = _sdate1;
            txteDate1.Text = _edate1;
            ddlstatus.SelectedValue = _status;
            ddldstatus.SelectedValue = _dstatus;
            ddlispush.SelectedValue = _pushstatus;
            ddlflag.SelectedValue = _flag;
            ddllock.SelectedValue = _lockstatus;
            txtOrderID.Text = _oid;
            ddlarea.SelectedValue = _area;
            ddlorderarea.SelectedValue = _orderarea;
        }
        #endregion

        private string backUrl()
        {
            return Utils.CombUrlTxt("orderApproval_list.aspx", "page={0}&flag={1}&txtCusName={2}&hCusId={3}&ddlContractPrice={4}&txtsDate={5}&txteDate={6}&txtsDate1={7}&txteDate1={8}&ddlstatus={9}&ddldstatus={10}&ddlispush={11}&ddlflag={12}&ddllock={13}&ddlarea={14}&ddlorderarea={15}", "__id__", flag, _cusName, _cid, _contractPrice, _sdate, _edate, _sdate1, _edate1, _status, _dstatus, _pushstatus, _flag, _lockstatus, _area, _orderarea);
        }

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                strTemp.Append(" and o_cid=" + _cid + "");
            }
            if (!string.IsNullOrEmpty(_cusName))
            {
                strTemp.Append(" and c_name like '%" + _cusName + "%'");
            }
            if (!string.IsNullOrEmpty(_oid))
            {
                strTemp.Append(" and o_id='" + _oid + "'");
            }
            if (!string.IsNullOrEmpty(_contractPrice))
            {
                strTemp.Append(" and o_contractPrice='" + _contractPrice + "'");
            }
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp.Append(" and datediff(s,o_sdate,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp.Append(" and datediff(s,o_sdate,'" + _edate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_sdate1))
            {
                strTemp.Append(" and datediff(s,o_edate,'" + _sdate1 + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate1))
            {
                strTemp.Append(" and datediff(s,o_edate,'" + _edate1 + "')>=0");
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
                    case "2":
                        strTemp.Append(" and exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5)) and not exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5) and (op_dstatus=0 or op_dstatus=1))");
                        break;
                    case "3":
                        strTemp.Append(" and not exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5))");
                        break;
                    case "4":
                        strTemp.Append(" and ((exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5)) and not exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5) and (op_dstatus=0 or op_dstatus=1))) or not exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5)))");
                        break;
                    default:
                        strTemp.Append(" and exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5) and op_dstatus=" + _dstatus + ")");
                        break;
                }
            }
            if (!string.IsNullOrEmpty(_pushstatus))
            {
                strTemp.Append(" and o_isPush='" + _pushstatus + "'");
            }
            if (!string.IsNullOrEmpty(_flag))
            {
                strTemp.Append(" and o_flag=" + _flag + "");
            }
            if (!string.IsNullOrEmpty(_lockstatus))
            {
                strTemp.Append(" and o_lockStatus='" + _lockstatus + "'");
            }
            if (!string.IsNullOrEmpty(_area))
            {
                strTemp.Append(" and o_place like '%" + _area + "%'");
            }
            if (!string.IsNullOrEmpty(_orderarea))
            {
                strTemp.Append(" and op_area='"+ _orderarea + "'");
            }


            if (flag == "0")
            {
                strTemp.Append(" and exists(select * from ms_finance where fin_oid=o_id) and exists(select * from MS_finance where fin_oid=o_id and fin_flag=0)");
            }
            else if (flag == "1")
            {
                strTemp.Append(" and o_lockStatus=0 and exists(select * from ms_finance where fin_oid=o_id) and not exists(select * from MS_finance where fin_oid=o_id and (fin_flag=0 or fin_flag=1))");
            }
            else if (flag == "2")
            {
                strTemp.Append(" and o_lockStatus=1");
            }
            else if (flag == "4")
            {
                strTemp.Append(" and o_lockStatus=2");
            }
            else
            {
                
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("orderApproval_page_size", "DTcmsPage"), out _pagesize))
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
            flag = DTRequest.GetFormString("flag");
            _cusName = DTRequest.GetFormString("txtCusName");
            _cid = DTRequest.GetFormString("hCusId");
            _contractPrice = DTRequest.GetFormString("ddlContractPrice");
            _sdate = DTRequest.GetFormString("txtsDate");
            _edate = DTRequest.GetFormString("txteDate");
            _sdate1 = DTRequest.GetFormString("txtsDate1");
            _edate1 = DTRequest.GetFormString("txteDate1");
            _status = DTRequest.GetFormString("ddlstatus");
            _dstatus = DTRequest.GetFormString("ddldstatus");
            _pushstatus = DTRequest.GetFormString("ddlispush");
            _flag = DTRequest.GetFormString("ddlflag");
            _lockstatus = DTRequest.GetFormString("ddllock");
            _oid = DTRequest.GetFormString("txtOrderID");
            _area = DTRequest.GetFormString("ddlarea");
            _orderarea = DTRequest.GetFormString("ddlorderarea");
            RptBind("1=1" + CombSqlTxt(), "o_addDate desc,o_id desc");

        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {           
            DataTable dt = new BLL.finance().GetApprovalList(this.pageSize, this.page, "1=1" + CombSqlTxt(), "o_addDate desc,o_id desc", out this.totalCount,false).Tables[0];
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=审单列表.xlsx"); //HttpUtility.UrlEncode(fileName));
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet("审单");
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
            headRow.CreateCell(1).SetCellValue("客户");
            headRow.CreateCell(2).SetCellValue("活动名称");
            headRow.CreateCell(3).SetCellValue("合同造价");
            headRow.CreateCell(4).SetCellValue("活动日期");
            headRow.CreateCell(5).SetCellValue("归属地");
            headRow.CreateCell(6).SetCellValue("订单状态");
            headRow.CreateCell(7).SetCellValue("是否推送");
            headRow.CreateCell(8).SetCellValue("上级审批");
            headRow.CreateCell(9).SetCellValue("锁单状态");
            headRow.CreateCell(10).SetCellValue("业务员");
            headRow.CreateCell(11).SetCellValue("待审核应收付");
            headRow.CreateCell(12).SetCellValue("审核未通过应收付");

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
            sheet.SetColumnWidth(5, 10 * 256);
            sheet.SetColumnWidth(6, 20 * 256);
            sheet.SetColumnWidth(7, 15 * 256);
            sheet.SetColumnWidth(8, 15 * 256);
            sheet.SetColumnWidth(9, 20 * 256);
            sheet.SetColumnWidth(10, 15 * 256);
            sheet.SetColumnWidth(11, 15 * 256);
            sheet.SetColumnWidth(12, 15 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["o_id"].ToString());
                    row.CreateCell(1).SetCellValue(dt.Rows[i]["c_name"].ToString());
                    row.CreateCell(2).SetCellValue(dt.Rows[i]["o_content"].ToString());
                    row.CreateCell(3).SetCellValue(dt.Rows[i]["o_contractPrice"].ToString());
                    row.CreateCell(4).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["o_sdate"]).Value.ToString("yyyy-MM-dd") + "/" + ConvertHelper.toDate(dt.Rows[i]["o_edate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(5).SetCellValue(new BLL.department().getAreaText(dt.Rows[i]["o_place"].ToString()));
                    row.CreateCell(6).SetCellValue(Common.BusinessDict.fStatus()[Convert.ToByte(dt.Rows[i]["o_status"])]);
                    row.CreateCell(7).SetCellValue(Common.BusinessDict.pushStatus()[Convert.ToBoolean(dt.Rows[i]["o_isPush"])]);
                    row.CreateCell(8).SetCellValue(Common.BusinessDict.checkStatus()[Convert.ToByte(dt.Rows[i]["o_flag"])]);
                    row.CreateCell(9).SetCellValue(Common.BusinessDict.lockStatus()[Utils.ObjToByte(dt.Rows[i]["o_lockStatus"])]);
                    row.CreateCell(10).SetCellValue(dt.Rows[i]["op_name"].ToString()+""+ dt.Rows[i]["op_number"].ToString());
                    row.CreateCell(11).SetCellValue(dt.Rows[i]["count0"].ToString());
                    row.CreateCell(12).SetCellValue(dt.Rows[i]["count1"].ToString());

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
        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("orderApproval_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }

        
    }
}