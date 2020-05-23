<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReconciliationSearch.aspx.cs" Inherits="MettingSys.Web.admin.finance.ReconciliationSearch" %>

<%@ Import Namespace="MettingSys.Common" %>
<%@ Import Namespace="MettingSys.BLL" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>审单</title>
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

        .sup {
            vertical-align: super;
            color: red;
            font-size: 12px;
            font-family: Arial, Helvetica, sans-serif;
            margin: 10px 0px 0px 0px;
            margin-left: 5px;
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
            <span>审单</span>
        </div>

        <div class="searchbar">
            <div class="menu-list" style="margin-bottom: 10px;">
                订单号：
                        <asp:TextBox ID="txtOrderID" runat="server" CssClass="input" Width="100"></asp:TextBox>
                应收付对象：
                        <asp:TextBox ID="txtCusName" runat="server" CssClass="input" Width="150"></asp:TextBox>
                <asp:HiddenField ID="hCusId" runat="server" />
                收付类别：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddltype" runat="server"></asp:DropDownList>
                        </div>
                对账标识：
                        <asp:TextBox ID="txtNum" runat="server" CssClass="input" Width="100"></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                <asp:LinkButton ID="btnExcel" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>
            </div>
        </div>

        <!--列表-->
        <div class="table-container">
            <asp:Repeater ID="rptList" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr style="text-align: left;">
                            <th width="8%">订单号</th>
                            <th width="12%">应收付对象</th>
                            <th width="12%">客户</th>
                            <th width="10%">活动日期</th>
                            <th width="8%">活动地点</th>
                            <th style="padding-left:8px;">活动名称</th>
                            <%--<th width="12%">业务日期</th>--%>
                            <th width="6%">收付性质</th>
                            <th width="6%">业务性质</th>
                            <th width="10%">业务明细</th>
                            <th width="10%">对账标识</th>
                            <th width="6%">对账金额</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><a href="../order/order_edit.aspx?action=<%# DTEnums.ActionEnum.Edit.ToString() %>&oID=<%#Eval("o_id")%>"><span class="orderstatus_<%#Eval("o_status")%>"><%#Eval("o_ID") %></span></a></td>
                        <td><%#Eval("c_name") %></td>
                        <td><%#Eval("cname") %></td>
                        <td><%#ConvertHelper.toDate(Eval("o_sdate")).Value.ToString("yyyy-MM-dd")%>/<%#ConvertHelper.toDate(Eval("o_edate")).Value.ToString("yyyy-MM-dd")%></td>
                        <td style="padding-left:8px;"><%#Eval("o_address") %></td>
                        <td style="padding-left:8px;"><%#Eval("o_content") %></td>
                        <%--<td><%#ConvertHelper.toDate(Eval("fin_sdate")).Value.ToString("yyyy-MM-dd")%>/<%#ConvertHelper.toDate(Eval("fin_edate")).Value.ToString("yyyy-MM-dd")%></td>--%>
                        <td><%#Eval("fin_type").ToString()=="True"?"<font color='blue'>应收</font>":"<font color='red'>应付</font>"%></td>
                        <td><%#Eval("na_name") %></td>
                        <td><%#Eval("fin_detail") %></td>
                        <td><%#Eval("fc_num") %></td>
                        <td><%#Eval("fc_money") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"9\">暂无记录</td></tr>" : ""%>
                </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div style="font-size: 12px;">
            <span style="float: left;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，总计对账金额：<asp:Label ID="pMoney" runat="server">0</asp:Label></span>
            <span style="float: right;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计对账金额：<asp:Label ID="tMoney" runat="server">0</asp:Label></span>
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
