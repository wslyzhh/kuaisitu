using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin
{
    public partial class finance_nature : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RptBind();
            }
        }
        #region 数据绑定=================================
        private void RptBind()
        {
            BLL.businessNature bll = new BLL.businessNature();
            this.rptList.DataSource = bll.GetList(0,"", "na_sort asc,na_id desc"); ; //bll.GetList(0, _strWhere, _orderby);
            this.rptList.DataBind();
        }
        #endregion

        //嵌套绑定
        protected void rptList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            //BLL.article_spec bll = new BLL.article_spec();
            //if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            //{
            //    Repeater rptSpecItem = (Repeater)e.Item.FindControl("rptSpecItem");
            //    //找到关联的数据项 
            //    DataRowView drv = (DataRowView)e.Item.DataItem;
            //    //提取父ID 
            //    int parentId = Convert.ToInt32(drv["id"]);
            //    //根据父ID查询并绑定
            //    rptSpecItem.DataSource = bll.GetList(0, "parent_id=" + parentId, "sort_id asc,id desc");
            //    rptSpecItem.DataBind();
            //}
        }
    }
}