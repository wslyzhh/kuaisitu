using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MettingSys.Common;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using BorderStyle = NPOI.SS.UserModel.BorderStyle;

namespace MettingSys.Web.admin.order
{
    public partial class order_edit : Web.UI.ManagePage
    {
        protected string action = string.Empty;
        protected string oID = "";
        protected Model.Order model = new Model.Order();
        Model.manager manager = null;
        bool isExecutiver = false;//是否是执行人员
        decimal? requestMoney = 0, confirmMoney = 0, finCost = 0, finProfit = 0, fin1 = 0, fin0 = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.action = DTRequest.GetQueryString("action");
            if (string.IsNullOrEmpty(action))
            {
                action = DTEnums.ActionEnum.Add.ToString();
            }
            ChkAdminLevel("sys_order_add", action); //检查权限
            manager = GetAdminInfo();
            if (!string.IsNullOrEmpty(action) && action == DTEnums.ActionEnum.Edit.ToString())
            {
                this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                this.oID = DTRequest.GetQueryString("oID");
                if (string.IsNullOrEmpty(this.oID))
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {                
                initData();
                if (string.IsNullOrEmpty(oID))
                {
                    uploadDiv.Visible = false;
                    uploadDiv2.Visible = false;
                }
                else
                {
                    uploadDiv.Visible = true;
                    uploadDiv2.Visible = true;
                }
                if (action == DTEnums.ActionEnum.Add.ToString())//添加
                {
                    NewShowInfo();
                }
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.oID);
                }
            }
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = new BLL.Order().getOrderCollect(oID);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=结束汇总(" + oID + ").xlsx"); //HttpUtility.UrlEncode(fileName));
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet("明细");
            IFont font = hssfworkbook.CreateFont();
            font.Boldweight = short.MaxValue;
            font.FontHeightInPoints = 11;

            #region 表格样式
            //设置单元格的样式：水平垂直对齐居中
            ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BottomBorderColor = HSSFColor.Black.Index;
            cellStyle.LeftBorderColor = HSSFColor.Black.Index;
            cellStyle.RightBorderColor = HSSFColor.Black.Index;
            cellStyle.TopBorderColor = HSSFColor.Black.Index;
            cellStyle.WrapText = true;//自动换行

            //设置表头的样式：水平垂直对齐居中，加粗
            ICellStyle titleCellStyle = hssfworkbook.CreateCellStyle();
            titleCellStyle.Alignment = HorizontalAlignment.Center;
            titleCellStyle.VerticalAlignment = VerticalAlignment.Center;
            titleCellStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index; //图案颜色
            titleCellStyle.FillPattern = FillPattern.SparseDots; //图案样式
            titleCellStyle.FillBackgroundColor = HSSFColor.Grey25Percent.Index; //背景颜色
            //设置边框
            titleCellStyle.BorderBottom = BorderStyle.Thin;
            titleCellStyle.BorderLeft = BorderStyle.Thin;
            titleCellStyle.BorderRight = BorderStyle.Thin;
            titleCellStyle.BorderTop = BorderStyle.Thin;
            titleCellStyle.BottomBorderColor = HSSFColor.Black.Index;
            titleCellStyle.LeftBorderColor = HSSFColor.Black.Index;
            titleCellStyle.RightBorderColor = HSSFColor.Black.Index;
            titleCellStyle.TopBorderColor = HSSFColor.Black.Index;
            //设置字体
            titleCellStyle.SetFont(font);
            #endregion
            //表头
            IRow headRow = sheet.CreateRow(0);
            headRow.HeightInPoints = 25;

            headRow.CreateCell(0).SetCellValue("收付对象");          
            headRow.CreateCell(1).SetCellValue("收付类别");
            headRow.CreateCell(2).SetCellValue("应收付总额");
            headRow.CreateCell(3).SetCellValue("已收付总额");
            headRow.CreateCell(4).SetCellValue("未收付总额");

            headRow.GetCell(0).CellStyle = titleCellStyle;
            headRow.GetCell(1).CellStyle = titleCellStyle;
            headRow.GetCell(2).CellStyle = titleCellStyle;
            headRow.GetCell(3).CellStyle = titleCellStyle;
            headRow.GetCell(4).CellStyle = titleCellStyle;

