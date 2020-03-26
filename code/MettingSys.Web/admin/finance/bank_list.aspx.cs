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
    public partial class bank_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string _cusName = "", _cid = "", _isUse = string.Empty;
        private Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _isUse = DTRequest.GetString("ddlisUse");

            this.pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                InitData();
                ChkAdminLevel("sys_customerBank", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("cb_id>0" + CombSqlTxt(), "cb_id asc");
            }
            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            ddlisUse.SelectedValue = _isUse;
        }
        #region 初始化数据=================================
        private void InitData()
        {
            //启用状态
            ddlisUse.DataSource = Common.BusinessDict.isUseStatus(1);
            ddlisUse.DataTextField = "value";
            ddlisUse.DataValueField = "key";
            ddlisUse.DataBind();
            ddlisUse.Items.Insert(0, new ListItem("不限", ""));
        }
        #endregion
        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.customerBank bll = new BLL.customerBank();
            this.rptList.DataSource = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, out this.totalCount);
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("bank_list.aspx", "page={0}&txtCusName={1}&hCusId={2}&ddlisUse={3}", "__id__", _cusName, _cid, _isUse);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_cid) && _cid != "0")
            {
                strTemp.Append(" and cb_cid = " + _cid + "");
            }
            if (!string.IsNullOrEmpty(_isUse))
            {
                strTemp.Append(" and cb_flag='" + _isUse + "'");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("customerBank_page_size", "DTcmsPage"), out _pagesize))
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
            _isUse = DTRequest.GetFormString("ddlisUse");
            RptBind("cb_id>0" + CombSqlTxt(), "cb_id asc");
            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            ddlisUse.SelectedValue = _isUse;
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("customerBank_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("bank_list.aspx", "page={0}&txtCusName={1}&hCusId={2}&ddlisUse={3}", "__id__", _cusName, _cid, _isUse));
        }
        //批量删除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            PrintLoad();
            ChkAdminLevel("sys_customerBank", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.customerBank bll = new BLL.customerBank();
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
                    result = bll.Delete(id, manager);
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
            JscriptMsg("共选择" + (success + error) + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), Utils.CombUrlTxt("bank_list.aspx", "page={0}&txtCusName={1}&hCusId={2}&ddlisUse={3}", "__id__", _cusName, _cid, _isUse));
        }
    }
}