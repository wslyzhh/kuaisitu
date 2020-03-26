<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="chooseCustomerDemo.aspx.cs" Inherits="MettingSys.Web.admin.chooseCustomerDemo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>    
    <link href="js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script src="../scripts/jquery-1.8.2.min.js"></script>
    <script src="js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript">
        $(function () {
            var nhlTeams = [{"id":1,"name":"你好","type":"普通客户" }, {"id":1,"name":"水电费水电费","type":"管理用客户" }]
            var nbaTeams = [{"id":3,"name":"啊飒飒水大师","type":"内部客户" }, {"id":7,"name":"十多你个电饭锅","type":"普通客户" }]            
            var teams = nhlTeams.concat(nbaTeams);
            $('#autocomplete').devbridgeAutocomplete({
                lookup: teams,
                minChars: 1,
                onSelect: function (suggestion) {
                    $('#selection').html('You selected: ' + suggestion.id + ', ' + suggestion.name);
                },
                showNoSuggestionNotice: true,
                noSuggestionNotice: '抱歉，没有匹配的选项',
                groupBy: 'type'
            });
        })
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox runat="server" ID="autocomplete"></asp:TextBox>
            <div id="selection"></div>
        </div>
    </form>
</body>
</html>
