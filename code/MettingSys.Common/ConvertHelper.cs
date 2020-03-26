﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MettingSys.Common
{
    /// <summary>
    /// 类型转换帮助类
    /// </summary>
    public class ConvertHelper
    {
        #region------------------ToInt32类型转换------------------
        /// <summary>
        /// 读取数据库中字符串并转换成Int32
        /// 为空时返回0
        /// </summary>
        /// <param name="obj">object类型的值</param>
        /// <returns>Int32类型</returns>
        public static int ToInt32(object obj)
        {
            int result = 0;
            if (IsInt(Convert.ToString(obj)))
            {
                result = Convert.ToInt32(obj);
            }
            else if (obj != null && obj is Enum) //处理非null值类型时(或者枚举)
            {
                result = ((IConvertible)obj).ToInt32(null);
            }
            return result;
        }

        /// <summary>
        /// 读取数据库中字符串并转换成Int32
        /// 为空时返回0
        /// </summary>
        /// <param name="str">string类型的值</param>
        /// <returns>Int32类型</returns>
        public static int ToInt32(string str)
        {
            int result = 0;
            if (IsInt(str))
            {
                result = Convert.ToInt32(str);
            }
            return result;
        }

        /// <summary>
        /// 判断一个字符串是否属于Int类型
        /// 如果是的返回true，如果不是返回false
        /// </summary>
        /// <param name="str">string类型的值</param>
        /// <returns>true：是Int的字符串(即可以转换成Int类型)，false：不是Int类型的字符串</returns>
        public static bool IsInt(string str)
        {
            bool result = false;
            if (str != "" && str != null)
            {
                Regex reg = new Regex("^[0-9]*$");
                if (reg.IsMatch(str))
                {
                    result = true;
                }
            }
            return result;
        }
        #endregion

        #region------------------ToString类型转换------------------
        /// <summary>
        ///  读取数据库中字符串并转换成string
        /// </summary>
        /// <param name="obj">object类型的值</param>
        /// <returns>string类型</returns>
        public static string ToString(object obj)
        {
            string result = "";
            if (obj != null)
            {
                result = Convert.ToString(obj);
            }
            return result;
        }
        #endregion

        #region------------------ToDouble类型转换------------------
        /// <summary>
        /// 判断一个字符串是否属于Double类型(包括负浮点型)
        /// 如果是的返回true，如果不是返回false
        /// </summary>
        /// <param name="str">string类型的值</param>
        /// <returns>true：是Double的字符串(即可以转换成Double类型)，false：不是Double类型的字符串</returns>
        public static bool IsDouble(string str)
        {
            bool result = false;
            if (str != "" && str != null)
            {
                Regex reg = new Regex(@"^(-?\d+)(\.\d+)?$");
                if (reg.IsMatch(str))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 读取数据库中字符串并转换成Int32
        /// 为空时返回0
        /// </summary>
        /// <param name="obj">object类型的值</param>
        /// <returns>Int32类型</returns>
        public static double ToDouble(object obj)
        {
            double result = 0.0;
            if (IsDouble(Convert.ToString(obj)))
            {
                result = Convert.ToDouble(obj);
            }
            else if (obj != null && obj is Enum) //处理枚举
            {
                result = ((IConvertible)obj).ToDouble(null);
            }
            return result;
        }

        /// <summary>
        /// 读取数据库中字符串并转换成Int32
        /// 为空时返回0
        /// </summary>
        /// <param name="str">string类型的值</param>
        /// <returns>Int32类型</returns>
        public static double ToDouble(string str)
        {
            double result = 0.0;
            if (IsDouble(str))
            {
                result = Convert.ToDouble(str);
            }
            return result;
        }
        #endregion

        #region------------------ToDateTime类型转换------------------
        /// <summary>
        /// 判断时间格式是否是时间类型
        /// 如23:10
        /// </summary>
        /// <param name="str">要判断的字符串</param>
        /// <returns>true：是时间类型的字符串(即可以转换成时间类型)，false：不是时间类型的字符串</returns>
        public static bool isDateTime(string str)
        {
            bool result = false;
            if (str != "" && str != null)
            {
                Regex reg = new Regex("(([01]\\d)|(2[0-3])):[0-5]\\d");
                if (reg.IsMatch(str))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? toDate(object obj)
        {
            if (obj == null) return null;
            DateTime dt = DateTime.Now;
            if (DateTime.TryParse(obj.ToString(),out dt))
            {
                return dt;
            }
            return null;
        }

        #endregion
    }
}
