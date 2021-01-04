<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="invoice_edit.aspx.cs" Inherits="MettingSys.Web.admin.finance.invoice_edit" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑发票</title>
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />

    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/Validform_v5.3.2_min.js"></script>
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
                    },
                    showNoSuggestionNotice: true,
                    noSuggestionNotice: '抱歉，没有匹配的选项',
                    groupBy: 'type'
                });
            });
            $("#txtCusName").change(function () {
                $("#hCusId").val("");
            });
            $("#txtmoney").blur(function () {
                var _money = $(this).val();
                if (_money != "" && _money != "0") {
                    if ($('#hCusId').val() == "" || $('#hCusId').val() == "0") {
                        layer.msg("请先选择客户");
                        $(this).val("");
                        $('#txtCusName').focus();
                        return false;
                    }
                    var postData = { cid: $('#hCusId').val(), money: _money ,oID:'<%=oID %>'};
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/business_ajax.ashx?action=computeInvMoney",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.status == 0) {
                                $("#labOverMoney").text(data.msg);
                            }
                        }
                    });
                }
            });
            $("#btnAudit").click(function () {
                layer.open({
                    type: 1,
                    title: '审批',
                    closeBtn: 0,
                    area: ['516px', '300px'],
                    //skin: 'layui-layer-nobg', //没有背景色
                    shadeClose: true,
                    content: $('#divflag')
                });
            });
        });
        function dealcheck() {
            if ($("#ddlflag").val() == "") {
                jsprint("请选择审批状态");
                return;
            }
            var postData = { "id": <%=id%>, "ctype": $("#ddlchecktype").val(), "cstatus": $("#ddlflag").val(), "remark": $("#txtCheckRemark").val() };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/Business_ajax.ashx?action=checkInvoice",
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
                            content: data.msg,
                            okValue: '确定',
                            ok: function () { }
                        }).showModal();
                    }
                }
            });
        }
    </script>
</head>

<body class="mainbody">
    <form id="form1" runat="server">
        <!--导航栏-->
        <div class="location" style="<%=fromOrder=="true"?"display:none;":"" %>">
            <a href="invoice_list.aspx" class="back"><i class="iconfont icon-up"></i><span>返回列表页</span></a>
            <a href="../center.aspx"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <a href="invoice_list.aspx"><span>发票列表</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span><%=action==DTEnums.ActionEnum.Add.ToString()?"添加":"编辑" %>发票</span>
        </div>
        <div class="line10"></div>
        <!--/导航栏-->

        <!--内容-->
        <div id="floatHead" class="content-tab-wrap" style="<%=fromOrder=="true"?"display:none;":"" %>">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a class="selected" href="javascript:;">发票信息</a></li>
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
                <dt>客户</dt>
                <dd>
                    <asp:TextBox ID="txtCusName" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <asp:HiddenField ID="hCusId" runat="server" />
                    <asp:Label ID="labCusName" runat="server"></asp:Label>
                </dd>
            </dl>
            <dl>
                <dt>购买方名称</dt>
                <dd>
                    <asp:TextBox ID="txtpurchaserName" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>购买方纳税人识别号</dt>
                <dd>
                    <asp:TextBox ID="txtpurchaserNum" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>购买方地址</dt>
                <dd>
                    <asp:TextBox ID="txtpurchaserAddress" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>购买方电话</dt>
                <dd>
                    <asp:TextBox ID="txtpurchaserPhone" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>购买方开户行</dt>
                <dd>
                    <asp:TextBox ID="txtpurchaserBank" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>购买方账号</dt>
                <dd>
                    <asp:TextBox ID="txtpurchaserBankNum" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>应税劳务、服务名称</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlserviceType" runat="server" OnSelectedIndexChanged="ddlserviceType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    </div>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlserviceName" runat="server">
                            <asp:ListItem Value="">请选择</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <asp:TextBox ID="txtserviceName" runat="server" CssClass="input normal"></asp:TextBox>
                </dd>
            </dl>
            <dl>
                <dt>发票类型</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlinvType" runat="server">
                            <asp:ListItem Value="">请选择</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </dd>
            </dl>
            <dl>
                <dt>开票金额</dt>
                <dd>
                    <asp:TextBox ID="txtmoney" runat="server" CssClass="input small" datatype="/^-?[1-9]+(\.\d+)?$|^-?0(\.\d+)?$|^-?[1-9]+[0-9]*(\.\d+)?$/" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                    <span>申请时超开
                    <asp:Label ID="labOverMoney" runat="server" ForeColor="Red">0</asp:Label></span>
                </dd>
            </dl>
            <dl>
                <dt>送票方式</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlsentWay" runat="server"></asp:DropDownList>
                    </div>
                </dd>
            </dl>
            <dl>
                <dt>开票区域</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddldarea" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddldarea_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    &nbsp;&nbsp;&nbsp;
                    开票单位
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlunit" runat="server"></asp:DropDownList>
                    </div>
                </dd>
            </dl>
            <dl>
                <dt>收票人名称</dt>
                <dd>
                    <asp:TextBox ID="txtreceiveName" runat="server" CssClass="input normal"></asp:TextBox>
                </dd>
            </dl>
            <dl>
                <dt>收票人电话</dt>
                <dd>
                    <asp:TextBox ID="txtreceivePhone" runat="server" CssClass="input normal"></asp:TextBox>
                </dd>
            </dl>
            <dl>
                <dt>收票人地址</dt>
                <dd>
                    <asp:TextBox ID="txtreceiveAddress" runat="server" CssClass="input normal"></asp:TextBox>
                </dd>
            </dl>
            <dl>
                <dt>备注</dt>
                <dd>
                    <asp:TextBox ID="txtremark" runat="server" CssClass="input normal" TextMode="MultiLine" />
                </dd>
            </dl>
        </div>
        <!--/内容-->

        <!--工具栏-->
        <div class="page-footer">
            <div class="btn-wrap" style="<%=fromOrder=="true"?"text-align:center;":"" %>">
                <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btn" OnClick="btnSubmit_Click" />
                <input type="button" id="btnAudit" runat="server" class="btn" value="审批" />
                <input name="btnReturn" type="button" value="返回上一页" class="btn yellow" style="<%=fromOrder=="true"?"display:none;":"" %>" onclick="javascript: history.back(-1);" />
            </div>
        </div>
        <!--/工具栏-->
        <div class="table-container" id="divflag" style="display: none;">
            <div class="tab-content" style="border: none;">                
                <dl>
                    <dt style="width: 100px;">审批类型</dt>
                    <dd>
                        <div class="rule-single-select">
                                    <asp:DropDownList ID="ddlchecktype" runat="server">
                                    <asp:ListItem Value="1">申请区域审批</asp:ListItem>
                                    <asp:ListItem Value="2">开票区域审批</asp:ListItem>
                                    <asp:ListItem Value="3">财务审批</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                    </dd>
                </dl>
                <dl>
                    <dt style="width: 100px;">审批状态</dt>
                    <dd>
                        <div class="rule-single-select">
                            <asp:DropDownList ID="ddlflag" runat="server">
                            </asp:DropDownList>
                        </div>
                    </dd>
                </dl>
                <dl>
                    <dt style="width: 100px;">备注</dt>
                    <dd>
                        <asp:TextBox ID="txtCheckRemark" runat="server" CssClass="input normal" TextMode="MultiLine" />
                    </dd>
                </dl>
                <dl>
                    <dt style="width: 100px;">&nbsp;</dt>
                    <dd>
                        <input type="button" value="提交" class="btn" onclick="dealcheck()" />
                    </dd>
                </dl>
            </div>
        </div>
    </form>
</body>
</html>
