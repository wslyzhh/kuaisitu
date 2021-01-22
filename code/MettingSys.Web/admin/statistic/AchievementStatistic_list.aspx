<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AchievementStatistic_list.aspx.cs" Inherits="MettingSys.Web.admin.statistic.AchievementStatistic_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<%@ Import Namespace="MettingSys.BLL" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>业绩统计</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery.form.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript">
        $(function () {
            //绑定活动归属地
            $("#specAddButton").click(function () {
                var liObj = $(this).parent();
                var d = top.dialog({
                    id: 'specDialogId',
                    padding: 0,
                    title: "区域",
                    url: 'admin/order/order_place.aspx'
                }).showModal();
                //将容器对象传进去
                d.data = liObj;
            });
            var ajaxFormOption = {
                dataType: "json", //数据类型  
                success: function (data) { //提交成功的回调函数  
                    layer.closeAll();
                    if (data.status == 0) {
                        //var d = top.dialog({ content: "订单保存成功,正在跳转页面..." }).show();
                        //setTimeout(function () {
                        //    d.close().remove();
                        //    location.href = 'order_edit.aspx?action=Edit&oID=' + data.msg;
                        //}, 2000);
                    } else {
                        //top.dialog({
                        //    title: '提示',
                        //    content: data.msg,
                        //    okValue: '确定',
                        //    ok: function () {
                        //        layer.closeAll();
                        //    }
                        //}).showModal();
                    }
                }
            };
            //提交订单
            $("#btnSave").click(function () {
                //printLoad();
                $("#ajaxForm").ajaxSubmit(ajaxFormOption);
            });
        })
        //人员选择
        function chooseEmployee(obj, n, flag, isShow) {
            //业务报账员和业务执行人员必须在选择活动归属地后才能选择
            var area = "";
            var liObj = $(obj).parent();
            var d = top.dialog({
                id: 'specDialogId',
                padding: 0,
                title: "选择员工",
                url: 'admin/selectEmployee.aspx?area=' + area + ''
            }).showModal();
            //将容器对象传进去
            d.data = liObj;
            d.n = n;//容器序号，一个页面上有多个容器时要传
            d.multi = flag;//true可以选择多个人,false只能选一个人
            d.showDstatus = isShow;//true显示接单状态，false不显示接单状态
        }
        //删除附件节点
        function delNode(obj) {
            $(obj).parent().remove();
        }
        //
        function toOrderAnalyze(area, num) {
            var type = $("#ddltype").val();
            var str = "";
            if (type == "0") {
                str = "&txtPerson1=" + num + "";
            }
            else if (type == "1") {
                str = "&txtPerson3=" + num + "";
            }
            else {
                str = "&txtPerson5=" + num + "";
            }

            location.href = "OrderAnalyze_list.aspx?statistic=0&txtsDate1=" + $('#txtsDate').val() + "&txteDate1=" + $('#txteDate').val() + "&ddlstatus=" + $('#ddlstatus').val() + "&ddllock=" + $('#ddllock').val() + "" + str + "";
        }
        function toFinanceList(ftype, area, num, detail) {
            var type = $("#ddltype").val();
            var str = "";
            if (type == "0") {
                str = "&txtPerson1=" + num + "";
            }
            else if (type == "1") {
                str = "&txtPerson3=" + num + "";
            }
            else {
                str = "&txtPerson5=" + num + "";
            }
            if ($("#cbIsRemove").attr("checked") == "checked") {
                str += "&isRemove=1";
            }
            location.href = "../finance/finance_list.aspx?type=" + ftype + "&txtOsdate=" + $('#txtsDate').val() + "&txtOedate=" + $('#txteDate').val() + "&ddlstatus=" + $('#ddlstatus').val() + "&ddllock=" + $('#ddllock').val() + "&txtDetails=" + escape(detail) + "" + str + "";
        }
    </script>
    <style type="text/css">
        .date-input {
            width: 100px;
        }

        .myRuleSelect .select-tit {
            padding: 5px 5px 7px 5px;
        }

        .txt-item {
            position: relative;
            display: inline-block;
            margin-right: 5px;
            vertical-align: middle;
            cursor: pointer;
        }

            .txt-item li {
                background-color: #e1e1e1;
            }

                .txt-item li.icon-btn i {
                    color: #fff;
                }

                .txt-item li.icon-btn {
                    background-color: #16a0d3;
                }
    </style>
