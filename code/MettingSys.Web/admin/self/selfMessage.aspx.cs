using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.self
{
    public partial class selfMessage : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string keywords = string.Empty,_isRead=string.Empty;
        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.keywords = DTRequest.GetString("txtKeywords");
            this._isRead = DTRequest.GetString("ddlisRead");
            this.pageSize = GetPageSize(10); //每页数量
            manager = GetAdminInfo();
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("sys_myself_message", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("me_id>0 and me_owner='" + manager.user_name + "'" + CombSqlTxt(this.keywords, this._isRead), "me_isRead asc,me_addDate desc,me_id desc");
            }
            txtKeywords.Text = keywords;
            ddlisRead.SelectedValue = _isRead;
        }

        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            this.txtKeywords.Text = this.keywords;
            BLL.selfMessage bll = new BLL.selfMessage();
            this.rptList.DataSource = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, out this.totalCount);
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("selfMessage.aspx", "txtKeywords={0}&page={1}&ddlisRead={2}", this.keywords, "__id__",_isRead);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt(string _keywords,string isRead)
        {
            StringBuilder strTemp = new StringBuilder();
            _keywords = _keywords.Replace("'", "");
            if (!string.IsNullOrEmpty(isRead))
            {
                strTemp.Append(" and me_isRead="+isRead+"");
            }
            if (!string.IsNullOrEmpty(_keywords))
            {
                strTemp.Append(" and (me_title like  '%" + _keywords + "%' of me_content like  '%" + _keywords + "%')");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("selfMessage_page_size", "DTcmsPage"), out _pagesize))
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
            this.keywords = DTRequest.GetFormString("txtKeywords");
            this._isRead = DTRequest.GetFormString("ddlisRead");
            RptBind("me_id>0 and me_owner='" + manager.user_name + "'" + CombSqlTxt(this.keywords, this._isRead), "me_isRead asc,me_addDate desc,me_id desc");
            txtKeywords.Text = keywords;
            ddlisRead.SelectedValue = _isRead;
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("selfMessage_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("selfMessage.aspx", "txtKeywords={0}&ddlisRead={1}", this.keywords,_isRead));
        }
        //批量删除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ChkAdminLevel("sys_myself_message", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.selfMessage bll = new BLL.selfMessage();
            logmodel = new Model.business_log();
            string idstr = string.Empty;
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                int id = Convert.ToInt32(((HiddenField)rptList.Items[i].FindControl("hidId")).Value);
                CheckBox cb = (CheckBox)rptList.Items[i].FindControl("chkId");
                if (cb.Checked)
                {
                    idstr += id.ToString() + ",";
                }
            }
            idstr = idstr.TrimEnd(',');
            DataSet ds = bll.GetList(0, "me_id in (" + idstr + ")", "");
            string nameStr = string.Empty;
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    nameStr += "[" + dr["me_id"] + "]" + dr["me_title"] + ",";
                }
                nameStr = nameStr.TrimEnd(',');
                if (bll.Delete(idstr))
                {
                    logmodel.ol_title = "删除个人消息";
                    logmodel.ol_content = nameStr;
                    logmodel.ol_operateDate = DateTime.Now;
                    AddBusinessLog(DTEnums.ActionEnum.Delete.ToString(), logmodel); //记录日志
                    JscriptMsg("删除成功！", Utils.CombUrlTxt("selfMessage.aspx", "keywords={0}&ddlisRead={1}", this.keywords,_isRead), "");
                }
            }
            JscriptMsg("删除失败！", Utils.CombUrlTxt("selfMessage.aspx", "txtKeywords={0}&ddlisRead={1}", this.keywords,_isRead), "");
        }
        //批量已读
        protected void btnRead_Click(object sender, EventArgs e)
        {
            ChkAdminLevel("sys_myself_message", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.selfMessage bll = new BLL.selfMessage();
            string idstr = string.Empty;
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                int id = Convert.ToInt32(((HiddenField)rptList.Items[i].FindControl("hidId")).Value);
                CheckBox cb = (CheckBox)rptList.Items[i].FindControl("chkId");
                if (cb.Checked)
                {
                    idstr += id.ToString() + ",";
                }
            }
            idstr = idstr.TrimEnd(',');
            if (bll.updateRecordReadStatus(idstr) > 0)
            {
                JscriptMsg("操作成功！", Utils.CombUrlTxt("selfMessage.aspx", "txtKeywords={0}&ddlisRead={1}", this.keywords,_isRead), "");
            }
            JscriptMsg("操作失败！", Utils.CombUrlTxt("selfMessage.aspx", "txtKeywords={0}&ddlisRead={1}", this.keywords,_isRead), "");
        }
    }
}