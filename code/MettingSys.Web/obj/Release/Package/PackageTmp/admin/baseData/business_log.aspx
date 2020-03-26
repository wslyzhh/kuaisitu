<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="business_log.aspx.cs" Inherits="MettingSys.Web.admin.baseData.business_log" ValidateRequest="false" %>

<%@ Import Namespace="MettingSys.Common" %>

<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>业务日志</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
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
            <span>业务日志</span>
        </div>
        <!--/导航栏-->

        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            
        </div>
        <!--/工具栏-->
        <div class="searchbar">
                操作时间：
                <asp:TextBox ID="txtStartTime" runat="server" CssClass="input rule-date-input" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                            -
          <asp:TextBox ID="txtEndTime" runat="server" CssClass="input rule-date-input" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtStartTime\')}'})"></asp:TextBox>
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
                            <th width="8%">操作类型</th>
                            <th width="8%">订单号</th>
                            <th width="8%">客户ID</th>
                            <th width="8%">相关ID</th>
                            <th width="15%">标题</th>
                            <th align="left">内容</th>
                            <th align="left" width="10%">操作人</th>
                            <th align="left" width="10%">操作时间</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td align="center"><%# Eval("ol_type") %></td>
                        <td align="center"><%# Eval("ol_oid") %></td>
                        <td align="center"><%# Eval("ol_cid") %></td>
                        <td align="center"><%# Eval("ol_relateID") %></td>
                        <td align="center"><%# Eval("ol_title") %></td>
                        <td><%# Eval("ol_content") %></td>
                        <td><%# Eval("ol_operaterNum") %> <%# Eval("ol_operaterName") %></td>
                        <td><%# ConvertHelper.toDate(Eval("ol_operateDate")).Value.ToString("yyyy-MM-dd HH:mm:ss") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"8\">暂无记录</td></tr>" : ""%>
  </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <!--/列表-->
        <div class="dRemark">
            <p>1.关键字筛查字段为：订单号、客户ID、相关ID、标题、内容、操作人</p>            
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
