<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="paydetail_list.aspx.cs" Inherits="MettingSys.Web.admin.finance.paydetail_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>付款明细列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <style type="text/css">
    </style>
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
            //显示付款明细汇总列表的工具栏
            $(".toolbar").removeClass("mini").find(".l-list").attr("display", "block");

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
            $("#checkDiv").toggle();
            $("#payMethodDiv").hide();
        }
        function togglePayMethodDiv() {
            $("#payMethodDiv").toggle();
            $("#checkDiv").hide();
        }
        //批量审批
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
                    url: "../../tools/Business_ajax.ashx?action=checkPayDetailStatus",
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
        //取消汇总
        function cancelCollect(rpdid) {
            var postData = { "rpdid": rpdid };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/Business_ajax.ashx?action=cancelCollect",
                data: postData,
                dataType: "json",
                success: function (data) {
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
                            ok: function () { }
                        }).showModal();
                    }
                }
            });
        }
        //批量取消汇总
        function mutlUnCollect() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            var idStr = "";
            $(".checkall input:checked").each(function () {
                idStr += $(this).parent().next().val() + ",";
            });
            idStr = idStr.substring(0, idStr.length - 1);

            var postData = { "ids": idStr };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/Business_ajax.ashx?action=mutlUnCollect",
                data: postData,
                dataType: "json",
                success: function (data) {
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
                            ok: function () { }
                        }).showModal();
                    }
                }
            });
        }
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
            if ($("#ddlmethod").val() == "") {
                jsprint("请选择付款方式");
                return;
            }
            $(".checkall input:checked").each(function () {
                var id = $(this).parent().next().val();
                var postData = { "id": id, "method": $("#ddlmethod").val()};
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=setPayMethod",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $("#tr" + id).children().last().children().last().html("");
                        if (data.status == 0) {
                            $("#tr" + id).find(".paymethodTd").text($("#ddlmethod option:selected").text());
                        } else {
                            $("#tr" + id).children().last().children().last().append(data.msg);
                        }
                    }
                });
            });
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
            <span>付款明细列表</span>
        </div>
        <!--/导航栏-->
        <div class="content-tab-wrap" id="titleDiv" runat="server">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a <%=_check=="0"?"class=\"selected\"":"" %> href="paydetail_list.aspx?check=0">付款明细列表</a></li>
                        <li><a <%=_check=="1"?"class=\"selected\"":"" %> href="paydetail_list.aspx?check=1">部门未审批</a></li>
                        <li><a <%=_check=="2"?"class=\"selected\"":"" %> href="paydetail_list.aspx?check=2">财务未审批</a></li>
                        <li><a <%=_check=="3"?"class=\"selected\"":"" %> href="paydetail_list.aspx?check=3">总经理未审批</a></li>
                        <li><a href="paydetailcollect.aspx">付款明细汇总列表</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="tab-content" style="padding-top: 0;">
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
                                <li id="checkLi" runat="server"><a href="javascript:;" onclick="toggleCheckDiv()"><span>审批</span></a></li>
                                <li id="uncollectLi" runat="server"><a href="javascript:;" onclick="mutlUnCollect()"><span>取消汇总</span></a></li>
                                <li id="paymethodLi" runat="server"><a href="javascript:;" onclick="togglePayMethodDiv()"><span>填写付款方式</span></a></li>
                            </ul>
                            <div id="checkDiv" style="display: none;">
                                审批类型：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlchecktype" runat="server">
                                        <asp:ListItem Value="">请选择</asp:ListItem>
                                        <asp:ListItem Value="1">部门审批</asp:ListItem>
                                        <asp:ListItem Value="2">财务审批</asp:ListItem>
                                        <asp:ListItem Value="3">总经理审批</asp:ListItem>
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
                            <div id="payMethodDiv" style="display: none;">
                                付款方式：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlmethod" runat="server"></asp:DropDownList>
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
                部门审批：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck1" runat="server"></asp:DropDownList>
                            </div>
                财务审批：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck2" runat="server"></asp:DropDownList>
                            </div>
                总经理审批：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck3" runat="server"></asp:DropDownList>
                            </div>
                预付日期：
                            <asp:TextBox ID="txtforesdate" runat="server" CssClass="input rule-date-input" Width="100" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                -
                            <asp:TextBox ID="txtforeedate" runat="server" CssClass="input rule-date-input" Width="100" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtforesdate\')}'})" />
                汇总状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcollect" runat="server">
                                    <asp:ListItem Value="">不限</asp:ListItem>
                                    <asp:ListItem Value="False">未汇总</asp:ListItem>
                                    <asp:ListItem Value="True">已汇总</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                申请人：
                           <asp:TextBox ID="txtPerson" runat="server" CssClass="input small"></asp:TextBox> 
                <input type="hidden" name="self" value="<%=_self %>" />
                金额：
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
                
            </div>
            <div class="searchbar">
                付款人：
                           <asp:TextBox ID="txtPerson1" runat="server" CssClass="input small"></asp:TextBox> 
                实付日期：
                            <asp:TextBox ID="txtsdate" runat="server" CssClass="input rule-date-input" Width="100" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                -
                            <asp:TextBox ID="txtedate" runat="server" CssClass="input rule-date-input" Width="100" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtsdate\')}'})" />
                订单号：
                            <asp:TextBox ID="txtorderid" runat="server" CssClass="input" Width="100" />
                区域：
                <div class="rule-single-select">
                    <asp:DropDownList ID="ddlarea" runat="server"></asp:DropDownList>
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
                                <th width="3%">选择</th>
                                <th align="left" width="8%">订单号</th>
                                <th align="left" width="8%">付款对象</th>
                                <th align="left" width="10%">客户银行账号</th>
                                <th align="left">付款内容</th>
                                <th align="left" width="6%">付款金额</th>
                                <th align="left" width="8%">预付日期</th>
                                <th align="left" width="8%">付款方式</th>
                                <th align="left" width="8%">申请人</th>
                                <th align="left" width="5%">区域</th>
                                <th align="left" width="5%">审批</th>                    
                                <th align="left" width="5%">付款人</th>
                                <th align="left" width="6%">实付日期</th>
                                <th width="5%">操作</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id="tr<%#Eval("rpd_id")%>">
                            <td align="center">
                                <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                                <asp:HiddenField ID="hidId" Value='<%#Eval("rpd_id")%>' runat="server" />
                            </td>
                            <td><a href="../order/order_edit.aspx?action=<%# DTEnums.ActionEnum.Edit.ToString() %>&oID=<%#Eval("rpd_oid")%>"><span class="orderstatus_<%#Eval("o_status")%>"><%#Eval("rpd_oid")%></span></a></td>
                            <td><%#Eval("c_name")%></td>
                            <td><%#Eval("cb_bankName")%><br /><%#Eval("cb_bankNum")%><br /><%#Eval("cb_bank")%></td>
                            <td><%#Eval("rpd_content")%></td>
                            <td class="moneyTd"><%#Eval("rpd_money")%></td>
                            <td><%# Convert.ToDateTime(Eval("rpd_foredate")).ToString("yyyy-MM-dd") %></td>
                            <td class="paymethodTd"><%#Eval("pm_name")%></td>
                            <td style="color:#2A72C5;"><span title="申请工号：<%#Eval("rpd_personNum")%>&#10;申请时间：<%#Eval("rpd_addDate")%>"><%#Eval("rpd_personName") %></span></td>
                            <td><%#Eval("rpd_area")%></td>
                            <td class="checkTd">
                                <span onmouseover="tip_index=layer.tips('部门审批<br/>审批人：<%#Eval("rpd_checkNum1")%>-<%#Eval("rpd_checkName1")%><br/>审批备注：<%#Eval("rpd_checkRemark1").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%><br/>审批时间：<%#Eval("rpd_checkTime1")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("rpd_flag1")%>"></span>
                                <span onmouseover="tip_index=layer.tips('财务审批<br/>审批人：<%#Eval("rpd_checkNum2")%>-<%#Eval("rpd_checkName2")%><br/>审批备注：<%#Eval("rpd_checkRemark2").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%><br/>审批时间：<%#Eval("rpd_checkTime2")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("rpd_flag2")%>"></span>
                                <span onmouseover="tip_index=layer.tips('总经理审批<br/>审批人：<%#Eval("rpd_checkNum3")%>-<%#Eval("rpd_checkName3")%><br/>审批备注：<%#Eval("rpd_checkRemark3").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%><br/>审批时间：<%#Eval("rpd_checkTime3")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("rpd_flag3")%>"></span>
                            </td>
                            <td><%#Eval("rp_confirmerName")%></td>
                            <td><%# ConvertHelper.toDate(Eval("rp_date"))==null?"":Convert.ToDateTime(Eval("rp_date")).ToString("yyyy-MM-dd") %></td>
                            <td align="center">
                                <!--存在审批失败的，或者部门审批是待审批的都可以编辑，其他情况只能查看-->
                                <%#(Eval("rpd_flag1").ToString() == "1" ||Eval("rpd_flag2").ToString() == "1" ||Eval("rpd_flag3").ToString() == "1" ) || Eval("rpd_flag1").ToString() == "0" || Utils.ObjToDecimal(Eval("rpd_money"),0)<0 ?"<a href=\"paydetail_edit.aspx?action="+DTEnums.ActionEnum.Edit+"&id="+Eval("rpd_id")+"\">修改</a>":"<a href=\"paydetail_edit.aspx?action="+DTEnums.ActionEnum.View+"&id="+Eval("rpd_id")+"\">查看</a>"%>
                                <%# (manager.area == Eval("rpd_area").ToString() && new MettingSys.BLL.permission().checkHasPermission(manager, "0603")) || new MettingSys.BLL.permission().checkHasPermission(manager, "0402,0601") ?"<a href=\"paydetail_edit.aspx?action="+DTEnums.ActionEnum.View+"&id="+Eval("rpd_id")+"\">审批</a>":""%>
                                <%#(Utils.ObjToInt(Eval("rpd_rpid"))>0 && !Utils.StrToBool(Utils.ObjectToStr(Eval("rp_isExpect")),false))?"<a href=\"javascript:;\" onclick=\"cancelCollect("+Eval("rpd_id")+")\">取消汇总</a>":"" %>
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
            <span style="float: left;">选中：<asp:Label ID="sCount" runat="server">0</asp:Label>条记录，付款金额：<asp:Label ID="sMoney" runat="server">0</asp:Label></span>
        </div>
        <div style="font-size: 12px;margin-top: 22px;">
            <span style="float: left;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，总计付款金额：<asp:Label ID="pMoney" runat="server">0</asp:Label></span>
            <span style="float: right;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计付款金额：<asp:Label ID="tMoney" runat="server">0</asp:Label></span>
        </div>
            <!--/列表-->
            <div class="dRemark" style="margin-top:45px;">
                <p>1.关键字筛查字段为：订单号、付款对象、付款内容</p>
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
        </div>
        <div class="tab-content" style="padding-top: 0; display: none;">
        </div>
    </form>
</body>
</html>
