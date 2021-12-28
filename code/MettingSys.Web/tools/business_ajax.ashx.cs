using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Text;
using MettingSys.Web.UI;
using Newtonsoft.Json.Linq;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections;

namespace MettingSys.Web.tools
{
    /// <summary>
    /// business_ajax 的摘要说明
    /// </summary>
    public class business_ajax : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            //检查管理员是否登录
            if (!new ManagePage().IsAdminLogin())
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"尚未登录或已超时，请登录后操作！\"}");
                return;
            }
            string action = DTRequest.GetQueryString("action");
            switch (action)
            {
                case "AddPay":
                    addOreditPay(context);
                    break;
                case "AddReceipt":
                    addOreditReceipt(context);
                    break;
                case "AddReceiptDetails":
                    addOreditReceiptDetails(context);
                    break;
                case "getAllNature":
                    getAllNature(context);
                    break;
                case "getAllDetail":
                    getAllDetail(context);
                    break;
                case "checkCustomerName":
                    check_CustomerName(context);
                    break;
                case "subContacts":
                    getSubContacts(context);
                    break;
                case "checkCustomerStatus":
                    check_CustomerStatus(context);
                    break;
                case "getAllCustomer":
                    get_AllCustomer(context);
                    break;
                case "updateCustomerOwner":
                    update_CustomerOwner(context);
                    break;
                case "getContactsByCid":
                    get_ContactsByCid(context);
                    break;
                case "getPhone":
                    get_ContactPhone(context);
                    break;
                case "mergeCustomer":
                    merge_Customer(context);
                    break;
                case "computeResult":
                    compute_Result(context);
                    break;
                case "getBusinessDetail":
                    get_BusinessDetail(context);
                    break;
                case "saveFinance":
                    save_Finance(context);
                    break;
                case "mutilAddFinance":
                    Add_Finance(context);
                    break;
                case "AddShareFinance":
                    AddShareFinance(context);
                    break;
                case "checkfinance":
                    check_finance(context);
                    break;
                case "checkfinanceStatus":
                    check_financeStatus(context);
                    break;
                case "deletefinance":
                    delete_finance(context);
                    break;
                case "mutildeletefinance":
                    mutildelete_finance(context);
                    break;
                case "insertFinRemark":
                    insert_FinRemark(context);
                    break;
                case "checkPay":
                    check_Pay(context);
                    break;
                case "confirmReceiptPay":
                    confirm_ReceiptPay(context);
                    break;
                case "checkPayDetailStatus":
                    check_PayDetailStatus(context);
                    break;
                case "getDetails":
                    get_Details(context);
                    break;
                case "collectDetail":
                    collect_Detail(context);
                    break;
                case "mutlCollect":
                    mutl_Collect(context);
                    break;
                case "cancelCollect":
                    cancelCollect(context);
                    break;
                case "mutlUnCollect":
                    mutlCancel_Collect(context);
                    break;
                case "setPayMethod":
                    set_PayMethod(context);
                    break;
                case "setPayMethod_mutli":
                    setPayMethod_mutli(context);
                    break;
                case "deleteReceiptPayDetail":
                    delete_ReceiptPayDetail(context);
                    break;
                case "computeInvMoney":
                    compute_InvMoney(context);
                    break;
                case "deleteinvoice":
                    delete_invoice(context);
                    break;
                case "checkInvoice":
                    check_Invoice(context);
                    break;
                case "confirmInvoice":
                    confirm_Invoice(context);
                    break;
                case "checkCertificateStatus":
                    check_CertificateStatus(context);
                    break;
                case "getAllCertificate":
                    get_AllCertificate(context);
                    break;
                case "signCertificate":
                    sign_Certificate(context);
                    break;
                case "unsignCertificate":
                    unsign_Certificate(context);
                    break;
                case "addCertificate":
                    add_Certificate(context);
                    break;
                case "addInnerCustomer":
                    add_InnerCustomer(context);
                    break;
                case "loadOrderExcel":
                    load_OrderExcel(context);
                    break;
                case "addFinChk":
                    add_FinChk(context);
                    break;
                case "cancelFinChk":
                    cancel_FinChk(context);
                    break;
                case "addChk":
                    add_Chk(context);
                    break;
                case "delChk":
                    del_Chk(context);
                    break;
                case "dealDistribute":
                    deal_Distribute(context);
                    break;
                case "getDistributeMoney":
                    get_DistributeMoney(context);
                    break;
                case "getCusBank":
                    get_customerBank(context);
                    break;
                case "saveBank":
                    save_Bank(context);
                    break;
                case "getChkDetail":
                    get_ChkDetail(context);
                    break;
                case "changeChkNum":
                    change_ChkNum(context);
                    break;
                default:
                    break;
            }
        }
        #region 添加编辑付款通知
        private void addOreditPay(HttpContext context)
        {
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            int rpid = 0;
            string result = string.Empty;
            Model.ReceiptPay model = new Model.ReceiptPay();
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            rpid = Utils.StrToInt(DTRequest.GetFormString("rpID"), 0);
            if (rpid == 0)
            {
                model.rp_type = false;
                model.rp_cid = Utils.StrToInt(DTRequest.GetFormString("hCusId"), 0);
                model.rp_money = Utils.StrToDecimal(DTRequest.GetFormString("txtMoney"), 0);
                model.rp_foredate = ConvertHelper.toDate(DTRequest.GetFormString("txtforedate"));
                model.rp_method = Utils.StrToInt(DTRequest.GetFormString("ddlmethod"), 0);
                model.rp_content = DTRequest.GetFormString("txtContent");
                model.rp_cbid = Utils.StrToInt(DTRequest.GetFormString("hBankId"), 0);
                if (context.Request.Files.Count >0)
                {
                    string fileext = "";
                    for (int i = 0; i < context.Request.Files.Count; i++)
                    {
                        fileext = System.IO.Path.GetExtension(context.Request.Files[i].FileName).TrimStart('.');//jpg,jpge,png,gif
                                                                                                                //检查文件扩展名是否合法
                        if (!CheckFileExt(fileext))
                        {
                            context.Response.Write("{ \"msg\":\"不允许上传" + fileext + "类型的文件\", \"status\":1, \"id\":" + rpid + " }");
                            context.Response.End();
                        }
                        byte[] byteData = FileHelper.ConvertStreamToByteBuffer(context.Request.Files[i].InputStream); //获取文件流
                                                                                                                   //检查文件大小是否合法
                        if (!CheckFileSize(fileext, byteData.Length))
                        {
                            context.Response.Write("{ \"msg\":\"文件超过限制的大小\", \"status\":1, \"id\":" + rpid + " }");
                            context.Response.End();
                        }
                    }
                }
                result = bll.Add(model, adminModel, DTRequest.GetFormString("txtCenum"), DTRequest.GetFormString("txtCedate"), out rpid);
                if (string.IsNullOrEmpty(result))
                {
                    if (context.Request.Files.Count>0)
                    {
                        for (int i = 0; i < context.Request.Files.Count; i++)
                        {
                            string savePath = context.Server.MapPath("~/uploadPay/");
                            if (!Directory.Exists(savePath))
                            {
                                //需要注意的是，需要对这个物理路径有足够的权限，否则会报错
                                //另外，这个路径应该是在网站之下，而将网站部署在C盘却把文件保存在D盘
                                Directory.CreateDirectory(savePath);
                            }
                            if (!Directory.Exists(savePath + "3/" + rpid + "/"))
                            {
                                Directory.CreateDirectory(savePath + "3/" + rpid + "/");
                            }
                            Model.payPic file = new Model.payPic();
                            file.pp_rid = rpid;
                            file.pp_type = 3;
                            file.pp_fileName = context.Request.Files[i].FileName;
                            file.pp_filePath = "uploadPay/3/" + rpid + "/" + context.Request.Files[i].FileName;
                            //file.pp_thumbFilePath = jo["thumb"].ToString();
                            file.pp_size = Math.Round((decimal)context.Request.Files[i].ContentLength / 1024, 2, MidpointRounding.AwayFromZero);
                            file.pp_addDate = DateTime.Now;
                            file.pp_addName = adminModel.real_name;
                            file.pp_addPerson = adminModel.user_name;

                            context.Request.Files[i].SaveAs(savePath + "3/" + rpid + "/" + context.Request.Files[i].FileName);//保存文件

                            new BLL.payPic().insertPayFile(file, adminModel);
                        }
                    }
                }

            }
            else
            {
                model = bll.GetModel(rpid);
                string _content = string.Empty;
                string _hcusid = DTRequest.GetFormString("hCusId");
                if (model.rp_cid.ToString() != _hcusid)
                {
                    _content += "付款对象ID：" + model.rp_cid + "→<font color='red'>" + _hcusid + "</font><br/>";
                }
                model.rp_cid = Utils.StrToInt(_hcusid, 0);
                bool updateMoney = false;
                string _money = DTRequest.GetFormString("txtMoney");
                if (model.rp_money.ToString() != _money)
                {
                    if ((model.rp_money < 0 && Utils.ObjToDecimal(_money, 0) >= 0) || (model.rp_money >= 0 && Utils.ObjToDecimal(_money, 0) < 0))
                    {
                        updateMoney = true;//表示金额从负数改为正数，或从正数改为负数
                    }
                    _content += "付款金额：" + model.rp_money + "→<font color='red'>" + _money + "</font><br/>";
                }
                model.rp_money = Utils.StrToDecimal(_money, 0);
                string _foredate = DTRequest.GetFormString("txtforedate");
                if (model.rp_foredate.Value.ToString("yyyy-MM-dd") != _foredate)
                {
                    _content += "预付日期：" + model.rp_foredate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + _foredate + "</font><br/>";
                }
                model.rp_foredate = ConvertHelper.toDate(_foredate);
                string _method = DTRequest.GetFormString("ddlmethod");
                if (model.rp_method.ToString() != _method)
                {
                    _content += "付款方式ID：" + model.rp_method + "→<font color='red'>" + _method + "</font><br/>";
                }
                model.rp_method = Utils.StrToInt(_method, 0);
                string _rpcontent = DTRequest.GetFormString("txtContent");
                if (model.rp_content != _rpcontent)
                {
                    _content += "付款内容：" + model.rp_content + "→<font color='red'>" + _rpcontent + "</font><br/>";
                }
                model.rp_content = _rpcontent;
                string _bankid = DTRequest.GetFormString("hBankId");
                if (model.rp_cbid != Utils.StrToInt(_bankid, 0))
                {
                    _content += "客户银行账号：" + model.rp_cbid + "→<font color='red'>" + _bankid + "</font><br/>";
                }
                model.rp_cbid = Utils.StrToInt(_bankid, 0);
                result = bll.Update(model, _content, adminModel, DTRequest.GetFormString("txtCenum"), DTRequest.GetFormString("txtCedate"), updateMoney);
            }
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0, \"id\":" + rpid + " }");
                context.Response.End();
            }
            else
            {
                context.Response.Write("{ \"msg\":\""+ result + "\", \"status\":1, \"id\":" + rpid + " }");
                context.Response.End();
            }
        }

        #endregion
        #region 添加编辑收款通知
        private void addOreditReceipt(HttpContext context)
        {
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            int rpid = 0;
            string result = string.Empty;
            Model.ReceiptPay model = new Model.ReceiptPay();
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            rpid = Utils.StrToInt(DTRequest.GetFormString("rpID"), 0);
            if (rpid == 0)
            {
                model.rp_type = true;
                model.rp_cid = Utils.StrToInt(DTRequest.GetFormString("hCusId"), 0);
                model.rp_money = Utils.StrToDecimal(DTRequest.GetFormString("txtMoney"), 0);
                model.rp_foredate = ConvertHelper.toDate(DTRequest.GetFormString("txtforedate"));
                model.rp_method = Utils.StrToInt(DTRequest.GetFormString("ddlmethod"), 0);
                model.rp_content = DTRequest.GetFormString("txtContent");
                model.rp_cbid = 0;
                if (model.rp_money < 0)
                {
                    model.rp_cbid = Utils.StrToInt(DTRequest.GetFormString("hBankId"), 0);
                }
                //在客户对账明细的账单打包时不是预收款
                string oidStr = DTRequest.GetFormString("oidStr");
                string tMoney = DTRequest.GetFormString("tMoney");
                string chk = DTRequest.GetFormString("chk");
                bool flag = true;
                if (!string.IsNullOrEmpty(oidStr))
                {
                    flag = false;
                }
                result = bll.Add(model, adminModel, DTRequest.GetFormString("txtCenum"), DTRequest.GetFormString("txtCedate"), out rpid, flag);
                if (string.IsNullOrEmpty(result))
                {
                    if (!string.IsNullOrEmpty(oidStr) && model.rp_money == Utils.StrToDecimal(tMoney, 0))
                    {
                        JArray ja = JArray.Parse(oidStr);
                        foreach (JObject item in ja)
                        {
                            Model.ReceiptPayDetail rpdModel = new Model.ReceiptPayDetail();
                            rpdModel.rpd_type = true;
                            rpdModel.rpd_rpid = rpid;
                            rpdModel.rpd_oid = item["oid"].ToString();
                            rpdModel.rpd_cid = model.rp_cid;
                            rpdModel.rpd_num = chk;
                            rpdModel.rpd_money = Utils.StrToDecimal(item["money"].ToString(), 0);
                            rpdModel.rpd_foredate = model.rp_foredate;
                            rpdModel.rpd_method = model.rp_method;
                            rpdModel.rpd_personName = adminModel.real_name;
                            rpdModel.rpd_personNum = adminModel.user_name;
                            rpdModel.rpd_flag1 = 2;
                            rpdModel.rpd_flag2 = 2;
                            rpdModel.rpd_flag3 = 2;
                            rpdModel.rpd_adddate = DateTime.Now;
                            rpdModel.rpd_content = "";
                            rpdModel.rpd_cbid = model.rp_cbid;
                            new BLL.ReceiptPayDetail().AddOrCancleDistribution(rpdModel, adminModel);
                        }
                    }
                }
            }
            else
            {
                model = bll.GetModel(rpid);                
                string _content = string.Empty;
                string _hcusid = DTRequest.GetFormString("hCusId");
                if (model.rp_cid.ToString() != _hcusid)
                {
                    _content += "收款对象ID：" + model.rp_cid + "→<font color='red'>" + _hcusid + "</font><br/>";
                }
                model.rp_cid = Utils.StrToInt(_hcusid, 0);
                string _money = DTRequest.GetFormString("txtMoney");
                if (model.rp_money.ToString() != _money)
                {
                    _content += "收款金额：" + model.rp_money + "→<font color='red'>" + _money + "</font><br/>";
                }
                model.rp_money = Utils.StrToDecimal(_money, 0); 
                string _foredate = DTRequest.GetFormString("txtforedate");
                if (model.rp_foredate.Value.ToString("yyyy-MM-dd") != _foredate)
                {
                    _content += "预收日期：" + model.rp_foredate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + _foredate + "</font><br/>";
                }
                model.rp_foredate = ConvertHelper.toDate(_foredate);
                string _method = DTRequest.GetFormString("ddlmethod");
                if (model.rp_method.ToString() != _method)
                {
                    _content += "收款方式ID：" + model.rp_method.ToString() + "→<font color='red'>" + _method + "</font><br/>";
                }
                model.rp_method = Utils.StrToInt(_method, 0);
                string _rpcontent = DTRequest.GetFormString("txtContent");
                if (model.rp_content != _rpcontent)
                {
                    _content += "收款内容：" + model.rp_content + "→<font color='red'>" + _rpcontent + "</font><br/>";
                }
                model.rp_content = _rpcontent;
                string _bankid = DTRequest.GetFormString("hBankId");
                if (model.rp_cbid != Utils.StrToInt(_bankid, 0))
                {
                    _content += "客户银行账号：" + model.rp_cbid + "→<font color='red'>" + _bankid + "</font><br/>";
                }
                model.rp_cbid = Utils.StrToInt(_bankid, 0);
                result = bll.Update(model, _content, adminModel, DTRequest.GetFormString("txtCenum"), DTRequest.GetFormString("txtCedate"));
            }
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0, \"id\":" + rpid + " }");
                context.Response.End();
            }
            else
            {
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1, \"id\":" + rpid + " }");
                context.Response.End();
            }
        }
        #endregion
        #region 添加编辑收款明细
        private void addOreditReceiptDetails(HttpContext context)
        {
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            int rpdid = 0;
            string result = string.Empty, fromOrder = string.Empty,oID= DTRequest.GetFormString("oID");
            Model.ReceiptPayDetail model = new Model.ReceiptPayDetail();
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            rpdid = Utils.StrToInt(DTRequest.GetFormString("rpdID"), 0);
            fromOrder = DTRequest.GetFormString("fromOrder");
            if (rpdid == 0)
            {
                model.rpd_type = true;
                model.rpd_oid = oID;
                model.rpd_cid = Utils.StrToInt(DTRequest.GetFormString("hCusId"), 0);
                model.rpd_content = DTRequest.GetFormString("txtContent");
                model.rpd_money = Utils.StrToDecimal(DTRequest.GetFormString("txtMoney"), 0);
                model.rpd_foredate = ConvertHelper.toDate(DTRequest.GetFormString("txtforedate"));
                model.rpd_method = Utils.StrToInt(DTRequest.GetFormString("ddlmethod"), 0);
                //model.rpd_content = txtContent.Text.Trim();
                model.rpd_personNum = adminModel.user_name;
                model.rpd_personName = adminModel.real_name;
                model.rpd_adddate = DateTime.Now;
                model.rpd_flag1 = 2;
                //model.rpd_area = manager.area;    
                model.rpd_cbid = 0;
                if (model.rpd_money < 0)
                {
                    model.rpd_cbid = Utils.StrToInt(DTRequest.GetFormString("hBankId"), 0);
                }
                result = bll.AddReceiptPay(model, adminModel, out rpdid);
            }
            else
            {
                model = bll.GetModel(rpdid);
                string _content = string.Empty;
                if (model.rpd_cid.ToString() != DTRequest.GetFormString("hCusId"))
                {
                    _content += "收款对象ID：" + model.rpd_cid + "→<font color='red'>" + DTRequest.GetFormString("hCusId") + "</font><br/>";
                }
                model.rpd_cid = Utils.StrToInt(DTRequest.GetFormString("hCusId"), 0);
                bool updateMoney = false;
                if (model.rpd_money.ToString() != DTRequest.GetFormString("txtMoney"))
                {
                    updateMoney = true;
                    _content += "收款金额：" + model.rpd_money + "→<font color='red'>" + DTRequest.GetFormString("txtMoney") + "</font><br/>";
                }
                model.rpd_money = Utils.StrToDecimal(DTRequest.GetFormString("txtMoney"), 0);
                if (model.rpd_foredate.Value.ToString("yyyy-MM-dd") != DTRequest.GetFormString("txtforedate"))
                {
                    _content += "预收日期：" + model.rpd_foredate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + DTRequest.GetFormString("txtforedate") + "</font><br/>";
                }
                model.rpd_foredate = ConvertHelper.toDate(DTRequest.GetFormString("txtforedate"));
                if (model.rpd_method.ToString() != DTRequest.GetFormString("ddlmethod"))
                {
                    _content += "收款方式ID：" + model.rpd_method + "→<font color='red'>" + DTRequest.GetFormString("ddlmethod") + "</font><br/>";
                } 
                model.rpd_method = Utils.StrToInt(DTRequest.GetFormString("ddlmethod"), 0);
                if (model.rpd_content != DTRequest.GetFormString("txtContent"))
                {
                    _content += "收款内容：" + model.rpd_content + "→<font color='red'>" + DTRequest.GetFormString("txtContent") + "</font><br/>";
                }
                model.rpd_content = DTRequest.GetFormString("txtContent");
                if (model.rpd_cbid != Utils.StrToInt(DTRequest.GetFormString("hBankId"), 0))
                {
                    _content += "客户银行账号：" + model.rpd_cbid + "→<font color='red'>" + DTRequest.GetFormString("hBankId") + "</font><br/>";
                }
                model.rpd_cbid = Utils.StrToInt(DTRequest.GetFormString("hBankId"), 0);
                result = bll.Update(model, _content, adminModel, updateMoney);                
            }
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0,\"fromOrder\":"+ fromOrder + ", \"oID\":\"" + oID + "\",\"id\":" + rpdid + " }");
                context.Response.End();
            }
            else
            {
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1,\"fromOrder\":" + fromOrder + ",\"oID\":\"" + oID + "\", \"id\":" + rpdid + " }");
                context.Response.End();
            }
        }
        #endregion
        #region 获取所有业务性质
        private void getAllNature(HttpContext context)
        {
            Model.manager manager = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.businessNature bll = new BLL.businessNature();
            string sqlwhere = "na_isUse=1";
            if (!new BLL.permission().checkHasPermission(manager, "0401"))
            {
                sqlwhere += " and na_flag=0";
            }
            DataSet ds = bll.GetNameList("na_id as id,na_name as name,na_type as type", sqlwhere, "na_sort asc,na_id desc");
            if (ds != null && ds.Tables.Count > 0)
            {
                context.Response.Write(JArray.FromObject(ds.Tables[0]));
                context.Response.End();
            }
            context.Response.Write("[]");
            context.Response.End();
        }
        #endregion
        #region 获取所有业务明细
        private void getAllDetail(HttpContext context)
        {
            int naid = DTRequest.GetInt("naid",0);
            BLL.businessDetails bll = new BLL.businessDetails();
            DataSet ds = bll.GetNameList("de_id as id,de_name as name", "de_nid=" + naid + "", "de_sort asc,de_id desc");
            if (ds != null && ds.Tables.Count > 0)
            {
                context.Response.Write(JArray.FromObject(ds.Tables[0]));
                context.Response.End();
            }
            context.Response.Write("[]");
            context.Response.End();
        }
        #endregion
        #region 检查客户名称是否存在
        private void check_CustomerName(HttpContext context)
        {
            string customerName = DTRequest.GetQueryString("name");
            int id = DTRequest.GetQueryInt("id");
            //如果为Null，退出
            if (string.IsNullOrEmpty(customerName))
            {
                context.Response.Write("{ \"info\":\"客户名称不可为空\", \"status\":\"n\" }");
                return;
            }
            BLL.Customer bll = new BLL.Customer();
            //查询数据库
            if (!bll.Exists(customerName, id))
            {
                context.Response.Write("{ \"info\":\"该客户名称可用\", \"status\":\"y\" }");
                return;
            }
            context.Response.Write("{ \"info\":\"该客户名称已存在\", \"status\":\"n\" }");
            return;
        }
        #endregion
        #region 获取次要联系人
        private void getSubContacts(HttpContext context)
        {
            int cid = DTRequest.GetQueryInt("cid");
            DataSet ds = new BLL.Contacts().GetList(0, "co_cid='" + cid + "'", "");
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    sb.Append("<li>" + dr["co_name"] + "," + dr["co_number"] + "</li>");
                }

            }
            else
            {
                sb.Append("<li>暂无联系人</li>");
            }
            sb.Append("</ul>");
            context.Response.Write(sb.ToString());
            context.Response.End();
        }
        #endregion
        #region 客户审批
        private void check_CustomerStatus(HttpContext context)
        {
            string ids = DTRequest.GetFormString("ids");
            string status = DTRequest.GetFormString("status");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.Customer bll = new BLL.Customer();
            string[] idlist = ids.Split(',');
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            string reason = "";
            foreach (string id in idlist)
            {
                reason = bll.checkStatus(Utils.ObjToInt(id,0), Utils.ObjToByte(status), adminModel);
                if (string.IsNullOrEmpty(reason))
                {
                    success++;
                }
                else
                {
                    error++;
                    sb.Append(reason + "<br/>");
                }
            }
            context.Response.Write("{ \"msg\":\"共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString() + "\", \"status\":" + (error > 0 ? "1" : "0") + "  }");
            context.Response.End();
        }
        #endregion
        #region 根据客户ID获取所有联系人
        private void get_ContactsByCid(HttpContext context)
        {
            int cid = DTRequest.GetQueryInt("cid");
            BLL.Contacts bll = new BLL.Contacts();
            DataSet ds = new BLL.Contacts().GetList(0, "co_cid=" + cid + "", "co_flag desc,co_id asc");
            if (ds != null && ds.Tables.Count > 0)
            {
                context.Response.Write(JArray.FromObject(ds.Tables[0]));
                context.Response.End();
            }
            context.Response.Write("[]");
            context.Response.End();
        }
        #endregion
        #region 根据联系人ID获取电话
        private void get_ContactPhone(HttpContext context)
        {
            int contactID = DTRequest.GetQueryInt("contactID");
            BLL.Contacts bll = new BLL.Contacts();
            Model.Contacts model = bll.GetModel(contactID);
            if (model != null )
            {
                context.Response.Write(model.co_number);
                context.Response.End();
            }
            context.Response.Write("");
            context.Response.End();
        }
        #endregion
        #region 获取所有客户
        private void get_AllCustomer(HttpContext context)
        {
            BLL.Customer bll = new BLL.Customer();
            DataSet ds = bll.GetNameList("c_id as id,c_name as name,(case when c_type=1 then '客户' else case when c_type=2 then '管理用客户' else case when c_type=3 then '内部客户' else case when c_type=4 then '供应商' else '客户兼供应商' end end end end) as type", "c_id > 0 and c_isUse=1", "c_isUse desc,c_addDate desc,c_id desc");
            if (ds != null && ds.Tables.Count > 0)
            {
                context.Response.Write(JArray.FromObject(ds.Tables[0]));
                context.Response.End();
            }
            context.Response.Write("[]");
            context.Response.End();
        }
        #endregion
        #region 更新客户所属人
        private void update_CustomerOwner(HttpContext context)
        {
            string ids = DTRequest.GetFormString("ids");
            string owner = DTRequest.GetFormString("owner");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.Customer bll = new BLL.Customer();
            string[] idlist = ids.Split(',');
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            string reason = "";
            foreach (string id in idlist)
            {
                reason = bll.updateOwner(Convert.ToInt32(id), owner, adminModel);
                if (string.IsNullOrEmpty(reason))
                {
                    success++;
                }
                else
                {
                    error++;
                    sb.Append(reason + "<br/>");
                }
            }
            context.Response.Write("{ \"msg\":\"共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString() + "\", \"status\":" + (error > 0 ? "1" : "0") + "  }");
            context.Response.End();
        }
        #endregion
        #region 合并客户
        private void merge_Customer(HttpContext context)
        {
            int cid1 = DTRequest.GetFormInt("cid1");
            int cid2 = DTRequest.GetFormInt("cid2");
            string cname1 = DTRequest.GetFormString("cname1");
            string cname2 = DTRequest.GetFormString("cname2");
            BLL.Customer bll = new BLL.Customer();
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            string result = bll.mergeCustomer(cid1, cname1, cid2, cname2, adminModel);
            if (!string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0 }");
            context.Response.End();
        }
        #endregion
        #region 计算金额表达式
        private void compute_Result(HttpContext context)
        {
            string expression = DTRequest.GetQueryString("expression");
            if (!string.IsNullOrEmpty(expression))
            {
                object o = new DataTable().Compute(expression, "false");
                decimal money = 0;
                if (decimal.TryParse(o.ToString(), out money))
                {
                    context.Response.Write("{ \"msg\":\"" + money + "\", \"status\":1 }");
                    context.Response.End();
                }
            }
            context.Response.Write("{ \"msg\":\"0\", \"status\":0 }");
            context.Response.End();
        }
        #endregion
        #region 根据业务性质获取业务明细
        private void get_BusinessDetail(HttpContext context)
        {
            int naid = DTRequest.GetFormInt("id", 0);
            if (naid != 0)
            {
                Model.businessNature model = new BLL.businessNature().GetModel(naid);
                if (model != null)
                {
                    if (!model.na_type.Value)
                    {
                        DataSet ds = new BLL.businessDetails().GetList(0, " de_nid =" + naid + " and de_isUse=1 ", "de_sort asc,de_id desc");
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            context.Response.Write(JArray.FromObject(ds.Tables[0]));
                            context.Response.End();
                        }
                    }
                    else
                    {
                        context.Response.Write("[{\"type\":1}]");
                        context.Response.End();
                    }
                }                
            }
            context.Response.Write("[]");
            context.Response.End();
        }
        #endregion
        #region 添加修改应收付记录
        private void save_Finance(HttpContext context)
        {
            string fromOrder = DTRequest.GetFormString("fromOrder");
            string actionType = DTRequest.GetFormString("actionType");
            string oID = DTRequest.GetFormString("orderID");
            bool type = DTRequest.GetFormString("finType") == "True" ? true : false;
            int id = DTRequest.GetFormInt("finID", 0);
            int cid = DTRequest.GetFormInt("hCusId",0);
            string cusName = DTRequest.GetFormString("txtCusName");
            int naid = DTRequest.GetFormInt("ddlnature",0);
            string detail = DTRequest.GetFormString("ddldetail");
            string employee2 = DTRequest.GetFormString("hide_employee2");
            string Illustration = DTRequest.GetFormString("txtIllustration");
            string Expression = DTRequest.GetFormString("txtExpression");
            //string sdate = DTRequest.GetFormString("txtsDate");
            //string edate = DTRequest.GetFormString("txteDate");
            Model.manager manager = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.finance bll = new BLL.finance();
            Model.finance model = new Model.finance();
            string typeText = "应收";
            if (!type)
            {
                typeText = "应付";
            }
            string result = string.Empty;
            if (actionType == DTEnums.ActionEnum.Add.ToString())
            {
                #region 添加
                model.fin_oid = oID;
                model.fin_type = type;
                model.fin_cid = cid;
                model.fin_nature = naid;
                Model.businessNature na = new BLL.businessNature().GetModel(naid);
                if (na != null)
                {
                    if (!na.na_type.Value)
                    {
                        model.fin_detail = detail;
                    }
                    else
                    {
                        model.fin_detail = employee2;
                    }
                }
                //model.fin_sdate = ConvertHelper.toDate(sdate);
                //model.fin_edate = ConvertHelper.toDate(edate);
                model.fin_illustration = Illustration.Trim();
                model.fin_expression = Expression.Trim();
                model.fin_money = Convert.ToDecimal(new DataTable().Compute(model.fin_expression, "false"));
                model.fin_flag = 0;
                //model.fin_area = manager.area;
                model.fin_personNum = manager.user_name;
                model.fin_personName = manager.real_name;
                model.fin_adddate = DateTime.Now;
                result = bll.Add(model, manager);
                #endregion
            }
            else
            {
                #region 编辑
                DataSet ds = bll.GetList(0, "fin_id=" + id + "", "");
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    context.Response.Write("{ \"msg\":\"数据不存在\", \"status\":1 , \"fromorder\":\"" + fromOrder + "\" , \"type\":\"" + type + "\"}");
                    context.Response.End();
                }
                DataRow dr = ds.Tables[0].Rows[0];
                
                model.fin_id = id;
                model.fin_oid = oID;
                model.fin_type = Convert.ToBoolean(dr["fin_type"]);

                string _content = string.Empty;
                if (dr["fin_cid"].ToString() != cid.ToString())
                {
                    _content += "" + typeText + "对象：" + dr["c_name"] + "→<font color='red'>" + cusName + "</font><br/>";
                }
                model.fin_cid = cid;
                model.fin_markNum = dr["fin_markNum"].ToString();
                model.fin_nature = Convert.ToInt32(dr["fin_nature"].ToString());
                Model.businessNature na = new BLL.businessNature().GetModel(model.fin_nature.Value);
                string newDetail = string.Empty;
                if (na != null)
                {
                    if (!na.na_type.Value)
                    {
                        newDetail = detail;
                    }
                    else
                    {
                        newDetail = employee2;
                    }
                }
                if (dr["fin_detail"].ToString() != newDetail)
                {
                    _content += "业务明细：" + dr["fin_detail"] + "→<font color='red'>" + newDetail + "</font><br/>";
                }
                model.fin_detail = newDetail;
                //if (Convert.ToDateTime(dr["fin_sdate"].ToString()).ToString("yyyy-MM-dd") != sdate)
                //{
                //    _content += "业务开始日期：" + Convert.ToDateTime(dr["fin_sdate"].ToString()).ToString("yyyy-MM-dd") + "→<font color='red'>" + sdate + "</font><br/>";
                //}
                //model.fin_sdate = Convert.ToDateTime(sdate);
                //if (Convert.ToDateTime(dr["fin_edate"].ToString()).ToString("yyyy-MM-dd") != edate)
                //{
                //    _content += "业务结束日期：" + Convert.ToDateTime(dr["fin_edate"].ToString()).ToString("yyyy-MM-dd") + "→<font color='red'>" + edate + "</font><br/>";
                //}
                //model.fin_edate = Convert.ToDateTime(edate);
                if (dr["fin_illustration"].ToString() != Illustration)
                {
                    _content += "业务说明：" + dr["fin_illustration"] + "→<font color='red'>" + Illustration + "</font><br/>";
                }
                model.fin_illustration = Illustration;
                if (dr["fin_expression"].ToString() != Expression)
                {
                    model.fin_money = Convert.ToDecimal(new DataTable().Compute(Expression.Trim(), "false"));
                    _content += "金额表达式：" + dr["fin_expression"] + "=" + dr["fin_money"] + "→<font color='red'>" + Expression.Trim() + "=" + model.fin_money + "</font><br/>";
                }
                model.fin_expression = Expression.Trim();
                model.fin_month = dr["fin_month"].ToString();
                model.fin_flag = Utils.ObjToByte(dr["fin_flag"]);
                model.fin_area = dr["fin_area"].ToString();
                model.fin_personNum = dr["fin_personNum"].ToString();
                model.fin_personName = dr["fin_personName"].ToString();
                model.fin_adddate = ConvertHelper.toDate(dr["fin_adddate"]);
                model.fin_remark = dr["fin_remark"].ToString();

                result = bll.Update(model, _content, manager);
                #endregion
            }
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0 , \"fromorder\":\"" + fromOrder + "\" , \"type\":\"" + type + "\"}");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 , \"fromorder\":\"" + fromOrder + "\" , \"type\":\"" + type + "\"}");
            context.Response.End();
        }
        #endregion                
        #region 批量添加应收付记录
        private void Add_Finance(HttpContext context)
        {
            string oID = DTRequest.GetFormString("oID");
            string json = DTRequest.GetFormString("json");
            string type = DTRequest.GetFormString("type");
            int cid = DTRequest.GetFormInt("cid", 0);
            if (string.IsNullOrEmpty(oID))
            {
                context.Response.Write("{ \"msg\":\"数据异常\", \"status\":1 }");
                context.Response.End();
            }
            if (string.IsNullOrEmpty(type))
            {
                context.Response.Write("{ \"msg\":\"请选择应收付类别\", \"status\":1 }");
                context.Response.End();
            }
            if (cid == 0)
            {
                context.Response.Write("{ \"msg\":\"请选择应收付对象\", \"status\":1 }");
                context.Response.End();
            }
            JObject ja = JObject.Parse(json);
            if (ja.Count == 0)
            {
                context.Response.Write("{ \"msg\":\"至少要添加一行数据\", \"status\":1 }");
                context.Response.End();
            }
            Model.manager manager = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.finance bll = new BLL.finance();
            Model.finance model = new Model.finance();
            string result = string.Empty;
            model.fin_oid = oID;
            model.fin_type = type == "1" ? true : false;
            model.fin_cid = cid;
            model.fin_nature = Utils.ObjToInt(ja["nature"], 0);
            model.fin_detail = Utils.ObjectToStr(ja["detail"]);
            //model.fin_sdate = ConvertHelper.toDate(ja["sdate"]);
            //model.fin_edate = ConvertHelper.toDate(ja["edate"]);
            model.fin_illustration = Utils.ObjectToStr(ja["illustration"]);
            model.fin_expression = Utils.ObjectToStr(ja["expression"]);
            model.fin_money = 0;
            if (!string.IsNullOrEmpty(model.fin_expression))
            {
                model.fin_money = Convert.ToDecimal(new DataTable().Compute(model.fin_expression, "false"));
            }
            model.fin_flag = 0;
            //model.fin_area = manager.area;
            model.fin_personNum = manager.user_name;
            model.fin_personName = manager.real_name;
            model.fin_adddate = DateTime.Now;
            if (model.fin_nature == 0)
            {
                context.Response.Write("{ \"msg\":\"业务性质为空\", \"status\":1 }");
                context.Response.End();
            }
            if (string.IsNullOrEmpty(model.fin_detail))
            {
                context.Response.Write("{ \"msg\":\"业务明细为空\", \"status\":1 }");
                context.Response.End();
            }
            //if (model.fin_sdate == null)
            //{
            //    context.Response.Write("{ \"msg\":\"业务开始日期为空\", \"status\":1 }");
            //    context.Response.End();
            //}
            //if (model.fin_edate == null)
            //{
            //    context.Response.Write("{ \"msg\":\"业务结束日期为空\", \"status\":1 }");
            //    context.Response.End();
            //}
            if (string.IsNullOrEmpty(model.fin_expression))
            {
                context.Response.Write("{ \"msg\":\"金额表达式为空\", \"status\":1 }");
                context.Response.End();
            }
            result = bll.Add(model, manager);
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"成功\", \"status\":0 }");
                context.Response.End();
            }
            else
            {
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 }");
                context.Response.End();
            }
        }
        #endregion
        #region 添加合作分成
        private void AddShareFinance(HttpContext context)
        {
            string oID = DTRequest.GetFormString("oID");
            bool type = Utils.StrToBool(DTRequest.GetFormString("type"),false);
            int cid = 71;
            int naid = 8;
            string detail = "内部分成";
            string area = DTRequest.GetFormString("area");
            string Illustration = "业绩分成(" + new BLL.department().getAreaText(area) + ")";
            string expression = DTRequest.GetFormString("expression");
            string sdate = DTRequest.GetFormString("sdate");
            string edate = DTRequest.GetFormString("edate");
            Model.manager manager = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.finance bll = new BLL.finance();
            Model.finance model = new Model.finance();
            model.fin_oid = oID;
            model.fin_type = type;
            model.fin_cid = cid;
            model.fin_nature = naid;
            model.fin_detail = detail;
            model.fin_sdate = ConvertHelper.toDate(sdate);
            model.fin_edate = ConvertHelper.toDate(edate);
            model.fin_illustration = Illustration.Trim();
            model.fin_expression = expression.Trim();
            model.fin_money = Convert.ToDecimal(new DataTable().Compute(model.fin_expression, "false"));
            model.fin_flag = 2;
            model.fin_area = area;
            model.fin_personNum = manager.user_name;
            model.fin_personName = manager.real_name;
            model.fin_adddate = DateTime.Now;
            string result = bll.Add(model, manager);
            if (!string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\""+ result + "\", \"status\":1 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"成功\", \"status\":0 }");
            context.Response.End();


        }
        #endregion
        #region 审批应收付记录
        private void check_finance(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id",0);
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.finance bll = new BLL.finance();

            Model.finance model = bll.GetModel(id);
            if (model == null)
            {
                context.Response.Write("{ \"msg\":\"数据不存在\", \"status\":1 }");
                context.Response.End();
            }
            byte? status = 0;
            if (model.fin_flag == 0)
            {
                status = 2;
            }
            else if (model.fin_flag == 2)
            {
                status = 1;
            }
            string result = bll.checkStatus(id, status, "", adminModel);
            if (!string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + status + "\", \"status\":0 }");
            context.Response.End();
        }
        #endregion                
        #region 批量审批应收付记录
        private void check_financeStatus(HttpContext context)
        {
            string ids = DTRequest.GetFormString("ids");
            string status = DTRequest.GetFormString("status");
            string remark = DTRequest.GetFormString("remark");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.finance bll = new BLL.finance();
            string[] idlist = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            string reason = "";
            foreach (string id in idlist)
            {
                reason = bll.checkStatus(Utils.ObjToInt(id,0), Utils.ObjToByte(status), remark, adminModel);
                if (string.IsNullOrEmpty(reason))
                {
                    success++;
                }
                else
                {
                    error++;
                    sb.Append(reason + "<br/>");
                }
            }
            context.Response.Write("{ \"msg\":\"共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString() + "\", \"status\":" + (error > 0 ? "1" : "0") + "  }");
            context.Response.End();
            
        }
        #endregion        
        #region 删除应收付记录
        private void mutildelete_finance(HttpContext context)
        {
            string ids = DTRequest.GetFormString("ids");
            string status = DTRequest.GetFormString("status");
            string remark = DTRequest.GetFormString("remark");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.finance bll = new BLL.finance();
            string[] idlist = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            string reason = "";
            foreach (string id in idlist)
            {
                reason = bll.Delete(Convert.ToInt32(id),adminModel);
                if (string.IsNullOrEmpty(reason))
                {
                    success++;
                }
                else
                {
                    error++;
                    sb.Append(reason + "<br/>");
                }
            }
            context.Response.Write("{ \"msg\":\"共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString() + "\", \"status\":" + (error > 0 ? "1" : "0") + "  }");
            context.Response.End();     
        }
        #endregion
        #region 插入财务备注
        private void insert_FinRemark(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id",0);
            string remark = DTRequest.GetFormString("remark");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.finance bll = new BLL.finance();
            Model.finance model = bll.GetModel(id);
            if (model == null)
            {
                context.Response.Write("{ \"msg\":\"数据不存在\", \"status\":1 }");
                context.Response.End();
            }
            if (model.fin_remark != remark)
            {
                model.fin_remark = remark;
                bll.Update(model);
            }
        }
        #endregion
        #region 批量删除应收付记录
        private void delete_finance(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.finance bll = new BLL.finance();
            string result = bll.Delete(id, adminModel);
            if (!string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"删除成功\", \"status\":0 }");
            context.Response.End();

        }
        #endregion
        #region 批量审批付款
        private void check_Pay(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id");
            string ctype = DTRequest.GetFormString("ctype");
            string status = DTRequest.GetFormString("status");
            string remark = DTRequest.GetFormString("remark");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            string reason = bll.checkPay(Utils.ObjToInt(id,0), Utils.ObjToByte(ctype), Utils.ObjToByte(status), remark, adminModel);
            if (string.IsNullOrEmpty(reason))
            {
                context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0 }");
                context.Response.End();
            }
            else
            {
                context.Response.Write("{ \"msg\":\"" + reason + "\", \"status\":1 }");
                context.Response.End();
            }
        }
        #endregion        
        #region 确认收付款
        private void confirm_ReceiptPay(HttpContext context)
        {
            //string type = DTRequest.GetQueryString("type");
            //string ids = DTRequest.GetFormString("ids");
            //string status = DTRequest.GetFormString("status");
            //string date = DTRequest.GetFormString("date");
            //string method = DTRequest.GetFormString("method");            
            //Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            //BLL.ReceiptPay bll = new BLL.ReceiptPay();
            //string[] idlist = ids.Split(',');
            //int success = 0, error = 0;
            //StringBuilder sb = new StringBuilder();
            //string reason = "";

            //foreach (string id in idlist)
            //{
            //    reason = bll.confirmReceiptPay(Convert.ToInt32(id), status, date, method, adminModel);
            //    if (string.IsNullOrEmpty(reason))
            //    {
            //        success++;
            //    }
            //    else
            //    {
            //        error++;
            //        sb.Append(reason + "<br/>");
            //    }
            //}
            //context.Response.Write("{ \"msg\":\"共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString() + "\", \"status\":" + (error > 0 ? "1" : "0") + " }");
            //context.Response.End();
            string type = DTRequest.GetQueryString("type");
            int id = DTRequest.GetFormInt("id");
            string status = DTRequest.GetFormString("status");
            string date = DTRequest.GetFormString("date");
            string method = DTRequest.GetFormString("method");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            string reason = bll.confirmReceiptPay(id, status, date, method, adminModel);
            if (string.IsNullOrEmpty(reason))
            {
                context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + reason + "\", \"status\":1 }");
            context.Response.End();
            
            
        }
        #endregion        
        #region 批量审批付款明细
        private void check_PayDetailStatus(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id");
            string ctype = DTRequest.GetFormString("ctype");
            string cstatus = DTRequest.GetFormString("cstatus");
            string remark = DTRequest.GetFormString("remark");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();

            string reason = bll.checkPayDetailStatus(id, Convert.ToByte(ctype), Convert.ToByte(cstatus), remark, adminModel);
            if (string.IsNullOrEmpty(reason))
            {
                context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + reason + "\", \"status\":1 }");
            context.Response.End();
        }
        #endregion        
        #region 获取收付款明细
        private void get_Details(HttpContext context)
        {
            int rpid = DTRequest.GetQueryInt("rpid");
            DataSet ds = new BLL.ReceiptPayDetail().GetList(0, "rpd_rpid=" + rpid + "", "rpd_adddate desc,rpd_id desc");
            if (ds != null && ds.Tables.Count > 0)
            {
                context.Response.Write(JArray.FromObject(ds.Tables[0]));
                context.Response.End();
            }
            context.Response.Write("[]");
            context.Response.End();
        }
        #endregion
        #region 汇总付款明细
        private void collect_Detail(HttpContext context)
        {
            int cid = DTRequest.GetQueryInt("cid");
            int method = DTRequest.GetQueryInt("method");
            int cbid = DTRequest.GetQueryInt("cbid");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息   
            string result = new BLL.ReceiptPayDetail().collectPayDetails(cid, adminModel.area, method, method,cbid, false, DateTime.Now,"","", adminModel);
            if (!string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"info\":\"" + result + "\", \"status\":\"1\" }");
                context.Response.End();
            }
            context.Response.Write("{ \"info\":\"汇总成功\", \"status\":\"0\" }");
            context.Response.End();
        }
        #endregion
        #region 批量汇总付款明细
        private void mutl_Collect(HttpContext context)
        {
            int cid = DTRequest.GetFormInt("cid");
            string ctype = DTRequest.GetFormString("ctype");
            int methodid = DTRequest.GetFormInt("methodid");
            string method = DTRequest.GetFormString("method");
            int cbid = DTRequest.GetFormInt("cbid");
            string sdate = DTRequest.GetFormString("sdate");
            string edate = DTRequest.GetFormString("edate");
            int newmethodid = DTRequest.GetFormInt("newmethodid");
            string newmethod = DTRequest.GetFormString("newmethod");
            string date = DTRequest.GetFormString("date");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息 
            bool isConfirm = false;
            DateTime _date = DateTime.Now;            
            if (ctype == "3")
            {
                isConfirm = true;
                if (string.IsNullOrEmpty(date))
                {
                    context.Response.Write("{ \"info\":请选择实付日期\", \"status\":\"1\" }");
                    context.Response.End();
                }
                _date = ConvertHelper.toDate(date).Value;
            }
            string result = new BLL.ReceiptPayDetail().collectPayDetails(cid, adminModel.area, methodid, newmethodid, cbid, isConfirm, _date, sdate, edate, adminModel);
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"成功\", \"status\":0 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 }");
            context.Response.End();
        }
        #endregion
        #region 取消汇总付款明细
        private void cancelCollect(HttpContext context)
        {
            int rpdid = DTRequest.GetFormInt("rpdid");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息   
            string result = new BLL.ReceiptPayDetail().cancelCollect(rpdid,adminModel);
            if (!string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"info\":\"" + result + "\", \"status\":\"1\" }");
                context.Response.End();
            }
            context.Response.Write("{ \"info\":\"取消汇总成功\", \"status\":\"0\" }");
            context.Response.End();
        }
        #endregion
        #region 批量取消汇总付款明细
        private void mutlCancel_Collect(HttpContext context)
        {
            string ids = DTRequest.GetFormString("ids");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息 
            string[] idlist = ids.Split(',');
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            string reason = "";
            foreach (string id in idlist)
            {
                string[] list = id.Split('|');
                reason = new BLL.ReceiptPayDetail().cancelCollect(Convert.ToInt32(list[0]), adminModel);
                if (string.IsNullOrEmpty(reason))
                {
                    success++;
                }
                else
                {
                    error++;
                    sb.Append(reason + "<br/>");
                }
            }
            context.Response.Write("{ \"info\":\"共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString() + "\", \"status\":\"" + (error > 0 ? "1" : "0") + "\" }");
            context.Response.End();
        }
        #endregion
        #region 填充付款方式
        private void set_PayMethod(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id");
            int method = DTRequest.GetFormInt("method");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            Model.ReceiptPayDetail model = bll.GetModel(id);
            if (method == 0)
            {
                context.Response.Write("{ \"msg\":\"请选择付款方式\", \"status\":1 }");
                context.Response.End();
            }
            if (model.rpd_method == method)
            {
                context.Response.Write("{ \"msg\":\"付款方式未改变\", \"status\":1 }");
                context.Response.End();
            }
            string content = "更新付款方式：" + model.rpd_method + "→<font color='red'>" + method + "</font><br/>";
            model.rpd_method = method;
            string result = bll.Update(model, content, adminModel,false,true);
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"成功\", \"status\":0 }");
                context.Response.End();
            }           
            context.Response.Write("{ \"msg\":\""+ result + "\", \"status\":1 }");
            context.Response.End();

        }
        #endregion
        #region 填充付款方式
        private void setPayMethod_mutli(HttpContext context)
        {
            int cid = DTRequest.GetFormInt("cid");
            int methodid = DTRequest.GetFormInt("methodid");
            string method = DTRequest.GetFormString("method");
            string sdate = DTRequest.GetFormString("sdate");
            string edate = DTRequest.GetFormString("edate");
            int newmethodid = DTRequest.GetFormInt("newmethodid");
            string newmethod = DTRequest.GetFormString("newmethod");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            //Model.ReceiptPayDetail model = bll.GetModel(cid);
            
            string result = bll.mutliUpdateMethod(cid, newmethodid, methodid, sdate, edate, adminModel);
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"成功\", \"status\":0 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 }");
            context.Response.End();

        }
        #endregion
        #region 删除收付款明细
        private void delete_ReceiptPayDetail(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            string result = bll.Delete(id, adminModel);
            if (!string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 }");
                context.Response.End();
            }
            //删除文件
            if (Directory.Exists(context.Server.MapPath("~/uploadPay/1/" + id + "/")))
            {
                Directory.Delete(context.Server.MapPath("~/uploadPay/1/" + id + "/"), true);
            }
            context.Response.Write("{ \"msg\":\"删除成功\", \"status\":0 }");
            context.Response.End();

        }
        #endregion
        #region 计算发票余额
        private void compute_InvMoney(HttpContext context)
        {
            int cid = DTRequest.GetFormInt("cid",0);
            decimal money = DTRequest.GetFormDecimal("money",0);
            string oID = DTRequest.GetFormString("oID");            
            BLL.invoices bll = new BLL.invoices();
            decimal? leftMoney = bll.computeInvoiceLeftMoney(oID, cid);
            context.Response.Write("{ \"msg\":\"" + (leftMoney - money >= 0 ? 0 : leftMoney - money) + "\", \"status\":0 }");
            context.Response.End();

        }
        #endregion
        #region 删除发票
        private void delete_invoice(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.invoices bll = new BLL.invoices();
            string result = bll.Delete(id, adminModel);
            if (!string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"删除成功\", \"status\":0 }");
            context.Response.End();

        }
        #endregion        
        #region 批量审批发票申请
        private void check_Invoice(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id");
            string ctype = DTRequest.GetFormString("ctype");
            string cstatus = DTRequest.GetFormString("cstatus");
            string remark = DTRequest.GetFormString("remark");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.invoices bll = new BLL.invoices();
            string reason = bll.checkInvoiceStatus(Utils.ObjToInt(id,0), Utils.ObjToByte(ctype), Utils.ObjToByte(cstatus), remark, adminModel);
            if (string.IsNullOrEmpty(reason))
            {
                context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + reason + "\", \"status\":1 }");
            context.Response.End();
        }
        #endregion        
        #region 批量开票
        private void confirm_Invoice(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id");
            string status = DTRequest.GetFormString("status");
            string date = DTRequest.GetFormString("date");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.invoices bll = new BLL.invoices();
            string reason = bll.confirmInvoice(Convert.ToInt32(id), Convert.ToBoolean(status), date, adminModel);
            if (string.IsNullOrEmpty(reason))
            {
                context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + reason + "\", \"status\":1 }");
            context.Response.End();
        }
        #endregion        
        #region 批量审批凭证
        private void check_CertificateStatus(HttpContext context)
        {
            string ids = DTRequest.GetFormString("ids");
            string status = DTRequest.GetFormString("status");
            string remark = DTRequest.GetFormString("remark");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.certificates bll = new BLL.certificates();
            string[] idlist = ids.Split(',');
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            string reason = "";
            foreach (string id in idlist)
            {
                reason = bll.checkCertificate(Utils.ObjToInt(id,0), Utils.ObjToByte(status), remark, adminModel.user_name, adminModel.real_name);
                if (string.IsNullOrEmpty(reason))
                {
                    success++;
                }
                else
                {
                    error++;
                    sb.Append(reason + "<br/>");
                }
            }
            context.Response.Write("{ \"msg\":\"共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString() + "\", \"status\":" + (error > 0 ? "1" : "0") + " }");
            context.Response.End();
        }
        #endregion        
        #region 获取所有凭证
        private void get_AllCertificate(HttpContext context)
        {
            BLL.certificates bll = new BLL.certificates();
            DataSet ds = bll.GetNumList("CONVERT(varchar(100),ce_date, 23) as id,ce_num as name", "ce_id>0", "ce_addDate desc,ce_id desc");
            if (ds != null && ds.Tables.Count > 0)
            {
                context.Response.Write(JArray.FromObject(ds.Tables[0]));
                context.Response.End();
            }
            context.Response.Write("[]");
            context.Response.End();
        }
        #endregion
        #region 标记凭证
        private void sign_Certificate(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id");
            string num = DTRequest.GetFormString("num");
            string date = DTRequest.GetFormString("date");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            string reason = bll.signCertificate(id, num, date, adminModel);
            if (string.IsNullOrEmpty(reason))
            {
                context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + reason + "\", \"status\":1 }");
            context.Response.End();
        }
        #endregion        
        #region 取消标记凭证
        private void unsign_Certificate(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.ReceiptPay bll = new BLL.ReceiptPay();
            string reason = bll.cancelSignCertificate(id, adminModel);
            if (string.IsNullOrEmpty(reason))
            {
                context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + reason + "\", \"status\":1 }");
            context.Response.End();
        }
        #endregion        
        #region 新增凭证
        private void add_Certificate(HttpContext context)
        {
            string num = DTRequest.GetFormString("num");
            string date = DTRequest.GetFormString("date");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            Model.certificates model = new Model.certificates();
            model.ce_num = num;
            model.ce_date = ConvertHelper.toDate(date);
            model.ce_personNum = adminModel.user_name;
            model.ce_personName = adminModel.real_name;
            int id = 0;
            string result = new BLL.certificates().Add(model, out id);
            if (id > 0)
            {
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\""+ result + "\", \"status\":1 }");
            context.Response.End();
        }
        #endregion
        #region 添加内部客户
        private void add_InnerCustomer(HttpContext context)
        {
            string list = DTRequest.GetFormString("list");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            string[] idlist = list.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            string reason = "";
            foreach (string item in idlist)
            {
                string[] array = item.Split('|');
                reason = new BLL.Customer().AddInnerCustomer(array[1],adminModel);
                if (string.IsNullOrEmpty(reason))
                {
                    success++;
                    sb.Append(array[0] + "：成功<br/>");
                }
                else
                {
                    error++;
                    sb.Append(array[0] + "："+ reason + "<br/>");
                }
            }
            context.Response.Write("{ \"msg\":\"共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString() + "\", \"status\":" + (error > 0 ? "1" : "0") + " }");
            context.Response.End();
        }
        #endregion
        #region 下单订单结算汇总
        private void load_OrderExcel(HttpContext context)
        {
            string oid = DTRequest.GetFormString("oid");
            BLL.finance bll = new BLL.finance();
            DataTable dt = bll.GetList(0, "fin_oid='" + oid + "'", "fin_type desc,fin_adddate").Tables[0];
            string exportTemplatePath = "~/admin/TempletExcel/订单结算汇总.xlsx";
            string download = GetPathByDataTableToExcel(dt, exportTemplatePath);
            context.Response.Write("{ \"msg\":\"" + download + "\", \"status\":0 }");
            context.Response.End();
        }

        /// <summary>
        /// DataTable填充Excel
        /// 存储excel
        /// 返回excel下载路径
        /// </summary>
        /// <param name="sourceTable">数据源</param>
        /// <param name="exportTemplatePath">模板路径</param>
        /// <returns>下载路径</returns>
        public string GetPathByDataTableToExcel(DataTable sourceTable, string exportTemplatePath)
        {
            /// ********************************需要引入NPOI组件*********************************************

            HSSFWorkbook workbook = null;
            MemoryStream ms = null;
            ISheet sheet = null;
            string templetFileName = HttpContext.Current.Server.MapPath(exportTemplatePath);
            FileStream file = new FileStream(templetFileName, FileMode.Open, FileAccess.Read);
            workbook = new HSSFWorkbook(file);
            string httpurl = "";
            try
            {
                sourceTable.Columns.RemoveAt(1);
                sourceTable.Columns.RemoveAt(3);
                ms = new MemoryStream();
                sheet = workbook.GetSheetAt(0);  //第一个Sheet页面
                int rowIndex = 1;  //行索引，  0为第一行，1为第二行

                decimal? total1 = 0, total2 = 0, total3 = 0;
                //遍历DataTable 填充所有数据
                foreach (DataRow row in sourceTable.Rows)
                {
                    HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in sourceTable.Columns)
                    {
                        dataRow.CreateCell(0).SetCellValue(row["fin_id"].ToString());
                        dataRow.CreateCell(1).SetCellValue(row["c_name"].ToString());
                        dataRow.CreateCell(2).SetCellValue(row["fin_type"].ToString()=="True"?"应收":"应付");
                        dataRow.CreateCell(3).SetCellValue(row["na_name"].ToString() + "/" + row["fin_detail"].ToString());
                        dataRow.CreateCell(4).SetCellValue(ConvertHelper.toDate(row["fin_sdate"]).Value.ToString("yyyy-MM-dd") + "/" + ConvertHelper.toDate(row["fin_edate"]).Value.ToString("yyyy-MM-dd"));
                        dataRow.CreateCell(5).SetCellValue(row["fin_illustration"].ToString());
                        dataRow.CreateCell(6).SetCellValue(row["fin_expression"].ToString());
                        dataRow.CreateCell(7).SetCellValue(row["fin_money"].ToString());
                        dataRow.CreateCell(8).SetCellValue(BusinessDict.checkStatus()[Utils.ObjToByte(row["fin_flag"])]);
                        dataRow.CreateCell(9).SetCellValue(row["fin_month"].ToString());
                        dataRow.CreateCell(10).SetCellValue(row["fin_remark"].ToString());
                    }

                    //total1 += Utils.StrToDecimal(row["finMoney"].ToString(), 0);
                    //total2 += Utils.StrToDecimal(row["rpdMoney"].ToString(), 0);
                    //total3 += Utils.StrToDecimal(row["unReceiptPay"].ToString(), 0);

                    ++rowIndex;
                }
                //HSSFRow lastdataRow = (HSSFRow)sheet.CreateRow(rowIndex);
                //lastdataRow.CreateCell(1).SetCellValue("总计：");
                //lastdataRow.CreateCell(2).SetCellValue(total1.ToString());
                //lastdataRow.CreateCell(3).SetCellValue(total2.ToString());
                //lastdataRow.CreateCell(4).SetCellValue(total3.ToString());

                workbook.Write(ms);
                ms.Flush();
            }
            catch (Exception)
            {

            }
            finally
            {
                ms.Close();
                sheet = null;
                workbook = null;

                //~/Document/TemporaryDocuments/  是项目下相对路径的文件存放地址，也可进行修改

                string tempExcelName = Path.GetFileNameWithoutExtension(templetFileName) + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(templetFileName);
                string tempExcel = "~/Document/" + tempExcelName;

                //文件另存
                System.IO.File.WriteAllBytes(HttpContext.Current.Server.MapPath(tempExcel), ms.GetBuffer());

                //获取项目绝对路径地址
                string url = HttpContext.Current.Request.Url.AbsoluteUri.ToString().Split('/')[0] + "//" + HttpContext.Current.Request.Url.Authority.ToString();


                var virtualPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
                string fileName = "";
                if (virtualPath != "/")
                {
                    //有子应用程序
                    fileName = virtualPath + "/Document/" + tempExcelName;
                }
                else
                {
                    fileName = "/Document/" + tempExcelName;
                }

                //拼接文件相对地址
                //string fileName = "/Document/TemporaryDocuments/" + tempExcelName;

                //返回文件url地址
                httpurl = url + fileName;

                //清除历史文件，避免历史文件越来越多，可进行删除
                DirectoryInfo dyInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Document/"));
                //获取文件夹下所有的文件
                foreach (FileInfo feInfo in dyInfo.GetFiles())
                {
                    //判断文件日期是否小于两天前，是则删除
                    if (feInfo.CreationTime < DateTime.Today.AddDays(-2))
                        feInfo.Delete();
                }
            }

            //返回下载地址
            return httpurl;
        }
        #endregion
        #region 批量对账
        private void add_FinChk(HttpContext context)
        {
            string idstr = DTRequest.GetFormString("idstr");
            string num = DTRequest.GetFormString("num");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            string[] idlist = idstr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            string reason = "";
            foreach (string item in idlist)
            {
                string[] array = item.Split('/');
                reason = new BLL.finance_chk().addFinancechk(Utils.StrToInt(array[0], 0), array[2], num, Utils.StrToDecimal(array[1], 0), adminModel,0,true);
                if (string.IsNullOrEmpty(reason))
                {
                    success++;
                    sb.Append(array[0] + "：成功<br/>");
                }
                else
                {
                    error++;
                    sb.Append(array[0] + "：" + reason + "<br/>");
                }
            }
            context.Response.Write("{ \"msg\":\"共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString() + "\", \"status\":" + (error > 0 ? "1" : "0") + " }");
            context.Response.End();
        }
        #endregion
        #region 批量取消对账
        private void cancel_FinChk(HttpContext context)
        {
            string idstr = DTRequest.GetFormString("idstr");
            string num = DTRequest.GetFormString("num");
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            string[] idlist = idstr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            string reason = "";
            foreach (string item in idlist)
            {
                string[] array = item.Split('-');
                reason = new BLL.finance_chk().delChkByFinID(Utils.StrToInt(array[0], 0), array[1], adminModel);
                if (string.IsNullOrEmpty(reason))
                {
                    success++;
                    sb.Append(array[0] + "：成功<br/>");
                }
                else
                {
                    error++;
                    sb.Append(array[0] + "：" + reason + "<br/>");
                }
            }
            context.Response.Write("{ \"msg\":\"共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString() + "\", \"status\":" + (error > 0 ? "1" : "0") + " }");
            context.Response.End();
        }
        #endregion
        #region 对账
        private void add_Chk(HttpContext context)
        {
            int finid = DTRequest.GetFormInt("finid", 0);
            string oid = DTRequest.GetFormString("oid");
            string num = DTRequest.GetFormString("num");
            decimal? money = DTRequest.GetFormDecimal("money", 0);
            int fcid = DTRequest.GetFormInt("fcid", 0);
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            string result = new BLL.finance_chk().addFinancechk(finid, oid, num, money, adminModel, fcid);
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"对账成功\", \"status\":0 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 }");
            context.Response.End();
        }
        #endregion
        #region 删除对账
        private void del_Chk(HttpContext context)
        {
            int id = DTRequest.GetFormInt("id", 0);
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            string result = new BLL.finance_chk().delFinancechk(id, adminModel);
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"删除成功\", \"status\":0 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 }");
            context.Response.End();
        }
        #endregion
        #region 确定分配
        private void deal_Distribute(HttpContext context)
        {
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            int rpid = DTRequest.GetFormInt("rpid", 0);
            int cid = DTRequest.GetFormInt("cid", 0);
            int cbid = DTRequest.GetFormInt("cbid", 0);
            string oid = DTRequest.GetFormString("oid");
            bool type = DTRequest.GetFormString("type") == "True" ? true : false;
            decimal disMoney = DTRequest.GetFormDecimal("disMoney", 0);
            DateTime foredate = Utils.StrToDateTime(DTRequest.GetFormString("foredate"));
            int method = DTRequest.GetFormInt("method", 0);
            string area = DTRequest.GetFormString("area");
            string chk= DTRequest.GetFormString("chk");
            Model.ReceiptPayDetail model = new Model.ReceiptPayDetail();
            model.rpd_type = type;
            model.rpd_rpid = rpid;
            model.rpd_oid = oid;
            model.rpd_cid = cid;
            model.rpd_cbid = cbid;
            model.rpd_num = chk;
            model.rpd_money = disMoney;
            model.rpd_foredate = foredate;
            model.rpd_method = method;
            model.rpd_personName = adminModel.real_name;
            model.rpd_personNum = adminModel.user_name;
            model.rpd_flag1 = 2;
            model.rpd_flag2 = 2;
            model.rpd_flag3 = 2;
            model.rpd_adddate = DateTime.Now;
            //model.rpd_area = area;

            string result = new BLL.ReceiptPayDetail().AddOrCancleDistribution(model, adminModel);
            decimal distributeMoney = new BLL.ReceiptPay().getDistributeMoney(rpid);//已分配总额
            context.Response.Write("{ \"msg\":\"" + result + "\", \"oid\":\"" + oid + "\",\"money\":\"" + distributeMoney + "\"}");
            context.Response.End();
        }
        #endregion        
        #region 获取已分配金额
        private void get_DistributeMoney(HttpContext context)
        {
            int rpid = DTRequest.GetFormInt("rpid", 0);
            decimal distributeMoney = new BLL.ReceiptPay().getDistributeMoney(rpid);//已分配总额
            context.Response.Write("{ \"msg\":\"" + distributeMoney + "\"}");
            context.Response.End();
        }
        #endregion   
        #region 获取客户银行账号
        private void get_customerBank(HttpContext context)
        {
            int cid = DTRequest.GetFormInt("cid", 0);
            bool selectField = DTRequest.GetFormString("field") == "1" ? true : false;
            BLL.customerBank bll = new BLL.customerBank();
            DataSet ds = bll.GetList(0, "cb_cid=" + cid + " and cb_flag=1", "cb_id asc", selectField);
            if (ds != null && ds.Tables.Count > 0)
            {
                context.Response.Write(JArray.FromObject(ds.Tables[0]));
                context.Response.End();
            }
            context.Response.Write("[]");
            context.Response.End();
        }
        #endregion
        #region 保存银行账号
        private void save_Bank(HttpContext context)
        {
            string actionType = DTRequest.GetFormString("actionType");
            int id = DTRequest.GetFormInt("bID", 0);
            int cid = DTRequest.GetFormInt("cID", 0);
            string bankName = DTRequest.GetFormString("txtBankName");
            string bankNum = DTRequest.GetFormString("txtBankNum");
            string bank = DTRequest.GetFormString("txtBank");
            string bankAddress = DTRequest.GetFormString("txtBankAddress");
            bool isUse = DTRequest.GetFormString("cbIsUse") == "on" ? true : false;
            string fromPay = DTRequest.GetFormString("fromPay");
            Model.manager manager = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            Model.customerBank model = new Model.customerBank();
            BLL.customerBank bll = new BLL.customerBank();
            string result = string.Empty;
            if (actionType == DTEnums.ActionEnum.Add.ToString())
            {
                model.cb_cid = cid;
                model.cb_bankName = bankName.Trim();
                model.cb_bankNum = bankNum.Trim();
                model.cb_bank = bank.Trim();
                model.cb_bankAddress = bankAddress.Trim();
                model.cb_flag = isUse;
                result = bll.Add(model, manager);
            }
            else
            {
                model = bll.GetModel(id);
                string _content = string.Empty;

                if (model.cb_bankName != bankName.Trim())
                {
                    _content += "银行账户名称:" + model.cb_bankName + "→<font color='red'>" + bankName.Trim() + "</font><br/>";
                }
                model.cb_bankName = bankName.Trim();
                if (model.cb_bankNum != bankNum.Trim())
                {
                    _content += "客户银行账号:" + model.cb_bankNum + "→<font color='red'>" + bankNum.Trim() + "</font><br/>";
                }
                model.cb_bankNum = bankNum.Trim();
                if (model.cb_bank != bank.Trim())
                {
                    _content += "开户行:" + model.cb_bank + "→<font color='red'>" + bank.Trim() + "</font><br/>";
                }
                model.cb_bank = bank.Trim();
                if (model.cb_bankAddress != bankAddress.Trim())
                {
                    _content += "开户地址:" + model.cb_bankAddress + "→<font color='red'>" + bankAddress.Trim() + "</font><br/>";
                }
                model.cb_bankAddress = bankAddress.Trim();
                if (model.cb_flag != isUse)
                {
                    _content += "状态:" + (model.cb_flag.Value ? "启用" : "禁用") + "→<font color='red'>" + (isUse ? "启用" : "禁用") + "</font><br/>";
                }
                model.cb_flag = isUse;
                result = bll.Update(model, _content, manager);
            }
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"保存成功\", \"status\":0, \"fromPay\":\"" + fromPay + "\",\"cid\":" + cid + "}");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1, \"fromPay\":\"" + fromPay + "\",\"cid\":" + cid + "}");
            context.Response.End();
        }
        #endregion
        #region 获取对账明细数据
        private void get_ChkDetail(HttpContext context)
        {
            int finid = DTRequest.GetFormInt("finid", 0);
            DataSet ds = new BLL.finance_chk().GetList(0, "fc_finid=" + finid + "", "fc_addDate desc");
            if (ds != null && ds.Tables.Count > 0)
            {
                context.Response.Write(JArray.FromObject(ds.Tables[0]));
                context.Response.End();
            }
            context.Response.Write("[]");
            context.Response.End();
        }
        #endregion
        #region 修改对账标识
        private void change_ChkNum(HttpContext context)
        {
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            int id = DTRequest.GetFormInt("id", 0);
            string num = DTRequest.GetFormString("num");

            string result = new BLL.finance_chk().updateChk(id, num, adminModel);
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"对账成功\", \"status\":0 }");
                context.Response.End();
            }
            context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 }");
            context.Response.End();            
        }
        #endregion        

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否为合法的上传文件
        /// </summary>
        private bool CheckFileExt(string _fileExt)
        {
            //检查危险文件
            string[] excExt = { "asp", "aspx", "ashx", "asa", "asmx", "asax", "php", "jsp", "htm", "html" };
            for (int i = 0; i < excExt.Length; i++)
            {
                if (excExt[i].ToLower() == _fileExt.ToLower())
                {
                    return false;
                }
            }
            //检查合法文件
            string[] allowExt = ("gif,jpg,jpeg,png,bmp, rar, zip, doc, xls, txt, docx, xlsx, pdf, flv,mp3,mp4,avi").Split(',');
            for (int i = 0; i < allowExt.Length; i++)
            {
                if (allowExt[i].Trim().ToLower() == _fileExt.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 检查文件大小是否合法
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        /// <param name="_fileSize">文件大小(B)</param>
        private bool CheckFileSize(string _fileExt, int _fileSize)
        {
            //将视频扩展名转换成ArrayList
            ArrayList lsVideoExt = new ArrayList("flv,mp3,mp4,avi".ToLower().Split(','));
            //判断是否为图片文件
            if (IsImage(_fileExt))
            {
                if (10240 > 0 && _fileSize > 10240 * 1024)
                {
                    return false;
                }
            }
            else if (lsVideoExt.Contains(_fileExt.ToLower()))
            {
                if (102400 > 0 && _fileSize > 102400 * 1024)
                {
                    return false;
                }
            }
            else
            {
                if (51200 > 0 && _fileSize > 51200 * 1024)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 是否为图片文件
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        private bool IsImage(string _fileExt)
        {
            ArrayList al = new ArrayList();
            al.Add("bmp");
            al.Add("jpeg");
            al.Add("jpg");
            al.Add("gif");
            al.Add("png");
            if (al.Contains(_fileExt.ToLower()))
            {
                return true;
            }
            return false;
        }
    }
}