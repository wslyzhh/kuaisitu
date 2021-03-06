﻿using MettingSys.Common;
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
    public partial class Contacts
    {
        public Contacts()
        {
        }
        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  MS_Contacts");
            strSql.Append(" where ");
            strSql.Append(" co_id = @id  ");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.Contacts model, SqlConnection conn = null, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_Contacts(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("co_id"))
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
            object obj;
            if (tran == null)
            {
                obj = DbHelperSQL.GetSingle(strSql.ToString(), paras.ToArray());
            }
            else
            {
                obj = DbHelperSQL.GetSingle(conn, tran, strSql.ToString(), paras.ToArray());
            }
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.Contacts model, SqlConnection conn = null, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_Contacts set ");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("co_id"))
                {
                    //判断属性值是否为空
                    if (pi.GetValue(model, null) != null)
                    {
                        str1.Append(pi.Name + "=@" + pi.Name + ",");//声明参数
                        paras.Add(new SqlParameter("@" + pi.Name, pi.GetValue(model, null)));//对参数赋值
                    }
                }
            }
            strSql.Append(str1.ToString().Trim(','));
            strSql.Append(" where co_id=@id ");
            paras.Add(new SqlParameter("@id", model.co_id));
            if (tran == null)
            {
                return DbHelperSQL.ExecuteSql(strSql.ToString(), paras.ToArray()) > 0;
            }
            else
            {
                return DbHelperSQL.ExecuteSql(conn,tran,strSql.ToString(), paras.ToArray()) > 0;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id, SqlConnection conn = null, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_Contacts ");
            strSql.Append(" where co_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            if (tran == null)
            {
                return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters.ToArray()) > 0;
            }
            else
            {
                return DbHelperSQL.ExecuteSql(conn, tran, strSql.ToString(), parameters.ToArray()) > 0;
            }
        }
        
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.Contacts GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.Contacts model = new Model.Contacts();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_Contacts");
            strSql.Append(" where co_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;
            DataTable dt = DbHelperSQL.Query(strSql.ToString(), parameters).Tables[0];

            if (dt.Rows.Count > 0)
            {
                return DataRowToModel(dt.Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM  MS_Contacts left join ms_customer on co_cid=c_id ");
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
        
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM MS_Contacts");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public DataSet Exists(string phone,int id=0)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from MS_Contacts co left join MS_customer c on co.co_cid = c.c_id");
            strSql.Append(" where co_number=@num");
            if (id > 0)
            {
                strSql.Append(" and co_id<>@id");
            }
            SqlParameter[] parameters = {
                   new SqlParameter("@num",SqlDbType.VarChar,100),
                   new SqlParameter("@id",SqlDbType.Int,4)
            };
            parameters[0].Value = phone;
            parameters[1].Value = id;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <returns></returns>
        public List<Model.Contacts> getList(string strWhere, string filedOrder)
        {
            string sql = "select * from MS_Contacts  ";
            if (!string.IsNullOrEmpty(strWhere))
            {
                sql += " where " + strWhere;
            }
            if (!string.IsNullOrEmpty(filedOrder))
            {
                sql += " order by " + filedOrder;
            }
            List<Model.Contacts> list = new List<Model.Contacts>();
            Model.Contacts model = null;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader sdr = cmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        model = new Model.Contacts();
                        model.co_id = Convert.ToInt32(sdr["co_id"]);
                        model.co_cid = Convert.ToInt32(sdr["co_cid"]);
                        model.co_flag = Convert.ToBoolean(sdr["co_flag"]);
                        model.co_name = sdr["co_name"].ToString();
                        model.co_number = sdr["co_number"].ToString();
                        list.Add(model);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 是否存在主要联系人
        /// </summary>
        public bool ExistsMainContact(int cid, int id = 0)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from MS_Contacts");
            strSql.Append(" where co_cid=@cid and co_flag=1");
            if (id > 0)
            {
                strSql.Append(" and co_id<>@id");
            }
            SqlParameter[] parameters = {
                   new SqlParameter("@id", SqlDbType.Int,4),
                   new SqlParameter("@cid", SqlDbType.Int,4)
            };
            parameters[0].Value = id;
            parameters[1].Value = cid;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.Contacts DataRowToModel(DataRow row)
        {
            Model.Contacts model = new Model.Contacts();
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
        /// 检查联系人是否被使用
        /// </summary>
        /// <param name="coid"></param>
        /// <returns></returns>
        public bool checkIsUse(int coid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from MS_Order where o_coid=@coid");

            SqlParameter[] parameters = {
                   new SqlParameter("@coid", SqlDbType.Int,4)
            };
            parameters[0].Value = coid;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }
        #endregion
    }
}
