<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="order_place1.aspx.cs" Inherits="MettingSys.Web.admin.order.order_place1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>活动归属地</title>
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript">
        var api = top.dialog.get(window);; //获取父窗体对象
        $(function () {
            //设置窗口按钮及事件
            api.button([{
                value: '确定',
                callback: function () {
                    if (!checkNum()) {
                        layer.msg("业绩比例须为大于0,小于等于100的正整数,且业绩比例之和必须等于100");
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
            //设置按钮事件
            //$(".spec-item li a").click(function () {
            //    if ($(this).parent().attr("data-type") == "1") {
            //        layer.msg("业务员的归属区域不能取消");
            //    }
            //    else {
            //        if ($(this).parent().hasClass("selected")) {
            //            $(this).parent().removeClass("selected").attr("data-type", "");
            //        } else {
            //            $(this).parent().addClass("selected").attr("data-type", "0");
            //            $(this).children().select();
            //        }
            //        catulateNum();
            //    }
            //});

            //初始化已选择的区域
            $(api.data).parent().find("input[name='hide_place']").each(function () {
                var that = $(this);
                var list = that.val().split('-');
                var hideId = list[0];
                $(".spec-item li").each(function () {
                    if ($(this).attr("id") == hideId) {
                        $(this).attr("data-type", list[3]);
                        if (list[3] == "1") {
                            $(this).find("input").val(list[2]).attr("disabled", true);
                        }
                        else {
                            $(this).find("input").val(list[2]);
                        }
                    }
                });
            });
            //更改业绩比例
            $(".ratioInput").blur(function () {
                var thisRatio = $(this).val();
                if ($(this).val() != "") {
                    if (!(/(^[1-9]\d*$)/.test(thisRatio)) || thisRatio <= 0 || thisRatio > 100) {
                        layer.msg("业绩比例须为正整数，且大于0小于100");
                        $(this).val("");
                        $(this).focus();
                        return
                    }
                }
                catulateNum();
            });
            $(".ratioInput").click(function (e) {
                //如果提供了事件对象，则这是一个非IE浏览器 
                if (e && e.stopPropagation)
                //因此它支持W3C的stopPropagation()方法 
                {
                    e.stopPropagation();
                }
                else
                //否则，我们需要使用IE的方式来取消事件冒泡 
                {
                    window.event.cancelBubble = true;
                }
                $(this).select();
            });
        });

        //计算业绩比例
        function catulateNum() {
            var homeImput = $(".spec-item").children("li[data-type=1]").children().children();
            var tRatio = 0;
            $(".spec-item").children("li[data-type=0]").each(function () {
                var v = $(this).children().children().val();
                if (v != "") {
                    tRatio += parseInt(v);
                }
                else {
                    tRatio += 0;
                }
            });
            homeImput.val(100 - tRatio);
        }
        //验证业绩比例
        function checkNum() {
            var tRatio = 0;
            $(".spec-item li").each(function () {
                if ($(this).children().children().val() != "") {
                    var v = parseInt($(this).children().children().val());
                    if ($(this).attr("data-type") == "1") {
                        if ((/(^[0-9]\d*$)/.test(v)) && v >= 0 && v <= 100) {
                            tRatio += v;
                        }
                    }
                    else {
                        if ((/(^[1-9]\d*$)/.test(v)) && v > 0 && v <= 100) {
                            tRatio += v;
                        }
                    }
                }
            });           
            if (tRatio != 100) {
                return false;
            }
            return true;
        }
        //插入区域节点
        function appendSpecHtml() {            
            $(api.data).siblings("li").remove(); //先删除所有同辈节点
            $(".spec-item li").each(function () {
                if ($(this).children().children().val() != "") {
                    execSpecHtml($(this).attr("title"), $(this).attr("id"), $(this).children().children().val(), $(this).attr("data-type"));
                }
            });
        }

        //创建区域的HTML
        function execSpecHtml(title, id,ratio,type) {
            var liHtml = '<li data-type=' + type + '><input name="hide_place" type="hidden" value="' + id + '-' + title + '-' + ratio + '-' + type + '" />';
            if (type == "0") {
                liHtml += '<a href="javascript:;" class="del" title="删除" onclick="delNode(this,' + ratio+');"><i class="iconfont icon-remove"></i></a>';
            }
            liHtml += '<span>' + title + '</span>(<span class="ratioText">' + ratio + '</span>%)</li>';
            $(api.data).before(liHtml);
        }
    </script>
    <style type="text/css">
        .spec-item li a {
            padding:5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="width: 500px; overflow: auto; padding: 10px;">
            <div class="div-content">
                <ul class="spec-item">
                    <asp:Repeater ID="rptList" runat="server" OnItemDataBound="rptList_ItemDataBound">
                        <ItemTemplate>
                            <li id="<%#Eval("key")%>" title="<%#Eval("value")%>" data-type="0">
                                <a href="javascript:;"><%#Eval("value")%><input type="text" style="margin-left:5px;width: 40px; text-align:right;" class="input small ratioInput" />%</a>
                                
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </div>
        <div style="font-size: 12px;margin-left: 10px;">注：业绩比例须为大于0,小于等于100的正整数,且业绩比例之和必须等于100。</div>
    </form>
</body>
</html>
