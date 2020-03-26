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
    public partial class paydetailcollect : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string _cusName = "", _cid = "", _sforedate = string.Empty, _eforedate = string.Empty, _check = string.Empty;
        protected Model.business_log logmodel = null;
        Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _sforedate = DTRequest.GetString("txtsforedate");
            _eforedate = DTRequest.GetString("txteforedate");
            this.pageSize = GetPageSize(10); //每页数量
            manager = GetAdminInfo();
            if (!Page.IsPostBack)
            {
                //_eforedate = DateTime.Now.ToString("yyyy-MM-dd");
                initData();
                ChkAdminLevel("sys_payment_detail0", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("rpd_id>0" + CombSqlTxt(), "c_name");
            }
            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            txtsforedate.Text = _sforedate;
            txteforedate.Text = _eforedate;
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
            ddlmethod.Items.Insert(0, new ListItem("请选择", ""));

            ddlmethod1.DataSource = new BLL.payMethod().GetList(0, "pm_isUse=1 " + sqlwhere + "", "pm_sort asc,pm_id asc");
            ddlmethod1.DataTextField = "pm_name";
            ddlmethod1.DataValueField = "pm_id";
            ddlmethod1.DataBind();
            ddlmethod1.Items.Insert(0, new ListItem("请选择", ""));
        }
        #endregion
        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            DataTable dt = bll.getCollectList(this.pageSize, this.page, _strWhere, _orderby, out this.totalCount, out decimal tmoney).Tables[0];
            this.rptList.DataSource = dt;
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("paydetailcollect.aspx", "page={0}&txtCusName={1}&hCusId={2}&txtsforedate={3}&txteforedate={4}", "__id__",_cusName,_cid,_sforedate,_eforedate);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            pCount.Text = dt.Rows.Count.ToString();
            decimal _pmoney = 0;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pmoney += Utils.ObjToDecimal(dr["total"], 0);
                }
            }
            pMoney.Text = _pmoney.ToString();
            tCount.Text = totalCount.ToString();
            tMoney.Text = tmoney.ToString();
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_cid) && _cid !="0")
            {
                strTemp.Append(" and rpd_cid = " + _cid + "");
            }
            if (!string.IsNullOrEmpty(_cusName))
            {
                strTemp.Append(" and c_name like '%" + _cusName + "%'");
            }
            if (!string.IsNullOrEmpty(_sforedate))
            {
                strTemp.Append(" and datediff(d,rpd_foreDate,'" + _sforedate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_eforedate))
            {
                strTemp.Append(" and datediff(d,rpd_foreDate,'" + _eforedate + "')>=0");
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
            _cusName = DTRequest.GetFormString("txtCusName");
            _cid = DTRequest.GetFormString("hCusId");
            _sforedate = DTRequest.GetFormString("txtsforedate");
            _eforedate = DTRequest.GetFormString("txteforedate");
            RptBind("rpd_id>0" + CombSqlTxt(), "c_name");

            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            txtsforedate.Text = _sforedate;
            txteforedate.Text = _eforedate;
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
            Response.Redirect(Utils.CombUrlTxt("paydetailcollect.aspx", "page={0}&txtCusName={1}&hCusId={2}&txtsforedate={3}&txteforedate={4}", "__id__", _cusName, _cid, _sforedate, _eforedate));
        }
        
    }
}