<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="unBusinessPay_edit.aspx.cs" Inherits="MettingSys.Web.admin.unBusiness.unBusinessPay_edit" %>

<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑站点</title>
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/Validform_v5.3.2_min.js"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/webuploader/webuploader.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/payUploader.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript">
        $(function () {
            //初始化表单验证
            $("#form1").initValidform();
            //初始化上传控件
            $(".upload-album").InitUploader({ btntext: "批量上传", multiple: true, ftype: 2, pid:<%=id%>, thumbnail: false, filetypes: "<%=sysConfig.fileextension %>", filesize: <%=sysConfig.attachsize %>, sendurl: "../../tools/upload_ajax.ashx", swf: "../../scripts/webuploader/uploader.swf" });
            $(".upload-album").children(".upload-btn").append("<div class='webuploader-pick1'>文件类型：<%=sysConfig.fileextension %>，文件大小限制：<%=sysConfig.attachsize %>KB</div>");

            $("#btnAudit").click(function () {
                layer.open({
                    type: 1,
                    title: '审批',
                    closeBtn: 0,
                    area: ['516px', '300px'],
                    //skin: 'layui-layer-nobg', //没有背景色
                    shadeClose: true,
                    content: $('#divflag')
                });
            });
        });
        function showPic(url) {
            parent.layer.open({
                type: 1,
                title: false,
                closeBtn: 0,
                shadeClose: true,
                content: '<img src="' + url + '"/>'
            });
        }
        function dealcheck() {
            if ($("#ddlflag").val() == "") {
                jsprint("请选择审批状态");
                return;
            }
            var postData = { "id": <%=id%>, "ctype": $("#ddlchecktype").val(), "cstatus": $("#ddlflag").val(), "remark": $("#txtCheckRemark").val() };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/unBusiness_ajax.ashx?action=checkStatus",
                data: postData,
                dataType: "json",
                success: function (data) {
                    if (data.status == 0) {
                        var d = top.dialog({ content: data.msg }).show();
                        setTimeout(function () {
                            d.close().remove();
                            location.reload();
                        }, 2000);
                    } else {
                        top.dialog({
                            title: '提示',
                            content: data.msg,
                            okValue: '确定',
                            ok: function () { }
                        }).showModal();
                    }
                }
            });
        }
    </script>
    <style type="text/css">
        .photo-list ul li {
            float: none;
            text-align: left;
        }

        .upload-box .upload-progress {
            position: relative;
        }

        .webuploader-pick1 {
            padding-left: 8px;
            display: inline-block;
            line-height: 30px;
            height: 30px;
            color: red;
            text-align: center;
            overflow: hidden;
        }
    </style>
</head>

