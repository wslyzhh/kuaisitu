<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="manager_list.aspx.cs" Inherits="MettingSys.Web.admin.manage.manager_list" ValidateRequest="false" %>

<%@ Import Namespace="MettingSys.Common" %>

<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>用户列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript">
        function removeDingtalkBind() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            $(".checkall input:checked").each(function () {
                var id = $(this).parent().next().val();
                var username = $(this).parent().parent().siblings(".uTd").children().html();
                var postData = { "username": username};
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/dingtalk_login.ashx?action=remove_OauthBind",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $("#tr" + id).children(".dingBind").html("");
                        if (data.status == 1) {                            
                            $("#tr" + id).children(".dingBind").html("<font color='green'>" + data.msg + "</font>");
                        } else {
                            $("#tr" + id).children(".dingBind").html("<font color='red'>" + data.msg + "</font>");
                        }
                    }
                });
            });
        }
        function sentMessage(userid) {
            var postData = { "id": userid};
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/dingtalk_login.ashx?action=sent_Message",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        layer.msg(data.msg);
                    }
                });
        }
    </script>
</head>

<body class="mainbody">
    <form id="form1" runat="server">
        <!--导航栏-->
        <div class="location">
            <a href="javascript:history.back(-1);" class="back"><i class="iconfont icon-up"></i><span>返回上一页</span></a>
            <a href="../center.aspx" class="home"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>用户列表</span>
        </div>
        <!--/导航栏-->

        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            <div class="toolbar">
                <div class="box-wrap">
                    <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                    <div class="l-list">
                        <ul class="icon-list">
                            <li><a href="manager_edit.aspx?action=<%=DTEnums.ActionEnum.Add %>"><i class="iconfont icon-close"></i><span>新增</span></a></li>
                            <li><a href="javascript:;" onclick="checkAll(this);"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                            <li>
                                <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton>
                            </li>
                            <li><a href="javascript:;" onclick="removeDingtalkBind();"><span>移除钉钉账号绑定</span></a></li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <!--/工具栏-->
        <div class="searchbar">
            关键字：
                            <asp:TextBox ID="txtKeywords" runat="server" CssClass="input" />
            <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
        </div>

        <!--列表-->
        <div class="table-container">
            <asp:Repeater ID="rptList" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr>
                            <th width="8%">选择</th>
                            <th align="left">用户名</th>
                            <th align="left" width="12%">姓名</th>
                            <th align="left" width="12%">角色</th>
                            <th align="left" width="12%">电话</th>
                            <th align="left" width="12%">岗位</th>
                            <th align="left" width="16%">添加时间</th>
                            <th width="8%">状态</th>
                            <th width="8%">钉钉是否绑定</th>
                            <th width="8%">操作</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr id="tr<%#Eval("id")%>">
                        <td align="center">
                            <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                            <asp:HiddenField ID="hidId" Value='<%#Eval("id")%>' runat="server" />
                        </td>
                        <td class="uTd"><a href="manager_edit.aspx?action=<%#DTEnums.ActionEnum.Edit %>&id=<%#Eval("id")%>"><%# Eval("user_name") %></a></td>
                        <td><%# Eval("real_name") %></td>
                        <td><%#new MettingSys.BLL.manager_role().GetTitle(Convert.ToInt32(Eval("role_id")))%></td>
                        <td><%# Eval("telephone") %></td>
                        <td><%# Eval("detaildepart") %></td>
                        <td><%#string.Format("{0:g}",Eval("add_time"))%></td>
                        <td align="center"><%#Eval("is_lock").ToString().Trim() == "0" ? "正常" : "禁用"%></td>
                        <td align="center" class="dingBind"><%#MettingSys.Common.Utils.ObjToInt(Eval("manager_id"))>0?"已绑定 <a href=\"javascript:;\" onclick=\"sentMessage('"+Eval("oauth_userid")+"')\">发消息</a>":"" %></td>
                        <td align="center"><a href="manager_edit.aspx?action=<%#DTEnums.ActionEnum.Edit %>&id=<%#Eval("id")%>">修改</a></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"10\">暂无记录</td></tr>" : ""%>
  </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <!--/列表-->

        <!--内容底部-->
        <div class="line20"></div>
        <div class="pagelist">
            <div class="l-btns">
                <span>显示</span><asp:TextBox ID="txtPageNum" runat="server" CssClass="pagenum" onkeydown="return checkNumber(event);"
                    OnTextChanged="txtPageNum_TextChanged" AutoPostBack="True"></asp:TextBox><span>条/页</span>
            </div>
            <div id="PageContent" runat="server" class="default"></div>
        </div>
        <!--/内容底部-->
    </form>
</body>
</html>
