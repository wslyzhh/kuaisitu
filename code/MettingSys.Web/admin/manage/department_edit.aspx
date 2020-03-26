<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="department_edit.aspx.cs" Inherits="MettingSys.Web.admin.manage.department_edit" %>

<%@ Import Namespace="MettingSys.Common" %>

<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑部门岗位</title>
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/Validform_v5.3.2_min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js?<%=DateTime.Now.ToString("yyyyMMddhhmmss") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript">
        $(function () {
            //初始化表单验证
            $("#form1").initValidform();
        });
    </script>
</head>

<body class="mainbody">
    <form id="form1" runat="server">
        <!--导航栏-->
        <div class="location">
            <a href="department_list.aspx" class="back"><i class="iconfont icon-up"></i><span>返回列表页</span></a>
            <a href="../center.aspx"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <a href="department_list.aspx"><span>部门岗位</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span><%=action==DTEnums.ActionEnum.Add.ToString()?"添加":"编辑" %>部门岗位</span>
        </div>
        <div class="line10"></div>
        <!--/导航栏-->

        <!--内容-->
        <div id="floatHead" class="content-tab-wrap">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a class="selected" href="javascript:;">基本信息</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="tab-content">
            <dl>
                <dt>上级机构</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlParentId" runat="server" OnSelectedIndexChanged="ddlParentId_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    </div>
                    <asp:Label ID="labDepartText" runat="server"></asp:Label>
                    <span class="Validform_checktip">添加后不能再更改</span>
                </dd>
            </dl>
            <dl>
                <dt>机构类别</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddltype" runat="server" OnSelectedIndexChanged="ddltype_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    </div>
                    <span class="Validform_checktip">添加后不能再更改</span>
                </dd>
            </dl>
            <dl>
                <dt>排序数字</dt>
                <dd>
                    <asp:TextBox ID="txtSortId" runat="server" CssClass="input small" datatype="n" sucmsg=" ">99</asp:TextBox>
                    <span class="Validform_checktip">*数字，越小越向前</span>
                </dd>
            </dl>
            <dl>
                <dt>机构全称</dt>
                <dd>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="input normal" datatype="*1-20" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl id="dlsubtitle" runat="server">
                <dt>机构简称</dt>
                <dd>
                    <asp:TextBox ID="txtSubTitle" runat="server" CssClass="input small"></asp:TextBox>
                    <span class="Validform_checktip">当机构类别为公司时必填，添加后不能再更改</span>
                </dd>
            </dl>
            <dl id="dlarea" runat="server">
                <dt>名称简码</dt>
                <dd>
                    <asp:TextBox ID="txtArea" runat="server" CssClass="input small"></asp:TextBox>
                    <span class="Validform_checktip">两位大写字母，当机构类别为公司时必填，添加后不能再更改</span>
                </dd>
            </dl>
            <dl id="dlgroup" runat="server">
                <dt>是否总部</dt>
                <dd>
                    <div class="rule-single-checkbox" id="isgroupDiv" runat="server">
                        <asp:CheckBox ID="cbIsGroup" runat="server" />
                    </div>
                    <asp:Label ID="labIsGroup" runat="server"></asp:Label>
                    <span class="Validform_checktip">添加后不能再更改</span>
                </dd>
            </dl>
            <dl>
                <dt>是否启用</dt>
                <dd>
                    <div class="rule-single-checkbox">
                        <asp:CheckBox ID="cbIsUse" runat="server" Checked="true" />
                    </div>
                </dd>
            </dl>
            <dl>
                <dt></dt>
                <dd><font color="red">编辑时只能更改排序数字、机构全称、状态</font></dd>
            </dl>
        </div>
        <!--/内容-->

        <!--工具栏-->
        <div class="page-footer">
            <div class="btn-wrap">
                <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btn" OnClick="btnSubmit_Click" />
                <input name="btnReturn" type="button" value="返回上一页" class="btn yellow" onclick="javascript: window.location.href = 'department_list.aspx';" />
            </div>
        </div>
        <!--/工具栏-->
    </form>
</body>
</html>
