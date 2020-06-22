using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using MettingSys.DBUtility;
using MettingSys.Common;
using MettingSys.Model;

namespace MettingSys.DAL
{
    /// <summary>
    /// 数据访问类:订单表
    /// </summary>
    public partial class Order
    {
        public Order() { }
        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int cid,string content,DateTime? sdate,string id="")
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  MS_Order");
            strSql.Append(" where o_cid=@cid and o_content=@content and o_sdate=@sdate");
            if (!string.IsNullOrEmpty(id))
            {
                strSql.Append(" and o_id <> @id  ");
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@cid", SqlDbType.Int,4),
                    new SqlParameter("@content", SqlDbType.VarChar,200),
                    new SqlParameter("@sdate", SqlDbType.DateTime,20),
                    new SqlParameter("@id", SqlDbType.VarChar,11)
            };
            parameters[0].Value = cid;
            parameters[1].Value = content;
            parameters[2].Value = sdate;
            if (!string.IsNullOrEmpty(id))
            {
                parameters[3].Value = id;
            }

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool AddOrder(Model.Order model)
        {
            #region 插入订单表语句
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_Order(");
            foreach (PropertyInfo pi in pros)
            {
                //拼接字段，忽略List<T>
                if (!typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
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
            #endregion
            bool result = false;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 插入订单表======================                        
                        int obj = DbHelperSQL.ExecuteSql(conn, trans, strSql.ToString(), paras.ToArray());
                        #endregion

                        #region 插入人员表==========================
                        if (obj > 0)
                        {
                            string sql = "insert into MS_OrderPerson(op_oid,op_type,op_number,op_name,op_area,op_dstatus) values(@oid,@type,@number,@name,@area,@status)";
                            foreach (OrderPerson person in model.personlist)
                            {
                                person.op_oid = model.o_id;
                                SqlParameter[] meter = new SqlParameter[] {
                                    new SqlParameter("@oid",SqlDbType.VarChar,11),
                                    new SqlParameter("@type",SqlDbType.TinyInt,4),
                                    new SqlParameter("@number",SqlDbType.VarChar,5),
                                    new SqlParameter("@name",SqlDbType.VarChar,20),
                                    new SqlParameter("@area",SqlDbType.VarChar,2),
                                    new SqlParameter("@status",SqlDbType.TinyInt,4)
                                };
                                meter[0].Value = person.op_oid;
                                meter[1].Value = person.op_type;
                                meter[2].Value = person.op_number;
                                meter[3].Value = person.op_name;
                                meter[4].Value = person.op_area;
                                meter[5].Value = person.op_dstatus;
                                DbHelperSQL.ExecuteSql(conn, trans, sql, meter);
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
        /// 编辑订单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSent"></param>
        /// <returns></returns>
        public bool UpdateOrder(Model.Order model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_Order set ");
            foreach (PropertyInfo pi in pros)
            {
                //拼接字段，忽略List<T>
                if (!typeof(System.Collections.IList).IsAssignableFrom(pi.PropertyType))
                {
                    //如果不是主键则追加sql字符串
                    if (!pi.Name.Equals("o_id"))
                    {
                        //判断属性值是否为空
                        if (pi.GetValue(model, null) != null)
                        {
                            str1.Append(pi.Name + "=@" + pi.Name + ",");//声明参数
                            paras.Add(new SqlParameter("@" + pi.Name, pi.GetValue(model, null)));//对参数赋值
                        }
                    }
                }
            }
            strSql.Append(str1.ToString().Trim(','));
            strSql.Append(" where o_id=@id ");
            paras.Add(new SqlParameter("@id", model.o_id));
            bool result = false;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 更新订单表======================                        
                        int obj = DbHelperSQL.ExecuteSql(conn, trans, strSql.ToString(), paras.ToArray());
                        #endregion

                        #region 更新人员表==========================
                        if (obj > 0)
                        {
                            //先删除就的人员
                            string sql = "delete from MS_OrderPerson where op_oid=@oid";
                            SqlParameter[] meter1 = new SqlParameter[] {
                                new SqlParameter("@oid",SqlDbType.VarChar,11)
                            };
                            meter1[0].Value = model.o_id;
                            DbHelperSQL.ExecuteSql(conn, trans, sql, meter1);

                            sql = "insert into MS_OrderPerson(op_oid,op_type,op_number,op_name,op_area,op_dstatus) values(@oid,@type,@number,@name,@area,@status)";
                            foreach (OrderPerson person in model.personlist)
                            {
                                person.op_oid = model.o_id;
                                SqlParameter[] meter = new SqlParameter[] {
                                    new SqlParameter("@oid",SqlDbType.VarChar,11),
                                    new SqlParameter("@type",SqlDbType.TinyInt,4),
                                    new SqlParameter("@number",SqlDbType.VarChar,5),
                                    new SqlParameter("@name",SqlDbType.VarChar,20),
                                    new SqlParameter("@area",SqlDbType.VarChar,2),
                                    new SqlParameter("@status",SqlDbType.TinyInt,4)
                                };
                                meter[0].Value = person.op_oid;
                                meter[1].Value = person.op_type;
                                meter[2].Value = person.op_number;
                                meter[3].Value = person.op_name;
                                meter[4].Value = person.op_area;
                                meter[5].Value = person.op_dstatus;
                                DbHelperSQL.ExecuteSql(conn, trans, sql, meter);
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
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.Order model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_Order set ");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("o_id"))
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
            strSql.Append(" where o_id=@id ");
            paras.Add(new SqlParameter("@id", model.o_id));
            return DbHelperSQL.ExecuteSql(strSql.ToString(), paras.ToArray()) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_Order ");
            strSql.Append(" where o_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.VarChar,11)};
            parameters[0].Value = id;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.Order GetModel(string id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.Order model = new Model.Order();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_Order");
            strSql.Append(" where o_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.VarChar,11)};
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
        /// 检查订单中是否存在上传文件
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public bool checkOrderFiles(string oid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from MS_Files where f_oid=@oid");
            SqlParameter[] parameters = {
                    new SqlParameter("@oid", SqlDbType.VarChar,11)};
            parameters[0].Value = oid;
            return DbHelperSQL.Exists(strSql.ToString(),parameters);
        }

        /// <summary>
        /// 检查订单中是否存在财务信息
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public bool checkOrderFinance(string oid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from MS_finance full join MS_ReceiptPayDetail on fin_oid=rpd_oid full join MS_invoices on fin_oid=inv_oid full join MS_unBusinessApply on fin_oid=uba_oid where fin_oid=@oid or rpd_oid=@oid or inv_oid=@oid or uba_oid=@oid");
            SqlParameter[] parameters = {
                    new SqlParameter("@oid", SqlDbType.VarChar,11)};
            parameters[0].Value = oid;
            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public bool deleteOrder(string oid)
        {
            bool result = false;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = "delete from MS_Order where o_id=@oid";
                        #region 删除订单表======================                        
                        SqlParameter[] parameters = {
                             new SqlParameter("@oid", SqlDbType.VarChar,11)};
                        parameters[0].Value = oid;
                        int obj = DbHelperSQL.ExecuteSql(conn, trans, sql, parameters);
                        #endregion

                        sql = "delete from MS_OrderPerson where op_oid =@oid";
                        #region 删除人员表==========================
                        if (obj > 0)
                        {
                            DbHelperSQL.ExecuteSql(conn, trans, sql, parameters);
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
            strSql.Append(" FROM  MS_Order left join ms_customer on o_cid=c_id left join ms_contacts on o_coid=co_id left join ms_orderperson on o_id=op_oid and op_type=1");
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
        /// 获取某个区域已推送未审批的订单数量
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public int getUnAduitOrder(string area)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from MS_Order left join MS_OrderPerson on o_id = op_oid and op_type=1 where o_isPush='True' and o_flag=0 and o_lockStatus='0' and op_area=@area");
            SqlParameter[] parameters = {
                    new SqlParameter("@area", SqlDbType.VarChar,11)};
            parameters[0].Value = area;
            return Utils.ObjToInt(DbHelperSQL.GetSingle(strSql.ToString(), parameters),0);
        }

        /// <summary>
        /// 获取业务支付未审核数量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public int getUnAduitPay(byte type, string area)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from MS_ReceiptPayDetail where 1=1 ");
            if (type == 1)
            {
                strSql.Append(" and rpd_area=@area and rpd_flag1=0");
            }
            else if (type == 2)
            {
                strSql.Append(" and rpd_flag1=2 and rpd_flag2=0");
            }
            else
            {
                strSql.Append(" and rpd_flag1=2 and rpd_flag2=2 and rpd_flag3=0");
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@area", SqlDbType.VarChar,11)};
            parameters[0].Value = area;
            return Utils.ObjToInt(DbHelperSQL.GetSingle(strSql.ToString(), parameters), 0);
        }

        /// <summary>
        /// 获取非业务支付未审核数量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public int getUnAduitUnBusinessPay(byte type, string area)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from MS_unBusinessApply where 1=1 ");
            if (type == 1)
            {
                strSql.Append(" and uba_area=@area and uba_flag1=0");
            }
            else if (type == 2)
            {
                strSql.Append(" and uba_flag1=2 and uba_flag2=0");
            }
            else
            {
                strSql.Append(" and uba_flag1=2 and uba_flag2=2 and uba_flag3=0");
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@area", SqlDbType.VarChar,11)};
            parameters[0].Value = area;
            return Utils.ObjToInt(DbHelperSQL.GetSingle(strSql.ToString(), parameters), 0);
        }

        /// <summary>
        /// 获取发票未审核数量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public int getUnAduitInvoice(byte type, string area)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from MS_invoices where 1=1 ");
            if (type == 1)
            {
                strSql.Append(" and (inv_farea=@area and inv_flag1=0) or (inv_darea = @area and inv_flag2=0)");
            }
            else
            {
                strSql.Append(" and inv_flag1=2 and inv_flag2=2 and inv_flag3=0");
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@area", SqlDbType.VarChar,11)};
            parameters[0].Value = area;
            return Utils.ObjToInt(DbHelperSQL.GetSingle(strSql.ToString(), parameters), 0);
        }
        /// <summary>
        /// 获取预付款未审核数量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public int getUnAduitExpectPay(byte type)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from MS_ReceiptPay where 1=1 ");
            if (type == 1)
            {
                strSql.Append(" and rp_flag=0");
            }
            else
            {
                strSql.Append(" and rp_flag=2 and rp_flag1 = 0");
            }
            SqlParameter[] parameters = {};
            return Utils.ObjToInt(DbHelperSQL.GetSingle(strSql.ToString(), parameters), 0);
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <param name="type">默认false,当为true时为订单上的未收款订单和多付款订单</param>
        /// <returns></returns>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount,bool type=false,string where="",bool isPage = true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select *,unMoney=finMoney - rpdMoney,profit=finMoney-finMoney1-isnull(o_financeCust,0) from (select *,person2 = (SELECT op_name FROM MS_OrderPerson WHERE op_oid=o_id and op_type=2),person3 = isnull(STUFF((SELECT ',' + op_name+'('+(case when op_dstatus=0 then '待定' else case when op_dstatus=1 then '处理中' else '已完成' end end)+')' FROM MS_OrderPerson WHERE  op_oid=o_id and op_type=3 FOR XML PATH('')), 1, 1, ''),'无')");
            strSql.Append(" , person4 = isnull(STUFF((SELECT ',' + op_name + '(' + (case when op_dstatus = 0 then '待定' else case when op_dstatus = 1 then '处理中' else '已完成' end end) + ')' FROM MS_OrderPerson WHERE  op_oid = o_id and op_type = 5 FOR XML PATH('')), 1, 1, ''),'无') ,finMoney=isnull((select sum(isnull(fin_money,0)) fin_money from MS_finance where fin_type=1 and fin_oid=o_id),0),finMoney1=isnull((select sum(isnull(fin_money,0)) fin_money from MS_finance where fin_type=0 and fin_oid=o_id),0),rpdMoney = isnull((select sum(isnull(rpd_money,0)) rpd_money from MS_ReceiptPayDetail left join MS_ReceiptPay on rp_id=rpd_rpid where rpd_type=1 and rp_isConfirm=1 and rpd_oid=o_id),0) FROM MS_Order left join ms_customer on o_cid=c_id left join ms_contacts on o_coid=co_id left join ms_orderperson on o_id=op_oid and op_type=1) t");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            string sql = string.Empty;
            if (type)
            {
                sql = "select * from (" + strSql.ToString() + ") v left join (";
                sql += " select isnull(fin_oid,rpd_oid) fin_oid,isnull(fin_type,rpd_type) fin_type,isnull(finMoney,0) finMoney,isnull(rpdMoney,0) rpdMoney from ";
                sql += " (select fin_oid, fin_type, sum(isnull(fin_money, 0)) finMoney from MS_finance where fin_flag <> 1 group by fin_oid, fin_type) t1";
                sql += " full join(select rpd_oid, rpd_type, sum(isnull(rpd_money, 0)) rpdMoney from MS_ReceiptPayDetail left join MS_ReceiptPay on rp_id = rpd_rpid where rp_isConfirm = 1 group by rpd_oid, rpd_type) t3 on t1.fin_oid = t3.rpd_oid and t1.fin_type = t3.rpd_type";
                sql += " left join MS_Order on isnull(fin_oid, rpd_oid) = o_id where 1 = 1";
                sql += " ) v3 on v.o_id = v3.fin_oid where isnull(fin_oid,'')<>'' "+ where;
            }
            else
            {
                sql = strSql.ToString();
            }
            recordCount = 0;
            if (isPage)
            {
                recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(sql)));
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, sql, filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(sql.ToString());
            }
        }
        #endregion

        /// <summary>
        /// 计算订单结算汇总
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public DataTable getOrderCollect(string oid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select o.o_content,o_sdate,o_edate,isnull(o_financeCust,0) o_financeCust,c_name,t.* from ( ");
            strSql.Append(" select isnull(fin_cid,rpd_cid) fin_cid,isnull(fin_type,rpd_type) fin_type,isnull(fin_oid,rpd_oid) fin_oid,isnull(finMoney,0) finMoney,isnull(profit,0) profit,isnull(rpdMoney,0) rpdMoney,isnull(isnull(finMoney,0)-isnull(rpdMoney,0),0) unReceiptPay from");
            strSql.Append(" (select fin_cid,fin_type,fin_oid,sum(isnull(fin_money,0)) finMoney,sum(case when fin_type=1 then fin_money else 0-fin_money end) profit from MS_finance where fin_oid=@oid group by fin_cid,fin_type,fin_oid) t1");
            strSql.Append(" full join ");
            strSql.Append(" (select rpd_cid,rpd_type,rpd_oid,sum(isnull(rpd_money,0)) rpdMoney from MS_ReceiptPayDetail left join MS_ReceiptPay on rpd_rpid=rp_id where rpd_oid=@oid and rp_isConfirm =1 group by rpd_cid,rpd_type,rpd_oid) t2 on t1.fin_cid=t2.rpd_cid and t1.fin_type=t2.rpd_type and t1.fin_oid=t2.rpd_oid");
            strSql.Append(" ) t left join MS_Customer c on fin_cid=c.c_id left join MS_Order o on fin_oid=o_id order by fin_type desc,c_name ");
            SqlParameter[] parameters = {
                    new SqlParameter("@oid", SqlDbType.VarChar,11)};
            parameters[0].Value = oid;
            return DbHelperSQL.Query(strSql.ToString(), parameters).Tables[0];
        }

        /// <summary>
        /// 计算存在未审应收付地接的订单数量
        /// </summary>
        /// <returns></returns>
        public int getUnCheckOrderCount(string _edate1,string _status,string _dstatus,string _flag)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from MS_Order");
            strSql.Append(" where exists(select * from ms_finance where fin_oid=o_id) and exists(select * from MS_finance where fin_oid=o_id and fin_flag=0)");
            if (!string.IsNullOrEmpty(_edate1))
            {
                strSql.Append(" and datediff(s,o_edate,'" + _edate1 + "')>=0");
            }
            if (!string.IsNullOrEmpty(_status))
            {
                switch (_status)
                {
                    case "3":
                        strSql.Append(" and (o_status=1 or o_status=2)");
                        break;
                    default:
                        strSql.Append(" and o_status=" + _status + "");
                        break;
                }
            }
            if (!string.IsNullOrEmpty(_dstatus))
            {
                switch (_dstatus)
                {
                    case "2":
                        strSql.Append(" and exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5)) and not exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5) and (op_dstatus=0 or op_dstatus=1))");
                        break;
                    case "3":
                        strSql.Append(" and not exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5))");
                        break;
                    case "4":
                        strSql.Append(" and ((exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5)) and not exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5) and (op_dstatus=0 or op_dstatus=1))) or not exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5)))");
                        break;
                    default:
                        strSql.Append(" and exists(select * from ms_orderperson where op_oid=o_id and (op_type=3 or op_type=5) and op_dstatus=" + _dstatus + ")");
                        break;
                }
            }
            if (!string.IsNullOrEmpty(_flag))
            {
                strSql.Append(" and o_flag=" + _flag + "");
            }
            SqlParameter[] parameters = {};
            return Utils.ObjToInt(DbHelperSQL.GetSingle(strSql.ToString(), parameters));
        }

        /// <summary>
        /// 计算应收付地接全部审批通过的未锁订单数量
        /// </summary>
        /// <returns></returns>
        public int getUnLockOrderCount()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from MS_Order");
            strSql.Append(" where o_lockStatus=0 and exists(select * from ms_finance where fin_oid=o_id) and not exists(select * from MS_finance where fin_oid=o_id and (fin_flag=0 or fin_flag=1))");
            SqlParameter[] parameters = { };
            return Utils.ObjToInt(DbHelperSQL.GetSingle(strSql.ToString(), parameters));
        }

        /// <summary>
        /// 计算应收付地接全部审批通过的待处理订单数量
        /// </summary>
        /// <returns></returns>
        public int getUnDealOrderCount()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from MS_Order");
            strSql.Append(" where o_lockStatus=2 ");
            SqlParameter[] parameters = { };
            return Utils.ObjToInt(DbHelperSQL.GetSingle(strSql.ToString(), parameters));
        }


        #region 扩展方法================================

        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.Order DataRowToModel(DataRow row)
        {
            Model.Order model = new Model.Order();
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

            #region 订单人员======================
            StringBuilder strSql2 = new StringBuilder();
            strSql2.Append("select * from MS_OrderPerson");
            strSql2.Append(" where op_oid=@oid");
            SqlParameter[] parameters2 = {
                    new SqlParameter("@oid", SqlDbType.VarChar,20)};
            parameters2[0].Value = model.o_id;

            DataTable dt2 = DbHelperSQL.Query(strSql2.ToString(), parameters2).Tables[0];
            if (dt2.Rows.Count > 0)
            {
                int rowsCount = dt2.Rows.Count;
                List<Model.OrderPerson> models = new List<Model.OrderPerson>();
                Model.OrderPerson modelt;
                for (int n = 0; n < rowsCount; n++)
                {
                    modelt = new Model.OrderPerson();
                    Type modeltType = modelt.GetType();
                    for (int i = 0; i < dt2.Rows[n].Table.Columns.Count; i++)
                    {
                        PropertyInfo proInfo = modeltType.GetProperty(dt2.Rows[n].Table.Columns[i].ColumnName);
                        if (proInfo != null && dt2.Rows[n][i] != DBNull.Value)
                        {
                            proInfo.SetValue(modelt, dt2.Rows[n][i], null);
                        }
                    }
                    models.Add(modelt);
                }
                model.personlist = models;
            }
            #endregion

            return model;
        }
        #endregion

        #region 订单文件相关
        /// <summary>
        /// 获取订单活动文件
        /// </summary>
        public DataSet GetFileList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM  MS_Files left join ms_order on f_oid=o_id");
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
        /// 添加订单活动文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public int insertOrderFile(Model.Files model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
                                                     //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_Files(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("f_id"))
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
            strSql.Append("delete from  MS_Files ");
            strSql.Append(" where f_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.VarChar,11)};
            parameters[0].Value = id;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.Files GetFileModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.Files model = new Model.Files();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_Files");
            strSql.Append(" where f_id=@id");
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
        public Model.Files DataRowToFileModel(DataRow row)
        {
            Model.Files model = new Model.Files();
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

        #region 订单人员相关
        /// <summary>
        /// 获取订单人员
        /// </summary>
        public DataSet GetPersonList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM MS_Order left join MS_OrderPerson  on o_id=op_oid");
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
        /// 更新接单状态
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="username"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool updateOrderDstatus(string oid, string username, byte? status)
        {
            string sql = "update MS_OrderPerson set op_dstatus=@status where op_oid=@oid and (op_type=3 or op_type=5) and op_number=@number";
            SqlParameter[] parameters = {
                    new SqlParameter("@status", SqlDbType.TinyInt,4),
                    new SqlParameter("@oid", SqlDbType.VarChar,20),
                    new SqlParameter("@number", SqlDbType.VarChar,20)
            };
            parameters[0].Value = status;
            parameters[1].Value = oid;
            parameters[2].Value = username;
            return DbHelperSQL.ExecuteSql(sql, parameters) > 0;
        }

        /// <summary>
        /// 检查订单中是否存在对应的应收或应付客户
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool checkOrderCusID(string orderID, bool type, int cusID)
        {
            string sql = "select count(*) from MS_finance where fin_oid=@oid and fin_cid=@cid and fin_type=@type";
            SqlParameter[] parameters = {
                    new SqlParameter("@oid", SqlDbType.VarChar,20),
                    new SqlParameter("@cid", SqlDbType.Int,4),
                    new SqlParameter("@type", SqlDbType.Bit,20)
            };
            parameters[0].Value = orderID;
            parameters[1].Value = cusID;
            parameters[2].Value = type;

            return DbHelperSQL.Exists(sql, parameters);
        }


        #endregion
    }
}
