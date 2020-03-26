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
    public partial class certificates
    {
        public certificates() { }
        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  MS_certificates");
            strSql.Append(" where ");
            strSql.Append(" ce_id = @id  ");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.certificates model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_certificates(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("ce_id"))
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
        public bool Update(Model.certificates model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_certificates set ");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("ce_id"))
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
            strSql.Append(" where ce_id=@id ");
            paras.Add(new SqlParameter("@id", model.ce_id));
            return DbHelperSQL.ExecuteSql(strSql.ToString(), paras.ToArray()) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_certificates ");
            strSql.Append(" where ce_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }
        /// <summary>
        /// 审批凭证
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public bool checkCertificate(int id, byte? status, string remark, string username, string realname)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update MS_certificates  set ce_flag=@flag,ce_checkNum=@num,ce_checkName=@name,ce_checkRemark=@remark");
            strSql.Append(" where ce_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4),
                    new SqlParameter("@flag", SqlDbType.TinyInt,4),
                    new SqlParameter("@num", SqlDbType.VarChar,20),
                    new SqlParameter("@name", SqlDbType.VarChar,20),
                    new SqlParameter("@remark", SqlDbType.VarChar,200)
            };
            parameters[0].Value = id;
            parameters[1].Value = status;
            parameters[2].Value = username;
            parameters[3].Value = realname;
            parameters[4].Value = remark;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.certificates GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.certificates model = new Model.certificates();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_certificates");
            strSql.Append(" where ce_id=@id");
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
        /// 得到一个对象实体
        /// </summary>
        public Model.certificates GetModel(string num,DateTime? date)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.certificates model = new Model.certificates();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_certificates");
            strSql.Append(" where ce_num=@num and ce_date=@date");
            SqlParameter[] parameters = {
                    new SqlParameter("@num", SqlDbType.VarChar,20),
                    new SqlParameter("@date", SqlDbType.DateTime,20)};
            parameters[0].Value = num;
            parameters[1].Value = date;
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
            strSql.Append(" FROM  MS_certificates ");
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
        /// 获取凭证的具体列
        /// </summary>
        /// <param name="Top"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <returns></returns>
        public DataSet GetNumList(string fields, string strWhere, string filedOrder)
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
            strSql.Append(" FROM  MS_certificates ");
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder,string group, out int recordCount,out decimal rMoney,out decimal pMoney,bool isPage=true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ce.ce_id,ce.ce_num,ce.ce_date,ce.ce_remark,ce.ce_flag,ce.ce_checkNum,ce.ce_checkName,ce.ce_checkRemark,ce.ce_addDate,");
            strSql.Append("sum(case when rp.rp_type=1 then rp.rp_money else 0 end) receipt,sum(case when rp.rp_type=0 then rp.rp_money else 0 end) pay ");
            strSql.Append("from MS_certificates ce left join MS_ReceiptPay rp on ce_id=rp_ceid ");            
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append("group by ce.ce_id,ce.ce_num,ce.ce_date,ce.ce_remark,ce.ce_flag,ce.ce_checkNum,ce.ce_checkName,ce.ce_checkRemark,ce.ce_addDate");

            string sql = "";
            if (string.IsNullOrEmpty(group))
            {
                sql = strSql.ToString();
            }
            else
            {
                switch (group)
                {
                    case "date":
                        sql = "select CONVERT(varchar(100), ce_date, 23) ce_date,sum(receipt) receipt,sum(pay) pay from(" + strSql.ToString() + ") t group by ce_date";
                        filedOrder = "ce_date desc";
                        break;
                    case "month":
                        sql = "select concat(datepart(year,ce_date),'-',datepart(month,ce_date)) ce_date,sum(receipt) receipt,sum(pay) pay from(" + strSql.ToString() + ") t group by datepart(year,ce_date),datepart(month,ce_date)";
                        filedOrder = "datepart(year,ce_date) desc,datepart(month,ce_date) desc";
                        break;
                    case "year":
                        sql = "select datepart(year,ce_date) ce_date,sum(receipt) receipt,sum(pay) pay from(" + strSql.ToString() + ") t group by datepart(year,ce_date)";
                        filedOrder = "datepart(year,ce_date) desc";
                        break;
                }
            }
            recordCount = 0;rMoney = 0;pMoney = 0;
            if (isPage)
            {
                //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(sql)));
                DataTable dt = DbHelperSQL.Query("select count(*) c,sum(receipt) receipt,sum(pay) pay from (" + sql + ") t").Tables[0];
                if (dt != null)
                {
                    recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                    rMoney = Utils.ObjToDecimal(dt.Rows[0]["receipt"], 0);
                    pMoney = Utils.ObjToDecimal(dt.Rows[0]["pay"], 0);
                }
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, sql, filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(sql + " order by "+filedOrder);
            }
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string num,DateTime? date, int id = 0)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from MS_certificates");
            strSql.Append(" where ce_num=@num and datediff(day,ce_date,@date)=0");
            if (id > 0)
            {
                strSql.Append(" and ce_id <>@id");
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@num", SqlDbType.NVarChar,100),
                    new SqlParameter("@date",SqlDbType.DateTime,20),
                    new SqlParameter("@id", SqlDbType.Int,4)
            };
            parameters[0].Value = num;
            parameters[1].Value = date;
            parameters[2].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.certificates DataRowToModel(DataRow row)
        {
            Model.certificates model = new Model.certificates();
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
