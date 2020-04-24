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
    public partial class ReceiptPayDetail
    {
        public ReceiptPayDetail() { }
        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  MS_ReceiptPayDetail");
            strSql.Append(" where ");
            strSql.Append(" rpd_id = @id  ");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.ReceiptPayDetail model, SqlConnection conn = null, SqlTransaction tran = null)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_ReceiptPayDetail(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("rpd_id"))
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
            object obj = 0;
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
        /// 增加收款明细
        /// </summary>
        public int Add(Model.ReceiptPayDetail model,Model.ReceiptPay rp)
        {
            int rpid = 0, rpdid = 0;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 插入收款通知======================
                        if (rp != null)
                        {
                            rpid = new DAL.ReceiptPay().Add(rp, conn, trans);
                        }
                        #endregion

                        #region 插入收付款明细==========================  
                        
                        model.rpd_rpid = rpid;
                        rpdid = Add(model, conn, trans);
                        if (rpdid > 0)
                        {
                            trans.Commit();
                        }
                        #endregion                        
                    }
                    catch
                    {
                        trans.Rollback(); //回滚事务
                        rpdid = 0;
                    }
                }
            }
            return rpdid;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.ReceiptPayDetail model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_ReceiptPayDetail set ");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("rpd_id"))
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
            strSql.Append(" where rpd_id=@id ");
            paras.Add(new SqlParameter("@id", model.rpd_id));
            return DbHelperSQL.ExecuteSql(strSql.ToString(), paras.ToArray()) > 0;
        }

        public int mutliUpdateMethod(int cid, int method, int oldmethod, string sdate, string edate, Model.manager manager)
        {
            string sql = "update MS_ReceiptPayDetail set rpd_method=@method where rpd_type=0 and rpd_flag3=2 and rpd_flag2=2 and rpd_flag1=2 and isnull(rpd_rpid,0)=0 and rpd_cid=@cid and isnull(rpd_method,0)=@oldmethod";
            if (!string.IsNullOrEmpty(sdate))
            {
                sql += " and datediff(d,rpd_foreDate,'" + sdate + "')<=0";
            }
            if (!string.IsNullOrEmpty(edate))
            {
                sql += " and datediff(d,rpd_foreDate,'" + edate + "')>=0";
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@method", SqlDbType.Int,4),
                    new SqlParameter("@cid", SqlDbType.Int,4),
                    new SqlParameter("@oldmethod", SqlDbType.Int,4)
            };
            parameters[0].Value = method;
            parameters[1].Value = cid;
            parameters[2].Value = oldmethod;

            return DbHelperSQL.ExecuteSql(sql.ToString(), parameters);

        }

        /// <summary>
        /// 修改收款明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateReceiptDetail(Model.ReceiptPayDetail model,bool updateMoney)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_ReceiptPayDetail set ");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("rpd_id"))
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
            strSql.Append(" where rpd_id=@id ");
            paras.Add(new SqlParameter("@id", model.rpd_id));
            bool result = false;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 修改收款明细======================
                        int obj = DbHelperSQL.ExecuteSql(conn, trans, strSql.ToString(), paras.ToArray());
                        #endregion

                        #region 修改收款通知==========================
                        if (obj > 0)
                        {
                            string sql = "select count(*) from MS_ReceiptPayDetail where rpd_rpid=@rpid";
                            List<SqlParameter> paras0 = new List<SqlParameter>();
                            paras0.Add(new SqlParameter("@rpid", model.rpd_rpid));
                            int count = Convert.ToInt32(DbHelperSQL.GetSingle(conn, trans, sql, paras0.ToArray()));
                            if (count > 0)
                            {
                                if (count == 1)
                                {
                                    //只有一条明细时，把收款对象，收款内容，收款金额，预收日期，收款方式一并更新到收款通知
                                    sql = "update MS_ReceiptPay set rp_cid=@cid,rp_content=@content,rp_money=@money,rp_foredate=@foredate,rp_method=@method where rp_id=@rpid and rp_isConfirm=0";
                                    List<SqlParameter> paras1 = new List<SqlParameter>();
                                    paras1.Add(new SqlParameter("@cid", model.rpd_cid));
                                    paras1.Add(new SqlParameter("@content", model.rpd_content));
                                    paras1.Add(new SqlParameter("@money", model.rpd_money));
                                    paras1.Add(new SqlParameter("@foredate", model.rpd_foredate));
                                    paras1.Add(new SqlParameter("@method", model.rpd_method));
                                    paras1.Add(new SqlParameter("@rpid", model.rpd_rpid));
                                    DbHelperSQL.ExecuteSql(conn, trans, sql, paras1.ToArray());
                                }
                                else
                                {
                                    //有多条明细时，只变更总金额到收款通知
                                    if (updateMoney)
                                    {
                                        sql = "update MS_ReceiptPay set rp_money=(select SUM(rpd_money) from MS_ReceiptPayDetail where rpd_rpid=@rpid) where rp_id=@rpid and rp_isConfirm=0";
                                        DbHelperSQL.ExecuteSql(conn, trans, sql, paras0.ToArray());
                                    }
                                }
                            }
                            trans.Commit();
                            result = true;
                        }
                        #endregion
                    }
                    catch(Exception err)
                    {
                        trans.Rollback(); //回滚事务
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 取消分配
        /// </summary>
        /// <param name="rpid"></param>
        /// <param name="oid"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public int delDistribution(int rpid,string oid,int cid,string isChk,string action="cancel")
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_ReceiptPayDetail ");
            strSql.Append(" where rpd_rpid=@rpid and rpd_oid=@oid and rpd_cid=@cid");
            if (!string.IsNullOrEmpty(isChk))
            {
                //存在对账标识
                if (action == "add")
                {
                    //把对账标识为空的分配删除
                    strSql.Append(" and (isnull(rpd_num,'')='' or rpd_num=@num)");
                }
                else
                {
                    strSql.Append(" and rpd_num=@num");
                }
            }
            else
            {
                //不存在对账标识
                if (action == "add")
                {
                    //把对账标识不为空的分配删除
                    //strSql.Append(" and isnull(rpd_num,'')<>''");
                }
                else
                {
                    strSql.Append(" and isnull(rpd_num,'')=''");
                }
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@rpid", SqlDbType.Int,4),
                    new SqlParameter("@oid", SqlDbType.Char,11),
                    new SqlParameter("@cid", SqlDbType.Int,4),
                    new SqlParameter("@num",SqlDbType.VarChar,100)
            };
            parameters[0].Value = rpid;
            parameters[1].Value = oid;
            parameters[2].Value = cid;
            parameters[3].Value = isChk;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_ReceiptPayDetail ");
            strSql.Append(" where rpd_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }
        /// <summary>
        /// 删除收款明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteReceiptDetail(Model.ReceiptPayDetail model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_ReceiptPayDetail ");
            strSql.Append(" where rpd_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = model.rpd_id;
            bool result = false;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = "select count(*) from MS_ReceiptPayDetail where rpd_rpid=@rpid";
                        List<SqlParameter> paras0 = new List<SqlParameter>();
                        paras0.Add(new SqlParameter("@rpid", model.rpd_rpid));
                        int count = Convert.ToInt32(DbHelperSQL.GetSingle(conn, trans, sql, paras0.ToArray()));

                        #region 删除收款明细======================
                        int obj = DbHelperSQL.ExecuteSql(conn, trans, strSql.ToString(), parameters.ToArray());
                        #endregion

                        #region 修改收款通知==========================
                        if (obj > 0)
                        {                            
                            if (count > 0)
                            {
                                if (count == 1)
                                {
                                    //只有一条明细时，把收款通知删除
                                    sql = "delete from MS_ReceiptPay where rp_id=@rpid and rp_isConfirm=0";
                                    List<SqlParameter> paras1 = new List<SqlParameter>();
                                    paras1.Add(new SqlParameter("@rpid", model.rpd_rpid));
                                    DbHelperSQL.ExecuteSql(conn, trans, sql, paras1.ToArray());
                                }
                                else
                                {
                                    //有多条明细时，只变更总金额到收款通知                                    
                                    sql = "update MS_ReceiptPay set rp_money=(select SUM(rpd_money) from MS_ReceiptPayDetail where rpd_rpid=@rpid) where rp_id=@rpid and rp_isConfirm=0";
                                    DbHelperSQL.ExecuteSql(conn, trans, sql, paras0.ToArray());
                                    
                                }
                            }
                            trans.Commit();
                            result = true;
                        }
                        #endregion
                    }
                    catch (Exception err)
                    {
                        trans.Rollback(); //回滚事务
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 计算某个客户在订单中的未付金额
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public decimal? getUnPayMoney(int cid, string oid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select (isnull(m1,0)-isnull(m2,0)) m from ");
            strSql.Append(" (select fin_cid,sum(isnull(fin_money,0)) m1 from MS_finance where fin_type='False' and fin_cid=@cid and fin_oid=@oid group by fin_cid) t1");
            strSql.Append(" left join ");
            strSql.Append(" (select rpd_cid,sum(isnull(rpd_money,0)) m2 FROM MS_ReceiptPayDetail where rpd_type='False' and rpd_cid=@cid and rpd_oid=@oid group by rpd_cid) t2 ");
            strSql.Append(" on t1.fin_cid = t2.rpd_cid");
            SqlParameter[] parameters = {
                    new SqlParameter("@cid", SqlDbType.Int,4),
                    new SqlParameter("@oid",SqlDbType.VarChar,11)
                    };
            parameters[0].Value = cid;
            parameters[1].Value = oid;
            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            return Utils.ObjToDecimal(obj, 0);
        }

        /// <summary>
        /// 付款明细审批
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public int checkPayDetailStatus(int id, byte? type, byte? status, string remark, string username, string realname)
        {
            string updateField = "rpd_flag1", updateField1 = "rpd_checkNum1", updateField2 = "rpd_checkName1", updateField3 = "rpd_checkRemark1";
            if (type == 2)
            {
                updateField = "rpd_flag2";
                updateField1 = "rpd_checkNum2";
                updateField2 = "rpd_checkName2";
                updateField3 = "rpd_checkRemark2";
            }
            else if (type == 3)
            {
                updateField = "rpd_flag3";
                updateField1 = "rpd_checkNum3";
                updateField2 = "rpd_checkName3";
                updateField3 = "rpd_checkRemark3";
            }
            string sql = "update MS_ReceiptPayDetail set " + updateField + "=@status," + updateField1 + "=@num," + updateField2 + "=@name," + updateField3 + "=@remark where rpd_id=@rpid";
            SqlParameter[] param = {
                new SqlParameter("@status",SqlDbType.TinyInt,4),
                new SqlParameter("@num",SqlDbType.Char,5),
                new SqlParameter("@name",SqlDbType.VarChar,20),
                new SqlParameter("@remark",SqlDbType.VarChar,200),
                new SqlParameter("@rpid",SqlDbType.Int,4)
            };
            param[0].Value = status;
            param[1].Value = username;
            param[2].Value = realname;
            param[3].Value = remark;
            param[4].Value = id;
            return DbHelperSQL.ExecuteSql(sql, param);
        }

        /// <summary>
        /// 检查是否存在相同状态的记录
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int existCheckCount(int id, byte? type, byte? status, bool tag = true)
        {
            string sql = "select * from MS_ReceiptPayDetail where rpd_id =" + id + "";
            if (type == 1)
            {
                if (tag)
                {
                    sql += " and rpd_flag1=" + status;
                }
                else
                {
                    sql += " and rpd_flag1<>" + status;
                }
            }
            else if (type == 2)
            {
                if (tag)
                {
                    sql += " and rpd_flag2=" + status;
                }
                else
                {
                    sql += " and rpd_flag2<>" + status;
                }
            }
            else
            {
                if (tag)
                {
                    sql += " and rpd_flag3=" + status;
                }
                else
                {
                    sql += " and rpd_flag3<>" + status;
                }
            }
            DataSet ds = DbHelperSQL.Query(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0].Rows.Count;
            }
            return 0;
        }

        /// <summary>
        /// 取消汇总
        /// </summary>
        /// <param name="rpdid"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool cancelCollect(DataRow dr)
        {
            bool result = false;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = "";
                        List<SqlParameter> paras= new List<SqlParameter>();
                        if (Utils.ObjToDecimal(dr["rp_money"], 0) > Utils.ObjToDecimal(dr["rpd_money"], 0))
                        {
                            //如果付款通知的金额大于付款明细的金额，则更新付款通知的金额
                            sql = "update MS_ReceiptPay set rp_money=@money where rp_id=@rpid";
                            paras.Add(new SqlParameter("@money", Utils.ObjToDecimal(dr["rp_money"], 0) - Utils.ObjToDecimal(dr["rpd_money"], 0)));
                            paras.Add(new SqlParameter("@rpid", dr["rp_id"]));
                            DbHelperSQL.ExecuteSql(conn, trans, sql, paras.ToArray());
                        }
                        else
                        {
                            //如果付款通知的金额等于付款明细的金额，则删除付款通知
                            sql = "delete MS_ReceiptPay where rp_id=@rpid";
                            paras.Add(new SqlParameter("@rpid", dr["rp_id"]));
                            DbHelperSQL.ExecuteSql(conn, trans, sql, paras.ToArray());
                        }
                        paras.Clear();
                        sql = "update MS_ReceiptPayDetail set rpd_rpid=0 where rpd_id=@rpdid";
                        paras.Add(new SqlParameter("@rpdid", dr["rpd_id"]));
                        DbHelperSQL.ExecuteSql(conn, trans, sql, paras.ToArray());
                        trans.Commit();
                        result = true;
                    }
                    catch (Exception err)
                    {
                        trans.Rollback(); //回滚事务
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        public bool Delete(string idStr)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_ReceiptPayDetail ");
            strSql.Append(" where rpd_id in (" + idStr + ")");
            SqlParameter[] parameters = { };


            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }

        /// <summary>
        /// 汇总付款明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int collectPayDetails(Model.ReceiptPay model,int method, string sdate, string edate)
        {
            int rpid = 0;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 插入付款通知======================
                        rpid = new DAL.ReceiptPay().Add(model, conn, trans);                        
                        #endregion

                        #region 修改收款明细中的rpd_rpid==========================
                        if (rpid > 0)
                        {
                            string sql = "update MS_ReceiptPayDetail set rpd_rpid=@rpid,rpd_method=@newmethod where rpd_type=0 and rpd_flag3=2 and rpd_flag2=2 and rpd_flag1=2 and isnull(rpd_rpid,0)=0 and rpd_cid=@cid and isnull(rpd_method,0)=@method and isnull(rpd_cbid,0)=@cbid";
                            if (!string.IsNullOrEmpty(sdate))
                            {
                                sql += " and datediff(d,rpd_foreDate,'" + sdate + "')<=0";
                            }
                            if (!string.IsNullOrEmpty(edate))
                            {
                                sql += " and datediff(d,rpd_foreDate,'" + edate + "')>=0";
                            }
                            List<SqlParameter> paras1 = new List<SqlParameter>();
                            paras1.Add(new SqlParameter("@rpid", rpid));
                            paras1.Add(new SqlParameter("@cid", model.rp_cid));
                            paras1.Add(new SqlParameter("@method", method));
                            paras1.Add(new SqlParameter("@newmethod", model.rp_method));
                            paras1.Add(new SqlParameter("@cbid", model.rp_cbid));
                            DbHelperSQL.ExecuteSql(conn, trans, sql, paras1.ToArray());

                            trans.Commit();                            
                        }
                        #endregion
                    }
                    catch (Exception err)
                    {
                        trans.Rollback(); //回滚事务
                        rpid = 0;
                    }
                }
            }
            return rpid;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.ReceiptPayDetail GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.ReceiptPayDetail model = new Model.ReceiptPayDetail();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_ReceiptPayDetail");
            strSql.Append(" where rpd_id=@id");
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
            strSql.Append(" FROM  MS_ReceiptPayDetail left join MS_ReceiptPay on rpd_rpid=rp_id left join MS_customer on rpd_cid=c_id left join MS_payMethod on rpd_method=pm_id left join MS_certificates on rp_ceid = ce_id left join MS_customerBank on rpd_cbid=cb_id  left join ms_order on rpd_oid=o_id");
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, out decimal pmoney, bool isPay=false,bool isPage=true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM MS_ReceiptPayDetail left join MS_customer on rpd_cid=c_id ");
            if (isPay)
            {
                strSql.Append(" left join MS_ReceiptPay on rpd_rpid=rp_id left join MS_order on rpd_oid=o_id left join MS_payMethod on rpd_method=pm_id left join MS_customerBank on rpd_cbid=cb_id");
            }
            else {
                strSql.Append(" left join MS_ReceiptPay on rpd_rpid=rp_id left join MS_payMethod on rpd_method=pm_id left join MS_order on rpd_oid=o_id");
            }
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = 0;pmoney = 0;
            if (isPage)
            {
                DataTable dt = DbHelperSQL.Query("select count(*) c,sum(rpd_money) tRpdMoney from (" + strSql.ToString() + ") t").Tables[0];
                if (dt != null)
                {
                    recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                    pmoney = Utils.ObjToDecimal(dt.Rows[0]["tRpdMoney"], 0);
                }
                //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString());
            }
        }

        public DataSet GetPayCertificationList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM MS_ReceiptPayDetail left join MS_customer on rpd_cid=c_id ");
            strSql.Append(" left join MS_ReceiptPay on rpd_rpid=rp_id left join MS_order on rpd_oid=o_id left join MS_payMethod on rpd_method=pm_id");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperSQL.Query(strSql.ToString());            
        }

        /// <summary>
        /// 获取付款明细汇总数据 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getCollectList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount,out decimal tmoney)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select t.* from (select rpd_cid,c_name,isnull(rpd_cbid,0)rpd_cbid,isnull(cb_bank+'('+cb_bankName+'/'+ cb_bankNum +')','') cbname,isnull(rpd_method,0) rpd_method,isnull(pm_name,'') pm_name,count(*) c,sum(rpd_money) as total from MS_ReceiptPayDetail left join MS_payMethod on rpd_method=pm_id left join MS_customer on rpd_cid=c_id left join MS_customerBank on rpd_cbid=cb_id where rpd_type=0 and rpd_flag3=2 and rpd_flag2=2 and rpd_flag1=2 and isnull(rpd_rpid,0)=0 ");
            if (!string.IsNullOrEmpty(strWhere))
            {
                strSql.Append(" and " + strWhere);
            }
            recordCount = 0;tmoney = 0;
            strSql.Append(" group by rpd_cid,c_name,rpd_cbid,cb_bank,cb_bankName,cb_bankNum,isnull(rpd_method,0),isnull(pm_name,'')) as t");
            //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataTable dt = DbHelperSQL.Query("select count(*) c,sum(total) t from (" + strSql.ToString() + ") v").Tables[0];
            if (dt!=null)
            {
                recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                tmoney = Utils.ObjToDecimal(dt.Rows[0]["t"], 0);
            }
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }
        #endregion

        #region 扩展方法================================        
        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.ReceiptPayDetail DataRowToModel(DataRow row)
        {
            Model.ReceiptPayDetail model = new Model.ReceiptPayDetail();
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
        #endregion
    }
}
