using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.finance
{
    public partial class ReconciliationBill : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小
        protected string trHtml = string.Empty;
        protected string oidStr = "", _cusName = "", _cid = "", _type = "", _sign = "", _money1 = "", _sign1 = "", _money2 = "", _nature = "", _sdate = "", _edate = "", _sdate1 = "", _edate1 = "", _name = "", _address = "", _person1 = "", _person2 = "", _person3 = "", _person4 = "", _person5 = "", _oid = "", _chk = "", _sdate2 = "", _edate2 = "", _sdate3 = "", _edate3 = "", _status = "", _lockstatus = "", _area = "", _self = "";
        decimal _p11 = 0, _p12 = 0, _p13 = 0, _p14 = 0;
        decimal _p21 = 0, _p22 = 0, _p23 = 0, _p24 = 0, _p25 = 0, _p26 = 0;
        protected Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.pageSize = GetPageSize(10); //每页数量
            oidStr = DTRequest.GetString("oidStr");
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
            _sdate3 = DTRequest.GetString("txtsDate3");
            _edate3 = DTRequest.GetString("txteDate3");
            manager = GetAdminInfo();
            if (!Page.IsPostBack)
            {                
                if (!string.IsNullOrEmpty(_cid) && _cid != "0")
                {
                    DataTable dt = new BLL.Customer().GetList(0, "c_id=" + _cid + "", "").Tables[0];
                    if (dt != null)
                    {
                        labCustomerName.Text = dt.Rows[0]["c_name"].ToString();
                        labCustomerPhone.Text = dt.Rows[0]["co_number"].ToString();
                    }
                    RptBind();
                }
                else
                {
                    JscriptMsg("请先选择应收付对象", "");
                    return;
                }
            }
        }
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
                sqlWhere += " and exists(select * from MS_finance_chk where fc_finid=fin_id and fc_num='" + _chk + "')";
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
                sqlWhere += " and datediff(day,fin_sdate,'" + _sdate2 + "')<=0";
            }
            if (!string.IsNullOrEmpty(_edate2))
            {
                dict.Add("edate2", _edate2);
                sqlWhere += "  and datediff(day,fin_sdate,'" + _edate2 + "')>=0";
            }
            if (!string.IsNullOrEmpty(_sdate3))
            {
                dict.Add("sdate3", _sdate3);
                sqlWhere += " and datediff(day,fin_edate,'" + _sdate3 + "')<=0";
            }
            if (!string.IsNullOrEmpty(_edate3))
            {
                dict.Add("edate3", _edate3);
                sqlWhere += " and datediff(day,fin_edate,'" + _edate3 + "')>=0";
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
                    if (!string.IsNullOrEmpty(oidStr) && oidStr.IndexOf(dr["o_id"].ToString()) == -1)
                    {
                        continue;
                    }
                    finDt = bll.GetList(0, "fin_oid='" + dr["o_id"] + "'" + _where, "").Tables[0];
                    _colspan = finDt.Rows.Count == 0 ? 1 : finDt.Rows.Count;
                    trHtml += "<tr style=\"text-align: center;\">";
                    trHtml += "<td rowspan=\"" + _colspan + "\"><a href=\"../order/order_edit.aspx?action=" + DTEnums.ActionEnum.Edit.ToString() + "&oID=" + dr["o_id"] + "\">" + dr["o_id"] + "</a></td>";
                    trHtml += "<td style=\"text-align: left;\" rowspan=\"" + _colspan + "\">" + dr["o_content"] + "</td>";
                    trHtml += "<td rowspan=\"" + _colspan + "\">" + ConvertHelper.toDate(finDt.Rows[0]["o_sdate"]).Value.ToString("yyyy-MM-dd") + "<br/>" + ConvertHelper.toDate(finDt.Rows[0]["o_edate"]).Value.ToString("yyyy-MM-dd") + "</td>";
                    if (finDt == null || finDt.Rows.Count == 0)
                    {
                        //trHtml += "<td></td>";
                        trHtml += "<td></td>";
                        trHtml += "<td></td>";
                        trHtml += "<td></td>";
                    }
                    else
                    {
                        //trHtml += "<td>" + ConvertHelper.toDate(finDt.Rows[0]["fin_sdate"]).Value.ToString("yyyy-MM-dd") + "<br/>" + ConvertHelper.toDate(finDt.Rows[0]["fin_edate"]).Value.ToString("yyyy-MM-dd") + "</td>";
                        trHtml += "<td style=\"text-align: left;\">" + finDt.Rows[0]["na_name"] + "<br/>" + finDt.Rows[0]["fin_detail"] + "</td>";
                        trHtml += "<td>" + finDt.Rows[0]["fin_illustration"] + "</td>";
                        trHtml += "<td style=\"word-wrap:break-word;word-break:break-all;\">" + finDt.Rows[0]["fin_expression"] + "<br/>=" + finDt.Rows[0]["fin_money"] + "</td>";
                    }
                    trHtml += "<td style=\"text-align: right;\" rowspan=\"" + _colspan + "\">" + (string.IsNullOrEmpty(_chk) ?  dr["fin_money"] : dr["fcMoney"]) + "</td>";
                    trHtml += "<td style=\"text-align: right;\" rowspan=\"" + _colspan + "\">" + (string.IsNullOrEmpty(_chk) ? dr["rpd_money"] : dr["chkMoney"]) + "</td>";
                    trHtml += "<td style=\"text-align: right;\" rowspan=\"" + _colspan + "\">" + (string.IsNullOrEmpty(_chk) ? dr["unReceiptPay"] : dr["unChkMoney"]) + "</td>";
                    trHtml += "<td rowspan=\"" + _colspan + "\">" + dr["co_name"] + "</td>";
                    trHtml += "</tr>";
                    if (finDt != null && finDt.Rows.Count > 1)
                    {
                        for (int i = 1; i < finDt.Rows.Count; i++)
                        {
                            trHtml += "<tr style=\"text-align: center;\">";
                            //trHtml += "<td>" + ConvertHelper.toDate(finDt.Rows[i]["fin_sdate"]).Value.ToString("yyyy-MM-dd") + "<br/>" + ConvertHelper.toDate(finDt.Rows[i]["fin_edate"]).Value.ToString("yyyy-MM-dd") + "</td>";
                            trHtml += "<td style=\"text-align: left;\">" + finDt.Rows[i]["na_name"] + "<br/>" + finDt.Rows[i]["fin_detail"] + "</td>";
                            trHtml += "<td>" + finDt.Rows[i]["fin_illustration"] + "</td>";
                            trHtml += "<td style=\"word-wrap:break-word;word-break:break-all;\">" + finDt.Rows[i]["fin_expression"] + "<br/>=" + finDt.Rows[i]["fin_money"] + "</td>";
                            trHtml += "</tr>";                            
                        }
                    }
                }
                trHtml += "<tr style=\"text-align: right;\"><td colspan=\"6\">合计：</td><td>" + _p24 + "</td><td>"+ _p25 + "</td><td>"+ _p26 + "</td><td></td></tr>";
                #endregion
            }
            else
            {
                trHtml = "<tr><td align=\"center\" colspan=\"10\">暂无记录</td></tr>";
            }
            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("ReconciliationBill.aspx", "page={0}&txtCusName={1}&hCusId={2}&ddltype={3}&ddlsign={4}&txtMoney1={5}&ddlnature={6}&txtsDate={7}&txteDate={8}&txtsDate1={9}&txteDate1={10}&txtName={11}&txtAddress={12}&ddlsign1={13}&txtMoney2={14}&txtPerson1={15}&txtPerson2={16}&txtPerson3={17}&txtPerson4={18}&txtPerson5={19}&txtOrderID={20}&txtChk={21}&ddlstatus={22}&ddllock={23}&ddlarea={24}&txtsDate2={25}&txteDate2={26}&txtsDate3={27}&txteDate3={28}&self={29}",
                "__id__", _cusName, _cid, _type, _sign, _money1, _nature, _sdate, _edate, _sdate1, _edate1, _name, _address, _sign1, _money2, _person1, _person2, _person3, _person4, _person5, _oid, _chk, _status, _lockstatus, _area, _sdate2, _edate2, _sdate3, _edate3, _self);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);
        }
        #endregion
        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("ReconciliationBill_page_size", "DTcmsPage"), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    return _pagesize;
                }
            }
            return _default_size;
        }
        #endregion

        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("ReconciliationBill_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("ReconciliationBill.aspx", "page={0}&txtCusName={1}&hCusId={2}&ddltype={3}&ddlsign={4}&txtMoney1={5}&ddlnature={6}&txtsDate={7}&txteDate={8}&txtsDate1={9}&txteDate1={10}&txtName={11}&txtAddress={12}&ddlsign1={13}&txtMoney2={14}&txtPerson1={15}&txtPerson2={16}&txtPerson3={17}&txtPerson4={18}&txtPerson5={19}&txtOrderID={20}&txtChk={21}&ddlstatus={22}&ddllock={23}&ddlarea={24}&txtsDate2={25}&txteDate2={26}&txtsDate3={27}&txteDate3={28}&self={29}",
                "__id__", _cusName, _cid, _type, _sign, _money1, _nature, _sdate, _edate, _sdate1, _edate1, _name, _address, _sign1, _money2, _person1, _person2, _person3, _person4, _person5, _oid, _chk, _status, _lockstatus, _area, _sdate2, _edate2, _sdate3, _edate3, _self));
        }
    }
}