using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_method, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class payMethod 
    {
        public payMethod()
        { }

        /// <summary>
        /// 构造函数 MS_method
        /// </summary>
        /// <param name="pm_id">pm_id</param>
        /// <param name="pm_name">pm_name</param>
        /// <param name="pm_type">pm_type</param>
        /// <param name="pm_isuse">pm_isUse</param>
        /// <param name="pm_sort">pm_sort</param>
        public payMethod(int? pm_id, string pm_name, bool? pm_type, bool? pm_isuse,int? pm_sort)
        {
            this.pm_id = pm_id;
            this.pm_name = pm_name;
            this.pm_type = pm_type;
            this.pm_isUse = pm_isuse;
            this.pm_sort = pm_sort;
        }

        #region 实体属性

        /// <summary>
        /// pm_id
        /// </summary>
        public int? pm_id { get; set; }

        /// <summary>
        /// pm_name
        /// </summary>
        public string pm_name { get; set; }

        /// <summary>
        /// pm_type
        /// </summary>
        public bool? pm_type { get; set; }
        
        /// <summary>
        /// pm_isUse
        /// </summary>
        public bool? pm_isUse { get; set; }

        /// <summary>
        /// pm_sort
        /// </summary>
        public int? pm_sort { get; set; }

        #endregion
    }
}