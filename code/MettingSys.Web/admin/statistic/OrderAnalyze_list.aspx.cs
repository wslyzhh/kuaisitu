﻿using MettingSys.Common;
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
    public partial class OrderAnalyze_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小
        protected string _orderid = "", _cusName = "", _cid = "", _status = "", _dstatus = "", _lockstatus = "", _content = "", _address = "", _sign = "", _money = "", _person1 = "", _person3 = "", _person5 = "", _sdate = "", _edate = "", _sdate1 = "", _edate1 = "", _area = "", _sign1 = "", _money1 = "", _sign2 = "", _money2 = "", _sign3 = "", _money3 = "", _sign4 = "", _money4 = "", _sign5 = "", _money5 = "", _orderarea = "", _method = "", _pushstatus = "", _flag = "", _sdate2 = "", _edate2 = "";
        Model.manager manager = null;
        protected Model.business_log logmodel = null;
        decimal money1 = 0, money2 = 0, money3 = 0, money4 = 0, money5 = 0, money6 = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            _orderid = DTRequest.GetString("txtOrderID");
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _status = DTRequest.GetString("ddlstatus");
            _dstatus = DTRequest.GetString("ddldstatus");
            _lockstatus = DTRequest.GetString("ddllock");
            _pushstatus = DTRequest.GetString("ddlispush");
            _flag = DTRequest.GetString("ddlflag");
            _content = DTRequest.GetString("txtContent");
            _address = DTRequest.GetString("txtAddress");
            _sign = DTRequest.GetString("ddlsign");
            _money = DTRequest.GetString("txtMoney");
            _person1 = DTRequest.GetString("txtPerson1");
            _person3 = DTRequest.GetString("txtPerson3");
            _person5 = DTRequest.GetString("txtPerson5");
            _sdate = DTRequest.GetString("txtsDate");
            _edate = DTRequest.GetString("txteDate");
            _sdate1 = DTRequest.GetString("txtsDate1");
            _edate1 = DTRequest.GetString("txteDate1");
            _area = DTRequest.GetString("ddlarea");
            _sign1 = DTRequest.GetString("ddlsign1");
            _money1 = DTRequest.GetString("txtMoney1");
            _sign2 = DTRequest.GetString("ddlsign2");
            _money2 = DTRequest.GetString("txtMoney2");
            _sign3 = DTRequest.GetString("ddlsign3");
            _money3 = DTRequest.GetString("txtMoney3");
            _sign4 = DTRequest.GetString("ddlsign4");
            _money4 = DTRequest.GetString("txtMoney4");
            _sign5 = DTRequest.GetString("ddlsign5");
            _money5 = DTRequest.GetString("txtMoney5");
            _orderarea = DTRequest.GetString("ddlorderarea");
            _method = DTRequest.GetString("ddlmethod");
            _sdate2 = DTRequest.GetString("txtsDate2");
            _edate2 = DTRequest.GetString("txteDate2");
            manager = GetAdminInfo();
            this.pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                if (string.IsNullOrEmpty(DTRequest.GetString("page")))
                {
                    string isStatistic = DTRequest.GetString("statistic");
                    if (isStatistic == "0")
                    {
                        if (!string.IsNullOrEmpty(_sdate1))
                        {
                            _sdate1 = _sdate1 + "-01";
                        }
                        if (!string.IsNullOrEmpty(_edate1))
                        {
                            DateTime d = ConvertHelper.toDate(_edate1 + "-01").Value;
                            _edate1 = d.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(DTRequest.GetString("fromReceiveOrder")))//如果是从策划与设计页面或员工未收款统计页面过来的则跳过
                        {
                            int day = DateTime.Now.Day;
                            DateTime date = DateTime.Now.AddDays(-day + 1);
                            _sdate1 = date.AddMonths(-1).ToString("yyyy-MM-dd");
                            _edate1 = date.AddDays(-1).ToString("yyyy-MM-dd");
                        }
                    }
                    
                }
                InitData();
                ChkAdminLevel("sys_order_list", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("1=1" + CombSqlTxt(), "o_addDate desc,o_id desc");
            }

        }

        #region 初始化
        private void InitData()
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
        }
        #endregion

        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.statisticBLL bll = new BLL.statisticBLL();
            DataTable dt = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, out this.totalCount, out money1, out money2, out money3, out money4, out money5, out money6).Tables[0];
            this.rptList.DataSource = dt;
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = backUrl();
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            pCount.Text = dt.Rows.Count.ToString();
            decimal _pmoney1 = 0, _pmoney2 = 0, _pmoney3 = 0, _pmoney4 = 0, _pmoney5 = 0, _pmoney6 = 0;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pmoney1 += Utils.ObjToDecimal(dr["shou"], 0);
                    _pmoney2 += Utils.ObjToDecimal(dr["weishou"], 0);
                    _pmoney3 += Utils.ObjToDecimal(dr["fu"], 0);
                    _pmoney4 += Utils.ObjToDecimal(dr["weifu"], 0);
                    _pmoney5 += Utils.ObjToDecimal(dr["o_financeCust"], 0);
                    _pmoney6 += Utils.ObjToDecimal(dr["profit"], 0);
                }
            }
            tCount.Text = totalCount.ToString();
            pMoney1.Text = _pmoney1.ToString();
            pMoney2.Text = _pmoney2.ToString();
            pMoney3.Text = _pmoney3.ToString();
            pMoney4.Text = _pmoney4.ToString();
            pMoney5.Text = _pmoney5.ToString();
            pMoney6.Text = _pmoney6.ToString();

            tMoney1.Text = money1.ToString();
            tMoney2.Text = money2.ToString();
            tMoney3.Text = money3.ToString();
            tMoney4.Text = money4.ToString();
            tMoney5.Text = money5.ToString();
            tMoney6.Text = money6.ToString();

            txtOrderID.Text = _orderid;
            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            ddlstatus.SelectedValue = _status;
            ddldstatus.SelectedValue = _dstatus;
            ddllock.SelectedValue = _lockstatus;
            txtContent.Text = _content;
            txtAddress.Text = _address;
            ddlsign.SelectedValue = _sign;
            txtMoney.Text = _money;
            txtPerson1.Text = _person1;
            txtPerson3.Text = _person3;
            txtPerson5.Text = _person5;
            txtsDate.Text = _sdate;
            txteDate.Text = _edate;
            txtsDate1.Text = _sdate1;
            txteDate1.Text = _edate1;
            ddlarea.SelectedValue = _area;
            ddlsign1.SelectedValue = _sign1;
            txtMoney1.Text = _money1;
            ddlsign2.SelectedValue = _sign2;
            txtMoney2.Text = _money2;
            ddlsign3.SelectedValue = _sign3;
            txtMoney3.Text = _money3;
            ddlsign4.SelectedValue = _sign4;
            txtMoney4.Text = _money4;
            ddlsign5.SelectedValue = _sign5;
            txtMoney5.Text = _money5;
            ddlorderarea.SelectedValue = _orderarea;
            ddlmethod.SelectedValue = _method;
        }
        #endregion

        private string backUrl()
        {
            return Utils.CombUrlTxt("OrderAnalyze_list.aspx", "page={0}&txtCusName={1}&hCusId={2}&ddlstatus={3}&ddldstatus={4}&ddllock={5}&txtContent={6}&txtAddress={7}&ddlsign={8}&txtMoney={9}&txtPerson1={10}&txtPerson3={11}&txtPerson5={12}&txtsDate={13}&txteDate={14}&txtsDate1={15}&txteDate1={16}&txtOrderID={17}&ddlarea={18}&ddlsign1={19}&txtMoney1={20}&ddlsign2={21}&txtMoney2={22}&ddlsign3={23}&txtMoney3={24}&ddlsign4={25}&txtMoney4={26}&ddlsign5={27}&txtMoney5={28}&ddlispush={29}&ddlflag={30}&txtsDate2={31}&txteDate2={32}&ddlmethod={33}&ddlorderarea={34}", "__id__", _cusName, _cid, _status, _dstatus, _lockstatus, _content, _address, _sign, _money, _person1, _person3, _person5, _sdate, _edate, _sdate1, _edate1, _orderid, _area, _sign1, _money1, _sign2, _money2, _sign3, _money3, _sign4, _money4, _sign5, _money5, _pushstatus, _flag, _sdate2, _edate2,_method,_orderarea);
        }

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_orderid))
            {
                strTemp.Append(" and o_id='" + _orderid + "'");
            }
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                strTemp.Append(" and o_cid=" + _cid + "");
            }
            if (!string.IsNullOrEmpty(_cusName))
            {
                strTemp.Append(" and c_name like '%" + _cusName + "%'");
            }
            if (!string.IsNullOrEmpty(_status))
            {
                if (_status == "3")
                {
                    strTemp.Append(" and (o_status=1 or o_status=2)");
                }
                else
                {
                    strTemp.Append(" and o_status=" + _status + "");
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
            if (!string.IsNullOrEmpty(_dstatus))
            {
                switch (_dstatus)
                {
                    case "5":
                        if (string.IsNullOrEmpty(_person3) && string.IsNullOrEmpty(_person5))
                        {
                            strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and (op_type=3 or op_type=5) and (op_dstatus=0 or op_dstatus=1))");
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(_person3))
                            {
                                strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type=3 and op_number='" + _person3 + "' and (op_dstatus=0 or op_dstatus=1))");
                            }
                            if (!string.IsNullOrEmpty(_person5))
                            {
                                strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type=5 and op_number='" + _person5 + "' and (op_dstatus=0 or op_dstatus=1))");
                            }
                        }
                        break;
                    case "4":
                        strTemp.Append(" and (not exists(select * from MS_OrderPerson where op_oid=o_id and (op_type=3 or op_type=5)) ");
                        if (string.IsNullOrEmpty(_person3) && string.IsNullOrEmpty(_person5))
                        {
                            strTemp.Append(" or not exists(select * from MS_OrderPerson where op_oid=o_id and (op_type=3 or op_type=5) and (op_dstatus=0 or op_dstatus=1))");
                        }
                        else if (!string.IsNullOrEmpty(_person3) && string.IsNullOrEmpty(_person5))
                        {
                            strTemp.Append(" or not exists(select * from MS_OrderPerson where op_oid=o_id and op_type=3 and op_number='" + _person3 + "' and (op_dstatus=0 or op_dstatus=1))");
                        }
                        else if (string.IsNullOrEmpty(_person3) && !string.IsNullOrEmpty(_person5))
                        {
                            strTemp.Append(" or not exists(select * from MS_OrderPerson where op_oid=o_id and op_type=5 and op_number='" + _person5 + "' and (op_dstatus=0 or op_dstatus=1))");
                        }
                        else
                        {
                            strTemp.Append(" or not exists(select * from MS_OrderPerson where op_oid=o_id and (op_type=3 or op_type=5) and (op_number='" + _person3 + "' or op_number='" + _person5 + "') and (op_dstatus=0 or op_dstatus=1))");
                        }
                        strTemp.Append(" )");
                        break;
                    case "3":
                        strTemp.Append(" and not exists(select * from MS_OrderPerson where op_oid=o_id and (op_type=3 or op_type=5))");
                        break;
                    case "2":
                        strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and (op_type=3 or op_type=5))");
                        if (string.IsNullOrEmpty(_person3) && string.IsNullOrEmpty(_person5))
                        {
                            strTemp.Append(" and not exists(select * from MS_OrderPerson where op_oid=o_id and (op_type=3 or op_type=5) and (op_dstatus=0 or op_dstatus=1))");
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(_person3))
                            {
                                strTemp.Append(" and not exists(select * from MS_OrderPerson where op_oid=o_id and op_type=3 and op_number='" + _person3 + "' and (op_dstatus=0 or op_dstatus=1))");
                            }
                            if (!string.IsNullOrEmpty(_person5))
                            {
                                strTemp.Append(" and not exists(select * from MS_OrderPerson where op_oid=o_id and op_type=5 and op_number='" + _person5 + "' and (op_dstatus=0 or op_dstatus=1))");
                            }
                        }
                        break;
                    default:
                        if (string.IsNullOrEmpty(_person3) && string.IsNullOrEmpty(_person5))
                        {
                            strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and (op_type=3 or op_type=5) and op_dstatus=" + _dstatus + ")");
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(_person3))
                            {
                                strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type=3 and op_number='" + _person3 + "' and op_dstatus=" + _dstatus + ")");
                            }
                            if (!string.IsNullOrEmpty(_person5))
                            {
                                strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type=5 and op_number='" + _person5 + "' and op_dstatus=" + _dstatus + ")");
                            }
                        }
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
            if (!string.IsNullOrEmpty(_pushstatus))
            {
                strTemp.Append(" and o_isPush='" + _pushstatus + "'");
            }
            if (!string.IsNullOrEmpty(_flag))
            {
                strTemp.Append(" and o_flag=" + _flag + "");
            }
            if (!string.IsNullOrEmpty(_content))
            {
                strTemp.Append(" and o_content like '%" + _content + "%'");
            }
            if (!string.IsNullOrEmpty(_address))
            {
                strTemp.Append(" and o_address like '%" + _address + "%'");
            }
            if (!string.IsNullOrEmpty(_money1))
            {
                strTemp.Append(" and isnull(shou,0) " + _sign1 + " " + _money1 + "");
            }
            if (!string.IsNullOrEmpty(_money))
            {
                strTemp.Append(" and isnull(weishou,0) " + _sign + " " + _money + "");
            }
            if (!string.IsNullOrEmpty(_money2))
            {
                strTemp.Append(" and isnull(fu,0) " + _sign2 + " " + _money2 + "");
            }
            if (!string.IsNullOrEmpty(_money3))
            {
                strTemp.Append(" and isnull(weifu,0) " + _sign3 + " " + _money3 + "");
            }
            if (!string.IsNullOrEmpty(_money4))
            {
                strTemp.Append(" and isnull(o_financeCust,0) " + _sign4 + " " + _money4 + "");
            }
            if (!string.IsNullOrEmpty(_money5))
            {
                strTemp.Append(" and isnull(profit,0) " + _sign5 + " " + _money5 + "");
            }
            if (!string.IsNullOrEmpty(_person1))
            {
                strTemp.Append(" and (op_number='" + _person1 + "' or op_name='" + _person1 + "')");
            }
            if (!string.IsNullOrEmpty(_person3))
            {
                strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =3 and (op_number ='" + _person3 + "' or op_name ='" + _person3 + "'))");
            }
            if (!string.IsNullOrEmpty(_person5))
            {
                strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =5 and (op_number ='" + _person5 + "' or op_name ='" + _person5 + "'))");
            }
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp.Append(" and datediff(day,o_sdate,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp.Append(" and datediff(day,o_sdate,'" + _edate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_sdate1))
            {
                strTemp.Append(" and datediff(day,o_edate,'" + _sdate1 + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate1))
            {
                strTemp.Append(" and datediff(day,o_edate,'" + _edate1 + "')>=0");
            }
            if (!string.IsNullOrEmpty(_method))
            {
                strTemp.Append(" and exists(select * from MS_ReceiptPayDetail where rpd_oid=o_id and rpd_type=1 and rpd_method=" + _method + ")");
            }
            if (!string.IsNullOrEmpty(_sdate2))
            {
                strTemp.Append(" and datediff(day,o_statusTime,'" + _sdate2 + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate2))
            {
                strTemp.Append(" and datediff(day,o_statusTime,'" + _edate2 + "')>=0");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("OrderAnalyze_size", "DTcmsPage"), out _pagesize))
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
            _orderid = DTRequest.GetFormString("txtOrderID");
            _cusName = DTRequest.GetFormString("txtCusName");
            _cid = DTRequest.GetFormString("hCusId");
            _status = DTRequest.GetFormString("ddlstatus");
            _dstatus = DTRequest.GetFormString("ddldstatus");
            _lockstatus = DTRequest.GetFormString("ddllock");
            _content = DTRequest.GetFormString("txtContent");
            _address = DTRequest.GetFormString("txtAddress");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _person1 = DTRequest.GetFormString("txtPerson1");
            _person3 = DTRequest.GetFormString("txtPerson3");
            _person5 = DTRequest.GetFormString("txtPerson5");
            _sdate = DTRequest.GetFormString("txtsDate");
            _edate = DTRequest.GetFormString("txteDate");
            _sdate1 = DTRequest.GetFormString("txtsDate1");
            _edate1 = DTRequest.GetFormString("txteDate1");
            _area = DTRequest.GetFormString("ddlarea");
            _sign1 = DTRequest.GetFormString("ddlsign1");
            _money1 = DTRequest.GetFormString("txtMoney1");
            _sign2 = DTRequest.GetFormString("ddlsign2");
            _money2 = DTRequest.GetFormString("txtMoney2");
            _sign3 = DTRequest.GetFormString("ddlsign3");
            _money3 = DTRequest.GetFormString("txtMoney3");
            _sign4 = DTRequest.GetFormString("ddlsign4");
            _money4 = DTRequest.GetFormString("txtMoney4");
            _sign5 = DTRequest.GetFormString("ddlsign5");
            _money5 = DTRequest.GetFormString("txtMoney5");
            _orderarea = DTRequest.GetFormString("ddlorderarea");
            _sdate2 = DTRequest.GetFormString("txtsDate2");
            _edate2 = DTRequest.GetFormString("txteDate2");
            RptBind("1=1" + CombSqlTxt(), "o_addDate desc,o_id desc");
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("OrderAnalyze_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _orderid = DTRequest.GetFormString("txtOrderID");
            _cusName = DTRequest.GetFormString("txtCusName");
            _cid = DTRequest.GetFormString("hCusId");
            _status = DTRequest.GetFormString("ddlstatus");
            _dstatus = DTRequest.GetFormString("ddldstatus");
            _lockstatus = DTRequest.GetFormString("ddllock");
            _pushstatus = DTRequest.GetFormString("ddlispush");
            _flag = DTRequest.GetFormString("ddlflag");
            _content = DTRequest.GetFormString("txtContent");
            _address = DTRequest.GetFormString("txtAddress");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _person1 = DTRequest.GetFormString("txtPerson1");
            _person3 = DTRequest.GetFormString("txtPerson3");
            _person5 = DTRequest.GetFormString("txtPerson5");
            _sdate = DTRequest.GetFormString("txtsDate");
            _edate = DTRequest.GetFormString("txteDate");
            _sdate1 = DTRequest.GetFormString("txtsDate1");
            _edate1 = DTRequest.GetFormString("txteDate1");
            _area = DTRequest.GetFormString("ddlarea");
            _sign1 = DTRequest.GetFormString("ddlsign1");
            _money1 = DTRequest.GetFormString("txtMoney1");
            _sign2 = DTRequest.GetFormString("ddlsign2");
            _money2 = DTRequest.GetFormString("txtMoney2");
            _sign3 = DTRequest.GetFormString("ddlsign3");
            _money3 = DTRequest.GetFormString("txtMoney3");
            _sign4 = DTRequest.GetFormString("ddlsign4");
            _money4 = DTRequest.GetFormString("txtMoney4");
            _sign5 = DTRequest.GetFormString("ddlsign5");
            _money5 = DTRequest.GetFormString("txtMoney5");
            _orderarea = DTRequest.GetFormString("ddlorderarea");
            _sdate2 = DTRequest.GetFormString("txtsDate2");
            _edate2 = DTRequest.GetFormString("txteDate2");
            BLL.statisticBLL bll = new BLL.statisticBLL();
            DataTable dt = bll.GetList(this.pageSize, this.page, "1=1" + CombSqlTxt(), "o_addDate desc,o_id desc", out this.totalCount, out money1, out money2, out money3, out money4, out money5, out money6, false).Tables[0];
            
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=订单分析.xlsx"); //HttpUtility.UrlEncode(fileName));
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
            headRow.CreateCell(1).SetCellValue("活动名称");
            headRow.CreateCell(2).SetCellValue("地点");
            headRow.CreateCell(3).SetCellValue("客户");
            headRow.CreateCell(4).SetCellValue("活动日期");
            headRow.CreateCell(5).SetCellValue("归属地");
            headRow.CreateCell(6).SetCellValue("订单状态");
            headRow.CreateCell(7).SetCellValue("锁单状态");
            headRow.CreateCell(8).SetCellValue("业务员");
            headRow.CreateCell(9).SetCellValue("策划人员");
            headRow.CreateCell(10).SetCellValue("设计人员");
            headRow.CreateCell(11).SetCellValue("应收款");
            headRow.CreateCell(12).SetCellValue("未收款");
            headRow.CreateCell(13).SetCellValue("应付款");
            headRow.CreateCell(14).SetCellValue("未付款");
            headRow.CreateCell(15).SetCellValue("税费");
            headRow.CreateCell(16).SetCellValue("业绩利润");
            headRow.CreateCell(17).SetCellValue("确认时间");

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
            headRow.GetCell(15).CellStyle = titleCellStyle;
            headRow.GetCell(16).CellStyle = titleCellStyle;
            headRow.GetCell(17).CellStyle = titleCellStyle;

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
            sheet.SetColumnWidth(15, 20 * 256);
            sheet.SetColumnWidth(16, 20 * 256);
            sheet.SetColumnWidth(17, 25 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["o_id"].ToString());
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["o_content"]));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["o_address"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["c_name"]));
                    row.CreateCell(4).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["o_sdate"]).Value.ToString("yyyy-MM-dd")+"/"+ ConvertHelper.toDate(dt.Rows[i]["o_edate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(5).SetCellValue(new MettingSys.BLL.department().getAreaText(dt.Rows[i]["o_place"].ToString()));
                    row.CreateCell(6).SetCellValue(BusinessDict.fStatus()[Utils.ObjToByte(dt.Rows[i]["o_status"])]);
                    row.CreateCell(7).SetCellValue(BusinessDict.lockStatus()[Utils.ObjToByte(dt.Rows[i]["o_lockStatus"])]);
                    row.CreateCell(8).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["op_name"]));
                    row.CreateCell(9).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["person3"]));
                    row.CreateCell(10).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["person4"]));
                    row.CreateCell(11).SetCellValue(Utils.ObjToDecimal(dt.Rows[i]["shou"], 0).ToString());
                    row.CreateCell(12).SetCellValue(Utils.ObjToDecimal(dt.Rows[i]["weishou"], 0).ToString());
                    row.CreateCell(13).SetCellValue(Utils.ObjToDecimal(dt.Rows[i]["fu"], 0).ToString());
                    row.CreateCell(14).SetCellValue(Utils.ObjToDecimal(dt.Rows[i]["weifu"], 0).ToString());
                    row.CreateCell(15).SetCellValue(Utils.ObjToDecimal(dt.Rows[i]["o_financeCust"], 0).ToString());
                    row.CreateCell(16).SetCellValue(Utils.ObjToDecimal(dt.Rows[i]["profit"], 0).ToString());
                    row.CreateCell(17).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["o_statusTime"]) == "" ? "" : Utils.ObjectToDateTime(dt.Rows[i]["o_statusTime"]).ToString("yyyy-MM-dd HH:mm:ss"));

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
                    row.GetCell(15).CellStyle = cellStyle;
                    row.GetCell(16).CellStyle = cellStyle;
                    row.GetCell(17).CellStyle = cellStyle;
                }
            }

            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }
    }
}