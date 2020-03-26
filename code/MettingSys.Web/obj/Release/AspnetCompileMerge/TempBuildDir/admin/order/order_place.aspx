<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="order_place.aspx.cs" Inherits="MettingSys.Web.admin.order.order_place" %>

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
                    appendSpecHtml();
                },
                autofocus: true
            }, {
                value: '取消',
                callback: function () { }
            }
            ]);

            //设置按钮事件
            $(".spec-item li a").click(function () {
                if ($(this).parent().hasClass("selected")) {
                    $(this).parent().removeClass("selected");
                } else {
                    $(this).parent().addClass("selected");
                }
            });

            //初始化已选择的区域
            $(api.data).parent().find("input[name='hide_place']").each(function () {
                var hideId = $(this).val();
                $(".spec-item li").each(function () {
                    if (!$(this).hasClass("selected") && $(this).attr("id") == hideId) {
                        $(this).addClass("selected");
                    }
                });
            });
        });

        //插入区域节点
        function appendSpecHtml() {
            $(api.data).siblings("li").remove(); //先删除所有同辈节点
            $(".spec-item li").each(function () {
                if ($(this).hasClass("selected")) {
                    execSpecHtml($(this).attr("title"), $(this).attr("id"));
                }
            });
        }

        //创建区域的HTML
        function execSpecHtml(title, id) {
            var liHtml = '<li>'
                + '<input name="hide_place" type="hidden" value="' + id + '" />'
                + '<a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>'
                + '<span>' + title + '</span>'
                + '</li>';
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
        <div style="width: 500px; height: 80px; overflow: auto; padding: 10px;">
            <div class="div-content">
                <ul class="spec-item">
                    <asp:Repeater ID="rptList" runat="server" OnItemDataBound="rptList_ItemDataBound">
                        <ItemTemplate>
                            <li id="<%#Eval("key")%>" title="<%#Eval("value")%>">
                                <a href="javascript:;"><%#Eval("value")%></a>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </div>
    </form>
</body>
</html>
