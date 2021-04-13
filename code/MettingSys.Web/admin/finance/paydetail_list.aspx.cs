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
    public partial class paydetail_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string _cusName = "", _cid = "", _check1 = string.Empty, _check2 = string.Empty, _check3 = string.Empty, _foresdate = string.Empty, _foreedate = string.Empty, _collect = "", _self = "", _person = "", _sign = "", _money = "", _oID = string.Empty, _area = string.Empty, _person1 = "", _sdate = "", _edate = "", _num = "";
        protected string _check = "", orderby = "rpd_adddate desc,rpd_id desc";
        protected Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _check = DTRequest.GetString("check");
            if (string.IsNullOrEmpty(this._check))
            {
                this._check = "0";
            }
            pageSize = GetPageSize(10); //每页数量
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _check1 = DTRequest.GetString("ddlcheck1"); 
            _check2 = DTRequest.GetString("ddlcheck2"); 
            _check3 = DTRequest.GetString("ddlcheck3"); 
            _foresdate = DTRequest.GetString("txtforesdate");
            _foreedate = DTRequest.GetString("txtforeedate");
            _collect = DTRequest.GetString("ddlcollect");
            _self = DTRequest.GetString("self");
            _person = DTRequest.GetString("txtPerson");
            _sign = DTRequest.GetString("ddlsign");
            _money = DTRequest.GetString("txtMoney");
            _oID = DTRequest.GetString("txtorderid");
            _area = DTRequest.GetString("ddlarea");
            _person1 = DTRequest.GetString("txtPerson1");
            _sdate = DTRequest.GetString("txtsdate");
            _edate = DTRequest.GetString("txtedate");
            _num = DTRequest.GetString("txtnum");
            manager = GetAdminInfo();
            switch (this._check)
            {
                case "1":
                    this._check1 = "0";
                    break;
                case "2":
                    this._check1 = "2";
                    this._check2 = "0";
                    orderby = "rpd_checkTime1 asc,rpd_id desc";//财务未审批页签按“部门审批”的时间降序排列
                    break;
                case "3":
                    this._check1 = "2";
                    this._check2 = "2";
                    this._check3 = "0";
                    orderby = "rpd_checkTime2 asc,rpd_id desc";//总经理未审批页签按“财务审批”的时间降序排列
                    break;
                default:
                    break;
            }
            if (_self == "1")
            {
                titleDiv.Visible = false;
                checkLi.Visible = false;
                uncollectLi.Visible = false;
            }
            if (!Page.IsPostBack)
            {
                if (string.IsNullOrEmpty(DTRequest.GetString("page")))
                {
                    _collect = "False";
                }
                InitData();
                if (_self != "1")
                {
                    ChkAdminLevel("sys_payment_detail0", DTEnums.ActionEnum.View.ToString()); //检查权限
                }
                RptBind("rpd_type=0" + CombSqlTxt(), orderby);
            }
            
        }
        #region 初始化数据=================================
        private void InitData()
        {
            //部门审批状态
            ddlcheck1.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck1.DataTextField = "value";
            ddlcheck1.DataValueField = "key";
            ddlcheck1.DataBind();
            ddlcheck1.Items.Insert(0, new ListItem("不限", ""));
            //财务审批状态
            ddlcheck2.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck2.DataTextField = "value";
            ddlcheck2.DataValueField = "key";
            ddlcheck2.DataBind();
            ddlcheck2.Items.Insert(0, new ListItem("不限", ""));
            //总经理审批状态
            ddlcheck3.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck3.DataTextField = "value";
            ddlcheck3.DataValueField = "key";
            ddlcheck3.DataBind();
            ddlcheck3.Items.Insert(0, new ListItem("不限", ""));

            ddlcheck.DataSource = Common.BusinessDict.checkStatus();
            ddlcheck.DataTextField = "value";
            ddlcheck.DataValueField = "key";
            ddlcheck.DataBind();
            ddlcheck.Items.Insert(0, new ListItem("请选择", ""));

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
            ddlmethod.Items.Insert(0, new ListItem("请选择", ""));

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
            DataTable dt = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, manager, out this.totalCount, out decimal _tmoney, true).Tables[0];
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
            ddlchecktype.SelectedValue = this._check;
            ddlcheck1.SelectedValue = this._check1;
            ddlcheck2.SelectedValue = this._check2;
            ddlcheck3.SelectedValue = this._check3;
            txtforesdate.Text = _foresdate;
            txtforeedate.Text = _foreedate;
            ddlcollect.SelectedValue = _collect;
            txtPerson.Text = _person;
            ddlsign.SelectedValue = _sign;
            txtMoney.Text = _money;
            txtorderid.Text = _oID;
            ddlarea.SelectedValue = _area;
            txtPerson1.Text = _person1;
            txtsdate.Text = _sdate;
            txtedate.Text = _edate;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            //所有页签下保留可以看到！只是针对HQ工号中有部门审批权限的，他的部门审批页签里只看本区域的
            if (_check == "1" && new BLL.permission().checkHasPermission(manager, "0603"))
            {
                strTemp.Append(" and rpd_area='" + manager.area + "'");
            }

            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                strTemp.Append(" and rpd_cid = " + _cid + "");
            }
            if (!string.IsNullOrEmpty(_cusName))
            {
                strTemp.Append(" and c_name like '%" + _cusName + "%'");
            }
            if (!string.IsNullOrEmpty(_check1))
            {
                strTemp.Append(" and rpd_flag1=" + _check1 + "");
            }
            if (!string.IsNullOrEmpty(_check2))
            {
                strTemp.Append(" and rpd_flag2=" + _check2 + "");
            }
            if (!string.IsNullOrEmpty(_check3))
            {
                strTemp.Append(" and rpd_flag3=" + _check3 + "");
            }
            if (!string.IsNullOrEmpty(_foresdate))
            {
                strTemp.Append(" and datediff(day,rpd_foreDate,'" + _foresdate + "')<=0 ");
            }
            if (!string.IsNullOrEmpty(_foreedate))
            {
                strTemp.Append(" and datediff(day,rpd_foreDate,'" + _foreedate + "')>=0 ");
            }
            if (!string.IsNullOrEmpty(_collect))
            {
                if (_collect == "True")
                {
                    strTemp.Append(" and isnull(rpd_rpid,0)>0");
                }
                else
                {
                    strTemp.Append(" and isnull(rpd_rpid,0)=0");
                }
            }
            if (_self == "1")
            {
                strTemp.Append(" and rpd_personNum='" + manager.user_name + "'");
            }
            if (!string.IsNullOrEmpty(_person.ToUpper()))
            {
                strTemp.Append(" and (rpd_personNum like '%" + _person.ToUpper() + "%' or rpd_personName like '%" + _person.ToUpper() + "%')");
            }
            if (!string.IsNullOrEmpty(_money))
            {
                strTemp.Append(" and rpd_money " + _sign + " " + _money + "");
            }
            if (!string.IsNullOrEmpty(_oID))
            {
                strTemp.Append(" and rpd_oid='" + _oID + "'");
            }
            if (!string.IsNullOrEmpty(_area))
            {
                strTemp.Append(" and rpd_area='" + _area + "'");
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
                strTemp.Append(" and datediff(day,rp_date,'" + _sdate + "')>=0 ");
            }
            if (!string.IsNullOrEmpty(_num))
            {
                strTemp.Append(" and rpd_num like '%" + _num + "%'");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("paydetail_page_size", "DTcmsPage"), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    return _pagesize;
                }
            }
            return _default_size;
        }
        #endregion

        //查询
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.isSearch = true;
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _check1 = DTRequest.GetFormString("ddlcheck1");
            _check2 = DTRequest.GetFormString("ddlcheck2");
            _check3 = DTRequest.GetFormString("ddlcheck3");
            _foresdate = DTRequest.GetFormString("txtforesdate");
            _foreedate = DTRequest.GetFormString("txtforeedate");
            _collect = DTRequest.GetFormString("ddlcollect");
            _self = DTRequest.GetFormString("self");
            _person = DTRequest.GetFormString("txtPerson");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _oID = DTRequest.GetFormString("txtorderid");
            _area = DTRequest.GetFormString("ddlarea");
            _person1 = DTRequest.GetFormString("txtPerson1");
            _sdate = DTRequest.GetFormString("txtsdate");
            _edate = DTRequest.GetFormString("txtedate");
            _num = DTRequest.GetFormString("txtnum");
            RptBind("rpd_type=0" + CombSqlTxt(), orderby);
            
        }

        private string backUrl()
        {
            return Utils.CombUrlTxt("paydetail_list.aspx", "page={0}&ddlcheck1={1}&ddlcheck2={2}&ddlcheck3={3}&txtforesdate={4}&txtforeedate={5}&self={6}&txtCusName={7}&hCusId={8}&ddlcollect={9}&txtPerson={10}&ddlsign={11}&txtmoney={12}&txtorderid={13}&ddlarea={14}&txtPerson1={15}&txtsdate={16}&txtedate={17}&check={18}&txtnum={19}", "__id__", _check1, _check2, _check3, _foresdate, _foreedate, _self, _cusName, _cid, _collect, _person, _sign, _money, _oID, _area, _person1, _sdate, _edate,_check,_num);
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("paydetail_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }

        //批量删除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (_self != "1")
            {
                ChkAdminLevel("sys_payment_detail0", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            }
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
                        //删除文件
                        if (Directory.Exists(Server.MapPath("~/uploadPay/1/" + id + "/")))
                        {
                            Directory.Delete(Server.MapPath("~/uploadPay/1/" + id + "/"), true);
                        }
                    }
                    else
                    {
                        error++;
                        sb.Append(result + "<br/>");
                    }
                }
            }
            JscriptMsg("共选择" + (success + error) + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), Utils.CombUrlTxt("paydetail_list.aspx", "page={0}&ddlcheck1={1}&ddlcheck2={2}&ddlcheck3={3}&txtforesdate={4}&txtforeedate={5}&self={6}&txtCusName={7}&hCusId={8}&ddlcollect={9}&txtPerson={10}&ddlsign={11}&txtmoney={12}&txtorderid={13}&ddlarea={14}&txtPerson1={15}&txtsdate={16}&txtedate={17}", "__id__", _check1, _check2, _check3, _foresdate, _foreedate, _self, _cusName, _cid,_collect,_person,_sign,_money,_oID,_area,_person1,_sdate,_edate));
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _check1 = DTRequest.GetFormString("ddlcheck1");
            _check2 = DTRequest.GetFormString("ddlcheck2");
            _check3 = DTRequest.GetFormString("ddlcheck3");
            _foresdate = DTRequest.GetFormString("txtforesdate");
            _foreedate = DTRequest.GetFormString("txtforeedate");
            _collect = DTRequest.GetFormString("ddlcollect");
            _self = DTRequest.GetFormString("self");
            _person = DTRequest.GetFormString("txtPerson");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _oID = DTRequest.GetFormString("txtorderid");
            _area = DTRequest.GetFormString("ddlarea");
            _person1 = DTRequest.GetFormString("txtPerson1");
            _sdate = DTRequest.GetFormString("txtsdate");
            _edate = DTRequest.GetFormString("txtedate");
            _num = DTRequest.GetFormString("txtnum");
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            DataTable dt = bll.GetList(this.pageSize, this.page, "rpd_type=0" + CombSqlTxt(), "rpd_adddate desc,rpd_id desc", manager, out this.totalCount, out decimal _tmoney, true, false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=付款明细列表.xlsx"); //HttpUtility.UrlEncode(fileName));
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
            headRow.CreateCell(1).SetCellValue("付款对象");
            headRow.CreateCell(2).SetCellValue("客户银行账号");
            headRow.CreateCell(3).SetCellValue("付款内容");
            headRow.CreateCell(4).SetCellValue("付款金额");
            headRow.CreateCell(5).SetCellValue("预付日期");
            headRow.CreateCell(6).SetCellValue("付款方式");
            headRow.CreateCell(7).SetCellValue("申请人");
            headRow.CreateCell(8).SetCellValue("区域");
            headRow.CreateCell(9).SetCellValue("部门审批");
            headRow.CreateCell(10).SetCellValue("财务审批");
            headRow.CreateCell(11).SetCellValue("总经理审批");
            headRow.CreateCell(12).SetCellValue("付款人");
            headRow.CreateCell(13).SetCellValue("实付日期");
            headRow.CreateCell(14).SetCellValue("对账标识");

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
            headRow.GetCell(13).CellStyle = titleCellStyle;
            headRow.GetCell(14).CellStyle = titleCellStyle;

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
            sheet.SetColumnWidth(13, 20 * 256);
            sheet.SetColumnWidth(14, 20 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["rpd_oid"].ToString());
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_name"]));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["cb_bankName"]) + "\r\n"+ Utils.ObjectToStr(dt.Rows[i]["cb_bankNum"]) + "\r\n" + Utils.ObjectToStr(dt.Rows[i]["cb_bank"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rpd_content"]));
                    row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rpd_money"]));
                    row.CreateCell(5).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["rpd_foredate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(6).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["pm_name"]));
                    row.CreateCell(7).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rpd_personNum"]) + "-" + Utils.ObjectToStr(dt.Rows[i]["rpd_personName"]));
                    row.CreateCell(8).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rpd_area"]));
                    row.CreateCell(9).SetCellValue(BusinessDict.checkStatus()[Utils.ObjToByte(dt.Rows[i]["rpd_flag1"].ToString())]);
                    row.CreateCell(10).SetCellValue(BusinessDict.checkStatus()[Utils.ObjToByte(dt.Rows[i]["rpd_flag2"].ToString())]);
                    row.CreateCell(11).SetCellValue(BusinessDict.checkStatus()[Utils.ObjToByte(dt.Rows[i]["rpd_flag3"].ToString())]);
                    row.CreateCell(12).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rp_confirmerName"]));
                    row.CreateCell(13).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["rp_date"]) == null ? "" : ConvertHelper.toDate(dt.Rows[i]["rp_date"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(14).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rpd_num"]));

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
                    row.GetCell(13).CellStyle = cellStyle;
                    row.GetCell(14).CellStyle = cellStyle;
                }
            }

            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }
    }
}