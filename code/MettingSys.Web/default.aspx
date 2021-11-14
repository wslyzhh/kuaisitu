<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="MettingSys.Web._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>欢迎登录海南快思图商务会展业务信息管理系统</title>
    <link rel="stylesheet" type="text/css" href="css/login.css" />
    <link rel="stylesheet" type="text/css" href="css/icon/iconfont.css">
    <script type="text/javascript" charset="utf-8" src="scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="scripts/cloud.js" type="text/javascript"></script>
    <script language="javascript">
        $(function () {
            //检测IE
            if ('undefined' == typeof (document.body.style.maxHeight)) {
                window.location.href = 'ie6update.html';
            }
            $('.loginbox').css({ 'position': 'absolute', 'left': ($(window).width() - 692) / 2 });
            $(window).resize(function () {
                $('.loginbox').css({ 'position': 'absolute', 'left': ($(window).width() - 692) / 2 });
            })
        });
        function fGetCode() {
            document.getElementById("checkImg").src = "tools/captcha.ashx?" + Math.random();
        }
        //转大写
        function cToUpper(obj) {
            $(obj).val($(obj).val().toUpperCase());
        }
    </script>
    <style type="text/css">
        body {
            background-color: #1c77ac;
            background-image: url(images/light.png);
            background-repeat: no-repeat;
            background-position: center top;
            overflow: hidden;
        }

        #msgtip {
            color: red;
            font-size: 14px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="mainBody">
            <div id="cloud1" class="cloud"></div>
            <div id="cloud2" class="cloud"></div>
        </div>
        <div class="logintop">
            <span>欢迎登录海南快思图商务会展业务信息管理系统</span>
            <%--<ul>
                <li><a href="#">回首页</a></li>
                <li><a href="#">帮助</a></li>
                <li><a href="#">关于</a></li>
            </ul>--%>
        </div>
        <div class="loginbody">            
            <span class="systemlogo"><%--<img src="images/logo.png" />--%>&nbsp;&nbsp;<span>海南快思图商务会展业务信息管理系统</span></span>
            <div class="loginbox loginbox3">
                <ul>
                    <li>
                        <asp:TextBox ID="txtUserName" runat="server" CssClass="loginuser" placeholder="请输入登录账号" title="请输入登录账号" onkeyup="cToUpper(this)"></asp:TextBox></li>
                    <li>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="loginpwd" TextMode="Password" placeholder="请输入登录密码" title="请输入登录密码"></asp:TextBox></li>
                    <li class="yzm">
                        <span>
                            <asp:TextBox ID="txtValidcode" runat="server" CssClass="loginpwd" placeholder="请输入验证码" title="请输入验证码" MaxLength="4"></asp:TextBox></span>
                        <cite>
                            <img id="checkImg" src="tools/captcha.ashx" class="yzm-img" onclick="fGetCode()" style="cursor: pointer; width: 112px; height: 37px;" align="absmiddle" title="看不清，换一张" alt="看不清，换一张" /></cite>
                    </li>
                    <li>
                        <asp:Button ID="BtnLogin" TabIndex="4" runat="server" Text="登 录" CssClass="loginbtn" OnClick="BtnLogin_Click" />
                        <label id="msgtip" runat="server">提示：请输入用户名和密码</label>
                    </li>
                </ul>
            </div>
        </div>
        <div class="loginbm">Copyright &copy; 【琼ICP备14002530号-1】 海南快思图商务会展有限公司&nbsp;&nbsp;版权所有</div>
    </form>
</body>
</html>
