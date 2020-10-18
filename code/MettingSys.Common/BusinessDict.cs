using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MettingSys.Common
{
    /// <summary>
    /// 业务字典统一管理
    /// </summary>
    public class BusinessDict
    {
        
        #region 非业务支付申请支付类别
        /// <summary>
        /// 非业务支付申请支付类别
        /// </summary>
        public static Dictionary<byte,string> unBusinessNature(byte? type = null)
        {
            Dictionary<byte, string> dict = new Dictionary<byte, string>();
            if (type == 0)
            {
                dict.Add(0, "员工往来");
            }
            else
            {
                dict.Add(0, "员工往来");
                dict.Add(1, "费用报销");
                dict.Add(2, "工薪支付");
                dict.Add(3, "采购支付");
                dict.Add(4, "对外往来");
            }
            return dict;
        }
        #endregion

        #region 非业务支付员工往来支付用途
        /// <summary>
        /// 非业务支付员工往来支付用途
        /// </summary>
        public static Dictionary<string, string> unBusinessPayFunction(byte? type = null)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (type == 0)
            {
                dict.Add("业务活动执行备用金借款", "业务活动执行备用金借款");
            }
            else if (type == 1)
            {
                dict.Add("行政办公备用金借款", "行政办公备用借金款");
                dict.Add("个人借款", "个人借款");
            }
            else
            {
                dict.Add("业务活动执行备用金借款", "业务活动执行备用金借款");
                dict.Add("行政办公备用金借款", "行政办公备用借金款");
                dict.Add("个人借款", "个人借款");
            }
            return dict;
        }
        #endregion

        #region 审批状态
        /// <summary>
        /// 审批状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<byte?, string> checkStatus()
        {
            Dictionary<byte?, string> dict = new Dictionary<byte?, string>();
            dict.Add(2, "审批通过");
            dict.Add(1, "审批未通过");
            dict.Add(0, "待审批");
            return dict;
        }
        #endregion

        #region 财务收付状态
        /// <summary>
        /// 财务收付状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<bool?, string> financeType()
        {
            Dictionary<bool?, string> dict = new Dictionary<bool?, string>();
            dict.Add(true, "收");
            dict.Add(false, "付");
            return dict;
        }
        #endregion

        #region 财务支付状态
        /// <summary>
        /// 财务支付状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<bool?, string> financeConfirmStatus(byte? type=3)
        {
            Dictionary<bool?, string> dict = new Dictionary<bool?, string>();
            if (type == 1)
            {
                dict.Add(false, "待收款");
                dict.Add(true, "已收款");
            }
            else if (type == 0)
            {
                dict.Add(false, "待付款");
                dict.Add(true, "已付款");
            }
            else
            {
                dict.Add(false, "待支付");
                dict.Add(true, "已支付");
            }
            return dict;
        }
        #endregion

        #region 启用禁用状态
        /// <summary>
        /// 启用禁用状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<bool?, string> isUseStatus(byte? flag=0)
        {
            Dictionary<bool?, string> dict = new Dictionary<bool?, string>();
            if (flag == 0)
            {
                dict.Add(false, "<font color='red'>禁用</font>");
                dict.Add(true, "<font color='green'>启用</font>");
            }
            else
            {
                dict.Add(false, "禁用");
                dict.Add(true, "启用");
            }
            return dict;
        }
        #endregion

        #region 客户类别
        /// <summary>
        /// 客户类别
        /// </summary>
        /// <returns></returns>
        public static Dictionary<byte?, string> customerType(byte? type=3)
        {
            Dictionary<byte?, string> dict = new Dictionary<byte?, string>();
            //if (type == 1)
            //{
            //    dict.Add(1, "普通客户");
            //}
            //else if (type == 2)
            //{
            //    dict.Add(1, "普通客户");
            //    dict.Add(2, "管理用客户");
            //}
            //else
            //{
            //    dict.Add(1, "普通客户");
            //    dict.Add(2, "管理用客户");
            //    dict.Add(3, "内部客户");
            //}
            if (type == 1)
            {
                dict.Add(1, "客户");
                dict.Add(4, "供应商");
                dict.Add(5, "客户兼供应商");
            }
            else if (type == 2)
            {
                dict.Add(1, "客户");
                dict.Add(4, "供应商");
                dict.Add(5, "客户兼供应商");
                dict.Add(2, "管理用客户");
            }
            else
            {
                dict.Add(1, "客户");
                dict.Add(4, "供应商");
                dict.Add(5, "客户兼供应商");
                dict.Add(2, "管理用客户");
                dict.Add(3, "内部客户");
            }
            return dict;
        }
        #endregion

        #region 应税劳务
        /// <summary>
        /// 应税劳务
        /// </summary>
        /// <returns></returns>
        public static Dictionary<byte?, string> taxableType()
        {
            Dictionary<byte?, string> dict = new Dictionary<byte?, string>();
            dict.Add(1, "会展服务");
            dict.Add(2, "设计服务");
            dict.Add(3, "代订服务");
            return dict;
        }
        #endregion

        #region 服务名称
        /// <summary>
        /// 服务名称
        /// </summary>
        /// <returns></returns>
        public static Dictionary<byte?, string> serviceName(byte? type)
        {
            Dictionary<byte?, string> dict = new Dictionary<byte?, string>();
            if (type == 1)
            {
                dict.Add(1, "会务服务");
                dict.Add(2, "展览展示");
                dict.Add(3, "会议及会务展览服务");
                dict.Add(4, "舞台搭建");
                dict.Add(5, "代订设备");
                dict.Add(6, "旅业服务");
            }
            else if (type == 2)
            {
                dict.Add(1, "广告制作");
            }
            else
            {
                dict.Add(1, "代订房费");
                dict.Add(2, "代订餐");
                dict.Add(3, "代订车");
                dict.Add(4, "代订船");
                dict.Add(5, "代订机票");
            }
            return dict;
        }
        #endregion

        #region 送票方式
        /// <summary>
        /// 送票方式
        /// </summary>
        /// <returns></returns>
        public static Dictionary<byte?, string> sentMethod()
        {
            Dictionary<byte?, string> dict = new Dictionary<byte?, string>();
            dict.Add(1, "邮寄");
            dict.Add(2, "业务员送达");
            return dict;
        }
        #endregion

        #region 专普票
        /// <summary>
        /// 财务收付状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<bool?, string> invType()
        {
            Dictionary<bool?, string> dict = new Dictionary<bool?, string>();
            dict.Add(true, "专票");
            dict.Add(false, "普票");
            return dict;
        }
        #endregion

        #region 发票开票状态
        /// <summary>
        /// 财务支付状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<bool?, string> invoiceConfirmStatus(byte? type = 3)
        {
            Dictionary<bool?, string> dict = new Dictionary<bool?, string>();
            dict.Add(false, "未开票");
            dict.Add(true, "已开票");
            return dict;
        }
        #endregion

        #region 机构类别
        /// <summary>
        /// 机构类别
        /// </summary>
        /// <returns></returns>
        public static Dictionary<byte?, string> departType(byte? type=3)
        {
            Dictionary<byte?, string> dict = new Dictionary<byte?, string>();
            if (type == 1)
            {
                dict.Add(1, "公司");
            }
            else if (type == 2)
            {
                dict.Add(2, "部门");
                dict.Add(3, "岗位");
            }
            else
            {
                dict.Add(1, "公司");
                dict.Add(2, "部门");
                dict.Add(3, "岗位");
            }
            return dict;
        }
        #endregion

        #region 合同造价
        /// <summary>
        /// 合同造价
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> ContractPriceType()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("50万以下", "普通活动");
            dict.Add("50万-100万", "重大活动");
            dict.Add("100万以上", "特大活动");
            return dict;
        }
        #endregion 

        #region 订单状态
        /// <summary>
        /// 订单状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<byte?, string> fStatus(byte? type=null)
        {
            Dictionary<byte?, string> dict = new Dictionary<byte?, string>();
            if (type == 1)
            {
                dict.Add(0, "待定");
                dict.Add(1, "取消");
                dict.Add(2, "确认");
                dict.Add(3, "确认或取消");
            }
            else if (type == 2)
            {
                dict.Add(0, "<font color='red'>待定</font>");
                dict.Add(1, "<font color='gray'>取消</font>");
                dict.Add(2, "<font color='green'>确认</font>");
            }
            else
            {
                dict.Add(0, "待定");
                dict.Add(1, "取消");
                dict.Add(2, "确认");
            }
            return dict;
        }
        #endregion

        #region 接单状态
        /// <summary>
        /// 接单状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<byte?, string> dStatus(byte? type = null)
        {
            Dictionary<byte?, string> dict = new Dictionary<byte?, string>();
            if (type == 1)
            {
                dict.Add(0, "待定");
                dict.Add(1, "处理中");
                dict.Add(2, "已完成");
                dict.Add(3, "未安排人员");
                dict.Add(4, "未安排人员或已完成");
                dict.Add(5, "待定与处理中");
            }
            else if (type == 2)
            {
                dict.Add(0, "<font color='red'>待定</font>");
                dict.Add(1, "<font color='blue'>处理中</font>");
                dict.Add(2, "<font color='green'>已完成</font>");
                dict.Add(5, "<font color='orange'>待定与处理中</font>");
            }
            else
            {
                dict.Add(0, "待定");
                dict.Add(1, "处理中");
                dict.Add(5, "待定与处理中");
                dict.Add(2, "已完成");
            }
            return dict;
        }
        #endregion

        #region 订单推送状态
        /// <summary>
        /// 订单推送状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<bool?, string> pushStatus()
        {
            Dictionary<bool?, string> dict = new Dictionary<bool?, string>();
            dict.Add(false, "未推送");
            dict.Add(true, "已推送");
            return dict;
        }
        #endregion

        #region 订单锁单状态
        /// <summary>
        /// 订单锁单状态
        /// </summary>
        /// <returns></returns>
        public static Dictionary<byte?, string> lockStatus()
        {
            Dictionary<byte?, string> dict = new Dictionary<byte?, string>();
            dict.Add(0, "未锁");
            dict.Add(1, "已锁");
            dict.Add(2, "待处理");
            return dict;
        }
        #endregion
    }
}
