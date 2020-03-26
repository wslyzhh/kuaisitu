using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using MettingSys.Common;

namespace MettingSys.BLL
{
    /// <summary>
    ///管理员信息表
    /// </summary>
    public partial class manager
    {
        private readonly Model.sysconfig sysConfig = new BLL.sysconfig().loadConfig(); //获得系统配置信息
        private readonly DAL.manager dal;

        public manager()
        {
            dal = new DAL.manager(sysConfig.sysdatabaseprefix);
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
        public string Add(Model.manager model,Model.manager manager)
        {
            if (model.role_id == 0)
            {
                return "请选择用户角色";
            }
            if (string.IsNullOrEmpty(model.user_name))
            {
                return "用户名不能为空";
            }
            if (Exists(model.user_name))
            {
                return "用户名已经存在";
            }
            if (string.IsNullOrEmpty(model.password))
            {
                return "密码不能为空";
            }
            if (string.IsNullOrEmpty(model.real_name))
            {
                return "姓名不能为空";
            }
            if (string.IsNullOrEmpty(model.telephone))
            {
                return "电话不能为空";
            }
            if (model.departID==0)
            {
                return "岗位不能为空";
            }
            Model.department de = new BLL.department().GetModel(model.departID);
            if (de==null)
            {
                return "岗位不存在";
            }
            if (de.de_type!=3)
            {
                return "请选择到岗位级别";
            }
            model.area = de.de_area;
            model.user_name = model.user_name.ToUpper();
            model.area = model.area.ToUpper();
            int id = 0;
            string result = dal.Add(model, out id);
            if (string.IsNullOrEmpty(result))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("用户名：" + model.user_name + "<br/>");
                sb.Append("管理角色：" + model.role_id + "<br/>");
                sb.Append("姓名：" + model.real_name + "<br/>");
                sb.Append("邮箱：" + model.email + "<br/>");
                sb.Append("电话：" + model.telephone + "<br/>");
                sb.Append("岗位：" + model.departTree + "<br/>");
                sb.Append("区域：" + model.area + "<br/>");
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_title = "添加用户";
                logmodel.ol_content = sb.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return result;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public string Update(Model.manager model,string content, Model.manager manager,bool flag=true, bool updateName = false, bool updateContact = false)
        {
            if (model.role_id == 0)
            {
                return "请选择用户角色";
            }
            if (string.IsNullOrEmpty(model.user_name))
            {
                return "用户名不能为空";
            }
            if (Exists(model.user_name,model.id))
            {
                return "用户名已经存在";
            }
            if (string.IsNullOrEmpty(model.password))
            {
                return "密码不能为空";
            }
            if (string.IsNullOrEmpty(model.real_name))
            {
                return "姓名不能为空";
            }
            if (string.IsNullOrEmpty(model.telephone))
            {
                return "电话不能为空";
            }
            if (flag)
            {
                if (model.departID == 0)
                {
                    return "岗位不能为空";
                }
                Model.department de = new BLL.department().GetModel(model.departID);
                if (de == null)
                {
                    return "岗位不存在";
                }
                if (de.de_type != 3)
                {
                    return "请选择到岗位级别";
                }
            }
            model.user_name = model.user_name.ToUpper();
            model.area = model.area.ToUpper();
            string result = dal.Update(model, updateName, updateContact);
            if (string.IsNullOrEmpty(result))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = model.id;
                logmodel.ol_title = "编辑用户";
                logmodel.ol_content = content;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return result;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public string Delete(int id,Model.manager manager)
        {
            Model.manager m = GetModel(id);
            if (m == null) return "用户不存在";
            if (dal.checkIsUse(m.user_name))
            {
                return "该用户已被使用，不能删除";
            }
            if (dal.Delete(id))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_title = "删除用户";
                logmodel.ol_content = "用户名：" + m.user_name + "，姓名：" + m.real_name + "";
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "删除失败";
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.manager GetModel(int id)
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
        /// 返回某些区域中含有某个权限的人员
        /// </summary>
        /// <param name="code">0501</param>
        /// <param name="area">SY,SH,YN</param>
        /// <returns></returns>
        public DataSet getUserByPermission(string code,string area="")
        {
            return dal.getUserByPermission(code, area);
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
        /// 查询用户名是否存在
        /// </summary>
        public bool Exists(string user_name,int id=0)
        {
            return dal.Exists(user_name,id);
        }

        /// <summary>
        /// 根据用户名取得Salt
        /// </summary>
        public string GetSalt(string user_name)
        {
            return dal.GetSalt(user_name);
        }

        /// <summary>
        /// 根据用户名密码返回一个实体
        /// </summary>
        public Model.manager GetModel(string user_name, string password)
        {
            return dal.GetModel(user_name, password);
        }
        /// <summary>
        /// 根据用户名密码返回一个实体
        /// </summary>
        public Model.manager GetModel(string user_name)
        {
            return dal.GetModel(user_name);
        }
        /// <summary>
        /// 根据用户名密码返回一个实体
        /// </summary>
        public Model.manager GetModel(string user_name, string password, bool is_encrypt)
        {
            //检查一下是否需要加密
            if (is_encrypt)
            {
                //先取得该用户的随机密钥
                string salt = dal.GetSalt(user_name);
                if (string.IsNullOrEmpty(salt))
                {
                    return null;
                }
                //把明文进行加密重新赋值
                password = DESEncrypt.Encrypt(password, salt);
            }
            return dal.GetModel(user_name, password);
        }
        #endregion
    }
}