<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FinancialCustomerDetail.aspx.cs" Inherits="MettingSys.Web.admin.finance.FinancialCustomerDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
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
        <div class="location">
            <a href="javascript:history.back(-1);" class="back"><i class="iconfont icon-up"></i><span>返回上一页</span></a>
            <a href="../center.aspx" class="home"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>已结账应收付明细账</span>
        </div>
        <!--/导航栏-->
        <div class="content-tab-wrap">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a href="FinancialClosing.aspx?flag=0">结账</a></li>
                        <li><a href="FinancialClosing.aspx?flag=1">反结账</a></li>
                        <li><a class="selected" href="FinancialCustomerDetail.aspx">已结账应收付明细账</a></li>
                        <li><a href="FinancialOrderDetail.aspx">应收付对象已结账订单地接明细</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="tab-content">
            <div class="searchbar">
                <div class="menu-list" style="margin-bottom: 10px;">
                    已结账月份：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM',maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
                    -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM',minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
                    收付类别：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddltype" runat="server"></asp:DropDownList>
                        </div>
                    
                    区域：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlarea" runat="server"></asp:DropDownList>
                                </div>
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
                                <th width="10%">应收付客户</th>
                                <th width="10%">收付类别</th>
                                <th width="10%">区域</th>
                                <th width="10%">开始月份</th>
                                <th width="10%">结束月份</th>
                                <th width="10%">应收付金额</th>
                                <th>操作</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("c_name") %></td>
                            <td><%#Eval("fin_type").ToString()=="True"?"<font color='blue'>收</font>":"<font color='red'>付</font>"%></td>
                            <td><%# Eval("fin_area") %></td>
                            <td><%=_smonth %></td>
                            <td><%=_emonth %></td>
                            <td><%# Eval("fin_money") %></td>
                            <td><a href="FinancialOrderDetail.aspx?tag=0&txtsDate=<%=_smonth %>&txteDate=<%=_emonth %>&ddltype=<%#Eval("fin_type") %>&txtCusName=<%# Eval("c_name") %>&hCusId=<%# Eval("fin_cid") %>&ddlarea=<%# Eval("fin_area") %>">明细</a></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"7\">暂无记录</td></tr>" : ""%>
                         </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div style="font-size: 12px;">
                <span style="float: left;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，合计应收金额：<asp:Label ID="pMoney1" runat="server">0</asp:Label>，应付金额：<asp:Label ID="pMoney2" runat="server">0</asp:Label></span>
                <span style="float: right;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计应收金额：<asp:Label ID="tMoney1" runat="server">0</asp:Label>，应付金额：<asp:Label ID="tMoney2" runat="server">0</asp:Label></span>
            </div>
            <div class="line20"></div>
            <div class="pagelist">
                <div class="l-btns">
                    <span>显示</span><asp:TextBox ID="txtPageNum" runat="server" CssClass="pagenum" onkeydown="return checkNumber(event);"
                        OnTextChanged="txtPageNum_TextChanged" AutoPostBack="True"></asp:TextBox><span>条/页</span>
                </div>
                <div id="PageContent" runat="server" class="default"></div>
                <input id="btnReturn" type="button" value="返回上一页" class="btn yellow" style="margin-left:5px;" onclick="javascript: history.back(-1);" />
            </div>
        </div>
        <div class="tab-content" style="display: none;">
        </div>
    </form>
</body>
</html>
