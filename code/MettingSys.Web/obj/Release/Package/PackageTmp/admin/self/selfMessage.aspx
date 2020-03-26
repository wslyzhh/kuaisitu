<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="selfMessage.aspx.cs" Inherits="MettingSys.Web.admin.self.selfMessage" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>个人消息列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
</head>

<body class="mainbody">
    <form id="form1" runat="server">
        <!--导航栏-->
        <div class="location">
            <a href="javascript:history.back(-1);" class="back"><i class="iconfont icon-up"></i><span>返回上一页</span></a>
            <a href="../center.aspx" class="home"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>个人消息列表</span>
        </div>
        <!--/导航栏-->

        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            <div class="toolbar">
                <div class="box-wrap">
                    <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                    <div class="l-list">
                        <ul class="icon-list">
                            <li><a href="javascript:;" onclick="checkAll(this);"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                            <li>
                                <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete','批量删除个人消息，是否继续？');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                            <li>
                                <asp:LinkButton ID="btnRead" runat="server" OnClientClick="return ExePostBack('btnRead','批量标为已读状态，是否继续？');" OnClick="btnRead_Click"><i class="iconfont icon-message"></i><span>标为已读</span></asp:LinkButton></li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <!--/工具栏-->
        <div class="searchbar">
            已读状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlisRead" runat="server">
                                    <asp:ListItem Value="">全部</asp:ListItem>
                                    <asp:ListItem Value="1">已读</asp:ListItem>
                                    <asp:ListItem Value="0">未读</asp:ListItem>
                                </asp:DropDownList>
                            </div>
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
                            <th width="6%">选择</th>
                            <th align="left" width="15%">标题</th>
                            <th align="left">内容</th>
                            <th align="left" width="10%">时间</th>
                            <th align="left" width="6%">是否已读</th>
                            <%--<th width="10%">操作</th>--%>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td align="center">
                            <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                            <asp:HiddenField ID="hidId" Value='<%#Eval("me_id")%>' runat="server" />
                        </td>
                        <td><%#Convert.ToBoolean(Eval("me_isRead"))?Eval("me_title"):"<span style='font-weight: bolder;'>"+Eval("me_title")+"</span>"%></td>
                        <td><%#Eval("me_content")%></td>
                        <td><%#Eval("me_addDate")%></td>
                        <td><%#Convert.ToBoolean(Eval("me_isRead"))?"<font color='green'>已读</font>":"<span style='font-weight: bolder;'>未读</span>"%></td>
                        <%--<td align="center"><a href="selfMessage_edit.aspx?action=<%#DTEnums.ActionEnum.Edit %>&id=<%#Eval("me_id")%>">查看</a></td>--%>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"5\">暂无记录</td></tr>" : ""%>
  </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <!--/列表-->
        <div class="dRemark">
            <p>1.关键字筛查字段为：标题、内容</p>
        </div>
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
