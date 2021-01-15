<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReconciliationDetail.aspx.cs" Inherits="MettingSys.Web.admin.finance.ReconciliationDetail" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>客户对账明细</title>
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

            if ($("#ddltype").val() == "True") {
                 $("#btncreateReceipt").show();
            }
            else {
                $("#btncreateReceipt").hide();
            }

            $(".checkall").click(function () {
                var obj = $(this);
                var oid = $(obj).attr("data-id");
                $(".check").each(function () {
                    if ($(this).attr("data-oid") == oid) {
                        $(this).prop("checked", obj.prop("checked"));

                        var sMoney = parseFloat($("#p31").html());
                        var tMoney = parseFloat($(this).attr("data-chkmoney"));
                        if ($(this).prop("checked") == true) {
                            $("#p31").html(eval(sMoney + tMoney));
                        } else {
                            $("#p31").html(eval(sMoney - tMoney));
                        }
                    }
                });

                var m33 = parseFloat($("#p33").html());
                var m34 = parseFloat($("#p34").html());
                var m35 = parseFloat($("#p35").html());
                if ($(this).prop("checked") == true) {
                    $("#p33").html(eval(m33 + parseFloat($(this).attr("data-finmoney"))));
                    $("#p34").html(eval(m34 + parseFloat($(this).attr("data-rpdmoney"))));
                    $("#p35").html(eval(m35 + parseFloat($(this).attr("data-unMoney"))));
                } else {
                    $("#p33").html(eval(m33 - parseFloat($(this).attr("data-finmoney"))));
                    $("#p34").html(eval(m34 - parseFloat($(this).attr("data-rpdmoney"))));
                    $("#p35").html(eval(m35 - parseFloat($(this).attr("data-unMoney"))));
                }

                var unchk = $(this).attr("data-unchkmoney");
                if (unchk != "--") {
                    var unchkmoney = parseFloat(unchk);
                    if ($("#p32").html() == "--") {
                        if ($(this).prop("checked") == true) {
                            $("#p32").html(unchkmoney);
                        }
                    }
                    else {
                        var m32 = parseFloat($("#p32").html());
                        if ($(this).prop("checked") == true) {
                            $("#p32").html(eval(m32 + unchkmoney));
                        }
                        else {
                            $("#p32").html(eval(m32 - unchkmoney));
                        }
                    }
                }
            });
            $(".check").click(function () {
                var sMoney = parseFloat($("#p31").html());
                var tMoney = parseFloat($(this).attr("data-chkmoney"));

                if ($(this).prop("checked") == true) {
                    $("#p31").html(eval(sMoney + tMoney));
                } else {
                    $("#p31").html(eval(sMoney - tMoney));
                }
            });
        })
        function selectAll() {
            $(".checkall").each(function () {
                if ($(this).prop("checked") == false) {
                    $(this).click();
                }
            });
        }
        function unSelectAll() {
            $(".checkall").click();
        }
        function addFinChk(finid, oid) {
            layer.open({
                type: 2,
                title: '对账',
                area: ['450px', '450px'],
                content: ['finance_chk.aspx?finid=' + finid + '&oid=' + oid, 'no'],
                end: function () {
                    location.reload();
                }
            });
        }
        //批量填充对账标识
        function mutilAddFinChk() {
            if ($(".check:checked").length <= 0) {
                layer.msg("请选择要操作的记录");
                return false;
            }
            var finIDstr = "";
            $(".check:checked").each(function () {
                finIDstr += $(this).attr("data-id") + "/" + $(this).attr("data-money") + "/" + $(this).attr("data-oid") + ",";
            });
            finIDstr = finIDstr.substring(0, finIDstr.length - 1);
            if ($("#txtNum").val() == "") {
                layer.msg("请填写对账标识");
                $("#txtNum").focus();
                return false;
            }
            parent.dialog({
                title: '提示',
                content: "确定要批量对账吗？",
                okValue: '确定',
                ok: function () {
                    var postData = { idstr: finIDstr, num: $("#txtNum").val() };
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/business_ajax.ashx?action=addFinChk",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.status == 0) {
                                layer.msg("对账成功");
                                setTimeout(function () {
                                    location.reload();
                                }, 1500);
                            } else {
                                top.dialog({
                                    title: '提示',
                                    content: data.msg,
                                    okValue: '确定',
                                    ok: function () { }
                                }).showModal();
                            }
                            closeLoad();
                        }
                    });
                },
                cancelValue: '取消',
                cancel: function () { }
            }).showModal();
        }
        //批量取消对账标识
        function mutilCancelFinChk() {
            if ($(".check:checked").length <= 0) {
                layer.msg("请选择要操作的记录");
                return false;
            }
            var finIDstr = "";
            $(".check:checked").each(function () {
                finIDstr += $(this).attr("data-id") + "-" + $(this).attr("data-oid") + ",";
            });
            finIDstr = finIDstr.substring(0, finIDstr.length - 1);
            parent.dialog({
                title: '提示',
                content: "确定要批量取消对账吗？",
                okValue: '确定',
                ok: function () {
                    var postData = { idstr: finIDstr};
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/business_ajax.ashx?action=cancelFinChk",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.status == 0) {
                                layer.msg("对账成功");
                                setTimeout(function () {
                                    location.reload();
                                }, 1500);
                            } else {
                                top.dialog({
                                    title: '提示',
                                    content: data.msg,
                                    okValue: '确定',
                                    ok: function () { }
                                }).showModal();
                            }
                            closeLoad();
                        }
                    });
                },
                cancelValue: '取消',
                cancel: function () { }
            }).showModal();
        }
        function createBill() {
            if ($("#hCusId").val() != "" && $("#hCusId").val() != "0") {
                var oIDstr = "";
                $(".checkall:checked").each(function () {
                    oIDstr += $(this).attr("data-id") + ",";
                });
                oIDstr = oIDstr.substring(0, oIDstr.length - 1);
                layer.open({
                    type: 2,
                    title: '账单',
                    area: ['1150px', '650px'],
                    //content: 'ReconciliationBill.aspx?oidStr=' + oIDstr + '&hCusId=' + $("#hCusId").val() + '&ddltype=' + $("#ddltype").val() + '&ddlsign=' + $("#ddlsign").val() + '&txtMoney1=' + $("#txtMoney1").val() + '&ddlnature=' + $("#ddlnature").val() + '&txtsDate=' + $("#txtsDate").val() + '&txteDate=' + $("#txteDate").val() + '&txtsDate1=' + $("#txtsDate1").val() + '&txteDate1=' + $("#txteDate1").val() + '&txtName=' + $("#txtName").val() + '&txtAddress=' + $("#txtAddress").val() + '&ddlsign1=' + $("#ddlsign1").val() + '&txtMoney2=' + $("#txtMoney2").val() + '&txtPerson1=' + $("#txtPerson1").val() + '&txtPerson2=' + $("#txtPerson2").val() + '&txtPerson3=' + $("#txtPerson3").val() + '&txtPerson4=' + $("#txtPerson4").val() + '&txtPerson5=' + $("#txtPerson5").val() + '&txtOrderID=' + $("#txtOrderID").val() + '&txtChk=' + $("#txtChk").val() + '&ddlstatus=' + $("#ddlstatus").val() + '&ddllock=' + $("#ddllock").val() + '&ddlarea=' + $("#ddlarea").val() + '&txtsDate2=' + $("#txtsDate2").val() + '&txteDate2=' + $("#txteDate2").val() + '&txtsDate3=' + $("#txtsDate3").val() + '&txteDate3=' + $("#txteDate3").val() + ''
                    content: 'ReconciliationBill.aspx?oidStr=' + oIDstr + '&hCusId=' + $("#hCusId").val() + '&ddltype=' + $("#ddltype").val() + '&ddlsign=' + $("#ddlsign").val() + '&txtMoney1=' + $("#txtMoney1").val() + '&ddlnature=' + $("#ddlnature").val() + '&txtsDate=' + $("#txtsDate").val() + '&txteDate=' + $("#txteDate").val() + '&txtsDate1=' + $("#txtsDate1").val() + '&txteDate1=' + $("#txteDate1").val() + '&txtName=' + $("#txtName").val() + '&txtAddress=' + $("#txtAddress").val() + '&ddlsign1=' + $("#ddlsign1").val() + '&txtMoney2=' + $("#txtMoney2").val() + '&txtPerson1=' + $("#txtPerson1").val() + '&txtPerson2=' + $("#txtPerson2").val() + '&txtPerson3=' + $("#txtPerson3").val() + '&txtPerson4=' + $("#txtPerson4").val() + '&txtPerson5=' + $("#txtPerson5").val() + '&txtOrderID=' + $("#txtOrderID").val() + '&txtChk=' + $("#txtChk").val() + '&ddlstatus=' + $("#ddlstatus").val() + '&ddllock=' + $("#ddllock").val() + '&ddlarea=' + $("#ddlarea").val() + ''
                });
            }
            else {
                layer.msg("请先选择应收付对象");
            }
        }
        function createReceipt() {
            if ($("#hCusId").val() != "" && $("#hCusId").val() != "0") {
                if ($(".checkall:checked").length == 0) {
                    layer.msg("请先选择订单");
                    return;
                }
                var oIDstr = "";
                var tMoney = 0;
                var oidList = [];
                
                $(".checkall:checked").each(function () {
                    var oidJson = {};
                    oIDstr += $(this).attr("data-id") + ",";
                    tMoney += parseFloat($(this).attr("data-unrpmoney"));
                    oidJson.oid = $(this).attr("data-id");
                    oidJson.money = parseFloat($(this).attr("data-unrpmoney"))
                    oidList.push(oidJson);
                });
                oIDstr = oIDstr.substring(0, oIDstr.length - 1);
                if ($("#ddltype").val() == "True") {
                    layer.open({
                        type: 2,
                        title: '账单打包',
                        area: ['950px', '650px'],
                        content: 'receipt_edit.aspx?action=Add&oidStr=' + encodeURI(JSON.stringify(oidList)) + '&oStr='+oIDstr+'&cid=' + $("#hCusId").val() + '&cusName=' + $("#txtCusName").val() + '&tMoney=' + tMoney + '&chk=' + $("#txtChk").val() + '&fromDetail=1'
                    });
                }
                else {
                    layer.msg("只能生成收款通知");
                }
            }
            else {
                layer.msg("请先选择应收付对象");
            }
        }
        function showButton(value) {
            if (value == "True") {
                $("#btncreateReceipt").show();
            }
            else {
                $("#btncreateReceipt").hide();
            }
        }
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
            <span>客户对账明细</span>
        </div>
        <!--/导航栏-->

        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            <div class="toolbar">
                <div class="menu-list" style="margin-bottom: 10px;">
                    应收付对象：
                                <asp:TextBox ID="txtCusName" runat="server" CssClass="input" Width="150"></asp:TextBox>
                    <asp:HiddenField ID="hCusId" runat="server" />
                    收付类别：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddltype" runat="server" onchange="showButton(this.value)"></asp:DropDownList>
                        </div>
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
                    业务性质：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddlnature" runat="server"></asp:DropDownList>
                        </div>
                    业务明细：
                    <asp:TextBox ID="txtDetails" runat="server" CssClass="input"></asp:TextBox>
                    活动开始日期：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
                    -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
                    活动结束日期：
                        <asp:TextBox ID="txtsDate1" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate1\')}'})"></asp:TextBox>
                    -
                        <asp:TextBox ID="txteDate1" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate1\')}'})"></asp:TextBox>

                    
                </div>
                <div class="menu-list" style="margin-bottom: 10px;">
                    活动名称：
                    <asp:TextBox ID="txtName" runat="server" CssClass="input"></asp:TextBox>
                    活动地点：
                    <asp:TextBox ID="txtAddress" runat="server" CssClass="input"></asp:TextBox>
                    订单未收金额：
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
                    <asp:TextBox ID="txtMoney2" runat="server" CssClass="input small"></asp:TextBox>
                    业务员：
                    <asp:TextBox ID="txtPerson1" runat="server" CssClass="input small" onkeyup="cToUpper(this)"></asp:TextBox>
                    报账人员：
                    <asp:TextBox ID="txtPerson2" runat="server" CssClass="input small" onkeyup="cToUpper(this)"></asp:TextBox>
                    策划人员：
                    <asp:TextBox ID="txtPerson3" runat="server" CssClass="input small" onkeyup="cToUpper(this)"></asp:TextBox>
                    设计人员：
                    <asp:TextBox ID="txtPerson5" runat="server" CssClass="input small" onkeyup="cToUpper(this)"></asp:TextBox>
                    执行人员：
                    <asp:TextBox ID="txtPerson4" runat="server" CssClass="input small" onkeyup="cToUpper(this)"></asp:TextBox>
                    <input type="hidden" name="self" value="<%=_self %>" />
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                    <asp:LinkButton ID="btnExcel" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>
                </div>
                <div class="menu-list" style="margin-bottom: 10px;">
                    订单号：
                    <asp:TextBox ID="txtOrderID" runat="server" CssClass="input small"></asp:TextBox>
                    对账标识：
                    <asp:TextBox ID="txtChk" runat="server" CssClass="input small"></asp:TextBox>

                    订单状态：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlstatus" runat="server"></asp:DropDownList>
                                </div>
                    锁单状态：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddllock" runat="server"></asp:DropDownList>
                                </div>
                    区域：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlarea" runat="server"></asp:DropDownList>
                                </div>
                    审核状态：
                    <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck" runat="server"></asp:DropDownList>
                            </div>
                    订单收款日期：
                        <asp:TextBox ID="txtsDate2" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate2\')}'})"></asp:TextBox>
                    -
                        <asp:TextBox ID="txteDate2" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate2\')}'})"></asp:TextBox>
                    
                    <%--业务开始日期：
                        <asp:TextBox ID="txtsDate2" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate2\')}'})"></asp:TextBox>
                    -
                        <asp:TextBox ID="txteDate2" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate2\')}'})"></asp:TextBox>
                    业务结束日期：
                        <asp:TextBox ID="txtsDate3" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate3\')}'})"></asp:TextBox>
                    -
                        <asp:TextBox ID="txteDate3" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate3\')}'})"></asp:TextBox>--%>
                </div>
            </div>
        </div>
        <!--/工具栏-->

        <!--列表-->
        <div class="table-container">
            <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                <tr style="text-align: center;">
                    <th width="3%">选择</th>
                    <th width="6%">订单号</th>
                    <th width="10%" style="text-align: left;">客源</th>
                    <th width="6%">活动日期</th>
                    <th width="10%" style="text-align: left;">活动地点/活动名称</th>
                    <th width="3%"></th>
                    <th width="6%">对账标识</th>
                    <th width="6%" style="text-align: right;">对账金额</th>
                    <%--<th width="6%">业务日期</th>--%>
                    <th width="8%" style="text-align: center;">业务性质/明细</th>
                    <th>业务说明</th>
                    <th width="8%">表达式</th>
                    <th width="3%">审核</th>
                    <th width="7%" style="text-align: right;"><%=string.IsNullOrEmpty(_chk)||_chk=="空"?"":"<font color='green'>对账金额/</font>" %><%=_type=="True"?"应收":"应付" %></th>
                    <th width="7%" style="text-align: right;"><%=string.IsNullOrEmpty(_chk)||_chk=="空"?"":"<font color='green'>对账金额/</font>" %><%=_type=="True"?"已收":"已付" %></th>
                    <th width="7%" style="text-align: right;"><%=string.IsNullOrEmpty(_chk)||_chk=="空"?"":"<font color='green'>对账金额/</font>" %><%=_type=="True"?"未收":"未付" %></th>
                </tr>
                <%=trHtml %>
            </table>
        </div>
        <div style="font-size: 12px;">
            <p><font color="blue">对账金额：</font>已对账金额本页合计：<asp:Label ID="p11" runat="server">0</asp:Label>，已对账未收付金额本页合计：<asp:Label ID="p12" runat="server">0</asp:Label>，已对账金额总计：<asp:Label ID="p13" runat="server">0</asp:Label>，已对账未收付金额总计：<asp:Label ID="p14" runat="server">0</asp:Label></p>
            <p><font color="blue">订单金额：</font>本页应收付合计：<asp:Label ID="p21" runat="server">0</asp:Label>，已收付：<asp:Label ID="p22" runat="server">0</asp:Label>，未收付：<asp:Label ID="p23" runat="server">0</asp:Label>；全部应收付：<asp:Label ID="p24" runat="server">0</asp:Label>，已收付：<asp:Label ID="p25" runat="server">0</asp:Label>，未收付：<asp:Label ID="p26" runat="server">0</asp:Label></p>
            <p><font color="blue">选中记录：</font>已对账金额：<asp:Label ID="p31" runat="server">0</asp:Label>，已对账未收付金额：<asp:Label ID="p32" runat="server">--</asp:Label>，应收付金额：<asp:Label ID="p33" runat="server">0</asp:Label>，已收付金额：<asp:Label ID="p34" runat="server">0</asp:Label>，未收付金额：<asp:Label ID="p35" runat="server">0</asp:Label></p>
        </div>
        <!--工具栏-->
        <div class="toolbar-wrap" style="float: left;">
            <input type="button" onclick="selectAll()" value="全选" class="btns" />
            <input type="button" onclick="unSelectAll()" value="反选" class="btns" />
            批量对账标识：
            <asp:TextBox ID="txtNum" runat="server" CssClass="input" Width="150"></asp:TextBox>
            <input type="button" onclick="mutilAddFinChk()" value="填充对账标识" class="btn" />
            <input type="button" onclick="mutilCancelFinChk()" value="取消对账标识" class="btn" />
            <input type="button" value="生成账单" onclick="createBill()" class="btn" />
            <input type="button" value="生成收款通知" id="btncreateReceipt" onclick="createReceipt()" class="btn" />
            <input id="btnReturn" type="button" value="返回上一页" class="btn yellow" onclick="javascript: history.back(-1);" />
        </div>
        <!--/工具栏-->

        <!--/列表-->
        <div class="dRemark">
            <p>1.搜索凭证号：填“0”表示搜索凭证号为空的记录；填“1”表示搜索凭证号不为空的记录
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
