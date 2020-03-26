using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using DingTalk.Api.Response;
using MettingSys.BLL;
using MettingSys.Common;
using Newtonsoft.Json.Linq;

namespace MettingSys.Web.tools
{
    /// <summary>
    /// dingtalk_login 的摘要说明
    /// </summary>
    public class dingtalk_login : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            //取得处事类型
            string action = DTRequest.GetQueryString("action");
            switch (action)
            {
                case "get_dingtalk_corpid": //获取钉钉企业ID
                    get_dingtalk_corpid(context);
                    break;
                case "dingtalk_userid_validate": //验证钉钉userid是否已绑定用户
                    Dingtalk_userid_validate(context);
                    break;
                case "dingtalk_userid_validate_Test":
                    Dingtalk_userid_validate_Test(context);
                    break;
                case "username_validate": //验证用户名是否可绑定
                    Username_validate(context);
                    break;
                case "manager_oauth_bind": //绑定钉钉userid
                    Manager_oauth_bind(context);
                    break;
                case "Manager_oauth_remove": //解除员工账号钉钉userid授权绑定
                    Manager_oauth_remove(context);
                    break;
                case "remove_OauthBind":
                    removeOauthBind(context);
                    break;
                case "sent_Message":
                    sentMessage(context);
                    break;
            }
        }

        private void get_dingtalk_corpid(HttpContext context)
        {
            context.Response.Write("{ \"corpid\": \"" + dingtalk_helper.corpid + "\"}");
            return;
        }

        #region 验证钉钉user_id是否绑定测试============================
        private void Dingtalk_userid_validate_Test(HttpContext context)
        {
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string payload = stream.ReadToEnd();
            JObject jo = JObject.Parse(payload);
            if (jo == null || jo["code"] == null || string.IsNullOrWhiteSpace(jo["code"].ToString()))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"ParameterIsNull\"}");
                return;
            }
            OapiGettokenResponse response = dingtalk_helper.GetDingTalkAccessToken();
            string userid = dingtalk_helper.GetDingTalkUserid(jo["code"].ToString(), response.AccessToken);

            //context.Response.Write("{\"status\":2, \"msg\": \"UseridIsNoOauth\"}");
            //return;
            //string userid = "wangyu";
            //string userid = jo["oauth_userid"].ToString();
            //如果为Null，退出
            if (string.IsNullOrEmpty(userid))
            {

                //context.Response.Write("{\"status\": 1, \"msg\": \"UseridIsNull\"}");
                return;
            }
            BLL.manager_oauth bll = new BLL.manager_oauth();
            Model.manager_oauth oauthModel = bll.GetModel("dingtalk", userid.Trim());
            //查询数据库
            if (oauthModel == null)
            {
                context.Response.Write("{\"status\":2, \"msg\": \"UseridIsNoOauth\"}");
                return;
            }
            else
            {
                Model.manager model = new BLL.manager().GetModel(oauthModel.manager_id);
                //写入登录日志
                Model.sysconfig sysConfig = new BLL.sysconfig().loadConfig();
                if (sysConfig.logstatus > 0)
                {
                    new BLL.manager_log().Add(model.id, model.user_name, DTEnums.ActionEnum.Login.ToString(), "用户登录钉钉平台");
                }
                context.Response.Write("{\"status\": 3, \"msg\": \"success\",\"model\":" + JObject.FromObject(model) + "}");
                return;
            }
        }
        #endregion

        #region 验证钉钉user_id是否绑定============================
        private void Dingtalk_userid_validate(HttpContext context)
        {
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string payload = stream.ReadToEnd();
            JObject jo = JObject.Parse(payload);
            if (jo == null || jo["code"] == null || string.IsNullOrWhiteSpace(jo["code"].ToString()))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"CodeIsNull\"}");
                return;
            }
            OapiGettokenResponse response = dingtalk_helper.GetDingTalkAccessToken();
            string userid = dingtalk_helper.GetDingTalkUserid(jo["code"].ToString(), response.AccessToken);

            Model.sysconfig sysConfig1 = new BLL.sysconfig().loadConfig();//获得系统配置信息
            string appkey = sysConfig1.AppKey;
            string appsecret = sysConfig1.AppSecret;
            //如果为Null，退出
            if (string.IsNullOrEmpty(userid))
            {
                context.Response.Write("{\"status\": 1, \"msg\": \"UseridIsNull,appkey:"+ appkey + ",appsecret:"+ appsecret + ",token:" + response.AccessToken + "\"}");
                return;
            }
            BLL.manager_oauth bll = new BLL.manager_oauth();
            Model.manager_oauth oauthModel = bll.GetModel("dingtalk", userid.Trim());
            //查询数据库
            if (oauthModel == null || oauthModel.is_lock == 0)
            {
                context.Response.Write("{\"status\":2, \"msg\": \"UseridIsNoOauth\"}");
                return;
            }
            else
            {
                Model.manager model = new BLL.manager().GetModel(oauthModel.manager_id);
                //写入登录日志
                Model.sysconfig sysConfig = new BLL.sysconfig().loadConfig();
                if (sysConfig.logstatus > 0)
                {
                    new BLL.manager_log().Add(model.id, model.user_name, DTEnums.ActionEnum.Login.ToString(), "用户登录钉钉平台");
                }
                context.Response.Write("{\"status\": 3, \"msg\": \"success\",\"model\":" + JObject.FromObject(model) + "}");
                return;
            }
        }
        #endregion

        #region 验证用户名是否可用OK============================
        private void Username_validate(HttpContext context)
        {
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string payload = stream.ReadToEnd();
            JObject jo = JObject.Parse(payload);
            if (jo == null)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"ParameterIsNull\"}");
                return;
            }
            string username = jo["username"].ToString();
            //如果为Null，退出
            if (string.IsNullOrEmpty(username))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"UsernameIsNull\"}");
                return;
            }
            BLL.manager bll = new BLL.manager();
            //查询数据库
            if (!bll.Exists(username.Trim()))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"UsernameNotExist\"}");
                return;
            }
            context.Response.Write("{\"status\": 1, \"msg\": \"Success\"}");
            return;
        }
        #endregion

        #region 员工账户绑定钉钉userid
        private void Manager_oauth_bind(HttpContext context)
        {
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string payload = stream.ReadToEnd();
            JObject jo = JObject.Parse(payload);

            if (jo == null || jo["code"] == null || string.IsNullOrWhiteSpace(jo["code"].ToString()))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"CodeIsNull\"}");
                return;
            }
            //检查用户名密码
            if (jo["username"] == null || string.IsNullOrWhiteSpace(jo["username"].ToString()))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"用户名不能为空\"}");
                return;
            }
            if (jo["password"] == null || string.IsNullOrWhiteSpace(jo["password"].ToString()))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"密码不能为空\"}");
                return;
            }
            string username = jo["username"].ToString().ToUpper();
            string password = jo["password"].ToString();

            BLL.manager bll = new BLL.manager();
            Model.manager model = bll.GetModel(username, password, true);
            if (model == null)
            {
                context.Response.Write("{\"status\":0, \"msg\":\"该员工账号不存在或密码不正确\"}");
                return;
            }
            BLL.manager_oauth oauthBll = new BLL.manager_oauth();
            Model.manager_oauth oauthModel1 = oauthBll.GetModel(username);
            if (oauthModel1 != null && oauthModel1.is_lock == 1)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"该账号已经绑定过钉钉，不能重复绑定\"}");
                return;
            }

            //获取钉钉授权数据
            OapiGettokenResponse response = dingtalk_helper.GetDingTalkAccessToken();
            string userid = dingtalk_helper.GetDingTalkUserid(jo["code"].ToString(), response.AccessToken);
            Model.manager_oauth oauthModel = oauthBll.GetModel("dingtalk", userid.Trim());
            if (oauthModel != null)
            {
                if (oauthModel.manager_name != username)
                {
                    context.Response.Write("{\"status\":0, \"msg\":\"已经绑定了工号" + oauthModel.manager_name + "，要重新绑定须先解除绑定！\"}");
                    return;
                }
                oauthModel.manager_id = model.id;
                oauthModel.manager_name = model.user_name;
                oauthModel.oauth_access_token = response.AccessToken;
                oauthModel.is_lock = 1;
                if (!oauthBll.Update(oauthModel))
                {
                    context.Response.Write("{\"status\":0, \"msg\":\"绑定用户授权失败，请联系技术支持处理！\"}");
                    return;
                }
            }
            else
            {
                //开始绑定
                oauthModel = new Model.manager_oauth();
                oauthModel.oauth_name = "dingtalk";
                oauthModel.manager_id = model.id;
                oauthModel.manager_name = model.user_name;
                oauthModel.oauth_access_token = response.AccessToken;
                oauthModel.oauth_userid = userid;
                oauthModel.is_lock = 1;
                int newId = oauthBll.Add(oauthModel);
                if (newId < 1)
                {
                    context.Response.Write("{\"status\":0, \"msg\":\"绑定用户授权失败，请联系技术支持处理！\"}");
                    return;
                }
            }
            //写入登录日志
            Model.sysconfig sysConfig = new BLL.sysconfig().loadConfig();
            if (sysConfig.logstatus > 0)
            {
                new BLL.manager_log().Add(model.id, model.user_name, DTEnums.ActionEnum.Login.ToString(), "用户授权绑定钉钉平台");
            }
            //返回实体类
            context.Response.Write("{\"status\": 1, \"msg\": \"success\",\"model\":" + JObject.FromObject(model) + "}");
            return;
        }
        #endregion

        #region 员工账户解除钉钉userid绑定
        private void Manager_oauth_remove(HttpContext context)
        {
            StreamReader stream = new StreamReader(context.Request.InputStream);
            string payload = stream.ReadToEnd();
            JObject jo = JObject.Parse(payload);

            if (jo == null || jo["code"] == null || string.IsNullOrWhiteSpace(jo["code"].ToString()))
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"CodeIsNull\"}");
                return;
            }
            //获取钉钉授权数据
            OapiGettokenResponse response = dingtalk_helper.GetDingTalkAccessToken();
            string userid = dingtalk_helper.GetDingTalkUserid(jo["code"].ToString(), response.AccessToken);
            BLL.manager_oauth oauthBll = new BLL.manager_oauth();
            Model.manager_oauth oauthModel = oauthBll.GetModel("dingtalk", userid);
            if (oauthModel != null)
            {
                if (!oauthBll.UpdateField("dingtalk", userid, "is_lock=0"))
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"解除用户授权失败，请联系技术支持处理！\"}");
                    return;
                }
                else
                {
                    context.Response.Write("{\"status\": 1, \"msg\": \"解除用户授权成功\"}");
                    return;
                }
            }
            else
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"该钉钉账户未授权或员工用户已不存在\"}");
                return;
            }

        }
        #endregion

        #region PC端移除钉钉绑定
        private void removeOauthBind(HttpContext context)
        {
            string username = DTRequest.GetFormString("username");
            BLL.manager_oauth oauthBll = new BLL.manager_oauth();
            Model.manager_oauth oauthModel = oauthBll.GetModel(username);
            if (oauthModel == null || oauthModel.is_lock == 0)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"该账号还未绑定钉钉\"}");
                return;
            }
            if (oauthModel.is_lock == 1)
            {
                if (oauthBll.Delete(oauthModel.id))
                {
                    context.Response.Write("{\"status\": 1, \"msg\": \"移除绑定成功\"}");
                    return;
                }
            }
            context.Response.Write("{\"status\": 0, \"msg\": \"移除绑定失败\"}");
            return;
        }
        #endregion

        private void sentMessage(HttpContext context)
        {
            string userid = DTRequest.GetFormString("id");
            if (dingtalk_helper.sentMessageToUser(userid, "测试工作消息通知：" + DateTime.Now.ToString()))
            {
                context.Response.Write("{\"status\": 1, \"msg\": \"发送成功\"}");
                return;
            }
            context.Response.Write("{\"status\": 0, \"msg\": \"发送失败\"}");
            return;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}