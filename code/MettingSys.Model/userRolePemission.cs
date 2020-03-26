using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_userRolePemission, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class userRolePemission
    {
        public userRolePemission()
        { }

        /// <summary>
        /// 构造函数 MS_userRolePemission
        /// </summary>
        /// <param name="urp_id">urp_id</param>
        /// <param name="urp_type">urp_type</param>
        /// <param name="urp_roleid">urp_roleid</param>
        /// <param name="urp_username">urp_username</param>
        /// <param name="urp_code">urp_code</param>
        public userRolePemission(int? urp_id, byte? urp_type, int? urp_roleid, string urp_username, string urp_code)
        {
            this.urp_id = urp_id;
            this.urp_type = urp_type;
            this.urp_roleid = urp_roleid;
            this.urp_username = urp_username;
            this.urp_code = urp_code;
        }

        #region 实体属性

        /// <summary>
        /// urp_id
        /// </summary>
        public int? urp_id { get; set; }

        /// <summary>
        /// urp_type
        /// </summary>
        public byte? urp_type { get; set; }

        /// <summary>
        /// urp_roleid
        /// </summary>
        public int? urp_roleid { get; set; }

        /// <summary>
        /// urp_username
        /// </summary>
        public string urp_username { get; set; }

        /// <summary>
        /// urp_code
        /// </summary>
        public string urp_code { get; set; }

        #endregion
        
    }
}