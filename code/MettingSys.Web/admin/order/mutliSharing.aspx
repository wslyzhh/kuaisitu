<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="mutliSharing.aspx.cs" Inherits="MettingSys.Web.admin.order.mutliSharing" %>

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
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript">
        $(function () {
            
        })
        
        //计算表达式
        function computeResult(obj,type) {
            var _val = $(obj).val();
            if (_val != "") {
                $.getJSON("../../tools/business_ajax.ashx?action=computeResult&expression=" + encodeURIComponent(_val), function (json) {
                    if (json.status == 1) {
                        $(obj).parent().next().children().text(json.msg);
                        computeAll(type);
                    }
                    else {
                        $(obj).parent().next().children().text(0);
                        computeAll(type);
                    }
                });
            }
            else {
                $(obj).parent().next().children().text(0);   
                computeAll(type);
            }
        }
        function computeAll(type) {            
            $("#txtArea" + type).val("0");
            var _money = 0;
            $(".computeInput" + type).each(function () {
                if ($(this).text() != "0") {
                    $("#txtArea" + type).val($("#txtArea" + type).val() + "-" + $(this).text());
                    _money += parseFloat($(this).text());
                }
            });
            $("#txtArea" + type).parent().next().text(0-_money);
        }
        //提交保存
        function saveForm() {                   
            if ($(".computeTR").length < 1) {
                layer.msg("不能添加合作分成");
                return false;
            }
            var list = [];
            $("input[name=txtExpression1]").each(function (index, element) {
                var obj = $(this);
                if ($(obj).val() != "" && $(obj).val() != "0" && $(obj).siblings().html().indexOf("成功")<0) {
                    var postData = { "oID": "<%=oID%>", "area": $(obj).attr("data-area"), "type": true, "sdate":"<%=sdate%>", "edate":"<%=edate%>", "expression": $(obj).val() };
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/Business_ajax.ashx?action=AddShareFinance",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.status == 0) {
                                $(obj).siblings().html("<font color='green'>" + data.msg + "</font>");
                            } else {
                                $(obj).siblings().html("<font color='red'>" + data.msg + "</font>");
                            }
                        }
                    });
                }
            });
            $("input[name=txtExpression0]").each(function (index, element) {
                var obj = $(this);
                if ($(obj).val() != "" && $(obj).val() != "0" && $(obj).siblings().html().indexOf("成功")<0) {
                    var postData = { "oID": "<%=oID%>", "area": $(obj).attr("data-area"), "type": false, "sdate":"<%=sdate%>", "edate":"<%=edate%>", "expression": $(obj).val() };
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/Business_ajax.ashx?action=AddShareFinance",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.status == 0) {
                                $(obj).siblings().html("<font color='green'>" + data.msg + "</font>");
                            } else {
                                $(obj).siblings().html("<font color='red'>" + data.msg + "</font>");
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
        .searchbar span {
            font-weight:bolder;
            margin-left:20px;
        }
    </style>
</head>
<body class="mainbody">
    <form id="form1" runat="server">
        <div class="searchbar">
            <span>应收付对象：</span>合作分成
            <span>业务性质：</span>合作分成
            <span>业务明细：</span>内部分成
            <input type="button" class="btn" onclick="saveForm()" value="提交保存" />
        </div>
        <div class="table-container">
            <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                <tr style="text-align: center;">
                    <th width="10%">应收付区域归属</th>
                    <th>业务说明</th>
                    <th width="25%">应收表达式</th>
                    <th width="10%">金额</th>
                    <th width="25%">应付表达式</th>
                    <th width="10%">金额</th>
                </tr>
                <%=trHtml %>
            </table>
        </div>
    </form>
</body>
</html>
