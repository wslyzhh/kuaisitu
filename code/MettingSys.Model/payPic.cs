using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 payPic, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class payPic
    {
        public payPic()
        { }

        /// <summary>
        /// 构造函数 payPic
        /// </summary>
        /// <param name="pp_id">pp_id</param>
        /// <param name="pp_type">pp_type</param>
        /// <param name="pp_rid">pp_rid</param>
        /// <param name="pp_filename">pp_fileName</param>
        /// <param name="pp_filepath">pp_filePath</param>
        /// <param name="pp_adddate">pp_addDate</param>
        /// <param name="pp_addperson">pp_addPerson</param>
        /// <param name="pp_addname">pp_addName</param>
        public payPic(int? pp_id, byte? pp_type, int? pp_rid, string pp_filename, string pp_filepath,string pp_thumbFilePath, decimal? pp_size, DateTime? pp_adddate, string pp_addperson, string pp_addname)
        {
            this.pp_id = pp_id;
            this.pp_type = pp_type;
            this.pp_rid = pp_rid;
            this.pp_fileName = pp_filename;
            this.pp_filePath = pp_filepath;
            this.pp_thumbFilePath = pp_thumbFilePath;
            this.pp_size = pp_size;
            this.pp_addDate = pp_adddate;
            this.pp_addPerson = pp_addperson;
            this.pp_addName = pp_addname;
        }

        #region 实体属性

        /// <summary>
        /// pp_id
        /// </summary>
        public int? pp_id { get; set; }

        /// <summary>
        /// pp_type
        /// </summary>
        public byte? pp_type { get; set; }

        /// <summary>
        /// pp_rid
        /// </summary>
        public int? pp_rid { get; set; }

        /// <summary>
        /// pp_fileName
        /// </summary>
        public string pp_fileName { get; set; }

        /// <summary>
        /// pp_filePath
        /// </summary>
        public string pp_filePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string pp_thumbFilePath { get; set; }

        /// <summary>
        /// pp_size
        /// </summary>
        public decimal? pp_size { get; set; }

        /// <summary>
        /// pp_addDate
        /// </summary>
        public DateTime? pp_addDate { get; set; }

        /// <summary>
        /// pp_addPerson
        /// </summary>
        public string pp_addPerson { get; set; }

        /// <summary>
        /// pp_addName
        /// </summary>
        public string pp_addName { get; set; }

        #endregion        
    }
}