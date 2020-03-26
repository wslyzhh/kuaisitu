<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="orderApproval_list.aspx.cs" Inherits="MettingSys.Web.admin.finance.orderApproval_list" %>

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
        <!--/导航栏-->
        <div class="content-tab-wrap">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a <%=flag=="0"?"class=\"selected\"":"" %> href="orderApproval_list.aspx?flag=0">待审订单列表<sup class="sup"><asp:Label ID="labUnCheckCount" runat="server">0</asp:Label></sup></a></li>
                        <li><a <%=flag=="1"?"class=\"selected\"":"" %> href="orderApproval_list.aspx?flag=1">已审待锁订单列表<sup class="sup"><asp:Label ID="labUnLockCount" runat="server">0</asp:Label></sup></a></li>
                        <li><a <%=flag=="4"?"class=\"selected\"":"" %> href="orderApproval_list.aspx?flag=4">待处理订单列表<sup class="sup"><asp:Label ID="labUnDealCount" runat="server">0</asp:Label></sup></a></li>
                        <li><a <%=flag=="2"?"class=\"selected\"":"" %> href="orderApproval_list.aspx?flag=2">已锁订单列表</a></li>
                        <li><a <%=flag=="3"?"class=\"selected\"":"" %> href="orderApproval_list.aspx?flag=3">全部订单列表</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="tab-content" style="padding-top: 0;">
            <div class="searchbar">
                <div class="menu-list" style="margin-bottom: 10px;">
                    订单号：
                        <asp:TextBox ID="txtOrderID" runat="server" CssClass="input" Width="100"></asp:TextBox>
                    客户：
                                <asp:TextBox ID="txtCusName" runat="server" CssClass="input" Width="150"></asp:TextBox>
                    <asp:HiddenField ID="hCusId" runat="server" />
                    合同造价：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlContractPrice" runat="server"></asp:DropDownList>
                                </div>
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
                    <input type="hidden" name="flag" value="<%=flag %>" />
                </div>
                <div class="menu-list">
                    订单状态：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlstatus" runat="server"></asp:DropDownList>
                                </div>
                    接单状态：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddldstatus" runat="server"></asp:DropDownList>
                                </div>
                    是否推送：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlispush" runat="server"></asp:DropDownList>
                                </div>
                    审批状态：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlflag" runat="server"></asp:DropDownList>
                                </div>
                    锁单状态：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddllock" runat="server"></asp:DropDownList>
                                </div>
                    归属地：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlarea" runat="server"></asp:DropDownList>
                            </div>
                    下单区域：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlorderarea" runat="server"></asp:DropDownList>
                            </div>
                </div>
            </div>

            <!--列表-->
            <div class="table-container">
                <asp:Repeater ID="rptList" runat="server">
                    <HeaderTemplate>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                            <tr style="text-align: left;">
                                <th width="6%">订单号</th>
                                <th width="8%">客户</th>
                                <th width="8%">活动名称</th>
                                <th width="6%">合同造价</th>
                                <th width="10%">活动日期</th>
                                <th width="6%">归属地</th>
                                <th width="6%">订单状态</th>
                                <th width="6%">是否推送</th>
                                <th width="6%">上级审批</th>
                                <th width="6%">锁单状态</th>
                                <th width="8%">业务员</th>
                                <th width="10%">待审核应收付</th>
                                <th width="10%">审核未通过应收付</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><a href="../order/order_edit.aspx?action=<%# DTEnums.ActionEnum.Edit.ToString() %>&oID=<%#Eval("o_id")%>"><%#Eval("o_id")%></a></td>
                            <td><%#Eval("c_name")%></td>
                            <td><%#Eval("o_content")%></td>
                            <td><%#Eval("o_contractPrice")%></td>
                            <td><%#ConvertHelper.toDate(Eval("o_sdate")).Value.ToString("yyyy-MM-dd")%>/<%#ConvertHelper.toDate(Eval("o_edate")).Value.ToString("yyyy-MM-dd")%></td>
                            <td><%# new MettingSys.BLL.department().getAreaText(Eval("o_place").ToString())%></td>
                            <td><%#MettingSys.Common.BusinessDict.fStatus()[Convert.ToByte(Eval("o_status"))]%></td>
                            <td><%#MettingSys.Common.BusinessDict.pushStatus()[Convert.ToBoolean(Eval("o_isPush"))]%></td>
                            <td><%#MettingSys.Common.BusinessDict.checkStatus()[Convert.ToByte(Eval("o_flag"))]%></td>
                            <td><%#MettingSys.Common.BusinessDict.lockStatus()[Utils.ObjToByte(Eval("o_lockStatus"))]%></td>
                            <td><span title="<%#Eval("o_addDate")%>"><%#Eval("op_name")%>-<%#Eval("op_number")%></span></td>
                            <td><%#Eval("count0")%></td>
                            <td><%#Eval("count1")%></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"13\">暂无记录</td></tr>" : ""%>
  </table>
                    </FooterTemplate>
                </asp:Repeater>
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

