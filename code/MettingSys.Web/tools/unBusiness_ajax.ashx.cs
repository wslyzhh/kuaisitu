using MettingSys.BLL;
using MettingSys.Common;
using MettingSys.Web.SignalR;
using MettingSys.Web.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace MettingSys.Web.tools
{
    /// <summary>
    /// unBusiness_ajax 的摘要说明
    /// </summary>
    public class unBusiness_ajax : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            //检查管理员是否登录
            if (!new ManagePage().IsAdminLogin())
            {
                context.Response.Write("{\"status\": 1, \"msg\": \"尚未登录或已超时，请登录后操作！\"}");
                return;
            }

            string action = DTRequest.GetQueryString("action");
            switch (action)
            {
                case "deleteUnbusiness":
                    delete_Unbusiness(context);
                    break;
                case "checkStatus":
                    check_Status(context);
                    break;
                case "confirmStatus":
                    confirm_Status(context);
                    break;
                case "selfmessage":
                    self_Message(context);
                    break;
                case "setMessageStatus":
                    setMessage_Status(context);
                    break;
                case "selfmessageOnline":
                    self_MessageOnline(context);
                    break;
                case "saveRole":
                    save_Role(context);
                    break;
                default:
                    break;
            }
        }
        #region 非业务支付申请删除============================
        private void delete_Unbusiness(HttpContext context)
        {
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息            
            int id = DTRequest.GetFormInt("id");
            BLL.unBusinessApply bll = new BLL.unBusinessApply();
            string result = bll.Delete(id,adminModel);
            if (!string.IsNullOrEmpty(result))
            {
                context.Response.Write("{ \"msg\":\"" + result + "\", \"status\":1 }");
                context.Response.End();
            }
            //删除文件
            if (Directory.Exists(context.Server.MapPath("~/uploadPay/2/" + id + "/")))
            {
                Directory.Delete(context.Server.MapPath("~/uploadPay/2/" + id + "/"), true);
            }
            context.Response.Write("{ \"msg\":\"操作成功\", \"status\":0 }");
            context.Response.End();
        }
        #endregion
        #region 非业务支付申请审批============================
        private void check_Status(HttpContext context)
        {
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息            
            //string ids = DTRequest.GetFormString("ids");
            //string ctype = DTRequest.GetFormString("ctype");
            //string cstatus = DTRequest.GetFormString("cstatus");
            //string remark = DTRequest.GetFormString("remark");
            //BLL.unBusinessApply bll = new BLL.unBusinessApply();
            //string[] idlist = ids.Split(',');
            //int success = 0, error = 0;
            //StringBuilder sb = new StringBuilder();
            //string reason = "";
            //foreach (string id in idlist)
            //{
            //    reason = bll.checkStatus(Convert.ToInt32(id), Utils.ObjToByte(ctype), Utils.ObjToByte(cstatus), remark, adminModel);
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
            //context.Response.Write("{ \"msg\":\"共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString() + "\", \"status\":" + (error > 0 ? "1" : "0") + "  }");
            //context.Response.End();            
            int id = DTRequest.GetFormInt("id");
            string ctype = DTRequest.GetFormString("ctype");
            string cstatus = DTRequest.GetFormString("cstatus");
            string checkMoney = DTRequest.GetFormString("checkMoney");
            string remark = DTRequest.GetFormString("remark");
            BLL.unBusinessApply bll = new BLL.unBusinessApply();
            StringBuilder sb = new StringBuilder();
            string reason = bll.checkStatus(id, Utils.ObjToByte(ctype), Utils.ObjToByte(cstatus), checkMoney, remark, adminModel);
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
        #region 非业务支付确认============================
        private void confirm_Status(HttpContext context)
        {
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            //string ids = DTRequest.GetFormString("ids");
            //string status = DTRequest.GetFormString("status");
            //string date = DTRequest.GetFormString("date");
            //int method = Utils.StrToInt(DTRequest.GetFormString("method"), 0);
            //string methodName = DTRequest.GetFormString("methodName");
            //BLL.unBusinessApply bll = new BLL.unBusinessApply();
            //string[] idlist = ids.Split(',');
            //int success = 0, error = 0;
            //StringBuilder sb = new StringBuilder();
            //string reason = "";
            //foreach (string id in idlist)
            //{
            //    reason = bll.confirmStatus(Convert.ToInt32(id), status, date, method, methodName, adminModel);
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
            //context.Response.Write("{ \"msg\":\"共选择" + idlist.Length + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString() + "\", \"status\":" + (error > 0 ? "1" : "0") + "  }");
            //context.Response.End();           
            int id = DTRequest.GetFormInt("id");
            string status = DTRequest.GetFormString("status");
            string date = DTRequest.GetFormString("date");
            int method = Utils.StrToInt(DTRequest.GetFormString("method"), 0);
            string methodName = DTRequest.GetFormString("methodName");
            BLL.unBusinessApply bll = new BLL.unBusinessApply();
            string reason = bll.confirmStatus(id, status, date, method, methodName, adminModel);
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
        #region 返回当前登录工号的未读消息============================
        private void self_Message(HttpContext context)
        {
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            BLL.selfMessage bll = new BLL.selfMessage();
            DataSet ds = bll.getUnReadMessage(adminModel.user_name, adminModel.real_name);
            if (ds != null && ds.Tables.Count>0)
            {
                context.Response.Write(JArray.FromObject(ds.Tables[0]));
                context.Response.End();
            }
            context.Response.Write("[]");
            context.Response.End();
        }
        #endregion
        #region 返回当前登录工号的未读消息============================
        private void self_MessageOnline(HttpContext context)
        {
            Model.manager adminModel = new ManagePage().GetAdminInfo();//获得当前登录管理员信息
            messageEntity message = new messageEntity();
            var model = message.GetData(adminModel.user_name, adminModel.real_name);
            if (model != null)
            {
                context.Response.Write(JArray.FromObject(model));
                context.Response.End();
            }
            context.Response.Write("[]");
            context.Response.End();
        }
        #endregion
        #region 设置消息的已读状态============================
        private void setMessage_Status(HttpContext context)
        {
            int mid = DTRequest.GetFormInt("mid");
            BLL.selfMessage bll = new BLL.selfMessage();
            Model.selfMessage model = bll.GetModel(mid);
            model.me_isRead = true;
            bll.Update(model);
            context.Response.End();
        }
        #endregion
        #region 添加编辑角色============================
        private void save_Role(HttpContext context)
        {
            int mid = DTRequest.GetFormInt("mid");
            BLL.selfMessage bll = new BLL.selfMessage();
            Model.selfMessage model = bll.GetModel(mid);
            model.me_isRead = true;
            bll.Update(model);
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
    }
}