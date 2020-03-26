using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.baseData
{
    public partial class business_log : Web.UI.ManagePage
    {
        protected int totalCount;
        protected int page;
        protected int pageSize;

        protected string keywords = string.Empty;
        protected string start_time = string.Empty;
        protected string end_time = string.Empty;
        Model.manager model = new Model.manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            this.start_time = DTRequest.GetQueryString("start_time");
            this.end_time = DTRequest.GetQueryString("end_time");
            this.keywords = DTRequest.GetQueryString("keywords");
            this.pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("business_log", DTEnums.ActionEnum.View.ToString()); //检查权限
                model = GetAdminInfo(); //取得当前管理员信息
                RptBind("ol_id>0" + CombSqlTxt(this.start_time, this.end_time, keywords), "ol_operateDate desc,ol_id desc");
            }
        }

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt(string _start_time, string _end_time, string _keywords)
        {
            StringBuilder strTemp = new StringBuilder();
            _start_time = _start_time.Replace("'", "");
            if (!string.IsNullOrEmpty(_start_time))
            {
                strTemp.Append(" and datediff(s,ol_operateDate,'" + _start_time + "')<=0");
            }
            _end_time = _end_time.Replace("'", "");
            if (!string.IsNullOrEmpty(_end_time))
            {
                strTemp.Append(" and datediff(s,ol_operateDate,'" + _end_time + "')>=0");
            }
            _keywords = _keywords.Replace("'", "");
            if (!string.IsNullOrEmpty(_keywords))
            {
                strTemp.Append(" and (ol_oid like  '%" + _keywords + "%' or ol_cid like '%" + _keywords + "%' or ol_relateID like '%" + _keywords + "%' or ol_title like '%" + _keywords + "%' or ol_content like '%" + _keywords + "%')");
            }

            return strTemp.ToString();
        }
        #endregion

        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            this.txtStartTime.Text = this.start_time;
            this.txtEndTime.Text = this.end_time;
            txtKeywords.Text = this.keywords;
            BLL.business_log bll = new BLL.business_log();
            this.rptList.DataSource = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, out this.totalCount);
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("business_log.aspx", "start_time={0}&end_time={1}&keywords={2}&page={3}",
                this.start_time, this.end_time, this.keywords, "__id__");
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("business_log_size", "DTcmsPage"), out _pagesize))
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
            Response.Redirect(Utils.CombUrlTxt("business_log.aspx", "start_time={0}&end_time={1}&keywords={2}", txtStartTime.Text, txtEndTime.Text, txtKeywords.Text));
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("business_log_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("business_log.aspx", "start_time={0}&end_time={1}&keywords={2}", this.start_time, this.end_time, this.keywords));
        }
    }
}