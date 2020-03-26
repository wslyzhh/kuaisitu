<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="finance_chk.aspx.cs" Inherits="MettingSys.Web.admin.finance.finance_chk" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript">
        function addChk() {
            if ($("#txtNum").val() == "") {
                layer.msg("请填写对账标识");
                $("#txtNum").focus();
                return;
            }
            if ($("#txtMoney").val() == "") {
                layer.msg("请填写对账金额");
                $("#txtMoney").focus();
                return;
            }
            var postData = { finid:<%=finid%>,oid:'<%=oid %>', num: $("#txtNum").val(),money:$("#txtMoney").val(),fcid:$("#txtFcID").val() };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/business_ajax.ashx?action=addChk",
                data: postData,
                dataType: "json",
                success: function (data) {
                    if (data.status == 0) {
                        layer.msg("对账成功");
                        setTimeout(function () {
                            location.reload();
                        }, 1500);
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
        function delChk(id) {
            var postData = { id: id };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/business_ajax.ashx?action=delChk",
                data: postData,
                dataType: "json",
                success: function (data) {
                    if (data.status == 0) {
                        layer.msg("删除成功");
                        setTimeout(function () {
                            location.reload();
                        }, 1500);
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
        function editChk(fcid, num, money) {
            $("#txtNum").val(num);
            $("#txtMoney").val(money);
            $("#txtFcID").val(fcid);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="table-container">
            <asp:Repeater ID="rptList" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable" style="min-width: 0;">
                        <tr style="text-align: center;">
                            <th width="40%">对账标识</th>
                            <th width="40%">对账金额</th>
                            <th>操作</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr style="text-align: center;">
                        <td><%#Eval("fc_num") %></td>
                        <td><%#Eval("fc_money") %></td>
                        <td>
                            <a href="javascript:;" onclick="editChk(<%#Eval("fc_id") %>,'<%#Eval("fc_num") %>',<%#Eval("fc_money") %>)">编辑</a>
                            <a href="javascript:;" onclick="delChk(<%#Eval("fc_id") %>)">删除</a>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"3\">暂无记录</td></tr>" : ""%>
                </table>
                </FooterTemplate>
            </asp:Repeater>
            <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable" style="min-width: 0;">
                <tr style="text-align: center;">
                    <td width="40%">
                        <asp:TextBox ID="txtNum" runat="server" CssClass="input" /></td>
                    <td width="40%">
                        <asp:TextBox ID="txtMoney" runat="server" CssClass="input" /></td>
                    <td>
                        <input type="hidden" id="txtFcID" value="0" />
                        <input type="button" onclick="addChk()" class="btn" value="保存" /></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
