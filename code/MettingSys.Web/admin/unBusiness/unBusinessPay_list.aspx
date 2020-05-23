<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="unBusinessPay_list.aspx.cs" Inherits="MettingSys.Web.admin.unBusiness.unBusinessPay_list" %>
<%@ Import Namespace="MettingSys.BLL" %>
<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>非业务支付申请列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript">
        $(function () {
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
                    url: "../../tools/unBusiness_ajax.ashx?action=checkStatus",
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
        /*批量确认支付 */
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
            if ($("#ddlPayStatus").val() == "" && $("#ddlPayMethod").val() == "") {
                jsprint("请选择支付状态或付款方式");
                return;
            }
            if ($("#ddlPayStatus").val() == "True") {
                if ($("#txtdate").val() == "") {
                    jsprint("请填写实付日期");
                    return;
                }
                //if ($("#ddlPayMethod").val() == "") {
                //    jsprint("请选择付款方式");
                //    return;
                //}
            }
            $(".checkall input:checked").each(function () {
                var id = $(this).parent().next().val();
                var postData = { "id": id, "status": $("#ddlPayStatus").val(), "date": $("#txtdate").val(), "method": $("#ddlPayMethod").val(), "methodName": $("#ddlPayMethod option:selected").text() };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/unBusiness_ajax.ashx?action=confirmStatus",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $("#tr" + id).children().last().children().last().html("");
                        if (data.status == 0) {                            
                            if ($("#ddlPayStatus").val() == "True") {
                                $("#tr" + id).find(".confirmTd").children().eq(0).removeClass().addClass("check_2");
                                $("#tr" + id).find(".dateTd").text($("#txtdate").val());
                            }
                            else {
                                $("#tr" + id).find(".confirmTd").children().eq(0).removeClass().addClass("check_0"); 
                                $("#tr" + id).find(".dateTd").html("");
                            }
                            if ($("#ddlPayMethod").val() != "") {
                                $("#tr" + id).find(".methodTd").text($("#ddlPayMethod option:selected").text());
                            }
                        } else {
                            $("#tr" + id).children().last().children().last().append(data.msg);
                        }
                    }
                });
            });
        }
        /*批量确认支付 */
        function selPay(obj) {
            if ($(obj).val() == "True") {
                $(obj).parent().next().attr("style", "display:inline-block;");
            }
            else {
                $(obj).parent().next().attr("style", "display:none;");
            }
        }
        //删除非业务支付申请
        function deleteUnbusinessPay(obj, id) {
            parent.dialog({
                title: '提示',
                content: "确定要删除吗？",
                okValue: '确定',
                ok: function () {                    
                    var postData = { id: id };
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/unBusiness_ajax.ashx?action=deleteUnbusiness",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.status == 0) {
                                var d = top.dialog({ content: data.msg }).show();
                                setTimeout(function () {
                                    d.close().remove();
                                    $(obj).parent().parent().remove();
                                }, 1500);
                            } else {
                                top.dialog({
                                    title: '提示',
                                    content: data.msg,
                                    okValue: '确定',
                                    ok: function () { 
                                        layer.closeAll();
                                    }
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
        //打印凭证
        function printCerti(ubaid) {
            layer.open({
                type: 2,
                title: '非业务付款凭证',
                area: ['800px', '500px'],
                content: 'printCertificate.aspx?ubaid=' + ubaid + ''
            });
        }
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
            <span>非业务支付申请列表</span>
        </div>
        <!--/导航栏-->
        <div class="content-tab-wrap" id="titleDiv" runat="server">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a <%=_check=="0"?"class=\"selected\"":"" %> href="unBusinessPay_list.aspx?check=0">全部列表</a></li>
                        <li><a <%=_check=="1"?"class=\"selected\"":"" %> href="unBusinessPay_list.aspx?check=1">部门未审批</a></li>
                        <li><a <%=_check=="2"?"class=\"selected\"":"" %> href="unBusinessPay_list.aspx?check=2">财务未审批</a></li>
                        <li><a <%=_check=="3"?"class=\"selected\"":"" %> href="unBusinessPay_list.aspx?check=3">总经理未审批</a></li>
                        <li><a <%=_check=="4"?"class=\"selected\"":"" %> href="unBusinessPay_list.aspx?check=4">审完未付款</a></li>
                        <li><a <%=_check=="5"?"class=\"selected\"":"" %> href="unBusinessPay_list.aspx?check=5">已付款</a></li>
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
                                <li><a href="unBusinessPay_edit.aspx?action=<%=DTEnums.ActionEnum.Add %>"><i class="iconfont icon-close"></i><span>新增</span></a></li>
                                <li><a href="javascript:;" onclick="checkAll(this);computeSelect();"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                                <li><a href="javascript:;" onclick="reverseCheckAll();computeSelect();"><i class="iconfont icon-check-mark"></i><span>反选</span></a></li>
                                <li>
                                    <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete','如果该非业务支付申请已被审批则无法删除，是否继续？');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                                <li id="liCheck" runat="server"><a href="javascript:;" onclick="toggleCheckDiv()"><span>审批</span></a></li>
                                <li id="liConfirm" runat="server"><a href="javascript:;" onclick="toggleconfirmDiv()"><span>支付操作</span></a></li>
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
                            <div id="confirmDiv" style="display: none;">
                                支付状态：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlPayStatus" runat="server" onchange="selPay(this)">
                                    </asp:DropDownList>
                                </div>
                                <div style="display: none;">
                                    实付日期：<asp:TextBox ID="txtdate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                                </div>
                                付款方式：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlPayMethod" runat="server">
                                    </asp:DropDownList>
                                </div>
                                <input type="button" class="btn" value="提交" onclick="submitConfirm()" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!--/工具栏-->

            <div class="searchbar">
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
                支付状态：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlConfirm" runat="server">
                                    </asp:DropDownList>
                                </div>
                预付日期：
                            <asp:TextBox ID="txtsforedate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                -
                            <asp:TextBox ID="txteforedate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtsforedate\')}'})" />
                实付日期：
                            <asp:TextBox ID="txtsdate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                -
                            <asp:TextBox ID="txtedate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtsdate\')}'})" />
                区域：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlarea" runat="server"></asp:DropDownList>
                                </div>
                <input type="hidden" name="self" value="<%=_self %>" />
                <input type="hidden" name="check" value="<%=_check %>" />
                <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                <asp:LinkButton ID="btnExcel" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>
            </div>
            <div class="searchbar" style="margin-top:10px;">
                支付类别：
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddltype" runat="server"></asp:DropDownList>
                    </div>
                支付用途：<asp:TextBox ID="txtfunction" runat="server" CssClass="input" Width="120"/>
                申请人：<asp:TextBox ID="txtOwner" runat="server" CssClass="input" Width="120"/>
                付款方式：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlPayMethod1" runat="server"></asp:DropDownList>
                            </div>
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
                收款账户：
                <asp:TextBox ID="txtBankName" runat="server" CssClass="input"></asp:TextBox>
            </div>
            <!--列表-->
            <div class="table-container">
                <asp:Repeater ID="rptList" runat="server">
                    <HeaderTemplate>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                            <tr>
                                <th width="3%">选择</th>
                                <th align="left" width="5%">支付类别</th>
                                <th align="left" width="10%">支付用途</th>
                                <th align="left" width="6%">订单号</th>
                                <th align="left">用途说明</th>
                                <th align="left" width="8%">收款账户</th>
                                <th align="left" width="10%">收款账号</th>
                                <th align="left" width="5%">收款银行</th>
                                <th align="left" width="5%">金额</th>
                                <th align="left" width="5%">预付日期</th>
                                <th align="left" width="5%">实付日期</th>
                                <th align="left" width="5%">付款方式</th>
                                <th align="left" width="5%">申请人</th>
                                <th align="left" width="5%">审批</th>
                                <th align="left" width="3%">支付确认</th>
                                <th width="8%">操作</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id="tr<%#Eval("uba_id")%>">
                            <td align="center">
                                <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                                <asp:HiddenField ID="hidId" Value='<%#Eval("uba_id")%>' runat="server" />
                            </td>
                            <td><%# BusinessDict.unBusinessNature()[Convert.ToByte(Eval("uba_type"))]%></td>
                            <td><%#Eval("uba_function") %></td>
                            <td><a href="../order/order_edit.aspx?action=<%# DTEnums.ActionEnum.Edit.ToString() %>&oID=<%#Eval("uba_oid")%>"><span class="orderstatus_<%#Eval("o_status")%>"><%#Eval("uba_oid") %></span></a></td>
                            <td style="padding-right:10px;"><%#Eval("uba_instruction") %></td>
                            <td><%#Eval("uba_receiveBankName") %></td>
                            <td><%#Eval("uba_receiveBankNum") %></td>
                            <td><%#Eval("uba_receiveBank") %></td>
                            <td class="moneyTd"><%#Eval("uba_money") %></td>
                            <td><%#DateTime.Parse(Eval("uba_foreDate").ToString()).ToString("yyyy-MM-dd") %></td>
                            <td class="dateTd"><%#MettingSys.Common.ConvertHelper.toDate(Eval("uba_Date")) == null ? "" : DateTime.Parse(Eval("uba_Date").ToString()).ToString("yyyy-MM-dd") %></td>
                            <td class="methodTd"><%#Eval("pm_name") %></td>
                            <td><span title="申请工号：<%#Eval("uba_personNum")%>&#10;申请时间：<%#Eval("uba_addDate")%>"><%#Eval("uba_personName") %></span></td>
                            <td class="checkTd">
                                <span onmouseover="tip_index=layer.tips('部门审批<br/>审批人：<%#Eval("uba_checkNum1")%>-<%#Eval("uba_checkName1")%><br/>审批备注：<%#Eval("uba_checkRemark1").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%><br/>审批时间：<%#Eval("uba_checkTime1")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("uba_flag1")%>"></span>
                                <span onmouseover="tip_index=layer.tips('财务审批<br/>审批人：<%#Eval("uba_checkNum2")%>-<%#Eval("uba_checkName2")%><br/>审批备注：<%#Eval("uba_checkRemark2").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%><br/>审批时间：<%#Eval("uba_checkTime2")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("uba_flag2")%>"></span>
                                <span onmouseover="tip_index=layer.tips('总经理审批<br/>审批人：<%#Eval("uba_checkNum3")%>-<%#Eval("uba_checkName3")%><br/>审批备注：<%#Eval("uba_checkRemark3").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%><br/>审批时间：<%#Eval("uba_checkTime3")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("uba_flag3")%>"></span>
                            </td>
                            <td class="confirmTd">
                                <span class="check_<%#Utils.StrToBool(Utils.ObjectToStr(Eval("uba_isConfirm")),false)? "2" : "0"%>"></span><span title="<%#Eval("uba_confirmerNum")%>"><%#Eval("uba_confirmerName")%></span>
                            </td>
                            <td align="center">
                                <!--存在审批失败的，或者部门审批是待审批的都可以编辑，其他情况只能查看-->
                                <%#(Eval("uba_flag1").ToString() == "1" ||Eval("uba_flag2").ToString() == "1" ||Eval("uba_flag3").ToString() == "1" ) || Eval("uba_flag1").ToString() == "0" ?"<a href=\"unBusinessPay_edit.aspx?action="+DTEnums.ActionEnum.Edit+"&id="+Eval("uba_id")+"\">修改</a>":"<a href=\"unBusinessPay_edit.aspx?action="+DTEnums.ActionEnum.View+"&id="+Eval("uba_id")+"\">查看</a>"%>
                                <%# Eval("uba_flag3").ToString() != "2" ?"<a href=\"javascript:\" onclick=\"deleteUnbusinessPay(this,"+Eval("uba_id")+")\">删除</a>":""%>
                                <%# (manager.area == Eval("uba_area").ToString() && new MettingSys.BLL.permission().checkHasPermission(manager, "0603")) || new MettingSys.BLL.permission().checkHasPermission(manager, "0402,0601") ?"<a href=\"unBusinessPay_edit.aspx?action="+DTEnums.ActionEnum.View+"&id="+Eval("uba_id")+"\">审批</a>":""%>
                                <%#Utils.StrToBool(Utils.ObjectToStr(Eval("uba_isConfirm")),false)? "<a href=\"javascript:void(0);\" onclick=\"printCerti("+Eval("uba_id")+")\">凭证</a>" : ""%>
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
                <span style="float: left;">选中：<asp:Label ID="sCount" runat="server">0</asp:Label>条记录，总计金额：<asp:Label ID="sMoney" runat="server">0</asp:Label></span>
            </div>
            <div style="font-size: 12px;margin-top: 20px;">
                <span style="float: left;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，总计金额：<asp:Label ID="pMoney" runat="server">0</asp:Label></span>
                <span style="float: right;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计金额：<asp:Label ID="tMoney" runat="server">0</asp:Label></span>
            </div>
            <!--/列表-->
            <div class="dRemark" style="margin-top:40px;">
                <p>1.审批：<span class="check_0"></span>待审批，<span class="check_1"></span>审批未通过，<span class="check_2"></span>审批通过</p>
                <p>2.支付：<span class="check_0"></span>待支付，<span class="check_2"></span>已支付</p>
                <p>3.查询时“收款账户”填0，则搜索收款账户为空的记录
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
