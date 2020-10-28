using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.statistic
{
    public partial class RevenueAnalysis_list : Web.UI.ManagePage
    {
        protected int totalCount; //数据总记录数
        protected int page; //当前页码
        protected int pageSize; //每页大小

        decimal _pShou = 0, _pFu = 0, _pProfit = 0;
        decimal _tShou = 0, _tFu = 0, _tProfit = 0;

        protected string action="",_page="",_excel="", _sMonth = "", _eMonth = "",_cusName="", _cid = "", _lockstatus = "", _area = "", _nature = "", _person1 = "", _person2 = "", _person3 = "", _person4 = "", _group = "", _isCust="";

        protected void Page_Load(object sender, EventArgs e)
        {
            ChkAdminLevel("sys_RevenueAnalysis", DTEnums.ActionEnum.View.ToString()); //检查权限
            this.pageSize = GetPageSize(10); //每页数量
            _page = DTRequest.GetString("page");
            _sMonth = DTRequest.GetString("txtsDate");
            _eMonth = DTRequest.GetString("txteDate");
            _cusName = DTRequest.GetString("txtCusName");
            _cid = DTRequest.GetString("hCusId");
            _lockstatus = DTRequest.GetString("ddllock");
            _area = DTRequest.GetString("hide_place");
            _nature = DTRequest.GetString("hide_nature");
            _person1 = DTRequest.GetString("hide_employee1");
            _person2 = DTRequest.GetString("hide_employee2");
            _person3 = DTRequest.GetString("hide_employee3");
            _person4 = DTRequest.GetString("hide_employee4");
            _group = DTRequest.GetString("ddlGroup");
            _isCust = DTRequest.GetString("cbIsCust");
            _excel = DTRequest.GetString("Excel");
            action = DTRequest.GetString("action");
            InitData();
            if (!IsPostBack && _excel != "on" && string.IsNullOrEmpty(_page))
            {
                _sMonth = DateTime.Now.ToString("yyyy-MM");
                _eMonth = DateTime.Now.ToString("yyyy-MM");
                _lockstatus = "1";
                _isCust = "on";
                RptBind();
            }
            if (action == "Search")
            {
                RptBind();
            }
            if (_excel == "on")
            {
                Excel();
            }

            #region 绑定控件值
            txtsDate.Text = _sMonth;
            txteDate.Text = _eMonth;
            ddllock.SelectedValue = _lockstatus;
            cbIsCust.Checked = _isCust == "on" ? true : false;
            txtCusName.Text = _cusName;
            hCusId.Value = _cid;
            ddlGroup.SelectedValue = _group;
            string placeStr = _area;
            if (!string.IsNullOrEmpty(placeStr))
            {
                Dictionary<string, string> areaDic = new BLL.department().getAreaDict();
                Dictionary<string, string> orderAreaDic = new Dictionary<string, string>();
                string[] list = placeStr.Split(',');
                foreach (string item in list)
                {
                    if (areaDic.ContainsKey(item))
                    {
                        orderAreaDic.Add(item, areaDic[item]);
                    }
                }
                rptAreaList.DataSource = orderAreaDic;
                rptAreaList.DataBind();
            }
            else
            {
                rptAreaList.DataSource = null;
                rptAreaList.DataBind();
            }
            if (!string.IsNullOrEmpty(_nature))
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("na_id");
                dt.Columns.Add("na_name");
                string[] list = _nature.Split(',');
                foreach (string item in list)
                {
                    DataRow dr = dt.NewRow();
                    string[] lis = item.Split('|');
                    dr["na_id"] = lis[0];
                    dr["na_name"] = lis[1];
                    dt.Rows.Add(dr);
                }                
                rptNatureList.DataSource = dt;
                rptNatureList.DataBind();
            }
            else
            {
                rptNatureList.DataSource = null;
                rptNatureList.DataBind();
            }
            if (!string.IsNullOrEmpty(_person1))
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("op_name");
                dt.Columns.Add("op_number");
                dt.Columns.Add("op_area");
                string[] list = _person1.Split(',');
                foreach (string item in list)
                {
                    DataRow dr = dt.NewRow();
                    string[] lis = item.Split('|');
                    dr["op_name"] = lis[0];
                    dr["op_number"] = lis[1];
                    dr["op_area"] = lis[2];
                    dt.Rows.Add(dr);
                }
                rptEmployee1.DataSource = dt;
                rptEmployee1.DataBind();
            }
            else
            {
                rptEmployee1.DataSource = null;
                rptEmployee1.DataBind();
            }
            if (!string.IsNullOrEmpty(_person2))
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("op_name");
                dt.Columns.Add("op_number");
                dt.Columns.Add("op_area");
                string[] list = _person2.Split(',');
                foreach (string item in list)
                {
                    DataRow dr = dt.NewRow();
                    string[] lis = item.Split('|');
                    dr["op_name"] = lis[0];
                    dr["op_number"] = lis[1];
                    dr["op_area"] = lis[2];
                    dt.Rows.Add(dr);
                }
                rptEmployee2.DataSource = dt;
                rptEmployee2.DataBind();
            }
            else
            {
                rptEmployee2.DataSource = null;
                rptEmployee2.DataBind();
            }
            if (!string.IsNullOrEmpty(_person3))
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("op_name");
                dt.Columns.Add("op_number");
                dt.Columns.Add("op_area");
                string[] list = _person3.Split(',');
                foreach (string item in list)
                {
                    DataRow dr = dt.NewRow();
                    string[] lis = item.Split('|');
                    dr["op_name"] = lis[0];
                    dr["op_number"] = lis[1];
                    dr["op_area"] = lis[2];
                    dt.Rows.Add(dr);
                }
                rptEmployee3.DataSource = dt;
                rptEmployee3.DataBind();
            }
            else
            {
                rptEmployee3.DataSource = null;
                rptEmployee3.DataBind();
            }
            if (!string.IsNullOrEmpty(_person4))
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("op_name");
                dt.Columns.Add("op_number");
                dt.Columns.Add("op_area");
                string[] list = _person4.Split(',');
                foreach (string item in list)
                {
                    DataRow dr = dt.NewRow();
                    string[] lis = item.Split('|');
                    dr["op_name"] = lis[0];
                    dr["op_number"] = lis[1];
                    dr["op_area"] = lis[2];
                    dt.Rows.Add(dr);
                }
                rptEmployee4.DataSource = dt;
                rptEmployee4.DataBind();
            }
            else
            {
                rptEmployee4.DataSource = null;
                rptEmployee4.DataBind();
            }
            #endregion
        }
        #region 初始化数据=================================
        private void InitData()
        {
            ddllock.DataSource = Common.BusinessDict.lockStatus(1);
            ddllock.DataTextField = "value";
            ddllock.DataValueField = "key";
            ddllock.DataBind();
            ddllock.Items.Insert(0, new ListItem("不限", ""));
        }
        #endregion

        private Dictionary<string, string> getDict()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(_sMonth))
            {
                dict.Add("smonth", _sMonth);
            }
            if (!string.IsNullOrEmpty(_eMonth))
            {
                dict.Add("emonth", _eMonth);
            }
            if (!string.IsNullOrEmpty(_lockstatus))
            {
                dict.Add("lockstatus", _lockstatus);
            }
            if (!string.IsNullOrEmpty(_cusName) || !string.IsNullOrEmpty(_cid))
            {
                dict.Add("cusname", _cusName);
                dict.Add("cid", _cid);
            }
            if (!string.IsNullOrEmpty(_area))
            {
                dict.Add("area", _area);
            }
            if (!string.IsNullOrEmpty(_nature))
            {
                string[] list = _nature.Split(',');
                string pStr = string.Empty;
                foreach (string item in list)
                {
                    string[] litem = item.Split('|');
                    pStr += litem[0] + ",";
                }
                dict.Add("nature", pStr.TrimEnd(','));
            }
            if (!string.IsNullOrEmpty(_person1))
            {
                string[] list = _person1.Split(',');
                string pStr = string.Empty;
                foreach (string item in list)
                {
                    string[] litem = item.Split('|');
                    pStr += litem[1] + ",";
                }
                dict.Add("person1", pStr.TrimEnd(','));
            }
            if (!string.IsNullOrEmpty(_person2))
            {
                string[] list = _person2.Split(',');
                string pStr = string.Empty;
                foreach (string item in list)
                {
                    string[] litem = item.Split('|');
                    pStr += litem[1] + ",";
                }
                dict.Add("person2", pStr.TrimEnd(','));
            }
            if (!string.IsNullOrEmpty(_person3))
            {
                string[] list = _person3.Split(',');
                string pStr = string.Empty;
                foreach (string item in list)
                {
                    string[] litem = item.Split('|');
                    pStr += litem[1] + ",";
                }
                dict.Add("person3", pStr.TrimEnd(','));
            }
            if (!string.IsNullOrEmpty(_person4))
            {
                string[] list = _person4.Split(',');
                string pStr = string.Empty;
                foreach (string item in list)
                {
                    string[] litem = item.Split('|');
                    pStr += litem[1] + ",";
                }
                dict.Add("person4", pStr.TrimEnd(','));
            }
            dict.Add("isCust", _isCust);
            dict.Add("group",_group);
            return dict;
        }

        #region 数据绑定=================================
        private void RptBind()
        {
            DataTable dt = null;
            this.page = DTRequest.GetQueryInt("page", 1);
            BLL.statisticBLL bll = new BLL.statisticBLL();
            Dictionary<string, string> dict = getDict();
            if (string.IsNullOrEmpty(_group))
            {
                dt = bll.getRevenueAnalysisListData(dict, this.pageSize, this.page, "o_id asc", out this.totalCount, out _tShou, out _tFu, out _tProfit).Tables[0];
                rptList.DataSource = dt;
                rptList.DataBind();
                rptList.Visible = true;
                rptList1.Visible = false;
                rptList2.Visible = false;
                rptList3.Visible = false;
                rptList4.Visible = false;
                rptList5.Visible = false;
                rptList6.Visible = false;
                rptList7.Visible = false;
            }
            else
            {
                switch (_group)
                {
                    case "1"://区域
                        dt= bll.getRevenueAnalysisListData1(dict, this.pageSize, this.page, "op_area", "op_area,o_financeCust", "op_area asc", out this.totalCount, out _tShou, out _tFu, out _tProfit).Tables[0];
                        rptList1.DataSource = dt;
                        rptList1.DataBind();
                        rptList.Visible = false;
                        rptList1.Visible = true;
                        rptList2.Visible = false;
                        rptList3.Visible = false;
                        rptList4.Visible = false;
                        rptList5.Visible = false;
                        rptList6.Visible = false;
                        rptList7.Visible = false;
                        break;
                    case "2"://客源
                        dt= bll.getRevenueAnalysisListData1(dict, this.pageSize, this.page, "c_name,op_area", "c_name,op_area,o_financeCust", "op_area asc", out this.totalCount, out _tShou, out _tFu, out _tProfit).Tables[0];
                        rptList2.DataSource = dt;
                        rptList2.DataBind();
                        rptList.Visible = false;
                        rptList1.Visible = false;
                        rptList2.Visible = true;
                        rptList3.Visible = false;
                        rptList4.Visible = false;
                        rptList5.Visible = false;
                        rptList6.Visible = false;
                        rptList7.Visible = false;
                        break;
                    case "3"://业务员
                        dt= bll.getRevenueAnalysisListData1(dict, this.pageSize, this.page, "op_name,op_area,op_number", "op_name,op_area,op_number,o_financeCust", "op_area asc", out this.totalCount, out _tShou, out _tFu, out _tProfit).Tables[0];
                        rptList3.DataSource = dt;
                        rptList3.DataBind();
                        rptList.Visible = false;
                        rptList1.Visible = false;
                        rptList2.Visible = false;
                        rptList3.Visible = true;
                        rptList4.Visible = false;
                        rptList5.Visible = false;
                        rptList6.Visible = false;
                        rptList7.Visible = false;
                        break;
                    case "4"://策划人员
                    case "8"://设计人员
                        dt = bll.getRevenueAnalysisListData2(dict, this.pageSize, this.page, "p3.op_name,p3.op_area,p3.op_number", "op_number asc", out this.totalCount, out _tShou, out _tFu, out _tProfit).Tables[0];
                        rptList4.DataSource = dt;
                        rptList4.DataBind();
                        rptList.Visible = false;
                        rptList1.Visible = false;
                        rptList2.Visible = false;
                        rptList3.Visible = false;
                        rptList4.Visible = true;
                        rptList5.Visible = false;
                        rptList6.Visible = false;
                        rptList7.Visible = false;
                        break;
                    case "5"://执行人员
                        dt= bll.getRevenueAnalysisListData3(dict, this.pageSize, this.page, "p3.op_name,p3.op_area,p3.op_number", "op_number asc", out this.totalCount, out _tShou, out _tFu, out _tProfit).Tables[0];
                        rptList5.DataSource = dt;
                        rptList5.DataBind();
                        rptList.Visible = false;
                        rptList1.Visible = false;
                        rptList2.Visible = false;
                        rptList3.Visible = false;
                        rptList4.Visible = false;
                        rptList5.Visible = true;
                        rptList6.Visible = false;
                        rptList7.Visible = false;
                        break;
                    case "6"://月份
                        dt= bll.getRevenueAnalysisListData1(dict, this.pageSize, this.page, "datepart(year,o_edate) oYear, datepart(month,o_edate) oMonth,op_area", "datepart(year,o_edate), datepart(month,o_edate),op_area,o_financeCust", "datepart(year,o_edate) asc,datepart(month,o_edate) asc", out this.totalCount, out _tShou, out _tFu, out _tProfit).Tables[0];
                        rptList6.DataSource = dt;
                        rptList6.DataBind();
                        rptList.Visible = false;
                        rptList1.Visible = false;
                        rptList2.Visible = false;
                        rptList3.Visible = false;
                        rptList4.Visible = false;
                        rptList5.Visible = false;
                        rptList6.Visible = true;
                        rptList7.Visible = false;
                        break;
                    case "7"://业务性质
                        dt= bll.getRevenueAnalysisListData1(dict, this.pageSize, this.page, "na_name,na_sort,op_area", "na_name,na_sort,op_area,o_financeCust", "na_sort asc", out this.totalCount, out _tShou, out _tFu, out _tProfit).Tables[0];
                        rptList7.DataSource = dt;
                        rptList7.DataBind();
                        rptList.Visible = false;
                        rptList1.Visible = false;
                        rptList2.Visible = false;
                        rptList3.Visible = false;
                        rptList4.Visible = false;
                        rptList5.Visible = false;
                        rptList6.Visible = false;
                        rptList7.Visible = true;
                        break;
                    default:
                        break;
                }
            }

            if (dt!=null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    _pShou += Utils.ObjToDecimal(dr["shou"], 0);
                    _pFu += Utils.ObjToDecimal(dr["fu"], 0);
                    _pProfit += Utils.ObjToDecimal(dr["profit"], 0);
                }
            }

            //绑定页码
            txtPageNum.Text = this.pageSize.ToString();
            string pageUrl = Utils.CombUrlTxt("RevenueAnalysis_list.aspx", "page={0}&txtsDate={1}&txteDate={2}&ddllock={3}&action={4}&hide_place={5}&hide_nature={6}&txtCusName={7}&hCusId={8}&cbIsCust={9}&hide_employee1={10}&hide_employee3={11}&hide_employee4={12}&ddlGroup={13}", "__id__", _sMonth, _eMonth,_lockstatus, action,_area,_nature,_cusName,_cid,_isCust,_person1,_person3,_person4, _group);
            PageContent.InnerHtml = Utils.OutPageList(this.pageSize, this.page, this.totalCount, pageUrl, 8);

            pCount.Text = dt.Rows.Count.ToString();
            pShou.Text = _pShou.ToString();
            pFu.Text = _pFu.ToString();
            pProfit.Text = _pProfit.ToString();

            tCount.Text = totalCount.ToString();
            tShou.Text = _tShou.ToString();
            tFu.Text = _tFu.ToString();
            tProfit.Text = _tProfit.ToString();
        }
        #endregion

        #region 返回每页数量=============================
        private int GetPageSize(int _default_size)
        {
            int _pagesize;
            if (int.TryParse(Utils.GetCookie("RevenueAnalysis_page_size", "DTcmsPage"), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    return _pagesize;
                }
            }
            return _default_size;
        }
        #endregion
        
        //设置分页数量
        protected void txtPageNum_TextChanged(object sender, EventArgs e)
        {
            int _pagesize;
            if (int.TryParse(DTRequest.GetFormString("txtPageNum"), out _pagesize))
            {
                if (_pagesize > 0)
                {
                    Utils.WriteCookie("RevenueAnalysis_page_size", "DTcmsPage", _pagesize.ToString(), 14400);
                }
            }
            Response.Redirect(Utils.CombUrlTxt("RevenueAnalysis_list.aspx", "page={0}&txtsDate={1}&txteDate={2}&ddllock={3}&action={4}&hide_place={5}&hide_nature={6}&txtCusName={7}&hCusId={8}&cbIsCust={9}&hide_employee1={10}&hide_employee3={11}&hide_employee4={12}&ddlGroup={13}", "__id__", _sMonth, _eMonth, _lockstatus, action, _area, _nature, _cusName, _cid, _isCust, _person1, _person3, _person4,_group));
        }
        protected void Excel()
        {
            string fileName = "";
            string[] strFieldsName = { };
            string[] strFields = new string[] { };
            DataTable dt = null;     
            BLL.statisticBLL bll = new BLL.statisticBLL();
            Dictionary<string, string> dict = getDict();            
            if (string.IsNullOrEmpty(_group))
            {
                fileName = "客源收益分析-明细列表";
                strFieldsName = new string[]{"订单号","客源","活动名称","活动地点", "活动结束日期", "税费成本", "业务性质", "应收金额", "应付金额", "业务毛利", "区域", "业务员", "设计策划人员", "执行人员" };
                strFields = new string[] { "o_id", "c_name", "o_content", "o_address", "o_edate", "o_financeCust", "na_name", "shou", "fu", "profit", "op_area", "op_name", "person3", "person4" };
                dt = bll.getRevenueAnalysisListData(dict, this.pageSize, this.page, "o_id asc", out this.totalCount, out _tShou, out _tFu, out _tProfit, false).Tables[0];
            }
            else
            {
                switch (_group)
                {
                    case "1"://区域
                        fileName = "客源收益分析-区域分组";
                        strFieldsName = new string[] { "区域", "应收金额", "应付金额", "业务毛利" };
                        strFields = new string[] { "op_area", "shou", "fu", "profit" };
                        dt = bll.getRevenueAnalysisListData1(dict, this.pageSize, this.page, "op_area", "op_area,o_financeCust", "op_area asc", out this.totalCount, out _tShou, out _tFu, out _tProfit, false).Tables[0];
                        break;
                    case "2"://客源
                        fileName = "客源收益分析-客源分组";
                        strFieldsName = new string[] { "客源", "区域", "应收金额", "应付金额", "业务毛利" };
                        strFields = new string[] { "c_name", "op_area", "shou", "fu", "profit" };
                        dt = bll.getRevenueAnalysisListData1(dict, this.pageSize, this.page, "c_name,op_area", "c_name,op_area,o_financeCust", "op_area asc", out this.totalCount, out _tShou, out _tFu, out _tProfit, false).Tables[0];
                        break;
                    case "3"://业务员
                        fileName = "客源收益分析-业务员分组";
                        strFieldsName = new string[] { "业务员","应收金额", "应付金额", "业务毛利" };
                        strFields = new string[] { "op_number/op_area/op_name", "shou", "fu", "profit" };
                        dt = bll.getRevenueAnalysisListData1(dict, this.pageSize, this.page, "op_name,op_area,op_number", "op_name,op_area,op_number,o_financeCust", "op_area asc", out this.totalCount, out _tShou, out _tFu, out _tProfit, false).Tables[0];
                        break;
                    case "4"://设计策划人员
                        fileName = "客源收益分析-设计策划人员分组";
                        strFieldsName = new string[] { "设计策划人员", "应收金额", "应付金额", "业务毛利" };
                        strFields = new string[] { "op_number/op_area/op_name", "shou", "fu", "profit" };
                        dt = bll.getRevenueAnalysisListData2(dict, this.pageSize, this.page, "p3.op_name,p3.op_area,p3.op_number", "p3.op_number asc", out this.totalCount, out _tShou, out _tFu, out _tProfit, false).Tables[0];
                        break;
                    case "5"://执行人员
                        fileName = "客源收益分析-执行人员分组";
                        strFieldsName = new string[] { "执行人员", "应收金额", "应付金额", "业务毛利" };
                        strFields = new string[] { "op_number/op_area/op_name", "shou", "fu", "profit" };
                        dt = bll.getRevenueAnalysisListData3(dict, this.pageSize, this.page, "p3.op_name,p3.op_area,p3.op_number", "p3.op_number asc", out this.totalCount, out _tShou, out _tFu, out _tProfit, false).Tables[0];
                        break;
                    case "6"://月份
                        fileName = "客源收益分析-月份分组";
                        strFieldsName = new string[] { "月份", "区域", "应收金额", "应付金额", "业务毛利" };
                        strFields = new string[] { "oYear/oMonth", "op_area", "shou", "fu", "profit" };
                        dt = bll.getRevenueAnalysisListData1(dict, this.pageSize, this.page, "datepart(year,o_edate) oYear, datepart(month,o_edate) oMonth,op_area", "datepart(year,o_edate), datepart(month,o_edate),op_area,o_financeCust", "datepart(year,o_edate) asc,datepart(month,o_edate) asc", out this.totalCount, out _tShou, out _tFu, out _tProfit, false).Tables[0];
                        break;
                    case "7"://业务性质
                        fileName = "客源收益分析-业务性质分组";
                        strFieldsName = new string[] { "业务性质", "区域", "应收金额", "应付金额", "业务毛利" };
                        strFields = new string[] { "na_name", "op_area", "shou", "fu", "profit" };
                        dt = bll.getRevenueAnalysisListData1(dict, this.pageSize, this.page, "na_name,na_sort,op_area", "na_name,na_sort,op_area,o_financeCust", "na_sort asc", out this.totalCount, out _tShou, out _tFu, out _tProfit, false).Tables[0];
                        break;
                    default:
                        break;
                }
            }
            ExcelHelper.Write(HttpContext.Current, dt, fileName, fileName, strFields, strFieldsName, string.Format("{0}.xlsx", fileName));
        }
    }
}