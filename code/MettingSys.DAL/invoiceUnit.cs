﻿using MettingSys.Common;
using MettingSys.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MettingSys.DAL
{
    /// <summary>
    /// 数据访问类开票单位
    /// </summary>
    public partial class invoiceUnit
    {
        public invoiceUnit()
        {
        }
        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  MS_invoiceUnit");
            strSql.Append(" where ");
            strSql.Append(" invU_id = @id  ");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.invoiceUnit model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_invoiceUnit(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("invU_id"))
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

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.invoiceUnit model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_invoiceUnit set ");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("invU_id"))
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
            strSql.Append(" where invU_id=@id ");
            paras.Add(new SqlParameter("@id", model.invU_id));
            return DbHelperSQL.ExecuteSql(strSql.ToString(), paras.ToArray()) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_invoiceUnit ");
            strSql.Append(" where invU_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }

        /// <summary>
        /// 根据区域获取开票单位
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public DataTable getUnitbyArea(string area)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select invU_id,invU_name FROM  MS_invoiceUnit where invU_area=@area and invU_flag=1");
            SqlParameter[] parameters = {
                    new SqlParameter("@area", SqlDbType.Char,2)};
            parameters[0].Value = area;
            return DbHelperSQL.Query(strSql.ToString(),parameters).Tables[0];
        }

        public bool isUse(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from MS_invoices ");
            strSql.Append(" where inv_unit=@id ");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;
            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.invoiceUnit GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.invoiceUnit model = new Model.invoiceUnit();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_invoiceUnit");
            strSql.Append(" where invU_id=@id");
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
            strSql.Append(" FROM  MS_invoiceUnit ");
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
            strSql.Append("select * FROM MS_invoiceUnit");
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
        public bool Exists(string area,string unit, int id = 0)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from MS_invoiceUnit");
            strSql.Append(" where invU_area=@area and invU_name=@name");
            if (id > 0)
            {
                strSql.Append(" and invU_id<>@id");
            }
            SqlParameter[] parameters = {
                   new SqlParameter("@area", SqlDbType.NVarChar,100),
                   new SqlParameter("@name", SqlDbType.NVarChar,100),
                   new SqlParameter("@id", SqlDbType.Int,4)
            };
            parameters[0].Value = area;
            parameters[1].Value = unit;
            parameters[2].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.invoiceUnit DataRowToModel(DataRow row)
        {
            Model.invoiceUnit model = new Model.invoiceUnit();
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
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(int id, string strValue)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update MS_invoiceUnit set " + strValue);
            strSql.Append(" where invU_id=" + id);
            return DbHelperSQL.ExecuteSql(strSql.ToString()) > 0;
        }
        #endregion
    }
}
