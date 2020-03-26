using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace MettingSys.BLL
{
    public partial class selfMessage
    {
        private readonly DAL.selfMessage dal;

        public selfMessage()
        {
            dal = new DAL.selfMessage();
        }

        #region 基本方法================================
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.selfMessage model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 消息通知
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="replaceContent"></param>
        /// <param name="replaceUser"></param>
        /// <param name="replaceUser1"></param>
        /// <param name="receiver"></param>
        /// <param name="receiverName"></param>
        public void AddMessage(string nodeName,string receiver,string receiverName, string replaceContent, string replaceUser, string replaceUser1="")
        {
            string title = string.Empty, content = string.Empty;
            string fileurl = new BLL.sysconfig().loadConfig().messageTemplateUrl;
            Common.XmlHelper.getNodeContent(fileurl, "/lists/list", nodeName, out title, out content);
            if (string.IsNullOrEmpty(content))
            {
                content = fileurl;
            }
            Model.selfMessage message = new Model.selfMessage();
            message.me_title = title;
            content = content.Replace("@content", replaceContent).Replace("@user1", replaceUser1).Replace("@user", replaceUser);
            message.me_content = content;
            message.me_isRead = false;
            message.me_owner = receiver;
            message.me_ownerName = receiverName;
            message.me_addDate = DateTime.Now;
            new BLL.selfMessage().Add(message);

        }

        public void sentDingMessage(string nodeName, string userIdList, string replaceContent, string replaceUser, string replaceUser1 = "")
        {
            string title = string.Empty, content = string.Empty;
            string fileurl = new BLL.sysconfig().loadConfig().messageTemplateUrl;
            Common.XmlHelper.getNodeContent(fileurl, "/lists/list", nodeName, out title, out content);
            if (string.IsNullOrEmpty(content))
            {
                content = fileurl;
            }
            content = content.Replace("@content", replaceContent).Replace("@user1", replaceUser1).Replace("@user", replaceUser);
            dingtalk_helper.sentMessageToUser(userIdList, title + "：" + content);

        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.selfMessage model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        public bool Delete(string idstr)
        {
            return dal.Delete(idstr);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.selfMessage GetModel(int id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetList(Top, strWhere, filedOrder);
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 批量更改已读状态
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int updateRecordReadStatus(string ids)
        {
            return dal.updateRecordReadStatus(ids);
        }
        /// <summary>
        /// 根据工号和姓名获取未读消息数量
        /// </summary>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public DataSet getUnReadMessage(string username, string realname)
        {
            //List<Model.selfMessage> list = new List<Model.selfMessage>();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(realname))
            {
                return null;
            }
            return dal.getUnReadMessage(username, realname);
        }
        #endregion
    }
}
