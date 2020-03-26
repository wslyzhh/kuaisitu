<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="receiptdetail_list.aspx.cs" Inherits="MettingSys.Web.admin.finance.receiptdetail_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>收款明细列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript" charset="utf-8">

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
            $(".checkall input").change(function () {
                computeSelect();
            });
        })
        function computeSelect() {
            $("#sCount").text($(".checkall input:checked").size());
            var _smoney = 0;
            $(".checkall input:checked").each(function () {
                _smoney += parseFloat($(this).parent().parent().parent().children(".moneyTd").html());
            });
            $("#sMoney").text(_smoney.toFixed(2));
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
            <span>收款明细列表</span>
        </div>
        <!--/导航栏-->

        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            <div class="toolbar">
                <div class="box-wrap">
                    <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                    <div class="l-list">
                        <ul class="icon-list">
                            <li><a href="javascript:;" onclick="checkAll(this);computeSelect();"><i class="iconfont icon-check"></i><span>全选</span></a></li>                            
                            <li><a href="javascript:;" onclick="reverseCheckAll();computeSelect();"><i class="iconfont icon-check-mark"></i><span>反选</span></a></li>
                            <li>
                                <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete','删除后无法恢复，是否继续？');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <!--/工具栏-->
        <div class="searchbar">
            收款对象：
                            <asp:TextBox ID="txtCusName" runat="server" CssClass="input"></asp:TextBox>
            <asp:HiddenField ID="hCusId" runat="server" />
            订单号：
                            <asp:TextBox ID="txtorderid" runat="server" CssClass="input" Width="100" />
            预收日期：
                            <asp:TextBox ID="txtsforedate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            -
                            <asp:TextBox ID="txteforedate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtsforedate\')}'})" />
            收款方式：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlmethod" runat="server"></asp:DropDownList>
                            </div> 
            区域：
                <div class="rule-single-select">
                    <asp:DropDownList ID="ddlarea" runat="server"></asp:DropDownList>
                </div>
            <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                <asp:LinkButton ID="btnExcel" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>
        </div>
        <div class="searchbar">
            收款人：
                        <asp:TextBox ID="txtPerson1" runat="server" CssClass="input small"></asp:TextBox> 
            实收日期：
                        <asp:TextBox ID="txtsdate" runat="server" CssClass="input rule-date-input" Width="100" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            -
                        <asp:TextBox ID="txtedate" runat="server" CssClass="input rule-date-input" Width="100" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtsdate\')}'})" />
        </div>

        <!--列表-->
        <div class="table-container">
            <asp:Repeater ID="rptList" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr>
                            <th width="6%">选择</th>
                            <th align="left" width="8%">订单号</th>
                            <th align="left" width="10%">收款对象</th>
                            <th align="left">收款内容</th>
                            <th align="left" width="6%">收款金额</th>
                            <th align="left" width="8%">预收日期</th>
                            <th align="left" width="8%">收款方式</th>
                            <th align="left" width="8%">申请人</th>
                            <th align="left" width="4%">状态</th>                            
                            <th align="left" width="5%">收款人</th>
                            <th align="left" width="6%">实收日期</th>
                            <th width="8%">操作</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td align="center">
                            <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                            <asp:HiddenField ID="hidId" Value='<%#Eval("rpd_id")%>' runat="server" />
                        </td>
                        <td><a href="../order/order_edit.aspx?action=<%# DTEnums.ActionEnum.Edit.ToString() %>&oID=<%#Eval("rpd_oid")%>"><%#Eval("rpd_oid")%></a></td>
                        <td><%#Eval("c_name")%></td>
                        <td><%#Eval("rpd_content")%></td>
                        <td class="moneyTd"><%#Eval("rpd_money")%></td>
                        <td><%# Convert.ToDateTime(Eval("rpd_foredate")).ToString("yyyy-MM-dd") %></td>
                        <td><%#Eval("pm_name")%></td>
                        <td><%# Eval("rpd_personNum") %>-<%# Eval("rpd_personName") %></td>
                        <td><span onmouseover="tip_index=layer.tips('审批人：<%#Eval("rpd_checkNum1")%>-<%#Eval("rpd_checkName1")%><br/>审批备注：<%#Eval("rpd_checkRemark1").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("rpd_flag1")%>"></span></td>
                        <td><%#Eval("rp_confirmerName")%></td>
                        <td><%# ConvertHelper.toDate(Eval("rp_date"))==null?"":Convert.ToDateTime(Eval("rp_date")).ToString("yyyy-MM-dd") %></td>
                        <td align="center"><a href="receiptdetail_edit.aspx?action=<%#DTEnums.ActionEnum.Edit %>&id=<%#Eval("rpd_id")%>">修改</a></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"12\">暂无记录</td></tr>" : ""%>
  </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div style="font-size: 12px;color:darkblue;">
            <span style="float: left;">选中：<asp:Label ID="sCount" runat="server">0</asp:Label>条记录，收款金额：<asp:Label ID="sMoney" runat="server">0</asp:Label></span>
        </div>
        <div style="font-size: 12px;margin-top: 22px;">
            <span style="float: left;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，总计收款金额：<asp:Label ID="pMoney" runat="server">0</asp:Label></span>
            <span style="float: right;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计收款金额：<asp:Label ID="tMoney" runat="server">0</asp:Label></span>
        </div>
        <!--/列表-->
        <div class="dRemark" style="margin-top:45px;">
            <p>1.审批：<span class="check_0"></span>待审批，<span class="check_1"></span>审批未通过，<span class="check_2"></span>审批通过</p>
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
