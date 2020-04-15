using MettingSys.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.finance
{
    public partial class paydetail_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        protected int id = 0, finid = 0, cid = 0;

        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;
        protected string oID = string.Empty,cname=string.Empty, contentText = string.Empty;
        protected string fromOrder = "";//true时表示从订单页面添加

        protected void Page_Load(object sender, EventArgs e)
        {
            action = DTRequest.GetQueryString("action");
            this.oID = DTRequest.GetQueryString("oID");
            fromOrder = DTRequest.GetQueryString("fromOrder");

            //ChkAdminLevel("sys_payment_detail0", action); //检查权限
            dlEditUpload.Visible = false;
            dlAddUpload.Visible = false;
            uploadDiv.Visible = false;
            if (action == DTEnums.ActionEnum.Add.ToString())
            {
                dlAddUpload.Visible = true;
            }
            manager = GetAdminInfo();
            if (!string.IsNullOrEmpty(action) && action == DTEnums.ActionEnum.Edit.ToString())
            {
                uploadDiv.Visible = true;
                dlEditUpload.Visible = true;
                this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                this.id = DTRequest.GetQueryInt("id");
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.ReceiptPayDetail().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            //查看
            if (action == DTEnums.ActionEnum.View.ToString())
            {
                dlEditUpload.Visible = true;
                this.action = DTEnums.ActionEnum.View.ToString();//修改类型
                this.id = DTRequest.GetQueryInt("id");
                btnSubmit.Visible = false;
            }
            if (!Page.IsPostBack)
            {
                finid = DTRequest.GetQueryInt("id",0);
                cid = DTRequest.GetQueryInt("cid", 0);
                cname = DTRequest.GetQueryString("cname");
                contentText = DTRequest.GetQueryString("contentText");
                if (action == DTEnums.ActionEnum.Add.ToString())
                {
                    hCusId.Value = cid.ToString();
                    txtCusName.Text = cname;
                    txtContent.Text = contentText;                    
                }
                if (action == DTEnums.ActionEnum.Edit.ToString() || action == DTEnums.ActionEnum.View.ToString()) //修改或查看
                {
                    ShowInfo(this.id);
                }
            }
        }        
        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            DataSet ds = bll.GetList(0, "rpd_id=" + _id + "", "");
            btnAudit.Visible = false;
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                oID = dr["rpd_oid"].ToString();
                txtCusName.Text = dr["c_name"].ToString();
                hCusId.Value = dr["rpd_cid"].ToString();
                txtBank.Text = dr["cb_bank"].ToString() + "(" + dr["cb_bankNum"].ToString() + ")";
                hBankId.Value = dr["rpd_cbid"].ToString();
                txtMoney.Text = dr["rpd_money"].ToString();
                if (dr["rpd_foredate"] != null)
                {
                    txtforedate.Text = Convert.ToDateTime(dr["rpd_foredate"]).ToString("yyyy-MM-dd");
                }               
                txtContent.Text = dr["rpd_content"].ToString();

                rptAlbumList.DataSource = new BLL.payPic().GetList(1, "pp_type=1 and pp_rid=" + _id + "", "pp_addDate desc");
                rptAlbumList.DataBind();

                if ((manager.area == dr["rpd_area"].ToString() && new BLL.permission().checkHasPermission(manager, "0603")) || new BLL.permission().checkHasPermission(manager, "0402,0601"))
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
                        ddlflag.SelectedValue = dr["rpd_flag1"].ToString();
                        txtCheckRemark.Text = dr["rpd_checkRemark1"].ToString();
                    }
                    else if (new BLL.permission().checkHasPermission(manager, "0402"))//财务审批
                    {
                        ddlchecktype.SelectedValue = "2";
                        ddlflag.SelectedValue = dr["rpd_flag2"].ToString();
                        txtCheckRemark.Text = dr["rpd_checkRemark2"].ToString();
                    }
                    else if (new BLL.permission().checkHasPermission(manager, "0601"))//总经理审批
                    {
                        ddlchecktype.SelectedValue = "3";
                        ddlflag.SelectedValue = dr["rpd_flag3"].ToString();
                        txtCheckRemark.Text = dr["rpd_checkRemark3"].ToString();
                    }
                }
            }

        }
        #endregion

        #region 增加操作=================================
        private string DoAdd(out int id)
        {
            id = 0;
            Model.ReceiptPayDetail model = new Model.ReceiptPayDetail();
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();     
            manager = GetAdminInfo();
            model.rpd_type = false;
            model.rpd_oid = oID;
            model.rpd_cid = Utils.StrToInt(hCusId.Value,0);
            model.rpd_money = Utils.StrToDecimal(txtMoney.Text.Trim(),0);
            model.rpd_foredate = ConvertHelper.toDate(txtforedate.Text.Trim());
            model.rpd_content = txtContent.Text.Trim();
            model.rpd_personNum = manager.user_name;
            model.rpd_personName = manager.real_name;
            model.rpd_adddate = DateTime.Now;
            model.rpd_cbid = Utils.StrToInt(hBankId.Value, 0);
            model.rpd_flag1 = 0;
            model.rpd_flag2 = 0;
            model.rpd_flag3 = 0;
            //model.rpd_area = manager.area;
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
            return bll.AddReceiptPay(model, manager,out id);
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
            BLL.ReceiptPayDetail bll = new BLL.ReceiptPayDetail();
            Model.ReceiptPayDetail model = bll.GetModel(_id);
            manager = GetAdminInfo();
            string _content = string.Empty;
            if (model.rpd_cid.ToString() != hCusId.Value)
            {
                _content += "付款对象ID：" + model.rpd_cid + "→<font color='red'>" + hCusId.Value + "</font><br/>";
            }
            model.rpd_cid = Utils.StrToInt(hCusId.Value, 0);
            bool updateMoney = false;
            if (model.rpd_money.ToString() != txtMoney.Text.Trim())
            {
                if ((model.rpd_money<0 && Utils.ObjToDecimal(txtMoney.Text.Trim(),0) >= 0) || (model.rpd_money >= 0 && Utils.ObjToDecimal(txtMoney.Text.Trim(), 0) < 0))
                {
                    updateMoney = true;//表示金额从负数改为正数，或从正数改为负数
                }
                _content += "付款金额：" + model.rpd_money + "→<font color='red'>" + txtMoney.Text.Trim() + "</font><br/>";
            }
            model.rpd_money = Utils.StrToDecimal(txtMoney.Text.Trim(), 0);
            if (model.rpd_foredate.Value.ToString("yyyy-MM-dd") != txtforedate.Text.Trim())
            {
                _content += "预付日期：" + model.rpd_foredate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + txtforedate.Text.Trim() + "</font><br/>";
            }
            model.rpd_foredate = ConvertHelper.toDate(txtforedate.Text.Trim());
            if (model.rpd_content != txtContent.Text.Trim())
            {
                _content += "付款内容：" + model.rpd_content + "→<font color='red'>" + txtContent.Text.Trim() + "</font><br/>";
            }
            model.rpd_content = txtContent.Text.Trim();
            if (model.rpd_cbid != Utils.StrToInt(hBankId.Value,0))
            {
                _content += "客户银行账号：" + model.rpd_cbid + "→<font color='red'>" + hBankId.Value + "</font><br/>";
            }
            model.rpd_cbid = Utils.StrToInt(hBankId.Value, 0);
            return bll.Update(model, _content, manager, updateMoney);
        }
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                //ChkAdminLevel("sys_payment_detail0", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    PrintJscriptMsg(result, "");
                    return;
                }
                if (fromOrder == "true")
                {
                    PrintMsg("修改付款明细成功！");
                }
                else
                {
                    JscriptMsg("修改付款明细成功！", "paydetail_list.aspx");
                }
            }
            else //添加
            {
                //ChkAdminLevel("sys_payment_detail0", DTEnums.ActionEnum.Add.ToString()); //检查权限
                int id = 0;
                result = DoAdd(out id);
                if (result != "")
                {
                    PrintJscriptMsg(result, "");
                    return;
                }
                if (fromOrder == "true")
                {
                    PrintMsg("添加付款明细成功！");
                }
                else
                {
                    JscriptMsg("添加付款明细成功！", "paydetail_list.aspx");
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
                        if (!Directory.Exists(savePath + "1/" + id + "/"))
                        {
                            Directory.CreateDirectory(savePath + "1/" + id + "/");
                        }
                        Model.payPic file = new Model.payPic();
                        file.pp_rid = id;
                        file.pp_type = 1;
                        file.pp_fileName = fileUp.PostedFiles[i].FileName;
                        file.pp_filePath = "uploadPay/1/" + id + "/" + fileUp.PostedFiles[i].FileName;
                        //file.pp_thumbFilePath = jo["thumb"].ToString();
                        file.pp_size = Math.Round((decimal)fileUp.PostedFiles[i].ContentLength / 1024, 2, MidpointRounding.AwayFromZero);
                        file.pp_addDate = DateTime.Now;
                        file.pp_addName = manager.real_name;
                        file.pp_addPerson = manager.user_name;

                        fileUp.PostedFiles[i].SaveAs(savePath + "1/" + id + "/" + fileUp.PostedFiles[i].FileName);//保存文件

                        new BLL.payPic().insertPayFile(file, manager);
                    }
                }
            }
        }
    }
}