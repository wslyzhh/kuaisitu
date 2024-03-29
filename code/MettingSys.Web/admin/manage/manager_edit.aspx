﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="manager_edit.aspx.cs" Inherits="MettingSys.Web.admin.manage.manager_edit" ValidateRequest="false" %>

<%@ Import Namespace="MettingSys.Common" %>

<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑用户</title>
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/Validform_v5.3.2_min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/webuploader/webuploader.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/uploader.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript">
        $(function () {
            //初始化表单验证
            $("#form1").initValidform();
            //初始化上传控件
            $(".upload-img").InitUploader({ sendurl: "../../tools/upload_ajax.ashx", swf: "../../scripts/webuploader/uploader.swf" });

            $("#ddlParentId").change(function () {                
                $.getJSON("../../tools/admin_ajax.ashx?action=getDepartText&olddepartid="+$("#hOldDepartID").val()+"&departid=" + $(this).val(), function (json) {
                    $("#labdepartStr").text(json.textTree);
                    $("#hIDTree").val(json.idTree);
                    $("#hTextTree").val(json.textTree);
                    if (json.username != "") {
                        $("#hUsername").val(json.username);
                        $("#txtUserName").val(json.username);
                    }
                });
            });
            //绑定角色权限
            $("#ddlRoleId").change(function () {
                $(".icon-btn").siblings().remove();
                $("#hCodeStr").val("");
                if ($(this).val() != "") {
                    $.getJSON("../../tools/admin_ajax.ashx?action=getRolePermission&roleid=" + $(this).val(), function (json) {
                        var codestr = ",";
                        $.each(json, function (index, item) {
                            execSpecHtml(item.urp_code);
                            codestr += item.urp_code + ",";
                        });
                        $("#hCodeStr").val(codestr);

                    });
                }
            });
        });
        //权限选择
        function choosePermission(obj) {
            var liObj = $(obj).parent();
            var d = top.dialog({
                id: 'specDialogId',
                padding: 0,
                title: "选择员工权限",
                url: 'admin/manage/selectPermission.aspx'
            }).showModal();
            //将容器对象传进去
            d.data = liObj;
            d.hcode = $("#hCodeStr");
        }
        //删除附件节点
        function delNode(obj, code) {
            $("#hCodeStr").val($("#hCodeStr").val().replace("," + code + ",", ","));
            $(obj).parent().remove();
        }
        //创建权限的HTML
        function execSpecHtml(code) {
            var liHtml = "<li>"
                + "<input name='hide_permission' type='hidden' value='" + code + "' />"
                + "<a href='javascript:;' class='del' title='删除' onclick='delNode(this,\""+code+"\");'><i class='iconfont icon-remove'></i></a>"
                + "<span>" + code + "</span>"
                + "</li>";
            $(".icon-btn").before(liHtml);
        }
    </script>
</head>

