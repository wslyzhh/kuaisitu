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
    public partial class settleCustomer : Web.UI.ManagePage
    {
        protected string _sdate = "", _edate = "", _sdate1 = "", _edate1 = "", _sdate2 = "", _edate2 = "", _status = "";
        Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _sdate = DTRequest.GetString("txtsDate");
            _edate = DTRequest.GetString("txteDate");
            _sdate1 = DTRequest.GetString("txtsDate1"); 
            _edate1 = DTRequest.GetString("txteDate1"); 
            _sdate2 = DTRequest.GetString("txtsDate2"); 
            _edate2 = DTRequest.GetString("txteDate2"); 
            _status = DTRequest.GetString("ddlstatus"); 

            manager = GetAdminInfo();
            if (!Page.IsPostBack)
            {
                InitData();
                ChkAdminLevel("sys_settlementCustomer", DTEnums.ActionEnum.View.ToString()); //检查权限
                RptBind();
            }
        }

        #region 初始化
        private void InitData()
        {
            ddlstatus.DataSource = Common.BusinessDict.fStatus(1);
            ddlstatus.DataTextField = "value";
            ddlstatus.DataValueField = "key";
            ddlstatus.DataBind();
            ddlstatus.Items.Insert(0, new ListItem("不限", ""));
        }
        #endregion

        #region 数据绑定=================================
        private void RptBind()
        {
            //decimal _tFinMoney = 0, _tRpMoney = 0, _tUnRpMoney = 0, _tRpdMoney = 0, _tUnRpdMoney = 0;
            BLL.finance bll = new BLL.finance();
            DataTable dt = bll.getSettleCustomerList(_sdate, _edate, _sdate1, _edate1, _sdate2, _edate2, _status).Tables[0];
            this.rptList.DataSource = dt;
            this.rptList.DataBind();

            //if (dt!=null)
            //{
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        _tFinMoney += Utils.ObjToDecimal(dr["finMoney"], 0);
            //        _tRpMoney += Utils.ObjToDecimal(dr["rpMoney"], 0);
            //        _tRpdMoney += Utils.ObjToDecimal(dr["rpdMoney"], 0);
            //    }
            //    _tUnRpMoney = _tFinMoney - _tRpMoney;
            //    _tUnRpdMoney = _tRpMoney - _tRpdMoney;
            //}

            //tFinMoney.Text = _tFinMoney.ToString();
            //tRpMoney.Text = _tRpMoney.ToString();
            //tUnRpMoney.Text = _tUnRpMoney.ToString();
            //tRpdMoney.Text = _tRpdMoney.ToString();
            //tUnRpdMoney.Text = _tUnRpdMoney.ToString();
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt()
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp.Append(" and datediff(day,o_sdate,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp.Append(" and datediff(day,o_sdate,'" + _edate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_sdate1))
            {
                strTemp.Append(" and datediff(day,o_edate,'" + _sdate1 + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate1))
            {
                strTemp.Append(" and datediff(day,o_edate,'" + _edate1 + "')>=0");
            }
            if (!string.IsNullOrEmpty(_status))
            {
                switch (_status)
                {
                    case "3":
                        strTemp.Append(" and (o_status=1 or o_status=2)");
                        break;
                    default:
                        strTemp.Append(" and o_status=" + _status + "");
                        break;
                }
            }
            return strTemp.ToString();
        }
        #endregion


        //关健字查询
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            RptBind();
        }
    }
}