<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="bankDetails.aspx.cs" Inherits="MettingSys.Web.admin.finance.bankDetails" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="table-container" style="padding:25px;width: 450px;">
            <table class="ltable" style="width:450px;min-width:450px;" border="1" cellspacing="0" cellpadding="0">
                <tr>
                    <th style="width:30%;">账户名称</th>
                    <th>银行账号</th>
                    <th style="width:30%;">开户行</th>
                </tr>
                <%if (dt != null && dt.Rows.Count > 0){ %>
                <%foreach (DataRow dr in dt.Rows){ %>
                <tr>
                    <td><%=dr["cb_bankName"] %></td>
                    <td><%=dr["cb_bankNum"] %></td>
                    <td><%=dr["cb_bank"] %></td>
                </tr>
                <%}} %>
            </table>
        </div>
    </form>
</body>
</html>
