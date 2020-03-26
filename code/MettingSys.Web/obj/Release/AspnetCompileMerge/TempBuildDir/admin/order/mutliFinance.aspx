<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="mutliFinance.aspx.cs" Inherits="MettingSys.Web.admin.order.mutliFinance" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link rel="stylesheet" type="text/css" href="../../scripts/layer/theme/default/layer.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
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

            bindNature();

            $("#ddlfStatus").change(function () {
                if ($(this).val() == "1") {
                    $("#txtCusName").val("<%=cusName%>");
                    $("#hCusId").val("<%=cid%>");
                }
                else {
                    $("#txtCusName").val("");
                    $("#hCusId").val("");
                }
            });

        })
        function bindNature() {
            $.getJSON("../../tools/business_ajax.ashx?action=getAllNature", function (json) {
                $('input[name=txtNature]').devbridgeAutocomplete({
                    lookup: json,
                    minChars: 0,
                    onSelect: function (suggestion) {
                        $(this).next().val(suggestion.id);
                        var _td = $(this).parent().siblings().children(".txt-item").html();
                        if (suggestion.type == false) {
                            $(this).parent().siblings().children(".txt-item").hide();
                            $(this).parent().siblings().children(".divDetail").show();

                            $.getJSON("../../tools/business_ajax.ashx?action=getAllDetail&naid=" + suggestion.id, function (json) {
                                $('input[name=txtDetail]').devbridgeAutocomplete({
                                    lookup: json,
                                    minChars: 0,
                                    onSelect: function (suggestion) {

                                    },
                                    showNoSuggestionNotice: true,
                                    noSuggestionNotice: '抱歉，没有匹配的选项'
                                });
                            });

                        }
                        else {
                            $(this).parent().siblings().children(".txt-item").show();
                            $(this).parent().siblings().children(".divDetail").hide();
                        }
                    },
                    showNoSuggestionNotice: true,
                    noSuggestionNotice: '抱歉，没有匹配的选项'
                });
            });
            $('input[name=txtNature]').blur(function () {
                if ($(this).val() == "") {
                    $(this).next().val("");
                }
            });
        }
        //人员选择
        function chooseEmployee(obj, flag, isShow) {
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
            d.n = $(obj).closest("tr").attr("data-num");//容器序号，一个页面上有多个容器时要传
            d.multi = flag;//true可以选择多个人,false只能选一个人
            d.showDstatus = isShow;//true显示接单状态，false不显示接单状态
        }
        //删除附件节点
        function delNode(obj) {
            $(obj).parent().remove();
        }
        //删除行
        function delLine(obj) {
            $(obj).parent().parent().remove();
        }
        //添加行
        function addRow() {
            var num = 0;
            if ($(".ltable tr").length == 1) {
                num = 0;
            }
            else {
                num = $(".ltable tr").last().attr("data-num");
            }
            var trhtml = "<tr data-num=\"" + (parseInt(num) + 1) + "\" style=\"text-align: center;\">"
                + "<td><input type=\"text\" name=\"txtNature\" style=\"width: 100px;\" class=\"input\" /><input type=\"hidden\" name=\"hNid\" /></td>"
                + "<td><div class=\"divDetail\" style=\"\"><input type=\"text\" name=\"txtDetail\" style=\"width: 100px;\" class=\"input\" /></div><div class=\"txt-item\" style=\"display: none;\"><ul><li class=\"icon-btn\"><a href=\"javascript:\" onclick=\"chooseEmployee(this,true,false)\"><i class=\"iconfont icon-close\"></i></a></li></ul></div></td>"
                + "<td><textarea name=\"txtIllustration\" class=\"input\" style=\"width:250px; height:55px;\"></textarea></td>"
                + "<td><input type=\"text\" name=\"txtExpression\" style=\"width: 140px;\" class=\"input\" onblur=\"computeResult(this)\" />=<span>0</span></td>"
                <%--+ "<td><input type=\"text\"  id=\"txtsdate\" name=\"txtsdate\" style=\"width: 100px;\" class=\"input rule-date-input\" onfocus=\"WdatePicker({maxDate: '<%=maxDate%>'})\" />-<input type=\"text\"  id=\"txtedate\" name=\"txtedate\" style=\"width: 100px;\" class=\"input rule-date-input\" onfocus=\"WdatePicker({maxDate: '<%=maxDate%>'})\" /></td>"--%>
                + "<td><img src=\"../../images/t03.png\" style=\"cursor:pointer;\" onclick=\"delLine(this)\" /></td>"
                + "<td></td>"
                + "</tr>";
            $(".ltable").append(trhtml);
            bindNature();
            $(".rule-date-input").ruleDateInput();
        }
        //计算表达式
        function computeResult(obj) {
            var _val = $(obj).val();
            if (_val != "") {
                $.getJSON("../../tools/business_ajax.ashx?action=computeResult&expression=" + encodeURIComponent(_val), function (json) {
                    if (json.status == 1) {
                        $(obj).siblings().text(json.msg);
                    }
                });
            }
        }
        //提交保存
        function saveForm() {
            //if ($("#ddlfStatus").val() == "") {
            //    layer.msg("请选择应收付类别");
            //    return false;
            //}
            //if ($("#hCusId").val() == "" || $("#hCusId").val() == "0") {
            //    layer.msg("请选择应收付对象");
            //    return false;
            //}
            if ($(".ltable tr").length <= 1) {
                layer.msg("至少要添加一行数据");
                return false;
            }
            var json = [];
            var flag = true;
            $(".ltable tr").each(function (index, element) {
                var num = $(element).attr("data-num");
                if (index > 0 && $(element).children().first().children().last().val() != "" && $(element).children().last().html().indexOf("成功")<0) {
                    var row = {};
                    $(element).children().each(function (n, obj) {
                        flag = true;
                        row.n = num;
                        if (n == 0) {//业务性质
                            row.nature = $(obj).children().last().val();
                        }
                        else if (n == 1) {//业务明细
                            row.detail = "";
                            if ($(obj).children().first().attr("style") == "") {
                                row.detail = $(obj).children().first().children().val();
                            }
                            else {
                                $(obj).find("input[name='hide_employee" + num + "']").each(function () {
                                    row.detail += $(this).val() + ",";
                                });
                                row.detail = row.detail.substring(0, row.detail.length - 1);
                            }
                            //if (row.detail == "") {
                            //    layer.msg("第" + index + "行，业务明细为空");
                            //    flag = false;
                            //    return false;
                            //}
                        }
                        else if (n == 2) {
                            row.illustration = $(obj).children().first().val();
                        }
                        else if (n == 3) {
                            row.expression = $(obj).children().first().val();
                            //if (row.expression == "") {
                            //    layer.msg("第" + index + "行，表达式为空");
                            //    flag = false;
                            //    return false;
                            //}
                        }
                        //else if (n == 4) {
                        //    row.sdate = $(obj).find("input[name='txtsdate']").val();
                        //    row.edate = $(obj).find("input[name='txtedate']").val();
                        //    //if (row.sdate == "") {
                        //    //    layer.msg("第" + index + "行，业务开始为空");
                        //    //    flag = false;
                        //    //    return false;
                        //    //}
                        //    //if (row.edate == "") {
                        //    //    layer.msg("第" + index + "行，业务结束为空");
                        //    //    flag = false;
                        //    //    return false;
                        //    //}
                        //}
                    });
                    var postData = { "oID": <%=oID%>, "json": JSON.stringify(row), "type": $("#ddlfStatus").val(), "cid": $("#hCusId").val() };
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/Business_ajax.ashx?action=mutilAddFinance",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            var json = eval(data);
                            console.log(json.status == 0);
                            if (json.status == 0) {
                                $(element).children().last().html("<font color='green'>" + json.msg + "</font>");
                            }
                            else {
                                console.log(json.msg);
                                $(element).children().last().html("<font color='red'>" + json.msg + "</font>");
                            }                            
                        }
                    });
                }
            });
        }
    </script>
    <style type="text/css">
        .date-input {
            width: 100px;
        }

        .myRuleSelect .select-tit {
            padding: 5px 5px 7px 5px;
        }
    </style>
