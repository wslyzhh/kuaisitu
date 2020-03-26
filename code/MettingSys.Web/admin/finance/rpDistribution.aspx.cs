using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.finance
{
    public partial class rpDistribution : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小
        protected Model.manager manager = null;
        protected int rpID = 0, _tag = 0;
        protected string _orderId = "", _sign = "", _money = "", _chk = "", _sdate = "", _edate = "", _sdate1 = "", _edate1 = "", _person = "",_moneyType="";

        
        protected Model.ReceiptPay model = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.pageSize = GetPageSize(10); //每页数量
            _tag = DTRequest.GetInt("tag",0);
            rpID = DTRequest.GetInt("id", 0);
            if (this.rpID == 0)
            {
                JscriptMsg("传输参数不正确！", "back");
                return;
            }
            model = new BLL.ReceiptPay().GetModel(rpID);
            if (model == null)
            {
                JscriptMsg("记录不存在！", "back");
                return;
            }
            _orderId = DTRequest.GetString("txtOrderId");
            _moneyType = DTRequest.GetString("ddlMoneyType");
            _sign = DTRequest.GetString("ddlsign");
            _money = DTRequest.GetString("txtMoney");
            _chk = DTRequest.GetString("txtChk");
            _sdate = DTRequest.GetString("txtsDate");
            _edate = DTRequest.GetString("txteDate");
            _sdate1 = DTRequest.GetString("txtsDate1");
            _edate1 = DTRequest.GetString("txteDate1");
            _person = DTRequest.GetString("txtPerson");
            if (!IsPostBack)
            {
                ddlMoneyType.Items.Insert(0, new ListItem(model.rp_type.Value ? "应收金额" : "应付金额", "0"));
                ddlMoneyType.Items.Insert(1, new ListItem(model.rp_type.Value ? "已收金额" : "已付金额", "1"));
                ddlMoneyType.Items.Insert(2, new ListItem(model.rp_type.Value ? "未收金额" : "未付金额", "2"));
                if (_tag == 0)
                {
                    _moneyType = "2";
                    _money = "0";
                    _sign = "<>";
                }
                RptBind();
            }
            txtOrderId.Text = _orderId;
            ddlMoneyType.SelectedValue = _moneyType;
            ddlsign.SelectedValue = _sign;
            txtMoney.Text = _money;
            txtChk.Text = _chk;
            txtsDate.Text = _sdate;
            txteDate.Text = _edate;
            txtsDate1.Text = _sdate1;
            txteDate1.Text = _edate1;
            txtPerson.Text = _person;
        }
        #region 数据绑定=================================
        private void RptBind()
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            Dictionary<string, string> dict = getDict();
            this.rptList.DataSource = bll.GetDistributionList(dict, pageSize, page, out totalCount);
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("rpDistribution.aspx", "page={0}&id={1}&tag={2}&txtOrderId={3}&ddlMoneyType={4}&ddlsign={5}&txtMoney={6}&txtChk={7}&txtsDate={8}&txteDate={9}&txtsDate1={10}&txteDate1={11}&txtPerson={12}", "__id__", rpID.ToString(), _tag.ToString(), _orderId, _moneyType, _sign, _money, _chk, _sdate, _edate, _sdate1, _edate1, _person);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            decimal _hasDistribute = bll.getDistributeMoney(rpID);
            labHasDistribute.Text = _hasDistribute.ToString();
            labLeftMoney.Text = (model.rp_money - _hasDistribute).ToString();
        }
        #endregion

        #region 添加搜索条件==========================
        protected Dictionary<string,string> getDict()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("tag", _tag.ToString());
            dict.Add("cid", model.rp_cid.ToString());
            dict.Add("type",model.rp_type.ToString());
            dict.Add("rpID", rpID.ToString());
            dict.Add("moneyType", _moneyType);
            if (!string.IsNullOrEmpty(_orderId))
            {
                dict.Add("oID",_orderId);
            }
            dict.Add("sign", _sign);
            if (!string.IsNullOrEmpty(_money))
            {
                dict.Add("money", _money);
            }
            if (!string.IsNullOrEmpty(_chk))
            {
                dict.Add("chk", _chk);
            }
            if (!string.IsNullOrEmpty(_sdate))
            {
                dict.Add("sdate", _sdate);
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                dict.Add("edate", _edate);
            }
            if (!string.IsNullOrEmpty(_sdate1))
            {
                dict.Add("sdate1", _sdate1);
            }
            if (!string.IsNullOrEmpty(_edate1))
            {
                dict.Add("edate1", _edate1);
            }
            if (!string.IsNullOrEmpty(_person))
            {
                dict.Add("person", _person);
            }
            return dict;
        }
        #endregion
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            _orderId = DTRequest.GetFormString("txtOrderId");
            _moneyType = DTRequest.GetFormString("ddlMoneyType");
            _sign = DTRequest.GetFormString("ddlsign");
            _money = DTRequest.GetFormString("txtMoney");
            _chk = DTRequest.GetFormString("txtChk");
            _sdate = DTRequest.GetFormString("txtsDate");
            _edate = DTRequest.GetFormString("txteDate");
            _sdate1 = DTRequest.GetFormString("txtsDate1");
            _edate1 = DTRequest.GetFormString("txteDate1");
            _person = DTRequest.GetFormString("txtPerson");
            RptBind();
            txtOrderId.Text = _orderId;
            ddlMoneyType.SelectedValue = _moneyType;
            ddlsign.SelectedValue = _sign;
            txtMoney.Text = _money;
            txtChk.Text = _chk;
            txtsDate.Text = _sdate;
            txteDate.Text = _edate;
            txtsDate1.Text = _sdate1;
            txteDate1.Text = _edate1;
            txtPerson.Text = _person;
        }

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("rpDistribution_page_size", "DTcmsPage"), out _pagesize))
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
                    Utils.WriteCookie("rpDistribution_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("rpDistribution.aspx", "page={0}&id={1}&tag={2}&txtOrderId={3}&ddlMoneyType={4}&ddlsign={5}&txtMoney={6}&txtChk={7}&txtsDate={8}&txteDate={9}&txtsDate1={10}&txteDate1={11}&txtPerson={12}", "__id__", rpID.ToString(), _tag.ToString(), _orderId, _moneyType, _sign, _money, _chk, _sdate, _edate, _sdate1, _edate1, _person));
        }
    }
}