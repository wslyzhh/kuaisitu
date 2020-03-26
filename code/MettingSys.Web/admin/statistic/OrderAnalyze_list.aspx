<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderAnalyze_list.aspx.cs" Inherits="MettingSys.Web.admin.statistic.OrderAnalyze_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<%@ Import Namespace="MettingSys.BLL" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>业务性质列表</title>
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
            <span>订单分析</span>
        </div>
        <!--/导航栏-->
        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            <div class="toolbar">
                <div class="box-wrap">
                    <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                    <div class="l-list">
                        <ul class="icon-list">
                            <li><input name="btnReturn" type="button" value="返回上一页" class="btn yellow" style="margin-left:10px;" onclick="javascript: history.back(-1);" /></li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <!--/工具栏-->
        <div class="tab-content" style="padding-top: 0;">
            <div class="searchbar">
                订 单 号 ：
                        <asp:TextBox ID="txtOrderID" runat="server" CssClass="input"></asp:TextBox>
                客&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;户：
                                <asp:TextBox ID="txtCusName" runat="server" CssClass="input"></asp:TextBox>
                <asp:HiddenField ID="hCusId" runat="server" />
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
                业务员：
                    <asp:TextBox ID="txtPerson1" runat="server" CssClass="input small" onkeyup="cToUpper(this)"></asp:TextBox>                
                策划人员：
                    <asp:TextBox ID="txtPerson3" runat="server" CssClass="input small" onkeyup="cToUpper(this)"></asp:TextBox>
                设计人员：
                    <asp:TextBox ID="txtPerson5" runat="server" CssClass="input small" onkeyup="cToUpper(this)"></asp:TextBox>                
                归属地：
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlarea" runat="server"></asp:DropDownList>
                    </div>
                下单区域：
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlorderarea" runat="server"></asp:DropDownList>
                    </div>
            </div>
            <div class="searchbar">
                活动名称：
                        <asp:TextBox ID="txtContent" runat="server" CssClass="input"></asp:TextBox>
                活动地点：
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="input"></asp:TextBox>
                应收款：
                        <div class="rule-single-select myRuleSelect">
                            <asp:DropDownList ID="ddlsign1" runat="server" Width="50">
                                <asp:ListItem Value=">">></asp:ListItem>
                                <asp:ListItem Value=">=">>=</asp:ListItem>
                                <asp:ListItem Value="=">=</asp:ListItem>
                                <asp:ListItem Value="<>"><></asp:ListItem>
                                <asp:ListItem Value="<"><</asp:ListItem>
                                <asp:ListItem Value="<="><=</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                <asp:TextBox ID="txtMoney1" runat="server" CssClass="input small"></asp:TextBox>
                未收款：
                        <div class="rule-single-select myRuleSelect">
                            <asp:DropDownList ID="ddlsign" runat="server" Width="50">
                                <asp:ListItem Value=">">></asp:ListItem>
                                <asp:ListItem Value=">=">>=</asp:ListItem>
                                <asp:ListItem Value="=">=</asp:ListItem>
                                <asp:ListItem Value="<>"><></asp:ListItem>
                                <asp:ListItem Value="<"><</asp:ListItem>
                                <asp:ListItem Value="<="><=</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                <asp:TextBox ID="txtMoney" runat="server" CssClass="input small"></asp:TextBox>
                应付款：
                        <div class="rule-single-select myRuleSelect">
                            <asp:DropDownList ID="ddlsign2" runat="server" Width="50">
                                <asp:ListItem Value=">">></asp:ListItem>
                                <asp:ListItem Value=">=">>=</asp:ListItem>
                                <asp:ListItem Value="=">=</asp:ListItem>
                                <asp:ListItem Value="<>"><></asp:ListItem>
                                <asp:ListItem Value="<"><</asp:ListItem>
                                <asp:ListItem Value="<="><=</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                <asp:TextBox ID="txtMoney2" runat="server" CssClass="input small"></asp:TextBox>
                未付款：
                        <div class="rule-single-select myRuleSelect">
                            <asp:DropDownList ID="ddlsign3" runat="server" Width="50">
                                <asp:ListItem Value=">">></asp:ListItem>
                                <asp:ListItem Value=">=">>=</asp:ListItem>
                                <asp:ListItem Value="=">=</asp:ListItem>
                                <asp:ListItem Value="<>"><></asp:ListItem>
                                <asp:ListItem Value="<"><</asp:ListItem>
                                <asp:ListItem Value="<="><=</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                <asp:TextBox ID="txtMoney3" runat="server" CssClass="input small"></asp:TextBox>
                税费：
                        <div class="rule-single-select myRuleSelect">
                            <asp:DropDownList ID="ddlsign4" runat="server" Width="50">
                                <asp:ListItem Value=">">></asp:ListItem>
                                <asp:ListItem Value=">=">>=</asp:ListItem>
                                <asp:ListItem Value="=">=</asp:ListItem>
                                <asp:ListItem Value="<>"><></asp:ListItem>
                                <asp:ListItem Value="<"><</asp:ListItem>
                                <asp:ListItem Value="<="><=</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                <asp:TextBox ID="txtMoney4" runat="server" CssClass="input small"></asp:TextBox>
                业绩利润：
                        <div class="rule-single-select myRuleSelect">
                            <asp:DropDownList ID="ddlsign5" runat="server" Width="50">
                                <asp:ListItem Value=">">></asp:ListItem>
                                <asp:ListItem Value=">=">>=</asp:ListItem>
                                <asp:ListItem Value="=">=</asp:ListItem>
                                <asp:ListItem Value="<>"><></asp:ListItem>
                                <asp:ListItem Value="<"><</asp:ListItem>
                                <asp:ListItem Value="<="><=</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                <asp:TextBox ID="txtMoney5" runat="server" CssClass="input small"></asp:TextBox>
            </div>
            <div class="searchbar">
                活动开始日期：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
                -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
                活动结束日期：
                        <asp:TextBox ID="txtsDate1" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate1\')}'})"></asp:TextBox>
                -
                        <asp:TextBox ID="txteDate1" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate1\')}'})"></asp:TextBox>

               <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
            <asp:LinkButton ID="btnExcel" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>
            </div>

            <!--列表-->
            <div class="table-container">
                <asp:Repeater ID="rptList" runat="server">
                    <HeaderTemplate>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                            <tr>
                                <th align="left" width="6%">订单号</th>
                                <th align="left">活动名称/地点</th>
                                <th align="left" width="8%">客户</th>
                                <th align="left" width="6%">活动日期</th>
                                <th align="left" width="6%">归属地</th>
                                <th align="left" width="6%">订单状态</th>
                                <th align="left" width="6%">锁单状态</th>
                                <th align="left" width="8%">业务员</th>
                                <th align="left" width="12%">策划人员</th>
                                <th align="left" width="12%">设计人员</th>
                                <th align="left" width="6%">收款</th>
                                <th align="left" width="6%">付款</th>
                                <th align="left" width="4%">税费</th>
                                <th align="left" width="4%">业绩利润</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><a href="../order/order_edit.aspx?action=<%# DTEnums.ActionEnum.Edit.ToString() %>&oID=<%#Eval("o_id")%>"><%#Eval("o_id")%></a></td>
                            <td>名称：<%#Eval("o_content")%><br />
                                地点：<%#Eval("o_address")%>
                            </td>
                            <td><%#Eval("c_name")%></td>
                            <td><%#ConvertHelper.toDate(Eval("o_sdate")).Value.ToString("yyyy-MM-dd")%><br /><%#ConvertHelper.toDate(Eval("o_edate")).Value.ToString("yyyy-MM-dd")%></td>
                            <td><%# new MettingSys.BLL.department().getAreaText(Eval("o_place").ToString())%></td>
                            <td><span onmouseover="tip_index=layer.tips('推送状态：<%#MettingSys.Common.BusinessDict.pushStatus()[Convert.ToBoolean(Eval("o_isPush"))]%><br/>上级审批：<%#MettingSys.Common.BusinessDict.checkStatus()[Convert.ToByte(Eval("o_flag"))]%>', this, { time: 0 });" onmouseout="layer.close(tip_index);"><%#MettingSys.Common.BusinessDict.fStatus(2)[Convert.ToByte(Eval("o_status"))]%></span></td>
                            <td><%#MettingSys.Common.BusinessDict.lockStatus()[Utils.ObjToByte(Eval("o_lockStatus"))]%></td>
                            <td><span title="工号：<%#Eval("op_number")%>，下单时间:<%#Eval("o_addDate")%>"><%#Eval("op_name")%></span></td>
                            <td><%#Eval("person3").ToString().Replace("待定","<font color='red'>待定</font>").Replace("处理中","<font color='blue'>处理中</font>").Replace("已完成","<font color='green'>已完成</font>")%></td>
                            <td><%#Eval("person4").ToString().Replace("待定","<font color='red'>待定</font>").Replace("处理中","<font color='blue'>处理中</font>").Replace("已完成","<font color='green'>已完成</font>")%></td>
                            <td>应收:<%#Utils.ObjToDecimal(Eval("shou"),0) %><br />未收:<%# Utils.ObjToDecimal(Eval("weishou"),0) %></td>
                            <td>应付:<%#Utils.ObjToDecimal(Eval("fu"),0) %><br />未付:<%#Utils.ObjToDecimal(Eval("weifu"),0) %></td>
                            <td><%# Utils.ObjToDecimal(Eval("o_financeCust"),0) %></td>
                            <td><%# Eval("profit") %></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"14\">暂无记录</td></tr>" : ""%>
                    </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        <div style="font-size: 12px;">
            <span style="display:block;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，总计应收款：<asp:Label ID="pMoney1" runat="server">0</asp:Label>，未收款：<asp:Label ID="pMoney2" runat="server">0</asp:Label>，应付款：<asp:Label ID="pMoney3" runat="server">0</asp:Label>，未付款：<asp:Label ID="pMoney4" runat="server">0</asp:Label>，税费：<asp:Label ID="pMoney5" runat="server">0</asp:Label>，业绩利润：<asp:Label ID="pMoney6" runat="server">0</asp:Label></span>
            <span style="display:block;float: left;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计应收款：<asp:Label ID="tMoney1" runat="server">0</asp:Label>，未收款：<asp:Label ID="tMoney2" runat="server">0</asp:Label>，应付款：<asp:Label ID="tMoney3" runat="server">0</asp:Label>，未付款：<asp:Label ID="tMoney4" runat="server">0</asp:Label>，税费：<asp:Label ID="tMoney5" runat="server">0</asp:Label>，业绩利润：<asp:Label ID="tMoney6" runat="server">0</asp:Label></span>
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
        </div>
    </form>
</body>
</html>
