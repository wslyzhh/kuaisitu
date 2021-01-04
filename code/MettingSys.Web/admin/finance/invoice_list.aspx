<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="invoice_list.aspx.cs" Inherits="MettingSys.Web.admin.finance.invoice_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>发票列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
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

            $("#ddlisConfirm1").change(function () {
                if ($(this).val() != "") {
                    if ($(this).val() == "True") {
                        $("#spanDate").show();
                    }
                    else {
                        $("#spanDate").hide();
                    }
                }
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
        function toggleCheckDiv() {
            $("#confirmDiv").hide();
            $("#checkDiv").toggle();
        }
        function toggleconfirmDiv() {
            $("#checkDiv").hide();
            $("#confirmDiv").toggle();
            $("#spanDate").show();
        }
        /*批量审批 */
        function submitCheck() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            if ($("#ddlchecktype").val() == "") {
                jsprint("请选择审批类型");
                return;
            }
            if ($("#ddlcheck").val() == "") {
                jsprint("请选择审批状态");
                return;
            }
            $(".checkall input:checked").each(function () {
                var id = $(this).parent().next().val();
                var postData = { "id": id, "ctype": $("#ddlchecktype").val(), "cstatus": $("#ddlcheck").val(), "remark": $("#txtremark").val() };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=checkInvoice",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $("#tr" + id).children().last().children().last().html("");
                        if (data.status == 0) {                            
                            $("#tr" + id).find(".checkTd").children().eq($("#ddlchecktype").val() - 1).removeClass().addClass("check_"+$("#ddlcheck").val());
                        } else {
                            $("#tr" + id).children().last().children().last().append(data.msg);
                        }
                    }
                });
            });

            
        }
        /*批量审批 */
        /*开票 */
        function submitConfirm() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            if ($("#ddlisConfirm1").val() == "") {
                jsprint("请选择开票状态");
                return;
            }
            if ($("#ddlisConfirm1").val() == "True" && $("#txtdate").val() == "") {
                jsprint("请选择开票日期");
                return;
            }
            $(".checkall input:checked").each(function () {
                var id = $(this).parent().next().val();
                var postData = { "id": id, "status": $("#ddlisConfirm1").val(), "date": $("#txtdate").val() };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=confirmInvoice",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $("#tr" + id).children().last().children().last().html("");
                        if (data.status == 0) {                            
                            if ($("#ddlisConfirm1").val() == "True") {
                                $("#tr" + id).find(".confirmTd").children().eq(0).removeClass().addClass("check_2");
                                $("#tr" + id).find(".dateTd").text($("#txtdate").val());
                            }
                            else {
                                $("#tr" + id).find(".confirmTd").children().eq(0).removeClass().addClass("check_0"); 
                                $("#tr" + id).find(".dateTd").html("");
                            }
                        } else {
                            $("#tr" + id).children().last().children().last().append(data.msg);
                        }
                    }
                });
            });            
        }
        /*开票 */
    </script>
    <style type="text/css">
        .date-input {
            width: 120px;
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
            <span>发票列表</span>
        </div>
        <!--/导航栏-->
        <div class="content-tab-wrap" id="titleDiv" runat="server">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a <%=_check=="0"?"class=\"selected\"":"" %> href="invoice_list.aspx?check=0">全部列表</a></li>
                        <li><a <%=_check=="1"?"class=\"selected\"":"" %> href="invoice_list.aspx?check=1">申请区域未审批</a></li>
                        <li><a <%=_check=="2"?"class=\"selected\"":"" %> href="invoice_list.aspx?check=2">开票区域未审批</a></li>
                        <li><a <%=_check=="3"?"class=\"selected\"":"" %> href="invoice_list.aspx?check=3">财务未审批</a></li>
                        <li><a <%=_check=="4"?"class=\"selected\"":"" %> href="invoice_list.aspx?check=4">已审批未开票</a></li>
                        <li><a <%=_check=="5"?"class=\"selected\"":"" %> href="invoice_list.aspx?check=5">已开票</a></li>
                    </ul>
                </div>
            </div>
        </div>
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
                            <li id="liCheck" runat="server"><a href="javascript:;" onclick="toggleCheckDiv()"><span>审批</span></a></li>
                            <li id="liConfirm" runat="server"><a href="javascript:;" onclick="toggleconfirmDiv()"><span>开票</span></a></li>
                        </ul>
                        <div id="checkDiv" style="display: none;">
                            审批类型：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlchecktype" runat="server">
                                    <asp:ListItem Value="">请选择</asp:ListItem>
                                    <asp:ListItem Value="1">申请区域审批</asp:ListItem>
                                    <asp:ListItem Value="2">开票区域审批</asp:ListItem>
                                    <asp:ListItem Value="3">财务审批</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            审批状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck" runat="server"></asp:DropDownList>
                            </div>
                            备注：
                            <asp:TextBox ID="txtremark" runat="server" CssClass="input normal"></asp:TextBox>
                            <input type="button" class="btn" value="提交" onclick="submitCheck()" />
                        </div>
                        <div id="confirmDiv" style="display: none;">
                            开票状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlisConfirm1" runat="server">
                                </asp:DropDownList>
                            </div>
                            <span id="spanDate" style="display: none;">开票日期：
                            <asp:TextBox ID="txtdate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                            </span>
                            <input type="button" class="btn" value="提交" onclick="submitConfirm()" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--/工具栏-->
        <div class="searchbar">
            订单号：
                            <asp:TextBox ID="txtOid" runat="server" CssClass="input" />
            客户：
                            <asp:TextBox ID="txtCusName" runat="server" CssClass="input"></asp:TextBox>
            <asp:HiddenField ID="hCusId" runat="server" />
            申请区域：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlfarea" runat="server"></asp:DropDownList>
                                </div>
            开票区域：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddldarea" runat="server"></asp:DropDownList>
                                </div>
            开票单位：
                            <asp:TextBox ID="txtUnit" runat="server" CssClass="input"  />
            申请区域审批：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck1" runat="server"></asp:DropDownList>
                            </div>
            开票区域审批：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck2" runat="server"></asp:DropDownList>
                            </div>
            
            
            <input type="hidden" name="self" value="<%=_self %>" />
            <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                <asp:LinkButton ID="btnExcel" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>
        </div>
        <div class="searchbar">
            财务审批：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck3" runat="server"></asp:DropDownList>
                            </div>
            开票状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlisConfirm" runat="server"></asp:DropDownList>
                            </div>
            开票金额：
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
            开票日期：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="120px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
            -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="120px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
            发票类型
            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlinvType" runat="server"></asp:DropDownList>
                            </div>
            申请人：
                            <asp:TextBox ID="txtName" runat="server" CssClass="input" onkeyup="cToUpper(this)" />
        </div>

        <!--列表-->
        <div class="table-container">
            <asp:Repeater ID="rptList" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr>
                            <th width="6%">选择</th>
                            <th align="left" width="6%">序号</th>
                            <th align="left" width="8%">客户</th>
                            <th align="left" width="8%">订单号</th>
                            <th align="left">开票项目</th>
                            <th align="left" width="6%">发票类型</th>
                            <th align="left" width="6%">开票金额</th>
                            <th align="left" width="6%">申请时超开</th>
                            <th align="left" width="6%">送票方式</th>
                            <th align="left" width="6%">开票区域</th>
                            <th align="left" width="6%">开票单位</th>
                            <th align="left" width="6%">申请人</th>
                            <th align="left" width="6%">审批</th>
                            <th align="left" width="6%">开票状态</th>
                            <th align="left" width="6%">开票日期</th>
                            <th width="5%">操作</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr id="tr<%#Eval("inv_id")%>">
                        <td align="center">
                            <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                            <asp:HiddenField ID="hidId" Value='<%#Eval("inv_id")%>' runat="server" />
                        </td>
                        <td><%#Eval("inv_id")%></td>
                        <td><%#Eval("c_name")%></td>
                        <td><a href="../order/order_edit.aspx?action=<%# DTEnums.ActionEnum.Edit.ToString() %>&oID=<%#Eval("inv_oid")%>"><span class="orderstatus_<%#Eval("o_status")%>"><%#Eval("inv_oid")%></span></a></td>
                        <td><%#Eval("inv_serviceType")%>/<%#Eval("inv_serviceName")%></td>
                        <td><%#Eval("inv_type")%></td>
                        <td class="moneyTd"><%#Eval("inv_money")%></td>
                        <td><%#Utils.StrToDecimal(Eval("inv_overmoney").ToString(),0)==0?"0":"<font color='red'>"+Eval("inv_overmoney")+"</font>"%></td>
                        <td><%#Eval("inv_sentWay")%></td>
                        <td><%#Eval("de_subname")%></td>
                        <td><%#Eval("invU_name")%></td>
                        <td title="申请工号：<%#Eval("inv_personNum")%>&#10;申请时间：<%#Eval("inv_addDate")%>"><%#Eval("inv_personName")%></td>
                        <td class="checkTd">
                            <span onmouseover="tip_index=layer.tips('申请区域审批<br/>审批人：<%#Eval("inv_checkNum1")%>-<%#Eval("inv_checkName1")%><br/>审批备注：<%#Eval("inv_checkRemark1").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%><br/>审批时间：<%#Eval("inv_checkTime1")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("inv_flag1")%>"></span>
                            <span onmouseover="tip_index=layer.tips('开票区域审批<br/>审批人：<%#Eval("inv_checkNum2")%>-<%#Eval("inv_checkName2")%><br/>审批备注：<%#Eval("inv_checkRemark2").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%><br/>审批时间：<%#Eval("inv_checkTime2")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("inv_flag2")%>"></span>
                            <span onmouseover="tip_index=layer.tips('财务审批<br/>审批人：<%#Eval("inv_checkNum3")%>-<%#Eval("inv_checkName3")%><br/>审批备注：<%#Eval("inv_checkRemark3").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%><br/>审批时间：<%#Eval("inv_checkTime3")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("inv_flag3")%>"></span>
                        </td>
                        <td class="confirmTd">
                            <span onmouseover="tip_index=layer.tips('开票人：<%#Eval("inv_confirmerNum")%>-<%#Eval("inv_confirmerName")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Convert.ToBoolean(Eval("inv_isConfirm").ToString())?"2":"0"%>"></span>
                        </td>
                        <td class="dateTd"><%#Eval("inv_date").ToString()==""?"":ConvertHelper.toDate(Eval("inv_date")).Value.ToString("yyyy-MM-dd")%></td>
                        <td align="center"><a href="invoice_edit.aspx?action=<%#DTEnums.ActionEnum.Edit %>&id=<%#Eval("inv_id")%>">修改</a>
                            <%# ((manager.area==Eval("inv_farea").ToString() || manager.area==Eval("inv_darea").ToString()) && new MettingSys.BLL.permission().checkHasPermission(manager, "0603")) || new MettingSys.BLL.permission().checkHasPermission(manager, "0402") ?"<a href=\"invoice_edit.aspx?action="+DTEnums.ActionEnum.View+"&id="+Eval("inv_id")+"\">审批</a>":""%>
                            <span style="color:red;"></span>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"16\">暂无记录</td></tr>" : ""%>
  </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div style="font-size: 12px;color:darkblue;">
            <span style="float: left;">选中：<asp:Label ID="sCount" runat="server">0</asp:Label>条记录，开票金额：<asp:Label ID="sMoney" runat="server">0</asp:Label></span>
        </div>
        <div style="font-size: 12px;margin-top: 22px;">
            <span style="float: left;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，总计开票金额：<asp:Label ID="pMoney" runat="server">0</asp:Label></span>
            <span style="float: right;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计开票金额：<asp:Label ID="tMoney" runat="server">0</asp:Label></span>
        </div>
        <!--/列表-->
        <div class="dRemark" style="margin-top:45px;">
            <p>1.关键字筛查字段为：客户、订单号、开票项目</p>
            <p>2.审批：<span class="check_0"></span>待审批，<span class="check_1"></span>审批未通过，<span class="check_2"></span>审批通过</p>
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

