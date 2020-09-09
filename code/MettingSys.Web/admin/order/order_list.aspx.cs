using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MettingSys.Common;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using BorderStyle = NPOI.SS.UserModel.BorderStyle;
using NPOI.HSSF.Util;

namespace MettingSys.Web.admin.order
{
    public partial class order_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小
        protected string _type = "";//type=check时是订单业务审批列表
        protected string flag = "", _orderid = "", _cusName = "", _cid = "", _contractPrice = "", _status = "", _dstatus = "", _pushstatus = "", _flag = "", _lockstatus = "", _content = "", _address = "", _sign = "", _money = "", _person1 = "", _person2 = "", _person3 = "", _person4 = "", _person5 = "", _sdate = "", _edate = "", _sdate1 = "", _edate1 = "", _area = "", _moneyType = "", _orderarea = "", _sdate2 = "", _edate2 = "";
        Model.manager manager = null;
        protected Model.business_log logmodel = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            flag = DTRequest.GetString("flag");
            _orderid = DTRequest.GetString("txtOrderID");
            _type = DTRequest.GetString("type");
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _contractPrice = DTRequest.GetString("ddlContractPrice"); 
            _status = DTRequest.GetString("ddlstatus"); 
            _dstatus = DTRequest.GetString("ddldstatus"); 
            _pushstatus = DTRequest.GetString("ddlispush"); 
            _flag = DTRequest.GetString("ddlflag"); 
            _lockstatus = DTRequest.GetString("ddllock");
            _content = DTRequest.GetString("txtContent");
            _address = DTRequest.GetString("txtAddress");
            _moneyType = DTRequest.GetString("ddlmoneyType");
            _sign = DTRequest.GetString("ddlsign");
            _money = DTRequest.GetString("txtMoney");
            _person1 = DTRequest.GetString("txtPerson1");
            _person2 = DTRequest.GetString("txtPerson2");
            _person3 = DTRequest.GetString("txtPerson3");
            _person4 = DTRequest.GetString("txtPerson4");
            _person5 = DTRequest.GetString("txtPerson5");
            _sdate = DTRequest.GetString("txtsDate");
            _edate = DTRequest.GetString("txteDate");
            _sdate1 = DTRequest.GetString("txtsDate1");
            _edate1 = DTRequest.GetString("txteDate1");
            _area = DTRequest.GetString("ddlarea");
            _sdate2 = DTRequest.GetString("txtsDate2");
            _edate2 = DTRequest.GetString("txteDate2");
            _orderarea = DTRequest.GetString("ddlorderarea");
            if (string.IsNullOrEmpty(flag))
            {
                flag = "0";
            }
            if (_type == "check")
            {
                li1.Visible = false;
                li2.Visible = false;
                li3.Visible = false;
                li4.Visible = false;
                li5.Visible = false;
                _pushstatus = "True";
                _flag = "0";
            }