</head>
<body class="mainbody">
    <form id="form1" runat="server">

        <div class="searchbar">
            应收付类别：<div class="rule-single-select">
                <asp:DropDownList ID="ddlfStatus" runat="server">
                    <asp:ListItem Value="">请选择</asp:ListItem>
                    <asp:ListItem Value="1">应收</asp:ListItem>
                    <asp:ListItem Value="0">应付</asp:ListItem>
                </asp:DropDownList>
            </div>
            应收付对象：<asp:TextBox ID="txtCusName" runat="server" CssClass="input normal"></asp:TextBox>
            <asp:HiddenField ID="hCusId" runat="server" />
            <input type="button" class="btn" onclick="addRow()" value="添加行" />
            <input type="button" class="btn" onclick="saveForm()" value="提交保存" />
        </div>
        <div class="table-container">
            <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                <tr style="text-align: center;">
                    <th width="15%">业务性质</th>
                    <th width="15%">业务明细</th>
                    <th>业务说明</th>
                    <th width="20%">表达式</th>
                    <%--<th width="22%">业务日期</th>--%>
                    <th width="6%">操作</th>
                    <th width="6%">状态</th>
                </tr>
                <tr data-num="1" style="text-align: center;">
                    <td>
                        <input type="text" name="txtNature" style="width: 100px;" class="input" />
                        <input type="hidden" name="hNid" />
                    </td>
                    <td>
                        <div class="divDetail" style="">
                            <input type="text" name="txtDetail" style="width: 100px;" class="input" />
                        </div>
                        <div class="txt-item" style="display: none;">
                            <ul>
                                <li class="icon-btn">
                                    <a href="javascript:" onclick="chooseEmployee(this,true,false)"><i class="iconfont icon-close"></i></a>
                                </li>
                            </ul>
                        </div>
                    </td>
                    <td>
                        <textarea name="txtIllustration" class="input" style="width:250px; height:55px;"></textarea>
                    </td>
                    <td>
                        <input type="text" name="txtExpression" style="width: 140px;"  class="input" onblur="computeResult(this)" />=<span>0</span>
                    </td>
                   <%-- <td>
                        <input type="text" id="txtsdate" name="txtsdate" style="width: 100px;" class="input rule-date-input" onfocus="WdatePicker({maxDate: '<%=maxDate%>'})" />
                        -
                        <input type="text" id="txtedate" name="txtedate" style="width: 100px;" class="input rule-date-input" onfocus="WdatePicker({minDate: '#F{$dp.$D(\'txtsdate\')}',maxDate: '<%=maxDate%>'})" />
                    </td>--%>
                    <td>
                        <img src="../../images/t03.png" style="cursor: pointer;" onclick="delLine(this)" />
                    </td>
                    <td></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
