using MettingSys.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.unBusiness
{
    public partial class unBusinessPay_edit : Web.UI.ManagePage
    {
        private string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        protected int id = 0;

        protected string fromOrder = "";//true时表示从订单页面添加
        protected string oID = string.Empty, _functionText = string.Empty;
        protected Model.business_log log = null;
        protected Model.manager manager = null;
        protected bool isShowCheckMoney = false;//是否显示批复金额
        protected void Page_Load(object sender, EventArgs e)
        {
            action = DTRequest.GetQueryString("action");
            fromOrder = DTRequest.GetQueryString("fromOrder");
            oID = DTRequest.GetQueryString("oID");
            _functionText = DTRequest.GetQueryString("functionText");

            ChkAdminLevel("sys_unBusiness_list", action); //检查权限    
            dlEditUpload.Visible = false;
            dlAddUpload.Visible = false;
            uploadDiv.Visible = false;
            if (action == DTEnums.ActionEnum.Add.ToString())
            {
                dlAddUpload.Visible = true;
            }
            //编辑
            if (action == DTEnums.ActionEnum.Edit.ToString())
            {
                uploadDiv.Visible = true;
                dlEditUpload.Visible = true;
                this.id = DTRequest.GetQueryInt("id");
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.unBusinessApply().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            //查看
            if (action == DTEnums.ActionEnum.View.ToString())
            {
                dlEditUpload.Visible = true;
                this.id = DTRequest.GetQueryInt("id");
                btnSubmit.Visible = false;
            }
            if (!Page.IsPostBack)
            {
                if (action == DTEnums.ActionEnum.Add.ToString())
                {
                    if (fromOrder == "true")
                    {
                        txtinstruction.Text = _functionText;
                        InitData(0);
                        ddlfunction.Visible = true;
                        txtfunction.Visible = false;
                    }
                    else
                    {
                        InitData(1);
                        ddlfunction.Visible = true;
                        txtfunction.Visible = false;
                    }
                }
                if (action == DTEnums.ActionEnum.Edit.ToString() || action == DTEnums.ActionEnum.View.ToString()) //修改或查看
                {
                    ShowInfo(this.id);
                }
            }
        }
        #region 初始化数据
        private void InitData(byte? key = null)
        {
            ddltype.DataSource = BusinessDict.unBusinessNature(key);
            ddltype.DataTextField = "value";
            ddltype.DataValueField = "key";
            ddltype.DataBind();

            //绑定支付用途
            ddlfunction.DataSource = BusinessDict.unBusinessPayFunction(key);
            ddlfunction.DataTextField = "value";
            ddlfunction.DataValueField = "key";
            ddlfunction.DataBind();
        }
        #endregion

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.unBusinessApply bll = new BLL.unBusinessApply();
            Model.unBusinessApply model = bll.GetModel(_id);

            manager = GetAdminInfo();
            //ddltype.SelectedValue = model.uba_type.ToString();
            ddltype.Visible = false;
            ddlfunction.Visible = false;
            txtfunction.Visible = false;
            btnAudit.Visible = false;
            labtype.Text = BusinessDict.unBusinessNature()[model.uba_type.Value];
            if (model.uba_type == 0)
            {
                ddlfunction.Visible = true;
                if (model.uba_function == "业务活动执行备用金借款")
                {
                    isShowCheckMoney = true;
                    InitData(0);
                }
                else
                {
                    InitData(1);
                }
                ddlfunction.SelectedValue = model.uba_function;
            }
            else
            {
                txtfunction.Visible = true;
                txtfunction.Text = model.uba_function;
            }
            txtinstruction.Text = model.uba_instruction;
            txtbank.Text = model.uba_receiveBank;
            txtbankname.Text = model.uba_receiveBankName;
            txtbanknum.Text = model.uba_receiveBankNum;
            txtmoney.Text = model.uba_money.ToString();
            txtforedate.Text = model.uba_foreDate.Value.ToString("yyyy-MM-dd");
            txtRemark.Text = model.uba_remark;

            rptAlbumList.DataSource = new BLL.payPic().GetList(2, "pp_type=2 and pp_rid=" + _id + "", "pp_addDate desc");
            rptAlbumList.DataBind();

            if ((manager.area == model.uba_area && new BLL.permission().checkHasPermission(manager, "0603")) || new BLL.permission().checkHasPermission(manager, "0402,0601"))
            {
                btnAudit.Visible = true;
                ddlflag.DataSource = Common.BusinessDict.checkStatus();
                ddlflag.DataTextField = "value";
                ddlflag.DataValueField = "key";
                ddlflag.DataBind();
                ddlflag.Items.Insert(0, new ListItem("请选择", ""));

                if (new BLL.permission().checkHasPermission(manager, "0603"))//部门审批
                {
                    ddlchecktype.SelectedValue = "1";
                    ddlflag.SelectedValue = model.uba_flag1.ToString();
                    txtCheckRemark.Text = model.uba_checkRemark1;
                }
                else if (new BLL.permission().checkHasPermission(manager, "0402"))//财务审批
                {
                    ddlchecktype.SelectedValue = "2";
                    ddlflag.SelectedValue = model.uba_flag2.ToString();
                    txtCheckRemark.Text = model.uba_checkRemark2;
                }
                else if (new BLL.permission().checkHasPermission(manager, "0601"))//总经理审批
                {
                    ddlchecktype.SelectedValue = "3";
                    ddlflag.SelectedValue = model.uba_flag3.ToString();
                    txtCheckRemark.Text = model.uba_checkRemark3;
                }
            }
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd(out int id)
        {
            id = 0;
            Model.unBusinessApply model = new Model.unBusinessApply();
            BLL.unBusinessApply bll = new BLL.unBusinessApply();
            manager = GetAdminInfo();

            model.uba_type = Utils.ObjToByte(ddltype.SelectedValue);
            if (string.IsNullOrWhiteSpace(fromOrder))
            {
                if (model.uba_type == 0)
                {
                    model.uba_function = ddlfunction.SelectedValue;
                }
                else
                {
                    model.uba_function = txtfunction.Text.Trim();
                }
            }
            else
            {
                model.uba_function = ddlfunction.SelectedValue;
            }
            if (model.uba_type == 0)
            {
                model.uba_oid = oID;
            }
            model.uba_instruction = txtinstruction.Text.Trim();
            model.uba_receiveBank = txtbank.Text.Trim();
            model.uba_receiveBankName = txtbankname.Text.Trim();
            model.uba_receiveBankNum = txtbanknum.Text.Trim();
            decimal _money = 0;
            if (!Decimal.TryParse(txtmoney.Text.Trim(), out _money))
            {
                return "金额格式有误";
            }
            model.uba_money = _money;
            model.uba_foreDate = ConvertHelper.toDate(txtforedate.Text.Trim());
            model.uba_remark = txtRemark.Text.Trim();
            model.uba_PersonNum = manager.user_name;
            model.uba_personName = manager.real_name;            
            model.uba_area = manager.area;
            if (fileUp.HasFile)
            {
                string fileext = "";
                for (int i = 0; i < fileUp.PostedFiles.Count; i++)
                {
                    fileext = System.IO.Path.GetExtension(fileUp.PostedFiles[i].FileName).TrimStart('.');//jpg,jpge,png,gif
                    //检查文件扩展名是否合法
                    if (!CheckFileExt(fileext))
                    {
                        return "不允许上传" + fileext + "类型的文件";
                    }
                    byte[] byteData = FileHelper.ConvertStreamToByteBuffer(fileUp.PostedFiles[i].InputStream); //获取文件流
                    //检查文件大小是否合法
                    if (!CheckFileSize(fileext, byteData.Length))
                    {
                        return "文件超过限制的大小";
                    }
                }                
            }
            return bll.Add(model, manager, out id);
        }
        #endregion
        /// <summary>
        /// 检查是否为合法的上传文件
        /// </summary>
        private bool CheckFileExt(string _fileExt)
        {
            //检查危险文件
            string[] excExt = { "asp", "aspx", "ashx", "asa", "asmx", "asax", "php", "jsp", "htm", "html" };
            for (int i = 0; i < excExt.Length; i++)
            {
                if (excExt[i].ToLower() == _fileExt.ToLower())
                {
                    return false;
                }
            }
            //检查合法文件
            string[] allowExt = (this.sysConfig.fileextension + "," + this.sysConfig.videoextension).Split(',');
            for (int i = 0; i < allowExt.Length; i++)
            {
                if (allowExt[i].ToLower() == _fileExt.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 检查文件大小是否合法
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        /// <param name="_fileSize">文件大小(B)</param>
        private bool CheckFileSize(string _fileExt, int _fileSize)
        {
            //将视频扩展名转换成ArrayList
            ArrayList lsVideoExt = new ArrayList(this.sysConfig.videoextension.ToLower().Split(','));
            //判断是否为图片文件
            if (IsImage(_fileExt))
            {
                if (this.sysConfig.imgsize > 0 && _fileSize > this.sysConfig.imgsize * 1024)
                {
                    return false;
                }
            }
            else if (lsVideoExt.Contains(_fileExt.ToLower()))
            {
                if (this.sysConfig.videosize > 0 && _fileSize > this.sysConfig.videosize * 1024)
                {
                    return false;
                }
            }
            else
            {
                if (this.sysConfig.attachsize > 0 && _fileSize > this.sysConfig.attachsize * 1024)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 是否为图片文件
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        private bool IsImage(string _fileExt)
        {
            ArrayList al = new ArrayList();
            al.Add("bmp");
            al.Add("jpeg");
            al.Add("jpg");
            al.Add("gif");
            al.Add("png");
            if (al.Contains(_fileExt.ToLower()))
            {
                return true;
            }
            return false;
        }
        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.unBusinessApply bll = new BLL.unBusinessApply();
            Model.unBusinessApply model = bll.GetModel(_id);
            string _content = string.Empty;
            manager = GetAdminInfo();
            //支付用途
            if (model.uba_type == 0)
            {
                if (model.uba_function != ddlfunction.SelectedValue)
                {
                    _content += "支付用途：" + model.uba_function + "→<font color='red'>" + ddlfunction.SelectedValue + "</font><br/>";
                }
                model.uba_function = ddlfunction.SelectedValue;
            }
            else
            {
                if (model.uba_function != txtfunction.Text.Trim())
                {
                    _content += "支付用途：" + model.uba_function + "→<font color='red'>" + txtfunction.Text.Trim() + "</font><br/>";
                }
                model.uba_function = txtfunction.Text.Trim();
            }
            //用途说明
            if (model.uba_instruction != txtinstruction.Text.Trim())
            {
                _content += "用途说明：" + model.uba_instruction + "→<font color='red'>" + txtinstruction.Text.Trim() + "</font><br/>";
            }
            model.uba_instruction = txtinstruction.Text.Trim();
            //收款银行
            if (model.uba_receiveBank != txtbank.Text.Trim())
            {
                _content += "收款银行：" + model.uba_receiveBank + "→<font color='red'>" + txtbank.Text.Trim() + "</font><br/>";
            }
            model.uba_receiveBank = txtbank.Text.Trim();
            //账户名称
            if (model.uba_receiveBankName != txtbankname.Text.Trim())
            {
                _content += "账户名称：" + model.uba_receiveBankName + "→<font color='red'>" + txtbankname.Text.Trim() + "</font><br/>";
            }
            model.uba_receiveBankName = txtbankname.Text.Trim();
            //收款账号
            if (model.uba_receiveBankNum != txtbanknum.Text.Trim())
            {
                _content += "收款账号：" + model.uba_receiveBankNum + "→<font color='red'>" + txtbanknum.Text.Trim() + "</font><br/>";
            }
            model.uba_receiveBankNum = txtbanknum.Text.Trim();
            //金额
            if (model.uba_money != Convert.ToDecimal(txtmoney.Text))
            {
                _content += "金额：" + model.uba_money + "→<font color='red'>" + txtmoney.Text.Trim() + "</font><br/>";
            }
            model.uba_money = Convert.ToDecimal(txtmoney.Text);
            //预付日期
            if (model.uba_foreDate != ConvertHelper.toDate(txtforedate.Text.Trim()))
            {
                _content += "预付日期：" + model.uba_foreDate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + txtforedate.Text.Trim() + "</font><br/>";
            }
            model.uba_foreDate = ConvertHelper.toDate(txtforedate.Text.Trim());
            //备注
            if (model.uba_remark != txtRemark.Text.Trim())
            {
                _content += "备注：" + model.uba_remark + "→<font color='red'>" + txtRemark.Text.Trim() + "</font><br/>";
            }
            model.uba_remark = txtRemark.Text.Trim();
            return bll.Update(model, _content.ToString(), manager);
        }

        protected void ddltype_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            if (!string.IsNullOrEmpty(ddl.SelectedValue))
            {
                if (ddl.SelectedValue == "0")
                {
                    ddlfunction.Visible = true;
                    txtfunction.Visible = false;
                }
                else
                {
                    ddlfunction.Visible = false;
                    txtfunction.Visible = true;
                }
            }
        }

        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                ChkAdminLevel("sys_unBusiness_list", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    PrintJscriptMsg(result, "");
                    return;
                }
                if (fromOrder == "true")
                {
                    PrintMsg("修改非业务支付申请成功！");
                }
                else
                {
                    JscriptMsg("修改非业务支付申请成功！", "unBusinessPay_list.aspx");
                }
            }
            else if (action == DTEnums.ActionEnum.Add.ToString())//添加
            {
                ChkAdminLevel("sys_unBusiness_list", DTEnums.ActionEnum.Add.ToString()); //检查权限
                int id = 0;
                result = DoAdd(out id);
                if (result != "")
                {
                    PrintJscriptMsg(result, "");
                    return;
                }
                if (fromOrder == "true")
                {
                    PrintMsg("添加非业务支付申请成功！");
                }
                else
                {
                    JscriptMsg("添加非业务支付申请成功！", "unBusinessPay_list.aspx");
                }
                if (fileUp.HasFile)
                {
                    for (int i = 0; i < fileUp.PostedFiles.Count; i++)
                    {
                        string savePath = Server.MapPath("~/uploadPay/");
                        if (!Directory.Exists(savePath))
                        {
                            //需要注意的是，需要对这个物理路径有足够的权限，否则会报错
                            //另外，这个路径应该是在网站之下，而将网站部署在C盘却把文件保存在D盘
                            Directory.CreateDirectory(savePath);
                        }
                        if (!Directory.Exists(savePath + "2/"+id+"/"))
                        {
                            Directory.CreateDirectory(savePath + "2/" + id + "/");
                        }
                        Model.payPic file = new Model.payPic();
                        file.pp_rid = id;
                        file.pp_type = 2;
                        file.pp_fileName = fileUp.PostedFiles[i].FileName;
                        file.pp_filePath = "uploadPay/2/" + id + "/" + fileUp.PostedFiles[i].FileName;
                        //file.pp_thumbFilePath = jo["thumb"].ToString();
                        file.pp_size = Math.Round((decimal)fileUp.PostedFiles[i].ContentLength / 1024, 2, MidpointRounding.AwayFromZero);
                        file.pp_addDate = DateTime.Now;
                        file.pp_addName = manager.real_name;
                        file.pp_addPerson = manager.user_name;

                        fileUp.PostedFiles[i].SaveAs(savePath + "2/" + id + "/" + fileUp.PostedFiles[i].FileName);//保存文件

                        new BLL.payPic().insertPayFile(file, manager);
                    }
                }
            }
        }
    }
}