            manager = GetAdminInfo();
            this.pageSize = GetPageSize(10); //每页数量
            if (flag=="1")
            {
                _person1 = manager.user_name;
                txtPerson1.Enabled = false;
            }
            else if (flag == "2")
            {
                _person2 = manager.user_name;
                txtPerson2.Enabled = false;
            }
            else if (flag == "3")
            {
                _person3 = manager.user_name;
                txtPerson3.Enabled = false;
            }
            else if (flag == "4")
            {
                _person4 = manager.user_name;
                txtPerson4.Enabled = false;
            }
            else if (flag == "5")
            {
                _person5 = manager.user_name;
                txtPerson5.Enabled = false;
            }
            if (!Page.IsPostBack)
            {
                if (flag != "0" && flag != "1" && flag != "2" && flag != "4" && string.IsNullOrEmpty(DTRequest.GetString("page")))
                {
                    _lockstatus = "0";
                }
                InitData();
                ChkAdminLevel("sys_order_list", DTEnums.ActionEnum.View.ToString()); //检查权限
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

            ddlstatus.DataSource = Common.BusinessDict.fStatus();
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
            BLL.Order bll = new BLL.Order();
            this.rptList.DataSource = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, out this.totalCount);
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = backUrl();
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            txtOrderID.Text = _orderid;
            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            ddlContractPrice.SelectedValue = _contractPrice;
            ddlstatus.SelectedValue = _status;
            ddldstatus.SelectedValue = _dstatus;
            ddlispush.SelectedValue = _pushstatus;
            ddlflag.SelectedValue = _flag;
            ddllock.SelectedValue = _lockstatus;
            txtContent.Text = _content;
            txtAddress.Text = _address;
            ddlmoneyType.SelectedValue = _moneyType;
            ddlsign.SelectedValue = _sign;
            txtMoney.Text = _money;
            txtPerson1.Text = _person1;
            txtPerson2.Text = _person2;
            txtPerson3.Text = _person3;
            txtPerson4.Text = _person4;
            txtPerson5.Text = _person5;
            txtsDate.Text = _sdate;
            txteDate.Text = _edate;
            txtsDate1.Text = _sdate1;
            txteDate1.Text = _edate1;
            ddlarea.SelectedValue = _area;
            txtsDate2.Text = _sdate2;
            txteDate2.Text = _edate2;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_orderid))
            {
                strTemp.Append(" and o_id='"+ _orderid + "'");
            }
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                strTemp.Append(" and o_cid=" + _cid + "");
            }
            if (!string.IsNullOrEmpty(_cusName))
            {
                strTemp.Append(" and c_name like '%" + _cusName + "%'");
            }
            if (!string.IsNullOrEmpty(_contractPrice))
            {
                strTemp.Append(" and o_contractPrice='" + _contractPrice + "'");
            }
            if (!string.IsNullOrEmpty(_status))
            {
                strTemp.Append(" and o_status=" + _status + "");
            }
            if (!string.IsNullOrEmpty(_area))
            {
                strTemp.Append(" and o_place like '%" + _area + "%'");
            }
            if (!string.IsNullOrEmpty(_orderarea))
            {
                strTemp.Append(" and op_area like '%" + _orderarea + "%'");
            }
            if (!string.IsNullOrEmpty(_dstatus))
            {
                switch (_dstatus)
                {
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
            if (!string.IsNullOrEmpty(_pushstatus))
            {
                strTemp.Append(" and o_isPush='" + _pushstatus + "'");
            }
            if (!string.IsNullOrEmpty(_flag))
            {
                strTemp.Append(" and isnull(o_flag,0)=" + _flag + "");
            }
            if (!string.IsNullOrEmpty(_lockstatus))
            {
                strTemp.Append(" and isnull(o_lockStatus,0)='" + _lockstatus + "'");
            }
            if (!string.IsNullOrEmpty(_content))
            {
                strTemp.Append(" and o_content like '%" + _content + "%'");
            }
            if (!string.IsNullOrEmpty(_address))
            {
                strTemp.Append(" and o_address like '%" + _address + "%'");
            }
            if (!string.IsNullOrEmpty(_money))
            {
                if (_moneyType == "0")
                {
                    strTemp.Append(" and finMoney " + _sign + " " + _money + "");
                }
                else
                {
                    strTemp.Append(" and finMoney - rpdMoney " + _sign + " " + _money + "");
                }
            }
            if (!string.IsNullOrEmpty(_person1))
            {
                strTemp.Append(" and (op_number='" + _person1 + "' or op_name='" + _person1 + "')");
            }
            if (!string.IsNullOrEmpty(_person2))
            {
                strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =2 and (op_number = '" + _person2 + "' or op_name = '" + _person2 + "'))");
            }
            if (!string.IsNullOrEmpty(_person3))
            {
                strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =3 and (op_number ='" + _person3 + "' or op_name ='" + _person3 + "'))");
            }
            if (!string.IsNullOrEmpty(_person4))
            {
                strTemp.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =4 and (op_number='" + _person4 + "' or op_name='" + _person4 + "'))");
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
            if (!string.IsNullOrEmpty(_sdate2))
            {
                strTemp.Append(" and datediff(day,o_statusTime,'" + _sdate2 + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate2))
            {
                strTemp.Append(" and datediff(day,o_statusTime,'" + _edate2 + "')>=0");
            }

            if (flag == "0")
            {
                if (_type == "check")
                {
                    if (!new BLL.permission().checkHasPermission(manager, "0603"))
                    {
                        string msgbox = "parent.jsdialog(\"错误提示\", \"您没有管理该页面的权限，请勿非法进入！\", \"back\")";
                        Response.Write("<script type=\"text/javascript\">" + msgbox + "</script>");
                        Response.End();
                    }
                    if (manager.area != new BLL.department().getGroupArea())
                    {
                        strTemp.Append(" and op_area='" + manager.area + "'");
                    }
                }
                else
                {
                    //列表权限控制
                    if (manager.area != new BLL.department().getGroupArea())//如果不是总部的工号
                    {
                        if (new BLL.permission().checkHasPermission(manager, "0602"))
                        {
                            //含有区域权限可以查看本区域添加的
                            strTemp.Append(" and (op_area='" + manager.area + "' or o_place like '%" + manager.area + "%')");
                        }
                        else
                        {
                            //只能
                            strTemp.Append(" and op_number='" + manager.user_name + "'");
                        }
                    }
                }
            }
            else if (flag == "1")
            {
                strTemp.Append(" and op_number='" + manager.user_name + "'");
            }
            else if (flag == "2")
            {
                strTemp.Append(" and exists (select * from ms_orderperson where o_id=op_oid and op_type=2 and op_number='" + manager.user_name + "')");
            }
            else if (flag == "3")
            {
                strTemp.Append(" and exists (select * from ms_orderperson where o_id=op_oid and op_type=3 and op_number='" + manager.user_name + "')");
            }
            else if (flag == "4")
            {
                strTemp.Append(" and exists (select * from ms_orderperson where o_id=op_oid and op_type=4 and op_number='" + manager.user_name + "')");
            }
            else {
                strTemp.Append(" and exists (select * from ms_orderperson where o_id=op_oid and op_type=5 and op_number='" + manager.user_name + "')");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("Order_page_size", "DTcmsPage"), out _pagesize))
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
            _type = DTRequest.GetFormString("type");
            _orderid = DTRequest.GetFormString("txtOrderID");
            _cusName = DTRequest.GetFormString("txtCusName");
            _cid = DTRequest.GetFormString("hCusId");
            _contractPrice = DTRequest.GetFormString("ddlContractPrice");
            _status = DTRequest.GetFormString("ddlstatus");
            _dstatus = DTRequest.GetFormString("ddldstatus");
            _pushstatus = DTRequest.GetFormString("ddlispush");
            _flag = DTRequest.GetFormString("ddlflag");
            _lockstatus = DTRequest.GetFormString("ddllock");
            _content = DTRequest.GetFormString("txtContent");
            _address = DTRequest.GetFormString("txtAddress");
            _moneyType = DTRequest.GetFormString("ddlmoneyType");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _person1 = DTRequest.GetFormString("txtPerson1");
            _person2 = DTRequest.GetFormString("txtPerson2");
            _person3 = DTRequest.GetFormString("txtPerson3");
            _person4 = DTRequest.GetFormString("txtPerson4");
            _person5 = DTRequest.GetFormString("txtPerson5");
            _sdate = DTRequest.GetFormString("txtsDate");
            _edate = DTRequest.GetFormString("txteDate");
            _sdate1 = DTRequest.GetFormString("txtsDate1");
            _edate1 = DTRequest.GetFormString("txteDate1");
            _sdate2 = DTRequest.GetFormString("txtsDate2");
            _edate2 = DTRequest.GetFormString("txteDate2");
            _area = DTRequest.GetFormString("ddlarea");
            _orderarea = DTRequest.GetFormString("ddlorderarea");
            if (flag == "1")
            {
                _person1 = manager.user_name;
                txtPerson1.Enabled = false;
            }
            else if (flag == "2")
            {
                _person2 = manager.user_name;
                txtPerson2.Enabled = false;
            }
            else if (flag == "3")
            {
                _person3 = manager.user_name;
                txtPerson3.Enabled = false;
            }
            else if (flag == "4")
            {
                _person4 = manager.user_name;
                txtPerson4.Enabled = false;
            }
            else if (flag == "5")
            {
                _person5 = manager.user_name;
                txtPerson5.Enabled = false;
            }
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
                    Utils.WriteCookie("Order_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(backUrl());
        }
        private string backUrl()
        {
            return Utils.CombUrlTxt("order_list.aspx", "page={0}&flag={1}&type={2}&txtCusName={3}&hCusId={4}&ddlContractPrice={5}&ddlstatus={6}&ddldstatus={7}&ddlispush={8}&ddlflag={9}&ddllock={10}&txtContent={11}&txtAddress={12}&ddlsign={13}&txtMoney={14}&txtPerson1={15}&txtPerson2={16}&txtPerson3={17}&txtPerson4={18}&txtPerson5={19}&txtsDate={20}&txteDate={21}&txtsDate1={22}&txteDate1={23}&txtOrderID={24}&ddlarea={25}&ddlmoneyType={26}&ddlorderarea={27}&txtsDate2={28}&txteDate2={29}", "__id__", flag, _type, _cusName, _cid, _contractPrice, _status, _dstatus, _pushstatus, _flag, _lockstatus, _content, _address, _sign, _money, _person1, _person2, _person3, _person4, _person5, _sdate, _edate, _sdate1, _edate1, _orderid, _area, _moneyType, _orderarea, _sdate2, _edate2);
        }
        //批量删除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ChkAdminLevel("sys_order_list", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.Order bll = new BLL.Order();
            string result = "";
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            manager = GetAdminInfo();
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                CheckBox cb = (CheckBox)rptList.Items[i].FindControl("chkId");
                if (cb.Checked)
                {
                    result = bll.DeleteOrder(((HiddenField)rptList.Items[i].FindControl("hidId")).Value, manager);
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

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            flag = DTRequest.GetFormString("flag");
            _type = DTRequest.GetFormString("type");
            _orderid = DTRequest.GetFormString("txtOrderID");
            _cusName = DTRequest.GetFormString("txtCusName");
            _cid = DTRequest.GetFormString("hCusId");
            _contractPrice = DTRequest.GetFormString("ddlContractPrice");
            _status = DTRequest.GetFormString("ddlstatus");
            _dstatus = DTRequest.GetFormString("ddldstatus");
            _pushstatus = DTRequest.GetFormString("ddlispush");
            _flag = DTRequest.GetFormString("ddlflag");
            _lockstatus = DTRequest.GetFormString("ddllock");
            _content = DTRequest.GetFormString("txtContent");
            _address = DTRequest.GetFormString("txtAddress");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _person1 = DTRequest.GetFormString("txtPerson1");
            _person2 = DTRequest.GetFormString("txtPerson2");
            _person3 = DTRequest.GetFormString("txtPerson3");
            _person4 = DTRequest.GetFormString("txtPerson4");
            _person5 = DTRequest.GetFormString("txtPerson5");
            _sdate = DTRequest.GetFormString("txtsDate");
            _edate = DTRequest.GetFormString("txteDate");
            _sdate1 = DTRequest.GetFormString("txtsDate1");
            _edate1 = DTRequest.GetFormString("txteDate1");
            _sdate2 = DTRequest.GetFormString("txtsDate2");
            _edate2 = DTRequest.GetFormString("txteDate2");
            _area = DTRequest.GetFormString("ddlarea");
            _orderarea = DTRequest.GetFormString("ddlorderarea");
            BLL.Order bll = new BLL.Order();
            DataTable dt = bll.GetList(this.pageSize, this.page, "1=1" + CombSqlTxt(), "o_addDate desc,o_id desc", out this.totalCount,false,"",false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=订单列表.xlsx"); //HttpUtility.UrlEncode(fileName));
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
            headRow.CreateCell(2).SetCellValue("活动地点");
            headRow.CreateCell(3).SetCellValue("客户");
            headRow.CreateCell(4).SetCellValue("合同造价");
            headRow.CreateCell(5).SetCellValue("活动日期");
            headRow.CreateCell(6).SetCellValue("归属地");
            headRow.CreateCell(7).SetCellValue("订单状态");
            headRow.CreateCell(8).SetCellValue("推送状态");
            headRow.CreateCell(9).SetCellValue("上级审批");
            headRow.CreateCell(10).SetCellValue("锁单状态");
            headRow.CreateCell(11).SetCellValue("业务员");
            headRow.CreateCell(12).SetCellValue("报账人员");
            headRow.CreateCell(13).SetCellValue("策划人员");
            headRow.CreateCell(14).SetCellValue("设计人员");
            headRow.CreateCell(15).SetCellValue("应收款");
            headRow.CreateCell(16).SetCellValue("未收款");
            headRow.CreateCell(17).SetCellValue("业绩利润");
            headRow.CreateCell(18).SetCellValue("确认时间");

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
            headRow.GetCell(18).CellStyle = titleCellStyle;

            sheet.SetColumnWidth(0, 15 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 20 * 256);
            sheet.SetColumnWidth(4, 20 * 256);
            sheet.SetColumnWidth(5, 30 * 256);
            sheet.SetColumnWidth(6, 20 * 256);
            sheet.SetColumnWidth(7, 20 * 256);
            sheet.SetColumnWidth(8, 20 * 256);
            sheet.SetColumnWidth(9, 20 * 256);
            sheet.SetColumnWidth(10, 15 * 256);
            sheet.SetColumnWidth(11, 15 * 256);
            sheet.SetColumnWidth(12, 15 * 256);
            sheet.SetColumnWidth(13, 15 * 256);
            sheet.SetColumnWidth(14, 15 * 256);
            sheet.SetColumnWidth(15, 15 * 256);
            sheet.SetColumnWidth(16, 15 * 256);
            sheet.SetColumnWidth(17, 15 * 256);
            sheet.SetColumnWidth(18, 25 * 256);

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
                    row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["o_contractPrice"]));
                    row.CreateCell(5).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["o_sdate"]).Value.ToString("yyyy-MM-dd") + "/" + ConvertHelper.toDate(dt.Rows[i]["o_sdate"]).Value.ToString("yyyy-MM-dd"));
                    row.CreateCell(6).SetCellValue(new BLL.department().getAreaText(Utils.ObjectToStr(dt.Rows[i]["o_place"])) );
                    row.CreateCell(7).SetCellValue(BusinessDict.fStatus()[Utils.ObjToByte(dt.Rows[i]["o_status"])]);
                    row.CreateCell(8).SetCellValue(BusinessDict.pushStatus()[Utils.StrToBool(Utils.ObjectToStr(dt.Rows[i]["o_isPush"]),false)]);
                    row.CreateCell(9).SetCellValue(BusinessDict.checkStatus()[Utils.ObjToByte(dt.Rows[i]["o_flag"])]);
                    row.CreateCell(10).SetCellValue(BusinessDict.lockStatus()[Utils.ObjToByte(dt.Rows[i]["o_lockStatus"])]);
                    row.CreateCell(11).SetCellValue(dt.Rows[i]["op_name"].ToString());
                    row.CreateCell(12).SetCellValue(dt.Rows[i]["person2"].ToString());
                    row.CreateCell(13).SetCellValue(dt.Rows[i]["person3"].ToString());
                    row.CreateCell(14).SetCellValue(dt.Rows[i]["person4"].ToString());
                    row.CreateCell(15).SetCellValue(dt.Rows[i]["finMoney"].ToString());
                    row.CreateCell(16).SetCellValue(dt.Rows[i]["unMoney"].ToString());
                    row.CreateCell(17).SetCellValue(dt.Rows[i]["profit"].ToString());
                    row.CreateCell(18).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["o_statusTime"])==""?"":Utils.ObjectToDateTime(dt.Rows[i]["o_statusTime"]).ToString("yyyy-MM-dd HH:mm:ss"));

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
                    row.GetCell(18).CellStyle = cellStyle;
                }
            }

            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }
    }
}