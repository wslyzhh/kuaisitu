using MettingSys.Common;
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
    public partial class Customer
    {
        public Customer()
        {
        }
        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  MS_Customer");
            strSql.Append(" where ");
            strSql.Append(" c_id = @id  ");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public string Add(Model.Customer model, Model.Contacts contact, out int cid)
        {
            cid = 0;
            string result = string.Empty;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        result = insertData(model, contact, conn, trans, out cid);
                        if (string.IsNullOrEmpty(result))
                        {
                            trans.Commit(); //提交事务
                        }
                    }
                    catch
                    {
                        trans.Rollback(); //回滚事务
                        result = "添加失败";
                    }
                }
            }
            return result;
        }
        public string insertData(Model.Customer model, Model.Contacts contact, SqlConnection conn, SqlTransaction trans, out int cid)
        {
            cid = 0;
            DataSet ds = null;
            #region 组织客户表添加语句
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_Customer(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("c_id"))
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
            #endregion
            #region 添加客户==========================
            int obj = 0;
            obj = Convert.ToInt32(DbHelperSQL.GetSingle(conn, trans, strSql.ToString(), paras.ToArray()));//带事务                       
            #endregion
            #region 添加联系人======================
            if (obj > 0)
            {
                //检查联系人号码是否与其他客户的联系人号码一样
                if (model.c_type != 2)
                {
                    string sql = "select * from MS_customer c left join MS_contacts co on c.c_id = co.co_cid where c.c_type<>2 and co.co_number='" + contact.co_number + "'";
                    ds = DbHelperSQL.Query(conn, trans, sql);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        return "客户【" + ds.Tables[0].Rows[0]["c_name"] + "】，联系人【" + ds.Tables[0].Rows[0]["co_name"] + "】的联系号码也是【" + contact.co_number + "】，请查实：是否为同一客户！";
                    }
                }
                cid = obj;
                contact.co_cid = obj;
                int oobj = new DAL.Contacts().Add(contact, conn, trans);
                if (oobj > 0)
                {
                    return "";
                }
            }
            #endregion
            return "添加失败";
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.Customer model, SqlConnection conn = null, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_Customer set ");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("c_id"))
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
            strSql.Append(" where c_id=@id ");
            paras.Add(new SqlParameter("@id", model.c_id));
            if (tran == null)
            {
                return DbHelperSQL.ExecuteSql(strSql.ToString(), paras.ToArray()) > 0;
            }
            else
            {
                return DbHelperSQL.ExecuteSql(conn, tran, strSql.ToString(), paras.ToArray()) > 0;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        private bool Delete(int id, SqlConnection conn = null, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_Customer ");
            strSql.Append(" where c_id=@id");
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
        /// 检查客户是否被使用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool checkIsUse(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from ms_order full join ms_finance on o_cid=fin_cid full join ms_receiptpay on o_cid = rp_cid full join ms_receiptpaydetail on o_cid=rpd_cid full join ms_invoices on o_cid = inv_cid");
            strSql.Append(" where o_cid=@id or fin_cid=@id or rp_cid=@id or rpd_cid=@id or inv_cid=@id");
            SqlParameter[] parameters = {
                   new SqlParameter("@id", SqlDbType.Int,4)
            };
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            StringBuilder strSql = new StringBuilder();
            SqlParameter[] parameters = {
                new SqlParameter("@id",SqlDbType.Int,id)
            };
            parameters[0].Value = id;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 删除客户==========================
                        strSql.Append("delete from  MS_Customer where c_id=@id");
                        DbHelperSQL.ExecuteSql(conn, trans, strSql.ToString(), parameters);
                        #endregion

                        #region 删除客户联系人======================
                        strSql.Clear();
                        strSql.Append("delete from  MS_Contacts where co_cid=@id");
                        DbHelperSQL.ExecuteSql(conn, trans, strSql.ToString(), parameters);
                        #endregion

                        #region 删除客户银行账号======================
                        strSql.Clear();
                        strSql.Append("delete from  MS_CustomerBank where cb_cid=@id");
                        DbHelperSQL.ExecuteSql(conn, trans, strSql.ToString(), parameters);
                        #endregion

                        trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback(); //回滚事务
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.Customer GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.Customer model = new Model.Customer();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_Customer");
            strSql.Append(" where c_id=@id");
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
            strSql.Append(" FROM  MS_Customer c left join MS_Contacts co on c.c_id=co.co_cid and co_flag=1 ");
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount,bool isPage=true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM MS_Customer c left join MS_Contacts co on c.c_id=co.co_cid and co_flag=1 left join dt_manager on c_owner=user_name");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = 0;
            if (isPage)
            {
                recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString() + " order by " + filedOrder);
            }
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string name, int id = 0)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from MS_Customer");
            strSql.Append(" where c_name=@name");
            if (id > 0)
            {
                strSql.Append(" and c_id<>@id");
            }
            SqlParameter[] parameters = {
                   new SqlParameter("@name", SqlDbType.NVarChar,100),
                   new SqlParameter("@id", SqlDbType.Int,4)
            };
            parameters[0].Value = name;
            parameters[1].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.Customer DataRowToModel(DataRow row)
        {
            Model.Customer model = new Model.Customer();
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
        /// 检查是否存在已审批的记录
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool existCheckCount(string ids, byte? status)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from MS_Customer");
            strSql.Append(" where c_id in (" + ids + ") and c_flag=@status");
            SqlParameter[] parameters = {
                   new SqlParameter("@status", SqlDbType.TinyInt,4)
            };
            parameters[0].Value = status;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 更新审批状态
        /// </summary>
        /// <param name="ids">记录ID，多条用“,”隔开</param>
        /// <param name="status">0待审批，1审批未通过，2审批通过</param>
        /// <returns></returns>
        public bool updateStatus(int id, byte? status)
        {
            string sql = "update MS_Customer set c_flag=@status where c_id = @id";
            SqlParameter[] param = {
                new SqlParameter("@status",SqlDbType.TinyInt,4),
                new SqlParameter("@id",SqlDbType.Int,4)
            };
            param[0].Value = status;
            param[1].Value = id;
            return DbHelperSQL.ExecuteSql(sql, param) > 0;
        }
        /// <summary>
        /// 获取客户的具体列
        /// </summary>
        /// <param name="Top"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <returns></returns>
        public DataSet GetNameList(string fields, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (string.IsNullOrEmpty(fields))
            {
                strSql.Append(" * ");
            }
            else
            {
                strSql.Append(fields);
            }
            strSql.Append(" FROM  MS_Customer ");
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
        public bool mergeCustomer(int cid, string cname, Model.Customer model, string username, string realname, bool isUpdateCustomer)
        {
            StringBuilder sqlStr = new StringBuilder();
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 更新客户表==========================
                        bool tag = false;
                        if (isUpdateCustomer)
                        {
                            tag = Update(model, conn, trans);
                        }
                        else
                        {
                            tag = true;
                        }
                        #endregion
                        if (tag)
                        {
                            #region 把源客户的主要联系人变更为目标客户的次要联系人======================
                            sqlStr.Append("update MS_contacts set co_flag=0,co_cid=@cid where co_cid=@cid1 and co_flag=1");
                            SqlParameter[] param = {
                                new SqlParameter("@cid",SqlDbType.Int,4),
                                new SqlParameter("@cid1",SqlDbType.Int,4)
                            };
                            param[0].Value = model.c_id;
                            param[1].Value = cid;
                            DbHelperSQL.ExecuteSql(conn, trans, sqlStr.ToString(), param);
                            #endregion
                            #region 把源客户的次要联系人更新到目标客户下======================
                            sqlStr.Clear();
                            sqlStr.Append("update MS_contacts set co_cid=@cid where co_cid=@cid1");
                            SqlParameter[] param1 = {
                                new SqlParameter("@cid",SqlDbType.Int,4),
                                new SqlParameter("@cid1",SqlDbType.Int,4)
                            };
                            param1[0].Value = model.c_id;
                            param1[1].Value = cid;
                            DbHelperSQL.ExecuteSql(conn, trans, sqlStr.ToString(), param1);
                            #endregion
                            #region 变更应收付地接的客户ID
                            sqlStr.Clear();
                            sqlStr.Append("update MS_finance set fin_cid=@cid where fin_cid=@cid1");
                            SqlParameter[] param2 = {
                                new SqlParameter("@cid",SqlDbType.Int,4),
                                new SqlParameter("@cid1",SqlDbType.Int,4)
                            };
                            param2[0].Value = model.c_id;
                            param2[1].Value = cid;
                            DbHelperSQL.ExecuteSql(conn, trans, sqlStr.ToString(), param2);
                            #endregion
                            #region 变更收付款的客户ID
                            sqlStr.Clear();
                            sqlStr.Append("update ms_receiptpay set rp_cid=@cid where rp_cid=@cid1");
                            SqlParameter[] param3 = {
                                new SqlParameter("@cid",SqlDbType.Int,4),
                                new SqlParameter("@cid1",SqlDbType.Int,4)
                            };
                            param3[0].Value = model.c_id;
                            param3[1].Value = cid;
                            DbHelperSQL.ExecuteSql(conn, trans, sqlStr.ToString(), param3);
                            #endregion
                            #region 变更收付款明细的客户ID
                            sqlStr.Clear();
                            sqlStr.Append("update ms_receiptpaydetail set rpd_cid=@cid where rpd_cid=@cid1");
                            SqlParameter[] param4 = {
                                new SqlParameter("@cid",SqlDbType.Int,4),
                                new SqlParameter("@cid1",SqlDbType.Int,4)
                            };
                            param4[0].Value = model.c_id;
                            param4[1].Value = cid;
                            DbHelperSQL.ExecuteSql(conn, trans, sqlStr.ToString(), param4);
                            #endregion
                            #region 变更发票申请的客户ID
                            sqlStr.Clear();
                            sqlStr.Append("update ms_invoices set inv_cid=@cid where inv_cid=@cid1");
                            SqlParameter[] param5 = {
                                new SqlParameter("@cid",SqlDbType.Int,4),
                                new SqlParameter("@cid1",SqlDbType.Int,4)
                            };
                            param5[0].Value = model.c_id;
                            param5[1].Value = cid;
                            DbHelperSQL.ExecuteSql(conn, trans, sqlStr.ToString(), param5);
                            #endregion
                            #region 变更订单的客户ID
                            sqlStr.Clear();
                            sqlStr.Append("update ms_order set o_cid=@cid where o_cid=@cid1");
                            SqlParameter[] param6 = {
                                new SqlParameter("@cid",SqlDbType.Int,4),
                                new SqlParameter("@cid1",SqlDbType.Int,4)
                            };
                            param6[0].Value = model.c_id;
                            param6[1].Value = cid;
                            DbHelperSQL.ExecuteSql(conn, trans, sqlStr.ToString(), param6);
                            #endregion
                            #region 删除源客户
                            Delete(cid, conn, trans);
                            #endregion
                            #region 删除源客户的银行账号======================
                            sqlStr.Clear();
                            sqlStr.Append("delete from  MS_CustomerBank where cb_cid =@cid");
                            SqlParameter[] param7 = {
                                new SqlParameter("@cid",SqlDbType.Int,4)
                            };
                            param7[0].Value = cid;
                            DbHelperSQL.ExecuteSql(conn, trans, sqlStr.ToString(), param7);
                            #endregion
                            trans.Commit();
                            return true;
                        }
                    }
                    catch
                    {
                        trans.Rollback(); //回滚事务
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 更新所属人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newUsername"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool updateOwner(int id, string newUsername,string name)
        {
            string sql = "update MS_Customer set c_owner=@owner,c_ownerName=@name where c_id = @id";
            SqlParameter[] param = {
                new SqlParameter("@owner",SqlDbType.VarChar,5),
                new SqlParameter("@name",SqlDbType.VarChar,20),
                new SqlParameter("@id",SqlDbType.Int,4)
            };
            param[0].Value = newUsername;
            param[1].Value = name;
            param[2].Value = id;
            return DbHelperSQL.ExecuteSql(sql, param) > 0;
        }
        #endregion
    }
}
