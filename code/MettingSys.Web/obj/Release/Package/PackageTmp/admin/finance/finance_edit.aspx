<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="finance_edit.aspx.cs" Inherits="MettingSys.Web.admin.finance.finance_edit" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title><%=action==DTEnums.ActionEnum.Add.ToString()?"添加":"编辑" %><%=typeText %>记录</title>
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery.form.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/Validform_v5.3.2_min.js"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <style type="text/css">
        .date-input {
            width: 100px;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            //初始化表单验证
            $("#form1").initValidform();
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
            $("#ddlnature").change(function () {
                if ($(this).val() != "") {
                    //业务性质为员工费用时，不限制业务日期范围
                    //if ($(this).val() == "17") {
                    //    $("#txtsDate").focus(function () {
                    //        bindsWdatePicker(false);
                    //    });
                    //    $("#txteDate").focus(function () {
                    //        bindeWdatePicker(false);
                    //    });
                    //}
                    //else {
                    //    $("#txtsDate").focus(function () {
                    //        bindsWdatePicker(true);
                    //    });
                    //    $("#txteDate").focus(function () {
                    //        bindeWdatePicker(true);
                    //    });
                    //}


                    var postData = { id: $(this).val() };
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/business_ajax.ashx?action=getBusinessDetail",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            var list = eval(data);
                            if (list.length == 1 && list[0]["type"] == 1) {
                                $("#detail1").hide();
                                $("#detail2").show();
                            }      
                            else { 
                                $("#detail2").hide();
                                $("#detail1").show();
                                if (list.length > 0) {
                                    //加载联系人
                                    $("#ddldetail").html("");
                                    var _ul = $("#ddldetail").prev().find(".select-items").first().children().first();
                                    _ul.html("");
                                    $.each(list, function (index, item) {
                                        $("#ddldetail").append("<option value='" + item.de_name + "'>" + item.de_name + "</option>");
                                        _ul.append("<li>" + item.de_name + "</li>");
                                    });
                                    //需要重新初始化下拉插件
                                    $(".rule-single-select").ruleSingleSelect();
                                }
                                else {
                                    $("#ddldetail").html("");
                                    $("#ddldetail").append("<option value=''>暂无明细</option>");
                                }
                            }
                        }
                    });
                }
            });
            var ajaxFormOption = {
                dataType: "json", //数据类型  
                success: function (data) { //提交成功的回调函数  
                    if (data.status == 0) {
                        if (data.fromorder == "true") {
                            var d = top.dialog({ content: data.msg }).show();
                            setTimeout(function () {
                                d.close().remove();
                                parent.location.reload();
                            }, 1500);
                        }
                        else {
                            var d = top.dialog({ content: data.msg }).show();
                            setTimeout(function () {
                                d.close().remove();
                                location.href = 'finance_list.aspx?type=' + data.type;
                            }, 1500);
                        }
                    } else {
                        top.dialog({
                            title: '提示',
                            content: data.msg,
                            okValue: '确定',
                            ok: function () {
                                
                            }
                        }).showModal();
                    }
                }
            };
            //提交
            $("#btnSave").click(function () {
                printLoad();
                $("#form1").ajaxSubmit(ajaxFormOption);
            });
        });
        function computeResult() {
            var expression = $("#txtExpression").val();
            $.getJSON("../../tools/business_ajax.ashx?action=computeResult&expression=" + encodeURIComponent(expression), function (json) {
                if (json.status == 1) {
                    $("#txtMoney").val(json.msg);
                }
            });
        }
        <%--function bindsWdatePicker() {
            WdatePicker({ dateFmt: 'yyyy-MM-dd' ,maxDate: '<%=maxDate%>'});
        }--%>
        <%--function bindeWdatePicker() {
            WdatePicker({ dateFmt: 'yyyy-MM-dd', minDate: '#F{$dp.$D(\'txtsDate\')}', maxDate: '<%=maxDate%>' });
        }--%>
        //人员选择
        function chooseEmployee(obj, n, flag,isShow) {
            //业务报账员和业务执行人员必须在选择活动归属地后才能选择
            var area = "";
            var liObj = $(obj).parent();
            var d = top.dialog({
                id: 'specDialogId',
                padding: 0,
                title: "选择员工",
                url: 'admin/selectEmployee.aspx?area=' + area + ''
            }).showModal();
            //将容器对象传进去
            d.data = liObj;
            d.n = n;//容器序号，一个页面上有多个容器时要传
            d.multi = flag;//true可以选择多个人,false只能选一个人
            d.showDstatus = isShow;//true显示接单状态，false不显示接单状态
        }
        //删除附件节点
        function delNode(obj) {
            $(obj).parent().remove();
        }
    </script>
