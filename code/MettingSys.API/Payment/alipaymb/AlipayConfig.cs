﻿using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Xml;
using System.Collections.Generic;
using MettingSys.Common;

namespace MettingSys.API.Payment.alipaymb
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.3
    /// 日期：2012-07-05
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// 
    /// 如何获取安全校验码和合作身份者ID
    /// 1.用您的签约支付宝账号登录支付宝网站(www.alipay.com)
    /// 2.点击“商家服务”(https://b.alipay.com/order/myOrder.htm)
    /// 3.点击“查询合作者身份(PID)”、“查询安全校验码(Key)”
    /// </summary>
    public class Config
    {
        #region 字段
        private string partner = string.Empty;
        private string seller_email = string.Empty;
        private string key = string.Empty;
        private string private_key = string.Empty;
        private string public_key = string.Empty;
        private string input_charset = string.Empty;
        private string sign_type = string.Empty;
        private string return_url = string.Empty;
        private string notify_url = string.Empty;
        #endregion

        public Config(int site_payment_id)
        {
            Model.site_payment model = new BLL.site_payment().GetModel(site_payment_id); //站点支付方式
            if (model != null)
            {
                Model.payment payModel = new BLL.payment().GetModel(model.payment_id); //支付平台
                Model.sites siteModel = new BLL.sites().GetModel(model.site_id); //站点配置
                Model.sysconfig sysConfig = new BLL.sysconfig().loadConfig(); //系统配置

                //收款支付宝账号
                seller_email = model.key1;
                //合作身份者ID，以2088开头由16位纯数字组成的字符串
                partner = model.key2;
                //如果签名方式设置为“MD5”时，请设置该参数
                key = model.key3;
                //商户的私钥，如果签名方式设置为“0001”时，请设置该参数
                private_key = @"";
                //支付宝的公钥，如果签名方式设置为“0001”时，请设置该参数
                public_key = @"";
                //回调处理地址
                if (!string.IsNullOrEmpty(siteModel.domain.Trim()) && siteModel.is_default == 0) //如果有自定义域名且不是默认站点
                {
                    return_url = "http://" + siteModel.domain + payModel.return_url;
                    notify_url = "http://" + siteModel.domain + payModel.notify_url;
                }
                else if (siteModel.is_default == 0) //不是默认站点也没有绑定域名
                {
                    return_url = "http://" + HttpContext.Current.Request.Url.Authority.ToLower() + sysConfig.webpath + siteModel.build_path.ToLower() + payModel.return_url;
                    notify_url = "http://" + HttpContext.Current.Request.Url.Authority.ToLower() + sysConfig.webpath + siteModel.build_path.ToLower() + payModel.notify_url;
                }
                else //否则使用当前域名
                {
                    return_url = "http://" + HttpContext.Current.Request.Url.Authority.ToLower() + sysConfig.webpath + payModel.return_url;
                    notify_url = "http://" + HttpContext.Current.Request.Url.Authority.ToLower() + sysConfig.webpath + payModel.notify_url;
                }
                //字符编码格式 目前支持 utf-8
                input_charset = "utf-8";
                //签名方式，选择项：0001(RSA)、MD5
                sign_type = "MD5";//无线的产品中，签名方式为rsa时，sign_type需赋值为0001而不是RSA
            }
        }

        #region 属性
        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public string Partner
        {
            get { return partner; }
            set { partner = value; }
        }
		
        /// <summary>
        /// 获取或设置收款支付宝账号
        /// </summary>
        public string Seller_email
        {
            get { return seller_email; }
            set { seller_email = value; }
        }

        /// <summary>
        /// 获取或设交易安全校验码
        /// </summary>
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// 获取或设置商户的私钥
        /// </summary>
        public string Private_key
        {
            get { return private_key; }
            set { private_key = value; }
        }

        /// <summary>
        /// 获取或设置支付宝的公钥
        /// </summary>
        public string Public_key
        {
            get { return public_key; }
            set { public_key = value; }
        }

        /// <summary>
        /// 获取页面跳转同步通知页面路径
        /// </summary>
        public string Return_url
        {
            get { return return_url; }
        }

        /// <summary>
        /// 获取服务器异步通知页面路径
        /// </summary>
        public string Notify_url
        {
            get { return notify_url; }
        }

        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public string Input_charset
        {
            get { return input_charset; }
        }

        /// <summary>
        /// 获取签名方式
        /// </summary>
        public string Sign_type
        {
            get { return sign_type; }
        }
        #endregion
    }
}