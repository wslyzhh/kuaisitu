<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="paydetailcollect.aspx.cs" Inherits="MettingSys.Web.admin.finance.paydetailcollect" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>付款明细汇总列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <link href="../../scripts/tip/tip.css" rel="stylesheet" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script src="../../scripts/tip/tip.js"></script>
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

            $("#ddlCollectType").change(function () {
                if ($(this).val() == "2" || $(this).val() == "3") {
                    $(".smethod").attr("style", "display:;");
                }
                else {
                    $(".smethod").attr("style", "display:none;");
                }
                if ($(this).val() == "3") {
                    $(".spdate").show();
                }
                else {
                    $(".spdate").hide();
                }
            });
            $(".checkall input").change(function () {
                computeSelect();
            });
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
        })
        function computeSelect() {
            $("#sCount").text($(".checkall input:checked").size());
            var _smoney = 0;
            $(".checkall input:checked").each(function () {
                _smoney += parseFloat($(this).parent().parent().parent().children(".moneyTd").html());
            });
            $("#sMoney").text(_smoney.toFixed(2));
        }
        function collectDetail(cid,method) {
            $.getJSON("../../tools/business_ajax.ashx?action=collectDetail&cid=" + cid + "&method="+method, function (data) {
                if (data.status == 0) {
                    var d = top.dialog({ content: data.info }).show();
                    setTimeout(function () {
                        d.close().remove();
                        location.reload();
                    }, 2000);
                } else {
                    top.dialog({
                        title: '提示',
                        content: data.info,
                        okValue: '确定',
                        ok: function () {
                            location.reload();
                        }
                    }).showModal();
                }
            });
        }
        function toggleCollectDiv() {
            $("#collectDiv").toggle();
            $("#payMethodDiv").hide();
        }
        function togglePayMethodDiv() {
            $("#payMethodDiv").toggle();
            $("#collectDiv").hide();
        }
        /*批量汇总 */
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
            if ($("#ddlCollectType").val() == "") {
                jsprint("请选择汇总方式");
                return;
            }
            if ($("#ddlCollectType").val() == "2" || $("#ddlCollectType").val() == "3") {
                $(".checkall input:checked").each(function () {
                    var methodid = $(this).parent().next().next().val();
                    if ((methodid == "" || methodid == "0") && $("#ddlmethod").val() == "") {
                        jsprint("请选择付款方式");
                        return;
                    }
                });
            }
            if ($("#ddlCollectType").val() == "3" && $("#txtdate").val() == "") {
                jsprint("请选择实付日期");
                return;
            }
            
            $(".checkall input:checked").each(function () {
                var cid = $(this).parent().next().val();
                var methodid = $(this).parent().next().next().val();
                var method = $(this).parent().next().next().next().val();
                var postData = { "cid": cid, "ctype": $("#ddlCollectType").val(), "methodid": methodid, "method": method, "sdate": $("#txtsforedate").val(), "edate": $("#txteforedate").val(), "newmethodid": $("#ddlmethod").val(), "newmethod": $("#ddlmethod option:selected").text(), "date": $("#txtdate").val() };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=mutlCollect",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.status == 0) {
                            $("#span_" + cid + "_" + methodid).html("<font color='green'>成功</font>");
                        } else {
                            $("#span_" + cid + "_" + methodid).html("<font color='red'>" + data.msg + "</font>");
                        }
                    }
                });
            });
            
        }
        /*批量汇总 */
        //填写付款方式
        function submitPayMethod() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            if ($("#ddlmethod1").val() == "") {
                jsprint("请选择付款方式");
                return;
            }
            $(".checkall input:checked").each(function () {
                var cid = $(this).parent().next().val();
                var methodid = $(this).parent().next().next().val();
                var method = $(this).parent().next().next().next().val();
                var postData = { "cid": cid,"methodid": methodid, "method": method, "sdate": $("#txtsforedate").val(), "edate": $("#txteforedate").val(),"newmethodid":$("#ddlmethod1").val(),"newmethod":$("#ddlmethod1 option:selected").text() };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=setPayMethod_mutli",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.status == 0) {
                            var d = top.dialog({ content: data.msg }).show();
                            setTimeout(function () {
                                d.close().remove();
                                location.reload();
                            }, 2000);
                        } else {
                            top.dialog({
                                title: '提示',
                                content: data.msg,
                                okValue: '确定',
                                ok: function () {
                                    location.reload();
                                }
                            }).showModal();
                        }
                    }                    
                });
            });
        }
    </script>
    <style type="text/css">
        .date-input {
            width: 120px;
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
            <span>付款明细汇总列表</span>
        </div>
        <!--/导航栏-->
        <div class="content-tab-wrap">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a <%=_check=="0"?"class=\"selected\"":"" %> href="paydetail_list.aspx?check=0">付款明细列表</a></li>
                        <li><a <%=_check=="1"?"class=\"selected\"":"" %> href="paydetail_list.aspx?check=1">部门未审批</a></li>
                        <li><a <%=_check=="2"?"class=\"selected\"":"" %> href="paydetail_list.aspx?check=2">财务未审批</a></li>
                        <li><a <%=_check=="3"?"class=\"selected\"":"" %> href="paydetail_list.aspx?check=3">总经理未审批</a></li>
                        <li><a class="selected" href="paydetailcollect.aspx">付款明细汇总列表</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="tab-content" style="padding-top: 0; display: none;">
        </div>
        <div class="tab-content" style="padding-top: 0;">
            <!--工具栏-->
            <div class="toolbar-wrap">
                <div class="toolbar">
                    <div class="box-wrap">
                        <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                        <div class="l-list" style="display: block;">
                            <ul class="icon-list">
                                <li><a href="javascript:;" onclick="checkAll(this);computeSelect();"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                                <li><a href="javascript:;" onclick="toggleCollectDiv()"><span>汇总</span></a></li>
                                <li id="paymethodLi" runat="server"><a href="javascript:;" onclick="togglePayMethodDiv()"><span>填写付款方式</span></a></li>
                            </ul>
                            <div id="collectDiv" style="display: none;">
                                汇总方式：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlCollectType" runat="server">
                                        <asp:ListItem Value="">请选择</asp:ListItem>
                                        <asp:ListItem Value="1">汇总</asp:ListItem>
                                        <asp:ListItem Value="2">汇总并填充付款方式</asp:ListItem>
                                        <asp:ListItem Value="3">汇总并确认付款</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <span class="smethod" style="display: none;">付款方式：</span>
                                <div class="rule-single-select smethod" style="display: none;">
                                    <asp:DropDownList ID="ddlmethod" runat="server"></asp:DropDownList>
                                </div>
                                <span class="spdate" style="display: none;">实付日期：
                                    <asp:TextBox ID="txtdate" runat="server" CssClass="input rule-date-input" Width="100" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                                </span>
                                <input type="button" class="btn" value="提交" onclick="submitCheck()" />
                            </div>
                            <div id="payMethodDiv" style="display: none;">
                                付款方式：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlmethod1" runat="server"></asp:DropDownList>
                                </div>                                
                                <input type="button" class="btn" value="提交" onclick="submitPayMethod()" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!--/工具栏-->

            <div class="searchbar">
                付款对象：
                            <asp:TextBox ID="txtCusName" runat="server" CssClass="input"></asp:TextBox>
                <asp:HiddenField ID="hCusId" runat="server" />
                预付日期：
                            <asp:TextBox ID="txtsforedate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                -
                            <asp:TextBox ID="txteforedate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtsforedate\')}'})" />
                <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
            </div>
            <!--列表-->
            <div class="table-container">
                <asp:Repeater ID="rptList" runat="server">
                    <HeaderTemplate>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                            <tr>
                                <th width="3%">选择</th>
                                <th align="left" width="15%">付款对象</th>
                                <th align="left" width="8%">付款方式</th>
                                <th align="left" width="6%">总金额</th>
                                <th align="left" width="6%">明细数量</th>
                                <th width="5%">操作</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id="tr_<%# Eval("rpd_cid")%>_<%# Eval("rpd_method")%>">
                            <td align="center">
                                <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                                <asp:HiddenField ID="hidId" Value='<%# Eval("rpd_cid")%>' runat="server" />
                                <asp:HiddenField ID="hipPmId" Value='<%# Eval("rpd_method")%>' runat="server" />
                                <asp:HiddenField ID="hidPmName" Value='<%# Eval("pm_name")%>' runat="server" />
                            </td>
                            <td><span class="cusTip" data-cid="<%#Eval("rpd_cid")%>"><%# Eval("c_name") %></span></td>
                            <td class="paymethodTd"><%# Eval("pm_name") %></td>
                            <td class="moneyTd"><%# Eval("total") %></td>
                            <td><a href="paydetail_list.aspx?txtCusName=<%# Eval("c_name") %>&hCusId=<%# Eval("rpd_cid") %>&ddlcheck3=2"><%# Eval("c") %></a></td>
                            <td align="center">
                                <a href="javascript:;" onclick="collectDetail(<%# Eval("rpd_cid") %>,<%#Eval("rpd_method") %>)">汇总</a>
                                <span id="span_<%# Eval("rpd_cid")%>_<%# Eval("rpd_method")%>"></span>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"6\">暂无记录</td></tr>" : ""%>
                </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        <div style="font-size: 12px;color:darkblue;">
            <span style="float: left;">选中：<asp:Label ID="sCount" runat="server">0</asp:Label>条记录，付款金额：<asp:Label ID="sMoney" runat="server">0</asp:Label></span>
        </div>
        <div style="font-size: 12px;margin-top: 22px;">
            <span style="float: left;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，总计付款金额：<asp:Label ID="pMoney" runat="server">0</asp:Label></span>
            <span style="float: right;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计付款金额：<asp:Label ID="tMoney" runat="server">0</asp:Label></span>
        </div>
            <!--/列表-->
            <div class="dRemark" style="margin-top:45px;">
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