</head>

<body class="mainbody">
    <form id="form1" runat="server" action="../../tools/business_ajax.ashx?action=saveFinance" method="post" enctype="multipart/form-data">
        <!--导航栏-->
        <div class="location" style="<%=fromOrder=="true"?"display:none;":"" %>">
            <a href="finance_list.aspx?type=<%=type %>" class="back"><i class="iconfont icon-up"></i><span>返回列表页</span></a>
            <a href="../center.aspx"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <a href="finance_list.aspx?type=<%=type %>"><span><%=typeText %>列表</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span><%=action==DTEnums.ActionEnum.Add.ToString()?"添加":"编辑" %><%=typeText %>记录</span>
        </div>
        <div class="line10"></div>
        <!--/导航栏-->

        <!--内容-->
        <div id="floatHead" class="content-tab-wrap" style="<%=fromOrder=="true"?"display:none;":"" %>">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a class="selected" href="javascript:;"><%=typeText %>记录</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="tab-content" style="<%=fromOrder=="true"?"border:none;padding:0;":"" %>">
            <dl>
                <dt>订单号</dt>
                <dd>
                    <%=oID %>
                </dd>
            </dl>
            <dl>
                <dt><%=typeText%>对象</dt>
                <dd>
                    <asp:TextBox ID="txtCusName" runat="server" CssClass="input normal" nullmsg="必填" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <asp:HiddenField ID="hCusId" runat="server" />
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>业务性质</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlnature" runat="server"></asp:DropDownList>
                    </div>
                    <asp:Label ID="labnature" runat="server"></asp:Label>
                </dd>
            </dl>
            <dl id="detail1" runat="server">
                <dt>业务明细</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddldetail" runat="server">
                            <asp:ListItem Value="">请先选择业务性质</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </dd>
            </dl>
            <dl id="detail2" runat="server" style="display:none;">
                <dt>业务明细</dt>
                <dd>
                    <div class="txt-item">
                        <ul>
                            <asp:Repeater ID="rptEmployee2" runat="server">
                                <ItemTemplate>
                                    <li title="<%#Eval("[\"op_number\"]")%>">
                                        <input name="hide_employee2" type="hidden" value="<%#Eval("[\"op_name\"]")%>|<%#Eval("[\"op_number\"]")%>|<%#Eval("[\"op_area\"]")%>" />
                                        <a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                        <span><%#Eval("[\"op_name\"]")%></span>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="icon-btn">
                                <a href="javascript:" onclick="chooseEmployee(this,2,true,false)"><i class="iconfont icon-close"></i></a>
                            </li>
                        </ul>
                    </div>
                </dd>
            </dl>
            <dl>
                <dt>业务说明</dt>
                <dd>
                    <asp:TextBox ID="txtIllustration" runat="server" CssClass="input normal" TextMode="MultiLine" />
                </dd>
            </dl>
            <dl>
                <dt>金额表达式</dt>
                <dd>
                    <asp:TextBox ID="txtExpression" runat="server" CssClass="input normal" datatype="*2-200" sucmsg=" " onblur="computeResult()"></asp:TextBox>
                    <asp:TextBox ID="txtMoney" runat="server" CssClass="input small" Enabled="false"></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <%--<dl>
                <dt>业务日期</dt>
                <dd>
                    <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" onfocus="bindsWdatePicker()" Width="100px" datatype="*2-100" sucmsg=" "></asp:TextBox>-
                    <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" onfocus="bindeWdatePicker()" Width="100px" datatype="*2-100" sucmsg=" "></asp:TextBox>
                </dd>
            </dl>--%>
        </div>
        <!--/内容-->

        <!--工具栏-->
        <div class="page-footer">
            <div class="btn-wrap" style="<%=fromOrder=="true"?"text-align:center;":"" %>">
                <%--<asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btn" OnClick="btnSubmit_Click" />--%>
                <input type="hidden" name="finID" value="<%=id %>" />
                <input type="hidden" name="actionType" value="<%=action %>" />
                <input type="hidden" name="orderID" value="<%=oID %>" />
                <input type="hidden" name="finType" value="<%=type %>" />
                <input type="hidden" name="fromOrder" value="<%=fromOrder %>" />
                <input id="btnSave" runat="server" type="button" value="提交保存" class="btn" />
                <input id="btnReturn" type="button" value="返回上一页" class="btn yellow" style="<%=fromOrder=="true"?"display:none;":"" %>" onclick="javascript: history.back(-1);" />
            </div>
        </div>
        <!--/工具栏-->

    </form>
</body>
</html>
