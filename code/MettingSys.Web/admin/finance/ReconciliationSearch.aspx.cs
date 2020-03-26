using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.finance
{
    public partial class ReconciliationSearch : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小
        protected string _orderid = "", _cusName = "", _cid = "", _type = "", _num = "";

        decimal _pMoney = 0, _tMoney = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            _orderid = DTRequest.GetString("txtOrderID");
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _type = DTRequest.GetString("ddltype");
            _num = DTRequest.GetString("txtNum");
            
            this.pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                InitData();
                ChkAdminLevel("sys_ReconciliationSearch", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("1=1" + CombSqlTxt(), "fc_addDate desc,fin_adddate desc");
            }
        }

        #region 初始化
        private void InitData()
        {
            ddltype.DataSource = Common.BusinessDict.financeType();
            ddltype.DataTextField = "value";
            ddltype.DataValueField = "key";
            ddltype.DataBind();
            ddltype.Items.Insert(0, new ListItem("不限", ""));
        }
        #endregion

        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.finance_chk bll = new BLL.finance_chk();
            DataTable dt= bll.GetList(this.pageSize, this.page, _strWhere, _orderby, out this.totalCount, out _tMoney).Tables[0];
            this.rptList.DataSource = dt;
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("ReconciliationSearch.aspx", "page={0}&txtOrderID={1}&txtCusName={2}&hCusId={3}&ddltype={4}&txtNum={5}", "__id__", _orderid, _cusName, _cid, _type, _num);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pMoney += Utils.ObjToDecimal(dr["fc_money"], 0);
                }
            }

            pCount.Text = dt.Rows.Count.ToString();
            pMoney.Text = _pMoney.ToString();

            tCount.Text = totalCount.ToString();
            tMoney.Text = _tMoney.ToString();

            txtOrderID.Text = _orderid;
            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            ddltype.SelectedValue = _type;
            txtNum.Text = _num;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                strTemp.Append(" and fin_cid=" + _cid + "");
            }
            if (!string.IsNullOrEmpty(_orderid))
            {
                strTemp.Append(" and o_id='" + _orderid + "'");
            }
            if (!string.IsNullOrEmpty(_type))
            {
                strTemp.Append(" and fin_type='" + _type + "'");
            }
            if (!string.IsNullOrEmpty(_num))
            {
                strTemp.Append(" and fc_num='" + _num + "'");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("ReconciliationSearch_page_size", "DTcmsPage"), out _pagesize))
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
            _type = DTRequest.GetFormString("ddltype");
            _num = DTRequest.GetFormString("txtNum");
            RptBind("1=1" + CombSqlTxt(), "o_addDate desc,o_id desc");
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            var fileName = "对账查询";
            string[] strFieldsName = { "订单号", "应收付对象","客户", "活动日期", "活动地点", "活动名称",  "收付性质", "业务性质", "业务明细", "对账标识", "对账金额" };
            string[] strFields = { "o_id","c_name","cname", "o_sdate/o_edate", "o_address", "o_content",  "fin_type", "na_name", "fin_detail", "fc_num", "fc_money" };
            DataTable dt = new BLL.finance_chk().GetList(0,0, "1=1" + CombSqlTxt(), "fc_addDate desc,fin_adddate desc", out totalCount, out _tMoney, false).Tables[0];
            ExcelHelper.Write(HttpContext.Current, dt, fileName, fileName, strFields, strFieldsName, string.Format("{0}.xlsx", fileName));
        }
        //设置分页数量 
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("ReconciliationSearch_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("ReconciliationSearch.aspx", "page={0}&txtOrderID={1}&txtCusName={2}&hCusId={3}&ddltype={4}&txtNum={5}", "__id__", _orderid, _cusName, _cid, _type, _num));
        }


    }
}