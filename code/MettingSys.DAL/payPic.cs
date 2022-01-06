using MettingSys.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.DAL
{
    public partial class payPic
    {
        /// <summary>
        /// 添加活动文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public int insertPayFile(Model.payPic model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
                                                     //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_payPic(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("pp_id"))
                {
                    //判断属性值是否为空
                    if (pi.GetValue(model, null) != null)
                    {
                        str1.Append(pi.Name + ",");//拼接字段
                        str2.Append("@" + pi.Name + ",");//声明参数
                        paras.Add(new SqlParameter("@" + pi.Name, pi.GetValue(model, null)));//对参数赋值
                    }
                }
            }
            strSql.Append(str1.ToString().Trim(','));
            strSql.Append(") values (");
            strSql.Append(str2.ToString().Trim(','));
            strSql.Append(") ");
            strSql.Append(";select @@IDENTITY;");
            object obj = DbHelperSQL.GetSingle(strSql.ToString(), paras.ToArray());
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }
        public bool deleteOrderFile(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_payPic ");
            strSql.Append(" where pp_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.VarChar,11)};
            parameters[0].Value = id;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }
        /// <summary>
        /// 删除记录下的全部图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool deleteFileByid(int id, byte? type)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_payPic ");
            strSql.Append(" where pp_type=@type and pp_rid=@rid");
            SqlParameter[] parameters = {
                new SqlParameter("@type", SqlDbType.TinyInt,4),
                new SqlParameter("@rid", SqlDbType.Int,4)};
            parameters[0].Value = type;
            parameters[1].Value = id;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.payPic GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.payPic model = new Model.payPic();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_payPic");
            strSql.Append(" where pp_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;
            DataTable dt = DbHelperSQL.Query(strSql.ToString(), parameters).Tables[0];

            if (dt.Rows.Count > 0)
            {
                return DataRowToFileModel(dt.Rows[0]);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.payPic DataRowToFileModel(DataRow row)
        {
            Model.payPic model = new Model.payPic();
            if (row != null)
            {
                //利用反射获得属性的所有公共属性
                Type modelType = model.GetType();
                for (int i = 0; i < row.Table.Columns.Count; i++)
                {
                    //查找实体是否存在列表相同的公共属性
                    PropertyInfo proInfo = modelType.GetProperty(row.Table.Columns[i].ColumnName);
                    if (proInfo != null && row[i] != DBNull.Value)
                    {
                        proInfo.SetValue(model, row[i], null);//用索引值设置属性值
                    }
                }
            }
            return model;
        }
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(byte? type, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select * FROM  MS_payPic");
            if (type == 1)
            {
                strSql.Append(" left join MS_ReceiptPayDetail on pp_type=1 and pp_rid=rpd_id");
            }
            else if (type == 2)
            {
                strSql.Append(" left join MS_unBusinessApply on pp_type=2 and pp_rid=uba_id");
            }
            else
            {
                strSql.Append(" left join MS_unBusinessApply on pp_type=3 and pp_rid=uba_id");
            }
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            return DbHelperSQL.Query(strSql.ToString());
        }
    }
}
