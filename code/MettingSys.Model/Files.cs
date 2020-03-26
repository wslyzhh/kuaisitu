using System;

namespace MettingSys.Model
{
    /// <summary>
    /// 实体类 MS_Files, 此类请勿动，以方便表字段更改时重新生成覆盖
    /// </summary>
    [Serializable]
    public partial class Files 
    {
        public Files()
        { }

        /// <summary>
        /// 构造函数 MS_Files
        /// </summary>
        /// <param name="f_id">f_id</param>
        /// <param name="f_oid">f_oid</param>
        /// <param name="f_filename">f_fileName</param>
        /// <param name="f_filepath">f_filePath</param>
        /// <param name="f_adddate">f_addDate</param>
        /// <param name="f_addperson">f_addPerson</param>
        /// <param name="f_addname">f_addName</param>
        public Files(int? f_id, string f_oid, string f_filename, string f_filepath, decimal? f_size, DateTime? f_adddate, string f_addperson, string f_addname)
        {
            this.f_id = f_id;
            this.f_oid = f_oid;
            this.f_fileName = f_filename;
            this.f_filePath = f_filepath;
            this.f_size = f_size;
            this.f_addDate = f_adddate;
            this.f_addPerson = f_addperson;
            this.f_addName = f_addname;
        }

        #region 实体属性

        /// <summary>
        /// f_id
        /// </summary>
        public int? f_id { get; set; }

        /// <summary>
        /// f_oid
        /// </summary>
        public string f_oid { get; set; }

        /// <summary>
        /// f_type
        /// </summary>
        public byte? f_type { get; set; }

        /// <summary>
        /// f_fileName
        /// </summary>
        public string f_fileName { get; set; }

        /// <summary>
        /// f_filePath
        /// </summary>
        public string f_filePath { get; set; }

        /// <summary>
        /// f_size
        /// </summary>
        public decimal? f_size { get; set; }

        /// <summary>
        /// f_addDate
        /// </summary>
        public DateTime? f_addDate { get; set; }

        /// <summary>
        /// f_addPerson
        /// </summary>
        public string f_addPerson { get; set; }

        /// <summary>
        /// f_addName
        /// </summary>
        public string f_addName { get; set; }

        #endregion
        
    }
}