using MettingSys.Common;
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
    public partial class businessNature_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        protected string keywords = string.Empty, _flag = "", _isUse = "";
        private Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            keywords = DTRequest.GetString("txtKeywords");
            _flag = DTRequest.GetString("ddlflag");
            _isUse = DTRequest.GetString("ddlisUse");

            this.pageSize = GetPageSize(10); //每页数量
            if (!Page.IsPostBack)
            {
                InitData();
                ChkAdminLevel("pub_nature", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind("na_id>0" + CombSqlTxt(this.keywords,_flag,_isUse), "na_sort asc,na_id desc");
            }
            txtKeywords.Text = keywords;
            ddlflag.SelectedValue = _flag;
            ddlisUse.SelectedValue = _isUse;
        }
        #region 初始化数据=================================
        private void InitData()
        {
            ddlisUse.DataSource = Common.BusinessDict.isUseStatus();
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
            this.txtKeywords.Text = this.keywords;
            BLL.businessNature bll = new BLL.businessNature();
            this.rptList.DataSource = bll.GetList(this.pageSize, this.page, _strWhere, _orderby, out this.totalCount);
            this.rptList.DataBind();

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("businessNature_list.aspx", "txtKeywords={0}&page={1}&ddlflag={2}&ddlisUse={3}", this.keywords, "__id__",_flag,_isUse);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt(string _keywords,string flag,string isUse)
        {
            StringBuilder strTemp = new StringBuilder();
            _keywords = _keywords.Replace("'", "");
            if (!string.IsNullOrEmpty(_keywords))
            {
                strTemp.Append(" and na_name like  '%" + _keywords + "%'");
            }
            if (!string.IsNullOrEmpty(flag))
            {
                strTemp.Append(" and na_flag='"+flag+"'");
            }
            if (!string.IsNullOrEmpty(isUse))
            {
                strTemp.Append(" and na_isUse='" + isUse + "'");
            }
            return strTemp.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("businessNature_page_size", "DTcmsPage"), out _pagesize))
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
            _flag = DTRequest.GetFormString("ddlflag");
            _isUse = DTRequest.GetFormString("ddlisUse");

            RptBind("na_id>0" + CombSqlTxt(this.keywords, _flag, _isUse), "na_sort asc,na_id desc");
            txtKeywords.Text = keywords;
            ddlflag.SelectedValue = _flag;
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
                    Utils.WriteCookie("businessNature_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("businessNature_list.aspx", "txtKeywords={0}&ddlflag={1}&ddlisUse={2}", txtKeywords.Text, ddlflag.SelectedValue, ddlisUse.SelectedValue));
        }

        //保存排序
        protected void btnSave_Click(object sender, EventArgs e)
        {
            ChkAdminLevel("pub_nature", DTEnums.ActionEnum.Edit.ToString()); //检查权限
            BLL.businessNature bll = new BLL.businessNature();
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
            JscriptMsg("保存排序成功！", Utils.CombUrlTxt("businessNature_list.aspx", "txtKeywords={0}&ddlflag={1}&ddlisUse={2}", txtKeywords.Text, ddlflag.SelectedValue, ddlisUse.SelectedValue));
        }

        //批量删除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            PrintLoad();
            ChkAdminLevel("pub_nature", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.businessNature bll = new BLL.businessNature();
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
            JscriptMsg("共选择" + (success + error) + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), Utils.CombUrlTxt("businessNature_list.aspx", "txtKeywords={0}&ddlflag={1}&ddlisUse={2}", txtKeywords.Text, ddlflag.SelectedValue, ddlisUse.SelectedValue));
        }
    }
}