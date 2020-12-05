<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="customer_list.aspx.cs" Inherits="MettingSys.Web.admin.customer.customer_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>客户列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript">
        $(function () {
            $.getJSON("../../tools/business_ajax.ashx?action=getAllCustomer", function (json) {
                $('#txtCusName').devbridgeAutocomplete({
                    lookup: json,
                    minChars: 1,
                    onSelect: function (suggestion) {
                        $('#hCusId').val(suggestion.id);
                    },
                    showNoSuggestionNotice: true,
                    noSuggestionNotice: '抱歉，没有匹配的选项',
                    groupBy: 'type'
                });
            });
            $("#txtCusName").change(function () {
                $("#hCusId").val("");
            });
        })
        var tip_index;
        function showTip(obj, cid) {
            $.get('../../tools/business_ajax.ashx?action=subContacts&cid=' + cid, function (data) {
                tip_index = layer.tips(data, obj, { time: 0 });
            });
        }
        function toggleCheckDiv() {
            $("#updateDiv").hide();
            $("#checkDiv").toggle();
        }
        function toggleUpdateDiv() {
            $("#checkDiv").hide();
            $("#updateDiv").toggle();
        }
        /*批量审批 */
        function submitCheck() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            if ($("#ddlcheck").val() == "") {
                jsprint("请选择审批状态");
                return;
            }
            var idStr = "";
            $(".checkall input:checked").each(function () {
                idStr += $(this).parent().next().val() + ",";
            });
            idStr = idStr.substring(0, idStr.length - 1);

            var postData = { "ids": idStr, "status": $("#ddlcheck").val() };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/Business_ajax.ashx?action=checkCustomerStatus",
                data: postData,
                dataType: "json",
                success: function (data) {
                    if (data.status == 0) {
                        var d = top.dialog({ content: data.msg }).show();
                        setTimeout(function () {
                            d.close().remove();
                            location.reload();
                        }, 2000);
                    } else {
                        top.dialog({
                            title: '提示',
                            content: '错误提示：' + data.msg,
                            okValue: '确定',
                            ok: function () { }
                        }).showModal();
                    }
                }
            });
        }
        /*批量审批 */
        /*合并客户 */
        function mergeCustomer() {
            layer.open({
                type: 2,
                title: '合并客户',
                shadeClose: true,
                shade: false,
                maxmin: false, //开启最大化最小化按钮
                area: ['500px', '300px'],
                shade: [0.8, '#393D49'],
                content: 'customer_merge.aspx'
            });
        }
        /*合并客户 */

        //修改所属人
        function submitUpdate() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            if ($("#txtOwner").val() == "") {
                jsprint("请填写新的所属人");
                return;
            }
            var idStr = "";
            $(".checkall input:checked").each(function () {
                idStr += $(this).parent().next().val() + ",";
            });
            idStr = idStr.substring(0, idStr.length - 1);

            var postData = { "ids": idStr, "owner": $("#txtOwner").val() };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/Business_ajax.ashx?action=updateCustomerOwner",
                data: postData,
                dataType: "json",
                success: function (data) {
                    if (data.status == 0) {
                        var d = top.dialog({ content: data.msg }).show();
                        setTimeout(function () {
                            d.close().remove();
                            location.reload();
                        }, 2000);
                    } else {
                        top.dialog({
                            title: '提示',
                            content: '错误提示：' + data.msg,
                            okValue: '确定',
                            ok: function () { }
                        }).showModal();
                    }
                }
            });
        }
        function addInnerCustomer() {
            layer.open({
                type: 2,
                title: '添加内部客户',
                shadeClose: true,
                shade: false,
                maxmin: false, //开启最大化最小化按钮
                area: ['400px', '300px'],
                shade: [0.8, '#393D49'],
                content: 'InnerCustomer_Add.aspx'
            });
        }
    </script>
</head>

