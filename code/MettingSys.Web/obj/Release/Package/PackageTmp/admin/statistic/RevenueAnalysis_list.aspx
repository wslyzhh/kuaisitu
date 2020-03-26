<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RevenueAnalysis_list.aspx.cs" Inherits="MettingSys.Web.admin.statistic.RevenueAnalysis_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>客源收益分析</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery.form.min.js"></script>
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
            //绑定业务性质
            $("#natureAddButton").click(function () {
                var liObj = $(this).parent();
                var d = top.dialog({
                    id: 'natureDialogId',
                    padding: 0,
                    title: "业务性质",
                    url: 'admin/finance_nature.aspx'
                }).showModal();
                //将容器对象传进去
                d.data = liObj;
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
    <form id="ajaxForm" runat="server" action="RevenueAnalysis_list.aspx" method="post" enctype="multipart/form-data">
        <!--导航栏-->
        <div class="location">
            <a href="javascript:history.back(-1);" class="back"><i class="iconfont icon-up"></i><span>返回上一页</span></a>
            <a href="../center.aspx" class="home"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>客源收益分析</span>
        </div>
        <!--/导航栏-->


        <div class="searchbar">
            <div class="menu-list" style="margin-bottom: 10px;">
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
                            <li class="icon-btn">
                                <a id="specAddButton"><i class="iconfont icon-close"></i></a>
                            </li>
                        </ul>
                    </div>
                业务性质：
                    <div class="txt-item">
                        <ul>
                            <asp:Repeater ID="rptNatureList" runat="server">
                                <ItemTemplate>
                                    <li>
                                        <input name="hide_nature" type="hidden" value="<%#Eval("na_id")%>|<%#Eval("na_name")%>" />
                                        <a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                        <span><%#Eval("na_name")%></span>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="icon-btn">
                                <a id="natureAddButton"><i class="iconfont icon-close"></i></a>
                            </li>
                        </ul>
                    </div>
                订单结束日期月份：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM',maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
                -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM',minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
                客源：
                                <asp:TextBox ID="txtCusName" runat="server" CssClass="input" Width="150"></asp:TextBox>
                <asp:HiddenField ID="hCusId" runat="server" />
                包含税费成本：
                    <div class="rule-single-checkbox">
                        <asp:CheckBox ID="cbIsCust" runat="server" Checked="true" />
                    </div>
                锁单状态：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddllock" runat="server"></asp:DropDownList>
                                </div>
                <input type="hidden" name="action" value="Search" />
                <input <%--id="btnSave"--%> type="submit" class="btn" value="查询" />
                <a href="<%=Utils.CombUrlTxt("RevenueAnalysis_list.aspx", "Excel={0}&txtsDate={1}&txteDate={2}&ddllock={3}&action={4}&hide_place={5}&hide_nature={6}&txtCusName={7}&hCusId={8}&cbIsCust={9}&hide_employee1={10}&hide_employee3={11}&hide_employee4={12}&ddlGroup={13}", "on", _sMonth, _eMonth, _lockstatus, action, _area, _nature, _cusName, _cid, _isCust, _person1, _person3, _person4,_group) %>"><i class="iconfont icon-exl"></i><span>导出Excel</span></a>
                <%--<asp:LinkButton ID="btnExcel" runat="server" PostBackUrl="~/admin/statistic/RevenueAnalysis_list.aspx?Excel=on"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>--%>
            </div>
            <div class="menu-list" style="margin-bottom: 10px;">
                业务员：
                    <div class="txt-item">
                        <ul>
                            <asp:Repeater ID="rptEmployee1" runat="server">
                                <ItemTemplate>
                                    <li title="<%#Eval("[\"op_number\"]")%>">
                                        <input name="hide_employee1" type="hidden" value="<%#Eval("[\"op_name\"]")%>|<%#Eval("[\"op_number\"]")%>|<%#Eval("[\"op_area\"]")%>" />
                                        <a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                        <span><%#Eval("[\"op_name\"]")%></span>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="icon-btn">
                                <a href="javascript:" onclick="chooseEmployee(this,1,true,false)"><i class="iconfont icon-close"></i></a>
                            </li>
                        </ul>
                    </div>
                策划人员：
                    <div class="txt-item">
                        <ul>
                            <asp:Repeater ID="rptEmployee2" runat="server">
                                <ItemTemplate>
                                    <li title="<%#Eval("[\"op_number\"]")%>">
                                        <input name="hide_employee2" type="hidden" value="<%#Eval("[\"op_name\"]")%>|<%#Eval("[\"op_number\"]")%>|<%#Eval("[\"op_area\"]")%>" />
                                        <a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                        <span><%#Eval("[\"op_name\"]")%></span>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="icon-btn">
                                <a href="javascript:" onclick="chooseEmployee(this,2,true,false)"><i class="iconfont icon-close"></i></a>
                            </li>
                        </ul>
                    </div>
                设计人员：
                    <div class="txt-item">
                        <ul>
                            <asp:Repeater ID="rptEmployee3" runat="server">
                                <ItemTemplate>
                                    <li title="<%#Eval("[\"op_number\"]")%>">
                                        <input name="hide_employee3" type="hidden" value="<%#Eval("[\"op_name\"]")%>|<%#Eval("[\"op_number\"]")%>|<%#Eval("[\"op_area\"]")%>" />
                                        <a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                        <span><%#Eval("[\"op_name\"]")%></span>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="icon-btn">
                                <a href="javascript:" onclick="chooseEmployee(this,3,true,false)"><i class="iconfont icon-close"></i></a>
                            </li>
                        </ul>
                    </div>
                执行人员：
                    <div class="txt-item">
                        <ul>
                            <asp:Repeater ID="rptEmployee4" runat="server">
                                <ItemTemplate>
                                    <li title="<%#Eval("[\"op_number\"]")%>">
                                        <input name="hide_employee4" type="hidden" value="<%#Eval("[\"op_name\"]")%>|<%#Eval("[\"op_number\"]")%>|<%#Eval("[\"op_area\"]")%>" />
                                        <a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                        <span><%#Eval("[\"op_name\"]")%></span>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="icon-btn">
                                <a href="javascript:" onclick="chooseEmployee(this,4,true,false)"><i class="iconfont icon-close"></i></a>
                            </li>
                        </ul>
                    </div>
                分组显示：
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlGroup" runat="server">
                            <asp:ListItem Value="">无分组</asp:ListItem>
                            <asp:ListItem Value="1">区域</asp:ListItem>
                            <asp:ListItem Value="2">客源</asp:ListItem>
                            <asp:ListItem Value="3">业务员</asp:ListItem>
                            <asp:ListItem Value="4">策划人员</asp:ListItem>
                            <asp:ListItem Value="8">设计人员</asp:ListItem>
                            <asp:ListItem Value="5">执行人员</asp:ListItem>
                            <asp:ListItem Value="6">月份</asp:ListItem>
                            <asp:ListItem Value="7">业务性质</asp:ListItem>
                        </asp:DropDownList>
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
                            <th width="8%">客源</th>
                            <th>活动名称</th>
                            <th width="8%">活动地点</th>
                            <th width="5%">活动结束日期</th>
                            <th width="5%">税费成本</th>
                            <th width="5%">业务性质</th>
                            <th width="5%">应收金额</th>
                            <th width="5%">应付金额</th>
                            <th width="5%">业务毛利</th>
                            <th width="5%">区域</th>
                            <th width="6%">业务员</th>
                            <th width="10%">策划人员</th>
                            <th width="10%">设计人员</th>
                            <th width="10%">执行人员</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("o_id") %></td>
                        <td><%#Eval("c_name") %></td>
                        <td><%#Eval("o_content") %></td>
                        <td><%#Eval("o_address") %></td>
                        <td><%#ConvertHelper.toDate(Eval("o_edate")).Value.ToString("yyyy-MM-dd") %></td>
                        <td><%#Eval("o_financeCust") %></td>
                        <td><%#Eval("na_name") %></td>
                        <td><%#Eval("shou") %></td>
                        <td><%#Eval("fu") %></td>
                        <td><%#Eval("profit") %></td>
                        <td><%#Eval("op_area") %></td>
                        <td><span title="<%#Eval("op_number") %>"><%#Eval("op_name") %></span></td>
                        <td><%#Eval("person2").ToString().Replace("待定","<font color='red'>待定</font>").Replace("处理中","<font color='blue'>处理中</font>").Replace("已完成","<font color='green'>已完成</font>") %></td>
                        <td><%#Eval("person3").ToString().Replace("待定","<font color='red'>待定</font>").Replace("处理中","<font color='blue'>处理中</font>").Replace("已完成","<font color='green'>已完成</font>") %></td>
                        <td><%#Eval("person4") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"15\">暂无记录</td></tr>" : ""%>
                 </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptList1" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr style="text-align: left;">
                            <th>区域</th>
                            <th width="20%">应收金额</th>
                            <th width="20%">应付金额</th>
                            <th width="20%">业务毛利</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("op_area") %></td>
                        <td><%#Eval("shou") %></td>
                        <td><%#Eval("fu") %></td>
                        <td><%#Eval("profit") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList1.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"4\">暂无记录</td></tr>" : ""%>
                 </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptList2" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr style="text-align: left;">
                            <th>客源</th>
                            <th width="20%">区域</th>
                            <th width="20%">应收金额</th>
                            <th width="20%">应付金额</th>
                            <th width="20%">业务毛利</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("c_name") %></td>
                        <td><%#Eval("op_area") %></td>
                        <td><%#Eval("shou") %></td>
                        <td><%#Eval("fu") %></td>
                        <td><%#Eval("profit") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList2.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"5\">暂无记录</td></tr>" : ""%>
                 </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptList3" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr style="text-align: left;">
                            <th>业务员</th>
                            <th width="20%">应收金额</th>
                            <th width="20%">应付金额</th>
                            <th width="20%">业务毛利</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("op_number") %>/<%#Eval("op_area") %>/<%#Eval("op_name") %></td>
                        <td><%#Eval("shou") %></td>
                        <td><%#Eval("fu") %></td>
                        <td><%#Eval("profit") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList3.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"4\">暂无记录</td></tr>" : ""%>
                 </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptList4" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr style="text-align: left;">
                            <th>设计策划人员</th>
                            <th width="20%">应收金额</th>
                            <th width="20%">应付金额</th>
                            <th width="20%">业务毛利</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("op_number") %>/<%#Eval("op_area") %>/<%#Eval("op_name") %></td>
                        <td><%#Eval("shou") %></td>
                        <td><%#Eval("fu") %></td>
                        <td><%#Eval("profit") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList4.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"4\">暂无记录</td></tr>" : ""%>
                 </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptList5" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr style="text-align: left;">
                            <th>执行人员</th>
                            <th width="20%">应收金额</th>
                            <th width="20%">应付金额</th>
                            <th width="20%">业务毛利</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("op_number") %>/<%#Eval("op_area") %>/<%#Eval("op_name") %></td>
                        <td><%#Eval("shou") %></td>
                        <td><%#Eval("fu") %></td>
                        <td><%#Eval("profit") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList5.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"4\">暂无记录</td></tr>" : ""%>
                 </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptList6" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr style="text-align: left;">
                            <th>月份</th>
                            <th width="20%">区域</th>
                            <th width="20%">应收金额</th>
                            <th width="20%">应付金额</th>
                            <th width="20%">业务毛利</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("oYear") %>/<%#Eval("oMonth") %></td>
                        <td><%#Eval("op_area") %></td>
                        <td><%#Eval("shou") %></td>
                        <td><%#Eval("fu") %></td>
                        <td><%#Eval("profit") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList6.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"5\">暂无记录</td></tr>" : ""%>
                 </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptList7" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr style="text-align: left;">
                            <th>业务性质</th>
                            <th width="20%">区域</th>
                            <th width="20%">应收金额</th>
                            <th width="20%">应付金额</th>
                            <th width="20%">业务毛利</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("na_name") %></td>
                        <td><%#Eval("op_area") %></td>
                        <td><%#Eval("shou") %></td>
                        <td><%#Eval("fu") %></td>
                        <td><%#Eval("profit") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList7.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"5\">暂无记录</td></tr>" : ""%>
                 </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <!--/列表-->
        <div style="font-size: 12px;">
            <span style="float: left;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，合计应收金额：<asp:Label ID="pShou" runat="server">0</asp:Label>，应付金额：<asp:Label ID="pFu" runat="server">0</asp:Label>，业务毛利：<asp:Label ID="pProfit" runat="server">0</asp:Label></span>
            <span style="float: right;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计应收金额：<asp:Label ID="tShou" runat="server">0</asp:Label>，应付金额：<asp:Label ID="tFu" runat="server">0</asp:Label>，业务毛利：<asp:Label ID="tProfit" runat="server">0</asp:Label></span>
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

    </form>
</body>
</html>
