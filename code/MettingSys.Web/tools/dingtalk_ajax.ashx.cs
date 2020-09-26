using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using MettingSys.Common;
using MettingSys.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MettingSys.Web.tools
{
    /// <summary>
    /// dingtalk_ajax 的摘要说明
    /// </summary>
    public class dingtalk_ajax : IHttpHandler, IRequiresSessionState
    {
        Model.sysconfig sysConfig = new BLL.sysconfig().loadConfig();//系统配置信息
        Model.userconfig userConfig = new BLL.userconfig().loadConfig();//会员配置信息
        private JObject jObject = new JObject();
        public void ProcessRequest(HttpContext context)
        {
            //检查管理员是否登录
            //if (!new ManagePage().IsAdminLogin())
            //{
            //    context.Response.Write("{\"status\": 0, \"msg\": \"尚未登录或已超时，请登录后操作！\"}");
            //    return;
            //}
            //取得处事类型
            string action = DTRequest.GetQueryString("action");
            switch (action)
            {
                #region 初始化数据绑定
                case "init_AduitCount":
                    init_AduitCount(context);
                    break;
                case "UpLoadFile": //上传文件
                    UpLoadFile(context);
                    break;
                case "File_delete"://删除文件
                    File_delete(context);
                    break;
                case "init_contractprice": //合同造价数据绑定
                    init_contractprice(context);
                    break;
                case "init_fstatus": //订单状态数据绑定
                    init_fstatus(context);
                    break;
                case "init_dstatus": //接单状态数据绑定
                    init_dstatus(context);
                    break;
                case "init_checkstatus": //审批状态数据绑定
                    init_checkstatus(context);
                    break;
                case "init_lockstatus": //锁单状态数据绑定
                    init_lockstatus(context);
                    break;
                case "init_invoiceconfirmstatus": //开票状态数据绑定
                    init_invoiceconfirmstatus(context);
                    break;
                case "init_pushstatus": //推送上级审批数据绑定
                    init_pushstatus(context);
                    break;
                case "init_area": //活动归属地数据绑定
                    init_area(context);
                    break;
                case "init_allcustomer": //获取所有客户信息
                    init_allcustomer(context);
                    break;
                case "init_contactsbycid": //根据客户ID获取相关主要联系人及联系号码
                    init_contactsbycid(context);
                    break;
                case "init_employeebyarea": //根据活动归属地ID获取组织架构+人员
                    init_employeebyarea(context);
                    break;
                case "init_unbusinessnature": //非业务支付申请支付类别
                    init_unbusinessnature(context);
                    break;
                case "init_unbusinesspayfunction": //非业务支付员工往来支付用途
                    init_unbusinesspayfunction(context);
                    break;
                case "init_nature": //业务性质数据绑定
                    init_nature(context);
                    break;
                case "init_naturedetail": //业务明细数据绑定
                    init_naturedetail(context);
                    break;
                case "init_paymethod": //支付方式数据绑定
                    init_paymethod(context);
                    break;
                case "init_servicetype": //应税劳务数据绑定
                    init_servicetype(context);
                    break;
                case "init_servicename": //服务名称数据绑定
                    init_servicename(context);
                    break;
                case "init_sentmethod": //送票方式数据绑定
                    init_sentmethod(context);
                    break;
                case "init_invoicearea": //开票区域数据绑定
                    init_invoicearea(context);
                    break;
                case "method_data": //支付方式数据绑定
                    method_data(context);
                    break;
                case "permission_check"://检查登录人权限
                    permission_check(context);
                    break;
                case "self_message"://个人未读消息
                    self_message(context);
                    break;
                case "self_messageDetail":
                    self_messageDetail(context);
                    break;
                case "self_messageDel":
                    self_messageDel(context);
                    break;
                case "self_messageRead":
                    self_messageRead(context);
                    break;
                #endregion

                #region 客户管理
                case "customer_list": //客户分页列表
                    customer_list(context);
                    break;
                case "customer_show": //查看客户详情
                    customer_show(context);
                    break;
                case "get_customerById": //获取客户详情
                    get_customerById(context);
                    break;
                case "customer_add": //添加客户详情
                    customer_add(context);
                    break;
                case "customer_edit": //编辑客户详情
                    customer_edit(context);
                    break;
                case "customer_del": //删除客户信息
                    customer_del(context);
                    break;

                case "get_contactById": //获取联系人信息
                    get_contactById(context);
                    break;
                case "contact_add": //添加次要联系人
                    contact_add(context);
                    break;
                case "contact_edit": //编辑主、次要联系人
                    contact_edit(context);
                    break;
                case "contact_del": //删除次要联系人
                    contact_del(context);
                    break;
                case "bank_Edit"://添加编辑银行账号
                    bank_Edit(context);
                    break;
                case "bank_Del"://删除银行账号
                    bank_Del(context);
                    break;
                #endregion

                #region 订单管理
                case "order_list": //查看订单列表
                    order_list(context);
                    break;
                case "order_edit": //新增和修改订单信息
                    order_edit(context);
                    break;
                case "order_show": //查看订单信息
                    order_show(context);
                    break;
                case "order_check"://业务上级审批
                    order_check(context);
                    break;
                case "order_delete": //删除订单
                    order_delete(context);
                    break;
                case "unbusinesspay_list": //非业务支付申请列表
                    unbusinesspay_list(context);
                    break;
                case "unbusinesspay_show": //查看非业务支付申请
                    unbusinesspay_show(context);
                    break;
                case "unbusinesspay_add": //新增非业务支付申请
                    unbusinesspay_add(context);
                    break;
                case "unbusinesspay_edit": //修改业务支付申请
                    unbusinesspay_edit(context);
                    break;
                case "unbusinesspay_auditBind"://绑定非业务支付审批类型
                    unbusinesspay_auditBind(context);
                    break;
                case "unbusinesspay_audit": //业务支付申请审核
                    unbusinesspay_audit(context);
                    break;
                case "unbusinesspay_del":
                    unbusinesspay_del(context);
                    break;
                case "unbusinesspay_confirm_pay": //业务支付申请支付确认
                    unbusinesspay_confirm_pay(context);
                    break;
                case "finance_add": //新增收款通知\付款通知
                    finance_add(context);
                    break;
                case "finance_audit": //审核业务
                    finance_audit(context);
                    break;
                case "invoice_list": //发票申请列表
                    invoice_list(context);
                    break;
                case "invoice_show": //查看发票申请
                    invoice_show(context);
                    break;
                case "invoice_auditBind": //审批发票申请
                    invoice_auditBind(context);
                    break;
                case "invoice_audit": //审批发票申请
                    invoice_audit(context);
                    break;
                case "invoice_confirm": //确认发票申请是否已开票
                    invoice_confirm(context);
                    break;
                case "invoice_add": //新增发票申请
                    invoice_add(context);
                    break;
                case "pay_list": //业务支付审核分页列表
                    pay_list(context);
                    break;
                case "pay_show": //查看业务支付审核
                    pay_show(context);
                    break;
                case "pay_audit": //审批业务支付
                    pay_audit(context);
                    break;
                case "pay_confirm": //确认支付业务支付
                    pay_confirm(context);
                    break;
                case "add_receiptpayDetail"://添加收付款明细
                    add_receiptpayDetail(context);
                    break;
                case "getBank"://获取客户银行账号
                    get_bank(context);
                    break;
                case "getSettlementlist"://获取订单结算汇总数据
                    get_settlementlist(context);
                    break;
                case "getInvoiceList"://获取订单发票申请汇总
                    get_InvoiceList(context);
                    break;
                case "getunBusinessList"://获取执行备用金借款明细
                    get_unBusinessList(context);
                    break;
                case "getunBusinessPic"://获取执行备用金借款明细的附件
                    get_unBusinessPic(context);
                    break;
                #endregion

                #region 财务管理
                case "paydetail_list"://付款明细分页列表
                    paydetail_list(context);
                break;
                case "paydetail_show"://付款明细详细信息
                    paydetail_show(context);
                    break;
                case "paydetail_audit"://审批付款明细
                    paydetail_audit(context);
                    break;
                #endregion

                #region 通知管理
                case "receipt_list"://收款通知分页列表
                    receipt_list(context); break;
                case "payment_list"://付款通知分页列表
                    payment_list(context); break;
                case "receiptpay_add"://添加收付款通知
                    receiptpay_add(context); break;
                
                case "bill_list"://开票通知分页列表
                    bill_list(context); break;

                #endregion

                #region 个人业务结算

                #endregion

                #region 业绩统计
                case "AchievementStatistic":
                    AchievementStatistic(context);
                    break;
                #endregion

                default:
                    {
                        context.Response.Write("{\"status\": 0, \"msg\": \"ActionIsNullOrError\"}");
                        return;
                    }
            }
        }

        #region 数据绑定
       
        /// <summary>
        /// 合同造价
        /// </summary>
        private void init_contractprice(HttpContext context)
        {
            StringBuilder sb = init_dictionary(context, Common.BusinessDict.ContractPriceType());
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
            return;
        }
        
        /// <summary>
        /// 订单状态
        /// </summary>
        private void init_fstatus(HttpContext context)
        {
            get_params(context, out jObject);
            string type = Utils.ObjectToStr(jObject["type"]);
            StringBuilder sb = init_dictionary(context, Common.BusinessDict.fStatus(Utils.ObjToByte(type))); 
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
        }
        
        /// <summary>
        /// 接单状态
        /// </summary>
        private void init_dstatus(HttpContext context)
        {
            StringBuilder sb = init_dictionary(context, Common.BusinessDict.dStatus());
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
        }
        
        /// <summary>
        /// 审批状态
        /// </summary>
        private void init_checkstatus(HttpContext context)
        {
            StringBuilder sb = init_dictionary(context, Common.BusinessDict.checkStatus());
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
        }
        
        /// <summary>
        /// 锁单状态
        /// </summary>
        private void init_lockstatus(HttpContext context)
        {
            StringBuilder sb = init_dictionary(context, Common.BusinessDict.lockStatus());
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
        }

        /// <summary>
        /// 开票状态
        /// </summary>
        private void init_invoiceconfirmstatus(HttpContext context)
        {
            StringBuilder sb = init_dictionary(context, Common.BusinessDict.invoiceConfirmStatus());
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
        }

        /// <summary>
        /// 获取所有活动归属地
        /// </summary>
        private void init_area(HttpContext context)
        {
            StringBuilder sb = init_dictionary(context, new BLL.department().getAreaDict());
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
        }
        
        /// <summary>
        /// 推送上级审批
        /// </summary>
        private void init_pushstatus(HttpContext context)
        {
            StringBuilder sb = init_dictionary(context, Common.BusinessDict.pushStatus());
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
        }

        /// <summary>
        /// 非业务支付申请支付类别
        /// </summary>
        private void init_unbusinessnature(HttpContext context)
        {
            get_params(context, out jObject);
            byte type = 0;
            if (jObject["type"] == null || string.IsNullOrWhiteSpace(jObject["type"].ToString()) || !byte.TryParse(jObject["type"].ToString(), out type))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                return;
            }
            StringBuilder sb = init_dictionary(context, Common.BusinessDict.unBusinessNature(type));
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
        }

        /// <summary>
        /// 非业务支付员工往来支付用途
        /// </summary>
        private void init_unbusinesspayfunction(HttpContext context)
        {
            get_params(context, out jObject);
            byte type = 0;
            if (jObject["type"] == null || string.IsNullOrWhiteSpace(jObject["type"].ToString()) || !byte.TryParse(jObject["type"].ToString(), out type))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                return;
            }
            StringBuilder sb = init_dictionary(context, Common.BusinessDict.unBusinessPayFunction(type));
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
        }

        /// <summary>
        /// 获取所有客户信息
        /// </summary>
        private void init_allcustomer(HttpContext context)
        {
            BLL.Customer bll = new BLL.Customer();
            DataSet ds = bll.GetNameList("c_id,c_name as name,c_type as type", "c_id > 0", "c_isUse desc,c_addDate desc,c_id desc");
            if (ds != null && ds.Tables.Count > 0)
            {
                context.Response.Write(JArray.FromObject(ds.Tables[0]));
                return;
            }
            context.Response.Write("[]");
            return;
        }

        /// <summary>
        /// 根据客户ID获取相关主要联系人及联系号码
        /// </summary>
        private void init_contactsbycid(HttpContext context)
        {
            get_params(context, out jObject);
            int c_id = 0;
            if (jObject["c_id"] == null || string.IsNullOrWhiteSpace(jObject["c_id"].ToString()) || !int.TryParse(jObject["c_id"].ToString(), out c_id))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                return;
            }
            DataSet ds = new BLL.Contacts().GetList(0, string.Format("co_cid={0}", c_id), "co_flag desc,co_id asc");
            if (ds != null && ds.Tables.Count > 0)
            {
                context.Response.Write(JArray.FromObject(ds.Tables[0]));
                return;
            }
            context.Response.Write("[]");
            return;
        }

        /// <summary>
        /// 根据活动归属地ID获取业务报账员和业务执行人员
        /// </summary>
        private void init_employeebyarea(HttpContext context)
        {
            get_params(context, out jObject);
            if (jObject["arealist"] == null || string.IsNullOrWhiteSpace(jObject["arealist"].ToString()))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                return;
            }

            string areaList = jObject["arealist"].ToString();
            DataTable dt = new BLL.department().getAllEmployee(areaList);
            if (dt != null && dt.Rows.Count > 0)
            {
                context.Response.Write(JArray.FromObject(dt));
                return;
            }
            context.Response.Write("[]");
            return;
        }

        /// <summary>
        /// 业务性质
        /// </summary>
        private void init_nature(HttpContext context)
        {
            get_params(context, out jObject);
            int managerid = 0;
            if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out managerid))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                return;
            }
            Model.manager managerModel = new BLL.manager().GetModel(managerid);
            if (managerModel != null)
            {
                string sqlwhere = "na_isUse=1";
                if (!new BLL.permission().checkHasPermission(managerModel, "0401"))
                {
                    sqlwhere += " and na_flag=0";
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                DataTable dt = new BLL.businessNature().GetList(0, sqlwhere, "na_sort asc,na_id desc").Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("{\"key\":\"" + dr["na_id"].ToString() + "\",\"value\":\"" + dr["na_name"].ToString() + "\"},");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
                context.Response.Write(sb.ToString());
                return;
            }
            context.Response.Write("[]");
            return;
        }

        /// <summary>
        /// 业务明细
        /// </summary>
        private void init_naturedetail(HttpContext context)
        {
            get_params(context, out jObject);
            int de_nid = 0;
            if (jObject["de_nid"] == null || string.IsNullOrWhiteSpace(jObject["de_nid"].ToString()) || !int.TryParse(jObject["de_nid"].ToString(), out de_nid))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            DataTable dt = new BLL.businessDetails().GetList(0, " de_nid =" + de_nid + " and de_isUse=1 ", "de_sort asc,de_id desc").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("{\"key\":\"" + dr["de_name"].ToString() + "\",\"value\":\"" + dr["de_name"].ToString() + "\"},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            context.Response.Write(sb.ToString());
            return;
        }

        /// <summary>
        /// 付款方式
        /// </summary>
        private void init_paymethod(HttpContext context)
        {
            get_params(context, out jObject);
            int managerid = 0;
            if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out managerid))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                return;
            }
            Model.manager managerModel = new BLL.manager().GetModel(managerid);
            if (managerModel != null)
            {
                //收付款方式
                //判断是否是财务来显示不同的收付款方式
                string sqlwhere = "";
                if (!new BLL.permission().checkHasPermission(managerModel, "0401"))
                {
                    sqlwhere = " and pm_type=0";
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                DataTable dt = new BLL.payMethod().GetList(0, "pm_isUse=1 " + sqlwhere + "", "pm_sort asc,pm_id asc").Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("{\"key\":\"" + dr["pm_id"].ToString() + "\",\"value\":\"" + dr["pm_name"].ToString() + "\"},");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
                context.Response.Write(sb.ToString());
                return;
            }
            context.Response.Write("[]");
            return;
        }

        /// <summary>
        /// 应税劳务
        /// </summary>
        private void init_servicetype(HttpContext context)
        {
            StringBuilder sb = init_dictionary(context, Common.BusinessDict.taxableType());
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        private void init_servicename(HttpContext context)
        {
            get_params(context, out jObject);
            if (jObject["inv_serviceType"] == null || string.IsNullOrWhiteSpace(jObject["inv_serviceType"].ToString()) || !byte.TryParse(jObject["inv_serviceType"].ToString(),out byte inv_serviceType))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                return;
            }

            StringBuilder sb = init_dictionary(context, Common.BusinessDict.serviceName(inv_serviceType));
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
        }

        /// <summary>
        /// 送票方式
        /// </summary>
        private void init_sentmethod(HttpContext context)
        {
            StringBuilder sb = init_dictionary(context, Common.BusinessDict.sentMethod());
            if (sb != null && sb.Length > 0) { context.Response.Write(sb.ToString()); return; }

            context.Response.Write("[]");
        }

        /// <summary>
        /// 开票区域
        /// </summary>
        private void init_invoicearea(HttpContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            DataTable dt = new BLL.department().GetList(0, "de_type=1 and de_parentid<>0", "de_sort,de_id").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("{\"key\":\"" + dr["de_area"].ToString() + "\",\"value\":\"" + dr["de_subname"].ToString() + "\"},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            context.Response.Write(sb.ToString());
            return;
        }

        /// <summary>
        /// 获取收付款方式
        /// </summary>
        private void method_data(HttpContext context)
        {
            get_params(context, out jObject);
            if (jObject["pmid"] == null || string.IsNullOrWhiteSpace(jObject["pmid"].ToString()))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                return;
            }

            int pmid = Utils.ObjToInt(jObject["pmid"], 0);
            Model.payMethod model = new BLL.payMethod().GetModel(pmid);
            if (model!=null)
            {
                context.Response.Write(JObject.FromObject(model));
                return;
            }
            context.Response.Write("{}");
            return;
        }

        /// <summary>
        /// 首页审批数量
        /// </summary>
        /// <param name="context"></param>
        private void init_AduitCount(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                int c1 = 0, c2 = 0, c3 = 0, c4 = 0, c5 = 0, c6 = 0;
                BLL.Order bll = new BLL.Order();
                if (new BLL.permission().checkHasPermission(managerModel, "0603"))//业务审批
                {
                    c1 = bll.getUnAduitOrder(managerModel.area);
                }
                if (new BLL.permission().checkHasPermission(managerModel, "0603,0402,0601"))//业务支付审核、非业务支付审核
                {
                    if (new BLL.permission().checkHasPermission(managerModel, "0603"))
                    {
                        c2 = bll.getUnAduitPay(1, managerModel.area);
                        c3 = bll.getUnAduitUnBusinessPay(1, managerModel.area);
                    }
                    else if (new BLL.permission().checkHasPermission(managerModel, "0402"))
                    {
                        c2 = bll.getUnAduitPay(2, "");
                        c3 = bll.getUnAduitUnBusinessPay(2, "");
                    }
                    else
                    {
                        c2 = bll.getUnAduitPay(3, "");
                        c3 = bll.getUnAduitUnBusinessPay(3, "");
                    }

                }
                if (new BLL.permission().checkHasPermission(managerModel, "0603,0402"))//发票审核
                {
                    if (new BLL.permission().checkHasPermission(managerModel, "0603"))
                    {
                        c4 = bll.getUnAduitInvoice(1, managerModel.area);
                    }
                    else
                    {
                        c4 = bll.getUnAduitInvoice(2, "");
                    }
                }
                if (new BLL.permission().checkHasPermission(managerModel, "0402,0601"))//预付款审批,业务退款审批
                {
                    if (new BLL.permission().checkHasPermission(managerModel, "0402"))
                    {
                        c5 = bll.getUnAduitExpectPay(1);
                        c6 = new BLL.ReceiptPay().getUnPaycount("1");
                    }
                    else
                    {
                        c5 = bll.getUnAduitExpectPay(2);
                        c6 = new BLL.ReceiptPay().getUnPaycount("2");
                    }
                }
                


                context.Response.Write("{\"status\": 1, \"count1\": "+ c1 + ", \"count2\": " + c2 + ", \"count3\": " + c3 + ", \"count4\": " + c4 + ", \"count5\": " + c5 + ", \"count6\": " + c6 + " }");
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 权限检查
        /// </summary>
        /// <param name="context"></param>
        private void permission_check(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);                
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string code = jObject["code"] == null ? "" : jObject["code"].ToString();//权限编码，多个用","隔开
                if (string.IsNullOrEmpty(code))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"NoCode\"}");
                    return;
                }
                string[] clist = code.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in clist)
                {
                    if (new BLL.permission().checkHasPermission(managerModel, item))
                    {
                        context.Response.Write("{\"status\": 1, \"msg\": \"True\"}");
                        return;
                    }
                }
                context.Response.Write("{\"status\": 1, \"msg\": \"False\"}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 个人未读消息
        /// </summary>
        /// <param name="context"></param>
        private void self_message(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["pageSize"].ToString(), out int pageSize) || !int.TryParse(jObject["pageIndex"].ToString(), out int pageIndex))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                                                
                string isRead = jObject["isRead"] == null ? "" : Utils.ObjectToStr(jObject["isRead"]);
                string keywords = jObject["keywords"] == null ? "" : Utils.ObjectToStr(jObject["keywords"]);
                string sqlWhere = " me_owner='" + managerModel.user_name + "' ";
                if (!string.IsNullOrEmpty(isRead))
                {
                    sqlWhere += " and me_isRead='"+ isRead + "'";
                }
                if (!string.IsNullOrEmpty(keywords))
                {
                    sqlWhere += " and (me_title like '%" + keywords + "&' or me_content like '%" + keywords + "&')";
                }

                BLL.selfMessage bll = new BLL.selfMessage();
                DataTable lst = bll.GetList(pageSize, pageIndex, sqlWhere, "me_isRead asc,me_addDate desc,me_id desc", out int pageTotal).Tables[0];
                if (lst != null && lst.Rows.Count > 0)
                {
                    context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"list\":");
                    context.Response.Write(JArray.FromObject(lst) + "}");
                    return;
                }
                context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 消息详情
        /// </summary>
        /// <param name="context"></param>
        private void self_messageDetail(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                
                int me_id = jObject["me_id"] == null ? 0 : Utils.ObjToInt(jObject["me_id"]);                
                BLL.selfMessage bll = new BLL.selfMessage();
                Model.selfMessage model = bll.GetModel(me_id);
                if (model == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"消息不存在\"}");
                    return;
                }
                context.Response.Write(JObject.FromObject(model));
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="context"></param>
        private void self_messageDel(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                int me_id = jObject["me_id"] == null ? 0 : Utils.ObjToInt(jObject["me_id"]);
                BLL.selfMessage bll = new BLL.selfMessage();
                if (bll.Delete(me_id))
                {
                    context.Response.Write("{\"status\": 1, \"msg\": \"删除成功\"}");
                    return;
                }
                context.Response.Write("{\"status\": 0, \"msg\": \"删除失败\"}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }
        /// <summary>
        /// 消息已读
        /// </summary>
        /// <param name="context"></param>
        private void self_messageRead(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                int me_id = jObject["me_id"] == null ? 0 : Utils.ObjToInt(jObject["me_id"]);
                BLL.selfMessage bll = new BLL.selfMessage();
                Model.selfMessage model = bll.GetModel(me_id);
                if (model == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"消息不存在\"}");
                    return;
                }
                model.me_isRead = true;
                if (bll.Update(model))
                {
                    context.Response.Write("{\"status\": 1, \"msg\": \"success\"}");
                    return;
                }
                context.Response.Write("{\"status\": 0, \"msg\": \"读取失败\"}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        #endregion

        #region 订单管理

        /// <summary>
        /// 查看订单列表
        /// </summary>
        private void order_list(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                //int pageTotal = 0; //数据总记录数
                //int pageSize = 10;
                //int pageIndex = 1;
                //int managerid = 0;
                //int flag = 0;//订单类型：0全部1我的订单

                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["pageSize"].ToString(), out int pageSize) || !int.TryParse(jObject["pageIndex"].ToString(), out int pageIndex) || !int.TryParse(jObject["flag"].ToString(), out int flag))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                string _type = jObject["type"]==null?"":jObject["type"].ToString();//是否为审核订单列表
                string _OrderID = jObject["orderid"]==null?"":jObject["orderid"].ToString();//订单号
                string _contractPrice = jObject["o_contractprice"]==null?"":jObject["o_contractprice"].ToString();//合同造价
                string _status = jObject["o_status"]==null?"":jObject["o_status"].ToString();//订单状态
                string _dstatus = jObject["o_dstatus"]==null?"":jObject["o_dstatus"].ToString();//接单状态
                string _pushstatus = jObject["o_ispush"]==null?"": jObject["o_ispush"].ToString();//是否推送
                string _flag = jObject["o_flag"]==null?"": jObject["o_flag"].ToString();//审批状态
                string _lockstatus = jObject["o_lockstatus"]==null?"": jObject["o_lockstatus"].ToString();//锁单状态

                int listType = Utils.ObjToInt(jObject["listType"], 0);//1未收款订单，2多付款订单
                bool lType = false;
                string sWhere = string.Empty;
                if (listType == 1 || listType == 2)
                {
                    lType = true;
                    if (listType == 1)
                    {
                        sWhere = " and fin_type=1 and finMoney-rpdMoney>0";
                    }
                    else
                    {
                        sWhere = " and fin_type=0 and finMoney-rpdMoney<0";
                    }
                }


                #region 筛选条件
                StringBuilder strTemp = new StringBuilder();
                if (!string.IsNullOrEmpty(_OrderID))
                {
                    strTemp.Append(" and (o_id like '%" + _OrderID + "%' or o_content like '%" + _OrderID + "%' or o_address like '%" + _OrderID + "%')");
                }
                if (!string.IsNullOrEmpty(_contractPrice))
                {
                    strTemp.Append(" and o_contractPrice='" + _contractPrice + "'");
                }
                if (!string.IsNullOrEmpty(_status))
                {
                    strTemp.Append(" and o_status=" + _status + "");
                }
                if (!string.IsNullOrEmpty(_dstatus))
                {
                    strTemp.Append(" and o_dstatus=" + _dstatus + "");
                }
                if (!string.IsNullOrEmpty(_pushstatus))
                {
                    strTemp.Append(" and o_isPush='" + _pushstatus + "'");
                }
                if (!string.IsNullOrEmpty(_flag))
                {
                    if (_flag == "3")
                    {
                        strTemp.Append(" and (o_flag=1 or o_flag=2)");
                    }
                    else
                    {
                        strTemp.Append(" and o_flag=" + _flag + ""); 
                    }
                }
                if (!string.IsNullOrEmpty(_lockstatus))
                {
                    strTemp.Append(" and o_lockStatus='" + _lockstatus + "'");
                }

                if (flag == 0)
                {
                    if (_type == "check")
                    {
                        if (!new BLL.permission().checkHasPermission(managerModel, "0603"))
                        {
                            context.Response.Write("{\"status\": 0, \"msg\": \"您没有管理该页面的权限，请勿非法进入！\"}");
                            return;
                        }
                        if (managerModel.area != new BLL.department().getGroupArea())
                        {
                            strTemp.Append(" and op_area='" + managerModel.area + "'");
                        }
                    }
                    else
                    {
                        //列表权限控制
                        if (managerModel.area != new BLL.department().getGroupArea())//如果不是总部的工号
                        {
                            if (new BLL.permission().checkHasPermission(managerModel, "0602"))
                            {
                                //含有区域权限可以查看本区域添加的
                                strTemp.Append(" and op_area='" + managerModel.area + "'");
                            }
                            else
                            {
                                //只能
                                strTemp.Append(" and op_number='" + managerModel.user_name + "'");
                            }
                        }
                    }
                }
                else if (flag == 1)
                {
                    strTemp.Append(" and (op_number='" + managerModel.user_name + "' or  exists (select * from ms_orderperson where o_id=op_oid and op_type=2 and op_number='" + managerModel.user_name + "'))");
                }
                else if (flag == 2)
                {
                    strTemp.Append(" and exists (select * from ms_orderperson where o_id=op_oid and op_type=2 and op_number='" + managerModel.user_name + "')");
                }
                else if (flag == 3)
                {
                    strTemp.Append(" and exists (select * from ms_orderperson where o_id=op_oid and op_type=3 and op_number='" + managerModel.user_name + "')");
                }
                else if (flag == 4)
                {
                    strTemp.Append(" and exists (select * from ms_orderperson where o_id=op_oid and op_type=4 and op_number='" + managerModel.user_name + "')");
                }
                else
                {
                    strTemp.Append(" and exists (select * from ms_orderperson where o_id=op_oid and op_type=5 and op_number='" + managerModel.user_name + "')");
                }
                #endregion
                                
                string _strWhere = "1=1" + strTemp.ToString();

                BLL.Order bll = new BLL.Order();
                DataTable lst = bll.GetList(pageSize, pageIndex, _strWhere, "o_addDate desc,o_id desc", out int pageTotal, lType,sWhere).Tables[0];
                if (lst != null && lst.Rows.Count > 0)
                {
                    context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"list\":");
                    context.Response.Write(JArray.FromObject(lst) + "}");
                    return;
                }
                context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \""+ ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 新增和修改订单方法
        /// </summary>
        private void order_edit(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                //int managerid = 0;
                //int cid = 0;//客户ID
                //int coid = 0;//联系人
                //int fstatus = 0;//订单状态

                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(Utils.ObjectToStr(jObject["c_id"]), out int cid) || !int.TryParse(Utils.ObjectToStr(jObject["co_id"]), out int coid) || !int.TryParse(Utils.ObjectToStr(jObject["fstatus"]), out int fstatus))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                string oID = jObject["orderID"] == null ? "" : jObject["orderID"].ToString();//订单号
                string contractPrice = jObject["o_contractprice"] == null ? "" : jObject["o_contractprice"].ToString();//合同造价
                string sdate = jObject["o_sdate"] == null ? "" : jObject["o_sdate"].ToString();//活动开始日期
                string edate = jObject["o_edate"] == null ? "" : jObject["o_edate"].ToString();//活动结束日期
                string address = jObject["o_address"] == null ? "" : jObject["o_address"].ToString();//活动地点
                string content = jObject["o_content"] == null ? "" : jObject["o_content"].ToString();//活动名称
                string contract = jObject["o_contractcontent"] == null ? "" : jObject["o_contractcontent"].ToString();//合同内容
                string remark = jObject["o_remarks"] == null ? "" : jObject["o_remarks"].ToString();//备注
                string place = jObject["o_place"] == null ? "" : jObject["o_place"].ToString();//活动归属地
                string employee1 = jObject["employee1"] == null ? "" : jObject["employee1"].ToString();//业务报账人员
                string employee2 = jObject["employee2"] == null ? "" : jObject["employee2"].ToString();//业务策划人员
                string employee3 = jObject["employee3"] == null ? "" : jObject["employee3"].ToString();//业务设计人员
                string employee4 = jObject["employee4"] == null ? "" : jObject["employee4"].ToString();//业务执行人员
                string pushStatus = jObject["o_isPush"] == null ? "" : jObject["o_isPush"].ToString();//推送上级审批

                Model.Order order = new Model.Order();
                order.o_id = oID;
                order.o_cid = cid;
                order.o_coid = coid;
                order.o_content = content;
                order.o_address = address;
                order.o_contractPrice = contractPrice;
                order.o_contractContent = contract;
                order.o_sdate = ConvertHelper.toDate(sdate);
                order.o_edate = ConvertHelper.toDate(edate);
                order.o_place = place;
                order.o_status = (byte)fstatus;
                order.o_isPush = pushStatus == "True" ? true : false;
                order.o_remarks = remark;
                string[] list = new string[] { }, pli = new string[] { };

                #region 业务报账员
                if (!string.IsNullOrEmpty(employee1))
                {
                    pli = employee1.Split('|');
                    order.personlist.Add(new Model.OrderPerson() { op_type = 2, op_name = pli[0], op_number = pli[1], op_area = pli[2] });
                }
                #endregion

                #region 业务策划人员
                if (!string.IsNullOrEmpty(employee2))
                {
                    pli = new string[] { };
                    list = employee2.Split(',');
                    foreach (string item in list)
                    {
                        pli = item.Split('|');
                        order.personlist.Add(new Model.OrderPerson() { op_type = 3, op_name = pli[0], op_number = pli[1], op_area = pli[2], op_dstatus = Utils.ObjToByte(pli[3]) });
                    }
                }
                #endregion

                #region 业务执行人员
                if (!string.IsNullOrEmpty(employee3))
                {
                    list = new string[] { };
                    pli = new string[] { };
                    list = employee3.Split(',');
                    foreach (string item in list)
                    {
                        pli = item.Split('|');
                        order.personlist.Add(new Model.OrderPerson() { op_type = 4, op_name = pli[0], op_number = pli[1], op_area = pli[2] });
                    }
                }
                #endregion

                #region 业务设计人员
                if (!string.IsNullOrEmpty(employee4))
                {
                    pli = new string[] { };
                    list = employee4.Split(',');
                    foreach (string item in list)
                    {
                        pli = item.Split('|');
                        order.personlist.Add(new Model.OrderPerson() { op_type = 5, op_name = pli[0], op_number = pli[1], op_area = pli[2], op_dstatus = Utils.ObjToByte(pli[3]) });
                    }
                }
                #endregion

                string oid = string.Empty;
                string result = string.Empty;
                BLL.manager_role bll = new BLL.manager_role();

                if (string.IsNullOrEmpty(oID))
                {
                    if (!bll.Exists(managerModel.role_id, "sys_order_add", "Add"))
                    {
                        context.Response.Write("{ \"msg\":\"您没有管理该页面的权限，请勿非法进入！\", \"status\":0 }");
                        return;
                    }
                    result = new BLL.Order().AddOrder(order, managerModel, out oid);
                }
                else
                {
                    if (!bll.Exists(managerModel.role_id, "sys_order_add", "Edit"))
                    {
                        context.Response.Write("{ \"msg\":\"您没有管理该页面的权限，请勿非法进入！\", \"status\":0 }");
                        return;
                    }
                    oid = oID;
                    result = new BLL.Order().UpdateOrder(order, managerModel);
                }
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 查看订单方法
        /// </summary>
        private void order_show(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                if (jObject["orderID"] == null || string.IsNullOrWhiteSpace(jObject["orderID"].ToString()))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                string _oid = jObject["orderID"].ToString();//订单号
                //string _oid = "20190603001";
                BLL.Order bll = new BLL.Order();
                DataSet ds = bll.GetList(0, "o_id='" + _oid + "'", "o_addDate desc");
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    context.Response.Write("{ \"msg\":\"订单不存在\", \"status\":0 }");
                    return;
                }

                JObject _object = new JObject();
                DataRow dr = ds.Tables[0].Rows[0];
                _object.Add("owner", new MettingSys.BLL.department().getAreaText(dr["op_area"].ToString()) + "," + dr["op_number"] + "," + dr["op_name"]);//下单人
                _object.Add("c_name", dr["c_name"].ToString());//客户
                _object.Add("c_id", dr["c_id"].ToString());//客户ID
                _object.Add("o_coid", dr["o_coid"].ToString());//联系人ID
                _object.Add("co_name", dr["co_name"].ToString());//联系人名称
                _object.Add("co_number", dr["co_number"].ToString());//联系电话
                _object.Add("o_contractprice", dr["o_contractPrice"].ToString());//合同造价
                _object.Add("o_sdate", ConvertHelper.toDate(dr["o_sdate"]).Value.ToString("yyyy-MM-dd"));//活动开始日期
                _object.Add("o_edate", ConvertHelper.toDate(dr["o_edate"]).Value.ToString("yyyy-MM-dd"));//活动结束日期
                _object.Add("o_address", dr["o_address"].ToString());//活动地点
                _object.Add("o_content", dr["o_content"].ToString());//活动名称
                _object.Add("o_contractContent", dr["o_contractContent"].ToString());//合同内容
                _object.Add("o_remarks", dr["o_remarks"].ToString());//备注
                _object.Add("o_status", dr["o_status"].ToString());//订单状态
                _object.Add("o_statusText", BusinessDict.fStatus()[Utils.ObjToByte(dr["o_status"].ToString())]);//订单状态
                _object.Add("o_ispush", dr["o_isPush"].ToString());//推送上级审批
                _object.Add("o_flag", dr["o_flag"].ToString());//业务上级审批
                _object.Add("o_flagText", Common.BusinessDict.checkStatus()[Utils.ObjToByte(dr["o_flag"])]);//业务上级审批
                _object.Add("o_lockStatusText", Common.BusinessDict.lockStatus()[Utils.ObjToByte(dr["o_lockStatus"])]);//锁单状态
                _object.Add("o_financecust", dr["o_financeCust"].ToString());//税费成本
                _object.Add("o_lockStatus", dr["o_lockStatus"].ToString());//锁单状态

                #region 归属地
                string placeStr = dr["o_place"].ToString();
                JArray ja = new JArray();
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
                    _object.Add("arealist", JObject.FromObject(orderAreaDic));//活动归属地
                }
                #endregion

                #region 人员
                DataTable pdt = bll.GetPersonList(0, "op_oid='" + _oid + "'", "op_id asc").Tables[0];
                if (pdt != null)
                {
                    DataRow[] plist = null;
                    for (int i = 2; i < 6; i++)
                    {
                        JArray pja = new JArray();
                        plist = pdt.Select("op_type=" + i + "");
                        foreach (DataRow pdr in plist)
                        {
                            JObject pjo = new JObject();
                            pjo.Add("op_id", Utils.ObjToInt(pdr["op_id"]));
                            pjo.Add("op_oid", Utils.ObjectToStr(pdr["op_oid"]));
                            pjo.Add("op_type", Utils.ObjToInt(pdr["op_type"]));
                            pjo.Add("op_number", Utils.ObjectToStr(pdr["op_number"]));
                            pjo.Add("op_name", Utils.ObjectToStr(pdr["op_name"]));
                            pjo.Add("op_area", Utils.ObjectToStr(pdr["op_area"]));
                            pjo.Add("op_dstatus", Utils.ObjToInt(pdr["op_dstatus"]));
                            pja.Add(pjo);
                        }
                        if (i == 2)
                        {
                            _object.Add("Employee1", pja);//业务报账人员
                        }
                        else if (i == 3)
                        {
                            _object.Add("Employee2", pja);//业务策划人员
                        }
                        else if (i == 4)
                        {
                            _object.Add("Employee3", pja);//业务执行人员
                        }
                        else {
                            _object.Add("Employee4", pja);//业务设计人员
                        }
                    }                    
                }
                //_object.Add("Employee1", JArray.FromObject(pdt.Select("op_type=2")));//业务报账人员
                //_object.Add("Employee2", JArray.FromObject(pdt.Select("op_type=3")));//业务策划人员
                //_object.Add("Employee3", JArray.FromObject(pdt.Select("op_type=4")));//业务执行人员
                //_object.Add("Employee4", JArray.FromObject(pdt.Select("op_type=5")));//业务设计人员

                #endregion

                #region 活动文件
                DataTable fdt = bll.GetFileList(0, "f_oid='" + _oid + "'", "f_addDate asc,f_id asc").Tables[0];
                if (fdt != null)
                {
                    DataRow[] flist = null;
                    for (int i = 1; i < 3; i++)
                    {
                        JArray fja = new JArray();
                        flist = fdt.Select("f_type=" + i + "");
                        foreach (DataRow fdr in flist)
                        {
                            JObject fjo = new JObject();
                            fjo.Add("f_id", Utils.ObjToInt(fdr["f_id"]));
                            fjo.Add("f_oid", Utils.ObjectToStr(fdr["f_oid"]));
                            fjo.Add("f_type", Utils.ObjToInt(fdr["f_type"]));
                            fjo.Add("f_fileName", Utils.ObjectToStr(fdr["f_fileName"]));
                            fjo.Add("f_filePath", Utils.ObjectToStr(fdr["f_filePath"]));
                            fjo.Add("f_size", Utils.ObjectToStr(fdr["f_size"]));
                            fjo.Add("f_addDate", Utils.ObjToInt(fdr["f_addDate"]));
                            fjo.Add("f_addPerson", Utils.ObjToInt(fdr["f_addPerson"]));
                            fjo.Add("f_addName", Utils.ObjToInt(fdr["f_addName"]));
                            fja.Add(fjo);
                        }
                        _object.Add("files"+i, fja);
                    }
                }
                //_object.Add("files1", JArray.FromObject(fdt.Select("f_type=1")));
                //_object.Add("files2", JArray.FromObject(fdt.Select("f_type=2")));

                #endregion

                context.Response.Write(_object);
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 业务上级审批
        /// </summary>
        /// <param name="context"></param>
        private void order_check(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string flag = jObject["o_flag"] == null ? "" : Utils.ObjectToStr(jObject["o_flag"]);
                if (!byte.TryParse(flag,out byte o_flag))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"flagError\"}");
                    return;
                }

                string result = new BLL.Order().updateFlag(Utils.ObjectToStr(jObject["orderID"]), o_flag, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="context"></param>
        private void order_delete(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string result = new BLL.Order().DeleteOrder(Utils.ObjectToStr(jObject["orderID"]), managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }
               
        /// <summary>
        /// 非业务支付审核分页列表
        /// </summary>
        private void unbusinesspay_list(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["pageSize"].ToString(), out int pageSize) || !int.TryParse(jObject["pageIndex"].ToString(), out int pageIndex))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string keywords = jObject["keywords"] == null ? "" : jObject["keywords"].ToString();
                string _type = jObject["type"] == null ? "" : jObject["type"].ToString();
                string _flag = jObject["flag"] == null ? "" : jObject["flag"].ToString();//flag:"0"待审批页签，"1"已审批页签
                #region 筛选条件
                StringBuilder strTemp = new StringBuilder();
                if (!string.IsNullOrEmpty(keywords))
                {
                    strTemp.Append(" and uba_oid like '%" + keywords + "%' or uba_function  like '%" + keywords + "%'");
                }
                #endregion

                #region 审核列表
                string checkType = "0";
                if (_type == "check")
                {
                    if (new BLL.permission().checkHasPermission(managerModel, "0603"))//部门审批
                    {
                        checkType = "1";
                        if (_flag == "0")
                        {
                            if (managerModel.area == new BLL.department().getGroupArea())
                            {
                                strTemp.Append(" and uba_flag1=0 and uba_flag2=0 and uba_flag3=0 and uba_isConfirm='False'");
                            }
                            else
                            {
                                strTemp.Append(" and uba_area='" + managerModel.area + "' and uba_flag1=0 and uba_flag2=0 and uba_flag3=0 and uba_isConfirm='False'");
                            }
                        }
                        else if (_flag == "1")
                        {
                            if (managerModel.area == new BLL.department().getGroupArea())
                            {
                                strTemp.Append(" and (uba_flag1=1 or  uba_flag1=2)");
                            }
                            else
                            {
                                strTemp.Append(" and uba_area='" + managerModel.area + "' and (uba_flag1=1 or  uba_flag1=2)");
                            }
                        }
                    }
                    else if (new BLL.permission().checkHasPermission(managerModel, "0402"))//财务审批
                    {
                        checkType = "2";
                        if (_flag == "0")
                        {
                            strTemp.Append(" and uba_flag1=2 and uba_flag2=0 and uba_flag3=0 and uba_isConfirm='False'");
                        }
                        else if (_flag == "1")
                        {
                            strTemp.Append(" and uba_flag1=2 and (uba_flag2=1 or  uba_flag2=2)");
                        }
                    }
                    else if (new BLL.permission().checkHasPermission(managerModel, "0601"))//总经理审批
                    {
                        checkType = "3";
                        if (_flag == "0")
                        {
                            strTemp.Append(" and uba_flag1=2 and uba_flag2=2 and uba_flag3=0 and uba_isConfirm='False'");
                        }
                        else if (_flag == "1")
                        {
                            strTemp.Append(" and uba_flag1=2 and uba_flag2=2 and (uba_flag3=1 or  uba_flag3=2)");
                        }
                    }
                    else
                    {
                        context.Response.Write("{ \"msg\":\"您无权限管理非业务支付申请\", \"status\":0 }");
                        return;
                    }
                }
                #endregion

                string _strWhere = "uba_id>0" + strTemp.ToString();

                BLL.unBusinessApply bll = new BLL.unBusinessApply();
                DataTable lst = bll.GetList(pageSize, pageIndex, _strWhere, "uba_addDate desc,uba_id desc", managerModel, out int pageTotal,out decimal _pmoney).Tables[0];
                if (lst != null && lst.Rows.Count > 0)
                {
                    context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"checkType\":" + checkType + ",\"list\":");
                    context.Response.Write(JArray.FromObject(lst) + "}");
                    return;
                }
                context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"checkType\":" + checkType + ",\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\"" + ex.Message + "\", \"status\":0 }");
                return;
            }
        }

        /// <summary>
        /// 新增非业务支付审核
        /// </summary>
        private void unbusinesspay_add(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);                
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !byte.TryParse(jObject["uba_type"].ToString(), out byte uba_type) || !DateTime.TryParse(jObject["uba_foreDate"].ToString(), out DateTime uba_foreDate))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                Model.unBusinessApply model = new Model.unBusinessApply();
                BLL.unBusinessApply bll = new BLL.unBusinessApply();

                if (jObject["uba_money"]==null || !Decimal.TryParse(jObject["uba_money"].ToString(), out decimal uba_money))
                {
                    context.Response.Write("{ \"msg\":\"金额格式有误\", \"status\":0 }");
                    return;
                }
                model.uba_money = uba_money;//金额
                model.uba_type = uba_type;//支付类别
                model.uba_function = jObject["uba_function"] == null ? "" : jObject["uba_function"].ToString();//支付用途
                if (jObject["uba_oid"] != null && !string.IsNullOrEmpty(Utils.ObjectToStr(jObject["uba_oid"])))
                {
                    model.uba_oid = jObject["uba_oid"].ToString();//订单号
                }
                model.uba_instruction = jObject["uba_instruction"] == null ? "" : jObject["uba_instruction"].ToString();//用途说明
                model.uba_receiveBank = jObject["uba_receiveBank"] == null ? "" : jObject["uba_receiveBank"].ToString();//收款银行
                model.uba_receiveBankName = jObject["uba_receiveBankName"] == null ? "" : jObject["uba_receiveBankName"].ToString();//账户名称
                model.uba_receiveBankNum = jObject["uba_receiveBankNum"] == null ? "" : jObject["uba_receiveBankNum"].ToString();//收款账号                
                model.uba_foreDate = uba_foreDate;//预付日期
                model.uba_remark = jObject["uba_remark"] == null ? "" : jObject["uba_remark"].ToString();//备注
                model.uba_PersonNum = managerModel.user_name;
                model.uba_personName = managerModel.real_name;
                model.uba_addDate = DateTime.Now;
                model.uba_flag1 = 0;
                model.uba_flag2 = 0;
                model.uba_flag3 = 0;
                model.uba_area = managerModel.area;
                int id = 0;
                string result = bll.Add(model, managerModel, out id);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1, \"uba_id\":"+ id + " }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\"" + ex.Message + "\", \"status\":0 }");
                return;
            }
        }

        /// <summary>
        /// 修改非业务支付审核
        /// </summary>
        private void unbusinesspay_edit(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                StringBuilder sb = new StringBuilder();
                BLL.unBusinessApply bll = new BLL.unBusinessApply();
                
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["uba_id"].ToString(), out int uba_id))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                Model.unBusinessApply model = bll.GetModel(uba_id);
                if (model == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyidIsNullOrError\"}");
                    return;
                }

                if (jObject["uba_function"] == null || string.IsNullOrWhiteSpace(jObject["uba_function"].ToString()))
                {
                    context.Response.Write("{ \"msg\":\"支付用途不能为空\", \"status\":0 }");
                    return;
                }
                string _uba_function = jObject["uba_function"].ToString();//支付用途
                if (model.uba_function != _uba_function)
                {
                    sb.Append("支付用途：" + model.uba_function + "→<font color='red'>" + _uba_function + "</font><br/>");
                }
                model.uba_function = _uba_function;

                string _uba_instruction = jObject["uba_instruction"] == null ? "" : jObject["uba_instruction"].ToString();//用途说明
                if (model.uba_instruction != _uba_instruction)
                {
                    sb.Append("用途说明：" + model.uba_instruction + "→<font color='red'>" + _uba_instruction + "</font><br/>");
                }
                model.uba_instruction = _uba_instruction;

                string _uba_receiveBank = jObject["uba_receiveBank"] == null ? "" : jObject["uba_receiveBank"].ToString();//收款银行
                if (model.uba_receiveBank != _uba_receiveBank)
                {
                    sb.Append("收款银行：" + model.uba_receiveBank + "→<font color='red'>" + _uba_receiveBank + "</font><br/>");
                }
                model.uba_receiveBank = _uba_receiveBank;

                string _uba_receiveBankName = jObject["uba_receiveBankName"] == null ? "" : jObject["uba_receiveBankName"].ToString();//账户名称
                if (model.uba_receiveBankName != _uba_receiveBankName)
                {
                    sb.Append("账户名称：" + model.uba_receiveBankName + "→<font color='red'>" + _uba_receiveBankName + "</font><br/>");
                }
                model.uba_receiveBankName = _uba_receiveBankName;

                string _uba_receiveBankNum = jObject["uba_receiveBankNum"] == null ? "" : jObject["uba_receiveBankNum"].ToString();//收款账号
                if (model.uba_receiveBankNum != _uba_receiveBankNum)
                {
                    sb.Append("收款账号：" + model.uba_receiveBankNum + "→<font color='red'>" + _uba_receiveBankNum + "</font><br/>");
                }
                model.uba_receiveBankNum = _uba_receiveBankNum;

                if (jObject["uba_money"] == null || !decimal.TryParse(jObject["uba_money"].ToString(), out decimal _uba_money))//金额
                {
                    context.Response.Write("{ \"msg\":\"金额格式有误\", \"status\":0 }");
                    return;
                }
                if (model.uba_money != _uba_money)
                {
                    sb.Append("金额：" + model.uba_money + "→<font color='red'>" + _uba_money + "</font><br/>");
                }
                model.uba_money = _uba_money;

                if (jObject["uba_foreDate"] == null || !DateTime.TryParse(jObject["uba_foreDate"].ToString(), out DateTime _uba_foreDate))//预付日期
                {
                    context.Response.Write("{ \"msg\":\"预付日期不能为空或格式有误\", \"status\":0 }");
                    return;
                }
                if (model.uba_foreDate != _uba_foreDate)
                {
                    sb.Append("预付日期：" + model.uba_foreDate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + _uba_foreDate + "</font><br/>");
                }
                model.uba_foreDate = _uba_foreDate;

                string _uba_remark = jObject["uba_remark"] == null ? "" : jObject["uba_remark"].ToString();//备注
                if (model.uba_remark != _uba_remark)
                {
                    sb.Append("备注：" + model.uba_remark + "→<font color='red'>" + _uba_remark + "</font><br/>");
                }
                model.uba_remark = _uba_remark;
                string result = bll.Update(model, sb.ToString(), managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\"" + ex.Message + "\", \"status\":0 }");
                return;
            }
        }

        /// <summary>
        /// 绑定非业务支付审批类型
        /// </summary>
        /// <param name="context"></param>
        private void unbusinesspay_auditBind(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                StringBuilder sb = new StringBuilder();
                BLL.unBusinessApply bll = new BLL.unBusinessApply();

                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                int ubaid = jObject["ubaid"] == null ? 0 : Utils.ObjToInt(jObject["ubaid"], 0);
                if (ubaid == 0)
                {
                    context.Response.Write("{ \"msg\":\"参数有误\", \"status\":0 }");
                    return;
                }
                Model.unBusinessApply ubApply = new BLL.unBusinessApply().GetModel(ubaid);
                if (ubApply == null)
                {
                    context.Response.Write("{ \"msg\":\"记录不存在\", \"status\":0 }");
                    return;
                }
                string type = "";//1部门审批，2财务审批，3总经理审批
                byte flag = 0;
                string remark = string.Empty;
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                if (ubApply.uba_area==managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0603"))//部门审批
                {
                    type = "1";
                    flag = ubApply.uba_flag1.Value;
                    remark = ubApply.uba_checkRemark1;
                }
                else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0402"))//财务审批
                {
                    type = "2";
                    flag = ubApply.uba_flag2.Value;
                    remark = ubApply.uba_checkRemark2;
                }
                else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0601"))//总经理审批
                {
                    type = "3";
                    flag = ubApply.uba_flag3.Value;
                    remark = ubApply.uba_checkRemark3;
                }
                else
                {
                    context.Response.Write("{ \"msg\":\"您无权限管理非业务支付申请\", \"status\":0 }");
                    return;
                }

                context.Response.Write("{ \"msg\":\"success\", \"status\":1,\"type\":"+type+ ",\"flag\":" + flag + ",\"remark\":\""+ remark + "\"}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\"" + ex.Message + "\", \"status\":0 }");
                return;
            }
        }

        /// <summary>
        /// 查看非业务支付审核
        /// </summary>
        private void unbusinesspay_show(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                //非业务支付申请表自增主键
                if (jObject["uba_id"] == null || string.IsNullOrWhiteSpace(jObject["uba_id"].ToString()) || !int.TryParse(jObject["uba_id"].ToString(), out int id))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                
                BLL.unBusinessApply bll = new BLL.unBusinessApply();
                Model.unBusinessApply model = bll.GetModel(id);
                if (model == null)
                {
                    context.Response.Write("{ \"msg\":\"非业务支付申请信息不存在\", \"status\":0 }");
                    return;
                }
                JObject _object = JObject.FromObject(model);
                _object.Add("uba_typeText", BusinessDict.unBusinessNature(1)[model.uba_type.Value]);

                #region 审批列表
                string type = jObject["type"] == null ? "" : jObject["type"].ToString();
                if (type == "check")
                {
                    string checktype = "", flag = "", remark = "";
                    if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                    {
                        context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                        return;
                    }

                    Model.manager managerModel = new BLL.manager().GetModel(managerid);
                    if (managerModel == null)
                    {
                        context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                        return;
                    }
                    if (model.uba_area == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0603"))//部门审批
                    {
                        checktype = "1";
                        flag = Utils.ObjectToStr(model.uba_flag1);
                        remark = Utils.ObjectToStr(model.uba_checkRemark1);
                    }
                    else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0402"))//财务审批
                    {
                        checktype = "2";
                        flag = Utils.ObjectToStr(model.uba_flag2);
                        remark = Utils.ObjectToStr(model.uba_checkRemark2);
                    }
                    else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0601"))//总经理审批
                    {
                        checktype = "3";
                        flag = Utils.ObjectToStr(model.uba_flag3);
                        remark = Utils.ObjectToStr(model.uba_checkRemark3);
                    }
                    else
                    {
                        context.Response.Write("{ \"msg\":\"您无权限审批业务支付\", \"status\":0 }");
                        return;
                    }
                    _object.Add("type", checktype);
                    _object.Add("flag", flag);
                    _object.Add("remark", remark);
                }
                #endregion

                JArray picJa = new JArray();
                DataSet ds = new BLL.payPic().GetList(2, "pp_type=2 and pp_rid=" + id + "", "pp_addDate desc");
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    picJa = JArray.FromObject(ds.Tables[0]);
                    _object.Add("picList", picJa);
                }
                else
                {
                    _object.Add("picList", picJa);
                }
                context.Response.Write(_object);
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\""+ ex.Message + "\", \"status\":0 }");
                return;
            }
        }

        /// <summary>
        /// 审批非业务支付
        /// </summary>
        private void unbusinesspay_audit(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                if (jObject["uba_id"] == null || !int.TryParse(jObject["uba_id"].ToString(), out int uba_id))
                {
                    context.Response.Write("{ \"msg\":\"信息不存在\", \"status\":0 }");
                    return;
                }
                if (jObject["ctype"] == null || !byte.TryParse(jObject["ctype"].ToString(), out byte ctype))
                {
                    context.Response.Write("{ \"msg\":\"请选择审批类型\", \"status\":0 }");
                    return;
                }
                if (jObject["cstatus"] == null || !byte.TryParse(jObject["cstatus"].ToString(), out byte cstatus))
                {
                    context.Response.Write("{ \"msg\":\"请选择审批状态\", \"status\":0 }");
                    return;
                }

                BLL.unBusinessApply bll = new BLL.unBusinessApply();
                string result = bll.checkStatus(uba_id, ctype, cstatus, jObject["remark"] == null ? "" : jObject["remark"].ToString(), managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\"" + ex.Message + "\", \"status\":0 }");
                return;
            }
        }

        /// <summary>
        /// 删除非业务支付申请
        /// </summary>
        /// <param name="context"></param>
        private void unbusinesspay_del(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                if (jObject["uba_id"] == null || !int.TryParse(jObject["uba_id"].ToString(), out int uba_id))
                {
                    context.Response.Write("{ \"msg\":\"信息不存在\", \"status\":0 }");
                    return;
                }

                BLL.unBusinessApply bll = new BLL.unBusinessApply();
                string result = bll.Delete(uba_id, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\"" + ex.Message + "\", \"status\":0 }");
                return;
            }
        }

        /// <summary>
        /// 非业务支付确认
        /// </summary>
        private void unbusinesspay_confirm_pay(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                if (jObject["uba_id"] == null || !int.TryParse(jObject["uba_id"].ToString(), out int uba_id))
                {
                    context.Response.Write("{ \"msg\":\"信息不存在\", \"status\":0 }");
                    return;
                }
                string status = jObject["uba_isConfirm"] == null ? "" : Utils.ObjectToStr(jObject["uba_isConfirm"]);
                string date = jObject["uba_date"] == null ? "" : Utils.ObjectToStr(jObject["uba_date"]);
                int method = Utils.ObjToInt(jObject["uba_payMethod"], 0);
                string methodName= jObject["methodName"] == null ? "" : Utils.ObjectToStr(jObject["methodName"]);
                //if (jObject["status"] == null || string.IsNullOrWhiteSpace(jObject["status"].ToString()))
                //{
                //    context.Response.Write("{ \"msg\":\"请选择支付状态\", \"status\":0 }");
                //    return;
                //}
                //if (jObject["date"] == null || string.IsNullOrWhiteSpace(jObject["date"].ToString()))
                //{
                //    context.Response.Write("{ \"msg\":\"请选择实付日期\", \"status\":0 }");
                //    return;
                //}
                //if (jObject["method"] == null || !int.TryParse(jObject["method"].ToString(), out int method))
                //{
                //    context.Response.Write("{ \"msg\":\"请选择付款方式\", \"status\":0 }");
                //    return;
                //}

                BLL.unBusinessApply bll = new BLL.unBusinessApply();
                string result = bll.confirmStatus(uba_id, status, date, method, methodName, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\"" + ex.Message + "\", \"status\":0 }");
                return;
            }
        }

        /// <summary>
        /// 新增收款通知\付款通知
        /// </summary>
        private void finance_add(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }

                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                if (jObject["rp_type"] == null || !bool.TryParse(jObject["rp_type"].ToString(),out bool rp_type))
                {
                    context.Response.Write("{ \"msg\":\"应收付标识不能为空\", \"status\":0 }");
                    return;
                }
                if (jObject["rp_cid"] == null || !int.TryParse(jObject["rp_cid"].ToString(), out int rp_cid))
                {
                    context.Response.Write("{ \"msg\":\"收付对象不能为空\", \"status\":0 }");
                    return;
                }
                if (jObject["fin_cid"] == null || !int.TryParse(jObject["fin_cid"].ToString(), out int fin_cid))
                {
                    context.Response.Write("{ \"msg\":\"应收付对象不能为空\", \"status\":0 }");
                    return;
                }
                if (jObject["fin_nature"] == null || !int.TryParse(jObject["fin_nature"].ToString(), out int fin_nature))
                {
                    context.Response.Write("{ \"msg\":\"请选择业务性质\", \"status\":0 }");
                    return;
                }
                if (jObject["fin_detail"] == null || string.IsNullOrWhiteSpace(jObject["fin_detail"].ToString()))
                {
                    context.Response.Write("{ \"msg\":\"请选择业务明细\", \"status\":0 }");
                    return;
                }
                if (jObject["fin_sdate"] == null || DateTime.TryParse(jObject["fin_sdate"].ToString(),out DateTime fin_sdate))
                {
                    context.Response.Write("{ \"msg\":\"请选择业务开始日期或格式不正确\", \"status\":0 }");
                    return;
                }
                if (jObject["fin_edate"] == null || DateTime.TryParse(jObject["fin_edate"].ToString(), out DateTime fin_edate))
                {
                    context.Response.Write("{ \"msg\":\"请选择业务结束日期或格式不正确\", \"status\":0 }");
                    return;
                }
                if (jObject["fin_expression"] == null || string.IsNullOrWhiteSpace(jObject["fin_expression"].ToString()))
                {
                    context.Response.Write("{ \"msg\":\"金额表达式不能为空\", \"status\":0 }");
                    return;
                }


                Model.ReceiptPay model = new Model.ReceiptPay();
                BLL.ReceiptPay bll = new BLL.ReceiptPay();

                model.rp_type = rp_type;
                model.rp_cid = rp_cid;
                //model.rpd_cid = Utils.StrToInt(jObject["rpd_cid"].ToString(), 0);
                //model.rpd_content = jObject["rpd_content"] == null ? "" : jObject["rpd_content"].ToString();
                //model.rpd_money = rpd_money;
                //model.rpd_foredate = ConvertHelper.toDate(jObject["rpd_foredate"].ToString());
                //model.rpd_method = Utils.StrToInt(jObject["rpd_method"].ToString(), 0);
                ////model.rpd_content = jo["rpd_content"].ToString();
                //model.rpd_personNum = managerModel.user_name;
                //model.rpd_personName = managerModel.real_name;
                //model.rpd_adddate = DateTime.Now;
                //model.rpd_flag1 = 2;
                //model.rpd_area = managerModel.area;
                //int id = 0;
                string result = "";//bll.Add(model, managerModel, txtCenum.Text.Trim(), txtCedate.Text.Trim(), out rpid);


                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\"" + ex.Message + "\", \"status\":0 }");
                return;
            }
        }

        /// <summary>
        /// 审核业务
        /// </summary>
        private void finance_audit(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["id"].ToString(), out int id) || !byte.TryParse(jObject["status"].ToString(), out byte status))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string remark = jObject["remark"] == null ? "" : jObject["remark"].ToString();
                BLL.finance bll = new BLL.finance();
                string result = bll.checkStatus(id, status, remark, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\"" + ex.Message + "\", \"status\":0 }");
                return;
            }
        }

        /// <summary>
        /// 发票申请分页列表
        /// </summary>
        private void invoice_list(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["pageSize"].ToString(), out int pageSize) || !int.TryParse(jObject["pageIndex"].ToString(), out int pageIndex))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                string keywords = jObject["keywords"].ToString();
                byte inv_flag = byte.Parse(jObject["inv_flag"].ToString());

                string checkType = "0";
                #region 筛选条件
                StringBuilder strTemp = new StringBuilder();
                if (!string.IsNullOrEmpty(keywords))
                {
                    strTemp.Append(" and inv_oid like '%" + keywords + "%' or inv_purchaserName  like '%" + keywords + "%'");
                }
                if (new BLL.permission().checkHasPermission(managerModel, "0603"))
                {
                    checkType = "1";
                    if (inv_flag == 0)
                    {
                        if (managerModel.area != new BLL.department().getGroupArea())
                        {
                            strTemp.Append(" and ((inv_farea='" + managerModel.area + "' and inv_flag1=0) or (inv_darea='" + managerModel.area + "' and inv_flag2=0))");
                        }
                        else
                        {
                            strTemp.Append(" and (inv_flag1=0 or inv_flag2=0)");
                        }
                    }
                    else
                    {
                        if (managerModel.area != new BLL.department().getGroupArea())
                        {
                            strTemp.Append(" and ((inv_farea='" + managerModel.area + "' and (inv_flag1=1 or  inv_flag1=2)) or (inv_darea='" + managerModel.area + "' and (inv_flag2=1 or inv_flag2=2)))");
                        }
                        else
                        {
                            strTemp.Append(" and ((inv_flag1=1 or  inv_flag1=2) or (inv_flag2=1 or inv_flag2=2))");
                        }
                    }
                }
                else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0402"))
                {
                    checkType = "2";
                    if (inv_flag == 0)
                    {
                        strTemp.Append(" and inv_flag1=2 and inv_flag2=2 and inv_flag3=0");
                    }
                    else
                    {
                        strTemp.Append(" and inv_flag1=2 and inv_flag2=2 and (inv_flag3=1 or  inv_flag3=2)");
                    }
                }
                else
                {
                    context.Response.Write("{ \"msg\":\"您无权限管理发票申请\", \"status\":0 }");
                    return;
                }
                #endregion
                
                string _strWhere = "inv_id>0" + strTemp.ToString();

                BLL.invoices bll = new BLL.invoices();
                DataTable lst = bll.GetList(pageSize, pageIndex, _strWhere, "inv_addDate desc,inv_id desc", managerModel, out int pageTotal,out decimal _tmoney).Tables[0];
                if (lst != null && lst.Rows.Count > 0)
                {
                    context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"userArea\":\"" + managerModel.area + "\",\"checkType\":" + checkType + ",\"list\":");
                    context.Response.Write(JArray.FromObject(lst) + "}");
                    return;
                }
                context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"userArea\":\"" + managerModel.area + "\",\"checkType\":" + checkType + ",\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }
       
        /// <summary>
        /// 查看发票申请
        /// </summary>
        private void invoice_show(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                //发票申请表自增主键
                if (jObject["inv_id"] == null || string.IsNullOrWhiteSpace(jObject["inv_id"].ToString()) || !int.TryParse(jObject["inv_id"].ToString(), out int id))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                //int id = 12;
                DataTable dt = new BLL.invoices().GetList(0, "inv_id=" + id + "", "").Tables[0];
                if (dt == null || dt.Rows.Count == 0)
                {
                    context.Response.Write("{ \"msg\":\"发票申请信息不存在\", \"status\":0 }");
                    return;
                }
                DataRow dr = dt.Rows[0];
                JArray ja = JArray.FromObject(dt);
                JObject _object = JObject.FromObject(ja[0]);
                string type = jObject["type"] == null ? "" : jObject["type"].ToString();
                if (type == "check")
                {
                    string checktype = "", flag = "", remark = "";
                    if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                    {
                        context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                        return;
                    }

                    Model.manager managerModel = new BLL.manager().GetModel(managerid);
                    if (managerModel == null)
                    {
                        context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                        return;
                    }
                    if (new BLL.permission().checkHasPermission(managerModel, "0603"))
                    {
                        if (Utils.ObjectToStr(dr["inv_farea"]) == Utils.ObjectToStr(dr["inv_darea"]))
                        {
                            checktype = "1";
                        }
                        else
                        {
                            if (Utils.ObjectToStr(dr["inv_farea"]) == managerModel.area)
                            {
                                checktype = "2";
                            }
                            else if (Utils.ObjectToStr(dr["inv_darea"]) == managerModel.area)
                            {
                                checktype = "3";
                            }
                        }

                    }
                    else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0402"))
                    {
                        checktype = "4";
                    }
                    else
                    {
                        context.Response.Write("{ \"msg\":\"您无权限管理发票申请\", \"status\":0 }");
                        return;
                    }
                    _object.Add("type", checktype);
                    _object.Add("flag", flag);
                    _object.Add("remark", remark);
                }                               
                context.Response.Write(_object);
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 新增发票申请
        /// </summary>
        private void invoice_add(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["inv_cid"].ToString(), out int inv_cid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }

                if (jObject["inv_oid"] == null || jObject["inv_purchaserName"] == null || jObject["inv_purchaserNum"] == null || jObject["inv_purchaserAddress"] == null || jObject["inv_purchaserPhone"] == null || jObject["inv_purchaserBank"] == null || jObject["inv_purchaserBankNum"] == null || jObject["inv_serviceType"] == null || jObject["inv_serviceName"] == null || jObject["inv_money"] == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }

                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                Model.invoices model = new Model.invoices();
                BLL.invoices bll = new BLL.invoices();
                
                model.inv_oid = jObject["inv_oid"].ToString();//订单号
                model.inv_cid = inv_cid;
                model.inv_type = Utils.StrToBool(Utils.ObjectToStr(jObject["inv_type"]), false);
                model.inv_purchaserName = jObject["inv_purchaserName"].ToString().Trim();
                model.inv_purchaserNum = jObject["inv_purchaserNum"].ToString().Trim();
                model.inv_purchaserAddress = jObject["inv_purchaserAddress"].ToString().Trim();
                model.inv_purchaserPhone = jObject["inv_purchaserPhone"].ToString().Trim();
                model.inv_purchaserBank = jObject["inv_purchaserBank"].ToString().Trim();
                model.inv_purchaserBankNum = jObject["inv_purchaserBankNum"].ToString().Trim();
                model.inv_serviceType = jObject["inv_serviceType"].ToString();
                model.inv_serviceName = jObject["inv_serviceName"].ToString();

                if (!new BLL.Order().checkOrderCusID(model.inv_oid,true, inv_cid))
                {
                    context.Response.Write("{ \"msg\":\"订单中不存在该应收客户，不能添加发票\", \"status\":0 }");
                    return;
                }

                if (string.IsNullOrEmpty(jObject["inv_money"].ToString().Trim()))
                {
                    context.Response.Write("{ \"msg\":\"请填写开票金额\", \"status\":0 }");
                    return;
                }
                decimal _money = 0;
                if (!decimal.TryParse(jObject["inv_money"].ToString().Trim(), out _money))
                {
                    context.Response.Write("{ \"msg\":\"请正确填写开票金额\", \"status\":0 }");
                    return;
                }
                model.inv_money = _money;
                model.inv_sentWay = jObject["inv_sentWay"] == null ? "" : jObject["inv_sentWay"].ToString();
                model.inv_farea = managerModel.area;
                model.inv_darea = jObject["inv_darea"] == null ? "" : jObject["inv_darea"].ToString();
                model.inv_receiveName = jObject["inv_receiveName"] == null ? "" : jObject["inv_receiveName"].ToString().Trim();
                model.inv_receivePhone = jObject["inv_receivePhone"] == null ? "" : jObject["inv_receivePhone"].ToString().Trim();
                model.inv_receiveAddress = jObject["inv_receiveAddress"] == null ? "" : jObject["inv_receiveAddress"].ToString().Trim();
                model.inv_remark = jObject["inv_remark"] == null ? "" : jObject["inv_remark"].ToString().Trim();
                model.inv_personName = managerModel.real_name;
                model.inv_personNum = managerModel.user_name;
                model.inv_addDate = DateTime.Now;
                model.inv_flag1 = 0;
                model.inv_flag2 = 0;
                model.inv_flag3 = 0;
                model.inv_isConfirm = false;
                string result = bll.Add(model, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 绑定发票审批类型
        /// </summary>
        private void invoice_auditBind(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                StringBuilder sb = new StringBuilder();
                BLL.invoices bll = new BLL.invoices();

                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                int invid = jObject["invid"] == null ? 0 : Utils.ObjToInt(jObject["invid"], 0);
                if (invid == 0)
                {
                    context.Response.Write("{ \"msg\":\"参数有误\", \"status\":0 }");
                    return;
                }
                Model.invoices invApply = new BLL.invoices().GetModel(invid);
                if (invApply == null)
                {
                    context.Response.Write("{ \"msg\":\"记录不存在\", \"status\":0 }");
                    return;
                }
                string type = "";//1申请区域审批，2开票区域审批，3财务审批
                byte flag = 0;
                string remark = string.Empty;
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                if (invApply.inv_farea == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0603"))//部门审批
                {
                    type = "1";
                    flag = invApply.inv_flag1.Value;
                    remark = invApply.inv_checkRemark1;
                }
                else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0402"))//财务审批
                {
                    type = "2";
                    flag = invApply.inv_flag2.Value;
                    remark = invApply.inv_checkRemark2;
                }
                else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0601"))//总经理审批
                {
                    type = "3";
                    flag = invApply.inv_flag3.Value;
                    remark = invApply.inv_checkRemark3;
                }
                else
                {
                    context.Response.Write("{ \"msg\":\"您无权限管理非业务支付申请\", \"status\":0 }");
                    return;
                }

                context.Response.Write("{ \"msg\":\"success\", \"status\":1,\"type\":" + type + ",\"flag\":" + flag + ",\"remark\":\"" + remark + "\"}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\"" + ex.Message + "\", \"status\":0 }");
                return;
            }
        }

        /// <summary>
        /// 审批发票申请
        /// </summary>
        private void invoice_audit(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["inv_id"].ToString(), out int inv_id) || !byte.TryParse(jObject["ctype"].ToString(), out byte ctype)|| !byte.TryParse(jObject["cstatus"].ToString(), out byte cstatus))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string remark = Utils.ObjectToStr(jObject["remark"]);
                BLL.invoices bll = new BLL.invoices();
                string result = bll.checkInvoiceStatus(inv_id, ctype, cstatus, remark, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 确认发票是否已开票
        /// </summary>
        private void invoice_confirm(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["inv_id"].ToString(), out int _id) || !bool.TryParse(jObject["inv_isConfirm"].ToString(), out bool status))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string date = jObject["inv_date"] == null ? "" : jObject["inv_date"].ToString();
                BLL.invoices bll = new BLL.invoices();
                string result = bll.confirmInvoice(_id, status, date, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 业务支付审核分页列表
        /// </summary>
        private void pay_list(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["pageSize"].ToString(), out int pageSize) || !int.TryParse(jObject["pageIndex"].ToString(), out int pageIndex))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string _keywords = jObject["keywords"].ToString();//付款对象或凭证号
                //string _rp_flag = jObject["rp_flag"].ToString();
                //string _rp_isExpect = jObject["rp_isExpect"].ToString();//预收付款
                if (jObject["rp_flag"] == null || string.IsNullOrWhiteSpace(jObject["rp_flag"].ToString()) || jObject["rp_isExpect"] == null && string.IsNullOrWhiteSpace(jObject["rp_isExpect"].ToString()))
                {
                    context.Response.Write("{ \"msg\":\"传值参数不能为空\", \"status\":0 }");
                    return;
                }
                
                #region 筛选条件
                StringBuilder strTemp = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(_keywords))
                {
                    strTemp.Append(" and (c_name like '%" + _keywords + "%' or ce_num like '%" + _keywords + "%')");
                }
                if (int.Parse(jObject["rp_isExpect"].ToString()) == 1)
                {
                    strTemp.Append(" and rp_personNum='" + managerModel.real_name + "' and rp_personName='" + managerModel.user_name + "')");
                }
                else if (int.TryParse(jObject["rp_flag"].ToString(),out int rp_flag))
                {
                    if (new BLL.permission().checkHasPermission(managerModel, "0402"))
                    {

                        if (rp_flag == 0)
                        {
                            strTemp.Append(" and rp_flag=0");
                        }
                        else if (rp_flag == 1 || rp_flag == 2)
                        {
                            strTemp.Append(" and (rp_flag=1 or  rp_flag=2)");
                        }
                        strTemp.Append(" and rp_checkNum='" + managerModel.user_name + "' and rp_checkName='" + managerModel.real_name + "'");
                    }
                    else if (new BLL.permission().checkHasPermission(managerModel, "0601"))
                    {
                        if (rp_flag == 0)
                        {
                            strTemp.Append(" and rp_flag1=0");
                        }
                        else if (rp_flag == 1 || rp_flag == 2)
                        {
                            strTemp.Append(" and (rp_flag1=1 or  rp_flag1=2)");
                        }
                        strTemp.Append(" and rp_checkNum1='" + managerModel.user_name + "' and rp_checkName1='" + managerModel.real_name + "'");
                    }
                    else
                    {
                        context.Response.Write("{ \"msg\":\"您无权限管理业务支付申请\", \"status\":0 }");
                        return;
                    }
                }
                #endregion

                string _strWhere = "rp_type=0" + strTemp.ToString();

                BLL.ReceiptPay bll = new BLL.ReceiptPay();
                DataTable lst = bll.GetList(pageSize, pageIndex, _strWhere, "rp_adddate desc,rp_id desc", out int pageTotal, out decimal _tmoney, out decimal _tunmoney).Tables[0];
                if (lst != null && lst.Rows.Count > 0)
                {
                    context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"list\":");
                    context.Response.Write(JArray.FromObject(lst) + "}");
                    return;
                }
                context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 查看业务支付审核
        /// </summary>
        private void pay_show(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["rp_id"] == null || string.IsNullOrWhiteSpace(jObject["rp_id"].ToString()) || !int.TryParse(jObject["rp_id"].ToString(), out int id))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                BLL.ReceiptPay bll = new BLL.ReceiptPay();
                DataSet ds = bll.GetList(0, "rp_id=" + id + "", "");
                if (ds == null || ds.Tables[0].Rows.Count <= 0)
                {
                    context.Response.Write("{ \"msg\":\"业务支付审核信息不存在\", \"status\":0 }");
                    return;
                }
                JObject _object = new JObject(); 
                DataRow dr = ds.Tables[0].Rows[0];
                _object.Add("rp_id", dr["rp_id"].ToString());//收付款表自增主键
                _object.Add("c_id", dr["c_id"].ToString());//收付对象ID
                _object.Add("c_name", dr["c_name"].ToString());//收付对象
                _object.Add("rp_content", dr["rp_content"].ToString());//收付内容
                _object.Add("rp_money", dr["rp_money"].ToString());//收付金额
                _object.Add("pm_name", dr["pm_name"].ToString());//收付款方式
                _object.Add("rp_foredate", Convert.ToDateTime(dr["rp_foredate"]).ToString("yyyy-MM-dd"));//预计收付日期
                _object.Add("rp_method", dr["rp_method"].ToString());//收付款方式
                _object.Add("rp_date", dr["rp_date"].ToString()==""?"": ConvertHelper.toDate(dr["rp_date"]).Value.ToString("yyyy-MM-dd"));//实际收付款日期
                _object.Add("rp_isConfirm", Convert.ToBoolean(dr["rp_isConfirm"]).ToString());//实际收付款日期
                _object.Add("rp_cbid", Utils.ObjectToStr(dr["rp_cbid"]));
                _object.Add("bankName", Utils.ObjToInt(dr["rp_cbid"]) == 0 ? "" : Utils.ObjectToStr(dr["cb_bank"]) + "(" + Utils.ObjectToStr(dr["cb_bankName"]) + "/" + Utils.ObjectToStr(dr["cb_bankNum"]) + ")");
                string type = jObject["type"] == null ? "" : jObject["type"].ToString();
                if (type == "check")
                {
                    string checktype = "",checktypeText="", flag = "", remark = "";
                    if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                    {
                        context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                        return;
                    }

                    Model.manager managerModel = new BLL.manager().GetModel(managerid);
                    if (managerModel == null)
                    {
                        context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                        return;
                    }
                    if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0402"))//财务审批
                    {
                        checktype = "1";
                        checktypeText = "财务审批";
                        flag = dr["rp_flag"].ToString();
                        remark = Utils.ObjectToStr(dr["rp_checkRemark"]);
                    }
                    else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0601"))//总经理审批
                    {
                        checktype = "2";
                        checktypeText = "总经理审批";
                        flag = dr["rp_flag1"].ToString();
                        remark = Utils.ObjectToStr(dr["rp_checkRemark1"]);
                    }
                    else
                    {
                        context.Response.Write("{ \"msg\":\"您无权限审批业务支付\", \"status\":0 }");
                        return;
                    }
                    _object.Add("type", checktype);
                    _object.Add("typeText", checktypeText);
                    _object.Add("flag", flag);
                    _object.Add("flagText", BusinessDict.checkStatus()[Utils.ObjToByte(flag)]);
                    _object.Add("remark", remark);
                }
                context.Response.Write(_object);
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 审批业务支付
        /// </summary>
        private void pay_audit(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["rp_id"].ToString(), out int id) || !byte.TryParse(jObject["ctype"].ToString(), out byte ctype) || !byte.TryParse(jObject["status"].ToString(), out byte status))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }                
                string remark = jObject["remark"]==null?"": jObject["remark"].ToString();
                BLL.ReceiptPay bll = new BLL.ReceiptPay();
                string result = bll.checkPay(id, ctype, status, remark, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        // <summary>
        /// 确认支付
        /// </summary>
        private void pay_confirm(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["id"].ToString(), out int id) || !byte.TryParse(jObject["type"].ToString(), out byte type))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string date = jObject["date"].ToString();
                string status = jObject["status"].ToString();
                string method = jObject["method"].ToString();

                BLL.ReceiptPay bll = new BLL.ReceiptPay();
                string result = bll.confirmReceiptPay(Convert.ToInt32(id), status, date, method, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 添加收付款明细
        /// </summary>
        /// <param name="context"></param>
        private void add_receiptpayDetail(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string rpd_type = Utils.ObjectToStr(jObject["rpd_type"]);
                string rpd_oid = Utils.ObjectToStr(jObject["rpd_oid"]);
                int rpd_cid = Utils.ObjToInt(jObject["rpd_cid"],0);
                int bankID = Utils.ObjToInt(jObject["bankID"], 0);
                string rpd_content = Utils.ObjectToStr(jObject["rpd_content"]);
                decimal rpd_money = Utils.ObjToDecimal(jObject["rpd_money"], 0);
                string rpd_foredate = Utils.ObjectToStr(jObject["rpd_foredate"]);
                int rpd_method = Utils.ObjToInt(jObject["rpd_method"], 0);

                Model.ReceiptPayDetail model = new Model.ReceiptPayDetail();
                BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();

                model.rpd_type = Utils.StrToBool(rpd_type,false);
                model.rpd_oid = rpd_oid;
                model.rpd_cid = rpd_cid;
                model.rpd_content = rpd_content;
                model.rpd_money = rpd_money;
                model.rpd_foredate = ConvertHelper.toDate(rpd_foredate);
                model.rpd_method = rpd_method;
                model.rpd_personNum = managerModel.user_name;
                model.rpd_personName = managerModel.real_name;
                model.rpd_adddate = DateTime.Now;
                model.rpd_cbid = bankID;
                if (model.rpd_type.Value)
                {
                    model.rpd_flag1 = 2;
                }
                else
                {
                    model.rpd_flag1 = 0;
                    model.rpd_flag2 = 0;
                    model.rpd_flag3 = 0;
                }
                if (!new BLL.Order().checkOrderCusID(model.rpd_oid, model.rpd_type.Value, rpd_cid))
                {
                    context.Response.Write("{ \"msg\":\"订单中不存在该应收付客户，不能添加\", \"status\":0 }");
                    return;
                }
                string result = bll.AddReceiptPay(model, managerModel, out int id);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1,\"rpd_id\":" + id + "}");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 查询客户银行账号
        /// </summary>
        /// <param name="context"></param>
        private void get_bank(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                int cid = Utils.ObjToInt(jObject["cid"], 0);
                DataSet ds = new BLL.customerBank().GetList(cid);
                if (ds != null && ds.Tables.Count > 0)
                {
                    //context.Response.Write("{\"status\": 1,\"list\":");
                    //context.Response.Write(JArray.FromObject(ds.Tables[0]) + "}");
                    context.Response.Write(JArray.FromObject(ds.Tables[0]));
                    return;
                }
                context.Response.Write("{\"status\": 0,\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 订单结算汇总数据
        /// </summary>
        /// <param name="context"></param>
        private void get_settlementlist(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string oid = Utils.ObjectToStr(jObject["oid"]);
                DataTable dt = new BLL.Order().getOrderCollect(oid);
                if (dt != null && dt.Rows.Count > 0)
                {
                    decimal? finProfit = 0, fin1 = 0, fin0 = 0;
                    foreach (DataRow inv in dt.Rows)
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
                    context.Response.Write("{\"status\": 1,\"fin1\": "+ fin1 + ",\"fin0\": " + fin0 + ",\"finProfit\": " + finProfit + ",\"finCust\": " + Utils.StrToDecimal(dt.Rows[0]["o_financeCust"].ToString(), 0) + ",\"Profit\": " + (finProfit- Utils.StrToDecimal(dt.Rows[0]["o_financeCust"].ToString(), 0)) + ",\"list\":" + JArray.FromObject(dt) + "}");
                    return;
                }
                context.Response.Write("{\"status\": 1,\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 订单发票数据
        /// </summary>
        /// <param name="context"></param>
        private void get_InvoiceList(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                bool isExecutiver = false;//是否是执行人员
                string oid = Utils.ObjectToStr(jObject["oid"]);
                DataTable pdt = new BLL.Order().GetPersonList(0, "op_oid='" + oid + "'", "op_id asc").Tables[0];
                if (pdt != null && pdt.Rows.Count > 0)
                {
                    DataRow[] drs4 = pdt.Select("op_type=4 and op_number='" + managerModel.user_name + "'");//业务执行人员
                    if (drs4.Length > 0)
                    {
                        isExecutiver = true;
                    }
                }
                string sqlwhere = "";
                if (isExecutiver)
                {
                    sqlwhere = " and inv_personNum='" + managerModel.user_name + "'";
                }
                DataTable invoiceData = new BLL.invoices().GetList(0, "inv_oid='" + oid + "' " + sqlwhere + "", "inv_addDate desc,inv_id desc").Tables[0];
                if (invoiceData != null && invoiceData.Rows.Count > 0)
                {
                    decimal? requestMoney = 0, confirmMoney = 0, leftInvMoney = 0;
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
                    leftInvMoney = new BLL.invoices().computeInvoiceLeftMoney(oid);
                    context.Response.Write("{\"status\": 1,\"requestMoney\": " + requestMoney + ",\"confirmMoney\": " + confirmMoney + ",\"leftInvMoney\": " + leftInvMoney + ",\"list\":" + JArray.FromObject(invoiceData) + "}");
                    return;
                }
                context.Response.Write("{\"status\": 1,\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 执行备用金借款明细
        /// </summary>
        /// <param name="context"></param>
        private void get_unBusinessList(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                bool isExecutiver = false;//是否是执行人员
                string oid = Utils.ObjectToStr(jObject["oid"]);
                DataTable pdt = new BLL.Order().GetPersonList(0, "op_oid='" + oid + "'", "op_id asc").Tables[0];
                if (pdt != null && pdt.Rows.Count > 0)
                {
                    DataRow[] drs4 = pdt.Select("op_type=4 and op_number='" + managerModel.user_name + "'");//业务执行人员
                    if (drs4.Length > 0)
                    {
                        isExecutiver = true;
                    }
                }
                string sqlwhere = "";
                if (isExecutiver)
                {
                    sqlwhere = " and uba_PersonNum='" + managerModel.user_name + "'";
                }
                DataTable Data = new BLL.unBusinessApply().GetList(0, "uba_oid='" + oid + "' " + sqlwhere + "", "uba_addDate desc,uba_id desc").Tables[0];
                if (Data != null && Data.Rows.Count > 0)
                {
                    //context.Response.Write("{\"status\": 1,\"list\":" + JArray.FromObject(Data) + "}");
                    //return;
                    DataTable DataPic = new DataTable();
                    JArray list = new JArray();
                    foreach (JObject item in JArray.FromObject(Data))
                    {
                        DataPic = new BLL.payPic().GetList(2, "pp_type=2 and pp_rid=" + item["uba_id"] + "", "pp_addDate desc").Tables[0];
                        item["piclist"] = JArray.FromObject(DataPic);
                        list.Add(item);
                    }
                    context.Response.Write("{\"status\": 1,\"list\":" + list + "}");
                    return;
                }
                context.Response.Write("{\"status\": 1,\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 执行备用金借款明细附件
        /// </summary>
        /// <param name="context"></param>
        private void get_unBusinessPic(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                int id = Utils.ObjToInt(jObject["ubaid"]);
                DataTable Data = new BLL.payPic().GetList(2, "pp_type=2 and pp_rid=" + id + "", "pp_addDate desc").Tables[0];
                if (Data != null && Data.Rows.Count > 0)
                {
                    context.Response.Write("{\"status\": 1,\"list\":" + JArray.FromObject(Data) + "}");
                    return;
                }
                context.Response.Write("{\"status\": 1,\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }
        #endregion

        #region 财务管理
        /// <summary>
        /// 付款明细分页列表
        /// </summary>
        private void paydetail_list(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["pageSize"].ToString(), out int pageSize) || !int.TryParse(jObject["pageIndex"].ToString(), out int pageIndex))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                #region 筛选条件
                StringBuilder strTemp = new StringBuilder();
                string keywords = jObject["keywords"].ToString();//凭证号或收款对象
                string _type = jObject["type"] == null ? "" : jObject["type"].ToString();
                string _flag = jObject["flag"] == null ? "" : jObject["flag"].ToString();//flag:"0"待审批页签，"1"已审批页签
                keywords = keywords.Replace("'", "");
                if (!string.IsNullOrEmpty(keywords))
                {
                    strTemp.Append(" and (rpd_num like '%" + keywords + "%' or c_name like '%" + keywords + "%')");

                }
                #endregion

                #region 审核列表
                string checkType = "0";
                if (_type == "check")
                {
                    if (new BLL.permission().checkHasPermission(managerModel, "0603"))//部门审批
                    {
                        checkType = "1";
                        if (_flag == "0")
                        {
                            if (managerModel.area == new BLL.department().getGroupArea())
                            {
                                strTemp.Append(" and rpd_flag1=0 and rpd_flag2=0 and rpd_flag3=0");
                            }
                            else
                            {
                                strTemp.Append(" and rpd_area='" + managerModel.area + "' and rpd_flag1=0 and rpd_flag2=0 and rpd_flag3=0");
                            }
                        }
                        else if (_flag == "1")
                        {
                            strTemp.Append(" and rpd_area='" + managerModel.area + "' and (rpd_flag1=1 or  rpd_flag1=2)");
                        }
                    }
                    else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0402"))//财务审批
                    {
                        checkType = "2";
                        if (_flag == "0")
                        {
                            strTemp.Append(" and rpd_flag1=2 and rpd_flag2=0 and rpd_flag3=0");
                        }
                        else if (_flag == "1")
                        {
                            strTemp.Append(" and rpd_flag1=2 and (rpd_flag2=1 or  rpd_flag2=2)");
                        }
                    }
                    else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0601"))//总经理审批
                    {
                        checkType = "3";
                        if (_flag == "0")
                        {
                            strTemp.Append(" and rpd_flag1=2 and rpd_flag2=2 and rpd_flag3=0");
                        }
                        else if (_flag == "1")
                        {
                            strTemp.Append(" and rpd_flag1=2 and rpd_flag2=2 and (rpd_flag3=1 or  rpd_flag3=2)");
                        }
                    }
                    else
                    {
                        context.Response.Write("{ \"msg\":\"无权限\", \"status\":0 }");
                        return;
                    }
                }
                else
                {
                    strTemp.Append(" and rpd_personNum='" + managerModel.user_name + "'");
                }
                #endregion

                string _strWhere = "rpd_type=0" + strTemp.ToString();

                BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
                DataTable lst = bll.GetList(pageSize, pageIndex, _strWhere, "rpd_adddate desc,rpd_id desc", managerModel, out int pageTotal, out decimal _tmoney).Tables[0];
                if (lst != null && lst.Rows.Count > 0)
                {
                    context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"checkType\":" + checkType + ",\"list\":");
                    context.Response.Write(JArray.FromObject(lst) + "}");
                    return;
                }
                context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"checkType\":" + checkType + ",\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 付款明细详细信息
        /// </summary>
        private void paydetail_show(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["rpd_id"] == null || string.IsNullOrWhiteSpace(jObject["rpd_id"].ToString()) || !int.TryParse(jObject["rpd_id"].ToString(), out int id))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
                DataSet ds = bll.GetList(0, "rpd_id=" + id + "", "");
                if (ds == null || ds.Tables[0].Rows.Count <= 0)
                {
                    context.Response.Write("{ \"msg\":\"付款明细信息不存在\", \"status\":0 }");
                    return;
                }

                JObject _object = new JObject();
                DataRow dr = ds.Tables[0].Rows[0];
                _object.Add("c_name", dr["c_name"].ToString());
                _object.Add("rpd_id", dr["rpd_id"].ToString());
                _object.Add("rpd_oid", dr["rpd_oid"].ToString());
                _object.Add("rpd_cid", dr["rpd_cid"].ToString());
                _object.Add("rpd_money", dr["rpd_money"].ToString());
                _object.Add("rpd_foredate", Convert.ToDateTime(dr["rpd_foredate"]).ToString("yyyy-MM-dd"));
                _object.Add("rpd_content", dr["rpd_content"].ToString());
                _object.Add("rpd_cbid", Utils.ObjectToStr(dr["rpd_cbid"]));
                _object.Add("bankName", Utils.ObjToInt(dr["rpd_cbid"])==0?"": Utils.ObjectToStr(dr["cb_bank"]) + "(" + Utils.ObjectToStr(dr["cb_bankName"]) + "/" + Utils.ObjectToStr(dr["cb_bankNum"]) + ")");
                DataTable dt = new BLL.payPic().GetList(1, "pp_type=1 and pp_rid=" + id + "", "pp_addDate desc").Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    _object.Add("albumlist", JArray.FromObject(dt));
                }
                else
                {
                    _object.Add("albumlist", null);
                }

                string type = jObject["type"] == null ? "" : jObject["type"].ToString();
                if (type == "check")
                {
                    string checktype = "", flag = "", remark = "";
                    if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                    {
                        context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                        return;
                    }

                    Model.manager managerModel = new BLL.manager().GetModel(managerid);
                    if (managerModel == null)
                    {
                        context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                        return;
                    }
                    if (dr["rpd_area"].ToString() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0603"))//部门审批
                    {
                        checktype = "1";
                        flag = dr["rpd_flag1"].ToString();
                        remark = Utils.ObjectToStr(dr["rpd_checkRemark1"]);
                    }
                    else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0402"))//财务审批
                    {
                        checktype = "2";
                        flag = dr["rpd_flag2"].ToString();
                        remark = Utils.ObjectToStr(dr["rpd_checkRemark2"]);
                    }
                    else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0601"))//总经理审批
                    {
                        checktype = "3";
                        flag = dr["rpd_flag3"].ToString();
                        remark = Utils.ObjectToStr(dr["rpd_checkRemark3"]);
                    }
                    else
                    {
                        context.Response.Write("{ \"msg\":\"您无权限审批业务支付\", \"status\":0 }");
                        return;
                    }
                    _object.Add("type", checktype);
                    _object.Add("flag", flag);
                    _object.Add("remark", remark);
                }
                
                context.Response.Write(_object);
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 审批付款明细
        /// </summary>
        /// <param name="context"></param>
        private void paydetail_audit(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                int rpdid = Utils.ObjToInt(jObject["rpdid"], 0);
                string type = Utils.ObjectToStr(jObject["type"]);
                string status = Utils.ObjectToStr(jObject["status"]);
                string remark = Utils.ObjectToStr(jObject["remark"]);

                BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
                string result = bll.checkPayDetailStatus(rpdid, Utils.ObjToByte(type), Utils.ObjToByte(status), remark, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }
        #endregion

        #region 通知管理

        /// <summary>
        ///  收款通知分页列表
        /// </summary>
        private void receipt_list(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["pageSize"].ToString(), out int pageSize) || !int.TryParse(jObject["pageIndex"].ToString(), out int pageIndex) || !byte.TryParse(jObject["rp_isconfirm"].ToString(), out byte rp_isConfirm))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                

                #region 筛选条件
                StringBuilder strTemp = new StringBuilder(" and rp_isConfirm=" + rp_isConfirm + "");
                string keywords = jObject["keywords"].ToString();//凭证号或收款对象
                keywords = keywords.Replace("'", "");
                if (!string.IsNullOrEmpty(keywords))
                {
                    strTemp.Append(" and (ce_num like '%" + keywords + "%' or c_name like '%" + keywords + "%')");

                }
                #endregion

                string _strWhere = "rp_type=1 and rp_personNum='" + managerModel.user_name + "' " + strTemp.ToString();

                BLL.ReceiptPay bll = new BLL.ReceiptPay();
                DataTable lst = bll.GetList(pageSize, pageIndex, _strWhere, "isnull(rp_date,'3000-01-01') desc,isnull(pm_sort,-1) asc,rp_id desc", out int pageTotal, out decimal _tmoney, out decimal _tunmoney).Tables[0];
                if (lst != null && lst.Rows.Count > 0)
                {
                    context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"list\":");
                    context.Response.Write(JArray.FromObject(lst) + "}");
                    return;
                }
                context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 付款通知分页列表
        /// </summary>
        private void payment_list(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["pageSize"].ToString(), out int pageSize) || !int.TryParse(jObject["pageIndex"].ToString(), out int pageIndex))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string keywords = jObject["keywords"]==null?"": jObject["keywords"].ToString();//凭证号或收款对象
                string rptype = Utils.ObjectToStr(jObject["rptype"]);//1收款，0付款
                if (string.IsNullOrEmpty(rptype))
                {
                    rptype = "0";
                }
                string isRefund = Utils.ObjectToStr(jObject["isRefund"]);//1退款
                string isExpect= jObject["isExpect"] == null ? "" : jObject["isExpect"].ToString();//预付款
                string _type = jObject["type"] == null ? "" : jObject["type"].ToString();
                string _flag = jObject["flag"] == null ? "" : jObject["flag"].ToString();//flag:"0"待审批页签，"1"已审批页签
                
                #region 筛选条件
                StringBuilder strTemp = new StringBuilder();
                keywords = keywords.Replace("'", "");
                if (!string.IsNullOrEmpty(keywords))
                {
                    strTemp.Append(" and (ce_num like '%" + keywords + "%' or c_name like '%" + keywords + "%')");

                }
                if (!string.IsNullOrEmpty(isExpect))
                {
                    strTemp.Append(" and rp_isExpect='" + isExpect + "'");
                }
                if (!string.IsNullOrEmpty(isRefund) && isRefund == "1")
                {
                    strTemp.Append(" and rp_money < 0");
                }
                #endregion

                #region 审核列表
                string checkType = "0";
                if (_type == "check")
                {
                    if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0402"))//财务审批
                    {
                        checkType = "1";
                        if (_flag == "0")
                        {
                            strTemp.Append(" and rp_flag=0 and rp_flag1=0 and rp_isConfirm='False'");
                        }
                        else if (_flag == "1")
                        {
                            strTemp.Append(" and (rp_flag=1 or  rp_flag=2)");
                        }
                    }
                    else if (new BLL.department().getGroupArea() == managerModel.area && new BLL.permission().checkHasPermission(managerModel, "0601"))//总经理审批
                    {
                        checkType = "2";
                        if (_flag == "0")
                        {
                            strTemp.Append(" and rp_flag=2 and rp_flag1=0 and rp_isConfirm='False'");
                        }
                        else if (_flag == "1")
                        {
                            strTemp.Append(" and rp_flag=2 and (rp_flag1=1 or  rp_flag1=2)");
                        }
                    }
                    else
                    {
                        context.Response.Write("{ \"msg\":\"无权限\", \"status\":0 }");
                        return;
                    }
                }
                #endregion

                string _strWhere = "rp_type="+ rptype + "" + strTemp.ToString();

                BLL.ReceiptPay bll = new BLL.ReceiptPay();
                DataTable lst = bll.GetList(pageSize, pageIndex, _strWhere, "isnull(rp_date,'3000-01-01') desc,isnull(pm_sort,-1) asc,rp_id desc", out int pageTotal, out decimal _tmoney, out decimal _tunmoney).Tables[0];
                if (lst != null && lst.Rows.Count > 0)
                {
                    context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"checkType\":" + checkType + ",\"list\":");
                    context.Response.Write(JArray.FromObject(lst) + "}");
                    return;
                }
                context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"checkType\":" + checkType + ",\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 添加编辑收付款通知
        /// </summary>
        /// <param name="context"></param>
        private void receiptpay_add(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                                
                string rptype = jObject["rptype"] == null ? "" : jObject["rptype"].ToString();
                string rpcid = jObject["rpcid"] == null ? "" : jObject["rpcid"].ToString();
                string rpmoney = jObject["rpmoney"] == null ? "" : jObject["rpmoney"].ToString();
                string rpforedate = jObject["rpforedate"] == null ? "" : jObject["rpforedate"].ToString();
                string rpmethod = jObject["rpmethod"] == null ? "" : jObject["rpmethod"].ToString();
                string rpcontent = jObject["rpcontent"] == null ? "" : jObject["rpcontent"].ToString();

                Model.ReceiptPay model = new Model.ReceiptPay();
                BLL.ReceiptPay bll = new BLL.ReceiptPay();
                int rpid = jObject["rpid"] == null ? 0 : Utils.ObjToInt(jObject["rpid"], 0);
                string _content = string.Empty;
                if (rpid > 0)
                {
                    model = bll.GetModel(rpid);
                    if (model.rp_cid.ToString() != rpcid)
                    {
                        _content += "收款对象ID：" + model.rp_cid + "→<font color='red'>" + rpcid + "</font><br/>";
                    }
                    if (model.rp_money.ToString() != rpmoney)
                    {
                        _content += "收款金额：" + model.rp_money + "→<font color='red'>" + rpmoney + "</font><br/>";
                    }
                    if (model.rp_foredate.Value.ToString("yyyy-MM-dd") != rpforedate)
                    {
                        _content += "预收日期：" + model.rp_foredate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + rpforedate + "</font><br/>";
                    }
                    if (model.rp_method.ToString() != rpmethod)
                    {
                        _content += "收款方式ID：" + model.rp_method.ToString() + "→<font color='red'>" + rpmethod + "</font><br/>";
                    }
                    if (model.rp_content != rpcontent)
                    {
                        _content += "收款内容：" + model.rp_content + "→<font color='red'>" + rpcontent + "</font><br/>";
                    }
                }

                model.rp_type = Utils.StrToBool(rptype, false);
                model.rp_cid = Utils.StrToInt(rpcid, 0);
                model.rp_money = Utils.StrToDecimal(rpmoney, 0);
                model.rp_foredate = ConvertHelper.toDate(rpforedate);
                model.rp_method = Utils.StrToInt(rpmethod, 0);
                model.rp_content = rpcontent;
                string result = string.Empty;
                if (rpid == 0)
                {
                    result = bll.Add(model, managerModel, "", "", out rpid, true);
                }
                else
                {
                    result = bll.Update(model, _content, managerModel, "", "");
                }
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }            
        }
        
        /// <summary>
        /// 开票通知分页列表
        /// </summary>
        private void bill_list(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["pageSize"].ToString(), out int pageSize) || !int.TryParse(jObject["pageIndex"].ToString(), out int pageIndex)|| !int.TryParse(jObject["inv_isConfirm"].ToString(), out int inv_isConfirm))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                #region 筛选条件
                StringBuilder strTemp = new StringBuilder(" and (inv_personNum='" + managerModel.user_name + "' and inv_personName='" + managerModel.real_name + "') and inv_isConfirm = " + inv_isConfirm + "");
                //string inv_isConfirm = jObject["inv_isConfirm"].ToString();//是否开票：0未开，1已开
                string keywords = jObject["keywords"].ToString();//凭证号或收款对象
                keywords = keywords.Replace("'", "");
                if (!string.IsNullOrEmpty(keywords))
                {
                    strTemp.Append(" and (inv_oid like '%" + keywords + "%' or c_name like '%" + keywords + "%')");
                }
                #endregion
                string _strWhere = "inv_id>0 and inv_personNum='" + managerModel.user_name + "'" + strTemp.ToString();

                BLL.invoices bll = new BLL.invoices();
                DataTable lst = bll.GetList(pageSize, pageIndex, _strWhere, "inv_addDate desc,inv_id desc", managerModel, out int pageTotal,out decimal _tmoney).Tables[0];
                if (lst != null && lst.Rows.Count > 0)
                {
                    context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"list\":");
                    context.Response.Write(JArray.FromObject(lst) + "}");
                    return;
                }
                context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        #endregion
        
        #region 个人业务结算

        #endregion

        #region 客户管理

        /// <summary>
        /// 客户列表
        /// </summary>
        private void customer_list(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["pageSize"].ToString(), out int pageSize) || !int.TryParse(jObject["pageIndex"].ToString(), out int pageIndex))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string _keywords = jObject["keywords"].ToString();
                string _type = jObject["type"].ToString();

                #region 筛选条件
                StringBuilder strTemp = new StringBuilder();
                _keywords = _keywords.Replace("'", "");
                if (!string.IsNullOrEmpty(_keywords))
                {
                    strTemp.Append(" and (c_name like  '%" + _keywords + "%')");
                }
                if (!string.IsNullOrEmpty(_type))
                {
                    strTemp.Append(" and c_type='" + _type + "'");
                }
                #endregion

                string _strWhere = "c_id>0" + strTemp.ToString();

                BLL.Customer bll = new BLL.Customer();
                DataTable lst = bll.GetList(pageSize, pageIndex, _strWhere, "c_isUse desc,c_addDate desc,c_id desc", managerModel, out int pageTotal, true, false).Tables[0];
                if (lst != null && lst.Rows.Count > 0)
                {
                    context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"list\":");
                    context.Response.Write(JArray.FromObject(lst) + "}");
                    return;
                }
                context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"pageTotal\":" + pageTotal + ",\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }
        
        /// <summary>
        /// 查看客户详情
        /// </summary>
        private void customer_show(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["c_id"] == null || string.IsNullOrWhiteSpace(jObject["c_id"].ToString()) || !int.TryParse(jObject["c_id"].ToString(), out int cid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.Customer model = new BLL.Customer().GetModel(cid);
                if (model == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"该客户信息不存在\"}");
                    return;
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(JObject.FromObject(model));
                sb.Remove(sb.ToString().LastIndexOf('}'), 1);
                sb.Append(",\"contacts_list\":");

                DataTable dt = new BLL.Contacts().GetList(0, "co_cid=" + cid + "", "co_flag desc,co_id asc").Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    sb.Append(JArray.FromObject(dt));
                }
                else
                {
                    sb.Append("[]");
                }
                sb.Append(",\"banks_list\":");
                DataTable bankDt = new BLL.customerBank().GetList(0, "cb_cid=" + cid + "", "cb_id asc").Tables[0];
                if (bankDt != null && bankDt.Rows.Count > 0)
                {
                    sb.Append(JArray.FromObject(bankDt));
                }
                else
                {
                    sb.Append("[]");
                }
                sb.Append("}");
                context.Response.Write(sb.ToString());
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }
               
        /// <summary>
        /// 获取客户详情
        /// </summary>
        private void get_customerById(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["c_id"] == null || string.IsNullOrWhiteSpace(jObject["c_id"].ToString()) || !int.TryParse(jObject["c_id"].ToString(), out int cid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.Customer model = new BLL.Customer().GetModel(cid);
                if (model == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"该客户信息不存在\"}");
                    return;
                }
                context.Response.Write(JObject.FromObject(model));
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 添加客户信息
        /// </summary>
        private void customer_add(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !byte.TryParse(jObject["c_type"].ToString(), out byte c_type) || !bool.TryParse(jObject["c_isUse"].ToString(), out bool c_isUse))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                
                Model.Customer model = new Model.Customer();
                Model.Contacts contact = new Model.Contacts();
                BLL.Customer bll = new BLL.Customer();

                if (jObject["c_name"] == null || string.IsNullOrWhiteSpace(jObject["c_name"].ToString()))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"客户名称不可为空\"}");
                    return;
                }
                model.c_name = jObject["c_name"].ToString();
                model.c_type = c_type;
                model.c_num = jObject["c_num"] == null ? "" : jObject["c_num"].ToString();
                model.c_isUse = c_isUse;
                model.c_remarks = jObject["c_remarks"] == null ? "" : jObject["c_remarks"].ToString();
                model.c_flag = 0;
                model.c_owner = managerModel.user_name;
                model.c_ownerName = managerModel.real_name;
                model.c_addDate = DateTime.Now;
                contact.co_flag = true;

                if (jObject["co_name"] == null || string.IsNullOrWhiteSpace(jObject["co_name"].ToString()))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"主要联系人不可为空\"}");
                    return;
                }
                contact.co_name = jObject["co_name"].ToString();
                if (jObject["co_number"] == null || string.IsNullOrWhiteSpace(jObject["co_number"].ToString()))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"主要联系人号码不可为空\"}");
                    return;
                }
                contact.co_number = jObject["co_number"].ToString();
                if (model.c_type == 2 || model.c_type == 3)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"只能添加普通客户\"}");
                    return;
                }
                string result = bll.Add(model, contact, managerModel, out int cid);

                if (result != "")
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"" + result + "\"}");
                }
                else
                {
                    context.Response.Write("{\"status\": 1, \"msg\": \"Success\"}");
                }
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 编辑客户详情
        /// </summary>
        private void customer_edit(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["c_id"].ToString(), out int c_id) || !byte.TryParse(jObject["c_type"].ToString(), out byte c_type) || !bool.TryParse(jObject["c_isUse"].ToString(), out bool c_isUse))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                //int managerid = jObject.GetValue("managerid").ToObject<int>();
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                BLL.Customer bll = new BLL.Customer();
                Model.Customer model = bll.GetModel(c_id);
                if (model == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"该客户信息不存在\"}");
                    return;
                }

                StringBuilder sb = new StringBuilder();
                byte oldtype = model.c_type.Value;
                if (jObject["c_name"] == null || string.IsNullOrWhiteSpace(jObject["c_name"].ToString()))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"客户名称不可为空\"}");
                    return;
                }
                string c_name = jObject["c_name"].ToString();
                if (model.c_name != c_name)
                {
                    sb.Append("客户名称:" + model.c_name + "→<font color='red'>" + c_name + "<font><br/>");
                }
                model.c_name = c_name;

                if (model.c_type != c_type)
                {
                    sb.Append("客户类别:" + Common.BusinessDict.customerType()[model.c_type] + "→<font color='red'>" + Common.BusinessDict.customerType()[c_type] + "<font><br/>");
                }
                model.c_type = c_type;

                string c_num = jObject["c_num"] == null ? "" : jObject["c_num"].ToString();
                if (model.c_num != c_num)
                {
                    sb.Append("信用代码(税号):" + model.c_num + "→<font color='red'>" + c_num + "<font><br/>");
                }
                model.c_num = c_num;

                if (model.c_isUse != c_isUse)
                {
                    sb.Append("启用状态:" + Common.BusinessDict.isUseStatus()[model.c_isUse] + "→<font color='red'>" + Common.BusinessDict.isUseStatus()[c_isUse] + "<font><br/>");
                }
                model.c_isUse = c_isUse;

                string remark = jObject["c_remarks"] == null ? "" : jObject["c_remarks"].ToString();
                if (model.c_remarks != remark)
                {
                    sb.Append("备注:" + model.c_remarks + "→<font color='red'>" + remark + "<font><br/>");
                }
                model.c_remarks = remark;

                string result = bll.Update(oldtype, model, managerModel, sb.ToString());

                if (result != "")
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"" + result + "\"}");
                }
                else
                {
                    context.Response.Write("{\"status\": 1, \"msg\": \"Success\"}");
                }
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 删除客户
        /// </summary>
        private void customer_del(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["c_id"].ToString(), out int c_id))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                //ChkAdminLevel(context, "sys_customer_list", DTEnums.ActionEnum.Delete.ToString(), managerModel);//检查权限                
                string result = new BLL.Customer().Delete(c_id, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 获取客户详情
        /// </summary>
        private void get_contactById(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["co_id"] == null || string.IsNullOrWhiteSpace(jObject["co_id"].ToString()) || !int.TryParse(jObject["co_id"].ToString(), out int co_id))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.Contacts model = new BLL.Contacts().GetModel(co_id);
                if (model == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"该联系人信息不存在\"}");
                    return;
                }
                context.Response.Write(JObject.FromObject(model));
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 添加次要联系人
        /// </summary>
        private void contact_add(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["c_id"].ToString(), out int co_cid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                Model.Contacts model = new Model.Contacts();
                BLL.Contacts bll = new BLL.Contacts();

                model.co_cid = co_cid;
                model.co_flag = false;
                if (jObject["co_name"] == null || string.IsNullOrWhiteSpace(jObject["co_name"].ToString()))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"联系人姓名不可为空\"}");
                    return;
                }
                model.co_name = jObject["co_name"].ToString();
                if (jObject["co_number"] == null || string.IsNullOrWhiteSpace(jObject["co_number"].ToString()))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"联系人号码不可为空\"}");
                    return;
                }
                model.co_number = jObject["co_number"].ToString();
                string result = bll.Add(model, managerModel);

                if (result != "")
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"" + result + "\"}");
                }
                else
                {
                    context.Response.Write("{\"status\": 1, \"msg\": \"Success\"}");
                }
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 编辑主、次要联系人
        /// </summary>
        private void contact_edit(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["co_id"].ToString(), out int co_id))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                //int managerid = jObject.GetValue("managerid").ToObject<int>();
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                BLL.Contacts bll = new BLL.Contacts();
                Model.Contacts model = bll.GetModel(co_id);
                if (model == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"该联系人信息不存在\"}");
                    return;
                }

                StringBuilder sb = new StringBuilder();
                if (jObject["co_name"] == null || string.IsNullOrWhiteSpace(jObject["co_name"].ToString()))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"联系人姓名不可为空\"}");
                    return;
                }
                string co_name = jObject["co_name"].ToString();
                if (model.co_name != co_name)
                {
                    sb.Append("联系人:" + model.co_name + "→<font color='red'>" + co_name + "<font><br/>");
                }
                model.co_name = co_name;
                
                if (jObject["co_number"] == null || string.IsNullOrWhiteSpace(jObject["co_number"].ToString()))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"联系人号码不可为空\"}");
                    return;
                }
                string co_number = jObject["co_number"].ToString();
                if (model.co_number != co_number)
                {
                    sb.Append("联系号码:" + model.co_number + "→<font color='red'>" + co_number + "<font><br/>");
                }
                model.co_number = co_number;

                string result = bll.Update(model, managerModel, sb.ToString());

                if (result != "")
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"" + result + "\"}");
                }
                else
                {
                    context.Response.Write("{\"status\": 1, \"msg\": \"Success\"}");
                }
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }
        
        /// <summary>
        /// 删除次要联系人
        /// </summary>
        private void contact_del(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["co_id"].ToString(), out int co_id))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
          
                string result = new BLL.Contacts().Delete(co_id, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 添加编辑银行账号
        /// </summary>
        /// <param name="context"></param>
        private void bank_Edit(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                int cb_id = Utils.ObjToInt(jObject["cb_id"], 0);
                int cb_cid = Utils.ObjToInt(jObject["cb_cid"], 0);
                string cb_bankName = Utils.ObjectToStr(jObject["cb_bankName"]);
                string cb_bankNum = Utils.ObjectToStr(jObject["cb_bankNum"]);
                string cb_bank = Utils.ObjectToStr(jObject["cb_bank"]);
                string cb_bankAddress = Utils.ObjectToStr(jObject["cb_bankAddress"]);
                bool cb_flag = Utils.StrToBool(Utils.ObjectToStr(jObject["cb_flag"]), false);

                BLL.customerBank bll = new BLL.customerBank();
                Model.customerBank model = new Model.customerBank();
                string _content = string.Empty, result = string.Empty;
                if (cb_id > 0)
                {
                    model = bll.GetModel(cb_id);
                    if (model.cb_bankName != cb_bankName)
                    {
                        _content += "银行账户名称:" + model.cb_bankName + "→<font color='red'>" + cb_bankName + "</font><br/>";
                    }
                    model.cb_bankName = cb_bankName;
                    if (model.cb_bankNum != cb_bankNum)
                    {
                        _content += "客户银行账号:" + model.cb_bankNum + "→<font color='red'>" + cb_bankNum + "</font><br/>";
                    }
                    model.cb_bankNum = cb_bankNum;
                    if (model.cb_bank != cb_bank)
                    {
                        _content += "开户行:" + model.cb_bank + "→<font color='red'>" + cb_bank + "</font><br/>";
                    }
                    model.cb_bank = cb_bank;
                    if (model.cb_bankAddress != cb_bankAddress)
                    {
                        _content += "开户地址:" + model.cb_bankAddress + "→<font color='red'>" + cb_bankAddress + "</font><br/>";
                    }
                    model.cb_bankAddress = cb_bankAddress;
                    if (model.cb_flag != cb_flag)
                    {
                        _content += "状态:" + (model.cb_flag.Value ? "启用" : "禁用") + "→<font color='red'>" + (cb_flag ? "启用" : "禁用") + "</font><br/>";
                    }
                    model.cb_flag = cb_flag;

                    result = bll.Update(model, _content, managerModel);
                }
                else
                {
                    model.cb_cid = cb_cid;
                    model.cb_bankName = cb_bankName;
                    model.cb_bankNum = cb_bankNum;
                    model.cb_bank = cb_bank;
                    model.cb_bankAddress = cb_bankAddress;
                    model.cb_flag = cb_flag;
                    result = bll.Add(model, managerModel);
                }                
                if (result != "")
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"" + result + "\"}");
                }
                else
                {
                    context.Response.Write("{\"status\": 1, \"msg\": \"Success\"}");
                }
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }

        /// <summary>
        /// 删除银行账号
        /// </summary>
        /// <param name="context"></param>
        private void bank_Del(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);

                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["cb_id"].ToString(), out int cb_id))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }

                string result = new BLL.customerBank().Delete(cb_id, managerModel);
                if (string.IsNullOrEmpty(result))
                {
                    context.Response.Write("{ \"msg\":\"success\", \"status\":1 }");
                    return;
                }
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":0 }");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }
        }
        #endregion

        #region 个人业务结算

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查管理员权限
        /// </summary>
        /// <param name="nav_name">菜单名称</param>
        /// <param name="action_type">操作类型</param>
        private void ChkAdminLevel(HttpContext context, string nav_name, string action_type, Model.manager managerModel)
        {
            Model.manager model = managerModel;
            BLL.manager_role bll = new BLL.manager_role();
            bool result = bll.Exists(model.role_id, nav_name, action_type);

            if (!result)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"您没有管理该页面的权限，请勿非法进入！\"}");
                return;
            }
        }

        /// <summary>
        /// 初始化接口调用回传参数公共方法
        /// </summary>
        private void get_params(HttpContext context, out JObject j)
        {
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string payload = stream.ReadToEnd();
            j = JObject.Parse(payload);
            if (j == null)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"ParameterIsNull\"}");
                return;
            }            
        }

        /// <summary>
        /// 字典类型转化为json字符串公用方法
        /// </summary>
        private StringBuilder init_dictionary(HttpContext context, Dictionary<string, string> dict)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                //if (jo["ddkey"] == null || string.IsNullOrWhiteSpace(jo["ddkey"].ToString()) || jo["ddkey"].ToString() != "dingzreafyvgzklylomj")
                //{
                //    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                //    return null;
                //}

                sb.Append("[");
                Dictionary<string, string> lst = dict;
                if (lst != null && lst.Count > 0)
                {
                    foreach (KeyValuePair<string, string> kvp in lst)
                    {
                        sb.Append("{\"key\":\"" + kvp.Key + "\",\"value\":\"" + kvp.Value + "\"},");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("]");
                }
                return sb;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return null;
            }
        }

        private StringBuilder init_dictionary(HttpContext context, Dictionary<byte?, string> dict)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("[");
                Dictionary<byte?, string> lst = dict;
                if (lst != null && lst.Count > 0)
                {
                    foreach (KeyValuePair<byte?, string> kvp in lst)
                    {
                        sb.Append("{\"key\":\"" + kvp.Key + "\",\"value\":\"" + kvp.Value + "\"},");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("]");
                }
                return sb;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return null;
            }
        }

        private StringBuilder init_dictionary(HttpContext context, Dictionary<byte, string> dict)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("[");
                Dictionary<byte, string> lst = dict;
                if (lst != null && lst.Count > 0)
                {
                    foreach (KeyValuePair<byte, string> kvp in lst)
                    {
                        sb.Append("{\"key\":\"" + kvp.Key + "\",\"value\":\"" + kvp.Value + "\"},");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("]");
                }
                return sb;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return null;
            }
        }

        private StringBuilder init_dictionary(HttpContext context, Dictionary<bool?, string> dict)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("[");
                Dictionary<bool?, string> lst = dict;
                if (lst != null && lst.Count > 0)
                {
                    foreach (KeyValuePair<bool?, string> kvp in lst)
                    {
                        sb.Append("{\"key\":\"" + kvp.Key + "\",\"value\":\"" + kvp.Value + "\"},");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("]");
                }
                return sb;
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return null;
            }
        }
        #endregion

        #region 上传文件处理===================================
        private void UpLoadFile(HttpContext context)
        {
            try
            {
                //get_params(context, out jObject);
                if (context.Request["managerid"] == null || string.IsNullOrWhiteSpace(context.Request["managerid"].ToString()) || !int.TryParse(context.Request["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                int type = Utils.ObjToInt(context.Request["type"], 0);//1订单文件，2非业务申请和付款明细的文件
                if (type != 1 && type != 2)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"typeError\"}");
                    return;
                }
                string keyID = Utils.ObjectToStr(context.Request["keyID"]);//type=1时传订单id；type=2时传付款明细或非业务申请主键ID
                if (string.IsNullOrEmpty(keyID))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"keyIDNull\"}");
                    return;
                }
                int pid = 0;
                if (type == 2 && !int.TryParse(keyID, out pid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"keyIDError\"}");
                    return;
                }
                string fileType = Utils.ObjectToStr(context.Request["fileType"]);//type=1时：1一类订单文件，2二类订单文件；type=2时：1付款明细文件，2非业务申请文件
                if (string.IsNullOrEmpty(fileType))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"fileTypeError\"}");
                    return;
                }                
                if (context.Request.Files.Count<1)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"请选择要上传文件\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                UpLoad upLoad = new UpLoad();
                string msg = string.Empty;
                int fileID = 0, scount = 0;
                JArray ja = new JArray();                
                for (int i = 0; i < context.Request.Files.Count; i++)
                {
                    byte[] byteData = FileHelper.ConvertStreamToByteBuffer(context.Request.Files[i].InputStream);
                    string fileName = context.Request.Files[i].FileName;
                    fileName = fileName.Replace(" ", "");//去掉空格
                    if (type == 1)
                    {
                        msg = upLoad.OrderFileSaveAs(byteData, fileName, keyID, fileType);
                    }
                    else
                    {                        
                        msg = upLoad.PayFileSaveAs(byteData, fileName, false, pid, fileType);
                    }
                    msg = Regex.Replace(msg, @"(\\[^bfrnt\\/'\""])", "\\$1");//利用正则表达式先把待解析的字符串中的带“\”特殊字符处理，再进行解析操作
                    JObject jo = JObject.Parse(msg);
                    if (jo["status"].ToString() == "1")
                    {
                        scount++;
                        if (type == 1)
                        {
                            Model.Files file = new Model.Files();
                            file.f_oid = keyID;
                            file.f_type = Utils.ObjToByte(fileType);
                            file.f_fileName = fileName;
                            file.f_filePath = jo["path"].ToString();
                            file.f_size = Utils.ObjToDecimal(jo["size"].ToString(), 0);
                            file.f_addDate = DateTime.Now;
                            file.f_addName = managerModel.real_name;
                            file.f_addPerson = managerModel.user_name;
                            fileID = new BLL.Order().insertOrderFile(file, managerModel);
                        }
                        else
                        {
                            Model.payPic file = new Model.payPic();
                            file.pp_rid = pid;
                            file.pp_type = Utils.ObjToByte(fileType);
                            file.pp_fileName = fileName;
                            file.pp_filePath = jo["path"].ToString();
                            file.pp_thumbFilePath = jo["thumb"].ToString();
                            file.pp_size = Utils.ObjToDecimal(jo["size"].ToString(), 0);
                            file.pp_addDate = DateTime.Now;
                            file.pp_addName = managerModel.real_name;
                            file.pp_addPerson = managerModel.user_name;
                            fileID = new BLL.payPic().insertPayFile(file, managerModel);
                        }
                        jo.Add("fileID", fileID);
                    }                  
                    ja.Add(jo);
                }
                JObject result = new JObject();
                if (scount == context.Request.Files.Count)
                {
                    context.Response.Write("{\"status\": 1, \"msg\": \"success\",\"list\":" + ja + "}");
                    return;
                }
                else if (scount == 0)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"附件上传失败\"}");
                    return;
                }
                else {
                    context.Response.Write("{\"status\": 0,\"count\":"+ scount + ",\"msg\": \"部分附件上传失败\"}");
                    return;
                }
                
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }            
        }

        /// <summary>
        /// 删除订单文件
        /// </summary>
        /// <param name="context"></param>
        private void File_delete(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                int fileID = Utils.ObjToInt(jObject["fileID"], 0);
                if (fileID == 0)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"fileIDIsNull\"}");
                    return;
                }
                int type = Utils.ObjToInt(jObject["type"], 0);//1删除订单文件，2删除非业务申请和付款明细的文件
                if (type != 1 && type != 2)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"typeError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                bool result = false;
                if (type == 1)
                {
                    BLL.Order bll = new BLL.Order();
                    Model.Files file = bll.GetFileModel(fileID);
                    result = bll.deleteOrderFile(file, managerModel);                    
                }
                else
                {
                    BLL.payPic bll = new BLL.payPic();
                    Model.payPic file = bll.GetModel(fileID);
                    result = bll.deletePayFile(file, managerModel);
                }
                if (result)
                {
                    context.Response.Write("{\"status\": 1, \"msg\": \"success\"}");
                    return;
                }
                context.Response.Write("{\"status\": 0, \"msg\": \"删除失败！\"}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\"" + ex.Message + "\", \"status\":0 }");
                return;
            }
        }

        #endregion

        #region 业绩统计
        /// <summary>
        /// 业绩统计
        /// </summary>
        /// <param name="context"></param>
        private void AchievementStatistic(HttpContext context)
        {
            try
            {
                get_params(context, out jObject);
                if (jObject["managerid"] == null || string.IsNullOrWhiteSpace(jObject["managerid"].ToString()) || !int.TryParse(jObject["managerid"].ToString(), out int managerid) || !int.TryParse(jObject["pageSize"].ToString(), out int pageSize) || !int.TryParse(jObject["pageIndex"].ToString(), out int pageIndex))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"KeyIsNullOrError\"}");
                    return;
                }
                Model.manager managerModel = new BLL.manager().GetModel(managerid);
                if (managerModel == null)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"ManageridIsNullOrError\"}");
                    return;
                }
                string type = jObject["type"] == null ? "" : Utils.ObjectToStr(jObject["type"]);//0下单，1策划接单，2设计接单
                string sMonth = jObject["sMonth"] == null ? "" : Utils.ObjectToStr(jObject["sMonth"]);
                string eMonth = jObject["eMonth"] == null ? "" : Utils.ObjectToStr(jObject["eMonth"]);
                string status = jObject["status"] == null ? "" : Utils.ObjectToStr(jObject["status"]);
                string lockStatus = jObject["lockStatus"] == null ? "" : Utils.ObjectToStr(jObject["lockStatus"]);
                string isRemove = jObject["isRemove"] == null ? "" : Utils.ObjectToStr(jObject["isRemove"]);
                string isCust = jObject["isCust"] == null ? "" : Utils.ObjectToStr(jObject["isCust"]);
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("type", type);
                if (!string.IsNullOrEmpty(sMonth))
                {
                    dict.Add("smonth", sMonth);
                }
                if (!string.IsNullOrEmpty(sMonth))
                {
                    dict.Add("emonth", sMonth);
                }
                if (!string.IsNullOrEmpty(status))
                {
                    dict.Add("status", status);
                }
                if (!string.IsNullOrEmpty(lockStatus))
                {
                    dict.Add("lockstatus", lockStatus);
                }                
                dict.Add("isRemove", isRemove);
                dict.Add("isCust", isCust);
                //权限控制
                if (managerModel.area != new BLL.department().getGroupArea())//如果不是总部的工号
                {
                    if (new BLL.permission().checkHasPermission(managerModel, "0602"))
                    {
                        //含有区域权限可以查看本区域添加的
                        dict.Add("area", managerModel.area);
                    }
                    else
                    {
                        //只能
                        dict.Add("person", managerModel.user_name);
                    }
                }
                BLL.statisticBLL bll = new BLL.statisticBLL();
                DataTable dt = bll.getAchievementStatisticData(dict, pageSize, pageIndex, type == "0" ? "p1.op_number asc" : "p3.op_number asc", out int totalCount, out int _tOrderCount,out decimal _tOrderShou,out decimal _tUnincome,out decimal _tOrderFu,out decimal _tUncost, out decimal _tOrderProfit, out decimal _tCust, out decimal _tProfit).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"totalCount\":" + totalCount + ",\"list\":");
                    context.Response.Write(JArray.FromObject(dt) + "}");
                    return;
                }
                context.Response.Write("{\"pageIndex\":" + pageIndex + ",\"pageSize\":" + pageSize + ",\"totalCount\":" + totalCount + ",\"list\":[]}");
                return;
            }
            catch (Exception ex)
            {
                context.Response.Write("{ \"msg\":\"" + ex.Message + "\", \"status\":0 }");
                return;
            }
        }
        #endregion
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}