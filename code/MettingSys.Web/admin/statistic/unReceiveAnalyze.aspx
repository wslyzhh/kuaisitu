<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="unReceiveAnalyze.aspx.cs" Inherits="MettingSys.Web.admin.statistic.unReceiveAnalyze" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>员工未收款统计</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript">
        $(function () {
            $.getJSON("../../tools/business_ajax.ashx?action=getAllCustomer", function (json) {
                $('#txtCusName').devbridgeAutocomplete({
                    lookup: json,
                    minChars: 1,
                    onSelect: function (suggestion) {
                        $('#hCusId').val(suggestion.id);
                    },
                    showNoSuggestionNotice: true,
                    noSuggestionNotice: '抱歉，没有匹配的选项',
                    groupBy: 'type'
                });
            });
            $("#txtCusName").change(function () {
                $("#hCusId").val("");
            });
        })
    </script>
    <style type="text/css">
        .date-input {
            width: 100px;
        }

        .myRuleSelect .select-tit {
            padding: 5px 5px 7px 5px;
        }
    </style>
</head>

<body class="mainbody">
    <form id="form1" runat="server">
        <!--导航栏-->
        <div class="location">
            <a href="javascript:history.back(-1);" class="back"><i class="iconfont icon-up"></i><span>返回上一页</span></a>
            <a href="../center.aspx" class="home"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>员工未收款统计</span>
        </div>
        <!--/导航栏-->


        <div class="searchbar">
            <div class="menu-list" style="margin-bottom: 10px;">
                收付类别：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddltype" runat="server"></asp:DropDownList>
                        </div>                
                活动开始日期：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
                -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
                活动结束日期：
                        <asp:TextBox ID="txtsDate1" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate1\')}'})"></asp:TextBox>
                -
                        <asp:TextBox ID="txteDate1" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate1\')}'})"></asp:TextBox>
                订单状态：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlstatus" runat="server"></asp:DropDownList>
                                </div>
                

            </div>
        </div>
        <div class="searchbar">
            <div class="menu-list" style="margin-bottom: 10px;">
                未收付金额：
                        <div class="rule-single-select myRuleSelect">
                            <asp:DropDownList ID="ddlsign" runat="server" Width="50">
                                <asp:ListItem Value=">">></asp:ListItem>
                                <asp:ListItem Value=">=">>=</asp:ListItem>
                                <asp:ListItem Value="=">=</asp:ListItem>
                                <asp:ListItem Value="<>" Selected="True"><></asp:ListItem>
                                <asp:ListItem Value="<"><</asp:ListItem>
                                <asp:ListItem Value="<="><=</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                <asp:TextBox ID="txtMoney1" runat="server" CssClass="input small"></asp:TextBox>
                锁单状态：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddllock" runat="server"></asp:DropDownList>
                        </div>
                区域：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddlarea" runat="server"></asp:DropDownList>
                        </div>
                业务员：
                    <asp:TextBox ID="txtPerson1" runat="server" CssClass="input small" onkeyup="cToUpper(this)"></asp:TextBox>
                <input type="hidden" name="tag" value="<%=_tag %>" />
                <input type="hidden" name="self" value="<%=_self %>" />
                <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                <asp:LinkButton ID="btnExcel" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>
            </div>
        </div>
        <!--列表-->
        <div class="table-container">            
            <asp:Repeater ID="rptPersonList" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr style="text-align: left;">
                            <th width="15%">业务员</th>
                            <th width="8%">收付类别</th>
                            <th width="10%">应收付款</th>
                            <th width="10%">订单已收付款</th>
                            <th width="10%">未收付款</th>
                            <th>操作</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("op_number") %>(<%# Eval("op_name") %>)</td>
                        <td><%#Eval("fin_type").ToString()=="True"?"<font color='blue'>收</font>":"<font color='red'>付</font>"%></td>
                        <td><%# Eval("orderFinMoney") %></td>
                        <td><%# Eval("orderRpdMoney") %></td>
                        <td><%# Eval("orderUnMoney")%></td>
                        <td><a href="settleCustomerDetail.aspx?&ddltype=<%#Eval("fin_type")%>&txtsDate=<%=_sdate %>&txteDate=<%=_edate %>&txtsDate1=<%=_sdate1 %>&txteDate1=<%=_edate1 %>&ddlstatus=<%=_status %>&ddlsign=<%=_sign %>&txtMoney1=<%=_money1 %>&self=<%=_self %>&ddllock=<%=_lockstatus %>&ddlarea=<%=_area %>&txtPerson1=<%# Eval("op_number") %>">明细</a></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptPersonList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"6\">暂无记录</td></tr>" : ""%>
                </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div style="font-size: 12px;">
            <span style="display:block;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，总计应收付款：<asp:Label ID="pMoney1" runat="server">0</asp:Label>，订单已收付款：<asp:Label ID="pMoney2" runat="server">0</asp:Label>，未收付款：<asp:Label ID="pMoney3" runat="server">0</asp:Label>，已收付款：<asp:Label ID="pMoney4" runat="server">0</asp:Label>，已分配款：<asp:Label ID="pMoney5" runat="server">0</asp:Label>，未分配款：<asp:Label ID="pMoney6" runat="server">0</asp:Label></span>
            <span style="display:block;float: left;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计应收付款：<asp:Label ID="tMoney1" runat="server">0</asp:Label>，订单已收付款：<asp:Label ID="tMoney2" runat="server">0</asp:Label>，未收付款：<asp:Label ID="tMoney3" runat="server">0</asp:Label>，已收付款：<asp:Label ID="tMoney4" runat="server">0</asp:Label>，已分配款：<asp:Label ID="tMoney5" runat="server">0</asp:Label>，未分配款：<asp:Label ID="tMoney6" runat="server">0</asp:Label></span>
        </div>
        <!--/列表-->
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

    </form>
</body>
</html>
