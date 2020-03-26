using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MettingSys.Common;

namespace MettingSys.Web.admin.finance
{
    public partial class FinancialClosing : Web.UI.ManagePage
    {
        protected string flag = "", _lastMonth = "", _smonth = "", _emonth = "", _defaultMonth = "",_area="";
        BLL.finance bll = new BLL.finance();
        decimal _money1 = 0, _money0 = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            flag = DTRequest.GetString("flag");
            _smonth = DTRequest.GetString("txtsDate");
            _emonth = DTRequest.GetString("txteDate");
            _area = DTRequest.GetString("ddlarea");
            if (string.IsNullOrEmpty(flag))
            {
                flag = "0";
            }
            _lastMonth = bll.getLastFinancialMonth();
            if (flag == "0")
            {
                if (string.IsNullOrEmpty(_lastMonth))
                {
                    //_lastMonth = DateTime.Now.ToString("yyyy-MM");
                    //labUnFinMonth.Text = _lastMonth;
                    _defaultMonth = DateTime.Now.ToString("yyyy-MM");
                    labUnFinMonth.Visible = false;
                    labtitle.Text = "请选择首次结账月份：";
                }
                else
                {
                    _defaultMonth = _lastMonth;
                    _lastMonth = ConvertHelper.toDate(_lastMonth + "-01").Value.AddMonths(1).ToString("yyyy-MM");
                    labUnFinMonth.Text = _lastMonth;
                    txtDate.Visible = false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_lastMonth))
                {
                    labFinMonth.Text = "还未结账过";
                    btnFinancial.Visible = false;
                }
                else
                {
                    labFinMonth.Text = _lastMonth;
                }
            }
            if (!IsPostBack)
            {
                InitData();
                _smonth = _defaultMonth;
                _emonth = _defaultMonth;
                RptBind();
                div0.Visible = false;
                div1.Visible = false;
                if (flag == "0")
                {
                    div0.Visible = true;                    
                }
                else
                {
                    div1.Visible = true;
                }
            }
            txtsDate.Text = _smonth;
            txteDate.Text = _emonth;
            ddlarea.SelectedValue = _area;
        }
        #region 初始化数据=================================
        private void InitData()
        {
            ddlarea.DataSource = new BLL.department().getAreaDict();
            ddlarea.DataTextField = "value";
            ddlarea.DataValueField = "key";
            ddlarea.DataBind();
            ddlarea.Items.Insert(0, new ListItem("不限", ""));
        }
        #endregion
        #region 数据绑定=================================
        private void RptBind()
        {
            DataTable dt = bll.getFinancialSumary(_smonth, _emonth,_area).Tables[0];
            this.rptList.DataSource = dt;
            this.rptList.DataBind();

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["fin_type"].ToString() == "True")
                    {
                        _money1 += Utils.ObjToDecimal(dr["fin_money"], 0);
                    }
                    else
                    {
                        _money0 += Utils.ObjToDecimal(dr["fin_money"], 0);
                    }
                }
                money1.Text = _money1.ToString();
                money0.Text = _money0.ToString();
            }

        }
        #endregion
        /// <summary>
        /// 结账
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUnFinancial_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_lastMonth))
            {
                if (string.IsNullOrEmpty(txtDate.Text.Trim()))
                {
                    JscriptMsg("请先选择首次结账月份", "");
                    return;
                }
                _lastMonth = txtDate.Text.Trim();
            }
            if (!bll.dealFinancial(_lastMonth))
            {
                JscriptMsg("结账失败", "");
                return;
            }
            JscriptMsg("结账成功！", "FinancialClosing.aspx?flag=0");
        }
        
        /// <summary>
        /// 反结账
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFinancial_Click(object sender, EventArgs e)
        {
            if (!bll.cancelFinancial(_lastMonth))
            {
                JscriptMsg("反结账失败", "");
                return;
            }
            JscriptMsg("反结账成功！", "FinancialClosing.aspx?flag=1");
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            _smonth = DTRequest.GetFormString("txtsDate");
            _emonth = DTRequest.GetFormString("txteDate");
            _area = DTRequest.GetFormString("ddlarea");
            RptBind();
            txtsDate.Text = _smonth;
            txteDate.Text = _emonth;
            ddlarea.SelectedValue = _area;
        }
    }
}