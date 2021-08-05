<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AchievementStatistic_Detail.aspx.cs" Inherits="MettingSys.Web.admin.statistic.AchievementStatistic_Detail" %>

<%@ Import Namespace="MettingSys.Common" %>
<%@ Import Namespace="MettingSys.BLL" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>区域业绩明细</title>
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
    <form id="ajaxForm" runat="server" action="AchievementStatistic_Detail.aspx" method="post" enctype="multipart/form-data">
        <!--导航栏-->
        <div class="location" style="margin-bottom:10px;">
            <a href="javascript:history.back(-1);" class="back"><i class="iconfont icon-up"></i><span>返回上一页</span></a>
            <a href="../center.aspx" class="home"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>员工业绩明细</span>
        </div>
        <!--/导航栏-->

        <div class="tab-content" style="padding-top: 0;">
        <div class="searchbar">
            <div class="menu-list" style="margin-bottom: 10px;">
                订 单 号 ：
                        <asp:TextBox ID="txtOrderID" runat="server" CssClass="input"></asp:TextBox>
                客&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;户：
                        <asp:TextBox ID="txtCusName" runat="server" CssClass="input"></asp:TextBox>
                活动名称：
                        <asp:TextBox ID="txtContent" runat="server" CssClass="input"></asp:TextBox>
                活动地点：
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="input"></asp:TextBox>
                订单结束日期月份：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM',maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
                -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM',minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
            </div>
        </div>
        <div class="searchbar">
            <div class="menu-list" style="margin-bottom: 10px;">
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
                业务员：
                    <asp:TextBox ID="txtUser" runat="server" CssClass="input" ></asp:TextBox>
                包含税费成本：
                    <div class="rule-single-checkbox">
                        <asp:CheckBox ID="cbIsCust" runat="server" Checked="true" />
                    </div>                
                排序：
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlorderType" runat="server">
                            <asp:ListItem Value="(shou-fu-oCust+ticheng)">提成前业绩</asp:ListItem>
                            <asp:ListItem Value=" case when (shou-unIncome)<>0 then (shou-fu-oCust+ticheng)*100/(shou-unIncome) else 0 end ">提成前业绩率</asp:ListItem>
                            <asp:ListItem Value="(shou-fu-oCust)">提成后业绩</asp:ListItem>
                            <asp:ListItem Value=" case when (shou-unIncome)<>0 then (shou-fu-oCust)*100/(shou-unIncome) else 0 end ">提成后业绩率</asp:ListItem>
                            <asp:ListItem Value="shou-unIncome">应收-非考核收入</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlorder" runat="server">
                            <asp:ListItem Value="Asc">升序</asp:ListItem>
                            <asp:ListItem Value="Desc">降序</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                <input type="hidden" name="action" value="Search" />
                <input <%--id="btnSave"--%> type="submit" class="btn" value="查询" />
                <a href="<%=Utils.CombUrlTxt("AchievementStatistic_Detail.aspx", "Excel={0}&txtsDate={1}&txteDate={2}&ddllock={3}&ddlstatus={4}&ddlarea={5}&cbIsCust={6}&action={7}&ddlorderType={8}&ddlorder={9}&txtOrderID={10}&txtCusName={11}&txtContent={12}&txtAddress={13}&txtUser={14}", "on", _sMonth, _eMonth, _lockstatus, _status, _area, _isCust,action,_ordertype,_order,_orderNum,_cusName,_content,_address,_user) %>"><i class="iconfont icon-exl"></i><span>导出Excel</span></a>
            </div>
        </div>

        <!--列表-->
        <div class="table-container">
            <asp:Repeater ID="rptList" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr style="text-align: left;">
                                <th width="6%">订单号</th>
                                <th>活动名称/地点</th>
                                <th width="8%">客户</th>
                                <th width="6%">活动日期</th>
                                <th width="4%">业务员</th>
                                <th width="6%">应收</th>
                                <th width="6%">非考核收入</th>
                                <th width="6%">应付</th>
                                <th width="6%">非考核成本</th>
                                <th width="6%">提成</th>
                                <th width="6%">税费</th>
                                <th width="6%">提成前业绩</th>
                                <th width="6%">提成前业绩率</th>
                                <th width="6%">提成后业绩</th>
                                <th width="6%">提成后业绩率</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><a href="../order/order_edit.aspx?action=<%# DTEnums.ActionEnum.Edit.ToString() %>&oID=<%#Eval("o_id")%>"><%# Eval("o_id") %></a></td>
                        <td><%# Eval("o_content") %><br /><%# Eval("o_address") %></td>
                        <td><%# Eval("c_name") %></td>
                        <td><%#ConvertHelper.toDate(Eval("o_sdate")).Value.ToString("yyyy-MM-dd")%><br /><%#ConvertHelper.toDate(Eval("o_edate")).Value.ToString("yyyy-MM-dd")%></td>
                        <td><%# Eval("op_name") %>(<%# Eval("op_ratio") %>%)</td>
                        <td><%# Eval("shou") %></td>
                        <td><%# Eval("unIncome") %></td>
                        <td><%# Eval("fu")%></td>
                        <td><%# Eval("unCost") %></td>
                        <td><%# Eval("ticheng") %></td>
                        <td><%# Eval("oCust") %></td>
                        <td><%# Eval("profit1") %></td>
                        <td><%# Eval("profitRatio1") %>%</td>
                        <td><%# Eval("profit2") %></td>
                        <td><%# Eval("profitRatio2") %>%</td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"15\">暂无记录</td></tr>" : ""%>
  </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <!--/列表-->
        <div style="font-size: 12px; line-height: 1.6em;">
            <span style="display:block;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，应收总额：<asp:Label ID="pShou" runat="server">0</asp:Label>，非考核收入：<asp:Label ID="pUnIncome" runat="server">0</asp:Label>，应付总额：<asp:Label ID="pFu" runat="server">0</asp:Label>，非考核成本：<asp:Label ID="pUnCost" runat="server">0</asp:Label>，订单税费：<asp:Label ID="pCust" runat="server">0</asp:Label>，提成：<asp:Label ID="pTicheng" runat="server">0</asp:Label>，提成前业绩：<asp:Label ID="pProfit1" runat="server">0</asp:Label>，提成后业绩：<asp:Label ID="pProfit2" runat="server">0</asp:Label></span>
            <span style="display:block;float: left;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，应收总额：<asp:Label ID="tShou" runat="server">0</asp:Label>，非考核收入：<asp:Label ID="tUnIncome" runat="server">0</asp:Label>，应付总额：<asp:Label ID="tFu" runat="server">0</asp:Label>，非考核成本：<asp:Label ID="tUnCost" runat="server">0</asp:Label>，订单税费：<asp:Label ID="tCust" runat="server">0</asp:Label>，提成：<asp:Label ID="tTicheng" runat="server">0</asp:Label>，提成前业绩：<asp:Label ID="tProfit1" runat="server">0</asp:Label>，提成后业绩：<asp:Label ID="tProfit2" runat="server">0</asp:Label></span>
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