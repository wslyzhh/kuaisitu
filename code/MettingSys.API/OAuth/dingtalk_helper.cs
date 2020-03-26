using System;
using System.Collections.Generic;
using System.Text;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using MettingSys.Common;
using Top.Api.Util;

namespace MettingSys.API.OAuth
{
    public class dingtalk_helper
    {
        public dingtalk_helper()
        { }

        private static readonly Model.sysconfig sysConfig = new BLL.sysconfig().loadConfig();//获得系统配置信息
        private static string appkey = sysConfig.AppKey;
        private static string appsecret = sysConfig.AppSecret;
        public static string corpid = sysConfig.CorpId;
        public static string agentid = sysConfig.AgentId;

        #region 忽略

        /// <summary>
        /// 取得临时的Access Token
        /// </summary>
        /// <param name="app_id">client_id</param>
        /// <param name="app_key">client_secret</param>
        /// <param name="return_uri">redirect_uri</param>
        /// <param name="code">临时Authorization Code，官方提示2小时过期</param>
        /// <returns>Dictionary</returns>
        /// 返回说明：
        /// {
        ///    "errcode": 0,
        ///    "errmsg": "ok",
        ///    "access_token": "fw8ef8we8f76e6f7s8df8s"
        /// }
        //public static Dictionary<string, object> get_access_token(string app_key, string app_secret, string getaccess_tokenurl)
        //{
        //    //参数
        //    IDictionary<string, string> parm = new Dictionary<string, string>();
        //    parm.Add("appkey", app_key);
        //    parm.Add("appsecret", app_secret);
        //    //调用官方C#的SDK方法进行请求
        //    WebUtils webhelper = new WebUtils();
        //    //发送并接受返回值
        //    string result = webhelper.DoGet(getaccess_tokenurl, parm);

        //    if (!result.Contains("\"errcode\": 0,"))
        //    {
        //        return null;
        //    }
        //    try
        //    {
        //        Dictionary<string, object> dic = JsonHelper.DataRowFromJSON(result);
        //        return dic;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// 获取用户userid
        /// </summary>
        /// <param name="access_token">临时的Access Token</param>
        /// <param name="code">免登授权码</param>
        /// <returns>JsonData</returns>
        //public static Dictionary<string, object> get_info(string access_token, string code, string getuser_info)
        //{
        //    //参数
        //    IDictionary<string, string> parm = new Dictionary<string, string>();
        //    parm.Add("access_token", access_token);
        //    parm.Add("code", code);
        //    //调用官方C#的SDK方法进行请求
        //    WebUtils webhelper = new WebUtils();
        //    //发送并接受返回值
        //    string result = webhelper.DoGet(getuser_info, parm);

        //    if (!result.Contains("\"errcode\": 0,"))
        //    {
        //        return null;
        //    }
        //    try
        //    {
        //        Dictionary<string, object> dic = JsonHelper.DataRowFromJSON(result);
        //        return dic;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        #endregion

        #region 钉钉接口方法
        /// <summary>
        /// 取得临时的Access Token
        /// {
        ///  "errcode": 0,
        ///  "errmsg": "ok",
        ///  "access_token": "fw8ef8we8f76e6f7s8df8s"
        /// }
        /// </summary>
        public static OapiGettokenResponse GetDingTalkAccessToken()
        {
            DefaultDingTalkClient client = new DefaultDingTalkClient(@"https://oapi.dingtalk.com/gettoken");
            OapiGettokenRequest req = new OapiGettokenRequest();
            req.Appkey = appkey;
            req.Appsecret = appsecret;
            req.SetHttpMethod("GET");
            OapiGettokenResponse response = client.Execute(req);
            return response;
        }

        /// <summary>
        /// 获取用户userid
        /// {
        /// "userid": "****",
        /// "sys_level": 1,
        /// "errmsg": "ok",
        /// "is_sys": true,
        /// "deviceId": "***",
        /// "errcode": 0
        /// }
        /// </summary>
        public static String GetDingTalkUserid(string requestAuthCode,string accessToken)
        {
            DefaultDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/user/getuserinfo");
            OapiUserGetuserinfoRequest req = new OapiUserGetuserinfoRequest();
            req.Code = requestAuthCode;
            req.SetHttpMethod("GET");
            OapiUserGetuserinfoResponse response = client.Execute(req, accessToken);
            return response.Userid;
        }

        //private void DingTalkOAuth(string accessToken)
        //{
        //    IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/user/get");
        //    OapiUserGetRequest req = new OapiUserGetRequest();
        //    req.Userid = "userid1";
        //    req.SetHttpMethod("GET");
        //    OapiUserGetResponse rsp = client.Execute(req, accessToken);
        //}

        /// <summary>
        /// 发送工作通知
        /// </summary>
        /// <param name="userList"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool sentMessageToUser(string userList, string content)
        {
            DefaultDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2");
            
            OapiMessageCorpconversationAsyncsendV2Request request = new OapiMessageCorpconversationAsyncsendV2Request();
            request.UseridList = userList;
            request.AgentId = Convert.ToInt64(agentid);
            request.ToAllUser = false;
            request.SetHttpMethod("POST");

            OapiMessageCorpconversationAsyncsendV2Request.MsgDomain msg = new OapiMessageCorpconversationAsyncsendV2Request.MsgDomain();
            msg.Msgtype = "text";
            msg.Text = new OapiMessageCorpconversationAsyncsendV2Request.TextDomain();
            msg.Text.Content = content;
            request.Msg_ = msg;
            
            string accessToken = GetDingTalkAccessToken().AccessToken;
            OapiMessageCorpconversationAsyncsendV2Response response = client.Execute(request, accessToken);
            return !response.IsError;
            //return getSentMessageResult(response.TaskId);
        }

        /// <summary>
        /// 查询工作通知消息的发送结果
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        public static string getSentMessageResult(long taskID)
        {
            DefaultDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/message/corpconversation/getsendresult");
            OapiMessageCorpconversationGetsendresultRequest request = new OapiMessageCorpconversationGetsendresultRequest();
            request.AgentId = Convert.ToInt64(agentid);
            request.TaskId = taskID;

            string accessToken = GetDingTalkAccessToken().AccessToken;
            OapiMessageCorpconversationGetsendresultResponse response = client.Execute(request, accessToken);
            return response.ErrCode;
        }

        #endregion
    }
}
