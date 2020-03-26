using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MettingSys.Web
{
    public class MyHttpModules : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.Error += new EventHandler(context_Error);
        }

        public void context_Error(object sender, EventArgs e)
        {
            //此处处理异常
            HttpContext ctx = HttpContext.Current;
            HttpResponse response = ctx.Response;
            HttpRequest request = ctx.Request;

            //获取到HttpUnhandledException异常，这个异常包含一个实际出现的异常
            Exception ex = ctx.Server.GetLastError();
            //实际发生的异常
            Exception iex = ex.InnerException;

            LogHelper.WriteLog("", "", iex.Message,iex.StackTrace);
            response.Redirect("/MettingSys/Error.aspx");
            //ctx.Server.ClearError();
        }
        
        void IHttpModule.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}