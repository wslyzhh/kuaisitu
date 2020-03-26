using System;
using System.Collections.Generic;
using System.Web;

namespace MettingSys.API.Payment.wxpay
{
    public class WxPayException : Exception 
    {
        public WxPayException(string msg) : base(msg)
        {

        }
    }
}
