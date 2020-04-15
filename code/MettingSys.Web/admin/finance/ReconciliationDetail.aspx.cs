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
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BorderStyle = NPOI.SS.UserModel.BorderStyle;

namespace MettingSys.Web.admin.finance
{
    public partial class ReconciliationDetail : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小
        protected string trHtml = string.Empty;
        protected string _cusName = "", _cid = "", _type = "", _sign = "", _money1 = "", _sign1 = "", _money2 = "", _nature = "", _sdate = "", _edate = "", _sdate1 = "", _edate1 = "", _name = "", _address = "", _person1 = "", _person2 = "", _person3 = "", _person4 = "", _person5 = "", _oid = "", _chk = "", _sdate2 = "", _edate2 = "", _status = "", _lockstatus = "", _area = "", _self = "", _check = "";

        decimal _p11 = 0, _p12 = 0, _p13 = 0, _p14 = 0;
        decimal _p21 = 0, _p22 = 0, _p23 = 0, _p24 = 0, _p25 = 0, _p26 = 0;
        private Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.pageSize = GetPageSize(10); //每页数量
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _type = DTRequest.GetString("ddltype");
            _sign = DTRequest.GetString("ddlsign");
            _money1 = DTRequest.GetString("txtMoney1");
            _nature = DTRequest.GetString("ddlnature");
            _sdate = DTRequest.GetString("txtsDate");
            _edate = DTRequest.GetString("txteDate");
            _sdate1 = DTRequest.GetString("txtsDate1");
            _edate1 = DTRequest.GetString("txteDate1");
            _name = DTRequest.GetString("txtName");
            _address = DTRequest.GetString("txtAddress");
            _sign1 = DTRequest.GetString("ddlsign1");
            _money2 = DTRequest.GetString("txtMoney2");
            _person1 = DTRequest.GetString("txtPerson1").ToUpper();
            _person2 = DTRequest.GetString("txtPerson2").ToUpper();
            _person3 = DTRequest.GetString("txtPerson3").ToUpper();
            _person4 = DTRequest.GetString("txtPerson4").ToUpper();
            _person5 = DTRequest.GetString("txtPerson5").ToUpper();
            _oid = DTRequest.GetString("txtOrderID");
            _chk = DTRequest.GetString("txtChk");
            _status = DTRequest.GetString("ddlstatus");
            _lockstatus = DTRequest.GetString("ddllock");
            _area = DTRequest.GetString("ddlarea");
            _sdate2 = DTRequest.GetString("txtsDate2");
            _edate2 = DTRequest.GetString("txteDate2");
            _check = DTRequest.GetString("ddlcheck");
            _self = DTRequest.GetString("self");
            if (_self == "1")
            {
                manager = GetAdminInfo();
                _person1 = manager.user_name;
                txtPerson1.Enabled = false;
            }

            if (!Page.IsPostBack)
            {
                InitData();
                if (_self != "1")
                {
                    ChkAdminLevel("sys_settlementCustomer", DTEnums.ActionEnum.View.ToString()); //检查权限
                }
                if (!string.IsNullOrEmpty(_cid) && _cid != "0")
                {
                    RptBind();
                }
                else
                {
                    JscriptMsg("请先选择应收付对象", "");
                    return;
                }
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

            ddltype.DataSource = Common.BusinessDict.financeType();
            ddltype.DataTextField = "value";
            ddltype.DataValueField = "key";
            ddltype.DataBind();

            ddlstatus.DataSource = Common.BusinessDict.fStatus(1);
            ddlstatus.DataTextField = "value";
            ddlstatus.DataValueField = "key";
            ddlstatus.DataBind();
            ddlstatus.Items.Insert(0, new ListItem("不限", ""));

            ddlnature.DataSource = new BLL.businessNature().GetList(0, "", "na_sort asc,na_id desc");
            ddlnature.DataTextField = "na_name";
            ddlnature.DataValueField = "na_id";
            ddlnature.DataBind();
            ddlnature.Items.Insert(0, new ListItem("不限", ""));

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
        }
        #endregion

