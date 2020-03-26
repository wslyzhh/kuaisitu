<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecryptPWD.aspx.cs" Inherits="MettingSys.Web.admin.DecryptPWD" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
         密文：<asp:TextBox ID="pwd1" runat="server"></asp:TextBox>         
         秘钥：<asp:TextBox ID="salt" runat="server"></asp:TextBox>
         明文：<asp:TextBox ID="pwd2" runat="server"></asp:TextBox>
        <asp:Button ID="btnSubmit" runat="server" Text="提交" OnClick="btnSubmit_Click" />
    </form>
</body>
</html>