<body class="mainbody">
    <form id="form1" runat="server">
        <!--导航栏-->
        <div class="location" style="<%=fromOrder=="true"?"display:none;": "" %>">
            <a href="unBusinessPay_list.aspx" class="back"><i class="iconfont icon-up"></i><span>返回列表页</span></a>
            <a href="../center.aspx"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <a href="unBusinessPay_list.aspx"><span>非业务支付申请列表</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>编辑非业务支付申请</span>
        </div>
        <div class="line10"></div>
        <!--/导航栏-->

        <!--内容-->
        <div id="floatHead" class="content-tab-wrap" style="<%=fromOrder=="true"?"display:none;": "" %>">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a class="selected" href="javascript:;">非业务支付申请</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="tab-content" style="<%=fromOrder=="true"?"border:none;padding:0;": "" %>">
            <dl>
                <dt>支付类别</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddltype" runat="server" OnSelectedIndexChanged="ddltype_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                    <asp:Label ID="labtype" runat="server"></asp:Label>
                </dd>
            </dl>
            <dl>
                <dt>支付用途</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlfunction" runat="server">
                        </asp:DropDownList>
                    </div>
                    <asp:TextBox ID="txtfunction" runat="server" TextMode="MultiLine" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip" id="tip0" runat="server">*</span>
                </dd>
            </dl>
            <dl>
                <dt>用途说明</dt>
                <dd>
                    <asp:TextBox ID="txtinstruction" runat="server" TextMode="MultiLine" CssClass="input normal"></asp:TextBox>
                </dd>
            </dl>
            <dl>
                <dt>收款银行</dt>
                <dd>
                    <asp:TextBox ID="txtbank" runat="server" CssClass="input normal"></asp:TextBox>
                </dd>
            </dl>
            <dl>
                <dt>账户名称</dt>
                <dd>
                    <asp:TextBox ID="txtbankname" runat="server" CssClass="input normal"></asp:TextBox>
                </dd>
            </dl>
            <dl>
                <dt>收款账号</dt>
                <dd>
                    <asp:TextBox ID="txtbanknum" runat="server" CssClass="input normal"></asp:TextBox>
                </dd>
            </dl>
            <dl>
                <dt>金额</dt>
                <dd>
                    <asp:TextBox ID="txtmoney" runat="server" CssClass="input small" datatype="/^-?[1-9]+(\.\d+)?$|^-?0(\.\d+)?$|^-?[1-9]+[0-9]*(\.\d+)?$/" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*请输入有效金额</span>
                </dd>
            </dl>
            <dl>
                <dt>预付日期</dt>
                <dd>
                    <asp:TextBox ID="txtforedate" runat="server" CssClass="input rule-date-input" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate: '%y-%M-#{%d}'})" datatype="*" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>备注</dt>
                <dd>
                    <asp:TextBox ID="txtRemark" runat="server" CssClass="input normal" TextMode="MultiLine" />
                </dd>
            </dl>
            <dl id="dlAddUpload" runat="server">
                <dt>上传图片</dt>
                <dd>
                    <asp:FileUpload runat="server" ID="fileUp" AllowMultiple="true" />
                    <div style="color: red;">文件类型：<%=sysConfig.fileextension %>，文件大小限制：<%=sysConfig.attachsize %>KB</div>
                </dd>
            </dl>
            <dl id="dlEditUpload" runat="server">
                <dt>上传图片</dt>
                <dd>
                    <div class="upload-box upload-album" id="uploadDiv" runat="server"></div>
                    <input type="hidden" name="hidFocusPhoto" id="hidFocusPhoto" runat="server" class="focus-photo" />
                    <div class="photo-list" style="margin-top: 26px;">
                        <ul>
                            <asp:Repeater ID="rptAlbumList" runat="server">
                                <ItemTemplate>
                                    <li>
                                        <input type="hidden" name="hid_photo_name" value="" />
                                        <input type="hidden" name="hid_photo_remark" value="" />
                                        <div>
                                            <i class="iconfont icon-attachment"></i>
                                            <span class="remark"><i><a target="_blank" href="<%#"../../"+Eval("[\"pp_filePath\"]") %>"><%#Eval("[\"pp_fileName\"]") %></a></i></span>
                                            <span style="font-weight: bolder;"><%#Eval("[\"pp_size\"]") %>KB</span>
                                            <%#Eval("[\"uba_flag1\"]").ToString()=="2" && Eval("[\"uba_flag2\"]").ToString()!="1" && Eval("[\"uba_flag3\"]").ToString()!="1" ?"":"<a href=\"javascript:;\" onclick=\"delImg(this,"+Eval("[\"pp_id\"]")+");\">删除</a>" %>
                                        </div>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                </dd>
            </dl>
        </div>
        <!--/内容-->

        <!--工具栏-->
        <div class="page-footer">
            <div class="btn-wrap" style="<%=fromOrder=="true"?"text-align:center;": "" %>">
                <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btn" OnClick="btnSubmit_Click" />
                <input type="button" id="btnAudit" runat="server" class="btn" value="审批" />
                <input name="btnReturn" type="button" value="返回上一页" class="btn yellow" style="<%=fromOrder=="true"?"display:none;": "" %>" onclick="javascript: history.back(-1);" />
            </div>
        </div>
        <!--/工具栏-->
        <div class="table-container" id="divflag" style="display: none;">
            <div class="tab-content" style="border: none;">                
                <dl>
                    <dt style="width: 100px;">审批类型</dt>
                    <dd>
                        <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlchecktype" runat="server">
                                        <asp:ListItem Value="1">部门审批</asp:ListItem>
                                        <asp:ListItem Value="2">财务审批</asp:ListItem>
                                        <asp:ListItem Value="3">总经理审批</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                    </dd>
                </dl>
                <dl>
                    <dt style="width: 100px;">审批状态</dt>
                    <dd>
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddlflag" runat="server">
                            </asp:DropDownList>
                        </div>
                    </dd>
                </dl>
                <dl>
                    <dt style="width: 100px;">备注</dt>
                    <dd>
                        <asp:TextBox ID="txtCheckRemark" runat="server" CssClass="input normal" TextMode="MultiLine" />
                    </dd>
                </dl>
                <dl>
                    <dt style="width: 100px;">&nbsp;</dt>
                    <dd>
                        <input type="button" value="提交" class="btn" onclick="dealcheck()" />
                    </dd>
                </dl>
            </div>
        </div>
    </form>
</body>
</html>
