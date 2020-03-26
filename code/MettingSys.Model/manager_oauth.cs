using System;

namespace MettingSys.Model
{
    /// <summary>
    /// OAuth授权用户信息
    /// </summary>
    [Serializable]
    public partial class manager_oauth
    {
        public manager_oauth() { }

        #region Model
        private int _id;
        private int _manager_id = 0;
        private string _manager_name = string.Empty;
        private string _oauth_name = string.Empty;
        private string _oauth_access_token = string.Empty;
        private string _oauth_userid = string.Empty;
        private int _is_lock = 0;
        private DateTime _add_time = DateTime.Now;
        /// <summary>
        /// 自增ID
        /// </summary>
        public int id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int manager_id
        {
            set { _manager_id = value; }
            get { return _manager_id; }
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string manager_name
        {
            set { _manager_name = value; }
            get { return _manager_name; }
        }
        /// <summary>
        /// 开放平台名称
        /// </summary>
        public string oauth_name
        {
            set { _oauth_name = value; }
            get { return _oauth_name; }
        }
        /// <summary>
        /// access_token
        /// </summary>
        public string oauth_access_token
        {
            set { _oauth_access_token = value; }
            get { return _oauth_access_token; }
        }
        /// <summary>
        /// 授权key
        /// </summary>
        public string oauth_userid
        {
            set { _oauth_userid = value; }
            get { return _oauth_userid; }
        }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public int is_lock
        {
            set { _is_lock = value; }
            get { return _is_lock; }
        }

        /// <summary>
        /// 授权时间
        /// </summary>
        public DateTime add_time
        {
            set { _add_time = value; }
            get { return _add_time; }
        }
        #endregion
    }
}
