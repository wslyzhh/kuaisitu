<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="selfFinance_list.aspx.cs" Inherits="MettingSys.Web.admin.self.selfFinance_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>审批未通过应收付列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript">
        $(function () {
            var tip_index;
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
        function toggleCheckDiv() {
            $("#checkDiv").toggle();
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
            if ($("#ddlcheck1").val() == "") {
                jsprint("请选择审批状态");
                return;
            }
            var idStr = "";
            $(".checkall input:checked").each(function () {
                idStr += $(this).parent().next().val() + ",";
            });
            idStr = idStr.substring(0, idStr.length - 1);

            var postData = { "ids": idStr, "status": $("#ddlcheck1").val(), "remark": $("#txtremark").val() };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/Business_ajax.ashx?action=checkfinanceStatus",
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
                            ok: function () { }
                        }).showModal();
                    }
                }
            });
        }
        /*批量审批 */
    </script>
    <style type="text/css">
        .date-input {
            width: 100px;
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
            <span>审批未通过应收付列表</span>
        </div>
        <!--/导航栏-->

        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            <div class="toolbar">
                <div class="box-wrap">
                    <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                    <div class="l-list">
                        <ul class="icon-list">
                            <%--<li><a href="finance_edit.aspx?action=<%=DTEnums.ActionEnum.Add %>&type=<%=type %>"><i class="iconfont icon-close"></i><span>新增</span></a></li>--%>
                            <li>
                            <li><a href="javascript:;" onclick="checkAll(this);"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                            <li>
                                <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete','删除后无法恢复，是否继续？');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                        </ul>
                        <div id="checkDiv" style="display: none;">
                            审批状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck1" runat="server"></asp:DropDownList>
                            </div>
                            备注：
                            <asp:TextBox ID="txtremark" runat="server" CssClass="input normal"></asp:TextBox>
                            <input type="button" class="btn" value="提交" onclick="submitCheck()" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--/工具栏-->
        <div class="searchbar">
            订单号：
                            <asp:TextBox ID="txtOrder" runat="server" CssClass="input"></asp:TextBox>
            收付性质：                            
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddltype" runat="server">
                                    <asp:ListItem Value="">不限</asp:ListItem>
                                    <asp:ListItem Value="True">应收</asp:ListItem>
                                    <asp:ListItem Value="False">应付</asp:ListItem>
                                </asp:DropDownList>
                            </div>
            应收付对象：
                            <asp:TextBox ID="txtCusName" runat="server" CssClass="input"></asp:TextBox>
            <asp:HiddenField ID="hCusId" runat="server" />
            审批状态：                            
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck" runat="server"></asp:DropDownList>
                            </div>
            已结账月份：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM',maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
            -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({dateFmt:'yyyy-MM',minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
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
                            <th align="left" width="6%">订单号</th>
                            <th align="left" width="6%">收付性质</th>
                            <th align="left" width="10%">应收付对象</th>
                            <th align="left" width="6%">对账凭证</th>
                            <th align="left" width="10%">业务性质/明细</th>
                            <th align="left" width="6%">业务日期</th>
                            <th align="left">业务说明</th>
                            <th align="left" width="10%">金额表达式</th>
                            <th align="left" width="5%">区域</th>
                            <th align="left" width="6%">结账月份</th>
                            <th align="left" width="6%">审批状态</th>
                            <th align="left" width="6%">添加人</th>
                            <th align="left" width="5%">财务备注</th>
                            <th width="6%">操作</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td align="center">
                            <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                            <asp:HiddenField ID="hidId" Value='<%#Eval("fin_id")%>' runat="server" />
                        </td>
                        <td><a href="../order/order_edit.aspx?action=<%# DTEnums.ActionEnum.Edit.ToString() %>&oID=<%#Eval("fin_oid")%>"><span class="orderstatus_<%#Eval("o_status")%>"><%#Eval("fin_oid")%></span></a></td>
                        <td><%#Eval("fin_type").ToString()=="True"?"<font color='blue'>应收</font>":"<font color='red'>应付</font>"%></td>
                        <td><%#Eval("c_name")%></td>
                        <td><%#Eval("chk")%></td>
                        <td><%#Eval("na_name")%><br /><%#Eval("fin_detail")%></td>
                        <td><%#Convert.ToDateTime(Eval("fin_sdate")).ToString("yyyy-MM-dd")%><br /><%#Convert.ToDateTime(Eval("fin_edate")).ToString("yyyy-MM-dd")%></td>
                        <td><span onmouseover="tip_index=layer.tips('<%#Eval("fin_illustration") %>', this, { time: 0 });" onmouseout="layer.close(tip_index);"><%#Eval("fin_illustration").ToString().Length<=12?Eval("fin_illustration").ToString():Eval("fin_illustration").ToString().Substring(0,12)+"..."%></span></td>
                        <td><%#Eval("fin_expression")%><br />=<%#Eval("fin_money")%></td>
                        <td><%#Eval("fin_area")%></td>
                        <td><%#Eval("fin_month")%></td>
                        <td><span onmouseover="tip_index=layer.tips('审批人：<%#Eval("fin_checkNum")%>-<%#Eval("fin_checkName")%><br/>审批备注：<%#Eval("fin_checkRemark")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("fin_flag")%>"></span></td>
                        <td><%#Eval("fin_personNum")%><br /><%#Eval("fin_personName")%></td>
                        <td><span onmouseover="tip_index=layer.tips('<%#Eval("fin_remark")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);">备注</span></td>
                        <td align="center"><a href="finance_edit.aspx?action=<%#DTEnums.ActionEnum.Edit %>&type=<%=_type %>&id=<%#Eval("fin_id")%>">修改</a></td>
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
            <p>1.审批：<span class="check_0"></span>待审批，<span class="check_1"></span>审批未通过，<span class="check_2"></span>审批通过</p>
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