<body class="mainbody">
    <form id="form1" runat="server">
        <!--导航栏-->
        <div class="location">
            <a href="manager_list.aspx" class="back"><i class="iconfont icon-up"></i><span>返回列表页</span></a>
            <a href="../center.aspx"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <a href="manager_list.aspx"><span>用户列表</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>编辑用户</span>
        </div>
        <div class="line10"></div>
        <!--/导航栏-->

        <!--内容-->
        <div id="floatHead" class="content-tab-wrap">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a class="selected" href="javascript:;">用户信息</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="tab-content">

            <%--<dl>
                <dt>信息待审</dt>
                <dd>
                    <div class="rule-single-checkbox">
                        <asp:CheckBox ID="cbIsAudit" runat="server" />
                    </div>
                    <span class="Validform_checktip">*发布的文章是否需要审核才显示</span>
                </dd>
            </dl>--%>
            <dl>
                <dt>用户角色</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlRoleId" runat="server" datatype="*" errormsg="请选择用户角色" sucmsg=" "></asp:DropDownList>
                    </div>
                </dd>
            </dl>
            <dl>
                <dt>岗位</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlParentId" runat="server"></asp:DropDownList>
                    </div>
                    <asp:Label ID="labdepartStr" runat="server"></asp:Label>
                    <asp:HiddenField ID="hIDTree" runat="server" />
                    <asp:HiddenField ID="hTextTree" runat="server" />
                    <asp:HiddenField ID="hUsername" runat="server" />
                    <asp:HiddenField ID="hOldDepartID" runat="server" />
                </dd>
            </dl>
            <dl>
                <dt>用户名</dt>
                <dd>
                    <asp:TextBox ID="txtUserName" runat="server" CssClass="input normal" onkeyup="cToUpper(this)"></asp:TextBox>
                    <span class="Validform_checktip">由系统自动生成，不可修改</span>
                </dd>
            </dl>
            <dl>
                <dt>登录密码</dt>
                <dd>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="input normal" TextMode="Password" datatype="*6-20" nullmsg="请设置密码" errormsg="密码范围在6-20位之间" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span></dd>
            </dl>
            <dl>
                <dt>确认密码</dt>
                <dd>
                    <asp:TextBox ID="txtPassword1" runat="server" CssClass="input normal" TextMode="Password" datatype="*" recheck="txtPassword" nullmsg="请再输入一次密码" errormsg="两次输入的密码不一致" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span></dd>
            </dl>
            <dl>
                <dt>头像</dt>
                <dd>
                    <asp:TextBox ID="txtAvatar" runat="server" CssClass="input normal upload-path" />
                    <div class="upload-box upload-img"></div>
                </dd>
            </dl>
            <dl>
                <dt>姓名</dt>
                <dd>
                    <asp:TextBox ID="txtRealName" runat="server" CssClass="input normal" datatype="*2-20" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>电话</dt>
                <dd>
                    <asp:TextBox ID="txtTelephone" runat="server" CssClass="input normal" datatype="*2-20" sucmsg=" "></asp:TextBox>
                </dd>
            </dl>
            <dl>
                <dt>邮箱</dt>
                <dd>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="input normal"></asp:TextBox>
                </dd>
            </dl>
            <dl>
                <dt>具体岗位</dt>
                <dd>
                    <asp:TextBox ID="txtDetailDepart" runat="server" CssClass="input normal"></asp:TextBox>
                </dd>
            </dl>
            <dl>
                <dt>是否启用</dt>
                <dd>
                    <div class="rule-single-checkbox">
                        <asp:CheckBox ID="cbIsLock" runat="server" Checked="True" />
                    </div>
                    <span class="Validform_checktip">*不启用则无法使用该账户登录</span>
                </dd>
            </dl>
            <%--<dl>
                <dt>管理权限</dt>
                <dd>
                    <asp:HiddenField ID="hCodeStr" runat="server" />
                    <div class="txt-item">
                        <ul>
                            <asp:Repeater ID="rptPermission" runat="server">
                                <ItemTemplate>
                                    <li>
                                        <input name="hide_permission" type="hidden" value="<%# DataBinder.Eval(Container.DataItem, "urp_code")%>" />
                                        <a href="javascript:;" class="del" title="删除" onclick="delNode(this,'<%# DataBinder.Eval(Container.DataItem, "urp_code")%>');"><i class="iconfont icon-remove"></i></a>
                                        <span><%# DataBinder.Eval(Container.DataItem, "urp_code")%></span>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="icon-btn">
                                <a href="javascript:" onclick="choosePermission(this)"><i class="iconfont icon-close"></i></a>
                            </li>
                        </ul>
                    </div>
                </dd>
            </dl>--%>
        </div>
        <!--/内容-->

        <!--工具栏-->
        <div class="page-footer">
            <div class="btn-wrap">
                <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btn" OnClick="btnSubmit_Click" />
                <input name="btnReturn" type="button" value="返回上一页" class="btn yellow" onclick="javascript: history.back(-1);" />
            </div>
        </div>
        <!--/工具栏-->

    </form>
</body>
</html>
