<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecryptPassword.aspx.cs" Inherits="MettingSys.Web.DecryptPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            加密密码：<asp:TextBox ID="txtPassword" runat="server"></asp:TextBox>
            随机码：<asp:TextBox ID="txtSalt" runat="server"></asp:TextBox>
            <asp:Button ID="btnSubmit" runat="server" Text="查询" OnClick="btnSubmit_Click" />
            <asp:Label ID="labpassword" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
