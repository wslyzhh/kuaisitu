using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using MettingSys.Common;

namespace MettingSys.Web.UI
{
    public class ManagePage : System.Web.UI.Page
    {
        protected internal Model.sysconfig sysConfig;

        public ManagePage()
        {
            this.Load += new EventHandler(ManagePage_Load);
            sysConfig = new BLL.sysconfig().loadConfig();
        }

        private void ManagePage_Load(object sender, EventArgs e)
        {
            PrintLoad();
            //判断管理员是否登录
            if (!IsAdminLogin())
            {
                Response.Write("<script>parent.location.href='" + sysConfig.webpath + sysConfig.webmanagepath + "/default.aspx'</script>");
                Response.End();
            }
        }

        override protected void OnLoadComplete(EventArgs e)
        {
            ColseLoad(); 
        }

        #region 管理员============================================
        /// <summary>
        /// 判断管理员是否已经登录(解决Session超时问题)
        /// </summary>
        public bool IsAdminLogin()
        {
            //如果Session为Null
            if (Session[DTKeys.SESSION_ADMIN_INFO] != null)
            {
                return true;
            }
            else
            {
                //检查Cookies
                string adminname = Utils.GetCookie("AdminName", "MettingSys");
                string adminpwd = Utils.GetCookie("AdminPwd", "MettingSys");
                if (adminname != "" && adminpwd != "")
                {
                    BLL.manager bll = new BLL.manager();
                    Model.manager model = bll.GetModel(adminname, adminpwd);
                    if (model != null)
                    {
                        Session[DTKeys.SESSION_ADMIN_INFO] = model;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 取得管理员信息
        /// </summary>
        public Model.manager GetAdminInfo()
        {
            if (IsAdminLogin())
            {
                Model.manager model = Session[DTKeys.SESSION_ADMIN_INFO] as Model.manager;
                if (model != null)
                {
                    return model;
                }
            }
            return null;
        }

        /// <summary>
        /// 检查管理员权限
        /// </summary>
        /// <param name="nav_name">菜单名称</param>
        /// <param name="action_type">操作类型</param>
        public void ChkAdminLevel(string nav_name, string action_type)
        {
            Model.manager model = GetAdminInfo();
            BLL.manager_role bll = new BLL.manager_role();
            bool result = bll.Exists(model.role_id, nav_name, action_type);

            if (!result)
            {
                string msgbox = "parent.jsdialog(\"错误提示\", \"您没有管理该页面的权限，请勿非法进入！\", \"back\")";
                Response.Write("<script type=\"text/javascript\">" + msgbox + "</script>");
                Response.End();
            }
        }

        /// <summary>
        /// 写入管理日志
        /// </summary>
        /// <param name="action_type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool AddAdminLog(string action_type, string remark)
        {
            if (sysConfig.logstatus > 0)
            {
                Model.manager model = GetAdminInfo();
                int newId = new BLL.manager_log().Add(model.id, model.user_name, action_type, remark);
                if (newId > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 写入业务日志
        /// </summary>
        /// <param name="action_type">操作类型</param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool AddBusinessLog(string action_type, Model.business_log modelLog)
        {
            Model.manager model = GetAdminInfo();
            int newId = new BLL.business_log().Add(action_type, modelLog, model.user_name, model.real_name);
            if (newId > 0)
            {
                return true;
            }            
            return false;
        }
        #endregion

        #region JS提示============================================
        protected void JsLoading()
        {
            string msbox = "jsloading();";
            ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsLoading", msbox, true);
        }
        /// <summary>
        /// 添加编辑删除提示
        /// </summary>
        /// <param name="msgtitle">提示文字</param>
        /// <param name="url">返回地址</param>
        protected void JscriptMsg(string msgtitle, string url)
        {
            string msbox = "parent.jsprint(\"" + msgtitle + "\", \"" + url + "\");";
            ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
        }
        /// <summary>
        /// 带回传函数的添加编辑删除提示
        /// </summary>
        /// <param name="msgtitle">提示文字</param>
        /// <param name="url">返回地址</param>
        /// <param name="callback">JS回调函数</param>
        protected void JscriptMsg(string msgtitle, string url, string callback)
        {
            string msbox = "parent.jsprint(\"" + msgtitle + "\", \"" + url + "\", \"" + callback + "\");";
            ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
        }
        /// <summary>
        /// 带确认按钮的提示框
        /// </summary>
        /// <param name="msgtitle">提示文字</param>
        /// <param name="url">返回地址</param>
        protected void JscriptDialog(string msgtitle, string msgcontent, string url, string callback)
        {
            string msbox = "parent.jsdialog(\"" + msgtitle + "\",\"" + msgcontent + "\", \"" + url + "\", \"" + callback + "\");";
            ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsDialog", msbox, true);
        }

        protected void PrintMsg(string msgtitle)
        {
            string msbox = "printMsg(\"" + msgtitle + "\");";
            ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
        }
        protected void PrintJscriptMsg(string msgtitle, string url)
        {
            string msbox = "jsprint(\"" + msgtitle + "\", \"" + url + "\");";
            ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
        }
        /// <summary>
        /// 弹出加载等待
        /// </summary>
        protected void PrintLoad()
        {
            string msbox = "printLoad();";
            ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsDialog", msbox, true);
        }

        /// <summary>
        /// 关闭加载等待
        /// </summary>
        protected void ColseLoad()
        {
            string msbox = "closeLoad();";
            ClientScript.RegisterClientScriptBlock(Page.GetType(), "CloseLayer", msbox, true);
        }
        #endregion

    }
}
