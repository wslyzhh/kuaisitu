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
    public partial class ReceiptPay
    {
        public ReceiptPay() { }
        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  MS_ReceiptPay");
            strSql.Append(" where ");
            strSql.Append(" rp_id = @id  ");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.ReceiptPay model,SqlConnection conn=null,SqlTransaction tran=null)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_ReceiptPay(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("rp_id"))
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
                obj = DbHelperSQL.GetSingle(conn,tran,strSql.ToString(), paras.ToArray());
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
        public bool Update(Model.ReceiptPay model,bool isChangeMethod=false)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_ReceiptPay set ");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("rp_id"))
                {
                    //判断属性值是否为空
                    //if (pi.GetValue(model, null) != null)
                    //{
                        str1.Append(pi.Name + "=@" + pi.Name + ",");//声明参数
                        paras.Add(new SqlParameter("@" + pi.Name, pi.GetValue(model, null)));//对参数赋值
                    //}
                }
            }
            strSql.Append(str1.ToString().Trim(','));
            strSql.Append(" where rp_id=@id ");
            paras.Add(new SqlParameter("@id", model.rp_id));
            if (DbHelperSQL.ExecuteSql(strSql.ToString(), paras.ToArray()) > 0)
            {
                if (isChangeMethod)
                {
                    DbHelperSQL.ExecuteSql("update MS_ReceiptPayDetail set rpd_method=" + model.rp_method + " where rpd_rpid=" + model.rp_id + "");
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 计算已审未支付退款的数量
        /// </summary>
        /// <returns></returns>
        public int getUnPaycount()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(*) from MS_ReceiptPay where rp_type=1 and rp_flag=2 and rp_flag1=2 and rp_isConfirm='False' and rp_money < 0");

            SqlParameter[] parameters = { };
            return Utils.ObjToInt(DbHelperSQL.GetSingle(strSql.ToString(), parameters));
        }

        public DataTable getOrderUser(int rpid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select op_number,op_name from MS_ReceiptPayDetail left join MS_Order on rpd_oid=o_id left join MS_OrderPerson on o_id=op_oid and op_type=1");
            strSql.Append(" where rpd_rpid=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = rpid;

            return DbHelperSQL.Query(strSql.ToString(), parameters).Tables[0];
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_ReceiptPay ");
            strSql.Append(" where rp_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }
        /// <summary>
        /// 批量删除收款
        /// </summary>
        /// <param name="idStr"></param>
        /// <returns></returns>
        public bool deleteReceipt(int id)
        {
            bool result = false;
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//打开数据连接
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        #region 删除收款通知======================
                        string sql = "delete from MS_ReceiptPay where rp_id=@rpid";
                        List<SqlParameter> paras0 = new List<SqlParameter>();
                        paras0.Add(new SqlParameter("@rpid", id));
                        DbHelperSQL.ExecuteSql(conn, trans, sql, paras0.ToArray());
                        #endregion

                        #region 删除收款明细==========================
                        sql = "delete from MS_ReceiptPayDetail where rpd_rpid=@rpid";
                        DbHelperSQL.ExecuteSql(conn, trans, sql, paras0.ToArray());

                        trans.Commit();
                        result = true;
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
        /// 删除付款
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool deletePay(Model.ReceiptPay model)
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
                        List<SqlParameter> paras0 = new List<SqlParameter>();
                        paras0.Add(new SqlParameter("@rpid", model.rp_id));
                        if (!model.rp_isExpect.Value)
                        {
                            #region 更新付款明细==========================
                            sql = "update MS_ReceiptPayDetail set rpd_rpid=0 where rpd_rpid=@rpid";
                            DbHelperSQL.ExecuteSql(conn, trans, sql, paras0.ToArray());
                            #endregion
                        }
                        else
                        {
                            #region 更新付款明细==========================
                            sql = "delete MS_ReceiptPayDetail where rpd_rpid=@rpid";
                            DbHelperSQL.ExecuteSql(conn, trans, sql, paras0.ToArray());
                            #endregion
                        }

                        #region 删除付款通知======================
                        sql = "delete from MS_ReceiptPay where rp_id=@rpid";
                        DbHelperSQL.ExecuteSql(conn, trans, sql, paras0.ToArray());
                        #endregion

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
        /// 付款审批
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
            string updateField = "rp_flag", updateField1 = "rp_checkNum", updateField2 = "rp_checkName", updateField3 = "rp_checkRemark";
            if (type == 2)
            {
                updateField = "rp_flag1";
                updateField1 = "rp_checkNum1";
                updateField2 = "rp_checkName1";
                updateField3 = "rp_checkRemark1";
            }
            string sql = "update MS_ReceiptPay set " + updateField + "=@status," + updateField1 + "=@num," + updateField2 + "=@name," + updateField3 + "=@remark where rp_id=@rpid";
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
        /// 得到一个对象实体
        /// </summary>
        public Model.ReceiptPay GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.ReceiptPay model = new Model.ReceiptPay();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_ReceiptPay");
            strSql.Append(" where rp_id=@id");
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
        /// 获取已分配总额
        /// </summary>
        /// <param name="rpid"></param>
        /// <returns></returns>
        public decimal getDistributeMoney(int rpid)
        {
            string sql = "select isnull(sum(rpd_money),0) from MS_ReceiptPayDetail where rpd_rpid=@id";
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = rpid;
            return Utils.ObjToDecimal(DbHelperSQL.GetSingle(sql, parameters),0);
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
            strSql.Append(" FROM  MS_ReceiptPay r left join MS_ReceiptPayDetail on rp_id = rpd_rpid left join MS_customer c on r.rp_cid=c.c_id left join MS_payMethod p on rp_method=pm_id left join MS_certificates on rp_ceid=ce_id left join MS_customerBank on rp_cbid=cb_id");
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount,out decimal pmoney,out decimal punmoney,bool isPage=true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from (select *,(rp_money-(select isnull(sum(rpd_money),0) from MS_ReceiptPayDetail rpd where rpd.rpd_rpid=r.rp_id)) as undistribute FROM MS_ReceiptPay r left join MS_customer c on r.rp_cid=c.c_id left join MS_payMethod p on rp_method=pm_id left join MS_certificates on rp_ceid=ce_id left join MS_customerBank on rp_cbid=cb_id) t");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = 0;pmoney = 0;punmoney = 0;
            if (isPage)
            {
                //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                DataTable dt = DbHelperSQL.Query("select count(*) c,sum(rp_money) tRpMoney,sum(undistribute) tUnMoney from (" + strSql.ToString() + ") t").Tables[0];
                if (dt!=null)
                {
                    recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                    pmoney = Utils.ObjToDecimal(dt.Rows[0]["tRpMoney"], 0);
                    punmoney = Utils.ObjToDecimal(dt.Rows[0]["tUnMoney"], 0);
                }
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString() + " order by  " + filedOrder);
            }
        }

        /// <summary>
        /// 分配列表
        /// </summary>
        public DataSet GetDistributionList(Dictionary<string,string> dict,int pageSize, int pageIndex,out int recordCount)
        {
            recordCount = 0;
            StringBuilder strSql = new StringBuilder();
            StringBuilder strSql1 = new StringBuilder();
            StringBuilder strSql2 = new StringBuilder();
            StringBuilder strSql3 = new StringBuilder();
            string addChkFiled1 = "", addChkFiled2 = "", addChkGroup = "";
            string selectFiled = "isnull(t1.fin_oid,t3.rpd_oid) fin_oid,o_content,isnull(finMoney,0) finMoney,isnull(rpdMoney,0) rpdMoney,(isnull(finMoney,0)-isnull(rpdMoney,0)) unMoney,isnull(totalDistribute,0) totalDistribute,isnull(currentDistribute,0) currentDistribute";
            string addTable = "";
            if (dict != null)
            {
                strSql2.Append(" and fin_cid=" + dict["cid"] + "");
                strSql2.Append(" and fin_type='" + dict["type"] + "'");
                strSql3.Append(" and rpd_cid=" + dict["cid"] + "");
                strSql3.Append(" and rpd_type='" + dict["type"] + "'");
                if (dict.ContainsKey("oID"))
                {
                    strSql1.Append(" and isnull(t1.fin_oid,t3.rpd_oid) in ('" + dict["oID"].Replace("，",",").Replace(",","','") + "')");
                }
                if (dict.ContainsKey("sdate"))
                {
                    strSql1.Append(" and datediff(day,o_sdate,'" + dict["sdate"] + "')<=0");
                }
                if (dict.ContainsKey("edate"))
                {
                    strSql1.Append(" and datediff(day,o_sdate,'" + dict["edate"] + "')>=0");
                }
                if (dict.ContainsKey("sdate1"))
                {
                    strSql1.Append(" and datediff(day,o_edate,'" + dict["sdate1"] + "')<=0");
                }
                if (dict.ContainsKey("edate1"))
                {
                    strSql1.Append(" and datediff(day,o_edate,'" + dict["edate1"] + "')>=0");
                }
                if (dict.ContainsKey("money"))
                {
                    switch (dict["moneyType"])
                    {
                        case "0":
                            strSql1.Append(" and isnull(finMoney,0) "+dict["sign"]+ " " + dict["money"] + "");
                            break;
                        case "1":
                            strSql1.Append(" and isnull(rpdMoney,0) " + dict["sign"] + " " + dict["money"] + "");
                            break;
                        case "2":
                            strSql1.Append(" and isnull(finMoney,0)-isnull(rpdMoney,0) " + dict["sign"] + " " + dict["money"] + "");
                            break;
                        default:
                            break;
                    }
                }
                if (dict.ContainsKey("person"))
                {
                    strSql1.Append(" and op_number='"+ dict["person"] + "'");
                }
                if (dict.ContainsKey("chk"))
                {
                    selectFiled = "isnull(t1.fin_oid,t3.rpd_oid) fin_oid,o_content,isnull(finMoney,0) finMoney,isnull(rpdMoney,0) rpdMoney,(isnull(finMoney,0)-isnull(rpdMoney,0)) unMoney,isnull(chkMoney,0) chkMoney,isnull(chktotalDistribute,0) chktotalDistribute,isnull(chkcurrentDistribute,0) chkcurrentDistribute,isnull(totalDistribute,0) totalDistribute,isnull(currentDistribute,0) currentDistribute,isnull(fc_num,'') fc_num,isnull(fcMoney,0) fcMoney";
                    addTable = "left join (select fin_oid,fc_num,sum(isnull(fc_money,0)) fcMoney from MS_finance left join MS_finance_chk on fin_id = fc_finid where fin_flag<>1 "+ strSql2 + " group by fin_oid,fc_num) t2 on t1.fin_oid=t2.fin_oid ";
                    strSql1.Append(" and fc_num='" + dict["chk"] + "'");
                    addChkFiled1 = ",sum(chkMoney) chkMoney,sum(chktotalDistribute) chktotalDistribute,sum(chkcurrentDistribute)  chkcurrentDistribute";
                    addChkFiled2 = ",(case when rpd_num='"+ dict["chk"] + "' and rp_isConfirm=1 then sum(isnull(rpd_money,0)) else 0 end) chkMoney, (case when rpd_num='" + dict["chk"] + "' then sum(isnull(rpd_money,0)) else 0 end) chktotalDistribute ,(case when rp_id = " + dict["rpID"] + " and rpd_num='" + dict["chk"] + "' then sum(isnull(rpd_money, 0)) else 0 end) chkcurrentDistribute";
                    addChkGroup = ",rpd_num";
                }
                if (dict["tag"] == "1")
                {
                    strSql1.Append(" and isnull(currentDistribute,0) <> 0");
                }
            }
            strSql.Append("select " + selectFiled + " from ");
            strSql.Append(" (select fin_oid,sum(isnull(fin_money, 0)) finMoney from MS_finance where fin_flag <> 1 " + strSql2 + " group by fin_oid) t1");
            strSql.Append(" "+ addTable + "");
            strSql.Append("  full join (select rpd_oid,sum(rpdMoney) rpdMoney "+ addChkFiled1 + ", sum(totalDistribute) totalDistribute, sum(currentDistribute) currentDistribute from(");
            strSql.Append(" select rpd_oid,(case when rp_isConfirm = 1 then sum(isnull(rpd_money, 0)) else 0 end) rpdMoney " + addChkFiled2 + ", sum(isnull(rpd_money, 0)) totalDistribute, (case when rp_id = " + dict["rpID"] +" then sum(isnull(rpd_money, 0)) else 0 end) currentDistribute");
            strSql.Append(" from MS_ReceiptPayDetail left join MS_ReceiptPay on rp_id = rpd_rpid");
            strSql.Append(" where 1=1 " + strSql3 + "");
            strSql.Append(" group by rpd_oid "+ addChkGroup + ",rp_isConfirm, rp_id) as r group by rpd_oid) t3 on t1.fin_oid = t3.rpd_oid ");
            strSql.Append(" left join MS_Order on isnull(t1.fin_oid,t3.rpd_oid) = o_id left join MS_OrderPerson on o_id=op_oid and op_type=1 where 1=1 " + strSql1 + "");
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), "isnull(t1.fin_oid,t3.rpd_oid) asc"));
        }

        #endregion

        #region 扩展方法================================        
        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.ReceiptPay DataRowToModel(DataRow row)
        {
            Model.ReceiptPay model = new Model.ReceiptPay();
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
