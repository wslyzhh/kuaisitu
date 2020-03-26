<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FinancialClosing.aspx.cs" Inherits="MettingSys.Web.admin.finance.FinancialClosing" %>

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
    <script type="text/javascript">
        //执行回传函数
        function NewExePostBack(objmsg) {
            var tag = false;
            layer.confirm("确定要进行" + objmsg + "操作吗，是否继续？", {
                btn: ['确定', '取消'] //按钮
            }, function (index) {
                layer.close(index);
                return true;
            }, function () {
                return false;
            }
            );
        }
    </script>
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
            <span>财务结账</span>
        </div>
        <!--/导航栏-->
        <div class="content-tab-wrap">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a <%=flag=="0"?"class=\"selected\"":"" %> href="FinancialClosing.aspx?flag=0">结账</a></li>
                        <li><a <%=flag=="1"?"class=\"selected\"":"" %> href="FinancialClosing.aspx?flag=1">反结账</a></li>
                        <li><a href="FinancialCustomerDetail.aspx">已结账应收付明细账</a></li>
                        <li><a href="FinancialOrderDetail.aspx">应收付对象已结账订单地接明细</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="tab-content">
            <div class="table-container" id="div0" runat="server">
                <asp:Label ID="labtitle" runat="server">当前最近未结账月份：</asp:Label><asp:Label ID="labUnFinMonth" runat="server" Style="font-size: 16px; font-weight: bolder; margin-right: 20px;"></asp:Label>
                <asp:TextBox ID="txtDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM'})"></asp:TextBox>
                <asp:Button ID="btnUnFinancial" runat="server" Text="结账" CssClass="btn" OnClick="btnUnFinancial_Click" />
                <div id="floatHead" class="toolbar-wrap">
                    <div class="toolbar searchbar">
                        <div class="menu-list" style="margin-bottom: 10px;">
                            已结账月份：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM',maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
                            -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM',minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
                            区域：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlarea" runat="server"></asp:DropDownList>
                                </div>
                            <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />

                        </div>
                    </div>
                </div>
                <!--/工具栏-->
                <!--列表-->
                <div class="table-container">
                    <asp:Repeater ID="rptList" runat="server">
                        <HeaderTemplate>
                            <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                                <tr style="text-align: left;">
                                    <th width="10%">收付类别</th>
                                    <th width="10%">区域</th>
                                    <th width="10%">应收付金额</th>
                                    <th>操作</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%#Eval("fin_type").ToString()=="True"?"<font color='blue'>收</font>":"<font color='red'>付</font>"%></td>
                                <td><%# Eval("fin_area") %></td>
                                <td><%# Eval("fin_money") %></td>
                                <td><a href="FinancialCustomerDetail.aspx?tag=0&txtsDate=<%=_smonth %>&txteDate=<%=_emonth %>&ddltype=<%#Eval("fin_type") %>&ddlarea=<%# Eval("fin_area") %>">明细</a></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"4\">暂无记录</td></tr>" : ""%>
  </table>
                        </FooterTemplate>
                    </asp:Repeater>

                    <div style="font-size: 12px;margin-top:10px;">
                        <span style="float: left;">收合计总额：<asp:Label ID="money1" runat="server">0</asp:Label>，付合计总额：<asp:Label ID="money0" runat="server">0</asp:Label></span>
                    </div>
                </div>
            </div>
            <div class="table-container" id="div1" runat="server">
                当前最近已结账月份：<asp:Label ID="labFinMonth" runat="server" Style="font-size: 16px; font-weight: bolder; margin-right: 20px;"></asp:Label><asp:Button ID="btnFinancial" runat="server" Text="反结账" CssClass="btn" OnClick="btnFinancial_Click" />
            </div>
        </div>
        <div class="tab-content" style="display: none;">
        </div>
    </form>
</body>
</html>
