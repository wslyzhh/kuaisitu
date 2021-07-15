using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_OrderPerson, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class OrderPerson 
    {
        public OrderPerson()
        { }

        /// <summary>
        /// 构造函数 MS_OrderPerson
        /// </summary>
        /// <param name="op_id">op_id</param>
        /// <param name="op_oid">op_oid</param>
        /// <param name="op_type">op_type</param>
        /// <param name="op_number">op_number</param>
        /// <param name="op_name">op_name</param>
        /// <param name="op_area">op_area</param>
        public OrderPerson(int? op_id, string op_oid, byte? op_type, string op_number, string op_name, string op_area,byte? op_dstatus,int? op_ratio, DateTime? op_addTime)
        {
            this.op_id = op_id;
            this.op_oid = op_oid;
            this.op_type = op_type;
            this.op_number = op_number;
            this.op_name = op_name;
            this.op_area = op_area;
            this.op_dstatus = op_dstatus;
            this.op_ratio = op_ratio;
            this.op_addTime = op_addTime;
        }

        #region 实体属性

        /// <summary>
        /// op_id
        /// </summary>
        public int? op_id { get; set; }

        /// <summary>
        /// op_oid
        /// </summary>
        public string op_oid { get; set; }

        /// <summary>
        /// op_type
        /// </summary>
        public byte? op_type { get; set; }

        /// <summary>
        /// op_number
        /// </summary>
        public string op_number { get; set; }

        /// <summary>
        /// op_name
        /// </summary>
        public string op_name { get; set; }

        /// <summary>
        /// op_area
        /// </summary>
        public string op_area { get; set; }
        
        /// <summary>
        /// o_dstatus
        /// </summary>
        public byte? op_dstatus { get; set; }

        public int? op_ratio { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime? op_addTime { get; set; }

        #endregion

    }
}