</head>

<body class="mainbody">
    <form id="ajaxForm" runat="server" action="AchievementStatistic_list.aspx" method="post" enctype="multipart/form-data">
        <!--导航栏-->
        <div class="location" style="margin-bottom: 10px;">
            <a href="javascript:history.back(-1);" class="back"><i class="iconfont icon-up"></i><span>返回上一页</span></a>
            <a href="../center.aspx" class="home"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>员工业绩统计</span>
        </div>
        <!--/导航栏-->

        <div class="tab-content" style="padding-top: 0;">
            <div class="searchbar">
                <div class="menu-list" style="margin-bottom: 10px;">
                    订单结束日期月份：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM',maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
                    -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM',minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
                    订单状态：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlstatus" runat="server"></asp:DropDownList>
                                </div>
                    锁单状态：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddllock" runat="server"></asp:DropDownList>
                                </div>
                    区域：
                    <div class="txt-item">
                        <ul>
                            <asp:Repeater ID="rptAreaList" runat="server">
                                <ItemTemplate>
                                    <li>
                                        <input name="hide_place" type="hidden" value="<%#Eval("Key")%>" />
                                        <a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                        <span><%#Eval("Value")%></span>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="icon-btn" id="liplace" runat="server">
                                <a id="specAddButton"><i class="iconfont icon-close"></i></a>
                            </li>
                        </ul>
                    </div>
                    员工：
                    <div class="txt-item">
                        <ul>
                            <asp:Repeater ID="rptEmployee2" runat="server">
                                <ItemTemplate>
                                    <li title="<%#Eval("[\"op_number\"]")%>">
                                        <input name="hide_employee3" type="hidden" value="<%#Eval("[\"op_name\"]")%>|<%#Eval("[\"op_number\"]")%>|<%#Eval("[\"op_area\"]")%>" />
                                        <a href="javascript:;" style="<%=_self=="1"?"display:none;": "" %>" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                        <span><%#Eval("[\"op_name\"]")%></span>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="icon-btn" id="liemployee3" runat="server">
                                <a href="javascript:" onclick="chooseEmployee(this,3,true)"><i class="iconfont icon-close"></i></a>
                            </li>
                        </ul>
                    </div>
                    排除员工提成：
                    <div class="rule-single-checkbox">
                        <asp:CheckBox ID="cbIsRemove" runat="server" />
                    </div>
                    包含税费成本：
                    <div class="rule-single-checkbox">
                        <asp:CheckBox ID="cbIsCust" runat="server" Checked="true" />
                    </div>
                    统计性质：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddltype" runat="server">
                                <asp:ListItem Value="0">下单</asp:ListItem>
                                <asp:ListItem Value="1">接单(业务策划)</asp:ListItem>
                                <asp:ListItem Value="2">接单(业务设计)</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    <%--排序：
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddlOrderName" runat="server">
                                <asp:ListItem Value="">不限</asp:ListItem>
                                <asp:ListItem Value="1">工号</asp:ListItem>
                                <asp:ListItem Value="2">应收</asp:ListItem>
                                <asp:ListItem Value="3">业绩利润</asp:ListItem>
                                <asp:ListItem Value="4">利润率</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddlOrderValue" runat="server">
                                <asp:ListItem Value="">不限</asp:ListItem>
                                <asp:ListItem Value="1">从大到小</asp:ListItem>
                                <asp:ListItem Value="2">从小到大</asp:ListItem>
                            </asp:DropDownList>
                        </div>--%>
                    <input type="hidden" name="action" value="Search" />
                    <input type="hidden" name="self" value="<%=_self %>" />
                    <input <%--id="btnSave"--%> type="submit" class="btn" value="查询" />
                    <a href="<%=Utils.CombUrlTxt("AchievementStatistic_list.aspx", "Excel={0}&txtsDate={1}&txteDate={2}&ddllock={3}&ddlstatus={4}&hide_place={5}&hide_employee3={6}&cbIsRemove={7}&cbIsCust={8}&ddltype={9}", "on", _sMonth, _eMonth, _lockstatus, _status, _area,_person, _isRemove, _isCust,_type) %>"><i class="iconfont icon-exl"></i><span>导出Excel</span></a>
                </div>
            </div>

            <!--列表-->
            <div class="table-container">
                <asp:Repeater ID="rptList" runat="server">
                    <HeaderTemplate>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                            <tr style="text-align: left;">
                                <th width="10%">员工</th>
                                <th width="10%">区域</th>
                                <th width="10%">订单数量</th>
                                <th width="10%">应收</th>
                                <th width="10%">非考核收入</th>
                                <th width="10%">应付</th>
                                <th width="10%">非考核成本</th>
                                <th width="10%">订单利润</th>
                                <th width="10%">订单税费</th>
                                <th>业绩利润</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("op_number") %>-<%# Eval("op_name") %></td>
                            <td><%#Eval("op_area")%>-<%# new department().getAreaText(Eval("op_area").ToString())%></td>
                            <td><a onclick="toOrderAnalyze('<%#Eval("op_area")%>','<%# Eval("op_number") %>')" href="javascript:void(0);"><%# Eval("oCount") %></a></td>
                            <td><a onclick="toFinanceList(true,'<%#Eval("op_area")%>','<%# Eval("op_number") %>','')" href="javascript:void(0);"><%# Eval("shou") %></a></td>
                            <td><a onclick="toFinanceList(true,'<%#Eval("op_area")%>','<%# Eval("op_number") %>','代收代付')" href="javascript:void(0);"><%# Eval("unIncome") %></a></td>
                            <td><a onclick="toFinanceList(false,'<%#Eval("op_area")%>','<%# Eval("op_number") %>','')" href="javascript:void(0);"><%# Eval("fu") %></a></td>
                            <td><a onclick="toFinanceList(false,'<%#Eval("op_area")%>','<%# Eval("op_number") %>','代收代付')" href="javascript:void(0);"><%# Eval("unCost") %></a></td>
                            <td><%# Eval("oProfit") %></td>
                            <td><%# Eval("oCust")%></td>
                            <td><%# Eval("bProfit") %></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"10\">暂无记录</td></tr>" : ""%>
  </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <!--/列表-->
            <div style="font-size: 12px; line-height: 1.6em;">
                <span style="display: block;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，合计订单数量：<asp:Label ID="pOrderCount" runat="server">0</asp:Label>，订单应收：<asp:Label ID="pOrderShou" runat="server">0</asp:Label>，非考核收入：<asp:Label ID="pUnIncome" runat="server">0</asp:Label>，订单应付：<asp:Label ID="pOrderFu" runat="server">0</asp:Label>，非考核成本：<asp:Label ID="pUnCost" runat="server">0</asp:Label>，订单利润：<asp:Label ID="pOrderProfit" runat="server">0</asp:Label>，订单税费：<asp:Label ID="pCust" runat="server">0</asp:Label>，业绩利润：<asp:Label ID="pProfit" runat="server">0</asp:Label></span>
                <span style="display: block; float: left;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，合计订单数量：<asp:Label ID="tOrderCount" runat="server">0</asp:Label>，订单应收：<asp:Label ID="tOrderShou" runat="server">0</asp:Label>，非考核收入：<asp:Label ID="tUnIncome" runat="server">0</asp:Label>，订单应付：<asp:Label ID="tOrderFu" runat="server">0</asp:Label>，非考核成本：<asp:Label ID="tUnCost" runat="server">0</asp:Label>，订单利润：<asp:Label ID="tOrderProfit" runat="server">0</asp:Label>，订单税费：<asp:Label ID="tCust" runat="server">0</asp:Label>，业绩利润：<asp:Label ID="tProfit" runat="server">0</asp:Label></span>
            </div>
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

