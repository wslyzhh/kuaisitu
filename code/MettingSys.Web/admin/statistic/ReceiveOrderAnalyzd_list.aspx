<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceiveOrderAnalyzd_list.aspx.cs" Inherits="MettingSys.Web.admin.statistic.ReceiveOrderAnalyzd_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>策划与设计</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery.form.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <style type="text/css">
        .date-input {
            width: 100px;
        }
    </style>
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <!--导航栏-->
        <div class="location" style="margin-bottom: 10px;">
            <a href="javascript:history.back(-1);" class="back"><i class="iconfont icon-up"></i><span>返回上一页</span></a>
            <a href="../center.aspx" class="home"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>策划与设计</span>
        </div>
        <div class="tab-content" style="padding-top: 0;">
            <!--/导航栏-->
            <div class="searchbar">
                活动结束日期：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
                -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
                接单人员
                        <asp:TextBox ID="txtPerson" runat="server" Width="100px" CssClass="input" />
                人员岗位
                        <asp:TextBox ID="txtDepart" runat="server" Width="100px" CssClass="input" />
                归属地：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddlarea" runat="server"></asp:DropDownList>
                        </div>
                下单区域：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddlorderarea" runat="server"></asp:DropDownList>
                        </div>
                订单状态：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddlstatus" runat="server"></asp:DropDownList>
                        </div>
                接单状态：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddldstatus" runat="server"></asp:DropDownList>
                        </div>
                锁单状态：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddllock" runat="server"></asp:DropDownList>
                        </div>
                <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                <asp:LinkButton ID="btnExcel" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>
            </div>

            <!--列表-->
            <div class="table-container">
                <asp:Repeater ID="rptList" runat="server">
                    <HeaderTemplate>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                            <tr>
                                <th width="10%">人员</th>
                                <th width="10%">岗位</th>
                                <th width="10%">策划订单数</th>
                                <th width="10%">设计订单数</th>
                                <th>合计订单数</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr style="text-align: center;">
                            <td><%#Eval("op_number")%>(<%#Eval("op_name")%>)</td>
                            <td><%#Eval("detaildepart")%></td>
                            <td><a href="OrderAnalyze_list.aspx?fromReceiveOrder=0&txtPerson3=<%#Eval("op_number")%>&txtsDate1=<%=_sdate %>&txteDate1=<%=_edate %>&ddlarea=<%=_area %>&ddlorderarea=<%=_orderarea %>&ddlstatus=<%=_status %>&ddldstatus=<%=_dstatus %>&ddllock=<%=_lockstatus %>"><%#Eval("type3")%></a></td>
                            <td><a href="OrderAnalyze_list.aspx?fromReceiveOrder=0&txtPerson5=<%#Eval("op_number")%>&txtsDate1=<%=_sdate %>&txteDate1=<%=_edate %>&ddlarea=<%=_area %>&ddlorderarea=<%=_orderarea %>&ddlstatus=<%=_status %>&ddldstatus=<%=_dstatus %>&ddllock=<%=_lockstatus %>"><%#Eval("type5")%></a></td>
                            <td><span <%# Utils.ObjToInt(Eval("sumType"),0)>=3?"style='color:red;'":"style='color:blue;'"%> style="color:red;"><%#Eval("sumType")%></span></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"5\">暂无记录</td></tr>" : ""%>
            </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <!--/列表-->
            <div style="font-size: 12px;line-height: 1.6em;">
                <span style="display: block;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，策划订单数量：<asp:Label ID="pOrder3Count" runat="server">0</asp:Label>，设计订单数量：<asp:Label ID="pOrder5Count" runat="server">0</asp:Label>，合计订单数量：<asp:Label ID="pOrderCount" runat="server">0</asp:Label></span>
                <span style="display: block; float: left;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，策划订单数量：<asp:Label ID="tOrder3Count" runat="server">0</asp:Label>，设计订单数量：<asp:Label ID="tOrder5Count" runat="server">0</asp:Label>，合计订单数量：<asp:Label ID="tOrderCount" runat="server">0</asp:Label></span>
            </div>
            <div class="dRemark">
                <p></p>
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
        </div>
    </form>
</body>
</html>
