<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="customer_merge.aspx.cs" Inherits="MettingSys.Web.admin.customer.customer_merge" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑业务明细</title>
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link type="text/css" rel="stylesheet" href="../skin/default/style.css" />
    <link href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript">
        $(function () {
            //发送AJAX请求
            $.getJSON("../../tools/business_ajax.ashx?action=getAllCustomer", function (json) {
                $('#txtCusName1').devbridgeAutocomplete({
                    lookup: json,
                    minChars: 1,
                    onSelect: function (suggestion) {
                        $('#hCusId1').val(suggestion.id);
                    },
                    showNoSuggestionNotice: true,
                    noSuggestionNotice: '抱歉，没有匹配的选项',
                    groupBy: 'type'
                });
                $("#txtCusName1").blur(function () {
                    if ($(this).val() == "") {
                        $("#hCusId1").val("");
                    }
                });
                $('#txtCusName2').devbridgeAutocomplete({
                    lookup: json,
                    minChars: 1,
                    onSelect: function (suggestion) {
                        $('#hCusId2').val(suggestion.id);
                    },
                    showNoSuggestionNotice: true,
                    noSuggestionNotice: '抱歉，没有匹配的选项',
                    groupBy: 'type'
                });
                $("#txtCusName2").blur(function () {
                    if ($(this).val() == "") {
                        $("#hCusId2").val("");
                    }
                });
            });
        });
        function confirmSubmit() {
            var v1 = $("#hCusId1").val();
            var v2 = $("#hCusId2").val();            
            if (v1 == "") {
                jsdialog('提示', '请填写源客户', '');
                return false;
            }
            if (v2 == "") {
                jsdialog('提示', '请填写目标客户', '');
                return false;
            }
            if (v1 == v2) {
                jsdialog('提示', '源客户和目标客户不能是同一个客户', '');
                return false;
            }
            var c1 = $("#txtCusName1").val();
            var c2 = $("#txtCusName2").val();
            var flag = false;
            layer.confirm("将客户【" + c1 + "】合并至客户【" + c2 + "】，是否继续？", {
                btn: ['确定', '取消'] //按钮
            }, function (index) {
                layer.close(index);
                var _index = layer.load(0, {shade: [0.7, '#fff']});
                var postData = { "cid1": v1, "cid2": v2, "cname1": c1, "cname2": c2 };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=mergeCustomer",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        layer.close(_index);
                        if (data.status == 0) {
                            jsprint(data.msg, "");
                        } else {
                            jsdialog('提示', data.msg, '');
                        }
                    }
                });
            }, function () { }
            );
        }
    </script>
    <style type="text/css">
        .tab-content dl dt {
            width: 60px;
        }

        .tab-content dl dd {
            margin-left: 80px;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <div class="tab-content" style="border: none;">
            <dl>
                <dt>源客户</dt>
                <dd>
                    <asp:TextBox runat="server" ID="txtCusName1" CssClass="input normal" ></asp:TextBox>
                    <asp:HiddenField ID="hCusId1" runat="server" />
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>目标客户</dt>
                <dd>
                    <asp:TextBox runat="server" ID="txtCusName2" CssClass="input normal" ></asp:TextBox>
                    <asp:HiddenField ID="hCusId2" runat="server" />
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl style="text-align: center;">
                <input type="button" class="btn" onclick="confirmSubmit()" value="提交" />
            </dl>
        </div>
        <div class="dRemark">
            <p>注：是把源客户合并至目标客户，客户合并后不可恢复，请注意</p>
        </div>
        <!--/内容-->
    </form>
</body>
</html>
