<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pay_list.aspx.cs" Inherits="MettingSys.Web.admin.finance.pay_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>付款通知列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <link href="../../scripts/tip/tip.css" rel="stylesheet" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script src="../../scripts/tip/tip.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
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
                if ($(this).val() == "True") {
                    $(".spdate").show();
                }
                else {
                    $(".spdate").hide();
                }
            });
            bingCertificate();

            $(".cusTip").on({                
                mouseover: function () {
                    var cid = $(this).attr("data-cid");
                    var that = this;
                    var postData = { "cid": cid };
                    var _table = $("<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"0\"><tr><th style=\"width:30%;\">账户名称</th><th>银行账号</th><th style=\"width:30%;\">开户行</th></tr></table>");
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/Business_ajax.ashx?action=getCusBank",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            var jlist = eval(data);
                            var trlist = "";
                            if (jlist.length > 0) {
                                for (var i = 0; i < jlist.length; i++) {
                                    var json = eval(jlist[i]);
                                    var _tr = "<tr style=\"text-align:center;\"><td>" + json.cb_bankName + "</td><td>" + json.cb_bankNum + "</td><td>" + json.cb_bank + "</td></tr>";
                                    trlist += _tr;
                                }
                            }
                            _table.append(trlist);
                            showTip(that,_table,600)
                        }
                    });
                }
            });
            $(".checkall input").change(function () {
                computeSelect();
            });
        })
        function computeSelect() {
            $("#sCount").text($(".checkall input:checked").size());
            var _smoney = 0, _sumoney = 0;
            $(".checkall input:checked").each(function () {
                _smoney += parseFloat($(this).parent().parent().parent().children(".moneyTd").html());
                _sumoney += parseFloat($(this).parent().parent().parent().children(".umoneyTd").children().html());
            });
            $("#sMoney").text(_smoney.toFixed(2));
            $("#suMoney").text(_sumoney.toFixed(2));
        }
        //绑定凭证
        function bingCertificate() {
            $.getJSON("../../tools/business_ajax.ashx?action=getAllCertificate", function (json) {
                $('#txtCenum').devbridgeAutocomplete({
                    lookup: json,
                    minChars: 0,
                    onSelect: function (suggestion) {
                        $('#txtCedate').val(suggestion.id);
                    },
                    showNoSuggestionNotice: true,
                    noSuggestionNotice: '抱歉，没有匹配的选项'
                });
            });
        }
        //展开明细
        function showDetail(rpid) {
            if ($(".Detail" + rpid).length == 0) {
                $.getJSON("../../tools/business_ajax.ashx?action=getDetails&rpid=" + rpid, function (json) {
                    $("#tr" + rpid).siblings(".Detail" + rpid + "").hide();
                    if (json.length > 0) {
                        var _trhtml = "<tr class='Detail" + rpid + "' style='background-color: gainsboro;'><td></td><td>订单号</td><td>付款对象</td><td>付款金额</td><td>预付日期</td><td>申请人</td><td>区域</td><td>状态</td><td colspan=\"5\">付款内容</td></tr>";
                        $.each(json, function (index, item) {
                            _trhtml += "<tr class='Detail" + rpid + "'><td></td><td><a href=\"../order/order_edit.aspx?action=Edit&oID=" + item.rpd_oid + "\">" + item.rpd_oid + "</a></td>"
                                + "<td>" + item.c_name + "</td>"
                                + "<td>" + item.rpd_money + "</td>"
                                + "<td>" + item.rpd_foredate.substring(0, 10) + "</td>"
                                + "<td title=\"" + item.rpd_personNum + "\">" + item.rpd_personName + "</td>"
                                + "<td>" + item.rpd_area + "</td>"
                                + "<td><span class=\"check_" + item.rpd_flag1 + "\"></span><span class=\"check_" + item.rpd_flag2 + "\"></span><span class=\"check_" + item.rpd_flag3 + "\"></span></td>"
                                + "<td colspan=\"5\">" + item.rpd_content + "</td></tr > ";
                        });
                        $("#tr" + rpid).after(_trhtml);
                    }
                });
            }
            else {
                $(".Detail" + rpid).remove();
            }
        }
        function toggleCheckDiv() {
            $("#confirmDiv").hide();
            $("#certificateDiv").hide();
            $("#checkDiv").toggle();
        }
        function toggleconfirmDiv() {
            $("#checkDiv").hide();
            $("#certificateDiv").hide();
            $("#confirmDiv").toggle();
            $(".spdate").show();
        }
        function togglecertificateDiv() {
            $("#checkDiv").hide();
            $("#confirmDiv").hide();
            $("#certificateDiv").toggle();
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
            if ($("#ddlcheck1").val() == "") {
                jsprint("请选择审批状态");
                return;
            }
            $(".checkall input:checked").each(function () {
                var id = $(this).parent().next().val();
                var postData = { "id": id, "ctype": $("#ddlchecktype").val(), "status": $("#ddlcheck1").val(), "remark": $("#txtremark").val() };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=checkPay",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $("#tr" + id).children().last().children().last().html("");
                        if (data.status == 0) {                            
                            $("#tr" + id).find(".checkTd").children().eq($("#ddlchecktype").val() - 1).removeClass().addClass("check_"+$("#ddlcheck1").val());
                        } else {
                            $("#tr" + id).children().last().children().last().append(data.msg);
                        }
                    }
                });
            });            
        }
        /*批量审批 */
        /*确认付款 */
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
            if ($("#ddlisConfirm1").val() == "" && $("#ddlmethod1").val() == "") {
                jsprint("请选择付款状态或付款方式");
                return;
            }
            if ($("#ddlisConfirm1").val() == "True") {
                if ($("#txtdate1").val() == "") {
                    jsprint("请选择实付日期");
                    return;
                }
                //if ($("#ddlmethod1").val() == "") {
                //    jsprint("请选择付款方式");
                //    return;
                //}
            }
            $(".checkall input:checked").each(function () {
                var id = $(this).parent().next().val();
                var postData = { "id": id, "status": $("#ddlisConfirm1").val(), "date": $("#txtdate1").val(), "method": $("#ddlmethod1").val() };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=confirmReceiptPay&type=0",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $("#tr" + id).children().last().children().last().html("");
                        if (data.status == 0) {                            
                            if ($("#ddlisConfirm1").val() == "True") {
                                $("#tr" + id).find(".confirmTd").children().eq(0).removeClass().addClass("check_2");
                                $("#tr" + id).find(".dateTd").text($("#txtdate1").val());
                            }
                            else {
                                $("#tr" + id).find(".confirmTd").children().eq(0).removeClass().addClass("check_0"); 
                                $("#tr" + id).find(".dateTd").html("");
                            }
                            if ($("#ddlmethod1").val() != "") {
                                $("#tr" + id).find(".methodTd").text($("#ddlmethod1 option:selected").text());
                            }
                        } else {
                            $("#tr" + id).children().last().children().last().append(data.msg);
                        }
                    }
                });
            });
        }
        /*确认付款 */
        /*凭证 */
        function submitCertificate() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            if ($("#txtCenum").val() == "") {
                jsprint("请选择凭证号");
                return;
            }
            if ($("#txtCedate").val() == "") {
                jsprint("请选择凭证日期");
                return;
            }
            $(".checkall input:checked").each(function () {
                var id = $(this).parent().next().val();
                var postData = { "id": id, "num": $("#txtCenum").val(), "date": $("#txtCedate").val() };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=signCertificate",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $("#tr" + id).children().last().children().last().html("");
                        if (data.status == 0) {   
                                $("#tr" + id).find(".numTd").html($("#txtCenum").val());
                        } else {
                            $("#tr" + id).children().last().children().last().append(data.msg);
                        }
                    }
                });
            });

            
        }
        /*凭证 */
        /*取消凭证 */
        function cancelCertificate() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            $(".checkall input:checked").each(function () {
                var id = $(this).parent().next().val();
                var postData = { "id": id };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=unsignCertificate",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $("#tr" + id).children().last().children().last().html("");
                        if (data.status == 0) {   
                                $("#tr" + id).find(".numTd").html("");
                        } else {
                            $("#tr" + id).children().last().children().last().append(data.msg);
                        }
                    }
                });
            });

            
        }
        /*取消凭证 */
        /*添加凭证 */
        function addCertificate() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            if ($("#txtCenum").val() == "") {
                jsprint("请选择凭证号");
                return;
            }
            if ($("#txtCedate").val() == "") {
                jsprint("请选择凭证日期");
                return;
            }
            var idStr = "";
            $(".checkall input:checked").each(function () {
                idStr += $(this).parent().next().val() + ",";
            });
            idStr = idStr.substring(0, idStr.length - 1);

            var postData = { "ids": idStr, "num": $("#txtCenum").val(), "date": $("#txtCedate").val() };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/Business_ajax.ashx?action=addCertificate",
                data: postData,
                dataType: "json",
                success: function (data) {
                    if (data.status == 0) {
                        var d = top.dialog({ content: data.msg }).show();
                        setTimeout(function () {
                            d.close().remove();
                        }, 2000);
                    } else {
                        top.dialog({
                            title: '提示',
                            content: data.msg,
                            okValue: '确定',
                            ok: function () { }
                        }).showModal();
                    }
                }
            });
        }
        /*添加凭证 */
        //分配
        function dealDistribution(id) {
            layer.open({
                type: 2,
                title: '分配付款金额',
                area: ['950px', '650px'],
                content: 'rpDistribution.aspx?id=' + id
            });
        }
        //付款凭证
        function showPay(rpid, unMoney) {
            layer.open({
                type: 2,
                title: '付款凭证',
                area: ['750px', '600px'],
                content: ['payCertification.aspx?rpid=' + rpid + '&unmoney=' + unMoney, 'no']
            });
        }
    </script>
    <style type="text/css">
        .date-input {
            width: 110px;
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
            <span>付款通知列表</span>
        </div>
        <!--/导航栏-->

        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            <div class="toolbar">
                <div class="box-wrap">
                    <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                    <div class="l-list">
                        <ul class="icon-list">
                            <li><a href="pay_edit.aspx?action=<%=DTEnums.ActionEnum.Add %>"><i class="iconfont icon-close"></i><span>新增</span></a></li>
                            <li><a href="javascript:;" onclick="checkAll(this);computeSelect();"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                            <li><a href="javascript:;" onclick="reverseCheckAll();computeSelect();"><i class="iconfont icon-check-mark"></i><span>反选</span></a></li>
                            <li>
                                <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete','删除后无法恢复，是否继续？');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                            <li><a href="javascript:;" onclick="toggleCheckDiv()"><span>审批</span></a></li>
                            <li><a href="javascript:;" onclick="toggleconfirmDiv()"><span>支付操作</span></a></li>
                            <li><a href="javascript:;" onclick="togglecertificateDiv()"><span>标记凭证</span></a></li>
                        </ul>
                        <div id="checkDiv" style="display: none;">
                            审批类型：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlchecktype" runat="server">
                                    <asp:ListItem Value="">请选择</asp:ListItem>
                                    <asp:ListItem Value="1">财务审批</asp:ListItem>
                                    <asp:ListItem Value="2">总经理审批</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            审批状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck1" runat="server"></asp:DropDownList>
                            </div>
                            备注：
                            <asp:TextBox ID="txtremark" runat="server" CssClass="input normal"></asp:TextBox>
                            <input type="button" class="btn" value="提交" onclick="submitCheck()" />
                        </div>
                        <div id="confirmDiv" style="display: none;">
                            付款状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlisConfirm1" runat="server">
                                </asp:DropDownList>
                            </div>
                            <span class="spdate" style="display: none;">实付日期：<asp:TextBox ID="txtdate1" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                            </span>
                            付款方式：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlmethod1" runat="server">
                                </asp:DropDownList>
                            </div>
                            <input type="button" class="btn" value="提交" onclick="submitConfirm()" />
                        </div>
                        <div id="certificateDiv" style="display: none;">
                            凭证号：<asp:TextBox ID="txtCenum" runat="server" CssClass="input " Width="150" />
                            凭证日期：<asp:TextBox ID="txtCedate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                            <input type="button" class="btn" value="标记凭证" onclick="submitCertificate()" />
                            <%--<input type="button" class="btn" value="添加凭证" onclick="addCertificate()" />--%>
                            <input type="button" class="btn" value="取消凭证" onclick="cancelCertificate()" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--/工具栏-->
        <div class="searchbar" style="margin-bottom: 10px;">
            付款对象：
                            <asp:TextBox ID="txtCusName" runat="server" CssClass="input"></asp:TextBox>
            <asp:HiddenField ID="hCusId" runat="server" />
            付款方式：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlmethod" runat="server"></asp:DropDownList>
                            </div>
            财务审批：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck" runat="server"></asp:DropDownList>
                            </div>
            总经理审批：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck2" runat="server"></asp:DropDownList>
                            </div>
            付款状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlisConfirm" runat="server"></asp:DropDownList>
                            </div>
            汇总日期：
                            <asp:TextBox ID="txtsforedate" runat="server" CssClass="input rule-date-input" Width="110" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            -
                            <asp:TextBox ID="txteforedate" runat="server" CssClass="input rule-date-input" Width="110" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtsforedate\')}'})" />
            实付日期：
                            <asp:TextBox ID="txtsdate" runat="server" CssClass="input rule-date-input" Width="110" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            -
                            <asp:TextBox ID="txtedate" runat="server" CssClass="input rule-date-input" Width="110" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtsdate\')}'})" />

        </div>
        <div class="searchbar" style="margin-bottom: 10px;">
            凭证号：
                            <asp:TextBox ID="txtNum" runat="server" Width="100px" CssClass="input" />
            凭证日期：
            <asp:TextBox ID="txtNumDate" runat="server" CssClass="input rule-date-input" Width="110" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            对账凭证：
                            <asp:TextBox ID="txtChk" runat="server" Width="100px" CssClass="input" />
            <div class="rule-single-select">
                            <asp:DropDownList ID="ddlmoneyType" runat="server">
                                <asp:ListItem Value="1">金额</asp:ListItem>
                                <asp:ListItem Value="2">未分配金额</asp:ListItem>
                            </asp:DropDownList>
                        </div>
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
            预付款：
            <div class="rule-single-select">
                            <asp:DropDownList ID="ddlType" runat="server">
                                <asp:ListItem Value="">不限</asp:ListItem>
                                <asp:ListItem Value="True">预付款</asp:ListItem>
                                <asp:ListItem Value="False">非预付款</asp:ListItem>
                            </asp:DropDownList>
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
                            <th width="6%">选择</th>
                            <th align="left" width="10%">付款对象</th>
                            <th align="left" width="6%">凭证号</th>
                            <th align="left">付款内容</th>
                            <th align="left" width="10%">客户银行账号</th>
                            <th align="left" width="6%">付款金额</th>
                            <th align="left" width="8%">未分配金额</th>
                            <th align="left" width="8%">汇总日期</th>
                            <th align="left" width="8%">付款方式</th>
                            <th align="left" width="8%">实付日期</th>
                            <th align="left" width="8%">申请人</th>
                            <th align="left" width="4%">审批</th>
                            <th align="left" width="4%">确认付款</th>
                            <th width="8%">操作</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr id="tr<%#Eval("rp_id")%>">
                        <td align="center">
                            <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                            <asp:HiddenField ID="hidId" Value='<%#Eval("rp_id")%>' runat="server" />
                        </td>
                        <td><span class="cusTip" data-cid="<%#Eval("rp_cid")%>"><%# Eval("c_name") %></span><%# bool.Parse(Eval("rp_isExpect").ToString())?"<font color='green'>[预]</font>":"" %></td>
                        <td class="numTd"><span onmouseover="tip_index=layer.tips('凭证日期：<%#Eval("ce_date").ToString()==""?"":Convert.ToDateTime(Eval("ce_date")).ToString("yyyy-MM-dd")%><br/>备注：<%# Eval("ce_remark") %>', this, { time: 0 });" onmouseout="layer.close(tip_index);"><%# Eval("ce_num") %></span></td>
                        <td><%# Eval("rp_content") %></td>
                        <td><%#Eval("cb_bankName")%><br /><%#Eval("cb_bankNum")%><br /><%#Eval("cb_bank")%></td>
                        <td class="moneyTd"><%# Eval("rp_money") %></td>
                        <td class="umoneyTd"><a href="javascript:;" onclick="showDetail(<%#Eval("rp_id")%>)"><%# Eval("undistribute") %></a></td>
                        <td><%# ConvertHelper.toDate(Eval("rp_foredate"))==null?"":ConvertHelper.toDate(Eval("rp_foredate")).Value.ToString("yyyy-MM-dd") %></td>
                        <td class="methodTd"><%# Eval("pm_name") %></td>
                        <td class="dateTd"><%# ConvertHelper.toDate(Eval("rp_date"))==null?"":Convert.ToDateTime(Eval("rp_date")).ToString("yyyy-MM-dd") %></td>
                        <td><%# Eval("rp_personNum") %>-<%# Eval("rp_personName") %></td>
                        <td class="checkTd">
                            <span onmouseover="tip_index=layer.tips('财务审批<br/>审批人：<%#Eval("rp_checkNum")%>-<%#Eval("rp_checkName")%><br/>审批备注：<%#Eval("rp_checkRemark").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%><br/>审批时间：<%#Eval("rp_checkTime")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("rp_flag")%>"></span>
                            <span onmouseover="tip_index=layer.tips('总经理审批<br/>审批人：<%#Eval("rp_checkNum1")%>-<%#Eval("rp_checkName1")%><br/>审批备注：<%#Eval("rp_checkRemark1").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%><br/>审批时间：<%#Eval("rp_checkTime1")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("rp_flag1")%>"></span>
                        </td>
                        <td class="confirmTd">
                            <span class="check_<%#Convert.ToBoolean(Eval("rp_isConfirm").ToString())?"2":"0"%>"></span><span title="<%#Eval("rp_confirmerNum")%>"><%#Eval("rp_confirmerName")%></span>
                        </td>
                        <td align="center">
                            <a href="pay_edit.aspx?action=<%#DTEnums.ActionEnum.Edit %>&id=<%#Eval("rp_id")%>">修改</a>
                            <a href="javascript:;" onclick="dealDistribution(<%#Eval("rp_id")%>)">分配</a>
                            <%#Convert.ToBoolean(Eval("rp_isConfirm").ToString())?"<a href=\"javascript:;\" onclick=\"showPay("+Eval("rp_id")+","+Eval("undistribute")+")\">凭证</a>":""%>
                            <%# new MettingSys.BLL.permission().checkHasPermission(manager,"0402,0601") ?"<a href=\"pay_edit.aspx?action="+DTEnums.ActionEnum.View+"&id="+Eval("rp_id")+"\">审批</a>":""%>
                            <span style="color:red;"></span>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"14\">暂无记录</td></tr>" : ""%>
  </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div style="font-size: 12px;color:darkblue;">
            <span style="float: left;">选中：<asp:Label ID="sCount" runat="server">0</asp:Label>条记录，付款金额：<asp:Label ID="sMoney" runat="server">0</asp:Label>，未分配金额：<asp:Label ID="suMoney" runat="server">0</asp:Label></span>
        </div>
        <div style="font-size: 12px;margin-top: 25px;">
            <span style="float: left;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，总计付款金额：<asp:Label ID="pMoney" runat="server">0</asp:Label>，总计未分配金额：<asp:Label ID="pUnMoney" runat="server">0</asp:Label></span>
            <span style="float: right;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计付款金额：<asp:Label ID="tMoney" runat="server">0</asp:Label>，总计未分配金额：<asp:Label ID="tUnMoney" runat="server">0</asp:Label></span>
        </div>
        <!--/列表-->
        <div class="dRemark" style="margin-top: 47px;">
            <p>1.付款：<span class="check_0"></span>待付款，<span class="check_2"></span>已付款</p>
            <p>2.删除付款通知不会删除付款明细</p>
            <p>3.付款对象后面带“<font color='green'>[预]</font>”，表示预付款</p>
            <p>4.凭证号填“0”：表示搜索凭证号为空的记录</p>
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
