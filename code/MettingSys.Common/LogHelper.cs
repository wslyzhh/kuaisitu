using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.Common
{
    /// <summary>
    /// 日志文件存放文件夹分类枚举
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 普通信息
        /// </summary>
        Info,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// 其他全部信息
        /// </summary>
        Overall,
    }
    public class LogHelper
    {
        /// <summary>
        /// 日志存放路径
        /// </summary>
        public static string LogPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + @"\log\"+ DateTime.Now.ToString("MM");
            }
        }        
        /// <summary>
        /// 写日志的最终执行动作
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="prefix">前缀</param>
        /// <param name="message">内容</param>
        public static void WriteLog(string path, string prefix, string message,string messageDetail)
        {
            path = LogPath + path;
            var fileName = string.Format("{0}{1}.log", prefix, DateTime.Now.ToString("yyyyMMdd"));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (!File.Exists(path+@"\"+ fileName))
            {
                File.Create(path + @"\" + fileName);
            }

            using (FileStream fs = new FileStream(path + @"\" + fileName, FileMode.Append, FileAccess.Write,
                                                  FileShare.ReadWrite, 1024, FileOptions.Asynchronous))
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes("\r\n" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + " " + message + "\r\n===================================\r\n" + messageDetail+ "\r\n");
                IAsyncResult writeResult = fs.BeginWrite(buffer, 0, buffer.Length,
                    (asyncResult) =>
                    {
                        var fStream = (FileStream)asyncResult.AsyncState;
                        fStream.EndWrite(asyncResult);
                    },

                    fs);
                fs.Close();
            }
        }
    }
}
