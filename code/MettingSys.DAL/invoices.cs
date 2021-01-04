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
    public partial class invoices
    {
        public invoices() { }
        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  MS_invoices");
            strSql.Append(" where ");
            strSql.Append(" inv_id = @id  ");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.invoices model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_invoices(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("inv_id"))
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
        public bool Update(Model.invoices model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_invoices set ");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("inv_id"))
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
            strSql.Append(" where inv_id=@id ");
            paras.Add(new SqlParameter("@id", model.inv_id));
            return DbHelperSQL.ExecuteSql(strSql.ToString(), paras.ToArray()) > 0;
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
            string sql = "select * from MS_invoices where inv_id =" + id + "";
            if (type == 1)
            {
                if (tag)
                {
                    sql += " and inv_flag1=" + status;
                }
                else
                {
                    sql += " and inv_flag1<>" + status;
                }
            }
            else if (type == 2)
            {
                if (tag)
                {
                    sql += " and inv_flag2=" + status;
                }
                else
                {
                    sql += " and inv_flag2<>" + status;
                }
            }
            else
            {
                if (tag)
                {
                    sql += " and inv_flag3=" + status;
                }
                else
                {
                    sql += " and inv_flag3<>" + status;
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
        /// 发票审批
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public int checkInvoiceStatus(int id, byte? type, byte? status, string remark, string username, string realname)
        {
            string updateField = "inv_flag1", updateField1 = "inv_checkNum1", updateField2 = "inv_checkName1", updateField3 = "inv_checkRemark1";
            if (type == 2)
            {
                updateField = "inv_flag2";
                updateField1 = "inv_checkNum2";
                updateField2 = "inv_checkName2";
                updateField3 = "inv_checkRemark2";
            }
            else if (type == 3)
            {
                updateField = "inv_flag3";
                updateField1 = "inv_checkNum3";
                updateField2 = "inv_checkName3";
                updateField3 = "inv_checkRemark3";
            }
            string sql = "update MS_invoices set " + updateField + "=@status," + updateField1 + "=@num," + updateField2 + "=@name," + updateField3 + "=@remark where inv_id=@rpid";
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
        /// 开票
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public bool confirmInvoice(int id, bool? status,DateTime? date, string username, string realname)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update MS_invoices set inv_isConfirm=@status,inv_date=@date,inv_confirmerNum=@num,inv_confirmerName=@name ");
            strSql.Append(" where inv_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4),
                    new SqlParameter("@status", SqlDbType.Bit,4),
                    new SqlParameter("@num", SqlDbType.VarChar,20),
                    new SqlParameter("@name", SqlDbType.VarChar,20),
                    new SqlParameter("@date", SqlDbType.DateTime,20)
            };
            parameters[0].Value = id;
            parameters[1].Value = status;
            if (status.Value)
            {
                parameters[2].Value = username;
                parameters[3].Value = realname;
                parameters[4].Value = date;
            }
            else
            {
                parameters[2].Value = "";
                parameters[3].Value = "";
                parameters[4].Value = null;
            }

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }

        /// <summary>
        /// 计算某个客户到现在为止在某个订单中剩余的开票金额,cid为0时计算整个订单的剩余开票金额
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public decimal? computeInvoiceLeftMoney(string oid, int cid=0)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select finMoney-isnull(invMoney,0) from (select fin_cid,sum(isnull(fin_money,0)) finMoney from MS_finance where fin_type=1 and fin_cid=@cid and fin_flag<>1 and fin_oid=@oid and fin_adddate<getdate() group by fin_cid) t1");
            strSql.Append(" left join (select inv_cid,sum(isnull(inv_money,0)) invMoney from MS_invoices where inv_cid=@cid and inv_flag1<>1 and inv_flag2<>1 and inv_flag3<>1 and inv_oid=@oid group by inv_cid) t2 on fin_cid=inv_cid");
            if (cid == 0)
            {
                strSql.Clear();
                strSql.Append("select finMoney-isnull(invMoney,0) from (select fin_oid,sum(isnull(fin_money,0)) finMoney from MS_finance where fin_type=1 and fin_flag<>1 and fin_oid=@oid group by fin_oid) t1");
                strSql.Append(" left join (select inv_oid,sum(isnull(inv_money,0)) invMoney from MS_invoices where inv_flag1<>1 and inv_flag2<>1 and inv_flag3<>1 and inv_oid=@oid group by inv_oid) t2 on fin_oid=inv_oid");
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@cid", SqlDbType.Int,4),
                    new SqlParameter("@oid", SqlDbType.VarChar,20)
            };
            parameters[0].Value = cid;
            parameters[1].Value = oid;

            return Utils.ObjToDecimal(DbHelperSQL.GetSingle(strSql.ToString(), parameters), 0);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_invoices ");
            strSql.Append(" where inv_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }
        

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.invoices GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.invoices model = new Model.invoices();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_invoices");
            strSql.Append(" where inv_id=@id");
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
            strSql.Append(" FROM  MS_invoices left join MS_customer on inv_cid=c_id left join MS_department on inv_darea = de_area and de_type=1 left join MS_invoiceUnit on inv_unit=invU_id");
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, out decimal pmoney, bool isPage=true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM MS_invoices left join MS_customer on inv_cid=c_id left join MS_department on inv_darea = de_area and de_type=1 left join ms_order on inv_oid=o_id left join MS_invoiceUnit on inv_unit=invU_id");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = 0; pmoney=0;
            if (isPage)
            {
                //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                DataTable dt = DbHelperSQL.Query("select count(*) c,sum(inv_money) pMoney from (" + strSql.ToString() + ") t").Tables[0];
                if (dt != null)
                {
                    recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                    pmoney = Utils.ObjToDecimal(dt.Rows[0]["pMoney"], 0);
                }
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
        /// 将对象转换实体
        /// </summary>
        public Model.invoices DataRowToModel(DataRow row)
        {
            Model.invoices model = new Model.invoices();
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