            sheet.SetColumnWidth(0, 15 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 20 * 256);
            sheet.SetColumnWidth(4, 20 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["c_name"].ToString());
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["fin_type"])=="1"?"收":"付");
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["finMoney"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["rpdMoney"]));
                    row.CreateCell(4).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["unReceiptPay"]));

                    row.GetCell(0).CellStyle = cellStyle;
                    row.GetCell(1).CellStyle = cellStyle;
                    row.GetCell(2).CellStyle = cellStyle;
                    row.GetCell(3).CellStyle = cellStyle;
                    row.GetCell(4).CellStyle = cellStyle;
                }
            }

            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }

        #region 初始化
        private void initData()
        {
            ddlcontractPrice.DataSource = Common.BusinessDict.ContractPriceType();
            ddlcontractPrice.DataTextField = "key";
            ddlcontractPrice.DataValueField = "key";
            ddlcontractPrice.DataBind();
            ddlcontractPrice.Items.Insert(0, new ListItem("请选择", ""));

            ddlfStatus.DataSource = Common.BusinessDict.fStatus();
            ddlfStatus.DataTextField = "value";
            ddlfStatus.DataValueField = "key";
            ddlfStatus.DataBind();

            ddldstatus.DataSource = Common.BusinessDict.dStatus();
            ddldstatus.DataTextField = "value";
            ddldstatus.DataValueField = "key";
            ddldstatus.DataBind();

            ddlflag.DataSource = Common.BusinessDict.checkStatus();
            ddlflag.DataTextField = "value";
            ddlflag.DataValueField = "key";
            ddlflag.DataBind();

            ddllockstatus.DataSource = Common.BusinessDict.lockStatus(1);
            ddllockstatus.DataTextField = "value";
            ddllockstatus.DataValueField = "key";
            ddllockstatus.DataBind();

            ddlpushStatus.DataSource = Common.BusinessDict.pushStatus();
            ddlpushStatus.DataTextField = "value";
            ddlpushStatus.DataValueField = "key";
            ddlpushStatus.DataBind();

        }

        #endregion

        private void NewShowInfo()
        {
            btnDstatus.Visible = false;
            btnFlag.Visible = false;
            btnLockstatus.Visible = false;
            btnUpdateCost.Visible = false;
            btnUnBusinessPay.Visible = false;
            btnReceiptPay.Visible = false;
            //btnPay.Visible = false;
            btnInvoince.Visible = false;
            //btnExcelIn.Visible = false;

            //默认生成下单人的活动归属地
            Dictionary<string, string> areaDic = new BLL.department().getAreaDict();
            //Dictionary<string, string> orderAreaDic = new Dictionary<string, string>();
            DataTable areaDT = new DataTable();
            areaDT.Columns.Add("area");
            areaDT.Columns.Add("areaText");
            areaDT.Columns.Add("ratio");
            areaDT.Columns.Add("type");
            DataRow dr = areaDT.NewRow();
            dr["area"] = manager.area;
            dr["areaText"] = areaDic[manager.area];
            dr["ratio"] = "100";
            dr["type"] = "1";
            areaDT.Rows.Add(dr);
            //orderAreaDic.Add(manager.area, areaDic[manager.area]);
            rptAreaList.DataSource = areaDT;
            rptAreaList.DataBind();

            //默认下单人
            DataTable pdt = new DataTable();
            pdt.Columns.Add("op_number");
            pdt.Columns.Add("op_name");
            pdt.Columns.Add("op_area");
            DataRow pdr= pdt.NewRow();
            pdr["op_number"] = manager.user_name;
            pdr["op_name"] = manager.real_name;
            pdr["op_area"] = manager.area;
            pdt.Rows.Add(pdr);
            rptEmployee0.DataSource = pdt;
            rptEmployee0.DataBind();
        }

        #region 赋值操作=================================
        private void ShowInfo(string _oid)
        {
            rptEmployee0.Visible = false;
            liemployee0.Visible = false;
            BLL.Order bll = new BLL.Order();
            DataSet ds = bll.GetList(0, "o_id='" + _oid + "'", "o_addDate desc");
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                JscriptMsg("订单不存在！", "");
                return;
            }
            DataRow dr = ds.Tables[0].Rows[0];
            labOwner.Text = new MettingSys.BLL.department().getAreaText(dr["op_area"].ToString()) + "," + dr["op_number"] + "," + dr["op_name"]+"("+dr["op_ratio"]+"%)";
            txtCusName.Text = dr["c_name"].ToString();
            hCusId.Value = dr["c_id"].ToString();
            List<Model.Contacts> contactlist = new BLL.Contacts().getList("co_cid="+ hCusId.Value + "", " co_flag desc,co_id asc");
            if (contactlist != null)
            {
                ddlcontact.DataSource = contactlist;
                ddlcontact.DataTextField = "co_name";
                ddlcontact.DataValueField = "co_id";
                ddlcontact.DataBind();
            }
            ddlcontact.SelectedValue = dr["o_coid"].ToString();
            txtPhone.Text = dr["co_number"].ToString();
            ddlcontractPrice.SelectedValue = dr["o_contractPrice"].ToString();
            txtsDate.Text = ConvertHelper.toDate(dr["o_sdate"]).Value.ToString("yyyy-MM-dd");
            txteDate.Text = ConvertHelper.toDate(dr["o_edate"]).Value.ToString("yyyy-MM-dd");
            txtAddress.Text = dr["o_address"].ToString();
            txtContent.Text = dr["o_content"].ToString();
            txtContract.Text = dr["o_contractContent"].ToString();
            txtRemark.Text = dr["o_remarks"].ToString();
            ddlfStatus.SelectedValue = dr["o_status"].ToString();
            //ddldstatus.SelectedValue = dr["o_dstatus"].ToString();
            ddlpushStatus.SelectedValue = dr["o_isPush"].ToString();
            labFlag.Text = Common.BusinessDict.checkStatus()[Utils.ObjToByte(dr["o_flag"])];
            ddlflag.SelectedValue = dr["o_flag"].ToString();
            labLockStatus.Text = Common.BusinessDict.lockStatus()[Utils.ObjToByte(dr["o_lockStatus"])];
            labfinanceCost.Text = dr["o_financeCust"].ToString();
            txtCost.Text = dr["o_financeCust"].ToString();
            finCost = Utils.StrToDecimal(dr["o_financeCust"].ToString(), 0);
            ddllockstatus.SelectedValue = dr["o_lockStatus"].ToString();
            labFinRemarks.Text = dr["o_finRemarks"].ToString();
            txtFinRemark.Text = dr["o_finRemarks"].ToString();
            labStatusTime.Text = Utils.ObjectToStr(dr["o_statusTime"]) == "" ? "" : Utils.StrToDateTime(Utils.ObjectToStr(dr["o_statusTime"])).ToString("yyyy-MM-dd HH:mm:ss");

            #region 归属地
            DataTable areaDt = new BLL.Order().getOrderPlace(_oid);
            if (areaDt != null)
            {
                rptAreaList.DataSource = areaDt;
                rptAreaList.DataBind();
            }

            #endregion

            #region 人员
            DataTable pdt = bll.GetPersonList(0, "op_oid='" + _oid + "'", "op_id asc").Tables[0];
            if (pdt != null && pdt.Rows.Count > 0)
            {
                rptEmployee1.DataSource = pdt.Select("op_type=2");
                rptEmployee1.DataBind();

                rptEmployee2.DataSource = pdt.Select("op_type=3");
                rptEmployee2.DataBind();

                rptEmployee3.DataSource = pdt.Select("op_type=4");
                rptEmployee3.DataBind();

                rptEmployee4.DataSource = pdt.Select("op_type=5");
                rptEmployee4.DataBind();

                rptEmployee6.DataSource = pdt.Select("op_type=6");
                rptEmployee6.DataBind();

                liplace.Visible = false;
                liemployee1.Visible = false;
                liemployee2.Visible = false;
                liemployee3.Visible = false;
                liemployee4.Visible = false;
                uploadDiv.Visible = false;
                uploadDiv2.Visible = false;
                btnSave.Visible = false;
                btnDstatus.Visible = false;
                btnFlag.Visible = false;
                btnLockstatus.Visible = false;
                btnUpdateCost.Visible = false;
                btnUnBusinessPay.Visible = false;
                btnReceiptPay.Visible = false;
                btnFinRemark.Visible = false;
                //btnPay.Visible = false;
                btnInvoince.Visible = false;
                //btnExcelIn.Visible = false;
                trFile.Visible = false;
                #region 根据当前登录账户显示不同按钮
                DataRow[] drs1 = pdt.Select("(op_type=1 or op_type=6) and op_number='" + manager.user_name + "'");//业务员
                DataRow[] drs2 = pdt.Select("op_type=2 and op_number='" + manager.user_name + "'");//业务报账人员
                DataRow[] drs3 = pdt.Select("op_type=3 and op_number='" + manager.user_name + "'");//业务策划人员
                DataRow[] drs4 = pdt.Select("op_type=4 and op_number='" + manager.user_name + "'");//业务执行人员
                DataRow[] drs6 = pdt.Select("op_type=5 and op_number='" + manager.user_name + "'");//业务设计人员
                bool showDetail = false;

                if (drs4.Length > 0)
                {
                    isExecutiver = true;
                    showDetail = true;
                    uploadDiv.Visible = true;
                    btnUnBusinessPay.Visible = true;
                    btnReceiptPay.Visible = true;
                    //btnPay.Visible = true;
                    btnInvoince.Visible = true;
                    //btnExcelIn.Visible = true;
                }
                if (drs3.Length > 0 || drs6.Length > 0)
                {
                    showDetail = true;
                    uploadDiv.Visible = true;
                    uploadDiv2.Visible = true;
                    btnDstatus.Visible = true;
                    trFile.Visible = true;
                    if (drs3.Length > 0)
                    {
                        ddldstatus.SelectedValue = drs3[0]["op_dstatus"].ToString();
                    }
                    else if (drs6.Length > 0)
                    {
                        ddldstatus.SelectedValue = drs6[0]["op_dstatus"].ToString();
                    }
                }
                if (drs2.Length > 0)
                {
                    isExecutiver = false;
                    showDetail = true;
                    liplace.Visible = true;
                    liemployee2.Visible = true;
                    liemployee3.Visible = true;
                    liemployee4.Visible = true;
                    uploadDiv.Visible = true;
                    uploadDiv2.Visible = true;
                    btnUnBusinessPay.Visible = true;
                    btnReceiptPay.Visible = true;
                    btnSave.Visible = true;
                    //btnPay.Visible = true;
                    btnInvoince.Visible = true;
                    //btnExcelIn.Visible = true;
                    trFile.Visible = true;
                }
                if (drs1.Length > 0)
                {
                    isExecutiver = false;
                    showDetail = true;
                    liplace.Visible = true;
                    liemployee1.Visible = true;
                    liemployee2.Visible = true;
                    liemployee3.Visible = true;
                    liemployee4.Visible = true;
                    uploadDiv.Visible = true;
                    uploadDiv2.Visible = true;
                    btnSave.Visible = true;
                    btnUnBusinessPay.Visible = true;
                    btnReceiptPay.Visible = true;
                    //btnPay.Visible = true;
                    btnInvoince.Visible = true;
                    //btnExcelIn.Visible = true;
                    trFile.Visible = true;
                }
                DataRow[] drs5 = pdt.Select("op_type=1 or op_type=6");
                //判断是否含有查看本区域数据的权限
                if (new BLL.permission().checkHasPermission(manager, "0602") && (drs5[0]["op_area"].ToString() == manager.area || Utils.ObjectToStr(dr["o_place"]).IndexOf(manager.area)>-1))
                {
                    showDetail = true;
                    trFile.Visible = true;
                }
                string groupArea = new BLL.department().getGroupArea();//总部

                //判断是否是本区域，且含有财务基本权限
                if ((drs5[0]["op_area"].ToString() == manager.area || groupArea == manager.area) && new BLL.permission().checkHasPermission(manager, "0401"))
                {
                    showDetail = true;
                    trFile.Visible = true;
                    btnUnBusinessPay.Visible = true;
                    btnReceiptPay.Visible = true;
                    uploadDiv2.Visible = true;
                    //btnPay.Visible = true;
                    btnInvoince.Visible = true;
                    //btnExcelIn.Visible = true;
                }
                //判断是否含有查看本区域审批权限
                if (drs5[0]["op_area"].ToString() == manager.area && new BLL.permission().checkHasPermission(manager, "0603"))
                {
                    trFile.Visible = true;
                    showDetail = true;
                    btnFlag.Visible = true;
                }
                //
                if (groupArea == manager.area)
                {
                    if (new BLL.permission().checkHasPermission(manager, "0401"))
                    {
                        showDetail = true;
                        trFile.Visible = true;
                    }
                    if (new BLL.permission().checkHasPermission(manager, "0405"))
                    {
                        showDetail = true;
                        btnLockstatus.Visible = true;
                        btnUpdateCost.Visible = true;
                        trFile.Visible = true;
                    }
                }
                if (new BLL.permission().checkHasPermission(manager, "0401"))
                {
                    btnFinRemark.Visible = true;                    
                }
                //以上都没有权限的，不能查看订单详细
                if (!showDetail)
                {
                    string msgbox = "parent.jsdialog(\"错误提示\", \"您没有管理该页面的权限，请勿非法进入！\", \"back\")";
                    Response.Write("<script type=\"text/javascript\">" + msgbox + "</script>");
                    Response.End();
                }
                #endregion
            }
            #endregion

            if (dr["o_lockStatus"].ToString()== "1")
            {
                liplace.Visible = false;
                liemployee1.Visible = false;
                liemployee2.Visible = false;
                liemployee3.Visible = false;
                liemployee4.Visible = false;
                uploadDiv.Visible = false;
                uploadDiv2.Visible = false;
                btnSave.Visible = false;
                btnDstatus.Visible = false;
                btnFlag.Visible = false;                
                btnUpdateCost.Visible = false;
                btnUnBusinessPay.Visible = false;
                btnReceiptPay.Visible = false;
                //btnPay.Visible = false;
                btnInvoince.Visible = true;
                //btnExcelIn.Visible = false;
            }

            #region 活动文件
            DataTable fdt = bll.GetFileList(0, "f_oid='" + _oid + "'", "f_addDate asc,f_id asc").Tables[0];
            if (fdt != null && fdt.Rows.Count > 0)
            {
                rptAlbumList.DataSource = fdt.Select("f_type=1");
                rptAlbumList.DataBind();

                rptAlbumList2.DataSource = fdt.Select("f_type=2");
                rptAlbumList2.DataBind();
            }
            #endregion

            string sqlwhere = "";

            decimal yingshou = 0, feikaoheshouru = 0, yingfu = 0, feikaohezhichu = 0, shuifei = 0, ticheng = 0, profit1 = 0, profit2 = 0;
            #region 员工业绩
            DataSet userAchievementData = bll.getOrderUserRatio(_oid);
            if (userAchievementData != null && userAchievementData.Tables[0].Rows.Count > 0)
            {
                DataRow userdr = userAchievementData.Tables[0].NewRow();
                foreach (DataRow udr in userAchievementData.Tables[0].Rows)
                {
                    yingshou += Utils.ObjToDecimal(udr["yingshou"],0);
                    feikaoheshouru += Utils.ObjToDecimal(udr["feikaoheshouru"], 0);
                    yingfu += Utils.ObjToDecimal(udr["yingfu"], 0);
                    feikaohezhichu += Utils.ObjToDecimal(udr["feikaohezhichu"], 0);
                    ticheng += Utils.ObjToDecimal(udr["ticheng"], 0);
                    shuifei += Utils.ObjToDecimal(udr["shuifei"], 0);
                    profit1 += Utils.ObjToDecimal(udr["profit1"], 0);
                    profit2 += Utils.ObjToDecimal(udr["profit2"], 0);
                }
                userdr["name"] = "合计";
                userdr["yingshou"] = yingshou;
                userdr["feikaoheshouru"] = feikaoheshouru;
                userdr["yingfu"] = yingfu;
                userdr["feikaohezhichu"] = feikaohezhichu;
                userdr["ticheng"] = ticheng;
                userdr["shuifei"] = shuifei;
                userdr["profit1"] = profit1;
                if ((yingshou - feikaoheshouru) != 0)
                {
                    userdr["profitRatio1"] = Math.Round(profit1 / (yingshou - feikaoheshouru) * 100, 2);
                }
                else
                {
                    userdr["profitRatio1"] = "0";
                }
                userdr["profit2"] = profit2;
                if ((yingshou - feikaoheshouru) != 0)
                {
                    userdr["profitRatio2"] = Math.Round(profit2 / (yingshou - feikaoheshouru) * 100, 2);
                }
                else
                {
                    userdr["profitRatio2"] = "0";
                }
                userAchievementData.Tables[0].Rows.Add(userdr);
                userAchievement.DataSource = userAchievementData;
                userAchievement.DataBind();
            }

            #endregion

            #region 区域业绩
            DataSet areaAchievementData = bll.getOrderAreaRatio(_oid);
            if (areaAchievementData != null && areaAchievementData.Tables[0].Rows.Count > 0)
            {
                yingshou = 0; feikaoheshouru = 0; yingfu = 0; feikaohezhichu = 0; shuifei = 0; ticheng = 0; profit1 = 0; profit2 = 0;
                DataRow userdr = areaAchievementData.Tables[0].NewRow();
                foreach (DataRow udr in areaAchievementData.Tables[0].Rows)
                {
                    yingshou += Utils.ObjToDecimal(udr["yingshou"], 0);
                    feikaoheshouru += Utils.ObjToDecimal(udr["feikaoheshouru"], 0);
                    yingfu += Utils.ObjToDecimal(udr["yingfu"], 0);
                    feikaohezhichu += Utils.ObjToDecimal(udr["feikaohezhichu"], 0);
                    ticheng += Utils.ObjToDecimal(udr["ticheng"], 0);
                    shuifei += Utils.ObjToDecimal(udr["shuifei"], 0);
                    profit1 += Utils.ObjToDecimal(udr["profit1"], 0);
                    profit2 += Utils.ObjToDecimal(udr["profit2"], 0);
                }
                userdr["name"] = "合计";
                userdr["yingshou"] = yingshou;
                userdr["feikaoheshouru"] = feikaoheshouru;
                userdr["yingfu"] = yingfu;
                userdr["feikaohezhichu"] = feikaohezhichu;
                userdr["ticheng"] = ticheng;
                userdr["shuifei"] = shuifei;
                userdr["profit1"] = profit1;
                if ((yingshou - feikaoheshouru) != 0)
                {
                    userdr["profitRatio1"] = Math.Round(profit1 / (yingshou - feikaoheshouru) * 100, 2);
                }
                else
                {
                    userdr["profitRatio1"] = "0";
                }
                userdr["profit2"] = profit2;
                if ((yingshou - feikaoheshouru) != 0)
                {
                    userdr["profitRatio2"] = Math.Round(profit2 / (yingshou - feikaoheshouru) * 100, 2);
                }
                else
                {
                    userdr["profitRatio2"] = "0";
                }
                areaAchievementData.Tables[0].Rows.Add(userdr);
                areaAchievement.DataSource = areaAchievementData;
                areaAchievement.DataBind();
            }
            #endregion

            #region 执行备用金借款明细
            if (isExecutiver)
            {
                sqlwhere = " and uba_PersonNum='" + manager.user_name + "'";
            }
            DataSet unBusinessData = new BLL.unBusinessApply().GetList(0, "uba_oid='" + _oid + "' " + sqlwhere + "", "uba_addDate desc,uba_id desc");
            if (unBusinessData != null && unBusinessData.Tables[0].Rows.Count > 0)
            {
                rptunBusinessList.DataSource = unBusinessData;
                rptunBusinessList.DataBind();
            }
            #endregion

            #region 应收付
            DataTable natureData = new BLL.finance().getNature(_oid, isExecutiver ? manager.user_name : "");
            if (natureData!= null && natureData.Rows.Count > 0)
            {
                rptNature.DataSource = natureData;
                rptNature.DataBind();
            }

            #endregion

            #region 发票
            if (isExecutiver)
            {
                sqlwhere = " and inv_personNum='" + manager.user_name + "'";
            }
            DataTable invoiceData = new BLL.invoices().GetList(0, "inv_oid='" + _oid + "' "+ sqlwhere + "", "inv_addDate desc,inv_id desc").Tables[0];
            if (invoiceData != null && invoiceData.Rows.Count > 0)
            {
                foreach (DataRow inv in invoiceData.Rows)
                {
                    if (inv["inv_flag1"].ToString() != "1" && inv["inv_flag2"].ToString() != "1" && inv["inv_flag3"].ToString() != "1")
                    {
                        requestMoney += Utils.StrToDecimal(inv["inv_money"].ToString(), 0);
                    }
                    if (Utils.StrToBool(inv["inv_isConfirm"].ToString(), false))
                    {
                        confirmMoney += Utils.StrToDecimal(inv["inv_money"].ToString(), 0);
                    }
                }
                rptInvoiceList.DataSource = invoiceData;
                rptInvoiceList.DataBind();                     
            }
            #endregion

            #region 已收付款
            if (isExecutiver)
            {
                sqlwhere = " and rpd_personNum='" + manager.user_name + "'";
            }
            DataTable rpData = new BLL.ReceiptPayDetail().GetList(0, "rpd_oid='" + _oid + "'", "rpd_type desc,rpd_adddate desc,rpd_id desc").Tables[0];
            if (rpData != null && rpData.Rows.Count > 0)
            {
                rptList.DataSource = rpData;
                rptList.DataBind();
            }
            #endregion

            #region 结算汇总
            if (!isExecutiver)//执行人员不可查看
            {
                DataTable collectData= bll.getOrderCollect(_oid);
                if (collectData != null && collectData.Rows.Count > 0)
                {
                    foreach (DataRow inv in collectData.Rows)
                    {
                        finProfit += Utils.StrToDecimal(inv["profit"].ToString(), 0);
                        if (inv["fin_type"].ToString() == "True")
                        {
                            fin1 += Utils.StrToDecimal(inv["finMoney"].ToString(), 0);
                        }
                        else
                        {
                            fin0 += Utils.StrToDecimal(inv["finMoney"].ToString(), 0);
                        }
                    }

                    rptCollect.DataSource = collectData;
                    rptCollect.DataBind();
                }
            }
            #endregion
        }
        #endregion
        

        protected void rptNature_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string sqlwhere = "";
                if (isExecutiver)
                {
                    sqlwhere = " and fin_personNum='" + manager.user_name + "'";
                }                
                Repeater rep = e.Item.FindControl("rptFinanceList") as Repeater;
                DataRowView rowv = (DataRowView)e.Item.DataItem;
                rep.DataSource = new BLL.finance().GetList(0, "fin_oid='" + rowv["fin_oid"] + "' and fin_nature=" + rowv["fin_nature"] + " " + sqlwhere + "", "fin_type desc,fin_adddate desc,fin_id desc");
                rep.DataBind();
            }
        }

        protected void rptInvoiceList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                Label _labRequestInv = e.Item.FindControl("labRequestInv") as Label;
                _labRequestInv.Text = requestMoney.ToString();
                Label _labConfirmInv = e.Item.FindControl("labConfirmInv") as Label;
                _labConfirmInv.Text = confirmMoney.ToString();
                Label _labLeftInv = e.Item.FindControl("labLeftInv") as Label;
                _labLeftInv.Text = new BLL.invoices().computeInvoiceLeftMoney(oID).ToString();
            }
        }

        protected void rptCollect_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                Label _labfin1 = e.Item.FindControl("labFinance1") as Label;
                _labfin1.Text = fin1.ToString();
                Label _labfin0 = e.Item.FindControl("labFinance0") as Label;
                _labfin0.Text = fin0.ToString();
                Label _labProfit = e.Item.FindControl("labProfit") as Label;
                _labProfit.Text = finProfit.ToString();
                Label _labCost = e.Item.FindControl("labCost") as Label;
                _labCost.Text = finCost.ToString();
                Label _labBusinessProfit = e.Item.FindControl("labBusinessProfit") as Label;
                _labBusinessProfit.Text = (finProfit - finCost).ToString();
            }
        }
    }
}