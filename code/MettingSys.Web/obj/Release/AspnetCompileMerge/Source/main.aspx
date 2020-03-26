<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="MettingSys.Web.main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>海南快思图商务会展业务信息管理系统</title>
    <link rel="stylesheet" type="text/css" href="scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="css/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="css/main.css" />
    <link rel="shortcut icon" href="favicon.ico" /> 
    <script type="text/javascript" charset="utf-8" src="scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="scripts/jquery/jquery.nicescroll.js"></script>
    <script type="text/javascript" charset="utf-8" src="scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="scripts/layindex.js"></script>
    <script type="text/javascript" charset="utf-8" src="scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="scripts/common.js"></script>
    <script src="scripts/jquery.signalR-2.4.1.min.js"></script>
    <script src="SignalR/js"></script>
    <script type="text/javascript">
        //页面加载完成时
        $(function () {
            //检测IE
            if ('undefined' == typeof (document.body.style.maxHeight)) {
                window.location.href = 'ie6update.html';
            }
            //登录成功时弹出未读消息提醒
            /*
            showSelfMessage("selfmessage");

             var proxy = $.connection.messageService;
            proxy.client.displayDatas = function () {
                showSelfMessage("selfmessageOnline");
            }
            $.connection.hub.start();
            showSelfMessage("selfmessageOnline");
            $.connection.hub.start().done(function () {
                showSelfMessage("selfmessageOnline");
            });
            */
        });
        //弹出未读消息======================
        function showSelfMessage(_action) {
            var postData = {};
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "tools/unBusiness_ajax.ashx?action=" + _action,
                data: postData,
                dataType: "json",
                success: function (data) {
                    if (data.length > 0) {
                        var contentstr = "", isShow = "", fid = 0, ftitle = "";
                        $.each(data, function (i, item) {
                            if (i > 0) {
                                isShow = "style='display:none;'";
                            }
                            else {
                                fid = item.me_id;
                                ftitle = item.me_title;
                            }
                            contentstr += "<span id='sp_" + item.me_id + "' tip='" + item.me_id + "' " + isShow + "><span class='sp_id'>" + item.me_id + "</span><span class='sp_title'>" + item.me_title + "</span><span class='sp_content' >" + item.me_content + "</span><span class='sp_status'>" + item.me_isRead + "</span></span>";
                        });
                        var htmlstr = "<div class=\"tipMessage\">"
                            + "<div class='messageContent'>" + contentstr + "</div>"
                            + "<div class='footer'><ul><li id='liUp' tip='" + fid + "' onclick='readMessage(this,0)'>上一条</li><li id='liDown' tip='" + fid + "' onclick='readMessage(this,1)'>下一条</li></ul></div></div>"
                            + "<script type=\"text/javascript\">"
                            + "function readMessage(obj,n) { "
                            + "var sp =$('#sp_'+$(obj).attr(\"tip\"));"
                            + "if(n==0){if(sp.prev().length==0){jsprint(\"没有上一条消息了\",\"\");}else{sp.prev().show().siblings().hide();$(obj).attr(\"tip\",sp.prev().attr(\"tip\")).next().attr(\"tip\",sp.prev().attr(\"tip\"));$(\"#messageDiv\").prev().text(sp.prev().children(\".sp_title\").text());}}"//上一页
                            + "else{updateReadStatus($(obj).attr(\"tip\"));if(sp.next().length == 0){jsprint(\"没有下一条消息了\",\"\")}else{sp.next().show().siblings().hide();$(obj).attr(\"tip\",sp.next().attr(\"tip\")).prev().attr(\"tip\",sp.next().attr(\"tip\"));$(\"#messageDiv\").prev().text(sp.next().children(\".sp_title\").text());}}"//下一页
                            + "}"
                            + "function updateReadStatus(mid) {"
                            + "var postData = { \"mid\": mid };"
                            + "$.ajax({type: \"post\",url: \"tools/unBusiness_ajax.ashx?action=setMessageStatus\",data: postData,dataType: \"json\",success: function (data) {}});"
                            + "}"
                            + "<\/script>";

                        layer.open({
                            type: 1
                            , offset: 'rb' //具体配置参考：http://www.layui.com/doc/modules/layer.html#offset
                            , id: 'messageDiv' //防止重复弹出
                            , title: ['测试', 'background-color: #066cac;font-weight:bolder;color:white;']
                            //, content: ['admin/showTip.aspx', 'no']
                            , content: htmlstr
                            , area: ['20%', '20%']
                            , scrollbar: false
                            , shade: 0 //不显示遮罩
                            , yes: function () {
                                layer.closeAll();
                            }
                            , success: function () {
                                $("#messageDiv").prev().text(ftitle);
                            }
                        });
                    }
                }
            });
        }
        //弹出未读消息======================         
    </script>
    <style type="text/css">
        /*弹出消息框样式======================*/
        #messageDiv {
            background-color: #f0f9fd;
        }
        .tipMessage {
            font-size: 13px;
        }
        .messageContent {
            padding: 20px;
            height: 80%;
            line-height: 18px;
            text-indent: 2em;
        }
        .messageContent .sp_id, .sp_title, .sp_status {
            display: none;
        }
        .footer {
            height: 30px;
            width: 100%;
            position: fixed;
            bottom: 0;
        }
        .footer ul li {
            display: inline-block;
            width: 100px;
            text-align: center;
            cursor: pointer;
        }
        /*弹出消息框样式======================*/
    </style>
