<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="certificate_list.aspx.cs" Inherits="MettingSys.Web.admin.finance.certificate_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>凭证管理列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript">
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
                url: "../../tools/Business_ajax.ashx?action=checkCertificateStatus",
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
            <span>凭证管理列表</span>
        </div>
        <!--/导航栏-->

        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            <div class="toolbar">
                <div class="box-wrap">
                    <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                    <div class="l-list">
                        <ul class="icon-list">
                            <li><a href="certificate_edit.aspx?action=<%=DTEnums.ActionEnum.Add %>"><i class="iconfont icon-close"></i><span>新增</span></a></li>
                            <li><a href="javascript:;" onclick="checkAll(this);"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                            <li><a href="javascript:;" onclick="reverseCheckAll();"><i class="iconfont icon-check-mark"></i><span>反选</span></a></li>
                            <li>
                                <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete','删除后无法恢复，是否继续？');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                            <li><a href="javascript:;" onclick="toggleCheckDiv()"><span>审批</span></a></li>
                        </ul>
                        <div id="checkDiv" style="display: none;">
                            审批状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck1" runat="server"></asp:DropDownList>
                            </div>
                            备注：
                            <asp:TextBox ID="txtremark" runat="server" CssClass="input normal"></asp:TextBox>
                            <input type="button" class="btn" value="提交" onclick="submitCheck()" />
                            <asp:LinkButton ID="btnExcel" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--/工具栏-->
        <div class="searchbar">
            凭证号：
                            <asp:TextBox ID="txtNum" runat="server" CssClass="input" />
            凭证日期：
                            <asp:TextBox ID="txtsdate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            -
                            <asp:TextBox ID="txtedate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtsdate\')}'})" />
            审批状态：                            
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck" runat="server"></asp:DropDownList>
                            </div>
            分组显示：                            
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlgroup" runat="server">
                                    <asp:ListItem Value="">不分组</asp:ListItem>
                                    <asp:ListItem Value="date">日期</asp:ListItem>
                                    <asp:ListItem Value="month">月份</asp:ListItem>
                                    <asp:ListItem Value="year">年份</asp:ListItem>
                                </asp:DropDownList>
                            </div>
            <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>
        </div>

        <!--列表-->
        <div class="table-container">
            <asp:Repeater ID="rptList" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr>
                            <th width="6%">选择</th>
                            <th align="left" width="8%">凭证号</th>
                            <th align="left" width="8%">凭证日期</th>
                            <th align="left" width="8%">已收总额</th>
                            <th align="left" width="8%">已付总额</th>
                            <th align="left">备注</th>
                            <th align="left" width="8%">状态</th>
                            <th width="10%">操作</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td align="center">
                            <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                            <asp:HiddenField ID="hidId" Value='<%#Eval("ce_id")%>' runat="server" />
                        </td>
                        <td><%#Eval("ce_num")%></td>
                        <td><%#ConvertHelper.toDate(Eval("ce_date")).Value.ToString("yyyy-MM-dd")%></td>
                        <td><a href="receipt_list.aspx?txtNum=<%#Eval("ce_num")%>&ddlisConfirm=True"><%#Eval("receipt")%></a></td>
                        <td><a href="pay_list.aspx?txtNum=<%#Eval("ce_num")%>&ddlisConfirm=True"><%#Eval("pay")%></a></td>
                        <td><%#Eval("ce_remark")%></td>
                        <td>
                            <span onmouseover="tip_index=layer.tips('审批人：<%#Eval("ce_checkNum")%>-<%#Eval("ce_checkName")%><br/>审批备注：<%#Eval("ce_checkRemark").ToString().Replace("\r\n","").Replace("\r","").Replace("\n","")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("ce_flag")%>"></span>
                        </td>
                        <td align="center"><a href="certificate_edit.aspx?action=<%#DTEnums.ActionEnum.Edit %>&id=<%#Eval("ce_id")%>">修改</a></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"8\">暂无记录</td></tr>" : ""%>
  </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Repeater ID="rptList1" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr>
                            <th align="left" width="10%"><%=ddlgroup.SelectedItem.Text %></th>
                            <th align="left" width="10%">已收总额</th>
                            <th align="left">已付总额</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("ce_date") %></td>
                        <td><%# Eval("receipt") %></td>
                        <td><%# Eval("pay") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList1.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"3\">暂无记录</td></tr>" : ""%>
  </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div style="font-size: 12px;">
            <span style="float: left;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，总计已收金额：<asp:Label ID="pMoney" runat="server">0</asp:Label>，总计已付金额：<asp:Label ID="pUnMoney" runat="server">0</asp:Label></span>
            <span style="float: right;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计已收金额：<asp:Label ID="tMoney" runat="server">0</asp:Label>，总计已付金额：<asp:Label ID="tUnMoney" runat="server">0</asp:Label></span>
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

    </form>
</body>
</html>
