using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class publicSetting
    {
        private readonly DAL.publicSetting dal;

        public publicSetting()
        {
            dal = new DAL.publicSetting();
        }

        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            return dal.Exists(id);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public string Add(Model.publicSetting model, Model.manager manager)
        {
            if (model.ps_sdate == null)
            {
                return "请填写开始执行日期起始日期";
            }
            //if (model.ps_edate == null)
            //{
            //    return "请填写开始执行日期截止日期";
            //}
            if (model.ps_ratio<=0 || model.ps_ratio >=100)
            {
                return "业绩比例需大于0，小于100";
            }
            int id = dal.Add(model);
            if (id > 0)
            {
                Model.business_log log = new Model.business_log();
                log.ol_cid = id;
                log.ol_title = "创建执行人员业绩比例";
                log.ol_content = "是否启用：" + model.ps_isuse + "<br/>开始执行日期：" + model.ps_sdate.Value.ToString("yyyy-MM-dd") + "/" + model.ps_edate.Value.ToString("yyyy-MM-dd") + "<br/>业绩比例：" + model.ps_ratio + "%";
                log.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "添加失败";
        }

        public string Update(Model.publicSetting model, Model.manager manager,string content)
        {
            if (model.ps_sdate == null)
            {
                return "请填写开始执行日期起始日期";
            }
            //if (model.ps_edate == null)
            //{
            //    return "请填写开始执行日期截止日期";
            //}
            if (model.ps_ratio <= 0 || model.ps_ratio >= 100)
            {
                return "业绩比例需大于0，小于100";
            }
            if (dal.Update(model))
            {
                Model.business_log log = new Model.business_log();
                log.ol_cid = model.ps_id.Value;
                log.ol_title = "更新执行人员业绩比例";
                log.ol_content = content;
                log.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "更新失败";
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.publicSetting GetModel(byte? type)
        {
            return dal.GetModel(type);
        }

        #endregion

        #region 扩展方法================================
        
        #endregion
    }
}
