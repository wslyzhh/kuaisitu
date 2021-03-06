﻿using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.baseData
{
    public partial class payMethod_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string keywords = string.Empty, _isUse = string.Empty;
        private Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            keywords = DTRequest.GetString("txtKeywords");
            _isUse = DTRequest.GetString("ddlIsUse");
            pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("pub_paymethod", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("pm_id>0" + CombSqlTxt(), "pm_sort asc,pm_id desc");
            }
            txtKeywords.Text = keywords;
            ddlIsUse.SelectedValue = _isUse;
        }

        #region 数据绑定=================================
        private void RptBind(string _strWhere, string _orderby)
        {
            this.page = DTRequest.GetQueryInt("page", 1);
            this.txtKeywords.Text = this.keywords;
            BLL.payMethod bll = new BLL.payMethod();
            this.rptList.DataSource = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, out this.totalCount);
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("payMethod_list.aspx", "txtKeywords={0}&page={1}&ddlIsUse={2}", this.keywords, "__id__",_isUse);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(keywords))
            {
                strTemp.Append(" and pm_name like  '%" + keywords + "%'");
            }
            if (!string.IsNullOrEmpty(_isUse))
            {
                strTemp.Append(" and pm_isUse='"+ _isUse + "'");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("payMethod_page_size", "DTcmsPage"), out _pagesize))
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
            keywords = DTRequest.GetFormString("txtKeywords");
            _isUse = DTRequest.GetFormString("ddlIsUse");
            RptBind("pm_id>0" + CombSqlTxt(), "pm_sort asc,pm_id desc");
            txtKeywords.Text = keywords;
            ddlIsUse.SelectedValue = _isUse;
        }

        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(txtPageNum.Text.Trim(), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("payMethod_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("payMethod_list.aspx", "txtKeywords={0}&page={1}&ddlIsUse={2}", this.keywords, "__id__", _isUse));
        }

        //保存排序
        protected void btnSave_Click(object sender, EventArgs e)
        {
            ChkAdminLevel("pub_paymethod", DTEnums.ActionEnum.Edit.ToString()); //检查权限
            BLL.payMethod bll = new BLL.payMethod();
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                int id = Convert.ToInt32(((HiddenField)rptList.Items[i].FindControl("hidId")).Value);
                int sortId;
                if (!int.TryParse(((TextBox)rptList.Items[i].FindControl("txtSortId")).Text.Trim(), out sortId))
                {
                    sortId = 99;
                }
                bll.UpdateSort(id, sortId);
            }
            JscriptMsg("保存排序成功！", Utils.CombUrlTxt("payMethod_list.aspx", "txtKeywords={0}&page={1}&ddlIsUse={2}", this.keywords, "__id__", _isUse));
        }

        //批量删除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            PrintLoad();
            ChkAdminLevel("pub_paymethod", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.payMethod bll = new BLL.payMethod();

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
            JscriptMsg("共选择" + (success + error) + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), Utils.CombUrlTxt("payMethod_list.aspx", "txtKeywords={0}&page={1}&ddlIsUse={2}", this.keywords, "__id__", _isUse));
        }
    }
}