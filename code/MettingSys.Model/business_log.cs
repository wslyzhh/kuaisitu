using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MettingSys.Model
{
    /// <summary>
    /// 操作类日志
    /// </summary>
    [Serializable]
    public partial class business_log
    {
        public business_log() { }

        #region Model
        private int _ol_id;
        private string _ol_type = string.Empty;
        private string _ol_oid = string.Empty;
        private int _ol_cid = 0;
        private int _ol_relateID = 0;
        private string _ol_title = string.Empty;
        private string _ol_content = string.Empty;
        private string _ol_operaterNum = string.Empty;
        private string _ol_operaterName = string.Empty;
        private DateTime _ol_operateDate = DateTime.Now;
        /// <summary>
        /// 自增ID
        /// </summary>
        public int ol_id
        {
            set { _ol_id = value; }
            get { return _ol_id; }
        }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string ol_type
        {
            set { _ol_type = value; }
            get { return _ol_type; }
        }
        /// <summary>
        /// 订单号
        /// </summary>
        public string ol_oid
        {
            set { _ol_oid = value; }
            get { return _ol_oid; }
        }
        /// <summary>
        /// 客户ID
        /// </summary>
        public int ol_cid
        {
            set { _ol_cid = value; }
            get { return _ol_cid; }
        }
        /// <summary>
        /// 相关表主键ID
        /// </summary>
        public int ol_relateID
        {
            set { _ol_relateID = value; }
            get { return _ol_relateID; }
        }
        
        /// <summary>
        /// 标题
        /// </summary>
        public string ol_title
        {
            set { _ol_title = value; }
            get { return _ol_title; }
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string ol_content
        {
            set { _ol_content = value; }
            get { return _ol_content; }
        }
        /// <summary>
        /// 操作工号
        /// </summary>
        public string ol_operaterNum
        {
            set { _ol_operaterNum = value; }
            get { return _ol_operaterNum; }
        }
        /// <summary>
        /// 操作姓名
        /// </summary>
        public string ol_operaterName
        {
            set { _ol_operaterName = value; }
            get { return _ol_operaterName; }
        }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime ol_operateDate
        {
            set { _ol_operateDate = value; }
            get { return _ol_operateDate; }
        }
        #endregion
    }
}
