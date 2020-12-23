<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="receiptdetail_edit.aspx.cs" Inherits="MettingSys.Web.admin.finance.receiptdetail_edit" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title><%=action==DTEnums.ActionEnum.Add.ToString()?"添加":"编辑" %>收款明细</title>
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery.form.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/Validform_v5.3.2_min.js"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
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
                        showBank(suggestion.id);
                    },
                    showNoSuggestionNotice: true,
                    noSuggestionNotice: '抱歉，没有匹配的选项',
                    groupBy: 'type'
                });
            });
            $("#txtCusName").change(function () {
                $("#hCusId").val("");
            });
            if (<%=cid %> > 0) {
                showBank(<%=cid %>);
            }
            $("#txtMoney").change(function () {
                if ($(this).val() != "") {
                    if ($(this).val() < 0) {
                        $("#dlBank").show();
                    }
                    else {
                        $("#dlBank").hide();
                    }
                }
            });

            $("#ddlmethod").change(function () {
                var ptype = $(this).find('option:selected').attr("py");
                if (ptype == "True" || $("#txtMoney").val() > 0) {
                    $("#dlBank").hide();
                }
                else {
                    if ($("#txtMoney").val() > 0) {
                        $("#dlBank").hide();
                    } else {
                        $("#dlBank").show();
                    }
                }
            });

            $("#txtBank").change(function () {
                $('#hBankId').val("");
                var cid = parseInt($('#hCusId').val());
                if (cid > 0) {
                    showBank(cid);
                }
            });
            if ("<%=action%>" == "<%=DTEnums.ActionEnum.Edit.ToString() %>") {
                if ("<%=isFushu%>" == "True") {
                    $("#dlBank").show();
                } else {
                    $("#dlBank").hide();
                }

                if ("<%=isChongzhang%>" == "True") {
                    $("#dlBank").hide();
                } else {
                    $("#dlBank").show();
                }
            }

            var ajaxFormOption = {
                dataType: "json", //数据类型  
                success: function (data) { //提交成功的回调函数  
                    if (data.status == 0) {
                        var d = top.dialog({ content: data.msg }).show();
                        setTimeout(function () {
                            d.close().remove();
                            if (data.fromOrder == true) {
                                parent.location.reload();
                            } else {
                                location.href = 'receiptdetail_list.aspx';
                            }
                        }, 1500);
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
                $("#form1").ajaxSubmit(ajaxFormOption);
            });
        });
        function showBank(cid) {
            var postData = { "cid": cid, "field": "1" };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/Business_ajax.ashx?action=getCusBank",
                data: postData,
                dataType: "json",
                success: function (json) {
                    $('#txtBank').devbridgeAutocomplete({
                        lookup: json,
                        minChars: 0,
                        width: '500px',
                        onSelect: function (suggestion) {
                            $(this).next().val(suggestion.id);
                        },
                        showNoSuggestionNotice: true,
                        noSuggestionNotice: '抱歉，没有匹配的选项'
                    });
                }
            });
        }
        function addBank() {
            var cid = parseInt($('#hCusId').val());
            if (cid > 0) {
                layer.open({
                    type: 2,
                    title: '添加银行账号',
                    shadeClose: true,
                    shade: false,
                    maxmin: false, //开启最大化最小化按钮
                    area: ['600px', '400px'],
                    content: '../customer/bank_edit.aspx?action=Add&fromPay=true&cid=' + cid + '&tag=1',
                    end: function () {
                        //location.reload();
                    }
                });
            }
            else {
                jsprint("请选择收款对象");
            }
        }
    </script>
</head>

<body class="mainbody">
    <form id="form1" runat="server" action="../../tools/business_ajax.ashx?action=AddReceiptDetails" method="post" enctype="multipart/form-data">
        <!--导航栏-->
        <div class="location"  style="<%=fromOrder=="true"?"display:none;":"" %>">
            <a href="receiptdetail_list.aspx" class="back"><i class="iconfont icon-up"></i><span>返回列表页</span></a>
            <a href="../center.aspx"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <a href="receiptdetail_list.aspx"><span>收款明细列表</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span><%=action==DTEnums.ActionEnum.Add.ToString()?"添加":"编辑" %>收款明细</span>
        </div>
        <div class="line10"></div>
        <!--/导航栏-->

        <!--内容-->
        <div id="floatHead" class="content-tab-wrap" style="<%=fromOrder=="true"?"display:none;":"" %>">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a class="selected" href="javascript:;">收款明细</a></li>
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
                <dt>收款对象</dt>
                <dd>
                    <asp:TextBox ID="txtCusName" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <asp:HiddenField ID="hCusId" runat="server" />
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>收款金额</dt>
                <dd>
                    <asp:TextBox ID="txtMoney" runat="server" CssClass="input small"  datatype="/^-?[1-9]+(\.\d+)?$|^-?0(\.\d+)?$|^-?[1-9]+[0-9]*(\.\d+)?$/" sucmsg=" "/>
                    <span class="Validform_checktip">*请输入有效金额</span>
                </dd>
            </dl>
            <dl>
                <dt>预收日期</dt>
                <dd>
                    <asp:TextBox ID="txtforedate" runat="server" CssClass="input rule-date-input" AutoCompleteType="None" onfocus="WdatePicker({ dateFmt: 'yyyy-MM-dd'});" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>      
            <dl>
                <dt>收款方式</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlmethod" runat="server" OnDataBound="ddlmethod_DataBound"></asp:DropDownList>
                    </div>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>     
            <dl id="dlBank" style="display:none;">
                <dt>客户银行账号</dt>
                <dd>
                    <asp:TextBox ID="txtBank" runat="server" CssClass="input normal" Width="380px"></asp:TextBox>
                    <asp:HiddenField ID="hBankId" runat="server" />
                    <a href="javascript:void(0);" onclick="addBank()">新增银行账号</a>
                </dd>
            </dl>
            <dl>
                <dt>收款内容</dt>
                <dd>
                    <asp:TextBox ID="txtContent" runat="server" CssClass="input normal" Width="400px" Height="110px" TextMode="MultiLine" />
                </dd>
            </dl>
        </div>
        <!--/内容-->

        <!--工具栏-->
        <div class="page-footer">
            <div class="btn-wrap" style="<%=fromOrder=="true"?"text-align:center;":"" %>">
                <input type="hidden" id="rpdID" name="rpdID" value="<%=id %>"/>
                <input type="hidden" id="oID" name="oID" value="<%=oID %>"/>
                <input type="hidden" id="fromOrder" name="fromOrder" value="<%=fromOrder %>"/>
                <input id="btnSave" runat="server" type="button" value="提交保存" class="btn" />
                <input id="btnReturn" type="button" value="返回上一页" class="btn yellow" style="<%=fromOrder=="true"?"display:none;":"" %>" onclick="javascript: history.back(-1);" />
            </div>
        </div>
        <!--/工具栏-->

    </form>
</body>
</html>