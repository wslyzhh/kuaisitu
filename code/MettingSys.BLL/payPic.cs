using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class payPic
    {
        private readonly DAL.payPic dal;
        public payPic()
        {
            dal = new DAL.payPic();
        }
        /// <summary>
        /// 添加活动文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public int insertPayFile(Model.payPic model, Model.manager manager)
        {
            int ret = dal.insertPayFile(model);
            if (ret > 0)
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = model.pp_rid.Value;
                logmodel.ol_title = model.pp_type==1?"添加付款明细附件":"添加费业务付款申请附件";
                logmodel.ol_content = "文件名：" + model.pp_fileName + "，文件路径：" + model.pp_filePath;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name);
            }
            return ret;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.payPic GetModel(int id)
        {
            return dal.GetModel(id);
        }
        /// <summary>
        /// 删除活动文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool deletePayFile(Model.payPic file, Model.manager manager)
        {
            if (dal.deleteOrderFile(file.pp_id.Value))
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + file.pp_filePath;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = file.pp_id.Value;
                logmodel.ol_title = file.pp_type == 1 ? "删除付款明细附件" : "删除费业务付款申请附件";
                logmodel.ol_content = "文件名：" + file.pp_fileName + "，文件路径：" + file.pp_filePath;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除记录下的全部图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool deleteFileByid(int id, byte? type)
        {
            return dal.deleteFileByid(id, type);
        }
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(byte? type, string strWhere, string filedOrder)
        {
            return dal.GetList(type, strWhere, filedOrder);
        }
    }
}
