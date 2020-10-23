<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="selectEmployee.aspx.cs" Inherits="MettingSys.Web.admin.selectEmployee" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="skin/default/style.css" />
    <script type="text/javascript" src="../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="js/common.js"></script>
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
            $('.tree-list').initCategoryTree(true);

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

            //初始化已选择人员
            $(api.data).parent().find("input[name='hide_employee" + api.n + "']").each(function () {
                var hideId = $(this).val();
                var list = hideId.split("|");
                var li = $("<li title='" + list[1] + "' tip='" + list[2] + "'>" + list[0] + "</li>").click(function () {
                    $(this).remove();
                });
                //if (api.n == 2 || api.n == 4) {
                //    li = $("<li title='" + list[1] + "' tip='" + list[2] + "'>" + list[0] + "</li>").click(function () {
                //        $(this).remove();
                //    });
                //}
                //else {
                //    li = $("<li title='" + list[1] + "' tip='" + list[2] + "'>" + list[0] + "</li>").click(function () {
                //        $(this).remove();
                //    });
                //}

                $("#employeelist").append(li);
            });
        });

        //插入人员
        function appendSpecHtml() {
            $(api.data).siblings("li").remove(); //先删除所有同辈节点
            $("#employeelist").children().each(function () {
                execSpecHtml($(this).text(), $(this).attr("title"), $(this).attr("tip"));
            });
        }

        //创建人员的HTML
        function execSpecHtml(name, num, area) {
            var liHtml = '<li title="' + num + '">'
                + '<input name="hide_employee' + api.n + '" type="hidden" value="' + name + '|' + num + '|' + area + '' + (api.showDstatus &&(api.n == 2 || api.n == 4 )? '|0' : '') + '" />'
                + '<a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>'
                + '<span>' + name + '' + (api.showDstatus &&(api.n == 2 || api.n == 4 )? '(<font style="color: brown;">待定</font>)' : '') + '</span>'
                + '</li>';
            $(api.data).before(liHtml);
        }
        //点击选择人员
        function addemployee(type, username, realname, area,orderCount) {
            if (type == "4") {
                var tag = false;
                $("#employeelist").children().each(function () {
                    if ($(this).attr("title") == username) {
                        tag = true;
                        return;
                    }
                });
                if (!tag) {
                    if (api.multi == false && $("#employeelist").children().length == 1) {
                        var d = dialog({ content: "只能选择一个人员" }).show();
                        setTimeout(function () {
                            d.close().remove();
                        }, 1000);
                        return;
                    }
                    
                    if (orderCount > 2) {
                        layer.confirm("<font color='red'>该员工目前工作饱和，请经他（她）同意后再确认！</font>", {
                            btn: ['确定选择', '放弃选择'] //按钮
                        }, function (index) {
                            layer.close(index);
                            var li = $("<li title='" + username + "' tip='" + area + "'>" + realname + "</li>").click(function () {
                                $(this).remove();
                            });
                            $("#employeelist").append(li);
                        }, function () { }
                        );
                    }
                    else {
                        var li = $("<li title='" + username + "' tip='" + area + "'>" + realname + "</li>").click(function () {
                            $(this).remove();
                        });
                        $("#employeelist").append(li);
                    }
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
            <div class="tree-list" style="float: left; min-width: 300px; height: 500px; width: 400px; border: none; border-right: 1px solid #f1f1f1; overflow: auto;">
            <asp:TextBox ID="txtPerson" runat="server" style="margin-left: 20px; width:200px;" CssClass="input " placeholder="输入员工工号或者姓名"></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                <div class="thead">
                    <div class="col col-1" style="padding-left: 20px;">组织机构</div>
                </div>
                <ul style="margin-left: 20px;">
                    <%if (IsshowNum)
                        { if (rptList.Items.Count > 0)
                            { %>
                    <asp:Repeater ID="rptList" runat="server">
                        <ItemTemplate>
                            <li class="layer-<%#Eval("class_layer")%>">
                                <div class="tbody">
                                    <div class="col index col-1">
                                        <a href="javascript:" onclick="addemployee('<%#Eval("de_type")%>','<%#Eval("de_subname")%>','<%#Eval("de_name")%>','<%#Eval("de_area")%>',<%#Eval("orderCount")%>)">
                                            
                                            <%# Eval("de_type").ToString()!="4" ? Eval("de_name")
                                                    :(Convert.ToInt32(Eval("orderCount"))<1?"<font color='green'>"+Eval("de_name")+"-"+Eval("de_subname")+"</font>":(Convert.ToInt32(Eval("orderCount"))>2?"<font color='red'>"+Eval("de_name")+"-"+Eval("de_subname")+"</font>":"<font color='orange'>"+Eval("de_name")+"-"+Eval("de_subname")+"</font>"))+"("+Eval("detailDepart")+") "
                                                    + (Convert.ToInt32(Eval("orderCount"))<1?"<font color='green'>"+Eval("orderCount")+"</font>":(Convert.ToInt32(Eval("orderCount"))>2?"<font color='red'>"+Eval("orderCount")+"</font>":"<font color='orange'>"+Eval("orderCount")+"</font>")) %>
                                                                                        
                                        </a>
                                    </div>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                    <%}else{ %>
                       <li style="margin-top:10px;">暂无记录...</li>
                    <%} %>
                    <%}else{ if(rptList1.Items.Count > 0){ %>
                    <asp:Repeater ID="rptList1" runat="server">
                        <ItemTemplate>
                            <li class="layer-<%#Eval("class_layer")%>">
                                <div class="tbody">
                                    <div class="col index col-1">
                                        <a href="javascript:" onclick="addemployee('<%#Eval("de_type")%>','<%#Eval("de_subname")%>','<%#Eval("de_name")%>','<%#Eval("de_area")%>')">
                                            
                                            <%# Eval("de_type").ToString()!="4" ? Eval("de_name"):Eval("de_name")+"-"+Eval("de_subname")+"("+Eval("detailDepart")+") " %>
                                                                                        
                                        </a>
                                    </div>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                    <%}else{ %>
                       <li style="margin-top:10px;">暂无记录...</li>
                    <%} %>
                    <%} %>
                </ul>
            </div>
            <div class="chooseEmployee" style="float: left; width: 200px;">
                <div class="thead">
                    <div class="col col-1">已选择员工</div>
                </div>
                <ul id="employeelist">
                </ul>
            </div>
        </div>
    </form>
</body>
</html>
