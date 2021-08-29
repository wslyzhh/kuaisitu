<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="order_list.aspx.cs" Inherits="MettingSys.Web.admin.order.order_list" ValidateRequest="false" %>

<%@ Import Namespace="MettingSys.Common" %>
<%@ Import Namespace="MettingSys.BLL" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>业务性质列表</title>
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

        .myRuleSelect .select-tit {
            padding: 5px 5px 7px 5px;
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
            <span>订单列表</span>
        </div>
        <!--/导航栏-->
        <div class="content-tab-wrap">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a <%=flag=="0"?"class=\"selected\"":"" %> href="order_list.aspx?flag=0&type=<%=_type %>">全部订单</a></li>
                        <li id="li1" runat="server"><a <%=flag=="1"?"class=\"selected\"":"" %> href="order_list.aspx?flag=1">我的订单</a></li>
                        <li id="li2" runat="server"><a <%=flag=="2"?"class=\"selected\"":"" %> href="order_list.aspx?flag=2">我的报账订单</a></li>
                        <li id="li3" runat="server"><a <%=flag=="3"?"class=\"selected\"":"" %> href="order_list.aspx?flag=3">我的策划订单<sup class="sup"><asp:Label ID="labPerson3Count" runat="server">0</asp:Label></sup></a></li>
                        <li id="li5" runat="server"><a <%=flag=="5"?"class=\"selected\"":"" %> href="order_list.aspx?flag=5">我的设计订单<sup class="sup"><asp:Label ID="labPerson5Count" runat="server">0</asp:Label></sup></a></li>
                        <li id="li4" runat="server"><a <%=flag=="4"?"class=\"selected\"":"" %> href="order_list.aspx?flag=4">我的执行订单</a></li>
                        <li id="li6" runat="server"><a <%=flag=="6"?"class=\"selected\"":"" %> href="order_list.aspx?flag=6">我的共同订单</a></li>

                        
                        <li id="li7" runat="server"><a <%=flag=="7"?"class=\"selected\"":"" %> href="order_list.aspx?flag=7&type=<%=_type %>">待审批<sup class="sup"><asp:Label ID="labCheck7Count" runat="server">0</asp:Label></sup></a></li>                        
                        <li id="li8" runat="server"><a <%=flag=="8"?"class=\"selected\"":"" %> href="order_list.aspx?flag=8&type=<%=_type %>">审批不通过<sup class="sup"><asp:Label ID="labCheck8Count" runat="server">0</asp:Label></sup></a></li>
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
                                <li><a href="javascript:;" onclick="checkAll(this);"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                                <li>
                                    <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete','删除后无法恢复，是否继续？');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
            <!--/工具栏-->
            <div class="searchbar">
                订 单 号 ：
                        <asp:TextBox ID="txtOrderID" runat="server" CssClass="input"></asp:TextBox>
                客&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;户：
                                <asp:TextBox ID="txtCusName" runat="server" CssClass="input"></asp:TextBox>
                <asp:HiddenField ID="hCusId" runat="server" />
                合同造价：
                                <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlContractPrice" runat="server"></asp:DropDownList>
                                </div>
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
            </div>
            <div class="searchbar">
                活动名称：
                        <asp:TextBox ID="txtContent" runat="server" CssClass="input"></asp:TextBox>
                活动地点：
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="input"></asp:TextBox>
                <div class="rule-single-select">
                            <asp:DropDownList ID="ddlmoneyType" runat="server" Width="50">
                                <asp:ListItem Value="0">应收款</asp:ListItem>
                                <asp:ListItem Value="1">未收款</asp:ListItem>
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
            </div>
            <div class="searchbar">
                归属地：
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlarea" runat="server"></asp:DropDownList>
                    </div>
                下单区域：
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlorderarea" runat="server"></asp:DropDownList>
                    </div>
                活动开始日期：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
                -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
                活动结束日期：
                        <asp:TextBox ID="txtsDate1" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate1\')}'})"></asp:TextBox>
                -
                        <asp:TextBox ID="txteDate1" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate1\')}'})"></asp:TextBox>
                订单确认时间：
                        <asp:TextBox ID="txtsDate2" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate2\')}'})"></asp:TextBox>
                -
                        <asp:TextBox ID="txteDate2" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate2\')}'})"></asp:TextBox>
                <input type="hidden" name="flag" value="<%=flag %>" />
                <input type="hidden" name="type" value="<%=_type %>" />
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
                                <th align="left" width="6%">订单号</th>
                                <th align="left">活动名称/地点</th>
                                <th align="left" width="8%">客户</th>
                                <th align="left" width="6%">合同造价</th>
                                <th align="left" width="7%">活动日期</th>
                                <th align="left" width="6%">归属地</th>
                                <th align="left" width="4%">订单状态</th>
                                <th align="left" width="4%">锁单状态</th>
                                <th align="left" width="5%">业务员</th>
                                <th align="left" width="6%">报账人员</th>
                                <th align="left" width="10%">策划人员</th>
                                <th align="left" width="10%">设计人员</th>
                                <th align="left" width="6%">收款</th>
                                <th align="left" width="4%">业绩利润</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td align="center">
                                <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                                <asp:HiddenField ID="hidId" Value='<%#Eval("o_id")%>' runat="server" />
                            </td>
                            <td><a href="../order/order_edit.aspx?action=<%# DTEnums.ActionEnum.Edit.ToString() %>&oID=<%#Eval("o_id")%>"><span class="orderstatus_<%#Eval("o_status")%>" title="确认时间：<%#Utils.ObjectToStr(Eval("o_statusTime"))==""?"": ConvertHelper.toDate(Utils.ObjectToStr(Eval("o_statusTime"))).Value.ToString("yyyy-MM-dd HH:mm:ss")%>"><%#Eval("o_id")%></span></a></td>
                            <td>名称：<%#Eval("o_content")%><br />
                                地点：<%#Eval("o_address")%>
                            </td>
                            <td><%#Eval("c_name")%></td>
                            <td><%#Eval("o_contractPrice")%></td>
                            <td><%#ConvertHelper.toDate(Eval("o_sdate")).Value.ToString("yyyy-MM-dd")%><br /><%#ConvertHelper.toDate(Eval("o_edate")).Value.ToString("yyyy-MM-dd")%></td>
                            <td><%# new MettingSys.BLL.department().getAreaText(Eval("place").ToString())%></td>
                            <td><span onmouseover="tip_index=layer.tips('推送状态：<%#MettingSys.Common.BusinessDict.pushStatus()[Convert.ToBoolean(Eval("o_isPush"))]%><br/>上级审批：<%#MettingSys.Common.BusinessDict.checkStatus()[Utils.ObjToByte(Eval("o_flag"))]%>', this, { time: 0 });" onmouseout="layer.close(tip_index);"><%#MettingSys.Common.BusinessDict.fStatus(2)[Utils.ObjToByte(Eval("o_status"))]%></span></td>
                            <td><%#MettingSys.Common.BusinessDict.lockStatus()[Utils.ObjToByte(Eval("o_lockStatus"))]%></td>
                            <td><span onmouseover="tip_index=layer.tips('工号：<%#Eval("op_number")%><br/>下单时间：<%#Eval("o_addDate")%><br/>共同业务员：<%#Eval("person6")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);"><%#Eval("op_name")%>(<%#Eval("op_ratio")%>%)</span></td>
                            <td><%# Utils.ObjectToStr(Eval("person2"))%></td>
                            <td><%# showColor(Utils.ObjectToStr(Eval("person3")))%></td>
                            <td><%# showColor(Utils.ObjectToStr(Eval("person4")))%></td>
                            <td>应收：<%#Eval("finMoney") %><br />未收：<%#Eval("unMoney") %></td>
                            <td><%#Eval("profit") %></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"15\">暂无记录</td></tr>" : ""%>
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
