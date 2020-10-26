using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using MettingSys.DBUtility;
using MettingSys.Common;

namespace MettingSys.DAL
{
    /// <summary>
    /// 数据访问类:管理员信息表
    /// </summary>
    public partial class manager
    {
        private string databaseprefix; //数据库表名前缀
        public manager(string _databaseprefix)
        {
            databaseprefix = _databaseprefix;
        }

        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  " + databaseprefix + "manager");
            strSql.Append(" where id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public string Add(Model.manager model, out int mid)
        {
            mid = 0;
            string result = string.Empty;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 主表信息==========================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();//数据字段
                        StringBuilder str2 = new StringBuilder();//数据参数
                                                                 //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<SqlParameter> paras = new List<SqlParameter>();
                        strSql.Append("insert into " + databaseprefix + "manager(");
                        foreach (PropertyInfo pi in pros)
                        {
                            //拼接字段，忽略List<T>
                            if (!typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
                            {
                                //如果不是主键则追加sql字符串
                                if (!pi.Name.Equals("id"))
                                {
                                    //判断属性值是否为空
                                    if (pi.GetValue(model, null) != null && !pi.GetValue(model, null).ToString().Equals(""))
                                    {
                                        str1.Append(pi.Name + ",");//拼接字段
                                        str2.Append("@" + pi.Name + ",");//声明参数
                                        paras.Add(new SqlParameter("@" + pi.Name, pi.GetValue(model, null)));//对参数赋值
                                    }
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(") values (");
                        strSql.Append(str2.ToString().Trim(','));
                        strSql.Append(") ");
                        strSql.Append(";select @@IDENTITY;");
                        object obj = DbHelperSQL.GetSingle(conn, trans, strSql.ToString(), paras.ToArray());//带事务
                        model.id = Convert.ToInt32(obj);
                        mid = model.id;
                        #endregion

                        #region 权限表信息
                        //if (model.UserPemissionList != null)
                        //{
                        //    string sql = "";
                        //    foreach (Model.userRolePemission item in model.UserPemissionList)
                        //    {
                        //        sql = "insert into MS_userRolePemission(urp_type,urp_roleid,urp_username,urp_code) values(@type,0,@name,@code)";
                        //        SqlParameter[] sp = new SqlParameter[] {
                        //            new SqlParameter("@type" , item.urp_type),
                        //            new SqlParameter("@name", item.urp_username),
                        //            new SqlParameter("@code", item.urp_code)
                        //        };
                        //        DbHelperSQL.ExecuteSql(conn, trans, sql, sp);
                        //    }
                        //}
                        #endregion

                        #region 生成内部客户
                        result = AddInnerCustomer(model, conn, trans);
                        if (!string.IsNullOrEmpty(result))
                        {
                            trans.Rollback();
                            return result;
                        }
                        trans.Commit(); //提交事务
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        trans.Rollback(); //回滚事务
                        result = ex.Message;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public string Update(Model.manager model, bool updateName = false, bool updateContact = false)
        {

            string result = string.Empty;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 主表信息==========================
                        StringBuilder strSql = new StringBuilder();
                        StringBuilder str1 = new StringBuilder();
                        //利用反射获得属性的所有公共属性
                        PropertyInfo[] pros = model.GetType().GetProperties();
                        List<SqlParameter> paras = new List<SqlParameter>();
                        strSql.Append("update " + databaseprefix + "manager set ");
                        foreach (PropertyInfo pi in pros)
                        {
                            //拼接字段，忽略List<T>
                            if (!typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
                            {
                                //如果不是主键则追加sql字符串
                                if (!pi.Name.Equals("id"))
                                {
                                    //判断属性值是否为空
                                    if (pi.GetValue(model, null) != null && !pi.GetValue(model, null).ToString().Equals(""))
                                    {
                                        str1.Append(pi.Name + "=@" + pi.Name + ",");//声明参数
                                        paras.Add(new SqlParameter("@" + pi.Name, pi.GetValue(model, null)));//对参数赋值
                                    }
                                }
                            }
                        }
                        strSql.Append(str1.ToString().Trim(','));
                        strSql.Append(" where id=@id ");
                        paras.Add(new SqlParameter("@id", model.id));
                        int obj = DbHelperSQL.ExecuteSql(conn, trans, strSql.ToString(), paras.ToArray());
                        #endregion

                        #region 权限表信息
                        //if (obj > 0)
                        //{
                        //    string sql = "delete from MS_userRolePemission where urp_username=@name";
                        //    SqlParameter[] sp1 = new SqlParameter[] {
                        //            new SqlParameter("@name" , model.user_name)
                        //        };
                        //    DbHelperSQL.ExecuteSql(conn, trans, sql, sp1);
                        //    if (model.UserPemissionList != null)
                        //    {
                        //        sql = "";
                        //        foreach (Model.userRolePemission item in model.UserPemissionList)
                        //        {
                        //            sql = "insert into MS_userRolePemission(urp_type,urp_roleid,urp_username,urp_code) values(@type,0,@name,@code)";
                        //            SqlParameter[] sp = new SqlParameter[] {
                        //                new SqlParameter("@type" , item.urp_type),
                        //                new SqlParameter("@name", item.urp_username),
                        //                new SqlParameter("@code", item.urp_code)
                        //            };
                        //            DbHelperSQL.ExecuteSql(conn, trans, sql, sp);
                        //        }
                        //    }
                        //}
                        #endregion
                        if (obj > 0)
                        {
                            if (updateName)
                            {
                                #region 生成新的内部客户
                                result = AddInnerCustomer(model, conn, trans);
                                if (!string.IsNullOrEmpty(result))
                                {
                                    trans.Rollback();
                                    return result;
                                }
                                #endregion
                            }
                            else
                            {
                                string sql = "";
                                if (updateContact)
                                {
                                    #region 更新原来的内部客户
                                    sql = "update a set a.co_number='" + model.telephone + "' from MS_Contacts a left join MS_Customer on co_cid = c_id and co_flag=1 where c_name ='员工-" + model.real_name + "(" + model.user_name + ")'";
                                    DbHelperSQL.Query(conn, trans, sql);
                                    #endregion
                                }
                            }
                        }

                        #region 更新用户钉钉授权绑定状态
                        if (model.is_lock == 0)
                        {
                            new DAL.manager_oauth(databaseprefix).UpdateField(conn, trans, model.id, "is_lock=0");
                        }
                        #endregion

                        trans.Commit(); //提交事务
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback(); //回滚事务
                        return ex.Message;
                    }
                }
            }
            return result;
        }

        public string AddInnerCustomer(Model.manager model,SqlConnection conn, SqlTransaction trans)
        {
            StringBuilder strSql1 = new StringBuilder();
            strSql1.Append("select count(*) from ms_customer where c_name=@name and c_type=3");
            SqlParameter[] pa = new SqlParameter[] {
                                    new SqlParameter("@name",SqlDbType.VarChar,50)
                                };
            pa[0].Value = "员工-" + model.real_name + "(" + model.user_name + ")";
            int obj = Convert.ToInt32(DbHelperSQL.GetSingle(conn, trans, strSql1.ToString(), pa));
            if (obj == 0)
            {
                Model.Customer cu = new Model.Customer();
                cu.c_name = "员工-" + model.real_name + "(" + model.user_name + ")";
                cu.c_type = 3;
                cu.c_flag = 2;
                cu.c_isUse = true;
                cu.c_owner = model.user_name;
                cu.c_ownerName = model.real_name;
                cu.c_addDate = DateTime.Now;

                Model.Contacts co = new Model.Contacts();
                co.co_flag = true;
                co.co_name = model.real_name;
                co.co_number = model.telephone;
                int cid = 0;
                return new DAL.Customer().insertData(cu, co, conn, trans, out cid);
            }
            return "";
        }
        /// <summary>
        /// 检查员工工号是否被使用
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool checkIsUse(string username)
        {
            bool result = false;
            StringBuilder strSql = new StringBuilder();
            //strSql.Append("select count(*) from MS_OrderPerson full join MS_Files on op_number=f_addPerson full join MS_Customer on op_number=c_owner ");
            //strSql.Append(" full join MS_unBusinessApply on op_number=uba_checkNum1 or op_number=uba_checkNum2 or op_number=uba_checkNum3 or op_number=uba_confirmerNum or op_number=uba_PersonNum");
            //strSql.Append(" full join MS_finance on op_number=fin_checkNum or op_number=fin_personNum");
            //strSql.Append(" full join MS_ReceiptPay on op_number=rp_personNum or op_number=rp_checkNum or op_number=rp_checkNum1 or op_number=rp_confirmerNum");
            //strSql.Append(" full join MS_ReceiptPayDetail on op_number=rpd_personNum or op_number=rpd_checkNum1 or op_number=rpd_checkNum2 or op_number=rpd_checkNum3");
            //strSql.Append(" full join MS_invoices on op_number=inv_personNum or op_number=inv_checkNum1 or op_number=inv_checkNum2 or op_number=inv_checkNum3 or op_number=inv_confirmerNum");
            //strSql.Append(" where op_number=@num or f_addPerson=@num or c_owner=@num or uba_checkNum1=@num or uba_checkNum2=@num or uba_checkNum3=@num or uba_confirmerNum=@num or uba_PersonNum=@num");
            //strSql.Append(" or fin_checkNum=@num or fin_personNum=@num or rp_personNum=@num or rp_checkNum=@num or rp_checkNum1=@num or rp_confirmerNum=@num or rpd_personNum=@num or rpd_checkNum1=@num");
            //strSql.Append(" or rpd_checkNum1=@num or rpd_checkNum2=@num or rpd_checkNum3=@num or inv_personNum=@num or inv_checkNum1=@num or inv_checkNum2=@num or inv_checkNum3=@num or inv_confirmerNum=@num");

            SqlParameter[] parameters = {
                   new SqlParameter("@num", SqlDbType.VarChar,5)
            };
            parameters[0].Value = username;
            strSql.Append("select count(*) from MS_OrderPerson where op_number=@num");
            result = DbHelperSQL.Exists(strSql.ToString(), parameters);
            if (result)
            {
                return result;
            }

            strSql.Clear();
            strSql.Append("select count(*) from MS_Files where f_addPerson=@num");
            result = DbHelperSQL.Exists(strSql.ToString(), parameters);
            if (result)
            {
                return result;
            }

            //strSql.Clear();
            //strSql.Append("select count(*) from MS_Customer where c_owner=@num");
            //result = DbHelperSQL.Exists(strSql.ToString(), parameters);
            //if (result)
            //{
            //    return result;
            //}

            strSql.Clear();
            strSql.Append("select count(*) from MS_unBusinessApply where uba_checkNum1=@num or uba_checkNum2=@num or uba_checkNum3=@num or uba_confirmerNum=@num or uba_PersonNum=@num");
            result = DbHelperSQL.Exists(strSql.ToString(), parameters);
            if (result)
            {
                return result;
            }

            strSql.Clear();
            strSql.Append("select count(*) from MS_finance where fin_checkNum=@num or fin_personNum=@num");
            result = DbHelperSQL.Exists(strSql.ToString(), parameters);
            if (result)
            {
                return result;
            }

            strSql.Clear();
            strSql.Append("select count(*) from MS_ReceiptPay where rp_personNum=@num or rp_checkNum=@num or rp_checkNum1=@num or rp_confirmerNum=@num");
            result = DbHelperSQL.Exists(strSql.ToString(), parameters);
            if (result)
            {
                return result;
            }

            strSql.Clear();
            strSql.Append("select count(*) from MS_ReceiptPayDetail where rpd_personNum=@num or rpd_checkNum1=@num or rpd_checkNum2=@num or rpd_checkNum3=@num");
            result = DbHelperSQL.Exists(strSql.ToString(), parameters);
            if (result)
            {
                return result;
            }

            strSql.Clear();
            strSql.Append("select count(*) from MS_invoices where inv_personNum=@num or inv_checkNum1=@num or inv_checkNum2=@num or inv_checkNum3=@num or inv_confirmerNum=@num");
            result = DbHelperSQL.Exists(strSql.ToString(), parameters);
            if (result)
            {
                return result;
            }
            return result;
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            bool result = false;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 权限表信息                        
                        //string sql = "delete urp from MS_userRolePemission urp left join dt_manager on urp_username=user_name where id=@id";
                        //SqlParameter[] sp1 = new SqlParameter[] {
                        //        new SqlParameter("@id" , id)
                        //    };
                        //DbHelperSQL.ExecuteSql(conn, trans, sql, sp1);
                        #endregion

                        #region 主表信息==========================
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("delete from  " + databaseprefix + "manager ");
                        strSql.Append(" where id=@id");
                        SqlParameter[] parameters = {
                            new SqlParameter("@id", SqlDbType.Int,4)};
                        parameters[0].Value = id;
                        result = DbHelperSQL.ExecuteSql(conn, trans, strSql.ToString(), parameters) > 0;
                        #endregion

                        #region 员工账户授权表
                        new DAL.manager_oauth(databaseprefix).Delete(conn, trans, id);
                        #endregion

                        trans.Commit(); //提交事务
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback(); //回滚事务
                        return false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.manager GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.manager model = new Model.manager();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                //拼接字段，忽略List<T>
                if (!typeof(System.Collections.IList).IsAssignableFrom(p.PropertyType))
                {
                    str1.Append(p.Name + ",");//拼接字段
                }
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from " + databaseprefix + "manager");
            strSql.Append(" where id=@id");
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
            strSql.Append(" FROM  " + databaseprefix + "manager ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return DbHelperSQL.Query(strSql.ToString());
        }
        /// <summary>
        /// 返回某些区域中含有某个权限的人员
        /// </summary>
        /// <param name="code">0501</param>
        /// <param name="area">SY,SH,YN</param>
        /// <returns></returns>
        public DataSet getUserByPermission(string code, string area="")
        {
            SqlParameter[] parameters = {
                    new SqlParameter("@code", SqlDbType.VarChar,20),
                    new SqlParameter("@area", SqlDbType.VarChar,4)
            };
            parameters[0].Value = code;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select m.user_name,m.real_name,m.area,isnull(mo.oauth_userid,'') oauth_userid from dt_manager_oauth mo left join dt_manager m on m.user_name = mo.manager_name and mo.is_lock=1 left join MS_userRolePemission on m.role_id = urp_roleid where urp_code=@code ");
            if (!string.IsNullOrEmpty(area))
            {
                strSql.Append(" and area=@area");
                parameters[1].Value = area;
            }
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }


        public DataSet getUserByUserName(string username)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select m.user_name,m.real_name,m.area,mo.oauth_userid from dt_manager_oauth mo left join dt_manager m  on m.user_name = mo.manager_name and mo.is_lock = 1 where isnull(mo.oauth_userid, '') <> '' and  user_name in ('" + username.Replace(",","','") + "')");

            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select m.*,isnull(mo.manager_id,0) manager_id,isnull(mo.oauth_userid,'') oauth_userid FROM " + databaseprefix + "manager m left join dt_manager_oauth mo on user_name = mo.manager_name and mo.is_lock = 1");
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
        /// 查询用户名是否存在
        /// </summary>
        public bool Exists(string user_name, int id = 0)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + databaseprefix + "manager");
            strSql.Append(" where user_name=@user_name ");
            if (id > 0)
            {
                strSql.Append("and id<>@id");
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@user_name", SqlDbType.NVarChar,100),
                    new SqlParameter("@id", SqlDbType.Int,4)
            };
            parameters[0].Value = user_name;
            parameters[1].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 根据用户名取得Salt
        /// </summary>
        public string GetSalt(string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 salt from " + databaseprefix + "manager");
            strSql.Append(" where user_name=@user_name");
            SqlParameter[] parameters = {
                    new SqlParameter("@user_name", SqlDbType.NVarChar,100)};
            parameters[0].Value = user_name;
            string salt = Convert.ToString(DbHelperSQL.GetSingle(strSql.ToString(), parameters));
            if (string.IsNullOrEmpty(salt))
            {
                return string.Empty;
            }
            return salt;
        }

        /// <summary>
        /// 根据用户名密码返回一个实体
        /// </summary>
        public Model.manager GetModel(string user_name, string password)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id from " + databaseprefix + "manager");
            strSql.Append(" where user_name=@user_name and password=@password and is_lock=0");
            SqlParameter[] parameters = {
                    new SqlParameter("@user_name", SqlDbType.NVarChar,100),
                    new SqlParameter("@password", SqlDbType.NVarChar,100)};
            parameters[0].Value = user_name;
            parameters[1].Value = password;

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if (obj != null)
            {
                return GetModel(Convert.ToInt32(obj));
            }
            return null;
        }
        /// <summary>
        /// 根据用户名密码返回一个实体
        /// </summary>
        public Model.manager GetModel(string user_name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id from " + databaseprefix + "manager");
            strSql.Append(" where user_name=@user_name ");
            SqlParameter[] parameters = {
                    new SqlParameter("@user_name", SqlDbType.NVarChar,100)};
            parameters[0].Value = user_name;

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if (obj != null)
            {
                return GetModel(Convert.ToInt32(obj));
            }
            return null;
        }
        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.manager DataRowToModel(DataRow row)
        {
            Model.manager model = new Model.manager();
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

                #region 员工权限======================
                //StringBuilder strSql2 = new StringBuilder();
                //strSql2.Append("select * from MS_userRolePemission");
                //strSql2.Append(" where urp_username=@name");
                //SqlParameter[] parameters2 = {
                //        new SqlParameter("@name", SqlDbType.VarChar,20)};
                //parameters2[0].Value = model.user_name;

                //DataTable dt2 = DbHelperSQL.Query(strSql2.ToString(), parameters2).Tables[0];
                //if (dt2.Rows.Count > 0)
                //{
                //    int rowsCount = dt2.Rows.Count;
                //    List<Model.userRolePemission> models = new List<Model.userRolePemission>();
                //    Model.userRolePemission modelt;
                //    for (int n = 0; n < rowsCount; n++)
                //    {
                //        modelt = new Model.userRolePemission();
                //        Type modeltType = modelt.GetType();
                //        for (int i = 0; i < dt2.Rows[n].Table.Columns.Count; i++)
                //        {
                //            PropertyInfo proInfo = modeltType.GetProperty(dt2.Rows[n].Table.Columns[i].ColumnName);
                //            if (proInfo != null && dt2.Rows[n][i] != DBNull.Value)
                //            {
                //                proInfo.SetValue(modelt, dt2.Rows[n][i], null);
                //            }
                //        }
                //        models.Add(modelt);
                //    }
                //    model.UserPemissionList = models;
                //}
                #endregion

                #region 员工的角色权限======================
                StringBuilder strSql3 = new StringBuilder();
                strSql3.Append("select * from MS_userRolePemission");
                strSql3.Append(" where urp_roleid=@name");
                SqlParameter[] parameters3 = {
                        new SqlParameter("@name", SqlDbType.VarChar,20)};
                parameters3[0].Value = model.role_id;

                DataTable dt3 = DbHelperSQL.Query(strSql3.ToString(), parameters3).Tables[0];
                if (dt3.Rows.Count > 0)
                {
                    int rowsCount = dt3.Rows.Count;
                    List<Model.userRolePemission> models = new List<Model.userRolePemission>();
                    Model.userRolePemission modelt;
                    for (int n = 0; n < rowsCount; n++)
                    {
                        modelt = new Model.userRolePemission();
                        Type modeltType = modelt.GetType();
                        for (int i = 0; i < dt3.Rows[n].Table.Columns.Count; i++)
                        {
                            PropertyInfo proInfo = modeltType.GetProperty(dt3.Rows[n].Table.Columns[i].ColumnName);
                            if (proInfo != null && dt3.Rows[n][i] != DBNull.Value)
                            {
                                proInfo.SetValue(modelt, dt3.Rows[n][i], null);
                            }
                        }
                        models.Add(modelt);
                    }
                    model.RolePemissionList = models;
                }
                #endregion
            }
            return model;
        }
        #endregion
    }
}