</head>
<body class="indexbody">
    <form id="form1" runat="server">
        <!--全局菜单-->
        <a class="btn-paograms" href="javascript:;" onclick="togglePopMenu();">
            <i class="iconfont icon-list-fill"></i>
        </a>
        <div id="pop-menu" class="pop-menu">
            <div class="pop-box">
                <h1 class="title"><i class="iconfont icon-setting"></i>导航菜单</h1>
                <i class="close iconfont icon-remove" onclick="togglePopMenu();"></i>
                <div class="list-box"></div>
            </div>
        </div>
        <!--/全局菜单-->
        <div class="main-top">
            <div class="nav-left">
                <a href="center.aspx" target="mainframe">
                    <img src="images/logo.png" width="310px" alt="系统首页" /></a>
            </div>
            <%--<a class="icon-menu"><i class="iconfont icon-nav"></i></a>--%>
            <div id="main-nav" class="main-nav"></div>
            <div class="nav-right">
                <div class="info">
                    <h4>
                        <%if (!string.IsNullOrEmpty(admin_info.avatar))
                            {%>
                        <img src="<%=admin_info.avatar%>" />
                        <%}
                            else
                            {%>
                        <i class="iconfont icon-user"></i>
                        <%}%>
                    </h4>
                    <span style="margin-right: 10px;">您好，<%=admin_info.user_name%>(<%=admin_info.real_name%>)<br />
                        <%=DepartName %>
                    </span>
                    <span style="padding-top: 10px;"><a target="mainframe" href="admin/self/selfMessage.aspx"><span>个人消息</span></a></span>
                </div>
                <div class="option">
                    <i class="iconfont icon-arrow-down"></i>
                    <div class="drop-wrap">
                        <ul class="item">
                            <li>
                                <a href="http://www.crystalmice.com" target="_blank">企业官网</a>
                            </li>
                            <li>
                                <a href="center.aspx" target="mainframe">管理中心</a>
                            </li>
                            <li>
                                <a href="admin/baseData/editinfo.aspx" onclick="linkMenuTree(false, '');" target="mainframe">修改密码</a>
                            </li>
                            <li>
                                <asp:LinkButton ID="lbtnExit" runat="server" OnClick="lbtnExit_Click">注销登录</asp:LinkButton>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <div class="main-left">
            <div id="sidebar-nav" class="sidebar-nav"></div>
        </div>
        <div class="main-container">
            <iframe id="mainframe" name="mainframe" frameborder="0" src="center.aspx"></iframe>
        </div>
    </form>
</body>
</html>
