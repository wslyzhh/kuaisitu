using MettingSys.Common;
using MettingSys.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace MettingSys.Web.tools
{
    /// <summary>
    /// Order_ajax 的摘要说明
    /// </summary>
    public class Order_ajax : IHttpHandler, IRequiresSessionState
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
                case "saveOrder":
                    save_Order(context);
                    break;
                case "changeOrderStatus":
                    change_OrderStatus(context);
                    break;
                default:
                    break;
            }
        }
        #region 保存订单信息
        private void save_Order(HttpContext context)
        {
            string oID = DTRequest.GetFormString("orderID");
            int cid = DTRequest.GetFormInt("hCusId",0);
            int coid = DTRequest.GetFormInt("ddlcontact",0);
            string contractPrice = DTRequest.GetFormString("ddlcontractPrice");
            string sdate = DTRequest.GetFormString("txtsDate");
            string edate = DTRequest.GetFormString("txteDate");
            string address = DTRequest.GetFormString("txtAddress");
            string content = DTRequest.GetFormString("txtContent");
            string contract = DTRequest.GetFormString("txtContract");
            string remark = DTRequest.GetFormString("txtRemark");
            string place = DTRequest.GetFormString("hide_place");
            int fstatus = DTRequest.GetFormInt("ddlfStatus",0);
            string employee1 = DTRequest.GetFormString("hide_employee1");
            string employee2 = DTRequest.GetFormString("hide_employee2");
            string employee3 = DTRequest.GetFormString("hide_employee3");
            string employee4 = DTRequest.GetFormString("hide_employee4");
            string pushStatus = DTRequest.GetFormString("ddlpushStatus");
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
                order.personlist.Add(new Model.OrderPerson() { op_type = 2, op_name = pli[0], op_number = pli[1], op_area = pli[2],op_addTime=DateTime.Now });
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
                    order.personlist.Add(new Model.OrderPerson() { op_type = 3, op_name = pli[0], op_number = pli[1], op_area = pli[2], op_dstatus = Utils.ObjToByte(pli[3]), op_addTime = DateTime.Now });
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
                    order.personlist.Add(new Model.OrderPerson() { op_type = 4, op_name = pli[0], op_number = pli[1], op_area = pli[2], op_addTime = DateTime.Now });
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
                    order.personlist.Add(new Model.OrderPerson() { op_type = 5, op_name = pli[0], op_number = pli[1], op_area = pli[2], op_dstatus = Utils.ObjToByte(pli[3]), op_addTime = DateTime.Now });
                }
            }
            #endregion
            string oid = string.Empty;
            string result = string.Empty;
            BLL.manager_role bll = new BLL.manager_role();
            Model.manager manager = new ManagePage().GetAdminInfo();
            if (string.IsNullOrEmpty(oID))
            {
                if (!bll.Exists(manager.role_id, "sys_order_add", "Add"))
                {
                    context.Response.Write("{ \"msg\":\"您没有管理该页面的权限，请勿非法进入！\", \"status\":\"1\" }");
                    return;
                }
                result = new BLL.Order().AddOrder(order, manager, out oid);
            }
            else
            {
                if (!bll.Exists(manager.role_id, "sys_order_add", "Edit"))
                {
                    context.Response.Write("{ \"msg\":\"您没有管理该页面的权限，请勿非法进入！\", \"status\":\"1\" }");
                    return;
                }
                oid = oID;
                result = new BLL.Order().UpdateOrder(order, manager);
            }
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"" + oid + "\", \"status\":\"0\" }");
                return;
            }
            context.Response.Write("{ \"msg\":\""+ result + "\", \"status\":\"1\" }");
            return;
        }
        #endregion
        #region 订单状态
        private void change_OrderStatus(HttpContext context)
        {
            int tag = DTRequest.GetQueryInt("tag", 0);
            string oID = DTRequest.GetQueryString("oID");
            int status = DTRequest.GetQueryInt("status",0);
            int flag = DTRequest.GetQueryInt("flag",0);
            byte? lockstatus = Utils.ObjToByte(DTRequest.GetQueryString("lockstatus"));
            decimal cost = DTRequest.GetQueryDecimal("cost", 0);
            string finRemark = DTRequest.GetQueryString("finRemark");
            if (tag == 0)
            {
                context.Response.Write("{ \"msg\":\"参数错误\", \"status\":\"1\" }");
                return;
            }
            BLL.Order bll = new BLL.Order();
            Model.manager manager = new ManagePage().GetAdminInfo();
            string result = string.Empty;
            if (tag == 1)
            {
                result = bll.updateDstatus(oID, (byte)status, manager);
            }
            else if (tag == 2)
            {
                result = bll.updateFlag(oID, (byte)flag, manager);
            }
            else if (tag == 3)
            {
                result = bll.updateLockStatus(oID, lockstatus, manager);
            }
            else if (tag == 4)
            {
                result = bll.updateCost(oID, cost, manager);
            }
            else {
                result = bll.updateFinRemark(oID, finRemark, manager);
            }
            if (string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\""+ oID + "\", \"status\":\"0\" }");
                return;
            }
            context.Response.Write("{ \"msg\":\""+ result + "\", \"status\":\"1\" }");
            return;
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