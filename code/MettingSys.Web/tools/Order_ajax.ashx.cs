using MettingSys.BLL;
using MettingSys.Common;
using MettingSys.Web.UI;
using System;
using System.Collections.Generic;
using System.Data;
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
                case "searchPerson":
                    searchPerson(context);
                    break;
                case "checkRatio":
                    checkRatio(context);
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
            string employee0 = DTRequest.GetFormString("hide_employee0");
            string employee1 = DTRequest.GetFormString("hide_employee1");
            string employee2 = DTRequest.GetFormString("hide_employee2");
            string employee3 = DTRequest.GetFormString("hide_employee3");
            string employee4 = DTRequest.GetFormString("hide_employee4");
            string employee6 = DTRequest.GetFormString("hide_employee6");
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
            int ratio3=0, ratio6 = 0;
            #region 业务报账员
            if (!string.IsNullOrEmpty(employee1))
            {
                pli = new string[] { };
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
                    pli = item.Split('-');
                    if (pli.Length == 4)
                    {
                        order.personlist.Add(new Model.OrderPerson() { op_type = 4, op_name = pli[0], op_number = pli[1], op_area = pli[2], op_ratio = Utils.ObjToInt(pli[3]), op_addTime = DateTime.Now });
                        ratio3 += Utils.ObjToInt(pli[3]);
                    }
                    else
                    {
                        order.personlist.Add(new Model.OrderPerson() { op_type = 4, op_name = pli[0], op_number = pli[1], op_area = pli[2], op_ratio = 0, op_addTime = DateTime.Now });
                    }
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
            #region 公共业务人员
            if (!string.IsNullOrEmpty(employee6))
            {
                pli = new string[] { };
                list = employee6.Split(',');
                foreach (string item in list)
                {
                    pli = item.Split('-');
                    order.personlist.Add(new Model.OrderPerson() { op_type = 6, op_name = pli[0], op_number = pli[1], op_area = pli[2],op_ratio=Utils.ObjToInt(pli[3],0), op_addTime = DateTime.Now });
                    ratio6 += Utils.ObjToInt(pli[3],0);
                }
            }
            #endregion
            #region 下单人
            if (!string.IsNullOrEmpty(employee0))
            {
                pli = employee0.Split('|');
                order.personlist.Add(new Model.OrderPerson() { op_type = 1, op_name = pli[0], op_number = pli[1], op_area = pli[2],op_ratio = 100-ratio3-ratio6, op_addTime = DateTime.Now });
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
            int tag = DTRequest.GetFormInt("tag", 0);
            string oID = DTRequest.GetFormString("oID");
            int status = DTRequest.GetFormInt("status",0);
            int flag = DTRequest.GetFormInt("flag",0);
            byte? lockstatus = Utils.ObjToByte(DTRequest.GetFormString("lockstatus"));
            decimal cost = DTRequest.GetFormDecimal("cost", 0);
            string finRemark = DTRequest.GetFormString("finRemark");
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
        #region 查找用户
        private void searchPerson(HttpContext context)
        {
            string word = DTRequest.GetFormString("wrod").Trim().ToUpper();
            if (string.IsNullOrEmpty(word))
            {
                context.Response.Write("{ \"msg\":\"请输入员工工号或者姓名\", \"status\":\"1\" }");
                return;
            }
            DataSet ds = new manager().GetList(1, "is_lock=0 and (user_name = '" + word + "' or real_name= '" + word + "')", "user_name");
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                context.Response.Write("{ \"msg\":\"无此员工信息\", \"status\":\"1\" }");
                return;
            }
            DataRow dr = ds.Tables[0].Rows[0];
            context.Response.Write("{ \"msg\":\"成功\", \"username\":\""+dr["user_name"] + "\",\"realname\":\"" + dr["real_name"] + "\",\"area\":\"" + dr["area"] + "\",\"status\":\"1\" }");
            return;
        }
        #endregion
        #region 执行人员业绩比例
        private void checkRatio(HttpContext context)
        {
            BLL.publicSetting bll = new BLL.publicSetting();
            Model.publicSetting model = bll.GetModel(1);
            if (model == null || model.ps_isuse==false)
            {
                context.Response.Write("{\"status\":\"0\" }");
                return;
            }
            context.Response.Write("{\"status\":\"1\",\"sdate\":\""+ model.ps_sdate.Value.ToString("yyyy-MM-dd") + "\",\"edate\":\"" + model.ps_edate.Value.ToString("yyyy-MM-dd") + "\",\"ratio\":\""+ model.ps_ratio + "\" }");
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