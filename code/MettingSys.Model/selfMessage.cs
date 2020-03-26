using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_message, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public class selfMessage 
    {
        public selfMessage()
        { }

        /// <summary>
        /// 构造函数 MS_message
        /// </summary>
        /// <param name="me_id">me_id</param>
        /// <param name="me_title">me_title</param>
        /// <param name="me_content">me_content</param>
        /// <param name="me_isread">me_isRead</param>
        /// <param name="me_owner">me_owner</param>
        /// <param name="me_ownername">me_ownerName</param>
        /// <param name="me_adddate">me_addDate</param>
        public selfMessage(int? me_id, string me_title, string me_content, bool? me_isread, string me_owner, string me_ownername, DateTime? me_adddate)
        {
            this.me_id = me_id;
            this.me_title = me_title;
            this.me_content = me_content;
            this.me_isRead = me_isread;
            this.me_owner = me_owner;
            this.me_ownerName = me_ownername;
            this.me_addDate = me_adddate;
        }

        #region 实体属性

        /// <summary>
        /// me_id
        /// </summary>
        public int? me_id { get; set; }

        /// <summary>
        /// me_title
        /// </summary>
        public string me_title { get; set; }

        /// <summary>
        /// me_content
        /// </summary>
        public string me_content { get; set; }

        /// <summary>
        /// me_isRead
        /// </summary>
        public bool? me_isRead { get; set; }

        /// <summary>
        /// me_owner
        /// </summary>
        public string me_owner { get; set; }

        /// <summary>
        /// me_ownerName
        /// </summary>
        public string me_ownerName { get; set; }

        /// <summary>
        /// me_addDate
        /// </summary>
        public DateTime? me_addDate { get; set; }

        #endregion
        
    }
}