        private Dictionary<string, string> getDict(out string sqlWhere)
        {
            sqlWhere = "";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(_type))
            {
                dict.Add("type", _type);
                sqlWhere += " and fin_type='" + _type + "'";
            }
            if (!string.IsNullOrEmpty(_cid))
            {
                dict.Add("cid", _cid);
                sqlWhere += " and fin_cid=" + _cid + "";
            }
            dict.Add("sign", _sign);
            if (!string.IsNullOrEmpty(_money1))
            {
                dict.Add("money1", _money1);
            }
            if (!string.IsNullOrEmpty(_nature))
            {
                dict.Add("nature", _nature);
                sqlWhere += " and fin_nature=" + _nature + "";
            }
            if (!string.IsNullOrEmpty(_sdate))//订单开始日期
            {
                dict.Add("sdate", _sdate);
            }
            if (!string.IsNullOrEmpty(_edate))//订单开始日期
            {
                dict.Add("edate", _edate);
            }
            if (!string.IsNullOrEmpty(_sdate1))//订单结束日期
            {
                dict.Add("sdate1", _sdate1);
            }
            if (!string.IsNullOrEmpty(_edate1))//订单结束日期
            {
                dict.Add("edate1", _edate1);
            }
            if (!string.IsNullOrEmpty(_name))//活动名称
            {
                dict.Add("name", _name);
            }
            if (!string.IsNullOrEmpty(_address))//活动地址
            {
                dict.Add("address", _address);
            }
            if (!string.IsNullOrEmpty(_person1))//业务员
            {
                dict.Add("person1", _person1);
            }
            if (!string.IsNullOrEmpty(_person2))//报账人员
            {
                dict.Add("person2", _person2);
            }
            if (!string.IsNullOrEmpty(_person3))//策划人员
            {
                dict.Add("person3", _person3);
            }
            if (!string.IsNullOrEmpty(_person4))//执行人员
            {
                dict.Add("person4", _person4);
            }
            if (!string.IsNullOrEmpty(_person5))//设计人员
            {
                dict.Add("person5", _person5);
            }
            if (!string.IsNullOrEmpty(_oid))
            {
                dict.Add("Oid", _oid);
            }
            if (!string.IsNullOrEmpty(_chk))
            {
                dict.Add("chk", _chk);
                if (_chk == "空")
                {
                    sqlWhere += " and not exists(select * from MS_finance_chk where fc_finid=fin_id and isnull(fc_num,'')<>'')";
                }
                else
                {
                    sqlWhere += " and exists(select * from MS_finance_chk where fc_finid=fin_id and fc_num='" + _chk + "')";
                }
            }
            if (!string.IsNullOrEmpty(_status))
            {
                dict.Add("status", _status);
            }
            if (!string.IsNullOrEmpty(_lockstatus))
            {
                dict.Add("lockstatus", _lockstatus);
            }
            if (!string.IsNullOrEmpty(_area))
            {
                dict.Add("area", _area);
            }
            if (!string.IsNullOrEmpty(_sdate2))
            {
                dict.Add("sdate2", _sdate2);
            }
            if (!string.IsNullOrEmpty(_edate2))
            {
                dict.Add("edate2", _edate2);
            }
            if (!string.IsNullOrEmpty(_check))
            {
                dict.Add("check", _check);
                sqlWhere += " and fin_flag=" + _check + "";
            }
            dict.Add("sign1", _sign1);
            if (!string.IsNullOrEmpty(_money2))
            {
                dict.Add("money2", _money2);
            }
            return dict;
        }

        #region 数据绑定=================================
        private void RptBind()
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.finance bll = new BLL.finance();
            string _where = "";
            Dictionary<string, string> dict = getDict(out _where);
            DataTable dt = bll.getReconciliationDetail(dict, this.pageSize, this.page, "o_id asc", out this.totalCount, out _p24, out _p25, out _p26, out _p13, out _p14).Tables[0];
            int _colspan = 1;
            DataTable finDt = null;
            if (dt.Rows.Count > 0)
            {
                #region 拼接列表Html
                foreach (DataRow dr in dt.Rows)
                {
                    finDt = bll.GetList(0, "fin_oid='" + dr["o_id"] + "'" + _where, _chk, "").Tables[0];
                    _colspan = finDt.Rows.Count == 0 ? 1 : finDt.Rows.Count;
                    trHtml += "<tr style=\"text-align: center;\">";
                    trHtml += "<td rowspan=\"" + _colspan + "\"><input type=\"checkbox\" class=\"checkall\" data-id=\"" + dr["o_id"] + "\" data-finmoney=\"" + dr["fin_money"] + "\" data-rpdmoney=\"" + dr["rpd_money"] + "\" data-unMoney=\"" + dr["unReceiptPay"] + "\" data-unchkmoney=\"" + (string.IsNullOrEmpty(_chk) ? "--" : "" + dr["unChkMoney"] + "") + "\"  data-unrpmoney=\"" + (string.IsNullOrEmpty(_chk) ? "" + dr["unReceiptPay"] + "" : "" + dr["unChkMoney"] + "") + "\"/></td>";
                    trHtml += "<td rowspan=\"" + _colspan + "\"><a href=\"../order/order_edit.aspx?action=" + DTEnums.ActionEnum.Edit.ToString() + "&oID=" + dr["o_id"] + "\">" + dr["o_id"] + "</a></td>";
                    trHtml += "<td style=\"text-align: left;\" rowspan=\"" + _colspan + "\">" + dr["c_name"] + "</td>";
                    trHtml += "<td rowspan=\"" + _colspan + "\">" + ConvertHelper.toDate(dr["o_sdate"]).Value.ToString("yyyy-MM-dd") + "<br/>" + ConvertHelper.toDate(dr["o_edate"]).Value.ToString("yyyy-MM-dd") + "</td>";
                    trHtml += "<td style=\"text-align: left;\" rowspan=\"" + _colspan + "\">" + dr["o_address"] + "/" + dr["o_content"] + "</td>";
                    if (finDt == null || finDt.Rows.Count == 0)
                    {

                        trHtml += "<td></td>";
                        trHtml += "<td></td>";
                        trHtml += "<td></td>";
                        //trHtml += "<td></td>";
                        trHtml += "<td></td>";
                        trHtml += "<td></td>";
                        trHtml += "<td></td>";
                        trHtml += "<td></td>";
                    }
                    else
                    {
                        _p11 += Utils.ObjToDecimal(finDt.Rows[0]["chkMoney"], 0);
                        trHtml += "<td><input type=\"checkbox\" class=\"check\" data-oid=\"" + dr["o_id"] + "\" data-id=\"" + finDt.Rows[0]["fin_id"] + "\" data-money=\"" + finDt.Rows[0]["fin_money"] + "\" data-chkmoney=\"" + finDt.Rows[0]["chkMoney"] + "\" /></td>";
                        trHtml += "<td style=\"word-wrap:break-word;word-break:break-all;\"><a href=\"javascript:;\" onclick=\"addFinChk(" + finDt.Rows[0]["fin_id"] + ",'" + finDt.Rows[0]["fin_oid"] + "')\">" + finDt.Rows[0]["chk"] + "</a></td>";
                        trHtml += "<td style=\"text-align: right;\">" + finDt.Rows[0]["chkMoney"] + "</td>";
                        //trHtml += "<td>" + ConvertHelper.toDate(finDt.Rows[0]["fin_sdate"]).Value.ToString("yyyy-MM-dd") + "<br/>" + ConvertHelper.toDate(finDt.Rows[0]["fin_edate"]).Value.ToString("yyyy-MM-dd") + "</td>";
                        trHtml += "<td style=\"text-align: center;\">" + finDt.Rows[0]["na_name"] + "<br/>" + finDt.Rows[0]["fin_detail"] + "</td>";
                        trHtml += "<td title=\"" + finDt.Rows[0]["fin_illustration"] + "\">" + (finDt.Rows[0]["fin_illustration"].ToString().Length <= 30 ? finDt.Rows[0]["fin_illustration"].ToString() : finDt.Rows[0]["fin_illustration"].ToString().Substring(0, 30)) + "</td>";
                        trHtml += "<td style=\"word-wrap:break-word;word-break:break-all;\">" + finDt.Rows[0]["fin_expression"] + "<br/>=" + finDt.Rows[0]["fin_money"] + "</td>";
                        trHtml += "<td><span onmouseover=\"tip_index = layer.tips('审批人：" + finDt.Rows[0]["fin_checkNum"] + "-" + finDt.Rows[0]["fin_checkName"] + "<br/>审批备注：" + finDt.Rows[0]["fin_checkRemark"] + "', this, { time: 0 }); \" onmouseout=\"layer.close(tip_index); \" class=\"check_" + finDt.Rows[0]["fin_flag"] + "\"></span></td>";
                    }
                    trHtml += "<td style=\"text-align: right;\" rowspan=\"" + _colspan + "\">" + ((string.IsNullOrEmpty(_chk) || _chk == "空" )? "" : "<font color='green'>" + dr["fcMoney"] + "/</font>") + "" + dr["fin_money"] + "</td>";
                    trHtml += "<td style=\"text-align: right;\" rowspan=\"" + _colspan + "\">" + ((string.IsNullOrEmpty(_chk) || _chk == "空" )? "" : "<font color='green'>" + dr["chkMoney"] + "/</font>") + "" + dr["rpd_money"] + "</td>";
                    trHtml += "<td style=\"text-align: right;\" rowspan=\"" + _colspan + "\">" + ((string.IsNullOrEmpty(_chk) || _chk == "空" )? "" : "<font color='green'>" + dr["unChkMoney"] + "/</font>") + "" + dr["unReceiptPay"] + "</td>";
                    trHtml += "</tr>";

                    if (!string.IsNullOrEmpty(_chk))
                    {
                        _p12 += Utils.ObjToDecimal(dr["unChkMoney"], 0);
                    }
                    _p21 += Utils.ObjToDecimal(dr["fin_money"], 0);
                    _p22 += Utils.ObjToDecimal(dr["rpd_money"], 0);
                    _p23 += Utils.ObjToDecimal(dr["unReceiptPay"], 0);
                    if (finDt != null && finDt.Rows.Count > 1)
                    {
                        for (int i = 1; i < finDt.Rows.Count; i++)
                        {
                            trHtml += "<tr style=\"text-align: center;\">";
                            trHtml += "<td><input type=\"checkbox\" class=\"check\" data-oid=\"" + dr["o_id"] + "\"  data-id=\"" + finDt.Rows[i]["fin_id"] + "\" data-money=\"" + finDt.Rows[i]["fin_money"] + "\" data-chkmoney=\"" + finDt.Rows[i]["chkMoney"] + "\" /></td>";
                            trHtml += "<td style=\"word-wrap:break-word;word-break:break-all;\"><a href=\"javascript:;\" onclick=\"addFinChk(" + finDt.Rows[i]["fin_id"] + ",'" + finDt.Rows[i]["fin_oid"] + "')\">" + finDt.Rows[i]["chk"] + "</a></td>";
                            trHtml += "<td style=\"text-align: right;\">" + finDt.Rows[i]["chkMoney"] + "</td>";
                            //trHtml += "<td>" + ConvertHelper.toDate(finDt.Rows[i]["fin_sdate"]).Value.ToString("yyyy-MM-dd") + "<br/>" + ConvertHelper.toDate(finDt.Rows[i]["fin_edate"]).Value.ToString("yyyy-MM-dd") + "</td>";
                            trHtml += "<td style=\"text-align: center;\">" + finDt.Rows[i]["na_name"] + "<br/>" + finDt.Rows[i]["fin_detail"] + "</td>";
                            trHtml += "<td title=\"" + finDt.Rows[i]["fin_illustration"] + "\">" + (finDt.Rows[i]["fin_illustration"].ToString().Length <= 30 ? finDt.Rows[i]["fin_illustration"].ToString() : finDt.Rows[i]["fin_illustration"].ToString().Substring(0, 30)) + "</td>";
                            trHtml += "<td style=\"word-wrap:break-word;word-break:break-all;\">" + finDt.Rows[i]["fin_expression"] + "<br/>=" + finDt.Rows[i]["fin_money"] + "</td>";
                            trHtml += "<td><span onmouseover=\"tip_index = layer.tips('审批人：" + finDt.Rows[i]["fin_checkNum"] + "-" + finDt.Rows[i]["fin_checkName"] + "<br/>审批备注：" + finDt.Rows[i]["fin_checkRemark"] + "', this, { time: 0 }); \" onmouseout=\"layer.close(tip_index); \" class=\"check_" + finDt.Rows[i]["fin_flag"] + "\"></span></td>";
                            trHtml += "</tr>";
                            _p11 += Utils.ObjToDecimal(finDt.Rows[i]["chkMoney"], 0);
                        }
                    }
                }
                #endregion
            }
            else
            {
                trHtml = "<tr><td align=\"center\" colspan=\"14\">暂无记录</td></tr>";
            }
            p11.Text = _p11.ToString();
            if (!string.IsNullOrEmpty(_chk))
            {
                p12.Text = _p12.ToString();
                p14.Text = _p14.ToString();
            }
            else
            {
                p12.Text = "--";
                p14.Text = "--";
            }
            p13.Text = _chk == "空"?"0":_p13.ToString();
            p21.Text = _p21.ToString();
            p22.Text = _p22.ToString();
            p23.Text = _p23.ToString();
            p24.Text = _p24.ToString();
            p25.Text = _p25.ToString();
            p26.Text = _p26.ToString();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("ReconciliationDetail.aspx", "page={0}&txtCusName={1}&hCusId={2}&ddltype={3}&ddlsign={4}&txtMoney1={5}&ddlnature={6}&txtsDate={7}&txteDate={8}&txtsDate1={9}&txteDate1={10}&txtName={11}&txtAddress={12}&ddlsign1={13}&txtMoney2={14}&txtPerson1={15}&txtPerson2={16}&txtPerson3={17}&txtPerson4={18}&txtPerson5={19}&txtOrderID={20}&txtChk={21}&ddlstatus={22}&ddllock={23}&ddlarea={24}&txtsDate2={25}&txteDate2={26}&self={27}",
                "__id__", _cusName, _cid, _type, _sign, _money1, _nature, _sdate, _edate, _sdate1, _edate1, _name, _address, _sign1, _money2, _person1, _person2, _person3, _person4, _person5, _oid, _chk, _status, _lockstatus, _area, _sdate2, _edate2, _self);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);



            ddltype.SelectedValue = _type;
            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            txtsDate.Text = _sdate;
            txteDate.Text = _edate;
            txtsDate1.Text = _sdate1;
            txteDate1.Text = _edate1;
            //txtsDate2.Text = _sdate2;
            //txteDate2.Text = _edate2;
            //txtsDate3.Text = _sdate3;
            //txteDate3.Text = _edate3;
            ddlstatus.SelectedValue = _status;
            ddlsign.SelectedValue = _sign;
            txtMoney1.Text = _money1;
            ddlsign1.SelectedValue = _sign1;
            txtMoney2.Text = _money2;
            ddlnature.SelectedValue = _nature;
            txtChk.Text = _chk;
            ddlarea.SelectedValue = _area;
            ddllock.SelectedValue = _lockstatus;
            txtOrderID.Text = _oid;
            txtName.Text = _name;
            txtAddress.Text = _address;
            txtPerson1.Text = _person1;
            txtPerson2.Text = _person2;
            txtPerson3.Text = _person3;
            txtPerson4.Text = _person4;
            txtPerson5.Text = _person5;
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("ReconciliationDetail_page_size", "DTcmsPage"), out _pagesize))
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
            _type = DTRequest.GetFormString("ddltype");
            _sign = DTRequest.GetFormString("ddlsign");
            _money1 = DTRequest.GetFormString("txtMoney1");
            _nature = DTRequest.GetFormString("ddlnature");
            _sdate = DTRequest.GetFormString("txtsDate");
            _edate = DTRequest.GetFormString("txteDate");
            _sdate1 = DTRequest.GetFormString("txtsDate1");
            _edate1 = DTRequest.GetFormString("txteDate1");
            _name = DTRequest.GetFormString("txtName");
            _address = DTRequest.GetFormString("txtAddress");
            _sign1 = DTRequest.GetFormString("ddlsign1");
            _money2 = DTRequest.GetFormString("txtMoney2");
            _person1 = DTRequest.GetFormString("txtPerson1");
            _person2 = DTRequest.GetFormString("txtPerson2");
            _person3 = DTRequest.GetFormString("txtPerson3");
            _person4 = DTRequest.GetFormString("txtPerson4");
            _person5 = DTRequest.GetFormString("txtPerson5");
            _oid = DTRequest.GetFormString("txtOrderID");
            _chk = DTRequest.GetFormString("txtChk");
            _status = DTRequest.GetFormString("ddlstatus");
            _lockstatus = DTRequest.GetFormString("ddllock");
            _area = DTRequest.GetFormString("ddlarea");
            _sdate2 = DTRequest.GetFormString("txtsDate2");
            _edate2 = DTRequest.GetFormString("txteDate2");
            _check = DTRequest.GetFormString("ddlcheck");
            _self = DTRequest.GetFormString("self");
            if (_self == "1")
            {
                manager = GetAdminInfo();
                _person1 = manager.user_name;
                txtPerson1.Enabled = false;
            }
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                RptBind();
            }
            else
            {
                JscriptMsg("请先选择应收付对象", "");
                return;
            }
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("ReconciliationDetail_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("ReconciliationDetail.aspx", "page={0}&txtCusName={1}&hCusId={2}&ddltype={3}&ddlsign={4}&txtMoney1={5}&ddlnature={6}&txtsDate={7}&txteDate={8}&txtsDate1={9}&txteDate1={10}&txtName={11}&txtAddress={12}&ddlsign1={13}&txtMoney2={14}&txtPerson1={15}&txtPerson2={16}&txtPerson3={17}&txtPerson4={18}&txtPerson5={19}&txtOrderID={20}&txtChk={21}&ddlstatus={22}&ddllock={23}&ddlarea={24}&txtsDate2={25}&txteDate2={26}&self={27}",
                "__id__", _cusName, _cid, _type, _sign, _money1, _nature, _sdate, _edate, _sdate1, _edate1, _name, _address, _sign1, _money2, _person1, _person2, _person3, _person4, _person5, _oid, _chk, _status, _lockstatus, _area, _sdate2, _edate2, _self));
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetFormString("txtCusName");
            _cid = DTRequest.GetFormString("hCusId");
            _type = DTRequest.GetFormString("ddltype");
            _sign = DTRequest.GetFormString("ddlsign");
            _money1 = DTRequest.GetFormString("txtMoney1");
            _nature = DTRequest.GetFormString("ddlnature");
            _sdate = DTRequest.GetFormString("txtsDate");
            _edate = DTRequest.GetFormString("txteDate");
            _sdate1 = DTRequest.GetFormString("txtsDate1");
            _edate1 = DTRequest.GetFormString("txteDate1");
            _name = DTRequest.GetFormString("txtName");
            _address = DTRequest.GetFormString("txtAddress");
            _sign1 = DTRequest.GetFormString("ddlsign1");
            _money2 = DTRequest.GetFormString("txtMoney2");
            _person1 = DTRequest.GetFormString("txtPerson1");
            _person2 = DTRequest.GetFormString("txtPerson2");
            _person3 = DTRequest.GetFormString("txtPerson3");
            _person4 = DTRequest.GetFormString("txtPerson4");
            _person5 = DTRequest.GetFormString("txtPerson5");
            _oid = DTRequest.GetFormString("txtOrderID");
            _chk = DTRequest.GetFormString("txtChk");
            _status = DTRequest.GetFormString("ddlstatus");
            _lockstatus = DTRequest.GetFormString("ddllock");
            _area = DTRequest.GetFormString("ddlarea");
            _sdate2 = DTRequest.GetFormString("txtsDate2");
            _edate2 = DTRequest.GetFormString("txteDate2");
            _check = DTRequest.GetFormString("ddlcheck");
            _self = DTRequest.GetFormString("self");
            string _where = "";
            Dictionary<string, string> dict = getDict(out _where);
            DataTable dt = new BLL.finance().getReconciliationDetail(dict, this.pageSize, this.page, "o_id asc", out this.totalCount, out _p24, out _p25, out _p26, out _p13, out _p14, false).Tables[0];

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=客户对账明细.xlsx"); //HttpUtility.UrlEncode(fileName));
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet("对账明细");
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
            headRow.CreateCell(1).SetCellValue("客源");
            headRow.CreateCell(2).SetCellValue("活动日期");
            headRow.CreateCell(3).SetCellValue("活动地点/活动名称");
            headRow.CreateCell(4).SetCellValue("对账标识");
            headRow.CreateCell(5).SetCellValue("对账金额");
            headRow.CreateCell(6).SetCellValue("业务性质/明细");
            headRow.CreateCell(7).SetCellValue("业务说明");
            headRow.CreateCell(8).SetCellValue("表达式");
            headRow.CreateCell(9).SetCellValue("审核");
            headRow.CreateCell(10).SetCellValue((string.IsNullOrEmpty(_chk) || _chk == "空" ? "" : "对账金额/") + (_type == "True" ? "应收" : "应付"));
            headRow.CreateCell(11).SetCellValue((string.IsNullOrEmpty(_chk) || _chk == "空" ? "" : "对账金额/") + (_type == "True" ? "已收" : "已付"));
            headRow.CreateCell(12).SetCellValue((string.IsNullOrEmpty(_chk) || _chk == "空" ? "" : "对账金额/") + (_type == "True" ? "未收" : "未付"));

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
                int rowIndex = 1;
                DataTable finDt = null;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataSet ds = new BLL.finance().GetList(0, "fin_oid='" + dt.Rows[i]["o_id"] + "'" + _where, _chk, "");
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        finDt = ds.Tables[0];
                        if (finDt != null && finDt.Rows.Count > 0)
                        {
                            for (int j = 0; j < finDt.Rows.Count; j++)
                            {
                                IRow row = sheet.CreateRow(rowIndex);
                                row.HeightInPoints = 22;
                                row.CreateCell(0).SetCellValue(dt.Rows[i]["o_id"].ToString());
                                row.CreateCell(1).SetCellValue(dt.Rows[i]["c_name"].ToString());
                                row.CreateCell(2).SetCellValue(ConvertHelper.toDate(dt.Rows[i]["o_sdate"]).Value.ToString("yyyy-MM-dd") + "/" + ConvertHelper.toDate(dt.Rows[i]["o_edate"]).Value.ToString("yyyy-MM-dd"));
                                row.CreateCell(3).SetCellValue(dt.Rows[i]["o_address"].ToString() + "/" + dt.Rows[i]["o_content"].ToString());
                                row.CreateCell(4).SetCellValue(finDt.Rows[j]["chk"].ToString());
                                row.CreateCell(5).SetCellValue(finDt.Rows[j]["chkMoney"].ToString());
                                row.CreateCell(6).SetCellValue(finDt.Rows[j]["na_name"] + "/" + finDt.Rows[j]["fin_detail"]);
                                row.CreateCell(7).SetCellValue(finDt.Rows[j]["fin_illustration"].ToString());
                                row.CreateCell(8).SetCellValue(finDt.Rows[j]["fin_expression"] + "=" + finDt.Rows[j]["fin_money"]);
                                row.CreateCell(9).SetCellValue(BusinessDict.checkStatus()[Convert.ToByte(finDt.Rows[j]["fin_flag"].ToString())]);
                                row.CreateCell(10).SetCellValue((string.IsNullOrEmpty(_chk) || _chk == "空" ? "" : "" + dt.Rows[i]["fcMoney"] + "/") + dt.Rows[i]["fin_money"]);
                                row.CreateCell(11).SetCellValue((string.IsNullOrEmpty(_chk) || _chk == "空" ? "" : "" + dt.Rows[i]["chkMoney"] + "/") + dt.Rows[i]["rpd_money"]);
                                row.CreateCell(12).SetCellValue((string.IsNullOrEmpty(_chk) || _chk == "空" ? "" : "" + dt.Rows[i]["unChkMoney"] + "/") + dt.Rows[i]["unReceiptPay"]);

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
                                rowIndex++;
                            }
                        }
                    }
                }
            }

            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();


        }
    }
}