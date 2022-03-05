<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="selectEmployee.aspx.cs" Inherits="MettingSys.Web.admin.order.selectEmployee" %>
<%@ Import Namespace="MettingSys.Common" %>
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
            line-height: 32px;
            cursor: pointer;
            margin-top: 5px;
            height: 33px;
        }

        .chooseEmployee ul li input {
            
        }

        .spanUser {
            display: block;
            float: left;
            width: 67%;
        }
        .spanSign {
            display: block;
            float: right;
            padding-top: 2px;
        }
    </style>
    <script type="text/javascript">
        var totalRatio = 100;
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
                    if (!checkNum()) {
                        if ($("#hemployeeType").val() == "6") {
                            layer.msg("业绩比例须为大于0,小于等于" + totalRatio + "的正整数,且业绩比例之和必须小于" + totalRatio + "");
                        }
                        else {
                            if ($("#hratio3").val() != "undefined") {
                                layer.msg("业绩比例之和必须等于" + totalRatio + "");                           
                            }
                        }
                        return false;
                    }
                    if ($("#hemployeeType").val() == "3") {
                        //检查共同业务员和执行人员的比例之和必须小于100
                        if (parseInt($("#hratio6").val()) + parseInt(totalRatio) >= 100) {
                            layer.msg("共同业务员和执行人员的比例之和必须小于100");
                            return false;
                        }
                    }
                    appendSpecHtml();
                },
                autofocus: true
            }, {
                value: '取消',
                callback: function () { }
            }
            ]);

            //初始化已选择人员
            $(api.data).parent().find("input[name='hide_employee" + $("#hemployeeType").val()+"']").each(function () {
                var hideId = $(this).val();
                var list = hideId.split("-");
                var li = $("<li title='" + list[1] + "' tip='" + list[2] + "'><span onclick='delLi(this)' class='spanUser'>" + list[0] + "</span><input type='text' style='width: 40px; text - align: right;' class='input small ratioInput' value='" + (!list[3]? '' : list[3]) + "'/><span class='spanSign'>%</span></li>");
                //执行人员界面，且没有符合条件
                if (list.length<4 && $("#hemployeeType").val() == "3" && $("#hratio3").val() == "undefined") {
                    li = $("<li title='" + list[1] + "' tip='" + list[2] + "'><span onclick='delLi(this)' class='spanUser'>" + list[0] + "</span></li>");
                }
                $("#employeelist").append(li);
                checkInputValue();
            });

            if ($("#hemployeeType").val() == 3) {
                totalRatio = $("#hratio3").val() == "undefined" ? 0 : $("#hratio3").val();
            }
        });

        //插入人员
        function appendSpecHtml() {
            $(api.data).siblings("li").remove(); //先删除所有同辈节点
            $("#employeelist").children().each(function () {
                execSpecHtml($(this).children(".spanUser").text(), $(this).attr("title"), $(this).children("input").val(), $(this).attr("tip"));
            });
        }

        //创建人员的HTML
        function execSpecHtml(name, num, ratio, area) {
            var liHtml = '<li title="' + num + '">'
                + '<input name="hide_employee' + $("#hemployeeType").val() + '" type="hidden" value="' + name + '-' + num + '-' + area + '-' + ratio + '" />'
                + '<a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>'
                + '<span>' + num + name + '</span>(<span class="ratioText">' + ratio + '</span>%)'
                + '</li>';
            if ($("#hemployeeType").val() == "3" && $("#hratio3").val() == "undefined") {
                liHtml = '<li title="' + num + '">'
                    + '<input name="hide_employee' + $("#hemployeeType").val() + '" type="hidden" value="' + name + '-' + num + '-' + area + '" />'
                    + '<a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>'
                    + '<span>' + num + name + '</span>'
                    + '</li>';
            }
            $(api.data).before(liHtml);
        }
        //点击选择人员
        function addemployee(type, username, realname, area, orderCount) {
            if (type == "4") {
                var tag = false;
                $("#employeelist").children().each(function () {
                    if ($(this).attr("title") == username) {
                        tag = true;
                        return;
                    }
                });
                if (!tag) {
                    var li = $("<li title='" + username + "' tip='" + area + "'><span onclick='delLi(this)' class='spanUser'>" + realname + "</span><input type='text' style='width: 40px; text - align: right;' class='input small ratioInput'/><span class='spanSign'>%</span></li>");
                    //执行人员界面，且没有符合条件
                    if ($("#hemployeeType").val() == "3" && $("#hratio3").val() == "undefined") {
                        li = $("<li title='" + username + "' tip='" + area + "'><span onclick='delLi(this)' class='spanUser'>" + realname + "</span></li>");
                    }
                    $("#employeelist").append(li);
                    checkInputValue();
                }
                else {
                    var d = dialog({ content: "已选择" }).show();
                    setTimeout(function () {
                        d.close().remove();
                    }, 1000);
                }
            }
        }
        function delLi(obj) {
            $(obj).parent().remove();
        }

        function checkInputValue() {
            //更改业绩比例
            $(".ratioInput").blur(function () {
                var thisRatio = $(this).val();
                if (!(/(^[1-9]\d*$)/.test(thisRatio)) || thisRatio <= 0 || thisRatio >= 100) {
                    layer.msg("业绩比例须为正整数，且大于0小于" + totalRatio);
                    $(this).val("");
                    return;
                }
            });
        }
        //验证业绩比例
        function checkNum() {
            var tRatio = 0;
            var t = false;
            $("#employeelist li").each(function () {
                var v = parseInt($(this).children(".ratioInput").val());
                if ($("#hemployeeType").val() == "6") {
                    if (!(/(^[1-9]\d*$)/.test(v)) || v <= 0 || v >= totalRatio) {
                        //return false;
                        t = false;
                    }
                    else {
                        tRatio += v;
                        t = true;
                    }
                }
                else {
                    if ($("#hratio3").val() == "undefined") {
                        t = true;
                    }
                    else {
                        if (!(/(^[1-9]\d*$)/.test(v)) || v <= 0 || v > totalRatio) {
                            //return false;
                            t = false;
                        }
                        else {
                            tRatio += v;
                            t = true;
                        }
                    }
                }
            });
            if (t) {
                if ($("#hemployeeType").val() == "6") {
                    if (tRatio >= totalRatio || tRatio <= 0) {
                        return false;
                    }
                    return true;
                }
                else {
                    if ($("#hratio3").val() == "undefined") {
                        return true;
                    }
                    if (tRatio != totalRatio) {
                        return false;
                    }
                    
                    return true;
                }
            }
            return false;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="table-container" style="width: 650px;">
            <div class="tree-list" style="float: left; min-width: 300px; height: 500px; width: 400px; border: none; border-right: 1px solid #f1f1f1; overflow: auto;">
                <asp:TextBox ID="txtPerson" runat="server" Style="margin-left: 20px; width: 200px;" CssClass="input " placeholder="输入员工工号或者姓名"></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                <div class="thead">
                    <div class="col col-1" style="padding-left: 20px;">组织机构</div>
                </div>
                <ul style="margin-left: 20px;">
                    <%if (rptList1.Items.Count > 0)
                        { %>
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
                    <%}
                    else
                    { %>
                    <li style="margin-top: 10px;">暂无记录...</li>
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
            <input type="hidden" id="hemployeeType" value ="<%=DTRequest.GetQueryString("type") %>" />
            <input type="hidden" id="hratio3" value ="<%=DTRequest.GetQueryString("ratio3") %>" />
            <input type="hidden" id="hratio6" value ="<%=DTRequest.GetQueryString("ratio6") %>" />
        </div>
    </form>
</body>
</html>
