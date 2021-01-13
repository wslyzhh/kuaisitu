<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="chkDetails.aspx.cs" Inherits="MettingSys.Web.admin.order.chkDetails" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
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
                    <th style="width:50%;">对账标识</th>
                    <th>对账金额</th>
                </tr>
                <%if (dt != null && dt.Rows.Count > 0){ %>
                <%foreach (DataRow dr in dt.Rows){ %>
                <tr>
                    <td><%=dr["fc_num"] %></td>
                    <td><%=dr["fc_money"] %></td>
                </tr>
                <%}} %>
            </table>
        </div>
    </form>
</body>
</html>
