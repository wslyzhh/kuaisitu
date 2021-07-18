<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="order_person.aspx.cs" Inherits="MettingSys.Web.admin.order.order_person" %>

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
    <script type="text/javascript">
        var api = top.dialog.get(window);; //获取父窗体对象
        $(function () {
            //查询员工
            $("#btnsearch").click(function () {
                var _wrod = $("#txtWord").val();
                if (_wrod != "") {
                    var postData = { wrod: _wrod };
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/Order_ajax.ashx?action=searchPerson",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.msg == "成功") {
                                $("#smsg").text("工号：" + data.username + "，姓名：" + data.realname + "，区域：" + data.area);
                                $("#huserid").val(data.username + "-" + data.realname + "-" + data.area);
                            } else {
                                $("#smsg").text(data.msg);
                                $("#huserid").val("");
                            }
                        }
                    });
                }
            });
            //添加员工
            $("#btnadd").click(function () {
                var userInfo = $("#huserid").val();
                if (userInfo == "") {
                    layer.msg("请先查找员工");
                    return;
                }
                var userInfoList = userInfo.split('-');
                var flag = true;
                //检查是否已存在
                $(".spec-item li").each(function () {
                    if ($(this).attr("data-id") == userInfoList[0]) {
                        layer.msg("该员工" + userInfoList[0] + "(" + userInfoList[1] + ")已存在");
                        flag = false;
                    }
                });
                if (flag) {
                    var uHtml = '<li data-id="' + userInfoList[0] + '" data-title="' + userInfoList[1] + '" data-area="' + userInfoList[2] + '" class="selected">'
                        + '<a href = "javascript:;" onclick="addSelect(this)"> ' + userInfoList[0] + '(' + userInfoList[1] + ')</a><input type="text" style=" width: 40px; text-align: right;" class="input small ratioInput" /> %'
                        + '</li >';
                    $(".spec-item").append(uHtml);
                    $("#huserid").val("");
                    $("#smsg").text("");
                    //更改业绩比例
                    $(".ratioInput").blur(function () {
                        //只有选中的区域更改业绩比例才有效
                        if ($(this).parent().hasClass("selected")) {
                            var thisRatio = $(this).val();
                            if (!(/(^[1-9]\d*$)/.test(thisRatio)) || thisRatio <= 0 || thisRatio >= 100) {
                                layer.msg("业绩比例须为正整数，且大于0小于100");
                                $(this).val("");
                                return;
                            }
                        }
                    });
                }
            });
            //设置窗口按钮及事件
            api.button([{
                value: '确定',
                callback: function () {
                    if (!checkNum()) {
                        layer.msg("业绩比例须为大于0,小于等于100的正整数,且业绩比例之和必须小于100");
                        return false;
                    }
                    else {
                        appendSpecHtml();
                    }
                },
                autofocus: true
            }, {
                value: '取消',
                callback: function () { }
            }
            ]);
            

            //初始化已选择的区域
            $(api.data).parent().find("input[name='hide_employee6']").each(function () {
                var that = $(this);
                var list = that.val().split('-');

                var uHtml = '<li data-id="' + list[1] + '" data-title="' + list[0] + '" data-area="' + list[2] + '" class="selected">'
                    + '<a href = "javascript:;" onclick="addSelect(this)"> ' + list[0] + '(' + list[1] + ')</a><input type="text" style="width:40px;text-align:right;" class="input small ratioInput" value="' + list[3]+'" /> %'
                    + '</li >';
                $(".spec-item").append(uHtml);
            });
            
        });
        //点击选中事件
        function addSelect(obj) {
            if ($(obj).parent().hasClass("selected")) {
                $(obj).parent().removeClass("selected");
            } else {
                $(obj).parent().addClass("selected");
                //$(this).children().select();
            }
        }
        //验证业绩比例
        function checkNum() {
            var tRatio = 0;
            $(".spec-item li").each(function () {
                if ($(this).hasClass("selected")) {
                    var v = parseInt($(this).children(".ratioInput").val());
                    if (!(/(^[1-9]\d*$)/.test(v)) || v <= 0 || v >= 100) {
                        return false;
                    }
                    else {
                        tRatio += v;
                    }
                }
            });
            if (tRatio >= 100) {
                return false;
            }
            return true;
        }
        //插入区域节点
        function appendSpecHtml() {

            $(api.data).siblings("li").remove(); //先删除所有同辈节点
            $(".spec-item li").each(function () {
                if ($(this).hasClass("selected")) {
                    execSpecHtml($(this).attr("data-title"), $(this).attr("data-id"), $(this).children(".ratioInput").val(), $(this).attr("data-area"));
                }
            });
        }

        //创建区域的HTML
        function execSpecHtml(title, id, ratio, area) {
            var liHtml = '<li data-type=' + id + '><input name="hide_employee6" type="hidden" value="' + title + '-' + id + '-' + area + '-' + ratio + '" />';
                liHtml += '<a href="javascript:;" class="del" title="删除" onclick="delNode(this,' + ratio + ');"><i class="iconfont icon-remove"></i></a>';
                liHtml += '<span>' + id + title + '</span>(<span class="ratioText">' + ratio + '</span>%)</li>';
                $(api.data).before(liHtml);
        }
    </script>
    <style type="text/css">
        .spec-item li a {
            display: inline;
            padding: 7px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="width: 500px; overflow: auto; padding: 10px;">
            <input type="text" id="txtWord" style="width: 110px; font-size: 10px;" class="input " placeholder="输入工号或者姓名" />
            <input id="btnsearch" runat="server" type="button" value="查询" class="btn" />
            <input id="btnadd" runat="server" type="button" value="添加" class="btn" />
            <input type="hidden" id="huserid" value="" />
            <span id="smsg" style="font-size: 12px; margin-left: 2px;"></span>
            <div class="div-content">
                <ul class="spec-item">

                </ul>
            </div>
        </div>
        <div style="font-size: 12px; margin-left: 10px;">注：业绩比例须为大于0,小于等于100的正整数,且业绩比例之和必须小于100。</div>
    </form>
</body>
</html>
