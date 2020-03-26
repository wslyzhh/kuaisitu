<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="selectPermission.aspx.cs" Inherits="MettingSys.Web.admin.manage.selectPermission" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <style type="text/css">
        .tree-list .col-1 {
            text-align: center;
        }

        .tree-list li .tbody {
            border-bottom: none;
        }

        .chooseEmployee .thead {
            padding: 8px 0 8px 20px;
            color: #333;
            font-size: 12px;
            font-weight: 500;
            line-height: 1.5em;
            border-bottom: 1px solid #eee;
        }

        .chooseEmployee ul {
            display: block;
            padding-left: 20px;
        }

            .chooseEmployee ul li {
                padding-top: 5px;
                cursor: pointer;
            }
    </style>
    <script type="text/javascript">
        var api = top.dialog.get(window);; //获取父窗体对象
        $(function () {
            //初始化分类的结构
            initCategoryHtml('.tree-list', 1);
            //初始化分类的事件
            $('.tree-list').initCategoryTree(false);

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

            //初始化已选择权限
            $(api.data).parent().find("input[name='hide_permission']").each(function () {
                var hideId = $(this).val();
                var li = $("<li title='" + hideId + "'>" + hideId + "</li>").click(function () {
                    $(this).remove();
                });;
                $("#employeelist").append(li);
            });
        });

        //插入权限
        function appendSpecHtml() {
            $(api.data).siblings("li").remove(); //先删除所有同辈节点
            var codeStr = ",";
            $("#employeelist").children().each(function (index) {
                codeStr += $(this).attr("title") + ',';
                execSpecHtml($(this).attr("title"));
            });
            $(api.hcode).val(codeStr);
        }

        //创建权限的HTML
        function execSpecHtml(code) {
            var liHtml = "<li>"
                + "<input name='hide_permission' type='hidden' value='" + code + "' />"
                + "<a href='javascript:;' class='del' title='删除' onclick='delNode(this,\""+code+"\");'><i class='iconfont icon-remove'></i></a>"
                + "<span>" + code + "</span>"
                + "</li>";
            $(api.data).before(liHtml);
        }
        //点击选择权限
        function addemployee(parentid,code,name) {
            if (parentid != "0") {
                var tag = false;
                $("#employeelist").children().each(function () {
                    if ($(this).attr("title") == code) {
                        tag = true;
                        return;
                    }
                });
                if (!tag) {
                    var li = $("<li title='" + code + "'>" + code+"</li>").click(function () {
                        $(this).remove();
                    });;
                    $("#employeelist").append(li);
                }
                else {
                    var d = dialog({ content: "已选择" }).show();
                    setTimeout(function () {
                        d.close().remove();
                    }, 1000);
                }
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="table-container" style="width: 650px;">
            <div class="tree-list" style="float: left; min-width: 300px; height: 500px; width: 300px; border: none; border-right: 1px solid #f1f1f1; overflow: auto;">
                <div class="thead">
                    <div class="col col-1" style="padding-left: 20px;">权限</div>
                </div>
                <ul style="margin-left: 20px;">
                    <asp:Repeater ID="rptList" runat="server">
                        <ItemTemplate>
                            <li class="layer-<%#Eval("class_layer")%>">
                                <div class="tbody">
                                    <div class="col index col-1">
                                        <a href="javascript:" onclick="addemployee(<%#Eval("pe_parentid")%>,'<%#Eval("pe_code")%>','<%#Eval("pe_name")%>')"><%#Eval("pe_code")%><%#Eval("pe_name")%></a>
                                    </div>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
            <div class="chooseEmployee" style="float: left; width: 300px;">
                <div class="thead">
                    <div class="col col-1">已选择权限</div>
                </div>
                <ul id="employeelist">
                </ul>
            </div>
        </div>
    </form>
</body>
</html>