<body class="mainbody">
    <form id="form1" runat="server">
        <!--导航栏-->
        <div class="location">
            <a href="javascript:history.back(-1);" class="back"><i class="iconfont icon-up"></i><span>返回上一页</span></a>
            <a href="../center.aspx" class="home"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>客户列表</span>
        </div>
        <!--/导航栏-->

        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            <div class="toolbar">
                <div class="box-wrap">
                    <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                    <div class="l-list">
                        <ul class="icon-list">
                            <li><a href="customer_edit.aspx?action=<%=DTEnums.ActionEnum.Add %>"><i class="iconfont icon-close"></i><span>新增</span></a></li>
                            <li><a href="javascript:;" onclick="checkAll(this);"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                            <li>
                                <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete','如果该客户已被使用则无法删除，删除客户会一同删除所属的联系人及客户的银行账号，是否继续？');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                            <li id="li1" runat="server"><a href="javascript:;" onclick="toggleCheckDiv()"><span>审批</span></a></li>
                            <li id="li2" runat="server"><a href="javascript:;" onclick="mergeCustomer()"><span>合并客户</span></a></li>
                            <li id="li3" runat="server"><a href="javascript:;" onclick="toggleUpdateDiv()"><span>修改所属人</span></a></li>
                            <li id="li4" runat="server"><a href="javascript:;" onclick="addInnerCustomer()"><span>添加内部客户</span></a></li>
                        </ul>
                        <div id="checkDiv" style="display: none;">
                            审批状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck" runat="server"></asp:DropDownList>
                            </div>
                            <input type="button" class="btn" value="提交" onclick="submitCheck()" />
                        </div>
                        <div id="updateDiv" style="display: none;">
                            新所属人：
                            <asp:TextBox ID="txtOwner" runat="server" CssClass="input small" onkeyup="cToUpper(this)"></asp:TextBox>
                            <input type="button" class="btn" value="提交" onclick="submitUpdate()" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--/工具栏-->
        <div class="searchbar">
            客&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;户：
                            <asp:TextBox ID="txtCusName" runat="server" CssClass="input"></asp:TextBox>
            <asp:HiddenField ID="hCusId" runat="server" />
            客户类别：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddltype" runat="server"></asp:DropDownList>
                            </div>
            客户审批：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcheck1" runat="server"></asp:DropDownList>
                            </div>
            启用状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlisUse" runat="server"></asp:DropDownList>
                            </div>
            所属人：
                            <asp:TextBox ID="txtOwner1" runat="server" CssClass="input small"></asp:TextBox>
            <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                <asp:LinkButton ID="btnExcel" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>
        </div>
        <div class="searchbar">
            业务范围：
                            <asp:TextBox ID="txtBusiness" runat="server" CssClass="input"></asp:TextBox>
        </div>
        <!--列表-->
        <div class="table-container" onload="alert('ddd');jsloading();">
            <asp:Repeater ID="rptList" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr style="text-align: center;">
                            <th width="4%">选择</th>
                            <th width="6%">客户ID</th>
                            <th width="15%">客户名称</th>
                            <th width="6%">客户类别</th>
                            <th>业务范围</th>
                            <th width="10%">信用代码(税号)</th>
                            <th width="6%">所属人</th>
                            <th width="6%">联系人</th>
                            <th width="6%">审批</th>
                            <th width="6%">启用</th>
                            <th width="6%">操作</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr style="text-align: center;">
                        <td>
                            <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                            <asp:HiddenField ID="hidId" Value='<%#Eval("c_id")%>' runat="server" />
                        </td>
                        <td><%#Eval("c_id")%></td>
                        <td style="text-align: left;"><%#Eval("c_name")%></td>
                        <td><%# MettingSys.Common.BusinessDict.customerType()[Utils.ObjToByte(Eval("c_type"))]%></td>
                        <td><%#Eval("c_business")%></td>
                        <td><%#Eval("c_num")%></td>
                        <td><%#Eval("c_owner")%><br/><%#Eval("c_ownerName")%></td>
                        <td style="text-align: left;">
                            <%#string.IsNullOrEmpty(Eval("co_name").ToString())?"暂无":"<span onmouseover=\"showTip(this,"+Eval("c_id")+")\"  onmouseout =\"layer.close(tip_index);\">"+Eval("co_name")+"<br/>"+Eval("co_number")+"</span>" %>
                        </td>
                        <td><span class="check_<%#Eval("c_flag")%>"></span></td>
                        <td><%#MettingSys.Common.BusinessDict.isUseStatus()[Convert.ToBoolean(Eval("c_isUse"))]%></td>
                        <td align="center">
                            <%# Eval("c_flag").ToString()=="2"?"<a href=\"customer_edit.aspx?action="+DTEnums.ActionEnum.View+"&id="+Eval("c_id")+"\">查看</a>": manager.user_name == Eval("c_owner").ToString()?"<a href=\"customer_edit.aspx?action="+DTEnums.ActionEnum.Edit+"&id="+Eval("c_id")+"\">修改</a>": new MettingSys.BLL.permission().checkHasPermission(manager, "0301")?"<a href=\"customer_edit.aspx?action="+DTEnums.ActionEnum.Edit+"&id="+Eval("c_id")+"\">修改</a>":"<a href=\"customer_edit.aspx?action="+DTEnums.ActionEnum.View+"&id="+Eval("c_id")+"\">查看</a>"%>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"11\">暂无记录</td></tr>" : ""%>
  </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <!--/列表-->
        <div class="dRemark">
            <p>1.关键字筛查字段为：客户名称</p>
            <p>2.审批：<span class="check_0"></span>待审批，<span class="check_1"></span>审批未通过，<span class="check_2"></span>审批通过</p>
            <p>3.联系人列为主要联系人，鼠标经过会显示次要联系人</p>
        </div>
        <!--内容底部-->
        <div class="line20"></div>
        <div class="pagelist">
            <div class="l-btns">
                <span>显示</span><asp:TextBox ID="txtPageNum" runat="server" CssClass="pagenum" onkeydown="return checkNumber(event);"
                    OnTextChanged="txtPageNum_TextChanged" AutoPostBack="True"></asp:TextBox><span>条/页</span>
            </div>
            <div id="PageContent" runat="server" class="default"></div>
        </div>
        <!--/内容底部-->

    </form>
</body>
</html>
