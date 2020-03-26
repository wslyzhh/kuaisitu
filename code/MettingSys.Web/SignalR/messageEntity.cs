using MettingSys.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MettingSys.Web.SignalR
{
    public class messageEntity
    {
        public IEnumerable<selfMessage> GetData(string username,string realname)
        {
            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(@"select me_id,me_title,me_content,me_isRead from dbo.MS_message where me_isRead=0 and me_owner='"+ username + "'", sqlConnection))
                {
                    sqlCommand.Notification = null;
                    SqlDependency dependency = new SqlDependency(sqlCommand);
                    dependency.OnChange += new OnChangeEventHandler(dependency_Onchange);
                    if (sqlConnection.State == System.Data.ConnectionState.Closed)
                        sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        return reader.Cast<IDataRecord>().Select(a => new selfMessage()
                        {
                            me_id = Convert.ToInt32(a["me_id"]),
                            me_title = a["me_title"].ToString(),
                            me_content = a["me_content"].ToString(),
                            me_isRead = Convert.ToBoolean(a["me_isRead"])
                        }).ToList();
                    }

                }
            }
        }
        public void dependency_Onchange(object sender, SqlNotificationEventArgs e)
        {
            MessageHub.Show();
        }
    }
}