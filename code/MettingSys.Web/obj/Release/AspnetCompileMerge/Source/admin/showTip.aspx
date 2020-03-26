<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="showTip.aspx.cs" Inherits="MettingSys.Web.admin.showTip" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script type="text/javascript" src="../scripts/jquery/jquery-1.11.2.min.js"></script>
    <style type="text/css">
        .tipMessage {
            font-size: 13px;
        }

        .messageContent {
            padding: 20px;
            height: 60%;
        }

        .mUp {
            float: left;
            margin-left: 50px;
            cursor: pointer;
        }

        .mdown {
            float: right;
            margin-right: 50px;
            cursor: pointer;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="tipMessage">
            <div class='messageContent'>####人员已增加你为订单（订单号 +活动名称+活动地点+活动开始日期）的业务报账人员 ！</div>
            <div style="margin-bottom: 0px;">
                <div class="mUp">
                    上一条
                </div>
                <div class="mdown">下一条</div>
            </div>
        </div>
    </form>
</body>
</html>
